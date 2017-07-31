﻿namespace Recver
{
	partial class Recver
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Recver));
			this.logInfoBox = new System.Windows.Forms.TextBox();
			this.btnSaveConfig = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tcpServerPort = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.tcpServerIP = new System.Windows.Forms.TextBox();
			this.dbGroupBox = new System.Windows.Forms.GroupBox();
			this.dbIP = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.dbPort = new System.Windows.Forms.NumericUpDown();
			this.password = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.userName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.modeSql = new System.Windows.Forms.RadioButton();
			this.modeWin = new System.Windows.Forms.RadioButton();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tips = new System.Windows.Forms.ToolTip();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
			this.showWinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.myNotify = new System.Windows.Forms.NotifyIcon();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.timerForLog = new System.Windows.Forms.Timer();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tcpServerPort)).BeginInit();
			this.dbGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dbPort)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// logInfoBox
			// 
			this.logInfoBox.BackColor = System.Drawing.SystemColors.WindowText;
			this.logInfoBox.ForeColor = System.Drawing.Color.Lime;
			this.logInfoBox.Location = new System.Drawing.Point(12, 236);
			this.logInfoBox.Multiline = true;
			this.logInfoBox.Name = "logInfoBox";
			this.logInfoBox.ReadOnly = true;
			this.logInfoBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logInfoBox.Size = new System.Drawing.Size(510, 145);
			this.logInfoBox.TabIndex = 44;
			this.logInfoBox.TabStop = false;
			this.tips.SetToolTip(this.logInfoBox, "日志信息");
			// 
			// btnSaveConfig
			// 
			this.btnSaveConfig.Location = new System.Drawing.Point(449, 141);
			this.btnSaveConfig.Name = "btnSaveConfig";
			this.btnSaveConfig.Size = new System.Drawing.Size(75, 23);
			this.btnSaveConfig.TabIndex = 12;
			this.btnSaveConfig.Text = "保存配置";
			this.tips.SetToolTip(this.btnSaveConfig, "保存配置到文件");
			this.btnSaveConfig.UseVisualStyleBackColor = true;
			this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tcpServerPort);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.tcpServerIP);
			this.groupBox2.Location = new System.Drawing.Point(241, 93);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 100);
			this.groupBox2.TabIndex = 40;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "TCP服务器";
			// 
			// tcpServerPort
			// 
			this.tcpServerPort.Location = new System.Drawing.Point(65, 62);
			this.tcpServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.tcpServerPort.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
			this.tcpServerPort.Name = "tcpServerPort";
			this.tcpServerPort.Size = new System.Drawing.Size(100, 21);
			this.tcpServerPort.TabIndex = 10;
			this.tips.SetToolTip(this.tcpServerPort, "除非你知道你在做什么，否则不要更改！");
			this.tcpServerPort.Value = new decimal(new int[] {
            54321,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 64);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(53, 12);
			this.label9.TabIndex = 17;
			this.label9.Text = "监听端口";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 29);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 12);
			this.label6.TabIndex = 14;
			this.label6.Text = "服务器IP";
			// 
			// tcpServerIP
			// 
			this.tcpServerIP.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
			this.tcpServerIP.Location = new System.Drawing.Point(65, 26);
			this.tcpServerIP.Name = "tcpServerIP";
			this.tcpServerIP.Size = new System.Drawing.Size(100, 21);
			this.tcpServerIP.TabIndex = 9;
			this.tcpServerIP.Text = "127.0.0.1";
			this.tips.SetToolTip(this.tcpServerIP, "除非你知道你在做什么，否则不要更改！");
			// 
			// dbGroupBox
			// 
			this.dbGroupBox.Controls.Add(this.dbIP);
			this.dbGroupBox.Controls.Add(this.label4);
			this.dbGroupBox.Controls.Add(this.label5);
			this.dbGroupBox.Controls.Add(this.dbPort);
			this.dbGroupBox.Controls.Add(this.password);
			this.dbGroupBox.Controls.Add(this.label8);
			this.dbGroupBox.Controls.Add(this.userName);
			this.dbGroupBox.Controls.Add(this.label7);
			this.dbGroupBox.Location = new System.Drawing.Point(20, 89);
			this.dbGroupBox.Name = "dbGroupBox";
			this.dbGroupBox.Size = new System.Drawing.Size(200, 129);
			this.dbGroupBox.TabIndex = 39;
			this.dbGroupBox.TabStop = false;
			this.dbGroupBox.Text = "数据库服务器";
			// 
			// dbIP
			// 
			this.dbIP.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
			this.dbIP.Location = new System.Drawing.Point(80, 20);
			this.dbIP.Name = "dbIP";
			this.dbIP.Size = new System.Drawing.Size(100, 21);
			this.dbIP.TabIndex = 3;
			this.tips.SetToolTip(this.dbIP, "数据库监听IP，可写成主机名或点分十进制方式");
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "数据库IP";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(10, 49);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 12);
			this.label5.TabIndex = 11;
			this.label5.Text = "监听端口";
			// 
			// dbPort
			// 
			this.dbPort.Location = new System.Drawing.Point(80, 47);
			this.dbPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.dbPort.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
			this.dbPort.Name = "dbPort";
			this.dbPort.Size = new System.Drawing.Size(100, 21);
			this.dbPort.TabIndex = 4;
			this.tips.SetToolTip(this.dbPort, "数据库监听端口");
			this.dbPort.Value = new decimal(new int[] {
            1433,
            0,
            0,
            0});
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(80, 101);
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.Size = new System.Drawing.Size(100, 21);
			this.password.TabIndex = 6;
			this.tips.SetToolTip(this.password, "数据库登录密码");
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(34, 104);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(29, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "密码";
			// 
			// userName
			// 
			this.userName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
			this.userName.Location = new System.Drawing.Point(80, 74);
			this.userName.Name = "userName";
			this.userName.Size = new System.Drawing.Size(100, 21);
			this.userName.TabIndex = 5;
			this.tips.SetToolTip(this.userName, "数据库登录用户名");
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(22, 77);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 12);
			this.label7.TabIndex = 15;
			this.label7.Text = "用户名";
			// 
			// modeSql
			// 
			this.modeSql.AutoSize = true;
			this.modeSql.Checked = true;
			this.modeSql.Location = new System.Drawing.Point(32, 42);
			this.modeSql.Name = "modeSql";
			this.modeSql.Size = new System.Drawing.Size(125, 16);
			this.modeSql.TabIndex = 2;
			this.modeSql.TabStop = true;
			this.modeSql.Text = "SqlServer身份验证";
			this.modeSql.UseVisualStyleBackColor = true;
			this.modeSql.CheckedChanged += new System.EventHandler(this.modeSql_CheckedChanged);
			// 
			// modeWin
			// 
			this.modeWin.AutoSize = true;
			this.modeWin.Location = new System.Drawing.Point(32, 20);
			this.modeWin.Name = "modeWin";
			this.modeWin.Size = new System.Drawing.Size(113, 16);
			this.modeWin.TabIndex = 1;
			this.modeWin.Text = "Windows身份验证";
			this.modeWin.UseVisualStyleBackColor = true;
			// 
			// btnExit
			// 
			this.btnExit.Enabled = false;
			this.btnExit.Location = new System.Drawing.Point(450, 170);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(75, 23);
			this.btnExit.TabIndex = 13;
			this.btnExit.Text = "停止";
			this.tips.SetToolTip(this.btnExit, "停止同步，同时会使sender停止");
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(450, 111);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 11;
			this.btnStart.Text = "开始";
			this.tips.SetToolTip(this.btnStart, "开始同步，需sender在线才能开始接收数据");
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showWinToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
			// 
			// showWinToolStripMenuItem
			// 
			this.showWinToolStripMenuItem.Name = "showWinToolStripMenuItem";
			this.showWinToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.showWinToolStripMenuItem.Text = "显示窗口";
			this.showWinToolStripMenuItem.Click += new System.EventHandler(this.showWinToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.exitToolStripMenuItem.Text = "退出";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// myNotify
			// 
			this.myNotify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.myNotify.BalloonTipText = "DBRecver将在后台运行，你可以在这里打开它";
			this.myNotify.BalloonTipTitle = "注意";
			this.myNotify.ContextMenuStrip = this.contextMenuStrip1;
			this.myNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("myNotify.Icon")));
			this.myNotify.Text = "DBSyncRecver";
			this.myNotify.DoubleClick += new System.EventHandler(this.myNotify_DoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.modeWin);
			this.groupBox1.Controls.Add(this.modeSql);
			this.groupBox1.Location = new System.Drawing.Point(20, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 64);
			this.groupBox1.TabIndex = 45;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "数据库连接验证方式";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 221);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 46;
			this.label1.Text = "日志信息";
			// 
			// timerForLog
			// 
			this.timerForLog.Enabled = true;
			this.timerForLog.Interval = 600000;
			this.timerForLog.Tick += new System.EventHandler(this.timerForLog_Tick);
			// 
			// Recver
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(544, 393);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.logInfoBox);
			this.Controls.Add(this.btnSaveConfig);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.dbGroupBox);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnStart);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Recver";
			this.Text = "DBSyncRecver";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Server_FormClosing);
			this.Load += new System.EventHandler(this.Server_Load);
			this.Resize += new System.EventHandler(this.Server_Resize);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tcpServerPort)).EndInit();
			this.dbGroupBox.ResumeLayout(false);
			this.dbGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dbPort)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSaveConfig;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown tcpServerPort;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tcpServerIP;
		private System.Windows.Forms.GroupBox dbGroupBox;
		private System.Windows.Forms.TextBox dbIP;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown dbPort;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox userName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton modeSql;
		private System.Windows.Forms.RadioButton modeWin;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.ToolTip tips;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.NotifyIcon myNotify;
		private System.Windows.Forms.ToolStripMenuItem showWinToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox logInfoBox;
		private System.Windows.Forms.Timer timerForLog;
	}
}

