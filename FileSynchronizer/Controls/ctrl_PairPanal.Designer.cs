namespace FileSynchronizer
{
    partial class ctrl_PairPanal
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            TxtPairLog = new System.Windows.Forms.RichTextBox();
            pnlPairExInfor = new System.Windows.Forms.Panel();
            lblNextSyncTime = new System.Windows.Forms.Label();
            lblLastSyncTime = new System.Windows.Forms.Label();
            lblSyncProgress = new System.Windows.Forms.Label();
            lblFileCountSync = new System.Windows.Forms.Label();
            lblFileCountFound = new System.Windows.Forms.Label();
            lblAnalysisProgress = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            lblBeingSync = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            lblPairInfor = new System.Windows.Forms.Label();
            pnlPairBasicInfor = new System.Windows.Forms.Panel();
            pnlPairExInfor.SuspendLayout();
            pnlPairBasicInfor.SuspendLayout();
            SuspendLayout();
            // 
            // TxtPairLog
            // 
            TxtPairLog.BackColor = System.Drawing.SystemColors.Info;
            TxtPairLog.Dock = System.Windows.Forms.DockStyle.Fill;
            TxtPairLog.Location = new System.Drawing.Point(0, 27);
            TxtPairLog.Margin = new System.Windows.Forms.Padding(4);
            TxtPairLog.Name = "TxtPairLog";
            TxtPairLog.ReadOnly = true;
            TxtPairLog.Size = new System.Drawing.Size(1073, 635);
            TxtPairLog.TabIndex = 0;
            TxtPairLog.Text = "";
            // 
            // pnlPairExInfor
            // 
            pnlPairExInfor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlPairExInfor.Controls.Add(lblNextSyncTime);
            pnlPairExInfor.Controls.Add(lblLastSyncTime);
            pnlPairExInfor.Controls.Add(lblSyncProgress);
            pnlPairExInfor.Controls.Add(lblFileCountSync);
            pnlPairExInfor.Controls.Add(lblFileCountFound);
            pnlPairExInfor.Controls.Add(lblAnalysisProgress);
            pnlPairExInfor.Controls.Add(label7);
            pnlPairExInfor.Controls.Add(label9);
            pnlPairExInfor.Controls.Add(lblBeingSync);
            pnlPairExInfor.Controls.Add(label4);
            pnlPairExInfor.Controls.Add(label3);
            pnlPairExInfor.Controls.Add(label1);
            pnlPairExInfor.Controls.Add(label5);
            pnlPairExInfor.Controls.Add(label2);
            pnlPairExInfor.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlPairExInfor.Location = new System.Drawing.Point(0, 662);
            pnlPairExInfor.Margin = new System.Windows.Forms.Padding(4);
            pnlPairExInfor.Name = "pnlPairExInfor";
            pnlPairExInfor.Size = new System.Drawing.Size(1073, 46);
            pnlPairExInfor.TabIndex = 20;
            // 
            // lblNextSyncTime
            // 
            lblNextSyncTime.AutoSize = true;
            lblNextSyncTime.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblNextSyncTime.Location = new System.Drawing.Point(308, 4);
            lblNextSyncTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNextSyncTime.Name = "lblNextSyncTime";
            lblNextSyncTime.Size = new System.Drawing.Size(95, 12);
            lblNextSyncTime.TabIndex = 29;
            lblNextSyncTime.Text = "(NextSyncTimeA)";
            // 
            // lblLastSyncTime
            // 
            lblLastSyncTime.AutoSize = true;
            lblLastSyncTime.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblLastSyncTime.Location = new System.Drawing.Point(97, 4);
            lblLastSyncTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLastSyncTime.Name = "lblLastSyncTime";
            lblLastSyncTime.Size = new System.Drawing.Size(95, 12);
            lblLastSyncTime.TabIndex = 27;
            lblLastSyncTime.Text = "(LastSyncTimeA)";
            // 
            // lblSyncProgress
            // 
            lblSyncProgress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblSyncProgress.AutoSize = true;
            lblSyncProgress.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblSyncProgress.Location = new System.Drawing.Point(1020, 4);
            lblSyncProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSyncProgress.Name = "lblSyncProgress";
            lblSyncProgress.Size = new System.Drawing.Size(41, 12);
            lblSyncProgress.TabIndex = 19;
            lblSyncProgress.Text = "(%%%%)";
            // 
            // lblFileCountSync
            // 
            lblFileCountSync.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblFileCountSync.AutoSize = true;
            lblFileCountSync.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblFileCountSync.Location = new System.Drawing.Point(832, 4);
            lblFileCountSync.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFileCountSync.Name = "lblFileCountSync";
            lblFileCountSync.Size = new System.Drawing.Size(47, 12);
            lblFileCountSync.TabIndex = 23;
            lblFileCountSync.Text = "(FileA)";
            // 
            // lblFileCountFound
            // 
            lblFileCountFound.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblFileCountFound.AutoSize = true;
            lblFileCountFound.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblFileCountFound.Location = new System.Drawing.Point(632, 4);
            lblFileCountFound.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFileCountFound.Name = "lblFileCountFound";
            lblFileCountFound.Size = new System.Drawing.Size(47, 12);
            lblFileCountFound.TabIndex = 20;
            lblFileCountFound.Text = "(FileF)";
            // 
            // lblAnalysisProgress
            // 
            lblAnalysisProgress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblAnalysisProgress.AutoSize = true;
            lblAnalysisProgress.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblAnalysisProgress.Location = new System.Drawing.Point(534, 4);
            lblAnalysisProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAnalysisProgress.Name = "lblAnalysisProgress";
            lblAnalysisProgress.Size = new System.Drawing.Size(41, 12);
            lblAnalysisProgress.TabIndex = 17;
            lblAnalysisProgress.Text = "(%%%%)";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label7.Location = new System.Drawing.Point(4, 4);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(89, 12);
            label7.TabIndex = 28;
            label7.Text = "最后同步时间：";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label9.Location = new System.Drawing.Point(215, 4);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(89, 12);
            label9.TabIndex = 30;
            label9.Text = "下次同步时间：";
            // 
            // lblBeingSync
            // 
            lblBeingSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblBeingSync.AutoSize = true;
            lblBeingSync.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblBeingSync.Location = new System.Drawing.Point(62, 27);
            lblBeingSync.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblBeingSync.Name = "lblBeingSync";
            lblBeingSync.Size = new System.Drawing.Size(60, 10);
            lblBeingSync.TabIndex = 26;
            lblBeingSync.Text = "(BeingSync)";
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label4.Location = new System.Drawing.Point(4, 27);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(55, 10);
            label4.TabIndex = 25;
            label4.Text = "正在进行：";
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label3.Location = new System.Drawing.Point(687, 4);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(137, 12);
            label3.TabIndex = 24;
            label3.Text = "个文件或目录对象，其中";
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label1.Location = new System.Drawing.Point(461, 4);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 12);
            label1.TabIndex = 18;
            label1.Text = "分析进度：";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label5.Location = new System.Drawing.Point(887, 4);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(125, 12);
            label5.TabIndex = 22;
            label5.Text = "个需同步，同步进度：";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            label2.Location = new System.Drawing.Point(583, 4);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(41, 12);
            label2.TabIndex = 21;
            label2.Text = "已找到";
            // 
            // lblPairInfor
            // 
            lblPairInfor.AutoSize = true;
            lblPairInfor.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblPairInfor.Location = new System.Drawing.Point(4, 1);
            lblPairInfor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPairInfor.Name = "lblPairInfor";
            lblPairInfor.Size = new System.Drawing.Size(157, 17);
            lblPairInfor.TabIndex = 2;
            lblPairInfor.Text = "PairName:DIR1<--->DIR2";
            // 
            // pnlPairBasicInfor
            // 
            pnlPairBasicInfor.Controls.Add(lblPairInfor);
            pnlPairBasicInfor.Dock = System.Windows.Forms.DockStyle.Top;
            pnlPairBasicInfor.Location = new System.Drawing.Point(0, 0);
            pnlPairBasicInfor.Margin = new System.Windows.Forms.Padding(4);
            pnlPairBasicInfor.Name = "pnlPairBasicInfor";
            pnlPairBasicInfor.Size = new System.Drawing.Size(1073, 27);
            pnlPairBasicInfor.TabIndex = 21;
            // 
            // ctrl_PairPanal
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(TxtPairLog);
            Controls.Add(pnlPairBasicInfor);
            Controls.Add(pnlPairExInfor);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "ctrl_PairPanal";
            Size = new System.Drawing.Size(1073, 708);
            pnlPairExInfor.ResumeLayout(false);
            pnlPairExInfor.PerformLayout();
            pnlPairBasicInfor.ResumeLayout(false);
            pnlPairBasicInfor.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TxtPairLog;
        private System.Windows.Forms.Panel pnlPairExInfor;
        private System.Windows.Forms.Label lblPairInfor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAnalysisProgress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSyncProgress;
        private System.Windows.Forms.Label lblFileCountFound;
        private System.Windows.Forms.Panel pnlPairBasicInfor;
        private System.Windows.Forms.Label lblBeingSync;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNextSyncTime;
        private System.Windows.Forms.Label lblLastSyncTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblFileCountSync;
        private System.Windows.Forms.Label label3;
    }
}
