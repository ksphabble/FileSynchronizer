namespace FileSynchronizer
{
    partial class frm_FileSynchronizer
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_FileSynchronizer));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.TxtProgramLog = new System.Windows.Forms.RichTextBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnAnalysis = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemShowMain = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemSync = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemSyncAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.PgmMenu = new System.Windows.Forms.MenuStrip();
            this.MenuItemPGM = new System.Windows.Forms.ToolStripMenuItem();
            this.全局设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理同步文件夹对ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.更新日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStopOpr = new System.Windows.Forms.Button();
            this.btnSyncAll = new System.Windows.Forms.Button();
            this.btnPauseSync = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnOpenPairLog = new System.Windows.Forms.Button();
            this.项目Github主页ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.NotifyMenu.SuspendLayout();
            this.PgmMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMain);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 162);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1240, 443);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.TxtProgramLog);
            this.tabPageMain.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPageMain.Location = new System.Drawing.Point(4, 27);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1232, 412);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "程序日志";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // TxtProgramLog
            // 
            this.TxtProgramLog.BackColor = System.Drawing.SystemColors.Info;
            this.TxtProgramLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtProgramLog.Location = new System.Drawing.Point(3, 3);
            this.TxtProgramLog.Name = "TxtProgramLog";
            this.TxtProgramLog.ReadOnly = true;
            this.TxtProgramLog.Size = new System.Drawing.Size(1226, 406);
            this.TxtProgramLog.TabIndex = 0;
            this.TxtProgramLog.TabStop = false;
            this.TxtProgramLog.Text = "";
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSync.Location = new System.Drawing.Point(224, 639);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(100, 30);
            this.btnSync.TabIndex = 4;
            this.btnSync.Text = "同步当前配对";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnAnalysis
            // 
            this.btnAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAnalysis.Location = new System.Drawing.Point(648, 639);
            this.btnAnalysis.Name = "btnAnalysis";
            this.btnAnalysis.Size = new System.Drawing.Size(100, 30);
            this.btnAnalysis.TabIndex = 5;
            this.btnAnalysis.Text = "分析当前配对";
            this.btnAnalysis.UseVisualStyleBackColor = true;
            this.btnAnalysis.Click += new System.EventHandler(this.btnAnalysis_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.NotifyMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "FileSynchronizer";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // NotifyMenu
            // 
            this.NotifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.NotifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemShowMain,
            this.MenuItemSync,
            this.MenuItemSyncAll,
            this.toolStripSeparator1,
            this.MenuItemExit});
            this.NotifyMenu.Name = "NotifyMenu";
            this.NotifyMenu.Size = new System.Drawing.Size(154, 106);
            // 
            // MenuItemShowMain
            // 
            this.MenuItemShowMain.Name = "MenuItemShowMain";
            this.MenuItemShowMain.Size = new System.Drawing.Size(153, 24);
            this.MenuItemShowMain.Text = "显示主界面";
            this.MenuItemShowMain.Click += new System.EventHandler(this.MenuItemShowMain_Click);
            this.MenuItemShowMain.MouseEnter += new System.EventHandler(this.MenuItemOther_MouseEnter);
            // 
            // MenuItemSync
            // 
            this.MenuItemSync.Name = "MenuItemSync";
            this.MenuItemSync.Size = new System.Drawing.Size(153, 24);
            this.MenuItemSync.Text = "同步";
            this.MenuItemSync.MouseEnter += new System.EventHandler(this.MenuItemSync_MouseEnter);
            // 
            // MenuItemSyncAll
            // 
            this.MenuItemSyncAll.Name = "MenuItemSyncAll";
            this.MenuItemSyncAll.Size = new System.Drawing.Size(153, 24);
            this.MenuItemSyncAll.Text = "同步所有";
            this.MenuItemSyncAll.Click += new System.EventHandler(this.MenuItemSyncAll_Click);
            this.MenuItemSyncAll.MouseEnter += new System.EventHandler(this.MenuItemOther_MouseEnter);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(153, 24);
            this.MenuItemExit.Text = "退出程序";
            this.MenuItemExit.Click += new System.EventHandler(this.MenuItemExit_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(12, 639);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 30);
            this.btnClearLog.TabIndex = 17;
            this.btnClearLog.Text = "清空日志";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // PgmMenu
            // 
            this.PgmMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.PgmMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemPGM,
            this.MenuItemHelp});
            this.PgmMenu.Location = new System.Drawing.Point(0, 0);
            this.PgmMenu.Name = "PgmMenu";
            this.PgmMenu.Size = new System.Drawing.Size(1264, 28);
            this.PgmMenu.TabIndex = 18;
            this.PgmMenu.Text = "PgmMenu";
            // 
            // MenuItemPGM
            // 
            this.MenuItemPGM.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.全局设置ToolStripMenuItem,
            this.管理同步文件夹对ToolStripMenuItem,
            this.toolStripSeparator2,
            this.退出ToolStripMenuItem});
            this.MenuItemPGM.Name = "MenuItemPGM";
            this.MenuItemPGM.Size = new System.Drawing.Size(53, 24);
            this.MenuItemPGM.Text = "程序";
            // 
            // 全局设置ToolStripMenuItem
            // 
            this.全局设置ToolStripMenuItem.Name = "全局设置ToolStripMenuItem";
            this.全局设置ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.全局设置ToolStripMenuItem.Text = "全局设置";
            this.全局设置ToolStripMenuItem.Click += new System.EventHandler(this.全局设置ToolStripMenuItem_Click);
            // 
            // 管理同步文件夹对ToolStripMenuItem
            // 
            this.管理同步文件夹对ToolStripMenuItem.Name = "管理同步文件夹对ToolStripMenuItem";
            this.管理同步文件夹对ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.管理同步文件夹对ToolStripMenuItem.Text = "管理目录配对";
            this.管理同步文件夹对ToolStripMenuItem.Click += new System.EventHandler(this.管理同步文件夹对ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // MenuItemHelp
            // 
            this.MenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.更新日志ToolStripMenuItem,
            this.关于ToolStripMenuItem1,
            this.项目Github主页ToolStripMenuItem});
            this.MenuItemHelp.Name = "MenuItemHelp";
            this.MenuItemHelp.Size = new System.Drawing.Size(53, 24);
            this.MenuItemHelp.Text = "帮助";
            // 
            // 更新日志ToolStripMenuItem
            // 
            this.更新日志ToolStripMenuItem.Name = "更新日志ToolStripMenuItem";
            this.更新日志ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.更新日志ToolStripMenuItem.Text = "更新日志";
            this.更新日志ToolStripMenuItem.Click += new System.EventHandler(this.更新日志ToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem1
            // 
            this.关于ToolStripMenuItem1.Name = "关于ToolStripMenuItem1";
            this.关于ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.关于ToolStripMenuItem1.Text = "关于";
            this.关于ToolStripMenuItem1.Click += new System.EventHandler(this.关于ToolStripMenuItem1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnStopOpr
            // 
            this.btnStopOpr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopOpr.Location = new System.Drawing.Point(436, 639);
            this.btnStopOpr.Name = "btnStopOpr";
            this.btnStopOpr.Size = new System.Drawing.Size(100, 30);
            this.btnStopOpr.TabIndex = 20;
            this.btnStopOpr.Text = "停止所有操作";
            this.btnStopOpr.UseVisualStyleBackColor = true;
            this.btnStopOpr.Click += new System.EventHandler(this.btnStopOpr_Click);
            // 
            // btnSyncAll
            // 
            this.btnSyncAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSyncAll.Location = new System.Drawing.Point(330, 639);
            this.btnSyncAll.Name = "btnSyncAll";
            this.btnSyncAll.Size = new System.Drawing.Size(100, 30);
            this.btnSyncAll.TabIndex = 21;
            this.btnSyncAll.Text = "同步所有配对";
            this.btnSyncAll.UseVisualStyleBackColor = true;
            this.btnSyncAll.Click += new System.EventHandler(this.btnSyncAll_Click);
            // 
            // btnPauseSync
            // 
            this.btnPauseSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPauseSync.Location = new System.Drawing.Point(542, 639);
            this.btnPauseSync.Name = "btnPauseSync";
            this.btnPauseSync.Size = new System.Drawing.Size(100, 30);
            this.btnPauseSync.TabIndex = 23;
            this.btnPauseSync.Text = "暂停自动同步";
            this.btnPauseSync.UseVisualStyleBackColor = true;
            this.btnPauseSync.Click += new System.EventHandler(this.btnPauseSync_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 17);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1234, 142);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1240, 162);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配对总览";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(12, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1240, 605);
            this.panel1.TabIndex = 24;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest.Location = new System.Drawing.Point(754, 639);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 30);
            this.btnTest.TabIndex = 25;
            this.btnTest.Text = "测试按钮";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnOpenPairLog
            // 
            this.btnOpenPairLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenPairLog.Location = new System.Drawing.Point(118, 639);
            this.btnOpenPairLog.Name = "btnOpenPairLog";
            this.btnOpenPairLog.Size = new System.Drawing.Size(100, 30);
            this.btnOpenPairLog.TabIndex = 26;
            this.btnOpenPairLog.Text = "打开日志";
            this.btnOpenPairLog.UseVisualStyleBackColor = true;
            this.btnOpenPairLog.Click += new System.EventHandler(this.btnOpenPairLog_Click);
            // 
            // 项目Github主页ToolStripMenuItem
            // 
            this.项目Github主页ToolStripMenuItem.Name = "项目Github主页ToolStripMenuItem";
            this.项目Github主页ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.项目Github主页ToolStripMenuItem.Text = "项目Github主页";
            this.项目Github主页ToolStripMenuItem.Click += new System.EventHandler(this.项目Github主页ToolStripMenuItem_Click);
            // 
            // frm_FileSynchronizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.btnOpenPairLog);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnPauseSync);
            this.Controls.Add(this.btnSyncAll);
            this.Controls.Add(this.btnStopOpr);
            this.Controls.Add(this.PgmMenu);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnAnalysis);
            this.Controls.Add(this.btnSync);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.PgmMenu;
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.Name = "frm_FileSynchronizer";
            this.Text = "FileSynchronizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileSynchronizer_FormClosing);
            this.Load += new System.EventHandler(this.frmFileSynchronizer_Load);
            this.SizeChanged += new System.EventHandler(this.FileSynchronizer_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.NotifyMenu.ResumeLayout(false);
            this.PgmMenu.ResumeLayout(false);
            this.PgmMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.RichTextBox TxtProgramLog;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnAnalysis;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.ContextMenuStrip NotifyMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemSyncAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.MenuStrip PgmMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemPGM;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管理同步文件夹对ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全局设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem MenuItemShowMain;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 更新日志ToolStripMenuItem;
        private System.Windows.Forms.Button btnStopOpr;
        private System.Windows.Forms.Button btnSyncAll;
        private System.Windows.Forms.Button btnPauseSync;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnOpenPairLog;
        private System.Windows.Forms.ToolStripMenuItem MenuItemSync;
        private System.Windows.Forms.ToolStripMenuItem 项目Github主页ToolStripMenuItem;
    }
}

