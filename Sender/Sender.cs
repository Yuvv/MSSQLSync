using System;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sender {
  public partial class Sender: Form {
    private TcpClient client;
    private NetworkStream stream2Server;
    private StreamReader reader;
    private StreamWriter writer;

    private SqlConnection dbConn;
    private SqlCommand dbCmd;
    private DataTable localTable;

    private List<TableItem> tableItems;
    private bool dbConnected = false;
    private bool tcpConnected = false;

    private Thread waitThread;

    public Sender() {
      InitializeComponent();
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

    /// <summary>
    /// get new data records and fill them in localTable.
    /// </summary>
    /// <param name="item">TableItem</param>
    /// <param name="maxSize">max record number</param>
    /// <returns>if record count>0, return true, else false.</returns>
    private bool getRecords(TableItem item, int maxSize = 50) {
      var currentID = item.lastID;
      dbCmd.CommandText = string.Format(
        "use [{0}];select ident_current('[{1}]');",
        item.database, item.name);
      object obj = dbCmd.ExecuteScalar();
      if (obj != null) {
        currentID = int.Parse(obj.ToString());	// get last max id
      }

      if (item.lastID < currentID) {
        log("Detect changes in table " + item.name);
        // 主动获取表结构
        localTable = new DataTable(item.name);
        dbCmd.CommandText = string.Format("select top {0} * from {1} where {2}>{3}",
          maxSize, item.name, item.identColumn, item.lastID);
        SqlDataAdapter adapter = new SqlDataAdapter(dbCmd);
        adapter.Fill(localTable);
        adapter.Dispose();

        var rows = localTable.Rows.Count;
        log("Get " + rows + " records.");

        // 更新lastID
        if (rows == 0) {
          item.lastID = currentID;
          return false;
        }

        return true;
      } else {
        if (item.backoffCycle > 16) {
          item.backoffTime = 0;
          item.backoffCycle = 2;
        } else {
          item.backoffTime = item.backoffCycle;
          item.backoffCycle *= 2;
        }
        return false;
      }
    }

    // 启动连接
    private void btnLink_Click(object sender, EventArgs e) {
      try {
        if (!dbConnected) {
          dbConn = new SqlConnection(getConnStr());
          dbConn.Open();
          dbConnected = true;
          dbCmd = dbConn.CreateCommand();
          dbCmd.CommandType = CommandType.Text;
          log("Connect to database succeed!");
        }
        // init tcp connection
        string addr = tcpServerIP.Text;
        int port = (int)tcpServerPort.Value;
        client = new TcpClient(addr, port);
        stream2Server = client.GetStream();
        reader = new StreamReader(stream2Server);
        writer = new StreamWriter(stream2Server);
        writer.AutoFlush = true;
        tcpConnected = true;
        log("Connect to tcp server succeed!");

        btnLink.Enabled = false;
        btnExit.Enabled = true;

        // 新开wait线程防止UI线程阻塞
        waitThread = new Thread(startLink);
        waitThread.IsBackground = true;
        waitThread.Start();
      } catch (SocketException ex) {
        MessageBox.Show("连接到tcp server失败！\n" + ex.Message);
        log(ex.Message);
      } catch (SqlException ex) {
        MessageBox.Show("连接到数据库失败！\n" + ex.Message);
        log(ex.Message);
      } catch (Exception ex) {
        log(ex.Message);
      }
    }

    // 关闭连接
    private void btnExit_Click(object sender, EventArgs e) {
      log("Now stop synchrony...");
      if (dbConnected) {
        dbConn.Close();
        dbConnected = false;
      }
      if (tcpConnected) {
        if (waitThread.ThreadState != ThreadState.Unstarted) {
          waitThread.Abort();
        }
        try {
          this.writer.WriteLine("88");		// 主动say 88
        } catch (Exception ex) {
          log("[ERROR] " + ex.Message);
        }
        timer.Stop();
        if (client.Client.Connected) {
          client.Close();
        }
        tcpConnected = false;
      }
      btnLink.Enabled = true;
      btnExit.Enabled = false;

      // write lastIDs back
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      JArray tables = new JArray();
      foreach (var item in tableItems) {
        tables.Add(new JObject(
          new JProperty("database", item.database),
          new JProperty("name", item.name),
          new JProperty("ident_col", item.identColumn),
          new JProperty("last_id", item.lastID),
          new JProperty("origin_table", item.origin_table)));
      }
      setting["Tables"] = tables;

      StreamWriter writer = new StreamWriter("setting.json");
      writer.Write(setting.ToString());
      writer.Close();
    }

    // 给其它线程执行exit函数的委托
    private delegate void clientDelegate();
    private void stopLink() {
      if (logInfoBox.InvokeRequired) {
        clientDelegate timerD = new clientDelegate(stopLink);
        BeginInvoke(timerD);
        return;
      }
      btnExit_Click(this, EventArgs.Empty);
      log("Now auto restart...");
      btnLink_Click(this, EventArgs.Empty);
    }

    // 等待recver临时线程
    private void startLink() {
      try {
        log("Waiting for start signal...");
        writer.WriteLine("sender");
        string signal = reader.ReadLine();
        if (string.IsNullOrEmpty(signal) || signal != "OK") {
          log("Link failed! Please restart.");
          stopLink();
        } else {
          // 启动计时器
          log("Now starting...");
          startTimer();
          client.ReceiveTimeout = 30000;		// 超时时间30s
        }
      } catch (ThreadAbortException) {
        log("Stop Waiting.");
      } catch (Exception ex) {
        log("[ERROR] " + ex.Message);
        stopLink();
      }
    }

    // 给startLink线程启动计时器的委托
    private delegate void timerDelegate();
    private void startTimer() {
      if (logInfoBox.InvokeRequired) {
        timerDelegate timerD = new timerDelegate(startTimer);
        BeginInvoke(timerD);
        return;
      }
      timer.Interval = (int)syncCycle.Value * 1000;
      timer.Start();
    }

    // timer回调
    private void timer_Tick(object sender, EventArgs e) {
      DataSet ds = new DataSet();
      string respStr = string.Empty;
      try {
        foreach (var item in tableItems) {
          if (item.backoffTime > 0) {
            log(item.name + " doesn't change. backoff time changed!");
            item.backoffTime -= 1;
            continue;
          }
          if (getRecords(item)) {
            var maxid = localTable.Rows[localTable.Rows.Count - 1][item.identColumn].ToString();
            log(string.Format("Local max ID in table {0} is {1}.", item.name, maxid));
            if (item.tgrTable) {
              localTable.Columns.Remove(item.identColumn);
            }
            ds.Tables.Add(localTable);
            ds.Tables.Add(new DataTable(item.database + "~" + maxid));
            var json = JsonConvert.SerializeObject(ds, Formatting.None);

            log("Now sending data to recver...");
            writer.WriteLine(json);
            localTable.Dispose();

            respStr = reader.ReadLine();
            if (!string.IsNullOrEmpty(respStr)) {
              // recver关闭连接
              if (respStr == "88") {
                ds.Dispose();
                log("Recver closed!");
                stopLink();	// auto restart
                break;
              }
              log("response string is " + respStr);

              string[] resp = respStr.Split(':');
              if (resp.Length > 1 && resp[0] == item.name) {
                if (item.lastID < int.Parse(resp[1])) {
                  item.lastID = int.Parse(resp[1]);
                }
              }
            }
          }
          ds.Tables.Clear();
          ds.Dispose();
        }
      } catch (Exception ex) {
        log("[ERROR] " + ex.Message);
        stopLink();
      }
    }

    public void reloadConfig() {
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      tableItems.Clear();
      foreach (var table in (JArray)setting["Tables"]) {
        tableItems.Add(new TableItem(
          table["database"].ToString(),
          table["name"].ToString(),
          table["ident_col"].ToString(),
          int.Parse(table["last_id"].ToString()),
          table["origin_table"].ToString(),
          (table["name"].ToString() == table["origin_table"].ToString()) ? false : true));
      }
      log("Reload configuration done!");
    }

    // 启动时加载配置
    private void Client_Load(object sender, EventArgs e) {
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();
      log("Load setting OK!");

      string[] ip_port = setting["Connection"]["Server"].ToString().Split(',');
      dbIP.Text = ip_port[0];
      dbPort.Value = int.Parse(ip_port[1]);
      userName.Text = setting["Connection"]["UID"].ToString();
      password.Text = setting["Connection"]["PWD"].ToString();
      tcpServerIP.Text = setting["Connection"]["TCP_IP"].ToString();
      tcpServerPort.Value = int.Parse(setting["Connection"]["TCP_Port"].ToString());
      syncCycle.Value = int.Parse(setting["Connection"]["Sync_Cycle"].ToString());
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

      // init last syncID and back-off time
      tableItems = new List<TableItem>();
      foreach (var table in (JArray)setting["Tables"]) {
        tableItems.Add(new TableItem(
          table["database"].ToString(),
          table["name"].ToString(),
          table["ident_col"].ToString(),
          int.Parse(table["last_id"].ToString()),
          table["origin_table"].ToString(),
          (table["name"].ToString() == table["origin_table"].ToString()) ? false : true));
      }

      log("Load table items done!");
    }

    private void modeSql_CheckedChanged(object sender, EventArgs e) {
      if (modeSql.Checked) {
        dbGroupBox.Enabled = true;
      } else {
        dbGroupBox.Enabled = false;
      }
    }

    private void btnSaveConfig_Click(object sender, EventArgs e) {
      StreamReader reader = new StreamReader("setting.json");
      JObject setting = JObject.Parse(reader.ReadToEnd());
      reader.Close();

      setting["Connection"]["AuthType"] = (modeWin.Checked) ? 0 : 1;

      setting["Connection"]["Server"] = dbIP.Text + "," + dbPort.Value;
      setting["Connection"]["UID"] = userName.Text;
      setting["Connection"]["PWD"] = password.Text;

      setting["Connection"]["TPC_IP"] = tcpServerIP.Text;
      setting["Connection"]["TPC_Port"] = tcpServerPort.Value;

      setting["Connection"]["Sync_Cycle"] = syncCycle.Value;

      StreamWriter writer = new StreamWriter("settion.json");
      writer.Write(setting.ToString());
      writer.Close();

      log("Configure write done!");
    }

    private void Client_Resize(object sender, EventArgs e) {
      if (WindowState == FormWindowState.Minimized) {
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Hide();
        myNotify.Visible = true;
        myNotify.ShowBalloonTip(1000);
      }
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

    private void showWinToolStripMenuItem_Click(object sender, EventArgs e) {
      myNotify_DoubleClick(sender, e);
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    // 关闭连接并退出
    private void Client_FormClosing(object sender, FormClosingEventArgs e) {
      if (tcpConnected) {
        MessageBox.Show(
          "与服务器的连接尚未关闭，请先关闭再退出！",
          "别乱关", MessageBoxButtons.OK);
        e.Cancel = true;
        return;
      }

      // 备份日志
      if (logInfoBox.Lines.Length > 0) {
        StreamWriter log = new StreamWriter("Sender.log", true);
        log.Write(logInfoBox.Text);
        log.Close();
      }
    }

    // 每10分钟检测日志长度是否超过5000行，超过则备份日志
    private void timerForLog_Tick(object sender, EventArgs e) {
      if (logInfoBox.Lines.Length > 5000) {
        StreamWriter log = new StreamWriter("Sender.log", true);
        log.Write(logInfoBox.Text);
        log.WriteLine("\n\n");
        log.Close();
        logInfoBox.Text = "";
      }
      FileInfo fInfo = new FileInfo("Sender.log");
      if (fInfo.Length > 10 * 1024 * 1024) {
        // 大于10MB进行归档
        fInfo.MoveTo(string.Format("Sender-{0}.log",
          DateTime.Now.ToString("yyMMdd-HHmmss")));
      }
    }

    private void btnSettingHelper_Click(object sender, EventArgs e) {
      SettingHelper.SettingHelper setting = new SettingHelper.SettingHelper();
      setting.ShowDialog();
      reloadConfig();
    }
  }
}
