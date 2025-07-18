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
            this.TxtPairLog = new System.Windows.Forms.RichTextBox();
            this.pnlPairExInfor = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblNextSyncTime = new System.Windows.Forms.Label();
            this.lblLastSyncTime = new System.Windows.Forms.Label();
            this.lblSyncProgress = new System.Windows.Forms.Label();
            this.lblFileCountSync = new System.Windows.Forms.Label();
            this.lblFileCountFound = new System.Windows.Forms.Label();
            this.lblAnalysisProgress = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblBeingSync = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPairInfor = new System.Windows.Forms.Label();
            this.pnlPairBasicInfor = new System.Windows.Forms.Panel();
            this.pnlPairExInfor.SuspendLayout();
            this.pnlPairBasicInfor.SuspendLayout();
            this.SuspendLayout();
            // 
            // TxtPairLog
            // 
            this.TxtPairLog.BackColor = System.Drawing.SystemColors.Info;
            this.TxtPairLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtPairLog.Location = new System.Drawing.Point(0, 19);
            this.TxtPairLog.Name = "TxtPairLog";
            this.TxtPairLog.ReadOnly = true;
            this.TxtPairLog.Size = new System.Drawing.Size(920, 448);
            this.TxtPairLog.TabIndex = 0;
            this.TxtPairLog.Text = "";
            // 
            // pnlPairExInfor
            // 
            this.pnlPairExInfor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPairExInfor.Controls.Add(this.progressBar1);
            this.pnlPairExInfor.Controls.Add(this.lblNextSyncTime);
            this.pnlPairExInfor.Controls.Add(this.lblLastSyncTime);
            this.pnlPairExInfor.Controls.Add(this.lblSyncProgress);
            this.pnlPairExInfor.Controls.Add(this.lblFileCountSync);
            this.pnlPairExInfor.Controls.Add(this.lblFileCountFound);
            this.pnlPairExInfor.Controls.Add(this.lblAnalysisProgress);
            this.pnlPairExInfor.Controls.Add(this.label7);
            this.pnlPairExInfor.Controls.Add(this.label9);
            this.pnlPairExInfor.Controls.Add(this.lblBeingSync);
            this.pnlPairExInfor.Controls.Add(this.label4);
            this.pnlPairExInfor.Controls.Add(this.label3);
            this.pnlPairExInfor.Controls.Add(this.label1);
            this.pnlPairExInfor.Controls.Add(this.label5);
            this.pnlPairExInfor.Controls.Add(this.label2);
            this.pnlPairExInfor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPairExInfor.Location = new System.Drawing.Point(0, 467);
            this.pnlPairExInfor.Name = "pnlPairExInfor";
            this.pnlPairExInfor.Size = new System.Drawing.Size(920, 33);
            this.pnlPairExInfor.TabIndex = 20;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(757, 19);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(162, 12);
            this.progressBar1.TabIndex = 31;
            // 
            // lblNextSyncTime
            // 
            this.lblNextSyncTime.AutoSize = true;
            this.lblNextSyncTime.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNextSyncTime.Location = new System.Drawing.Point(264, 3);
            this.lblNextSyncTime.Name = "lblNextSyncTime";
            this.lblNextSyncTime.Size = new System.Drawing.Size(95, 12);
            this.lblNextSyncTime.TabIndex = 29;
            this.lblNextSyncTime.Text = "(NextSyncTimeA)";
            // 
            // lblLastSyncTime
            // 
            this.lblLastSyncTime.AutoSize = true;
            this.lblLastSyncTime.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLastSyncTime.Location = new System.Drawing.Point(83, 3);
            this.lblLastSyncTime.Name = "lblLastSyncTime";
            this.lblLastSyncTime.Size = new System.Drawing.Size(95, 12);
            this.lblLastSyncTime.TabIndex = 27;
            this.lblLastSyncTime.Text = "(LastSyncTimeA)";
            // 
            // lblSyncProgress
            // 
            this.lblSyncProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSyncProgress.AutoSize = true;
            this.lblSyncProgress.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSyncProgress.Location = new System.Drawing.Point(874, 3);
            this.lblSyncProgress.Name = "lblSyncProgress";
            this.lblSyncProgress.Size = new System.Drawing.Size(41, 12);
            this.lblSyncProgress.TabIndex = 19;
            this.lblSyncProgress.Text = "(%%%%)";
            // 
            // lblFileCountSync
            // 
            this.lblFileCountSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileCountSync.AutoSize = true;
            this.lblFileCountSync.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFileCountSync.Location = new System.Drawing.Point(698, 3);
            this.lblFileCountSync.Name = "lblFileCountSync";
            this.lblFileCountSync.Size = new System.Drawing.Size(47, 12);
            this.lblFileCountSync.TabIndex = 23;
            this.lblFileCountSync.Text = "(FileA)";
            // 
            // lblFileCountFound
            // 
            this.lblFileCountFound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileCountFound.AutoSize = true;
            this.lblFileCountFound.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFileCountFound.Location = new System.Drawing.Point(511, 3);
            this.lblFileCountFound.Name = "lblFileCountFound";
            this.lblFileCountFound.Size = new System.Drawing.Size(47, 12);
            this.lblFileCountFound.TabIndex = 20;
            this.lblFileCountFound.Text = "(FileF)";
            // 
            // lblAnalysisProgress
            // 
            this.lblAnalysisProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnalysisProgress.AutoSize = true;
            this.lblAnalysisProgress.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAnalysisProgress.Location = new System.Drawing.Point(433, 3);
            this.lblAnalysisProgress.Name = "lblAnalysisProgress";
            this.lblAnalysisProgress.Size = new System.Drawing.Size(41, 12);
            this.lblAnalysisProgress.TabIndex = 17;
            this.lblAnalysisProgress.Text = "(%%%%)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 28;
            this.label7.Text = "最后同步时间：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(184, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 30;
            this.label9.Text = "下次同步时间：";
            // 
            // lblBeingSync
            // 
            this.lblBeingSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBeingSync.AutoSize = true;
            this.lblBeingSync.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblBeingSync.Location = new System.Drawing.Point(53, 19);
            this.lblBeingSync.Name = "lblBeingSync";
            this.lblBeingSync.Size = new System.Drawing.Size(60, 10);
            this.lblBeingSync.TabIndex = 26;
            this.lblBeingSync.Text = "(BeingSync)";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(3, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 10);
            this.label4.TabIndex = 25;
            this.label4.Text = "正在进行：";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(564, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "个文件或目录对象，其中";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(370, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "分析进度：";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(751, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 12);
            this.label5.TabIndex = 22;
            this.label5.Text = "个需同步，同步进度：";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(473, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "已找到";
            // 
            // lblPairInfor
            // 
            this.lblPairInfor.AutoSize = true;
            this.lblPairInfor.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPairInfor.Location = new System.Drawing.Point(3, 1);
            this.lblPairInfor.Name = "lblPairInfor";
            this.lblPairInfor.Size = new System.Drawing.Size(157, 17);
            this.lblPairInfor.TabIndex = 2;
            this.lblPairInfor.Text = "PairName:DIR1<--->DIR2";
            // 
            // pnlPairBasicInfor
            // 
            this.pnlPairBasicInfor.Controls.Add(this.lblPairInfor);
            this.pnlPairBasicInfor.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPairBasicInfor.Location = new System.Drawing.Point(0, 0);
            this.pnlPairBasicInfor.Name = "pnlPairBasicInfor";
            this.pnlPairBasicInfor.Size = new System.Drawing.Size(920, 19);
            this.pnlPairBasicInfor.TabIndex = 21;
            // 
            // ctrl_PairPanal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.TxtPairLog);
            this.Controls.Add(this.pnlPairBasicInfor);
            this.Controls.Add(this.pnlPairExInfor);
            this.Name = "ctrl_PairPanal";
            this.Size = new System.Drawing.Size(920, 500);
            this.pnlPairExInfor.ResumeLayout(false);
            this.pnlPairExInfor.PerformLayout();
            this.pnlPairBasicInfor.ResumeLayout(false);
            this.pnlPairBasicInfor.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
