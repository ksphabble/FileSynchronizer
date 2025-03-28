namespace FileSynchronizer.Forms
{
    partial class frm_SQLRunner
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
            this.txtTableNames = new System.Windows.Forms.RichTextBox();
            this.txtSQLEditor = new System.Windows.Forms.RichTextBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClearResult = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRunSQL = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTableNames
            // 
            this.txtTableNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTableNames.Location = new System.Drawing.Point(12, 33);
            this.txtTableNames.Name = "txtTableNames";
            this.txtTableNames.ReadOnly = true;
            this.txtTableNames.Size = new System.Drawing.Size(184, 170);
            this.txtTableNames.TabIndex = 0;
            this.txtTableNames.Text = "";
            // 
            // txtSQLEditor
            // 
            this.txtSQLEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSQLEditor.Location = new System.Drawing.Point(202, 33);
            this.txtSQLEditor.Name = "txtSQLEditor";
            this.txtSQLEditor.Size = new System.Drawing.Size(740, 170);
            this.txtSQLEditor.TabIndex = 1;
            this.txtSQLEditor.Text = "";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(12, 238);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(930, 219);
            this.txtOutput.TabIndex = 2;
            this.txtOutput.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "数据表列表";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "SQL编辑器";
            // 
            // btnClearResult
            // 
            this.btnClearResult.Location = new System.Drawing.Point(69, 209);
            this.btnClearResult.Name = "btnClearResult";
            this.btnClearResult.Size = new System.Drawing.Size(75, 23);
            this.btnClearResult.TabIndex = 5;
            this.btnClearResult.Text = "清空输出";
            this.btnClearResult.UseVisualStyleBackColor = true;
            this.btnClearResult.Click += new System.EventHandler(this.btnClearResult_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 214);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "输出结果";
            // 
            // btnRunSQL
            // 
            this.btnRunSQL.Location = new System.Drawing.Point(265, 4);
            this.btnRunSQL.Name = "btnRunSQL";
            this.btnRunSQL.Size = new System.Drawing.Size(75, 23);
            this.btnRunSQL.TabIndex = 7;
            this.btnRunSQL.Text = "执行SQL";
            this.btnRunSQL.UseVisualStyleBackColor = true;
            this.btnRunSQL.Click += new System.EventHandler(this.btnRunSQL_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(12, 460);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(863, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "非常重要！非专业人士请勿使用本窗口，如果只是因为打开了调试模式发现的话请立即关闭，若因滥用本窗体功能导致的数据丢失，开发者恕不负责！";
            // 
            // frm_SQLRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 481);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnRunSQL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClearResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtSQLEditor);
            this.Controls.Add(this.txtTableNames);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_SQLRunner";
            this.ShowIcon = false;
            this.Text = "SQLRunner";
            this.Load += new System.EventHandler(this.frm_SQLRunner_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtTableNames;
        private System.Windows.Forms.RichTextBox txtSQLEditor;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClearResult;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRunSQL;
        private System.Windows.Forms.Label label4;
    }
}