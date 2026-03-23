
namespace FileSynchronizer
{
    partial class frm_GlobalSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_GlobalSettings));
            label18 = new System.Windows.Forms.Label();
            lblPGMVer = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            lblDBVer = new System.Windows.Forms.Label();
            chkboxDebugMode = new System.Windows.Forms.CheckBox();
            chkboxLogMsgToFile = new System.Windows.Forms.CheckBox();
            button3 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            btnFixPair = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtboxTraceLevel = new System.Windows.Forms.TextBox();
            chkboxDeleteToBackup = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label6 = new System.Windows.Forms.Label();
            txtboxAutoCheckUpdateInterval = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            txtboxMaxKeepBackup = new System.Windows.Forms.TextBox();
            chkBoxMinStart = new System.Windows.Forms.CheckBox();
            chkBoxAutoClearLog = new System.Windows.Forms.CheckBox();
            btnSelectLocalTempFolder = new System.Windows.Forms.Button();
            txtboxLocalTempFolder = new System.Windows.Forms.TextBox();
            chkboxUseLocalTemp = new System.Windows.Forms.CheckBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            txtboxRetrySyncInterval = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtboxRetrySyncCount = new System.Windows.Forms.TextBox();
            btnClearLogFile = new System.Windows.Forms.Button();
            chkboxAutoRun = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            chkboxDevelopMode = new System.Windows.Forms.CheckBox();
            pnlDebugTools = new System.Windows.Forms.Panel();
            btnSetGithubToken = new System.Windows.Forms.Button();
            btnSQLRunner = new System.Windows.Forms.Button();
            lblCurrentDB = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            comboxDBMigration = new System.Windows.Forms.ComboBox();
            btnStartDBMigration = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            pnlDebugTools.SuspendLayout();
            SuspendLayout();
            // 
            // label18
            // 
            label18.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(410, 468);
            label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(80, 17);
            label18.TabIndex = 104;
            label18.Text = "主程序版本：";
            // 
            // lblPGMVer
            // 
            lblPGMVer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            lblPGMVer.AutoSize = true;
            lblPGMVer.Location = new System.Drawing.Point(507, 468);
            lblPGMVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPGMVer.Name = "lblPGMVer";
            lblPGMVer.Size = new System.Drawing.Size(88, 17);
            lblPGMVer.TabIndex = 105;
            lblPGMVer.Text = "MainPrgmVer";
            // 
            // label17
            // 
            label17.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(410, 502);
            label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(80, 17);
            label17.TabIndex = 102;
            label17.Text = "数据库版本：";
            // 
            // lblDBVer
            // 
            lblDBVer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            lblDBVer.AutoSize = true;
            lblDBVer.Location = new System.Drawing.Point(507, 502);
            lblDBVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDBVer.Name = "lblDBVer";
            lblDBVer.Size = new System.Drawing.Size(83, 17);
            lblDBVer.TabIndex = 103;
            lblDBVer.Text = "DatabaseVer";
            // 
            // chkboxDebugMode
            // 
            chkboxDebugMode.AutoSize = true;
            chkboxDebugMode.Location = new System.Drawing.Point(7, 31);
            chkboxDebugMode.Margin = new System.Windows.Forms.Padding(4);
            chkboxDebugMode.Name = "chkboxDebugMode";
            chkboxDebugMode.Size = new System.Drawing.Size(75, 21);
            chkboxDebugMode.TabIndex = 106;
            chkboxDebugMode.Text = "调试模式";
            chkboxDebugMode.UseVisualStyleBackColor = true;
            // 
            // chkboxLogMsgToFile
            // 
            chkboxLogMsgToFile.AutoSize = true;
            chkboxLogMsgToFile.Location = new System.Drawing.Point(7, 62);
            chkboxLogMsgToFile.Margin = new System.Windows.Forms.Padding(4);
            chkboxLogMsgToFile.Name = "chkboxLogMsgToFile";
            chkboxLogMsgToFile.Size = new System.Drawing.Size(123, 21);
            chkboxLogMsgToFile.TabIndex = 107;
            chkboxLogMsgToFile.Text = "保存日志到文件：";
            chkboxLogMsgToFile.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button3.Location = new System.Drawing.Point(138, 472);
            button3.Margin = new System.Windows.Forms.Padding(4);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(117, 47);
            button3.TabIndex = 109;
            button3.Text = "关闭";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button2.Location = new System.Drawing.Point(14, 472);
            button2.Margin = new System.Windows.Forms.Padding(4);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(117, 47);
            button2.TabIndex = 108;
            button2.Text = "保存";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnFixPair
            // 
            btnFixPair.Location = new System.Drawing.Point(139, 44);
            btnFixPair.Margin = new System.Windows.Forms.Padding(4);
            btnFixPair.Name = "btnFixPair";
            btnFixPair.Size = new System.Drawing.Size(128, 35);
            btnFixPair.TabIndex = 110;
            btnFixPair.Text = "修复文件夹配对";
            btnFixPair.UseVisualStyleBackColor = true;
            btnFixPair.Click += btnFixPair_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(303, 32);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(247, 17);
            label1.TabIndex = 111;
            label1.Text = "日志等级（1~5，等级越高日志记录越完整）";
            // 
            // txtboxTraceLevel
            // 
            txtboxTraceLevel.Location = new System.Drawing.Point(546, 29);
            txtboxTraceLevel.Margin = new System.Windows.Forms.Padding(4);
            txtboxTraceLevel.Name = "txtboxTraceLevel";
            txtboxTraceLevel.Size = new System.Drawing.Size(23, 23);
            txtboxTraceLevel.TabIndex = 112;
            // 
            // chkboxDeleteToBackup
            // 
            chkboxDeleteToBackup.AutoSize = true;
            chkboxDeleteToBackup.Location = new System.Drawing.Point(7, 98);
            chkboxDeleteToBackup.Margin = new System.Windows.Forms.Padding(4);
            chkboxDeleteToBackup.Name = "chkboxDeleteToBackup";
            chkboxDeleteToBackup.Size = new System.Drawing.Size(350, 21);
            chkboxDeleteToBackup.TabIndex = 113;
            chkboxDeleteToBackup.Text = "删除文件到\"_FSBackup\"目录 (隐藏目录)，最大保留备份数量";
            chkboxDeleteToBackup.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(txtboxAutoCheckUpdateInterval);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtboxMaxKeepBackup);
            groupBox1.Controls.Add(chkBoxMinStart);
            groupBox1.Controls.Add(chkBoxAutoClearLog);
            groupBox1.Controls.Add(btnSelectLocalTempFolder);
            groupBox1.Controls.Add(txtboxLocalTempFolder);
            groupBox1.Controls.Add(chkboxUseLocalTemp);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtboxRetrySyncInterval);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtboxRetrySyncCount);
            groupBox1.Controls.Add(btnClearLogFile);
            groupBox1.Controls.Add(chkboxAutoRun);
            groupBox1.Controls.Add(chkboxDeleteToBackup);
            groupBox1.Controls.Add(chkboxLogMsgToFile);
            groupBox1.Location = new System.Drawing.Point(14, 17);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(576, 242);
            groupBox1.TabIndex = 114;
            groupBox1.TabStop = false;
            groupBox1.Text = "通用设置";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(210, 210);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(159, 17);
            label6.TabIndex = 132;
            label6.Text = "天（设置为0则不自动检查）";
            // 
            // txtboxAutoCheckUpdateInterval
            // 
            txtboxAutoCheckUpdateInterval.Location = new System.Drawing.Point(174, 205);
            txtboxAutoCheckUpdateInterval.Margin = new System.Windows.Forms.Padding(4);
            txtboxAutoCheckUpdateInterval.Name = "txtboxAutoCheckUpdateInterval";
            txtboxAutoCheckUpdateInterval.Size = new System.Drawing.Size(28, 23);
            txtboxAutoCheckUpdateInterval.TabIndex = 131;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(7, 210);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(128, 17);
            label7.TabIndex = 130;
            label7.Text = "自动查找程序更新，每";
            // 
            // txtboxMaxKeepBackup
            // 
            txtboxMaxKeepBackup.Location = new System.Drawing.Point(365, 96);
            txtboxMaxKeepBackup.Margin = new System.Windows.Forms.Padding(4);
            txtboxMaxKeepBackup.Name = "txtboxMaxKeepBackup";
            txtboxMaxKeepBackup.Size = new System.Drawing.Size(28, 23);
            txtboxMaxKeepBackup.TabIndex = 129;
            // 
            // chkBoxMinStart
            // 
            chkBoxMinStart.AutoSize = true;
            chkBoxMinStart.Location = new System.Drawing.Point(168, 28);
            chkBoxMinStart.Margin = new System.Windows.Forms.Padding(4);
            chkBoxMinStart.Name = "chkBoxMinStart";
            chkBoxMinStart.Size = new System.Drawing.Size(147, 21);
            chkBoxMinStart.TabIndex = 127;
            chkBoxMinStart.Text = "程序启动时最小化窗口";
            chkBoxMinStart.UseVisualStyleBackColor = true;
            // 
            // chkBoxAutoClearLog
            // 
            chkBoxAutoClearLog.AutoSize = true;
            chkBoxAutoClearLog.Location = new System.Drawing.Point(421, 62);
            chkBoxAutoClearLog.Margin = new System.Windows.Forms.Padding(4);
            chkBoxAutoClearLog.Name = "chkBoxAutoClearLog";
            chkBoxAutoClearLog.Size = new System.Drawing.Size(147, 21);
            chkBoxAutoClearLog.TabIndex = 126;
            chkBoxAutoClearLog.Text = "每日自动清除界面日志";
            chkBoxAutoClearLog.UseVisualStyleBackColor = true;
            // 
            // btnSelectLocalTempFolder
            // 
            btnSelectLocalTempFolder.Location = new System.Drawing.Point(532, 125);
            btnSelectLocalTempFolder.Margin = new System.Windows.Forms.Padding(4);
            btnSelectLocalTempFolder.Name = "btnSelectLocalTempFolder";
            btnSelectLocalTempFolder.Size = new System.Drawing.Size(36, 30);
            btnSelectLocalTempFolder.TabIndex = 125;
            btnSelectLocalTempFolder.Text = "...";
            btnSelectLocalTempFolder.UseVisualStyleBackColor = true;
            btnSelectLocalTempFolder.Click += btnSelectLocalTempFolder_Click;
            // 
            // txtboxLocalTempFolder
            // 
            txtboxLocalTempFolder.Location = new System.Drawing.Point(168, 129);
            txtboxLocalTempFolder.Margin = new System.Windows.Forms.Padding(4);
            txtboxLocalTempFolder.Name = "txtboxLocalTempFolder";
            txtboxLocalTempFolder.Size = new System.Drawing.Size(356, 23);
            txtboxLocalTempFolder.TabIndex = 124;
            // 
            // chkboxUseLocalTemp
            // 
            chkboxUseLocalTemp.AutoSize = true;
            chkboxUseLocalTemp.Location = new System.Drawing.Point(7, 135);
            chkboxUseLocalTemp.Margin = new System.Windows.Forms.Padding(4);
            chkboxUseLocalTemp.Name = "chkboxUseLocalTemp";
            chkboxUseLocalTemp.Size = new System.Drawing.Size(123, 21);
            chkboxUseLocalTemp.TabIndex = 123;
            chkboxUseLocalTemp.Text = "使用本地临时目录";
            chkboxUseLocalTemp.UseVisualStyleBackColor = true;
            chkboxUseLocalTemp.CheckedChanged += chkboxUseLocalTemp_CheckedChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(329, 171);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(20, 17);
            label4.TabIndex = 121;
            label4.Text = "次";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(210, 171);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(68, 17);
            label3.TabIndex = 120;
            label3.Text = "分钟后重试";
            // 
            // txtboxRetrySyncInterval
            // 
            txtboxRetrySyncInterval.Location = new System.Drawing.Point(174, 167);
            txtboxRetrySyncInterval.Margin = new System.Windows.Forms.Padding(4);
            txtboxRetrySyncInterval.Name = "txtboxRetrySyncInterval";
            txtboxRetrySyncInterval.Size = new System.Drawing.Size(28, 23);
            txtboxRetrySyncInterval.TabIndex = 119;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 171);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(140, 17);
            label2.TabIndex = 117;
            label2.Text = "当文件同步失败时，等待";
            // 
            // txtboxRetrySyncCount
            // 
            txtboxRetrySyncCount.Location = new System.Drawing.Point(293, 167);
            txtboxRetrySyncCount.Margin = new System.Windows.Forms.Padding(4);
            txtboxRetrySyncCount.Name = "txtboxRetrySyncCount";
            txtboxRetrySyncCount.Size = new System.Drawing.Size(28, 23);
            txtboxRetrySyncCount.TabIndex = 118;
            // 
            // btnClearLogFile
            // 
            btnClearLogFile.Location = new System.Drawing.Point(274, 54);
            btnClearLogFile.Margin = new System.Windows.Forms.Padding(4);
            btnClearLogFile.Name = "btnClearLogFile";
            btnClearLogFile.Size = new System.Drawing.Size(140, 35);
            btnClearLogFile.TabIndex = 115;
            btnClearLogFile.Text = "清理日志文件";
            btnClearLogFile.UseVisualStyleBackColor = true;
            btnClearLogFile.Click += btnClearLogFile_Click;
            // 
            // chkboxAutoRun
            // 
            chkboxAutoRun.AutoSize = true;
            chkboxAutoRun.Location = new System.Drawing.Point(7, 28);
            chkboxAutoRun.Margin = new System.Windows.Forms.Padding(4);
            chkboxAutoRun.Name = "chkboxAutoRun";
            chkboxAutoRun.Size = new System.Drawing.Size(135, 21);
            chkboxAutoRun.TabIndex = 114;
            chkboxAutoRun.Text = "进入系统后自动启动";
            chkboxAutoRun.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(txtboxTraceLevel);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(chkboxDevelopMode);
            groupBox2.Controls.Add(pnlDebugTools);
            groupBox2.Controls.Add(chkboxDebugMode);
            groupBox2.Location = new System.Drawing.Point(14, 268);
            groupBox2.Margin = new System.Windows.Forms.Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4);
            groupBox2.Size = new System.Drawing.Size(576, 196);
            groupBox2.TabIndex = 115;
            groupBox2.TabStop = false;
            groupBox2.Text = "开发人员设置(!!!仅开发人员，请谨慎使用，可能会有未知BUG!!!)";
            // 
            // chkboxDevelopMode
            // 
            chkboxDevelopMode.AutoSize = true;
            chkboxDevelopMode.Location = new System.Drawing.Point(84, 31);
            chkboxDevelopMode.Margin = new System.Windows.Forms.Padding(4);
            chkboxDevelopMode.Name = "chkboxDevelopMode";
            chkboxDevelopMode.Size = new System.Drawing.Size(231, 21);
            chkboxDevelopMode.TabIndex = 118;
            chkboxDevelopMode.Text = "开发者模式（需要重新启动程序生效）";
            chkboxDevelopMode.UseVisualStyleBackColor = true;
            // 
            // pnlDebugTools
            // 
            pnlDebugTools.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pnlDebugTools.Controls.Add(btnSetGithubToken);
            pnlDebugTools.Controls.Add(btnSQLRunner);
            pnlDebugTools.Controls.Add(lblCurrentDB);
            pnlDebugTools.Controls.Add(label5);
            pnlDebugTools.Controls.Add(comboxDBMigration);
            pnlDebugTools.Controls.Add(btnFixPair);
            pnlDebugTools.Controls.Add(btnStartDBMigration);
            pnlDebugTools.Controls.Add(button1);
            pnlDebugTools.Location = new System.Drawing.Point(7, 67);
            pnlDebugTools.Margin = new System.Windows.Forms.Padding(4);
            pnlDebugTools.Name = "pnlDebugTools";
            pnlDebugTools.Size = new System.Drawing.Size(562, 120);
            pnlDebugTools.TabIndex = 117;
            // 
            // btnSetGithubToken
            // 
            btnSetGithubToken.Location = new System.Drawing.Point(410, 44);
            btnSetGithubToken.Margin = new System.Windows.Forms.Padding(4);
            btnSetGithubToken.Name = "btnSetGithubToken";
            btnSetGithubToken.Size = new System.Drawing.Size(133, 35);
            btnSetGithubToken.TabIndex = 123;
            btnSetGithubToken.Text = "设置Github Token";
            btnSetGithubToken.UseVisualStyleBackColor = true;
            btnSetGithubToken.Click += btnSetGithubToken_Click;
            // 
            // btnSQLRunner
            // 
            btnSQLRunner.Location = new System.Drawing.Point(274, 44);
            btnSQLRunner.Margin = new System.Windows.Forms.Padding(4);
            btnSQLRunner.Name = "btnSQLRunner";
            btnSQLRunner.Size = new System.Drawing.Size(128, 35);
            btnSQLRunner.TabIndex = 122;
            btnSQLRunner.Text = "SQLRunner";
            btnSQLRunner.UseVisualStyleBackColor = true;
            btnSQLRunner.Click += btnSQLRunner_Click;
            // 
            // lblCurrentDB
            // 
            lblCurrentDB.AutoSize = true;
            lblCurrentDB.Location = new System.Drawing.Point(4, 13);
            lblCurrentDB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCurrentDB.Name = "lblCurrentDB";
            lblCurrentDB.Size = new System.Drawing.Size(80, 17);
            lblCurrentDB.TabIndex = 121;
            lblCurrentDB.Text = "当前数据库：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(170, 13);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(80, 17);
            label5.TabIndex = 120;
            label5.Text = "数据库迁移至";
            // 
            // comboxDBMigration
            // 
            comboxDBMigration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboxDBMigration.FormattingEnabled = true;
            comboxDBMigration.Items.AddRange(new object[] { "SQLITE", "ACCESS" });
            comboxDBMigration.Location = new System.Drawing.Point(267, 8);
            comboxDBMigration.Margin = new System.Windows.Forms.Padding(4);
            comboxDBMigration.Name = "comboxDBMigration";
            comboxDBMigration.Size = new System.Drawing.Size(128, 25);
            comboxDBMigration.TabIndex = 119;
            // 
            // btnStartDBMigration
            // 
            btnStartDBMigration.Location = new System.Drawing.Point(402, 4);
            btnStartDBMigration.Margin = new System.Windows.Forms.Padding(4);
            btnStartDBMigration.Name = "btnStartDBMigration";
            btnStartDBMigration.Size = new System.Drawing.Size(140, 35);
            btnStartDBMigration.TabIndex = 117;
            btnStartDBMigration.Text = "开始数据库迁移";
            btnStartDBMigration.UseVisualStyleBackColor = true;
            btnStartDBMigration.Click += btnStartDBMigration_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(4, 44);
            button1.Margin = new System.Windows.Forms.Padding(4);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(128, 35);
            button1.TabIndex = 116;
            button1.Text = "备份数据库";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // frm_GlobalSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = button3;
            ClientSize = new System.Drawing.Size(604, 531);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label18);
            Controls.Add(lblPGMVer);
            Controls.Add(label17);
            Controls.Add(lblDBVer);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            Name = "frm_GlobalSettings";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "全局设置";
            Load += frm_GlobalSettings_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            pnlDebugTools.ResumeLayout(false);
            pnlDebugTools.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblPGMVer;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblDBVer;
        private System.Windows.Forms.CheckBox chkboxDebugMode;
        private System.Windows.Forms.CheckBox chkboxLogMsgToFile;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnFixPair;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtboxTraceLevel;
        private System.Windows.Forms.CheckBox chkboxDeleteToBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkboxAutoRun;
        private System.Windows.Forms.Button btnClearLogFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtboxRetrySyncInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtboxRetrySyncCount;
        private System.Windows.Forms.CheckBox chkboxUseLocalTemp;
        private System.Windows.Forms.TextBox txtboxLocalTempFolder;
        private System.Windows.Forms.Button btnSelectLocalTempFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnStartDBMigration;
        private System.Windows.Forms.ComboBox comboxDBMigration;
        private System.Windows.Forms.Panel pnlDebugTools;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCurrentDB;
        private System.Windows.Forms.CheckBox chkBoxAutoClearLog;
        private System.Windows.Forms.CheckBox chkBoxMinStart;
        private System.Windows.Forms.TextBox txtboxMaxKeepBackup;
        private System.Windows.Forms.Button btnSQLRunner;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtboxAutoCheckUpdateInterval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSetGithubToken;
        private System.Windows.Forms.CheckBox chkboxDevelopMode;
    }
}