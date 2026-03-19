
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
            label1 = new System.Windows.Forms.Label();
            txtBoxSyncInterval = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            grpboxSyncRule.SuspendLayout();
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
            dataGridView1.Location = new System.Drawing.Point(18, 20);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 23;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(1080, 422);
            dataGridView1.TabIndex = 0;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // btnNewPair
            // 
            btnNewPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnNewPair.Location = new System.Drawing.Point(200, 707);
            btnNewPair.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnNewPair.Name = "btnNewPair";
            btnNewPair.Size = new System.Drawing.Size(188, 42);
            btnNewPair.TabIndex = 1;
            btnNewPair.Text = "新增文件夹配对";
            btnNewPair.UseVisualStyleBackColor = true;
            btnNewPair.Click += btnNewPair_Click;
            // 
            // btnDelPair
            // 
            btnDelPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDelPair.Location = new System.Drawing.Point(592, 707);
            btnDelPair.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnDelPair.Name = "btnDelPair";
            btnDelPair.Size = new System.Drawing.Size(188, 42);
            btnDelPair.TabIndex = 2;
            btnDelPair.Text = "删除文件夹配对";
            btnDelPair.UseVisualStyleBackColor = true;
            btnDelPair.Click += btnDelPair_Click;
            // 
            // btnQuit
            // 
            btnQuit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnQuit.Location = new System.Drawing.Point(948, 707);
            btnQuit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new System.Drawing.Size(150, 42);
            btnQuit.TabIndex = 3;
            btnQuit.Text = "关闭";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRefresh.Location = new System.Drawing.Point(789, 707);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(150, 42);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblDir1
            // 
            lblDir1.AutoSize = true;
            lblDir1.Location = new System.Drawing.Point(9, 60);
            lblDir1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDir1.Name = "lblDir1";
            lblDir1.Size = new System.Drawing.Size(78, 20);
            lblDir1.TabIndex = 5;
            lblDir1.Text = "目录1路径";
            // 
            // lblDIr2
            // 
            lblDIr2.AutoSize = true;
            lblDIr2.Location = new System.Drawing.Point(9, 157);
            lblDIr2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDIr2.Name = "lblDIr2";
            lblDIr2.Size = new System.Drawing.Size(78, 20);
            lblDIr2.TabIndex = 6;
            lblDIr2.Text = "目录2路径";
            // 
            // txtBoxDir1
            // 
            txtBoxDir1.Location = new System.Drawing.Point(106, 55);
            txtBoxDir1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtBoxDir1.MaxLength = 255;
            txtBoxDir1.Name = "txtBoxDir1";
            txtBoxDir1.Size = new System.Drawing.Size(457, 27);
            txtBoxDir1.TabIndex = 7;
            // 
            // txtBoxDir2
            // 
            txtBoxDir2.Location = new System.Drawing.Point(106, 152);
            txtBoxDir2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtBoxDir2.MaxLength = 255;
            txtBoxDir2.Name = "txtBoxDir2";
            txtBoxDir2.Size = new System.Drawing.Size(457, 27);
            txtBoxDir2.TabIndex = 8;
            // 
            // lblPairName
            // 
            lblPairName.AutoSize = true;
            lblPairName.Location = new System.Drawing.Point(9, 15);
            lblPairName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPairName.Name = "lblPairName";
            lblPairName.Size = new System.Drawing.Size(69, 20);
            lblPairName.TabIndex = 9;
            lblPairName.Text = "配对名称";
            // 
            // txtBoxPairName
            // 
            txtBoxPairName.Location = new System.Drawing.Point(106, 10);
            txtBoxPairName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtBoxPairName.MaxLength = 50;
            txtBoxPairName.Name = "txtBoxPairName";
            txtBoxPairName.Size = new System.Drawing.Size(158, 27);
            txtBoxPairName.TabIndex = 10;
            // 
            // btnUpdPair
            // 
            btnUpdPair.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnUpdPair.Location = new System.Drawing.Point(396, 707);
            btnUpdPair.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnUpdPair.Name = "btnUpdPair";
            btnUpdPair.Size = new System.Drawing.Size(188, 42);
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
            panel1.Location = new System.Drawing.Point(18, 452);
            panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(627, 245);
            panel1.TabIndex = 12;
            // 
            // lblPairLogSize
            // 
            lblPairLogSize.AutoSize = true;
            lblPairLogSize.Location = new System.Drawing.Point(400, 15);
            lblPairLogSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPairLogSize.Name = "lblPairLogSize";
            lblPairLogSize.Size = new System.Drawing.Size(84, 20);
            lblPairLogSize.TabIndex = 22;
            lblPairLogSize.Text = "(PAIRLOG)";
            // 
            // lblPairLog
            // 
            lblPairLog.AutoSize = true;
            lblPairLog.Location = new System.Drawing.Point(276, 15);
            lblPairLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPairLog.Name = "lblPairLog";
            lblPairLog.Size = new System.Drawing.Size(99, 20);
            lblPairLog.TabIndex = 21;
            lblPairLog.Text = "配对日志文件";
            // 
            // btnSelectDIR2
            // 
            btnSelectDIR2.Location = new System.Drawing.Point(574, 152);
            btnSelectDIR2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSelectDIR2.Name = "btnSelectDIR2";
            btnSelectDIR2.Size = new System.Drawing.Size(48, 35);
            btnSelectDIR2.TabIndex = 20;
            btnSelectDIR2.Text = "...";
            btnSelectDIR2.UseVisualStyleBackColor = true;
            btnSelectDIR2.Click += btnSelectDIR2_Click;
            // 
            // btnSelectDIR1
            // 
            btnSelectDIR1.Location = new System.Drawing.Point(574, 55);
            btnSelectDIR1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSelectDIR1.Name = "btnSelectDIR1";
            btnSelectDIR1.Size = new System.Drawing.Size(48, 35);
            btnSelectDIR1.TabIndex = 19;
            btnSelectDIR1.Text = "...";
            btnSelectDIR1.UseVisualStyleBackColor = true;
            btnSelectDIR1.Click += btnSelectDIR1_Click;
            // 
            // btnClearPairLog
            // 
            btnClearPairLog.Location = new System.Drawing.Point(502, 5);
            btnClearPairLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnClearPairLog.Name = "btnClearPairLog";
            btnClearPairLog.Size = new System.Drawing.Size(120, 42);
            btnClearPairLog.TabIndex = 18;
            btnClearPairLog.Text = "清除日志";
            btnClearPairLog.UseVisualStyleBackColor = true;
            btnClearPairLog.Click += btnClearPairLog_Click;
            // 
            // btnClearDir2BK
            // 
            btnClearDir2BK.Location = new System.Drawing.Point(466, 197);
            btnClearDir2BK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnClearDir2BK.Name = "btnClearDir2BK";
            btnClearDir2BK.Size = new System.Drawing.Size(156, 42);
            btnClearDir2BK.TabIndex = 17;
            btnClearDir2BK.Text = "清理备份空间";
            btnClearDir2BK.UseVisualStyleBackColor = true;
            btnClearDir2BK.Click += btnClearDir2BK_Click;
            // 
            // lblDir2BackupSpace
            // 
            lblDir2BackupSpace.AutoSize = true;
            lblDir2BackupSpace.Location = new System.Drawing.Point(142, 207);
            lblDir2BackupSpace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDir2BackupSpace.Name = "lblDir2BackupSpace";
            lblDir2BackupSpace.Size = new System.Drawing.Size(119, 20);
            lblDir2BackupSpace.TabIndex = 16;
            lblDir2BackupSpace.Text = "(DIR2BKSPACE)";
            // 
            // lblDir2Backup
            // 
            lblDir2Backup.AutoSize = true;
            lblDir2Backup.Location = new System.Drawing.Point(9, 207);
            lblDir2Backup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDir2Backup.Name = "lblDir2Backup";
            lblDir2Backup.Size = new System.Drawing.Size(108, 20);
            lblDir2Backup.TabIndex = 15;
            lblDir2Backup.Text = "目录2备份空间";
            // 
            // btnClearDir1BK
            // 
            btnClearDir1BK.Location = new System.Drawing.Point(466, 100);
            btnClearDir1BK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnClearDir1BK.Name = "btnClearDir1BK";
            btnClearDir1BK.Size = new System.Drawing.Size(156, 42);
            btnClearDir1BK.TabIndex = 14;
            btnClearDir1BK.Text = "清理备份空间";
            btnClearDir1BK.UseVisualStyleBackColor = true;
            btnClearDir1BK.Click += btnClearDir1BK_Click;
            // 
            // lblDir1BackupSpace
            // 
            lblDir1BackupSpace.AutoSize = true;
            lblDir1BackupSpace.Location = new System.Drawing.Point(142, 110);
            lblDir1BackupSpace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDir1BackupSpace.Name = "lblDir1BackupSpace";
            lblDir1BackupSpace.Size = new System.Drawing.Size(119, 20);
            lblDir1BackupSpace.TabIndex = 12;
            lblDir1BackupSpace.Text = "(DIR1BKSPACE)";
            // 
            // lblDir1Backup
            // 
            lblDir1Backup.AutoSize = true;
            lblDir1Backup.Location = new System.Drawing.Point(9, 110);
            lblDir1Backup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDir1Backup.Name = "lblDir1Backup";
            lblDir1Backup.Size = new System.Drawing.Size(108, 20);
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
            grpboxSyncRule.Controls.Add(label1);
            grpboxSyncRule.Controls.Add(txtBoxSyncInterval);
            grpboxSyncRule.Location = new System.Drawing.Point(654, 452);
            grpboxSyncRule.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpboxSyncRule.Name = "grpboxSyncRule";
            grpboxSyncRule.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpboxSyncRule.Size = new System.Drawing.Size(444, 245);
            grpboxSyncRule.TabIndex = 13;
            grpboxSyncRule.TabStop = false;
            grpboxSyncRule.Text = "同步规则";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "目录1<->目录2（双向同步并删除）", "目录1<->目录2（双向同步不删除）", "目录1->目录2（单向同步并删除）", "目录1->目录2（单向同步不删除）", "目录2->目录1（单向同步并删除）", "目录2->目录1（单向同步不删除）" });
            comboBox1.Location = new System.Drawing.Point(98, 147);
            comboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(332, 28);
            comboBox1.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(9, 152);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(69, 20);
            label3.TabIndex = 13;
            label3.Text = "同步方向";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 28);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(289, 20);
            label2.TabIndex = 11;
            label2.Text = "排除规则(不同关键字之间用英文逗号隔开)";
            // 
            // txtBoxFilterRule
            // 
            txtBoxFilterRule.Location = new System.Drawing.Point(12, 53);
            txtBoxFilterRule.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtBoxFilterRule.MaxLength = 255;
            txtBoxFilterRule.Multiline = true;
            txtBoxFilterRule.Name = "txtBoxFilterRule";
            txtBoxFilterRule.Size = new System.Drawing.Size(418, 81);
            txtBoxFilterRule.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 202);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(253, 20);
            label1.TabIndex = 9;
            label1.Text = "自动同步间隔(分钟)，0代表实时同步";
            // 
            // txtBoxSyncInterval
            // 
            txtBoxSyncInterval.Location = new System.Drawing.Point(356, 197);
            txtBoxSyncInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtBoxSyncInterval.MaxLength = 255;
            txtBoxSyncInterval.Name = "txtBoxSyncInterval";
            txtBoxSyncInterval.Size = new System.Drawing.Size(74, 27);
            txtBoxSyncInterval.TabIndex = 10;
            // 
            // frm_DirectoryPairManagement
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnQuit;
            ClientSize = new System.Drawing.Size(1116, 768);
            Controls.Add(grpboxSyncRule);
            Controls.Add(panel1);
            Controls.Add(btnUpdPair);
            Controls.Add(btnRefresh);
            Controls.Add(btnQuit);
            Controls.Add(btnDelPair);
            Controls.Add(btnNewPair);
            Controls.Add(dataGridView1);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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