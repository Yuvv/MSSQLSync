using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using log4net;

namespace TServer {
  class TServer {
    public static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private TcpListener listener;
    private TcpClient sender;
    private TcpClient recver;

    private StreamWriter senderWriter, recverWriter;
    private StreamReader senderReader, recverReader;


    public TServer(IPAddress ip, int port) {
      try {
        this.listener = new TcpListener(ip, port);
      } catch (SocketException ex) {
        _log.Error(ex.Message);
        _log.Info("Please shutdown and restart!");
      }
    }

    private void Swap<T>(ref T a, ref T b) {
      T t = a;
      a = b;
      b = t;
    }

    public void close() {
      if (this.listener != null) {
        this.listener.Stop();
      }
    }

    private void transCallback() {
      try {
        this.senderReader = new StreamReader(this.sender.GetStream());
        this.senderWriter = new StreamWriter(this.sender.GetStream());
        this.recverReader = new StreamReader(this.recver.GetStream());
        this.recverWriter = new StreamWriter(this.recver.GetStream());
        string sstr = this.senderReader.ReadLine();
        string rstr = this.recverReader.ReadLine();
        if (rstr == "sender" && sstr == "recver") {
          this.Swap(ref this.recver, ref this.sender);
          this.Swap(ref this.recverReader, ref this.senderReader);
          this.Swap(ref this.recverWriter, ref this.senderWriter);
        } else if (rstr != "recver" || sstr != "sender") {
          // 非法类型连接
          this.recverWriter.WriteLine("88");
          this.senderWriter.WriteLine("88");
          _log.Warn("Unknow link type.");
          this.recver.Close();
          this.sender.Close();
          return;
        }

        this.recverWriter.AutoFlush = true;
        this.senderWriter.AutoFlush = true;

        string transStr = string.Empty;
        this.senderWriter.WriteLine("OK");
        this.recverWriter.WriteLine("OK");
        while (true) {
          transStr = this.senderReader.ReadLine();
          if (!string.IsNullOrEmpty(transStr)) {
            if (transStr == "88") {
              this.recverWriter.WriteLine("88");
              _log.Info("Client closed the connection!");
              break;
            }

            _log.Info(string.Format("Received {0} byte(s) data from sender.",
              transStr.Length));
            _log.Info("[data] [sender] " + transStr.Substring(0,
              (transStr.Length > 100) ? 100 : transStr.Length));
            _log.Info("Now sending data to recver...");
            this.recverWriter.WriteLine(transStr);
          }

          transStr = this.recverReader.ReadLine();
          if (!string.IsNullOrEmpty(transStr)) {
            if (transStr == "88") {
              this.senderWriter.WriteLine("88");
              _log.Info("Client closed the connection!");
              break;
            }

            _log.Info(string.Format("Received {0} byte(s) data from recver.",
              transStr.Length));
            _log.Info("[data] [recver] " + transStr);
            _log.Info("Now sending data to sender...");
            this.senderWriter.WriteLine(transStr);
          }
        }
      } catch (Exception ex) {
        _log.Error(ex.Message);
      } finally {
        this.recver.Close();
        this.sender.Close();
      }
    }

    public void run() {
      this.listener.Start(2);
      _log.Info("TServer started!");
      Console.WriteLine("TServer started!");

      List<TcpClient> clients = new List<TcpClient>(2);
      try {
        _log.Info("Waiting for sender & recver...");
        while (true) {
          clients.Add(this.listener.AcceptTcpClient());
          _log.Info("Accept a client from " + clients[clients.Count - 1].Client.LocalEndPoint);
          if (clients.Count == 2) {
            if (clients[0].Connected && clients[1].Connected) {
              this.sender = clients[0];
              this.sender.ReceiveTimeout = 10000;	// 10s超时时间
              this.recver = clients[1];
              this.recver.ReceiveTimeout = 10000;
              this.transCallback();
              clients.Clear();
              _log.Info("Now reListening...");
            }
          }
        }
      } catch (Exception ex) {
        _log.Error(ex.Message);
      }
    }
  }
}
