namespace FileSynchronizer
{
    partial class ctrl_PairPanal
    {
        private bool _disposed = false;

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
            if (!_disposed)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                _disposed = true;

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
            pnlPairExInfor = new System.Windows.Forms.Panel();
            labelProgress = new System.Windows.Forms.Label();
            lblBeingSync = new System.Windows.Forms.Label();
            lblPairInfor = new System.Windows.Forms.Label();
            pnlPairBasicInfor = new System.Windows.Forms.Panel();
            btnStopMonitoringRMD = new System.Windows.Forms.Button();
            TxtPairLog = new System.Windows.Forms.RichTextBox();
            btnRefreshFileAndDir = new System.Windows.Forms.Button();
            pnlPairExInfor.SuspendLayout();
            pnlPairBasicInfor.SuspendLayout();
            SuspendLayout();
            // 
            // pnlPairExInfor
            // 
            pnlPairExInfor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlPairExInfor.Controls.Add(labelProgress);
            pnlPairExInfor.Controls.Add(lblBeingSync);
            pnlPairExInfor.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlPairExInfor.Location = new System.Drawing.Point(0, 682);
            pnlPairExInfor.Margin = new System.Windows.Forms.Padding(4);
            pnlPairExInfor.Name = "pnlPairExInfor";
            pnlPairExInfor.Size = new System.Drawing.Size(1073, 26);
            pnlPairExInfor.TabIndex = 20;
            // 
            // labelProgress
            // 
            labelProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelProgress.AutoSize = true;
            labelProgress.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            labelProgress.Location = new System.Drawing.Point(0, 8);
            labelProgress.Margin = new System.Windows.Forms.Padding(0);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new System.Drawing.Size(341, 12);
            labelProgress.TabIndex = 18;
            labelProgress.Text = "已分析{0:p}，找到{1}个对象，其中{2}个需同步，已同步{3:p}";
            // 
            // lblBeingSync
            // 
            lblBeingSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblBeingSync.AutoSize = true;
            lblBeingSync.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblBeingSync.Location = new System.Drawing.Point(341, 9);
            lblBeingSync.Margin = new System.Windows.Forms.Padding(0);
            lblBeingSync.Name = "lblBeingSync";
            lblBeingSync.Size = new System.Drawing.Size(110, 10);
            lblBeingSync.TabIndex = 26;
            lblBeingSync.Text = "正在进行：(BeingSync)";
            // 
            // lblPairInfor
            // 
            lblPairInfor.AutoSize = true;
            lblPairInfor.Font = new System.Drawing.Font("微软雅黑", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblPairInfor.Location = new System.Drawing.Point(4, 4);
            lblPairInfor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPairInfor.Name = "lblPairInfor";
            lblPairInfor.Size = new System.Drawing.Size(180, 20);
            lblPairInfor.TabIndex = 2;
            lblPairInfor.Text = "PairName:DIR1<--->DIR2";
            // 
            // pnlPairBasicInfor
            // 
            pnlPairBasicInfor.Controls.Add(btnRefreshFileAndDir);
            pnlPairBasicInfor.Controls.Add(btnStopMonitoringRMD);
            pnlPairBasicInfor.Controls.Add(lblPairInfor);
            pnlPairBasicInfor.Dock = System.Windows.Forms.DockStyle.Top;
            pnlPairBasicInfor.Location = new System.Drawing.Point(0, 0);
            pnlPairBasicInfor.Margin = new System.Windows.Forms.Padding(4);
            pnlPairBasicInfor.Name = "pnlPairBasicInfor";
            pnlPairBasicInfor.Size = new System.Drawing.Size(1073, 27);
            pnlPairBasicInfor.TabIndex = 21;
            // 
            // btnStopMonitoringRMD
            // 
            btnStopMonitoringRMD.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStopMonitoringRMD.Location = new System.Drawing.Point(873, 3);
            btnStopMonitoringRMD.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            btnStopMonitoringRMD.Name = "btnStopMonitoringRMD";
            btnStopMonitoringRMD.Size = new System.Drawing.Size(100, 23);
            btnStopMonitoringRMD.TabIndex = 3;
            btnStopMonitoringRMD.Text = "停止监控(%s1)";
            btnStopMonitoringRMD.UseVisualStyleBackColor = true;
            btnStopMonitoringRMD.Click += btnStopMonitoringRMD_Click;
            // 
            // TxtPairLog
            // 
            TxtPairLog.BackColor = System.Drawing.SystemColors.Info;
            TxtPairLog.Dock = System.Windows.Forms.DockStyle.Fill;
            TxtPairLog.Location = new System.Drawing.Point(0, 27);
            TxtPairLog.Margin = new System.Windows.Forms.Padding(4);
            TxtPairLog.Name = "TxtPairLog";
            TxtPairLog.ReadOnly = true;
            TxtPairLog.Size = new System.Drawing.Size(1073, 655);
            TxtPairLog.TabIndex = 0;
            TxtPairLog.Text = "";
            // 
            // btnRefreshFileAndDir
            // 
            btnRefreshFileAndDir.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRefreshFileAndDir.Location = new System.Drawing.Point(973, 3);
            btnRefreshFileAndDir.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            btnRefreshFileAndDir.Name = "btnRefreshFileAndDir";
            btnRefreshFileAndDir.Size = new System.Drawing.Size(100, 23);
            btnRefreshFileAndDir.TabIndex = 4;
            btnRefreshFileAndDir.Text = "刷新配对内容";
            btnRefreshFileAndDir.UseVisualStyleBackColor = true;
            btnRefreshFileAndDir.Click += btnRefreshFileAndDir_Click;
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
        private System.Windows.Forms.Panel pnlPairExInfor;
        private System.Windows.Forms.Label lblPairInfor;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Panel pnlPairBasicInfor;
        private System.Windows.Forms.Label lblBeingSync;
        private System.Windows.Forms.Button btnStopMonitoringRMD;
        private System.Windows.Forms.RichTextBox TxtPairLog;
        private System.Windows.Forms.Button btnRefreshFileAndDir;
    }
}
