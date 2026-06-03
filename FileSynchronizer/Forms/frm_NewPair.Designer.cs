namespace FileSynchronizer
{
    partial class frm_NewPair
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
            grpboxSyncRule = new System.Windows.Forms.GroupBox();
            comboBox1 = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtBoxFilterRule = new System.Windows.Forms.TextBox();
            btnSelectDIR2 = new System.Windows.Forms.Button();
            btnSelectDIR1 = new System.Windows.Forms.Button();
            lblPairName = new System.Windows.Forms.Label();
            lblDir1 = new System.Windows.Forms.Label();
            txtBoxPairName = new System.Windows.Forms.TextBox();
            lblDIr2 = new System.Windows.Forms.Label();
            txtBoxDir1 = new System.Windows.Forms.TextBox();
            txtBoxDir2 = new System.Windows.Forms.TextBox();
            btnConfirmed = new System.Windows.Forms.Button();
            btnQuit = new System.Windows.Forms.Button();
            grpboxAutoSync = new System.Windows.Forms.GroupBox();
            checkBoxPauseSync = new System.Windows.Forms.CheckBox();
            comBoxSyncDay = new System.Windows.Forms.ComboBox();
            timePickerSyncTime = new System.Windows.Forms.DateTimePicker();
            radioButtonFixedTime = new System.Windows.Forms.RadioButton();
            radioButtonSyncInterval = new System.Windows.Forms.RadioButton();
            txtBoxSyncInterval = new System.Windows.Forms.TextBox();
            checkBoxRealTimeSync = new System.Windows.Forms.CheckBox();
            grpboxSyncRule.SuspendLayout();
            grpboxAutoSync.SuspendLayout();
            SuspendLayout();
            // 
            // grpboxSyncRule
            // 
            grpboxSyncRule.Controls.Add(comboBox1);
            grpboxSyncRule.Controls.Add(label3);
            grpboxSyncRule.Controls.Add(label2);
            grpboxSyncRule.Controls.Add(txtBoxFilterRule);
            grpboxSyncRule.Location = new System.Drawing.Point(12, 107);
            grpboxSyncRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Name = "grpboxSyncRule";
            grpboxSyncRule.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Size = new System.Drawing.Size(291, 142);
            grpboxSyncRule.TabIndex = 15;
            grpboxSyncRule.TabStop = false;
            grpboxSyncRule.Text = "同步规则";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "目录1<->目录2（双向同步并删除）", "目录1<->目录2（双向同步不删除）", "目录1->目录2（单向同步并删除）", "目录1->目录2（单向同步不删除）", "目录2->目录1（单向同步并删除）", "目录2->目录1（单向同步不删除）" });
            comboBox1.Location = new System.Drawing.Point(69, 109);
            comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(216, 25);
            comboBox1.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(7, 114);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(56, 17);
            label3.TabIndex = 13;
            label3.Text = "同步方向";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 25);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(232, 17);
            label2.TabIndex = 11;
            label2.Text = "排除规则(不同关键字之间用英文逗号隔开)";
            // 
            // txtBoxFilterRule
            // 
            txtBoxFilterRule.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBoxFilterRule.Location = new System.Drawing.Point(9, 45);
            txtBoxFilterRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxFilterRule.MaxLength = 255;
            txtBoxFilterRule.Multiline = true;
            txtBoxFilterRule.Name = "txtBoxFilterRule";
            txtBoxFilterRule.Size = new System.Drawing.Size(276, 56);
            txtBoxFilterRule.TabIndex = 12;
            // 
            // btnSelectDIR2
            // 
            btnSelectDIR2.Location = new System.Drawing.Point(585, 74);
            btnSelectDIR2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSelectDIR2.Name = "btnSelectDIR2";
            btnSelectDIR2.Size = new System.Drawing.Size(37, 25);
            btnSelectDIR2.TabIndex = 20;
            btnSelectDIR2.Text = "...";
            btnSelectDIR2.UseVisualStyleBackColor = true;
            btnSelectDIR2.Click += btnSelectDIR2_Click;
            // 
            // btnSelectDIR1
            // 
            btnSelectDIR1.Location = new System.Drawing.Point(585, 44);
            btnSelectDIR1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSelectDIR1.Name = "btnSelectDIR1";
            btnSelectDIR1.Size = new System.Drawing.Size(37, 25);
            btnSelectDIR1.TabIndex = 19;
            btnSelectDIR1.Text = "...";
            btnSelectDIR1.UseVisualStyleBackColor = true;
            btnSelectDIR1.Click += btnSelectDIR1_Click;
            // 
            // lblPairName
            // 
            lblPairName.AutoSize = true;
            lblPairName.Location = new System.Drawing.Point(12, 16);
            lblPairName.Name = "lblPairName";
            lblPairName.Size = new System.Drawing.Size(56, 17);
            lblPairName.TabIndex = 9;
            lblPairName.Text = "配对名称";
            // 
            // lblDir1
            // 
            lblDir1.AutoSize = true;
            lblDir1.Location = new System.Drawing.Point(5, 48);
            lblDir1.Name = "lblDir1";
            lblDir1.Size = new System.Drawing.Size(63, 17);
            lblDir1.TabIndex = 5;
            lblDir1.Text = "目录1路径";
            // 
            // txtBoxPairName
            // 
            txtBoxPairName.Location = new System.Drawing.Point(74, 13);
            txtBoxPairName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxPairName.MaxLength = 50;
            txtBoxPairName.Name = "txtBoxPairName";
            txtBoxPairName.Size = new System.Drawing.Size(548, 23);
            txtBoxPairName.TabIndex = 10;
            // 
            // lblDIr2
            // 
            lblDIr2.AutoSize = true;
            lblDIr2.Location = new System.Drawing.Point(5, 78);
            lblDIr2.Name = "lblDIr2";
            lblDIr2.Size = new System.Drawing.Size(63, 17);
            lblDIr2.TabIndex = 6;
            lblDIr2.Text = "目录2路径";
            // 
            // txtBoxDir1
            // 
            txtBoxDir1.Location = new System.Drawing.Point(74, 44);
            txtBoxDir1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir1.MaxLength = 255;
            txtBoxDir1.Name = "txtBoxDir1";
            txtBoxDir1.Size = new System.Drawing.Size(505, 23);
            txtBoxDir1.TabIndex = 7;
            // 
            // txtBoxDir2
            // 
            txtBoxDir2.Location = new System.Drawing.Point(74, 75);
            txtBoxDir2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir2.MaxLength = 255;
            txtBoxDir2.Name = "txtBoxDir2";
            txtBoxDir2.Size = new System.Drawing.Size(505, 23);
            txtBoxDir2.TabIndex = 8;
            // 
            // btnConfirmed
            // 
            btnConfirmed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnConfirmed.Location = new System.Drawing.Point(382, 213);
            btnConfirmed.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            btnConfirmed.Name = "btnConfirmed";
            btnConfirmed.Size = new System.Drawing.Size(117, 36);
            btnConfirmed.TabIndex = 16;
            btnConfirmed.Text = "确定";
            btnConfirmed.UseVisualStyleBackColor = true;
            btnConfirmed.Click += btnConfirmed_Click;
            // 
            // btnQuit
            // 
            btnQuit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnQuit.Location = new System.Drawing.Point(505, 213);
            btnQuit.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new System.Drawing.Size(117, 36);
            btnQuit.TabIndex = 15;
            btnQuit.Text = "关闭";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
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
            grpboxAutoSync.Location = new System.Drawing.Point(309, 107);
            grpboxAutoSync.Name = "grpboxAutoSync";
            grpboxAutoSync.Size = new System.Drawing.Size(313, 101);
            grpboxAutoSync.TabIndex = 17;
            grpboxAutoSync.TabStop = false;
            grpboxAutoSync.Text = "自动同步设置";
            // 
            // checkBoxPauseSync
            // 
            checkBoxPauseSync.AutoSize = true;
            checkBoxPauseSync.Location = new System.Drawing.Point(87, 22);
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
            comBoxSyncDay.Location = new System.Drawing.Point(110, 75);
            comBoxSyncDay.Name = "comBoxSyncDay";
            comBoxSyncDay.Size = new System.Drawing.Size(115, 25);
            comBoxSyncDay.TabIndex = 16;
            // 
            // timePickerSyncTime
            // 
            timePickerSyncTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            timePickerSyncTime.Location = new System.Drawing.Point(231, 76);
            timePickerSyncTime.Name = "timePickerSyncTime";
            timePickerSyncTime.ShowUpDown = true;
            timePickerSyncTime.Size = new System.Drawing.Size(76, 23);
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
            // txtBoxSyncInterval
            // 
            txtBoxSyncInterval.Location = new System.Drawing.Point(158, 48);
            txtBoxSyncInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxSyncInterval.MaxLength = 255;
            txtBoxSyncInterval.Name = "txtBoxSyncInterval";
            txtBoxSyncInterval.Size = new System.Drawing.Size(67, 23);
            txtBoxSyncInterval.TabIndex = 10;
            // 
            // checkBoxRealTimeSync
            // 
            checkBoxRealTimeSync.AutoSize = true;
            checkBoxRealTimeSync.Location = new System.Drawing.Point(6, 22);
            checkBoxRealTimeSync.Name = "checkBoxRealTimeSync";
            checkBoxRealTimeSync.Size = new System.Drawing.Size(75, 21);
            checkBoxRealTimeSync.TabIndex = 20;
            checkBoxRealTimeSync.Text = "实时同步";
            checkBoxRealTimeSync.UseVisualStyleBackColor = true;
            checkBoxRealTimeSync.CheckedChanged += checkBoxRealTimeSync_CheckedChanged;
            // 
            // frm_NewPair
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(634, 259);
            Controls.Add(grpboxSyncRule);
            Controls.Add(grpboxAutoSync);
            Controls.Add(btnSelectDIR2);
            Controls.Add(btnConfirmed);
            Controls.Add(btnSelectDIR1);
            Controls.Add(btnQuit);
            Controls.Add(lblPairName);
            Controls.Add(lblDir1);
            Controls.Add(txtBoxPairName);
            Controls.Add(txtBoxDir2);
            Controls.Add(lblDIr2);
            Controls.Add(txtBoxDir1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frm_NewPair";
            ShowIcon = false;
            Text = "新增文件夹配对";
            FormClosing += frm_NewPair_FormClosing;
            grpboxSyncRule.ResumeLayout(false);
            grpboxSyncRule.PerformLayout();
            grpboxAutoSync.ResumeLayout(false);
            grpboxAutoSync.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpboxSyncRule;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxFilterRule;
        private System.Windows.Forms.Button btnSelectDIR2;
        private System.Windows.Forms.Button btnSelectDIR1;
        private System.Windows.Forms.Label lblPairName;
        private System.Windows.Forms.Label lblDir1;
        private System.Windows.Forms.TextBox txtBoxPairName;
        private System.Windows.Forms.Label lblDIr2;
        private System.Windows.Forms.TextBox txtBoxDir1;
        private System.Windows.Forms.TextBox txtBoxDir2;
        private System.Windows.Forms.Button btnConfirmed;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.GroupBox grpboxAutoSync;
        private System.Windows.Forms.CheckBox checkBoxPauseSync;
        private System.Windows.Forms.ComboBox comBoxSyncDay;
        private System.Windows.Forms.DateTimePicker timePickerSyncTime;
        private System.Windows.Forms.RadioButton radioButtonFixedTime;
        private System.Windows.Forms.RadioButton radioButtonSyncInterval;
        private System.Windows.Forms.TextBox txtBoxSyncInterval;
        private System.Windows.Forms.CheckBox checkBoxRealTimeSync;
    }
}