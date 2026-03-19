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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_FileSynchronizer));
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPageMain = new System.Windows.Forms.TabPage();
            TxtProgramLog = new System.Windows.Forms.RichTextBox();
            btnSync = new System.Windows.Forms.Button();
            btnAnalysis = new System.Windows.Forms.Button();
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            NotifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            MenuItemShowMain = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemSync = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemSyncAll = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            btnClearLog = new System.Windows.Forms.Button();
            PgmMenu = new System.Windows.Forms.MenuStrip();
            MenuItemPGM = new System.Windows.Forms.ToolStripMenuItem();
            全局设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            管理同步文件夹对ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            更新日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            关于ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            项目Github主页ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            检查更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            btnStopOpr = new System.Windows.Forms.Button();
            btnSyncAll = new System.Windows.Forms.Button();
            btnPauseSync = new System.Windows.Forms.Button();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel1 = new System.Windows.Forms.Panel();
            btnTest = new System.Windows.Forms.Button();
            btnOpenPairLog = new System.Windows.Forms.Button();
            timerAutoUpdate = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            tabPageMain.SuspendLayout();
            NotifyMenu.SuspendLayout();
            PgmMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageMain);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            tabControl1.Location = new System.Drawing.Point(0, 230);
            tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1055, 411);
            tabControl1.TabIndex = 3;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPageMain
            // 
            tabPageMain.Controls.Add(TxtProgramLog);
            tabPageMain.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            tabPageMain.Location = new System.Drawing.Point(4, 24);
            tabPageMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabPageMain.Name = "tabPageMain";
            tabPageMain.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabPageMain.Size = new System.Drawing.Size(1047, 383);
            tabPageMain.TabIndex = 0;
            tabPageMain.Text = "程序日志";
            tabPageMain.UseVisualStyleBackColor = true;
            // 
            // TxtProgramLog
            // 
            TxtProgramLog.BackColor = System.Drawing.SystemColors.Info;
            TxtProgramLog.Dock = System.Windows.Forms.DockStyle.Fill;
            TxtProgramLog.Location = new System.Drawing.Point(3, 4);
            TxtProgramLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TxtProgramLog.Name = "TxtProgramLog";
            TxtProgramLog.ReadOnly = true;
            TxtProgramLog.Size = new System.Drawing.Size(1041, 375);
            TxtProgramLog.TabIndex = 0;
            TxtProgramLog.TabStop = false;
            TxtProgramLog.Text = "";
            // 
            // btnSync
            // 
            btnSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnSync.Location = new System.Drawing.Point(240, 679);
            btnSync.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSync.Name = "btnSync";
            btnSync.Size = new System.Drawing.Size(109, 34);
            btnSync.TabIndex = 4;
            btnSync.Text = "同步当前配对";
            btnSync.UseVisualStyleBackColor = true;
            btnSync.Click += btnSync_Click;
            // 
            // btnAnalysis
            // 
            btnAnalysis.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAnalysis.Location = new System.Drawing.Point(701, 679);
            btnAnalysis.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnAnalysis.Name = "btnAnalysis";
            btnAnalysis.Size = new System.Drawing.Size(109, 34);
            btnAnalysis.TabIndex = 5;
            btnAnalysis.Text = "分析当前配对";
            btnAnalysis.UseVisualStyleBackColor = true;
            btnAnalysis.Click += btnAnalysis_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = NotifyMenu;
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "FileSynchronizer";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // NotifyMenu
            // 
            NotifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            NotifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuItemShowMain, MenuItemSync, MenuItemSyncAll, toolStripSeparator1, MenuItemExit });
            NotifyMenu.Name = "NotifyMenu";
            NotifyMenu.Size = new System.Drawing.Size(137, 98);
            // 
            // MenuItemShowMain
            // 
            MenuItemShowMain.Name = "MenuItemShowMain";
            MenuItemShowMain.Size = new System.Drawing.Size(136, 22);
            MenuItemShowMain.Text = "显示主界面";
            MenuItemShowMain.Click += MenuItemShowMain_Click;
            MenuItemShowMain.MouseEnter += MenuItemOther_MouseEnter;
            // 
            // MenuItemSync
            // 
            MenuItemSync.Name = "MenuItemSync";
            MenuItemSync.Size = new System.Drawing.Size(136, 22);
            MenuItemSync.Text = "同步";
            MenuItemSync.MouseEnter += MenuItemSync_MouseEnter;
            // 
            // MenuItemSyncAll
            // 
            MenuItemSyncAll.Name = "MenuItemSyncAll";
            MenuItemSyncAll.Size = new System.Drawing.Size(136, 22);
            MenuItemSyncAll.Text = "同步所有";
            MenuItemSyncAll.Click += MenuItemSyncAll_Click;
            MenuItemSyncAll.MouseEnter += MenuItemOther_MouseEnter;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // MenuItemExit
            // 
            MenuItemExit.Name = "MenuItemExit";
            MenuItemExit.Size = new System.Drawing.Size(136, 22);
            MenuItemExit.Text = "退出程序";
            MenuItemExit.Click += MenuItemExit_Click;
            // 
            // btnClearLog
            // 
            btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearLog.Location = new System.Drawing.Point(10, 679);
            btnClearLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new System.Drawing.Size(109, 34);
            btnClearLog.TabIndex = 17;
            btnClearLog.Text = "清空日志";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += btnClearLog_Click;
            // 
            // PgmMenu
            // 
            PgmMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            PgmMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuItemPGM, MenuItemHelp });
            PgmMenu.Location = new System.Drawing.Point(0, 0);
            PgmMenu.Name = "PgmMenu";
            PgmMenu.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            PgmMenu.Size = new System.Drawing.Size(1076, 27);
            PgmMenu.TabIndex = 18;
            PgmMenu.Text = "PgmMenu";
            // 
            // MenuItemPGM
            // 
            MenuItemPGM.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { 全局设置ToolStripMenuItem, 管理同步文件夹对ToolStripMenuItem, toolStripSeparator2, 退出ToolStripMenuItem });
            MenuItemPGM.Name = "MenuItemPGM";
            MenuItemPGM.Size = new System.Drawing.Size(44, 21);
            MenuItemPGM.Text = "程序";
            // 
            // 全局设置ToolStripMenuItem
            // 
            全局设置ToolStripMenuItem.Name = "全局设置ToolStripMenuItem";
            全局设置ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            全局设置ToolStripMenuItem.Text = "全局设置";
            全局设置ToolStripMenuItem.Click += 全局设置ToolStripMenuItem_Click;
            // 
            // 管理同步文件夹对ToolStripMenuItem
            // 
            管理同步文件夹对ToolStripMenuItem.Name = "管理同步文件夹对ToolStripMenuItem";
            管理同步文件夹对ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            管理同步文件夹对ToolStripMenuItem.Text = "管理目录配对";
            管理同步文件夹对ToolStripMenuItem.Click += 管理同步文件夹对ToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(145, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // MenuItemHelp
            // 
            MenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { 更新日志ToolStripMenuItem, 关于ToolStripMenuItem1, 项目Github主页ToolStripMenuItem, 检查更新ToolStripMenuItem });
            MenuItemHelp.Name = "MenuItemHelp";
            MenuItemHelp.Size = new System.Drawing.Size(44, 21);
            MenuItemHelp.Text = "帮助";
            // 
            // 更新日志ToolStripMenuItem
            // 
            更新日志ToolStripMenuItem.Name = "更新日志ToolStripMenuItem";
            更新日志ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            更新日志ToolStripMenuItem.Text = "更新日志";
            更新日志ToolStripMenuItem.Click += 更新日志ToolStripMenuItem_Click;
            // 
            // 关于ToolStripMenuItem1
            // 
            关于ToolStripMenuItem1.Name = "关于ToolStripMenuItem1";
            关于ToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            关于ToolStripMenuItem1.Text = "关于";
            关于ToolStripMenuItem1.Click += 关于ToolStripMenuItem1_Click;
            // 
            // 项目Github主页ToolStripMenuItem
            // 
            项目Github主页ToolStripMenuItem.Name = "项目Github主页ToolStripMenuItem";
            项目Github主页ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            项目Github主页ToolStripMenuItem.Text = "项目Github主页";
            项目Github主页ToolStripMenuItem.Click += 项目Github主页ToolStripMenuItem_Click;
            // 
            // 检查更新ToolStripMenuItem
            // 
            检查更新ToolStripMenuItem.Name = "检查更新ToolStripMenuItem";
            检查更新ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            检查更新ToolStripMenuItem.Text = "检查更新";
            检查更新ToolStripMenuItem.Click += 检查更新ToolStripMenuItem_Click;
            // 
            // btnStopOpr
            // 
            btnStopOpr.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnStopOpr.Location = new System.Drawing.Point(471, 679);
            btnStopOpr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStopOpr.Name = "btnStopOpr";
            btnStopOpr.Size = new System.Drawing.Size(109, 34);
            btnStopOpr.TabIndex = 20;
            btnStopOpr.Text = "停止所有操作";
            btnStopOpr.UseVisualStyleBackColor = true;
            btnStopOpr.Click += btnStopOpr_Click;
            // 
            // btnSyncAll
            // 
            btnSyncAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnSyncAll.Location = new System.Drawing.Point(355, 679);
            btnSyncAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSyncAll.Name = "btnSyncAll";
            btnSyncAll.Size = new System.Drawing.Size(109, 34);
            btnSyncAll.TabIndex = 21;
            btnSyncAll.Text = "同步所有配对";
            btnSyncAll.UseVisualStyleBackColor = true;
            btnSyncAll.Click += btnSyncAll_Click;
            // 
            // btnPauseSync
            // 
            btnPauseSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnPauseSync.Location = new System.Drawing.Point(586, 679);
            btnPauseSync.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnPauseSync.Name = "btnPauseSync";
            btnPauseSync.Size = new System.Drawing.Size(109, 34);
            btnPauseSync.TabIndex = 23;
            btnPauseSync.Text = "暂停自动同步";
            btnPauseSync.UseVisualStyleBackColor = true;
            btnPauseSync.Click += btnPauseSync_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(3, 20);
            dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView1.RowTemplate.Height = 23;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(1049, 206);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellMouseDoubleClick += dataGridView1_CellMouseDoubleClick;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dataGridView1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(1055, 230);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "配对总览";
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.Controls.Add(tabControl1);
            panel1.Controls.Add(groupBox1);
            panel1.Location = new System.Drawing.Point(10, 30);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1055, 641);
            panel1.TabIndex = 24;
            // 
            // btnTest
            // 
            btnTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnTest.Location = new System.Drawing.Point(816, 679);
            btnTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnTest.Name = "btnTest";
            btnTest.Size = new System.Drawing.Size(109, 34);
            btnTest.TabIndex = 25;
            btnTest.Text = "测试按钮";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnOpenPairLog
            // 
            btnOpenPairLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnOpenPairLog.Location = new System.Drawing.Point(125, 679);
            btnOpenPairLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOpenPairLog.Name = "btnOpenPairLog";
            btnOpenPairLog.Size = new System.Drawing.Size(109, 34);
            btnOpenPairLog.TabIndex = 26;
            btnOpenPairLog.Text = "打开日志";
            btnOpenPairLog.UseVisualStyleBackColor = true;
            btnOpenPairLog.Click += btnOpenPairLog_Click;
            // 
            // timerAutoUpdate
            // 
            timerAutoUpdate.Tick += timerAutoUpdate_Tick;
            // 
            // frm_FileSynchronizer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(1076, 732);
            Controls.Add(btnOpenPairLog);
            Controls.Add(btnTest);
            Controls.Add(panel1);
            Controls.Add(btnPauseSync);
            Controls.Add(btnSyncAll);
            Controls.Add(btnStopOpr);
            Controls.Add(PgmMenu);
            Controls.Add(btnClearLog);
            Controls.Add(btnAnalysis);
            Controls.Add(btnSync);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = PgmMenu;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimumSize = new System.Drawing.Size(1092, 771);
            Name = "frm_FileSynchronizer";
            Text = "FileSynchronizer";
            FormClosing += FileSynchronizer_FormClosing;
            Load += frmFileSynchronizer_Load;
            SizeChanged += FileSynchronizer_SizeChanged;
            tabControl1.ResumeLayout(false);
            tabPageMain.ResumeLayout(false);
            NotifyMenu.ResumeLayout(false);
            PgmMenu.ResumeLayout(false);
            PgmMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem 检查更新ToolStripMenuItem;
        private System.Windows.Forms.Timer timerAutoUpdate;
    }
}

