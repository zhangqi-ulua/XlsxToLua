namespace TableStringChecker
{
    partial class MainForm
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
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.lbFormat = new System.Windows.Forms.Label();
            this.tbFormat = new System.Windows.Forms.TextBox();
            this.lbData = new System.Windows.Forms.Label();
            this.tbData = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.lbOutput = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAnalyze.Location = new System.Drawing.Point(192, 347);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(96, 36);
            this.btnAnalyze.TabIndex = 0;
            this.btnAnalyze.Text = "解析";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // lbFormat
            // 
            this.lbFormat.AutoSize = true;
            this.lbFormat.Location = new System.Drawing.Point(21, 28);
            this.lbFormat.Name = "lbFormat";
            this.lbFormat.Size = new System.Drawing.Size(41, 12);
            this.lbFormat.TabIndex = 1;
            this.lbFormat.Text = "格式：";
            // 
            // tbFormat
            // 
            this.tbFormat.Location = new System.Drawing.Point(60, 25);
            this.tbFormat.Multiline = true;
            this.tbFormat.Name = "tbFormat";
            this.tbFormat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFormat.Size = new System.Drawing.Size(388, 68);
            this.tbFormat.TabIndex = 2;
            // 
            // lbData
            // 
            this.lbData.AutoSize = true;
            this.lbData.Location = new System.Drawing.Point(21, 119);
            this.lbData.Name = "lbData";
            this.lbData.Size = new System.Drawing.Size(41, 12);
            this.lbData.TabIndex = 3;
            this.lbData.Text = "数据：";
            // 
            // tbData
            // 
            this.tbData.Location = new System.Drawing.Point(60, 116);
            this.tbData.Multiline = true;
            this.tbData.Name = "tbData";
            this.tbData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbData.Size = new System.Drawing.Size(388, 47);
            this.tbData.TabIndex = 4;
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(60, 187);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(388, 146);
            this.tbOutput.TabIndex = 5;
            // 
            // lbOutput
            // 
            this.lbOutput.AutoSize = true;
            this.lbOutput.Location = new System.Drawing.Point(21, 190);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(41, 12);
            this.lbOutput.TabIndex = 6;
            this.lbOutput.Text = "输出：";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 398);
            this.Controls.Add(this.lbOutput);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.lbData);
            this.Controls.Add(this.tbFormat);
            this.Controls.Add(this.lbFormat);
            this.Controls.Add(this.btnAnalyze);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TableString辅助检查工具 by张齐";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Label lbFormat;
        private System.Windows.Forms.TextBox tbFormat;
        private System.Windows.Forms.Label lbData;
        private System.Windows.Forms.TextBox tbData;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Label lbOutput;
    }
}

