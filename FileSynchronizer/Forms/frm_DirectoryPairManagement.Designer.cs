
namespace FileSynchronizer
{
    partial class frm_DirectoryPairManagement
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
            dataGridView1 = new System.Windows.Forms.DataGridView();
            btnNewPair = new System.Windows.Forms.Button();
            btnDelPair = new System.Windows.Forms.Button();
            btnQuit = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();
            lblDir1 = new System.Windows.Forms.Label();
            lblDIr2 = new System.Windows.Forms.Label();
            txtBoxDir1 = new System.Windows.Forms.TextBox();
            txtBoxDir2 = new System.Windows.Forms.TextBox();
            lblPairName = new System.Windows.Forms.Label();
            txtBoxPairName = new System.Windows.Forms.TextBox();
            btnUpdPair = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            lblPairLogSize = new System.Windows.Forms.Label();
            lblPairLog = new System.Windows.Forms.Label();
            btnSelectDIR2 = new System.Windows.Forms.Button();
            btnSelectDIR1 = new System.Windows.Forms.Button();
            btnClearPairLog = new System.Windows.Forms.Button();
            btnClearDir2BK = new System.Windows.Forms.Button();
            lblDir2BackupSpace = new System.Windows.Forms.Label();
            lblDir2Backup = new System.Windows.Forms.Label();
            btnClearDir1BK = new System.Windows.Forms.Button();
            lblDir1BackupSpace = new System.Windows.Forms.Label();
            lblDir1Backup = new System.Windows.Forms.Label();
            grpboxSyncRule = new System.Windows.Forms.GroupBox();
            comboBox1 = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtBoxFilterRule = new System.Windows.Forms.TextBox();
            txtBoxSyncInterval = new System.Windows.Forms.TextBox();
            grpboxAutoSync = new System.Windows.Forms.GroupBox();
            checkBoxRealTimeSync = new System.Windows.Forms.CheckBox();
            checkBoxPauseSync = new System.Windows.Forms.CheckBox();
            comBoxSyncDay = new System.Windows.Forms.ComboBox();
            timePickerSyncTime = new System.Windows.Forms.DateTimePicker();
            radioButtonFixedTime = new System.Windows.Forms.RadioButton();
            radioButtonSyncInterval = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            grpboxSyncRule.SuspendLayout();
            grpboxAutoSync.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(14, 17);
            dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 23;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(840, 332);
            dataGridView1.TabIndex = 0;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // btnNewPair
            // 
            btnNewPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnNewPair.Location = new System.Drawing.Point(158, 606);
            btnNewPair.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnNewPair.Name = "btnNewPair";
            btnNewPair.Size = new System.Drawing.Size(146, 36);
            btnNewPair.TabIndex = 1;
            btnNewPair.Text = "新增文件夹配对";
            btnNewPair.UseVisualStyleBackColor = true;
            btnNewPair.Click += btnNewPair_Click;
            // 
            // btnDelPair
            // 
            btnDelPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDelPair.Location = new System.Drawing.Point(462, 606);
            btnDelPair.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnDelPair.Name = "btnDelPair";
            btnDelPair.Size = new System.Drawing.Size(146, 36);
            btnDelPair.TabIndex = 2;
            btnDelPair.Text = "删除文件夹配对";
            btnDelPair.UseVisualStyleBackColor = true;
            btnDelPair.Click += btnDelPair_Click;
            // 
            // btnQuit
            // 
            btnQuit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnQuit.Location = new System.Drawing.Point(739, 606);
            btnQuit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new System.Drawing.Size(117, 36);
            btnQuit.TabIndex = 3;
            btnQuit.Text = "关闭";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRefresh.Location = new System.Drawing.Point(616, 606);
            btnRefresh.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(117, 36);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblDir1
            // 
            lblDir1.AutoSize = true;
            lblDir1.Location = new System.Drawing.Point(7, 51);
            lblDir1.Name = "lblDir1";
            lblDir1.Size = new System.Drawing.Size(63, 17);
            lblDir1.TabIndex = 5;
            lblDir1.Text = "目录1路径";
            // 
            // lblDIr2
            // 
            lblDIr2.AutoSize = true;
            lblDIr2.Location = new System.Drawing.Point(7, 133);
            lblDIr2.Name = "lblDIr2";
            lblDIr2.Size = new System.Drawing.Size(63, 17);
            lblDIr2.TabIndex = 6;
            lblDIr2.Text = "目录2路径";
            // 
            // txtBoxDir1
            // 
            txtBoxDir1.Location = new System.Drawing.Point(82, 47);
            txtBoxDir1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir1.MaxLength = 255;
            txtBoxDir1.Name = "txtBoxDir1";
            txtBoxDir1.Size = new System.Drawing.Size(356, 23);
            txtBoxDir1.TabIndex = 7;
            // 
            // txtBoxDir2
            // 
            txtBoxDir2.Location = new System.Drawing.Point(82, 129);
            txtBoxDir2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir2.MaxLength = 255;
            txtBoxDir2.Name = "txtBoxDir2";
            txtBoxDir2.Size = new System.Drawing.Size(356, 23);
            txtBoxDir2.TabIndex = 8;
            // 
            // lblPairName
            // 
            lblPairName.AutoSize = true;
            lblPairName.Location = new System.Drawing.Point(7, 13);
            lblPairName.Name = "lblPairName";
            lblPairName.Size = new System.Drawing.Size(56, 17);
            lblPairName.TabIndex = 9;
            lblPairName.Text = "配对名称";
            // 
            // txtBoxPairName
            // 
            txtBoxPairName.Location = new System.Drawing.Point(82, 8);
            txtBoxPairName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxPairName.MaxLength = 50;
            txtBoxPairName.Name = "txtBoxPairName";
            txtBoxPairName.Size = new System.Drawing.Size(124, 23);
            txtBoxPairName.TabIndex = 10;
            // 
            // btnUpdPair
            // 
            btnUpdPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnUpdPair.Location = new System.Drawing.Point(310, 606);
            btnUpdPair.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnUpdPair.Name = "btnUpdPair";
            btnUpdPair.Size = new System.Drawing.Size(146, 36);
            btnUpdPair.TabIndex = 11;
            btnUpdPair.Text = "更改文件夹配对";
            btnUpdPair.UseVisualStyleBackColor = true;
            btnUpdPair.Click += btnUpdPair_Click;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.Controls.Add(lblPairLogSize);
            panel1.Controls.Add(lblPairLog);
            panel1.Controls.Add(btnSelectDIR2);
            panel1.Controls.Add(btnSelectDIR1);
            panel1.Controls.Add(btnClearPairLog);
            panel1.Controls.Add(btnClearDir2BK);
            panel1.Controls.Add(lblDir2BackupSpace);
            panel1.Controls.Add(lblDir2Backup);
            panel1.Controls.Add(btnClearDir1BK);
            panel1.Controls.Add(lblDir1BackupSpace);
            panel1.Controls.Add(lblDir1Backup);
            panel1.Controls.Add(lblPairName);
            panel1.Controls.Add(lblDir1);
            panel1.Controls.Add(txtBoxPairName);
            panel1.Controls.Add(lblDIr2);
            panel1.Controls.Add(txtBoxDir1);
            panel1.Controls.Add(txtBoxDir2);
            panel1.Location = new System.Drawing.Point(14, 357);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(488, 243);
            panel1.TabIndex = 12;
            // 
            // lblPairLogSize
            // 
            lblPairLogSize.AutoSize = true;
            lblPairLogSize.Location = new System.Drawing.Point(311, 13);
            lblPairLogSize.Name = "lblPairLogSize";
            lblPairLogSize.Size = new System.Drawing.Size(68, 17);
            lblPairLogSize.TabIndex = 22;
            lblPairLogSize.Text = "(PAIRLOG)";
            // 
            // lblPairLog
            // 
            lblPairLog.AutoSize = true;
            lblPairLog.Location = new System.Drawing.Point(215, 13);
            lblPairLog.Name = "lblPairLog";
            lblPairLog.Size = new System.Drawing.Size(80, 17);
            lblPairLog.TabIndex = 21;
            lblPairLog.Text = "配对日志文件";
            // 
            // btnSelectDIR2
            // 
            btnSelectDIR2.Location = new System.Drawing.Point(446, 129);
            btnSelectDIR2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSelectDIR2.Name = "btnSelectDIR2";
            btnSelectDIR2.Size = new System.Drawing.Size(37, 30);
            btnSelectDIR2.TabIndex = 20;
            btnSelectDIR2.Text = "...";
            btnSelectDIR2.UseVisualStyleBackColor = true;
            btnSelectDIR2.Click += btnSelectDIR2_Click;
            // 
            // btnSelectDIR1
            // 
            btnSelectDIR1.Location = new System.Drawing.Point(446, 47);
            btnSelectDIR1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSelectDIR1.Name = "btnSelectDIR1";
            btnSelectDIR1.Size = new System.Drawing.Size(37, 30);
            btnSelectDIR1.TabIndex = 19;
            btnSelectDIR1.Text = "...";
            btnSelectDIR1.UseVisualStyleBackColor = true;
            btnSelectDIR1.Click += btnSelectDIR1_Click;
            // 
            // btnClearPairLog
            // 
            btnClearPairLog.Location = new System.Drawing.Point(390, 4);
            btnClearPairLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClearPairLog.Name = "btnClearPairLog";
            btnClearPairLog.Size = new System.Drawing.Size(93, 36);
            btnClearPairLog.TabIndex = 18;
            btnClearPairLog.Text = "清除日志";
            btnClearPairLog.UseVisualStyleBackColor = true;
            btnClearPairLog.Click += btnClearPairLog_Click;
            // 
            // btnClearDir2BK
            // 
            btnClearDir2BK.Location = new System.Drawing.Point(362, 167);
            btnClearDir2BK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClearDir2BK.Name = "btnClearDir2BK";
            btnClearDir2BK.Size = new System.Drawing.Size(121, 36);
            btnClearDir2BK.TabIndex = 17;
            btnClearDir2BK.Text = "清理备份空间";
            btnClearDir2BK.UseVisualStyleBackColor = true;
            btnClearDir2BK.Click += btnClearDir2BK_Click;
            // 
            // lblDir2BackupSpace
            // 
            lblDir2BackupSpace.AutoSize = true;
            lblDir2BackupSpace.Location = new System.Drawing.Point(110, 176);
            lblDir2BackupSpace.Name = "lblDir2BackupSpace";
            lblDir2BackupSpace.Size = new System.Drawing.Size(97, 17);
            lblDir2BackupSpace.TabIndex = 16;
            lblDir2BackupSpace.Text = "(DIR2BKSPACE)";
            // 
            // lblDir2Backup
            // 
            lblDir2Backup.AutoSize = true;
            lblDir2Backup.Location = new System.Drawing.Point(7, 176);
            lblDir2Backup.Name = "lblDir2Backup";
            lblDir2Backup.Size = new System.Drawing.Size(87, 17);
            lblDir2Backup.TabIndex = 15;
            lblDir2Backup.Text = "目录2备份空间";
            // 
            // btnClearDir1BK
            // 
            btnClearDir1BK.Location = new System.Drawing.Point(362, 85);
            btnClearDir1BK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClearDir1BK.Name = "btnClearDir1BK";
            btnClearDir1BK.Size = new System.Drawing.Size(121, 36);
            btnClearDir1BK.TabIndex = 14;
            btnClearDir1BK.Text = "清理备份空间";
            btnClearDir1BK.UseVisualStyleBackColor = true;
            btnClearDir1BK.Click += btnClearDir1BK_Click;
            // 
            // lblDir1BackupSpace
            // 
            lblDir1BackupSpace.AutoSize = true;
            lblDir1BackupSpace.Location = new System.Drawing.Point(110, 94);
            lblDir1BackupSpace.Name = "lblDir1BackupSpace";
            lblDir1BackupSpace.Size = new System.Drawing.Size(97, 17);
            lblDir1BackupSpace.TabIndex = 12;
            lblDir1BackupSpace.Text = "(DIR1BKSPACE)";
            // 
            // lblDir1Backup
            // 
            lblDir1Backup.AutoSize = true;
            lblDir1Backup.Location = new System.Drawing.Point(7, 94);
            lblDir1Backup.Name = "lblDir1Backup";
            lblDir1Backup.Size = new System.Drawing.Size(87, 17);
            lblDir1Backup.TabIndex = 11;
            lblDir1Backup.Text = "目录1备份空间";
            // 
            // grpboxSyncRule
            // 
            grpboxSyncRule.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            grpboxSyncRule.Controls.Add(comboBox1);
            grpboxSyncRule.Controls.Add(label3);
            grpboxSyncRule.Controls.Add(label2);
            grpboxSyncRule.Controls.Add(txtBoxFilterRule);
            grpboxSyncRule.Location = new System.Drawing.Point(508, 357);
            grpboxSyncRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Name = "grpboxSyncRule";
            grpboxSyncRule.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Size = new System.Drawing.Size(346, 136);
            grpboxSyncRule.TabIndex = 13;
            grpboxSyncRule.TabStop = false;
            grpboxSyncRule.Text = "同步规则";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "目录1<->目录2（双向同步并删除）", "目录1<->目录2（双向同步不删除）", "目录1->目录2（单向同步并删除）", "目录1->目录2（单向同步不删除）", "目录2->目录1（单向同步并删除）", "目录2->目录1（单向同步不删除）" });
            comboBox1.Location = new System.Drawing.Point(76, 105);
            comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(264, 25);
            comboBox1.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(7, 109);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(56, 17);
            label3.TabIndex = 13;
            label3.Text = "同步方向";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 20);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(232, 17);
            label2.TabIndex = 11;
            label2.Text = "排除规则(不同关键字之间用英文逗号隔开)";
            // 
            // txtBoxFilterRule
            // 
            txtBoxFilterRule.Location = new System.Drawing.Point(9, 41);
            txtBoxFilterRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxFilterRule.MaxLength = 255;
            txtBoxFilterRule.Multiline = true;
            txtBoxFilterRule.Name = "txtBoxFilterRule";
            txtBoxFilterRule.Size = new System.Drawing.Size(331, 56);
            txtBoxFilterRule.TabIndex = 12;
            // 
            // txtBoxSyncInterval
            // 
            txtBoxSyncInterval.Location = new System.Drawing.Point(158, 48);
            txtBoxSyncInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxSyncInterval.MaxLength = 255;
            txtBoxSyncInterval.Name = "txtBoxSyncInterval";
            txtBoxSyncInterval.Size = new System.Drawing.Size(67, 23);
            txtBoxSyncInterval.TabIndex = 10;
            // 
            // grpboxAutoSync
            // 
            grpboxAutoSync.Controls.Add(checkBoxRealTimeSync);
            grpboxAutoSync.Controls.Add(checkBoxPauseSync);
            grpboxAutoSync.Controls.Add(comBoxSyncDay);
            grpboxAutoSync.Controls.Add(timePickerSyncTime);
            grpboxAutoSync.Controls.Add(radioButtonFixedTime);
            grpboxAutoSync.Controls.Add(radioButtonSyncInterval);
            grpboxAutoSync.Controls.Add(txtBoxSyncInterval);
            grpboxAutoSync.Location = new System.Drawing.Point(508, 500);
            grpboxAutoSync.Name = "grpboxAutoSync";
            grpboxAutoSync.Size = new System.Drawing.Size(346, 101);
            grpboxAutoSync.TabIndex = 14;
            grpboxAutoSync.TabStop = false;
            grpboxAutoSync.Text = "自动同步设置";
            // 
            // checkBoxRealTimeSync
            // 
            checkBoxRealTimeSync.AutoSize = true;
            checkBoxRealTimeSync.Location = new System.Drawing.Point(7, 22);
            checkBoxRealTimeSync.Name = "checkBoxRealTimeSync";
            checkBoxRealTimeSync.Size = new System.Drawing.Size(75, 21);
            checkBoxRealTimeSync.TabIndex = 19;
            checkBoxRealTimeSync.Text = "实时同步";
            checkBoxRealTimeSync.UseVisualStyleBackColor = true;
            checkBoxRealTimeSync.CheckedChanged += checkBoxRealTimeSync_CheckedChanged;
            // 
            // checkBoxPauseSync
            // 
            checkBoxPauseSync.AutoSize = true;
            checkBoxPauseSync.Location = new System.Drawing.Point(88, 22);
            checkBoxPauseSync.Name = "checkBoxPauseSync";
            checkBoxPauseSync.Size = new System.Drawing.Size(143, 21);
            checkBoxPauseSync.TabIndex = 18;
            checkBoxPauseSync.Text = "不自动同步(暂停同步)";
            checkBoxPauseSync.UseVisualStyleBackColor = true;
            // 
            // comBoxSyncDay
            // 
            comBoxSyncDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comBoxSyncDay.FormattingEnabled = true;
            comBoxSyncDay.Items.AddRange(new object[] { "每天", "每周一", "每周二", "每周三", "每周四", "每周五", "每周六", "每周日" });
            comBoxSyncDay.Location = new System.Drawing.Point(108, 75);
            comBoxSyncDay.Name = "comBoxSyncDay";
            comBoxSyncDay.Size = new System.Drawing.Size(117, 25);
            comBoxSyncDay.TabIndex = 16;
            // 
            // timePickerSyncTime
            // 
            timePickerSyncTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            timePickerSyncTime.Location = new System.Drawing.Point(231, 75);
            timePickerSyncTime.Name = "timePickerSyncTime";
            timePickerSyncTime.ShowUpDown = true;
            timePickerSyncTime.Size = new System.Drawing.Size(109, 23);
            timePickerSyncTime.TabIndex = 15;
            // 
            // radioButtonFixedTime
            // 
            radioButtonFixedTime.AutoSize = true;
            radioButtonFixedTime.Location = new System.Drawing.Point(6, 76);
            radioButtonFixedTime.Name = "radioButtonFixedTime";
            radioButtonFixedTime.Size = new System.Drawing.Size(98, 21);
            radioButtonFixedTime.TabIndex = 12;
            radioButtonFixedTime.TabStop = true;
            radioButtonFixedTime.Text = "定时同步，于";
            radioButtonFixedTime.UseVisualStyleBackColor = true;
            radioButtonFixedTime.CheckedChanged += radioButtonFixedTime_CheckedChanged;
            // 
            // radioButtonSyncInterval
            // 
            radioButtonSyncInterval.AutoSize = true;
            radioButtonSyncInterval.Location = new System.Drawing.Point(6, 49);
            radioButtonSyncInterval.Name = "radioButtonSyncInterval";
            radioButtonSyncInterval.Size = new System.Drawing.Size(146, 21);
            radioButtonSyncInterval.TabIndex = 11;
            radioButtonSyncInterval.TabStop = true;
            radioButtonSyncInterval.Text = "自动同步间隔（分钟）";
            radioButtonSyncInterval.UseVisualStyleBackColor = true;
            radioButtonSyncInterval.CheckedChanged += radioButtonSyncInterval_CheckedChanged;
            // 
            // frm_DirectoryPairManagement
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnQuit;
            ClientSize = new System.Drawing.Size(868, 653);
            Controls.Add(grpboxAutoSync);
            Controls.Add(grpboxSyncRule);
            Controls.Add(panel1);
            Controls.Add(btnUpdPair);
            Controls.Add(btnRefresh);
            Controls.Add(btnQuit);
            Controls.Add(btnDelPair);
            Controls.Add(btnNewPair);
            Controls.Add(dataGridView1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frm_DirectoryPairManagement";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "配对管理";
            FormClosing += DirectoryPairManager_FormClosing;
            Load += DirectoryPairManager_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            grpboxSyncRule.ResumeLayout(false);
            grpboxSyncRule.PerformLayout();
            grpboxAutoSync.ResumeLayout(false);
            grpboxAutoSync.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnNewPair;
        private System.Windows.Forms.Button btnDelPair;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblDir1;
        private System.Windows.Forms.Label lblDIr2;
        private System.Windows.Forms.TextBox txtBoxDir1;
        private System.Windows.Forms.TextBox txtBoxDir2;
        private System.Windows.Forms.Label lblPairName;
        private System.Windows.Forms.TextBox txtBoxPairName;
        private System.Windows.Forms.Button btnUpdPair;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grpboxSyncRule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxFilterRule;
        private System.Windows.Forms.TextBox txtBoxSyncInterval;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDir1BackupSpace;
        private System.Windows.Forms.Label lblDir1Backup;
        private System.Windows.Forms.Button btnClearDir2BK;
        private System.Windows.Forms.Label lblDir2BackupSpace;
        private System.Windows.Forms.Label lblDir2Backup;
        private System.Windows.Forms.Button btnClearDir1BK;
        private System.Windows.Forms.Button btnClearPairLog;
        private System.Windows.Forms.Button btnSelectDIR2;
        private System.Windows.Forms.Button btnSelectDIR1;
        private System.Windows.Forms.Label lblPairLogSize;
        private System.Windows.Forms.Label lblPairLog;
        private System.Windows.Forms.GroupBox grpboxAutoSync;
        private System.Windows.Forms.RadioButton radioButtonSyncInterval;
        private System.Windows.Forms.RadioButton radioButtonFixedTime;
        private System.Windows.Forms.DateTimePicker timePickerSyncTime;
        private System.Windows.Forms.ComboBox comBoxSyncDay;
        private System.Windows.Forms.CheckBox checkBoxPauseSync;
        private System.Windows.Forms.CheckBox checkBoxRealTimeSync;
    }
}