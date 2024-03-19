
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
            this.label18 = new System.Windows.Forms.Label();
            this.lblPGMVer = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblDBVer = new System.Windows.Forms.Label();
            this.chkboxDebugMode = new System.Windows.Forms.CheckBox();
            this.chkboxLogMsgToFile = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnFixPair = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtboxTraceLevel = new System.Windows.Forms.TextBox();
            this.chkboxDeleteToBackup = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxMinStart = new System.Windows.Forms.CheckBox();
            this.chkBoxAutoClearLog = new System.Windows.Forms.CheckBox();
            this.btnSelectLocalTempFolder = new System.Windows.Forms.Button();
            this.txtboxLocalTempFolder = new System.Windows.Forms.TextBox();
            this.chkboxUseLocalTemp = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtboxRetrySyncInterval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtboxRetrySyncCount = new System.Windows.Forms.TextBox();
            this.btnClearLogFile = new System.Windows.Forms.Button();
            this.chkboxAutoRun = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pnlDebugTools = new System.Windows.Forms.Panel();
            this.lblCurrentDB = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboxDBMigration = new System.Windows.Forms.ComboBox();
            this.btnStartDBMigration = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtboxMaxKeepBackup = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pnlDebugTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(318, 326);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 12);
            this.label18.TabIndex = 104;
            this.label18.Text = "主程序版本：";
            // 
            // lblPGMVer
            // 
            this.lblPGMVer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPGMVer.AutoSize = true;
            this.lblPGMVer.Location = new System.Drawing.Point(401, 326);
            this.lblPGMVer.Name = "lblPGMVer";
            this.lblPGMVer.Size = new System.Drawing.Size(71, 12);
            this.lblPGMVer.TabIndex = 105;
            this.lblPGMVer.Text = "MainPrgmVer";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(318, 350);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 12);
            this.label17.TabIndex = 102;
            this.label17.Text = "数据库版本：";
            // 
            // lblDBVer
            // 
            this.lblDBVer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDBVer.AutoSize = true;
            this.lblDBVer.Location = new System.Drawing.Point(401, 350);
            this.lblDBVer.Name = "lblDBVer";
            this.lblDBVer.Size = new System.Drawing.Size(71, 12);
            this.lblDBVer.TabIndex = 103;
            this.lblDBVer.Text = "DatabaseVer";
            // 
            // chkboxDebugMode
            // 
            this.chkboxDebugMode.AutoSize = true;
            this.chkboxDebugMode.Location = new System.Drawing.Point(6, 22);
            this.chkboxDebugMode.Name = "chkboxDebugMode";
            this.chkboxDebugMode.Size = new System.Drawing.Size(180, 16);
            this.chkboxDebugMode.TabIndex = 106;
            this.chkboxDebugMode.Text = "调试模式（仅开发人员使用）";
            this.chkboxDebugMode.UseVisualStyleBackColor = true;
            // 
            // chkboxLogMsgToFile
            // 
            this.chkboxLogMsgToFile.AutoSize = true;
            this.chkboxLogMsgToFile.Location = new System.Drawing.Point(6, 44);
            this.chkboxLogMsgToFile.Name = "chkboxLogMsgToFile";
            this.chkboxLogMsgToFile.Size = new System.Drawing.Size(120, 16);
            this.chkboxLogMsgToFile.TabIndex = 107;
            this.chkboxLogMsgToFile.Text = "保存日志到文件：";
            this.chkboxLogMsgToFile.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(118, 329);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 33);
            this.button3.TabIndex = 109;
            this.button3.Text = "关闭";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 329);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 33);
            this.button2.TabIndex = 108;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnFixPair
            // 
            this.btnFixPair.Location = new System.Drawing.Point(129, 31);
            this.btnFixPair.Name = "btnFixPair";
            this.btnFixPair.Size = new System.Drawing.Size(120, 25);
            this.btnFixPair.TabIndex = 110;
            this.btnFixPair.Text = "修复文件夹配对";
            this.btnFixPair.UseVisualStyleBackColor = true;
            this.btnFixPair.Click += new System.EventHandler(this.btnFixPair_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(263, 12);
            this.label1.TabIndex = 111;
            this.label1.Text = "日志消息等级（1~5，等级越高日志记录越完整）";
            // 
            // txtboxTraceLevel
            // 
            this.txtboxTraceLevel.Location = new System.Drawing.Point(274, 46);
            this.txtboxTraceLevel.Name = "txtboxTraceLevel";
            this.txtboxTraceLevel.Size = new System.Drawing.Size(40, 21);
            this.txtboxTraceLevel.TabIndex = 112;
            // 
            // chkboxDeleteToBackup
            // 
            this.chkboxDeleteToBackup.AutoSize = true;
            this.chkboxDeleteToBackup.Location = new System.Drawing.Point(6, 69);
            this.chkboxDeleteToBackup.Name = "chkboxDeleteToBackup";
            this.chkboxDeleteToBackup.Size = new System.Drawing.Size(348, 16);
            this.chkboxDeleteToBackup.TabIndex = 113;
            this.chkboxDeleteToBackup.Text = "删除文件到\"_FSBackup\"目录 (隐藏目录)，最大保留备份数量";
            this.chkboxDeleteToBackup.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtboxMaxKeepBackup);
            this.groupBox1.Controls.Add(this.chkBoxMinStart);
            this.groupBox1.Controls.Add(this.chkBoxAutoClearLog);
            this.groupBox1.Controls.Add(this.btnSelectLocalTempFolder);
            this.groupBox1.Controls.Add(this.txtboxLocalTempFolder);
            this.groupBox1.Controls.Add(this.chkboxUseLocalTemp);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtboxRetrySyncInterval);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtboxRetrySyncCount);
            this.groupBox1.Controls.Add(this.btnClearLogFile);
            this.groupBox1.Controls.Add(this.chkboxAutoRun);
            this.groupBox1.Controls.Add(this.chkboxDeleteToBackup);
            this.groupBox1.Controls.Add(this.chkboxLogMsgToFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 167);
            this.groupBox1.TabIndex = 114;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "通用设置";
            // 
            // chkBoxMinStart
            // 
            this.chkBoxMinStart.AutoSize = true;
            this.chkBoxMinStart.Location = new System.Drawing.Point(144, 20);
            this.chkBoxMinStart.Name = "chkBoxMinStart";
            this.chkBoxMinStart.Size = new System.Drawing.Size(144, 16);
            this.chkBoxMinStart.TabIndex = 127;
            this.chkBoxMinStart.Text = "程序启动时最小化窗口";
            this.chkBoxMinStart.UseVisualStyleBackColor = true;
            // 
            // chkBoxAutoClearLog
            // 
            this.chkBoxAutoClearLog.AutoSize = true;
            this.chkBoxAutoClearLog.Location = new System.Drawing.Point(310, 44);
            this.chkBoxAutoClearLog.Name = "chkBoxAutoClearLog";
            this.chkBoxAutoClearLog.Size = new System.Drawing.Size(144, 16);
            this.chkBoxAutoClearLog.TabIndex = 126;
            this.chkBoxAutoClearLog.Text = "每日自动清除界面日志";
            this.chkBoxAutoClearLog.UseVisualStyleBackColor = true;
            // 
            // btnSelectLocalTempFolder
            // 
            this.btnSelectLocalTempFolder.Location = new System.Drawing.Point(423, 87);
            this.btnSelectLocalTempFolder.Name = "btnSelectLocalTempFolder";
            this.btnSelectLocalTempFolder.Size = new System.Drawing.Size(31, 25);
            this.btnSelectLocalTempFolder.TabIndex = 125;
            this.btnSelectLocalTempFolder.Text = "...";
            this.btnSelectLocalTempFolder.UseVisualStyleBackColor = true;
            this.btnSelectLocalTempFolder.Click += new System.EventHandler(this.btnSelectLocalTempFolder_Click);
            // 
            // txtboxLocalTempFolder
            // 
            this.txtboxLocalTempFolder.Location = new System.Drawing.Point(144, 91);
            this.txtboxLocalTempFolder.Name = "txtboxLocalTempFolder";
            this.txtboxLocalTempFolder.Size = new System.Drawing.Size(273, 21);
            this.txtboxLocalTempFolder.TabIndex = 124;
            // 
            // chkboxUseLocalTemp
            // 
            this.chkboxUseLocalTemp.AutoSize = true;
            this.chkboxUseLocalTemp.Location = new System.Drawing.Point(6, 95);
            this.chkboxUseLocalTemp.Name = "chkboxUseLocalTemp";
            this.chkboxUseLocalTemp.Size = new System.Drawing.Size(120, 16);
            this.chkboxUseLocalTemp.TabIndex = 123;
            this.chkboxUseLocalTemp.Text = "使用本地临时目录";
            this.chkboxUseLocalTemp.UseVisualStyleBackColor = true;
            this.chkboxUseLocalTemp.CheckedChanged += new System.EventHandler(this.chkboxUseLocalTemp_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 145);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(186, 16);
            this.checkBox2.TabIndex = 122;
            this.checkBox2.Text = "(XXXXXXXOccupyLabelXXXXXXX)";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(282, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 121;
            this.label4.Text = "次";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(180, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 120;
            this.label3.Text = "分钟后重试";
            // 
            // txtboxRetrySyncInterval
            // 
            this.txtboxRetrySyncInterval.Location = new System.Drawing.Point(149, 118);
            this.txtboxRetrySyncInterval.Name = "txtboxRetrySyncInterval";
            this.txtboxRetrySyncInterval.Size = new System.Drawing.Size(25, 21);
            this.txtboxRetrySyncInterval.TabIndex = 119;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 12);
            this.label2.TabIndex = 117;
            this.label2.Text = "当文件同步失败时，等待";
            // 
            // txtboxRetrySyncCount
            // 
            this.txtboxRetrySyncCount.Location = new System.Drawing.Point(251, 118);
            this.txtboxRetrySyncCount.Name = "txtboxRetrySyncCount";
            this.txtboxRetrySyncCount.Size = new System.Drawing.Size(25, 21);
            this.txtboxRetrySyncCount.TabIndex = 118;
            // 
            // btnClearLogFile
            // 
            this.btnClearLogFile.Location = new System.Drawing.Point(184, 39);
            this.btnClearLogFile.Name = "btnClearLogFile";
            this.btnClearLogFile.Size = new System.Drawing.Size(120, 25);
            this.btnClearLogFile.TabIndex = 115;
            this.btnClearLogFile.Text = "清理日志文件";
            this.btnClearLogFile.UseVisualStyleBackColor = true;
            this.btnClearLogFile.Click += new System.EventHandler(this.btnClearLogFile_Click);
            // 
            // chkboxAutoRun
            // 
            this.chkboxAutoRun.AutoSize = true;
            this.chkboxAutoRun.Location = new System.Drawing.Point(6, 20);
            this.chkboxAutoRun.Name = "chkboxAutoRun";
            this.chkboxAutoRun.Size = new System.Drawing.Size(132, 16);
            this.chkboxAutoRun.TabIndex = 114;
            this.chkboxAutoRun.Text = "进入系统后自动启动";
            this.chkboxAutoRun.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pnlDebugTools);
            this.groupBox2.Controls.Add(this.chkboxDebugMode);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtboxTraceLevel);
            this.groupBox2.Location = new System.Drawing.Point(12, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 138);
            this.groupBox2.TabIndex = 115;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "开发人员设置(!!!请谨慎使用，可能会有未知BUG!!!)";
            // 
            // pnlDebugTools
            // 
            this.pnlDebugTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDebugTools.Controls.Add(this.lblCurrentDB);
            this.pnlDebugTools.Controls.Add(this.label5);
            this.pnlDebugTools.Controls.Add(this.comboxDBMigration);
            this.pnlDebugTools.Controls.Add(this.btnFixPair);
            this.pnlDebugTools.Controls.Add(this.btnStartDBMigration);
            this.pnlDebugTools.Controls.Add(this.button1);
            this.pnlDebugTools.Location = new System.Drawing.Point(6, 73);
            this.pnlDebugTools.Name = "pnlDebugTools";
            this.pnlDebugTools.Size = new System.Drawing.Size(448, 59);
            this.pnlDebugTools.TabIndex = 117;
            // 
            // lblCurrentDB
            // 
            this.lblCurrentDB.AutoSize = true;
            this.lblCurrentDB.Location = new System.Drawing.Point(3, 9);
            this.lblCurrentDB.Name = "lblCurrentDB";
            this.lblCurrentDB.Size = new System.Drawing.Size(77, 12);
            this.lblCurrentDB.TabIndex = 121;
            this.lblCurrentDB.Text = "当前数据库：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(146, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 120;
            this.label5.Text = "数据库迁移至";
            // 
            // comboxDBMigration
            // 
            this.comboxDBMigration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboxDBMigration.FormattingEnabled = true;
            this.comboxDBMigration.Items.AddRange(new object[] {
            "SQLITE",
            "ACCESS"});
            this.comboxDBMigration.Location = new System.Drawing.Point(229, 6);
            this.comboxDBMigration.Name = "comboxDBMigration";
            this.comboxDBMigration.Size = new System.Drawing.Size(90, 20);
            this.comboxDBMigration.TabIndex = 119;
            // 
            // btnStartDBMigration
            // 
            this.btnStartDBMigration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartDBMigration.Location = new System.Drawing.Point(325, 3);
            this.btnStartDBMigration.Name = "btnStartDBMigration";
            this.btnStartDBMigration.Size = new System.Drawing.Size(120, 25);
            this.btnStartDBMigration.TabIndex = 117;
            this.btnStartDBMigration.Text = "开始数据库迁移";
            this.btnStartDBMigration.UseVisualStyleBackColor = true;
            this.btnStartDBMigration.Click += new System.EventHandler(this.btnStartDBMigration_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(3, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 25);
            this.button1.TabIndex = 116;
            this.button1.Text = "备份数据库";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtboxMaxKeepBackup
            // 
            this.txtboxMaxKeepBackup.Location = new System.Drawing.Point(360, 67);
            this.txtboxMaxKeepBackup.Name = "txtboxMaxKeepBackup";
            this.txtboxMaxKeepBackup.Size = new System.Drawing.Size(25, 21);
            this.txtboxMaxKeepBackup.TabIndex = 129;
            // 
            // frm_GlobalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(484, 371);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.lblPGMVer);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lblDBVer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frm_GlobalSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "全局设置";
            this.Load += new System.EventHandler(this.frm_GlobalSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pnlDebugTools.ResumeLayout(false);
            this.pnlDebugTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.CheckBox checkBox2;
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
    }
}