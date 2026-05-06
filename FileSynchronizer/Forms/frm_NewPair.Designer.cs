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
            label1 = new System.Windows.Forms.Label();
            txtBoxSyncInterval = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
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
            grpboxSyncRule.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // grpboxSyncRule
            // 
            grpboxSyncRule.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpboxSyncRule.Controls.Add(comboBox1);
            grpboxSyncRule.Controls.Add(label3);
            grpboxSyncRule.Controls.Add(label2);
            grpboxSyncRule.Controls.Add(txtBoxFilterRule);
            grpboxSyncRule.Controls.Add(label1);
            grpboxSyncRule.Controls.Add(txtBoxSyncInterval);
            grpboxSyncRule.Location = new System.Drawing.Point(7, 116);
            grpboxSyncRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Name = "grpboxSyncRule";
            grpboxSyncRule.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpboxSyncRule.Size = new System.Drawing.Size(533, 142);
            grpboxSyncRule.TabIndex = 15;
            grpboxSyncRule.TabStop = false;
            grpboxSyncRule.Text = "同步规则";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "目录1<->目录2（双向同步并删除）", "目录1<->目录2（双向同步不删除）", "目录1->目录2（单向同步并删除）", "目录1->目录2（单向同步不删除）", "目录2->目录1（单向同步并删除）", "目录2->目录1（单向同步不删除）" });
            comboBox1.Location = new System.Drawing.Point(69, 109);
            comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(198, 25);
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
            txtBoxFilterRule.Size = new System.Drawing.Size(518, 56);
            txtBoxFilterRule.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(273, 112);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(203, 17);
            label1.TabIndex = 9;
            label1.Text = "自动同步间隔(分钟)，0代表实时同步";
            // 
            // txtBoxSyncInterval
            // 
            txtBoxSyncInterval.Location = new System.Drawing.Point(482, 109);
            txtBoxSyncInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxSyncInterval.MaxLength = 255;
            txtBoxSyncInterval.Name = "txtBoxSyncInterval";
            txtBoxSyncInterval.Size = new System.Drawing.Size(45, 23);
            txtBoxSyncInterval.TabIndex = 10;
            // 
            // panel1
            // 
            panel1.Controls.Add(grpboxSyncRule);
            panel1.Controls.Add(btnSelectDIR2);
            panel1.Controls.Add(btnSelectDIR1);
            panel1.Controls.Add(lblPairName);
            panel1.Controls.Add(lblDir1);
            panel1.Controls.Add(txtBoxPairName);
            panel1.Controls.Add(lblDIr2);
            panel1.Controls.Add(txtBoxDir1);
            panel1.Controls.Add(txtBoxDir2);
            panel1.Location = new System.Drawing.Point(12, 13);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(545, 262);
            panel1.TabIndex = 14;
            // 
            // btnSelectDIR2
            // 
            btnSelectDIR2.Location = new System.Drawing.Point(497, 82);
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
            btnSelectDIR1.Location = new System.Drawing.Point(497, 43);
            btnSelectDIR1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSelectDIR1.Name = "btnSelectDIR1";
            btnSelectDIR1.Size = new System.Drawing.Size(37, 30);
            btnSelectDIR1.TabIndex = 19;
            btnSelectDIR1.Text = "...";
            btnSelectDIR1.UseVisualStyleBackColor = true;
            btnSelectDIR1.Click += btnSelectDIR1_Click;
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
            // lblDir1
            // 
            lblDir1.AutoSize = true;
            lblDir1.Location = new System.Drawing.Point(7, 51);
            lblDir1.Name = "lblDir1";
            lblDir1.Size = new System.Drawing.Size(63, 17);
            lblDir1.TabIndex = 5;
            lblDir1.Text = "目录1路径";
            // 
            // txtBoxPairName
            // 
            txtBoxPairName.Location = new System.Drawing.Point(76, 10);
            txtBoxPairName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxPairName.MaxLength = 50;
            txtBoxPairName.Name = "txtBoxPairName";
            txtBoxPairName.Size = new System.Drawing.Size(154, 23);
            txtBoxPairName.TabIndex = 10;
            // 
            // lblDIr2
            // 
            lblDIr2.AutoSize = true;
            lblDIr2.Location = new System.Drawing.Point(7, 89);
            lblDIr2.Name = "lblDIr2";
            lblDIr2.Size = new System.Drawing.Size(63, 17);
            lblDIr2.TabIndex = 6;
            lblDIr2.Text = "目录2路径";
            // 
            // txtBoxDir1
            // 
            txtBoxDir1.Location = new System.Drawing.Point(76, 47);
            txtBoxDir1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir1.MaxLength = 255;
            txtBoxDir1.Name = "txtBoxDir1";
            txtBoxDir1.Size = new System.Drawing.Size(415, 23);
            txtBoxDir1.TabIndex = 7;
            // 
            // txtBoxDir2
            // 
            txtBoxDir2.Location = new System.Drawing.Point(76, 85);
            txtBoxDir2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBoxDir2.MaxLength = 255;
            txtBoxDir2.Name = "txtBoxDir2";
            txtBoxDir2.Size = new System.Drawing.Size(415, 23);
            txtBoxDir2.TabIndex = 8;
            // 
            // btnConfirmed
            // 
            btnConfirmed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnConfirmed.Location = new System.Drawing.Point(317, 283);
            btnConfirmed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            btnQuit.Location = new System.Drawing.Point(440, 283);
            btnQuit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new System.Drawing.Size(117, 36);
            btnQuit.TabIndex = 15;
            btnQuit.Text = "关闭";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // frm_NewPair
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(569, 326);
            Controls.Add(btnConfirmed);
            Controls.Add(btnQuit);
            Controls.Add(panel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frm_NewPair";
            ShowIcon = false;
            Text = "新增文件夹配对";
            FormClosing += frm_NewPair_FormClosing;
            grpboxSyncRule.ResumeLayout(false);
            grpboxSyncRule.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpboxSyncRule;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxFilterRule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxSyncInterval;
        private System.Windows.Forms.Panel panel1;
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
    }
}