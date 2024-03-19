
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnNewPair = new System.Windows.Forms.Button();
            this.btnDelPair = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblDir1 = new System.Windows.Forms.Label();
            this.lblDIr2 = new System.Windows.Forms.Label();
            this.txtBoxDir1 = new System.Windows.Forms.TextBox();
            this.txtBoxDir2 = new System.Windows.Forms.TextBox();
            this.lblPairName = new System.Windows.Forms.Label();
            this.txtBoxPairName = new System.Windows.Forms.TextBox();
            this.btnUpdPair = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPairLogSize = new System.Windows.Forms.Label();
            this.lblPairLog = new System.Windows.Forms.Label();
            this.btnSelectDIR2 = new System.Windows.Forms.Button();
            this.btnSelectDIR1 = new System.Windows.Forms.Button();
            this.btnClearPairLog = new System.Windows.Forms.Button();
            this.btnClearDir2BK = new System.Windows.Forms.Button();
            this.lblDir2BackupSpace = new System.Windows.Forms.Label();
            this.lblDir2Backup = new System.Windows.Forms.Label();
            this.btnClearDir1BK = new System.Windows.Forms.Button();
            this.lblDir1BackupSpace = new System.Windows.Forms.Label();
            this.lblDir1Backup = new System.Windows.Forms.Label();
            this.grpboxSyncRule = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxFilterRule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxSyncInterval = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.grpboxSyncRule.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(720, 253);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // btnNewPair
            // 
            this.btnNewPair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewPair.Location = new System.Drawing.Point(133, 424);
            this.btnNewPair.Name = "btnNewPair";
            this.btnNewPair.Size = new System.Drawing.Size(125, 25);
            this.btnNewPair.TabIndex = 1;
            this.btnNewPair.Text = "新增文件夹配对";
            this.btnNewPair.UseVisualStyleBackColor = true;
            this.btnNewPair.Click += new System.EventHandler(this.btnNewPair_Click);
            // 
            // btnDelPair
            // 
            this.btnDelPair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelPair.Location = new System.Drawing.Point(395, 424);
            this.btnDelPair.Name = "btnDelPair";
            this.btnDelPair.Size = new System.Drawing.Size(125, 25);
            this.btnDelPair.TabIndex = 2;
            this.btnDelPair.Text = "删除文件夹配对";
            this.btnDelPair.UseVisualStyleBackColor = true;
            this.btnDelPair.Click += new System.EventHandler(this.btnDelPair_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(632, 424);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(100, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "关闭";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(526, 424);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 25);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblDir1
            // 
            this.lblDir1.AutoSize = true;
            this.lblDir1.Location = new System.Drawing.Point(6, 36);
            this.lblDir1.Name = "lblDir1";
            this.lblDir1.Size = new System.Drawing.Size(59, 12);
            this.lblDir1.TabIndex = 5;
            this.lblDir1.Text = "目录1路径";
            // 
            // lblDIr2
            // 
            this.lblDIr2.AutoSize = true;
            this.lblDIr2.Location = new System.Drawing.Point(6, 94);
            this.lblDIr2.Name = "lblDIr2";
            this.lblDIr2.Size = new System.Drawing.Size(59, 12);
            this.lblDIr2.TabIndex = 6;
            this.lblDIr2.Text = "目录2路径";
            // 
            // txtBoxDir1
            // 
            this.txtBoxDir1.Location = new System.Drawing.Point(71, 33);
            this.txtBoxDir1.MaxLength = 255;
            this.txtBoxDir1.Name = "txtBoxDir1";
            this.txtBoxDir1.Size = new System.Drawing.Size(306, 21);
            this.txtBoxDir1.TabIndex = 7;
            // 
            // txtBoxDir2
            // 
            this.txtBoxDir2.Location = new System.Drawing.Point(71, 91);
            this.txtBoxDir2.MaxLength = 255;
            this.txtBoxDir2.Name = "txtBoxDir2";
            this.txtBoxDir2.Size = new System.Drawing.Size(306, 21);
            this.txtBoxDir2.TabIndex = 8;
            // 
            // lblPairName
            // 
            this.lblPairName.AutoSize = true;
            this.lblPairName.Location = new System.Drawing.Point(6, 9);
            this.lblPairName.Name = "lblPairName";
            this.lblPairName.Size = new System.Drawing.Size(53, 12);
            this.lblPairName.TabIndex = 9;
            this.lblPairName.Text = "配对名称";
            // 
            // txtBoxPairName
            // 
            this.txtBoxPairName.Location = new System.Drawing.Point(71, 6);
            this.txtBoxPairName.MaxLength = 50;
            this.txtBoxPairName.Name = "txtBoxPairName";
            this.txtBoxPairName.Size = new System.Drawing.Size(107, 21);
            this.txtBoxPairName.TabIndex = 10;
            // 
            // btnUpdPair
            // 
            this.btnUpdPair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdPair.Location = new System.Drawing.Point(264, 424);
            this.btnUpdPair.Name = "btnUpdPair";
            this.btnUpdPair.Size = new System.Drawing.Size(125, 25);
            this.btnUpdPair.TabIndex = 11;
            this.btnUpdPair.Text = "更改文件夹配对";
            this.btnUpdPair.UseVisualStyleBackColor = true;
            this.btnUpdPair.Click += new System.EventHandler(this.btnUpdPair_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblPairLogSize);
            this.panel1.Controls.Add(this.lblPairLog);
            this.panel1.Controls.Add(this.btnSelectDIR2);
            this.panel1.Controls.Add(this.btnSelectDIR1);
            this.panel1.Controls.Add(this.btnClearPairLog);
            this.panel1.Controls.Add(this.btnClearDir2BK);
            this.panel1.Controls.Add(this.lblDir2BackupSpace);
            this.panel1.Controls.Add(this.lblDir2Backup);
            this.panel1.Controls.Add(this.btnClearDir1BK);
            this.panel1.Controls.Add(this.lblDir1BackupSpace);
            this.panel1.Controls.Add(this.lblDir1Backup);
            this.panel1.Controls.Add(this.lblPairName);
            this.panel1.Controls.Add(this.lblDir1);
            this.panel1.Controls.Add(this.txtBoxPairName);
            this.panel1.Controls.Add(this.lblDIr2);
            this.panel1.Controls.Add(this.txtBoxDir1);
            this.panel1.Controls.Add(this.txtBoxDir2);
            this.panel1.Location = new System.Drawing.Point(12, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(418, 147);
            this.panel1.TabIndex = 12;
            // 
            // lblPairLogSize
            // 
            this.lblPairLogSize.AutoSize = true;
            this.lblPairLogSize.Location = new System.Drawing.Point(267, 9);
            this.lblPairLogSize.Name = "lblPairLogSize";
            this.lblPairLogSize.Size = new System.Drawing.Size(59, 12);
            this.lblPairLogSize.TabIndex = 22;
            this.lblPairLogSize.Text = "(PAIRLOG)";
            // 
            // lblPairLog
            // 
            this.lblPairLog.AutoSize = true;
            this.lblPairLog.Location = new System.Drawing.Point(184, 9);
            this.lblPairLog.Name = "lblPairLog";
            this.lblPairLog.Size = new System.Drawing.Size(77, 12);
            this.lblPairLog.TabIndex = 21;
            this.lblPairLog.Text = "配对日志文件";
            // 
            // btnSelectDIR2
            // 
            this.btnSelectDIR2.Location = new System.Drawing.Point(383, 91);
            this.btnSelectDIR2.Name = "btnSelectDIR2";
            this.btnSelectDIR2.Size = new System.Drawing.Size(32, 21);
            this.btnSelectDIR2.TabIndex = 20;
            this.btnSelectDIR2.Text = "...";
            this.btnSelectDIR2.UseVisualStyleBackColor = true;
            this.btnSelectDIR2.Click += new System.EventHandler(this.btnSelectDIR2_Click);
            // 
            // btnSelectDIR1
            // 
            this.btnSelectDIR1.Location = new System.Drawing.Point(383, 33);
            this.btnSelectDIR1.Name = "btnSelectDIR1";
            this.btnSelectDIR1.Size = new System.Drawing.Size(32, 21);
            this.btnSelectDIR1.TabIndex = 19;
            this.btnSelectDIR1.Text = "...";
            this.btnSelectDIR1.UseVisualStyleBackColor = true;
            this.btnSelectDIR1.Click += new System.EventHandler(this.btnSelectDIR1_Click);
            // 
            // btnClearPairLog
            // 
            this.btnClearPairLog.Location = new System.Drawing.Point(335, 3);
            this.btnClearPairLog.Name = "btnClearPairLog";
            this.btnClearPairLog.Size = new System.Drawing.Size(80, 25);
            this.btnClearPairLog.TabIndex = 18;
            this.btnClearPairLog.Text = "清除日志";
            this.btnClearPairLog.UseVisualStyleBackColor = true;
            this.btnClearPairLog.Click += new System.EventHandler(this.btnClearPairLog_Click);
            // 
            // btnClearDir2BK
            // 
            this.btnClearDir2BK.Location = new System.Drawing.Point(311, 118);
            this.btnClearDir2BK.Name = "btnClearDir2BK";
            this.btnClearDir2BK.Size = new System.Drawing.Size(104, 25);
            this.btnClearDir2BK.TabIndex = 17;
            this.btnClearDir2BK.Text = "清理备份空间";
            this.btnClearDir2BK.UseVisualStyleBackColor = true;
            this.btnClearDir2BK.Click += new System.EventHandler(this.btnClearDir2BK_Click);
            // 
            // lblDir2BackupSpace
            // 
            this.lblDir2BackupSpace.AutoSize = true;
            this.lblDir2BackupSpace.Location = new System.Drawing.Point(95, 124);
            this.lblDir2BackupSpace.Name = "lblDir2BackupSpace";
            this.lblDir2BackupSpace.Size = new System.Drawing.Size(83, 12);
            this.lblDir2BackupSpace.TabIndex = 16;
            this.lblDir2BackupSpace.Text = "(DIR2BKSPACE)";
            // 
            // lblDir2Backup
            // 
            this.lblDir2Backup.AutoSize = true;
            this.lblDir2Backup.Location = new System.Drawing.Point(6, 124);
            this.lblDir2Backup.Name = "lblDir2Backup";
            this.lblDir2Backup.Size = new System.Drawing.Size(83, 12);
            this.lblDir2Backup.TabIndex = 15;
            this.lblDir2Backup.Text = "目录2备份空间";
            // 
            // btnClearDir1BK
            // 
            this.btnClearDir1BK.Location = new System.Drawing.Point(311, 60);
            this.btnClearDir1BK.Name = "btnClearDir1BK";
            this.btnClearDir1BK.Size = new System.Drawing.Size(104, 25);
            this.btnClearDir1BK.TabIndex = 14;
            this.btnClearDir1BK.Text = "清理备份空间";
            this.btnClearDir1BK.UseVisualStyleBackColor = true;
            this.btnClearDir1BK.Click += new System.EventHandler(this.btnClearDir1BK_Click);
            // 
            // lblDir1BackupSpace
            // 
            this.lblDir1BackupSpace.AutoSize = true;
            this.lblDir1BackupSpace.Location = new System.Drawing.Point(95, 66);
            this.lblDir1BackupSpace.Name = "lblDir1BackupSpace";
            this.lblDir1BackupSpace.Size = new System.Drawing.Size(83, 12);
            this.lblDir1BackupSpace.TabIndex = 12;
            this.lblDir1BackupSpace.Text = "(DIR1BKSPACE)";
            // 
            // lblDir1Backup
            // 
            this.lblDir1Backup.AutoSize = true;
            this.lblDir1Backup.Location = new System.Drawing.Point(6, 66);
            this.lblDir1Backup.Name = "lblDir1Backup";
            this.lblDir1Backup.Size = new System.Drawing.Size(83, 12);
            this.lblDir1Backup.TabIndex = 11;
            this.lblDir1Backup.Text = "目录1备份空间";
            // 
            // grpboxSyncRule
            // 
            this.grpboxSyncRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpboxSyncRule.Controls.Add(this.comboBox1);
            this.grpboxSyncRule.Controls.Add(this.label3);
            this.grpboxSyncRule.Controls.Add(this.label2);
            this.grpboxSyncRule.Controls.Add(this.txtBoxFilterRule);
            this.grpboxSyncRule.Controls.Add(this.label1);
            this.grpboxSyncRule.Controls.Add(this.txtBoxSyncInterval);
            this.grpboxSyncRule.Location = new System.Drawing.Point(436, 271);
            this.grpboxSyncRule.Name = "grpboxSyncRule";
            this.grpboxSyncRule.Size = new System.Drawing.Size(296, 147);
            this.grpboxSyncRule.TabIndex = 13;
            this.grpboxSyncRule.TabStop = false;
            this.grpboxSyncRule.Text = "同步规则";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "目录1<->目录2（双向同步并删除）",
            "目录1<->目录2（双向同步不删除）",
            "目录1->目录2（单向同步并删除）",
            "目录1->目录2（单向同步不删除）",
            "目录2->目录1（单向同步并删除）",
            "目录2->目录1（单向同步不删除）"});
            this.comboBox1.Location = new System.Drawing.Point(65, 88);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(223, 20);
            this.comboBox1.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "同步方向";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(233, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "排除规则(不同关键字之间用英文逗号隔开)";
            // 
            // txtBoxFilterRule
            // 
            this.txtBoxFilterRule.Location = new System.Drawing.Point(8, 32);
            this.txtBoxFilterRule.MaxLength = 255;
            this.txtBoxFilterRule.Multiline = true;
            this.txtBoxFilterRule.Name = "txtBoxFilterRule";
            this.txtBoxFilterRule.Size = new System.Drawing.Size(280, 50);
            this.txtBoxFilterRule.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "自动同步间隔(分钟)，0不触发自动同步";
            // 
            // txtBoxSyncInterval
            // 
            this.txtBoxSyncInterval.Location = new System.Drawing.Point(237, 118);
            this.txtBoxSyncInterval.MaxLength = 255;
            this.txtBoxSyncInterval.Name = "txtBoxSyncInterval";
            this.txtBoxSyncInterval.Size = new System.Drawing.Size(51, 21);
            this.txtBoxSyncInterval.TabIndex = 10;
            // 
            // frm_DirectoryPairManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnQuit;
            this.ClientSize = new System.Drawing.Size(744, 461);
            this.Controls.Add(this.grpboxSyncRule);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnUpdPair);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnDelPair);
            this.Controls.Add(this.btnNewPair);
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_DirectoryPairManagement";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "配对管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DirectoryPairManager_FormClosing);
            this.Load += new System.EventHandler(this.DirectoryPairManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grpboxSyncRule.ResumeLayout(false);
            this.grpboxSyncRule.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label label1;
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
    }
}