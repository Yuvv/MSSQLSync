using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Recver {
  public partial class Recver: Form {
    private TcpClient client;
    private Thread tcpThread;

    private SqlConnection dbConn;
    private SqlCommand dbCmd;

    private Dictionary<string, int> lastIDs;

    private bool dbConnected = false;
    private bool tcpConnected = false;

    public Recver() {
      InitializeComponent();
    }

    // 给其它线程操作日志的委托
    private delegate void logDelegate(string logStr);
    private void log(string logStr) {
      if (logInfoBox.InvokeRequired) {
        logDelegate logD = new logDelegate(log);
        BeginInvoke(logD, new object[] { logStr });
        return;
      }
      logInfoBox.AppendText(string.Format(
        "[{0}] {1}", DateTime.Now.ToLongTimeString(), logStr));
      logInfoBox.AppendText(Environment.NewLine);
      logInfoBox.ScrollToCaret();
    }

    private string getConnStr() {
      // 参数详见 https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring%28v=vs.110%29.aspx

      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

      builder.Add("Server", dbIP.Text + "," + dbPort.Value.ToString());
      if (modeWin.Checked) {
        builder.Add("Integrated Security", "SSPI");
      } else {
        builder.Add("UID", userName.Text);
        builder.Add("PWD", password.Text);
        builder.Add("Network Library", "DBMSSOCN");
      }

      return builder.ConnectionString;
    }

    // clientCallback作为后台线程，防止UI线程阻塞
    private void clientCallback() {
      NetworkStream stream2Client = client.GetStream();
      StreamReader reader = new StreamReader(stream2Client);
      StreamWriter writer = new StreamWriter(stream2Client);
      writer.AutoFlush = true;

      writer.WriteLine("recver");
      log("Waiting for sender to transport data...");
      string recvStr = string.Empty;
      try {
        recvStr = reader.ReadLine();
        if (string.IsNullOrEmpty(recvStr) || recvStr != "OK") {
          log("Something seems wrong! Please retry.");
          stopLink();
          return;
        }
        stream2Client.ReadTimeout = 30000;	// 30s超时时间
        while (true) {
          recvStr = reader.ReadLine();
          if (!string.IsNullOrEmpty(recvStr)) {
            if (recvStr == "88") {
              log("Client closed the connection!");
              break;
            }
            try {
              log(string.Format("Received {0} byte(s) data.", recvStr.Length));
              DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(recvStr);
              DataTable table = dataSet.Tables[0];

              var rowNum = table.Rows.Count;
              log(string.Format("Received {0} row(s) in table {1}.",
                rowNum, table.TableName));
              var database = dataSet.Tables[1].TableName.Split('~')[0];
              var maxID = int.Parse(dataSet.Tables[1].TableName.Split('~')[1]);
              log("Last max ID is " + maxID);

              // 防止因为网络原因sender未收到回复而重发数据
              if (!lastIDs.ContainsKey(database + ".." + table.TableName) ||
                  maxID > lastIDs[database + ".." + table.TableName]) {
                dbCmd.CommandText = string.Format("use [{0}];", database);
                dbCmd.ExecuteNonQuery();
                dbCmd.CommandText = string.Format("select top 1 * from [{0}]", table.TableName);
                SqlDataAdapter adapter = new SqlDataAdapter(dbCmd);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(table);
                cmdBuilder.RefreshSchema();
                cmdBuilder.Dispose();
                adapter.Dispose();
                lastIDs[database + ".." + table.TableName] = maxID;
              } else {
                maxID = lastIDs[database + ".." + table.TableName];
              }

              log("Update to database done. Now sending ACK to client...");
              writer.WriteLine(dataSet.Tables[0].TableName + ":" + maxID);
              // 释放资源
              dataSet.Dispose();
            } catch (Exception ex) {
              log("[ERROR] " + ex.Message);
              writer.WriteLine("ERROR:0");
            }
          } else {
            break;
          }
        }
        stopLink();
      } catch (ThreadAbortException) {
        log("Thread abort!");
        writer.WriteLine("88");
        log("Now dispose all resource in this tcp connection.");
        reader.Close();
        writer.Close();
        stream2Client.Close();
        client.Close();
      } catch (Exception ex) {
        log("[ERROR] " + ex.Message);
        client.Close();
        stopLink();
      }
    }

    // 给clientCallback线程执行exit函数的委托
    private delegate void clientDelegate();
    private void stopLink() {
      if (logInfoBox.InvokeRequired) {
        clientDelegate timerD = new clientDelegate(stopLink);
        BeginInvoke(timerD);
        return;
      }
      btnExit_Click(this, EventArgs.Empty);
      log("Now auto restart...");
      btnStart_Click(this, EventArgs.Empty);
    }

    private void myNotify_DoubleClick(object sender, EventArgs e) {
      if (ShowInTaskbar == false) {
        ShowInTaskbar = true;
        myNotify.Visible = false;
        Show();
        Activate();
        WindowState = FormWindowState.Normal;
      }
    }

    private void Server_Resize(object sender, EventArgs e) {
      if (WindowState == FormWindowState.Minimized) {
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Hide();
        myNotify.Visible = true;
        myNotify.ShowBalloonTip(1000);
      }
    }

    private void showWinToolStripMenuItem_Click(object sender, EventArgs e) {
      myNotify_DoubleClick(sender, e);
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void btnSaveConfig_Click(object sender, EventArgs e) {
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      setting["Connection"]["AuthType"] = modeWin.Checked ? 0 : 1;
      setting["Connection"]["Server"] = dbIP.Text + "," + dbPort.Value;
      setting["Connection"]["UID"] = userName.Text;
      setting["Connection"]["PWD"] = password.Text;

      setting["Connection"]["TCP_IP"] = tcpServerIP.Text;
      setting["Connection"]["TCP_Port"] = tcpServerPort.Value;

      StreamWriter writer = new StreamWriter("setting.json");
      writer.Write(setting.ToString());
      writer.Close();

      log("Configure write done!");
    }

    private void btnStart_Click(object sender, EventArgs e) {
      try {
        if (dbConnected == false) {
          // init mssql connection
          dbConn = new SqlConnection(getConnStr());
          dbConn.Open();
          dbCmd = dbConn.CreateCommand();
          dbCmd.CommandType = CommandType.Text;
          dbConnected = true;
          log("Connect to database succeed!");
        }

        // init tcp link
        var lIP = tcpServerIP.Text;
        var lPort = (int)tcpServerPort.Value;
        client = new TcpClient(lIP, lPort);
        tcpConnected = true;

        btnStart.Enabled = false;
        btnExit.Enabled = true;
        log("Connect to tcp server succeed!");

        // 新线程开启
        tcpThread = new Thread(clientCallback);
        tcpThread.IsBackground = true;
        tcpThread.Start();
      } catch (SqlException ex) {
        log("Link to database failed!");
        log(ex.Message);
      } catch (SocketException ex) {
        log("Open tcp listener failed!");
        log(ex.Message);
      } catch (Exception ex) {
        log(ex.Message);
      }
    }

    // 停止tcp线程，关闭数据库和tcp listener，
    private void btnExit_Click(object sender, EventArgs e) {
      if (tcpConnected) {
        if (dbConnected) {
          dbConn.Close();
          dbConnected = false;
        }

        if (tcpThread.ThreadState != ThreadState.Unstarted) {
          tcpThread.Abort();
          log("TCP server thread closed!");
        }
        tcpConnected = false;

        log("Link closed!");
        log("Now you can link to database and tcp server again.");
      }

      btnStart.Enabled = true;
      btnExit.Enabled = false;

      // write last id back
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      JObject tables = new JObject();
      foreach (var item in lastIDs) {
        tables.Add(new JProperty(item.Key, item.Value));
      }
      setting["Tables"] = tables;

      StreamWriter writer = new StreamWriter("setting.json");
      writer.Write(setting.ToString());
      writer.Close();
    }

    private void modeSql_CheckedChanged(object sender, EventArgs e) {
      if (modeSql.Checked) {
        dbGroupBox.Enabled = true;
      } else {
        dbGroupBox.Enabled = false;
      }
    }

    private void Server_Load(object sender, EventArgs e) {
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      string[] ip_port = setting["Connection"]["Server"].ToString().Split(',');
      dbIP.Text = ip_port[0];
      dbPort.Value = int.Parse(ip_port[1]);
      userName.Text = setting["Connection"]["UID"].ToString();
      password.Text = setting["Connection"]["PWD"].ToString();
      tcpServerIP.Text = setting["Connection"]["TCP_IP"].ToString();
      tcpServerPort.Value = int.Parse(setting["Connection"]["TCP_Port"].ToString());

      var mode = int.Parse(setting["Connection"]["AuthType"].ToString());
      if (0 == mode) {
        modeSql.Checked = false;
        modeWin.Checked = true;
        dbGroupBox.Enabled = false;
      } else {
        modeSql.Checked = true;
        modeWin.Checked = false;
      }

      log("Load configuration done!");

      lastIDs = setting["Tables"].ToObject<Dictionary<string, int>>();
      log("Load Last id done!");
    }

    private void Server_FormClosing(object sender, FormClosingEventArgs e) {
      if (tcpConnected) {
        MessageBox.Show(
          "与服务器的连接尚未关闭，请先关闭再退出！",
          "别乱关", MessageBoxButtons.OK);

        e.Cancel = true;
        return;
      }

      // 备份日志
      if (logInfoBox.Lines.Length > 0) {
        StreamWriter log = new StreamWriter("Recver.log", true);
        log.Write(logInfoBox.Text);
        log.Close();
      }
    }

    // 每10分钟检测日志长度是否超过5000行，超过则备份日志
    private void timerForLog_Tick(object sender, EventArgs e) {
      if (logInfoBox.Lines.Length > 5000) {
        StreamWriter log = new StreamWriter("Recver.log", true);
        log.Write(logInfoBox.Text);
        log.WriteLine("\n\n");
        log.Close();
        logInfoBox.Text = "";
      }
      FileInfo fInfo = new FileInfo("Recver.log");
      if (fInfo.Length > 10 * 1024 * 1024) {
        // 大于10MB进行归档
        fInfo.MoveTo(string.Format("Recver-{0}.log",
          DateTime.Now.ToString("yyMMdd-HHmmss")));
      }
    }
  }
}
