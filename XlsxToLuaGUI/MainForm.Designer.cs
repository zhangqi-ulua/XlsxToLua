namespace XlsxToLuaGUI
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
            this.lbExcelFolderPath = new System.Windows.Forms.Label();
            this.tbExcelFolderPath = new System.Windows.Forms.TextBox();
            this.btnChooseExcelFolderPath = new System.Windows.Forms.Button();
            this.lbExportLuaPath = new System.Windows.Forms.Label();
            this.tbExportLuaFolderPath = new System.Windows.Forms.TextBox();
            this.btnChooseExportLuaFolderPath = new System.Windows.Forms.Button();
            this.lbClientFolderPath = new System.Windows.Forms.Label();
            this.tbClientFolderPath = new System.Windows.Forms.TextBox();
            this.btnChooseClientFolderPath = new System.Windows.Forms.Button();
            this.lbLangFilePath = new System.Windows.Forms.Label();
            this.tbLangFilePath = new System.Windows.Forms.TextBox();
            this.btnChooseLangFilePath = new System.Windows.Forms.Button();
            this.lbExtraParam = new System.Windows.Forms.Label();
            this.cbColumnInfo = new System.Windows.Forms.CheckBox();
            this.cbUnchecked = new System.Windows.Forms.CheckBox();
            this.cbPrintEmptyStringWhenLangNotMatching = new System.Windows.Forms.CheckBox();
            this.cbExportMySQL = new System.Windows.Forms.CheckBox();
            this.cbPart = new System.Windows.Forms.CheckBox();
            this.tbPartExcelNames = new System.Windows.Forms.TextBox();
            this.btnChoosePartExcel = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnGenerateBat = new System.Windows.Forms.Button();
            this.lbProgramPath = new System.Windows.Forms.Label();
            this.tbProgramPath = new System.Windows.Forms.TextBox();
            this.btnChooseProgramPath = new System.Windows.Forms.Button();
            this.cbAllowedNullNumber = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lbExcelFolderPath
            // 
            this.lbExcelFolderPath.AutoSize = true;
            this.lbExcelFolderPath.Location = new System.Drawing.Point(13, 61);
            this.lbExcelFolderPath.Name = "lbExcelFolderPath";
            this.lbExcelFolderPath.Size = new System.Drawing.Size(119, 12);
            this.lbExcelFolderPath.TabIndex = 0;
            this.lbExcelFolderPath.Text = "Excel文件所在目录：";
            // 
            // tbExcelFolderPath
            // 
            this.tbExcelFolderPath.Location = new System.Drawing.Point(138, 58);
            this.tbExcelFolderPath.Name = "tbExcelFolderPath";
            this.tbExcelFolderPath.Size = new System.Drawing.Size(366, 21);
            this.tbExcelFolderPath.TabIndex = 1;
            // 
            // btnChooseExcelFolderPath
            // 
            this.btnChooseExcelFolderPath.Location = new System.Drawing.Point(519, 56);
            this.btnChooseExcelFolderPath.Name = "btnChooseExcelFolderPath";
            this.btnChooseExcelFolderPath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseExcelFolderPath.TabIndex = 2;
            this.btnChooseExcelFolderPath.Text = "选择";
            this.btnChooseExcelFolderPath.UseVisualStyleBackColor = true;
            this.btnChooseExcelFolderPath.Click += new System.EventHandler(this.btnChooseExcelFolderPath_Click);
            // 
            // lbExportLuaPath
            // 
            this.lbExportLuaPath.AutoSize = true;
            this.lbExportLuaPath.Location = new System.Drawing.Point(13, 95);
            this.lbExportLuaPath.Name = "lbExportLuaPath";
            this.lbExportLuaPath.Size = new System.Drawing.Size(107, 12);
            this.lbExportLuaPath.TabIndex = 3;
            this.lbExportLuaPath.Text = "lua文件导出目录：";
            // 
            // tbExportLuaFolderPath
            // 
            this.tbExportLuaFolderPath.Location = new System.Drawing.Point(138, 92);
            this.tbExportLuaFolderPath.Name = "tbExportLuaFolderPath";
            this.tbExportLuaFolderPath.Size = new System.Drawing.Size(366, 21);
            this.tbExportLuaFolderPath.TabIndex = 4;
            // 
            // btnChooseExportLuaFolderPath
            // 
            this.btnChooseExportLuaFolderPath.Location = new System.Drawing.Point(519, 90);
            this.btnChooseExportLuaFolderPath.Name = "btnChooseExportLuaFolderPath";
            this.btnChooseExportLuaFolderPath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseExportLuaFolderPath.TabIndex = 5;
            this.btnChooseExportLuaFolderPath.Text = "选择";
            this.btnChooseExportLuaFolderPath.UseVisualStyleBackColor = true;
            this.btnChooseExportLuaFolderPath.Click += new System.EventHandler(this.btnChooseExportLuaFolderPath_Click);
            // 
            // lbClientFolderPath
            // 
            this.lbClientFolderPath.AutoSize = true;
            this.lbClientFolderPath.Location = new System.Drawing.Point(13, 129);
            this.lbClientFolderPath.Name = "lbClientFolderPath";
            this.lbClientFolderPath.Size = new System.Drawing.Size(125, 12);
            this.lbClientFolderPath.TabIndex = 6;
            this.lbClientFolderPath.Text = "项目Client所在目录：";
            // 
            // tbClientFolderPath
            // 
            this.tbClientFolderPath.Location = new System.Drawing.Point(138, 126);
            this.tbClientFolderPath.Name = "tbClientFolderPath";
            this.tbClientFolderPath.Size = new System.Drawing.Size(366, 21);
            this.tbClientFolderPath.TabIndex = 7;
            // 
            // btnChooseClientFolderPath
            // 
            this.btnChooseClientFolderPath.Location = new System.Drawing.Point(519, 124);
            this.btnChooseClientFolderPath.Name = "btnChooseClientFolderPath";
            this.btnChooseClientFolderPath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseClientFolderPath.TabIndex = 8;
            this.btnChooseClientFolderPath.Text = "选择";
            this.btnChooseClientFolderPath.UseVisualStyleBackColor = true;
            this.btnChooseClientFolderPath.Click += new System.EventHandler(this.btnChooseClientFolderPath_Click);
            // 
            // lbLangFilePath
            // 
            this.lbLangFilePath.AutoSize = true;
            this.lbLangFilePath.Location = new System.Drawing.Point(13, 163);
            this.lbLangFilePath.Name = "lbLangFilePath";
            this.lbLangFilePath.Size = new System.Drawing.Size(113, 12);
            this.lbLangFilePath.TabIndex = 9;
            this.lbLangFilePath.Text = "lang文件所在路径：";
            // 
            // tbLangFilePath
            // 
            this.tbLangFilePath.Location = new System.Drawing.Point(138, 160);
            this.tbLangFilePath.Name = "tbLangFilePath";
            this.tbLangFilePath.Size = new System.Drawing.Size(366, 21);
            this.tbLangFilePath.TabIndex = 10;
            // 
            // btnChooseLangFilePath
            // 
            this.btnChooseLangFilePath.Location = new System.Drawing.Point(519, 158);
            this.btnChooseLangFilePath.Name = "btnChooseLangFilePath";
            this.btnChooseLangFilePath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseLangFilePath.TabIndex = 11;
            this.btnChooseLangFilePath.Text = "选择";
            this.btnChooseLangFilePath.UseVisualStyleBackColor = true;
            this.btnChooseLangFilePath.Click += new System.EventHandler(this.btnChooseLangFilePath_Click);
            // 
            // lbExtraParam
            // 
            this.lbExtraParam.AutoSize = true;
            this.lbExtraParam.Location = new System.Drawing.Point(13, 209);
            this.lbExtraParam.Name = "lbExtraParam";
            this.lbExtraParam.Size = new System.Drawing.Size(65, 12);
            this.lbExtraParam.TabIndex = 12;
            this.lbExtraParam.Text = "可选参数：";
            // 
            // cbColumnInfo
            // 
            this.cbColumnInfo.AutoSize = true;
            this.cbColumnInfo.Location = new System.Drawing.Point(15, 236);
            this.cbColumnInfo.Name = "cbColumnInfo";
            this.cbColumnInfo.Size = new System.Drawing.Size(432, 16);
            this.cbColumnInfo.TabIndex = 13;
            this.cbColumnInfo.Text = "-columnInfo（在生成lua文件的最上方用注释形式显示列信息，默认不开启）";
            this.cbColumnInfo.UseVisualStyleBackColor = true;
            // 
            // cbUnchecked
            // 
            this.cbUnchecked.AutoSize = true;
            this.cbUnchecked.Location = new System.Drawing.Point(15, 258);
            this.cbUnchecked.Name = "cbUnchecked";
            this.cbUnchecked.Size = new System.Drawing.Size(276, 16);
            this.cbUnchecked.TabIndex = 14;
            this.cbUnchecked.Text = "-unchecked（不对表格进行查错，不推荐使用）";
            this.cbUnchecked.UseVisualStyleBackColor = true;
            this.cbUnchecked.CheckedChanged += new System.EventHandler(this.cbUnchecked_CheckedChanged);
            // 
            // cbPrintEmptyStringWhenLangNotMatching
            // 
            this.cbPrintEmptyStringWhenLangNotMatching.Location = new System.Drawing.Point(15, 280);
            this.cbPrintEmptyStringWhenLangNotMatching.Name = "cbPrintEmptyStringWhenLangNotMatching";
            this.cbPrintEmptyStringWhenLangNotMatching.Size = new System.Drawing.Size(510, 31);
            this.cbPrintEmptyStringWhenLangNotMatching.TabIndex = 15;
            this.cbPrintEmptyStringWhenLangNotMatching.Text = "-printEmptyStringWhenLangNotMatching（当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串即" +
    "xx = \"\"，默认为输出nil）";
            this.cbPrintEmptyStringWhenLangNotMatching.UseVisualStyleBackColor = true;
            // 
            // cbExportMySQL
            // 
            this.cbExportMySQL.AutoSize = true;
            this.cbExportMySQL.Location = new System.Drawing.Point(15, 317);
            this.cbExportMySQL.Name = "cbExportMySQL";
            this.cbExportMySQL.Size = new System.Drawing.Size(366, 16);
            this.cbExportMySQL.TabIndex = 16;
            this.cbExportMySQL.Text = "-exportMySQL（将表格数据导出到MySQL数据库中，默认不导出）";
            this.cbExportMySQL.UseVisualStyleBackColor = true;
            // 
            // cbPart
            // 
            this.cbPart.AutoSize = true;
            this.cbPart.Location = new System.Drawing.Point(15, 361);
            this.cbPart.Name = "cbPart";
            this.cbPart.Size = new System.Drawing.Size(570, 16);
            this.cbPart.TabIndex = 17;
            this.cbPart.Text = "-part（后面在英文小括号内声明本次要导出的Excel文件名，用|分隔，未声明的文件将被本工具忽略）";
            this.cbPart.UseVisualStyleBackColor = true;
            // 
            // tbPartExcelNames
            // 
            this.tbPartExcelNames.Location = new System.Drawing.Point(69, 383);
            this.tbPartExcelNames.Name = "tbPartExcelNames";
            this.tbPartExcelNames.Size = new System.Drawing.Size(435, 21);
            this.tbPartExcelNames.TabIndex = 18;
            // 
            // btnChoosePartExcel
            // 
            this.btnChoosePartExcel.Location = new System.Drawing.Point(519, 381);
            this.btnChoosePartExcel.Name = "btnChoosePartExcel";
            this.btnChoosePartExcel.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePartExcel.TabIndex = 19;
            this.btnChoosePartExcel.Text = "选择";
            this.btnChoosePartExcel.UseVisualStyleBackColor = true;
            this.btnChoosePartExcel.Click += new System.EventHandler(this.btnChoosePartExcel_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(30, 434);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(75, 23);
            this.btnSaveConfig.TabIndex = 20;
            this.btnSaveConfig.Text = "保存配置";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(120, 434);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(75, 23);
            this.btnLoadConfig.TabIndex = 21;
            this.btnLoadConfig.Text = "载入配置";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExecute.Location = new System.Drawing.Point(242, 423);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(117, 39);
            this.btnExecute.TabIndex = 22;
            this.btnExecute.Text = "执行";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnGenerateBat
            // 
            this.btnGenerateBat.Location = new System.Drawing.Point(404, 434);
            this.btnGenerateBat.Name = "btnGenerateBat";
            this.btnGenerateBat.Size = new System.Drawing.Size(133, 23);
            this.btnGenerateBat.TabIndex = 23;
            this.btnGenerateBat.Text = "生成bat批处理脚本";
            this.btnGenerateBat.UseVisualStyleBackColor = true;
            this.btnGenerateBat.Click += new System.EventHandler(this.btnGenerateBat_Click);
            // 
            // lbProgramPath
            // 
            this.lbProgramPath.AutoSize = true;
            this.lbProgramPath.Location = new System.Drawing.Point(13, 27);
            this.lbProgramPath.Name = "lbProgramPath";
            this.lbProgramPath.Size = new System.Drawing.Size(89, 12);
            this.lbProgramPath.TabIndex = 24;
            this.lbProgramPath.Text = "工具所在路径：";
            // 
            // tbProgramPath
            // 
            this.tbProgramPath.Location = new System.Drawing.Point(138, 24);
            this.tbProgramPath.Name = "tbProgramPath";
            this.tbProgramPath.Size = new System.Drawing.Size(366, 21);
            this.tbProgramPath.TabIndex = 25;
            // 
            // btnChooseProgramPath
            // 
            this.btnChooseProgramPath.Location = new System.Drawing.Point(519, 22);
            this.btnChooseProgramPath.Name = "btnChooseProgramPath";
            this.btnChooseProgramPath.Size = new System.Drawing.Size(75, 23);
            this.btnChooseProgramPath.TabIndex = 26;
            this.btnChooseProgramPath.Text = "选择";
            this.btnChooseProgramPath.UseVisualStyleBackColor = true;
            this.btnChooseProgramPath.Click += new System.EventHandler(this.btnChooseProgramPath_Click);
            // 
            // cbAllowedNullNumber
            // 
            this.cbAllowedNullNumber.AutoSize = true;
            this.cbAllowedNullNumber.Location = new System.Drawing.Point(15, 339);
            this.cbAllowedNullNumber.Name = "cbAllowedNullNumber";
            this.cbAllowedNullNumber.Size = new System.Drawing.Size(408, 16);
            this.cbAllowedNullNumber.TabIndex = 27;
            this.cbAllowedNullNumber.Text = "-allowedNullNumber（允许int、float型字段下填写空值，不推荐使用）";
            this.cbAllowedNullNumber.UseVisualStyleBackColor = true;
            this.cbAllowedNullNumber.CheckedChanged += new System.EventHandler(this.cbAllowedNullNumber_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 480);
            this.Controls.Add(this.cbAllowedNullNumber);
            this.Controls.Add(this.btnChooseProgramPath);
            this.Controls.Add(this.tbProgramPath);
            this.Controls.Add(this.lbProgramPath);
            this.Controls.Add(this.btnGenerateBat);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.btnChoosePartExcel);
            this.Controls.Add(this.tbPartExcelNames);
            this.Controls.Add(this.cbPart);
            this.Controls.Add(this.cbExportMySQL);
            this.Controls.Add(this.cbPrintEmptyStringWhenLangNotMatching);
            this.Controls.Add(this.cbUnchecked);
            this.Controls.Add(this.cbColumnInfo);
            this.Controls.Add(this.lbExtraParam);
            this.Controls.Add(this.btnChooseLangFilePath);
            this.Controls.Add(this.tbLangFilePath);
            this.Controls.Add(this.lbLangFilePath);
            this.Controls.Add(this.btnChooseClientFolderPath);
            this.Controls.Add(this.tbClientFolderPath);
            this.Controls.Add(this.lbClientFolderPath);
            this.Controls.Add(this.btnChooseExportLuaFolderPath);
            this.Controls.Add(this.tbExportLuaFolderPath);
            this.Controls.Add(this.lbExportLuaPath);
            this.Controls.Add(this.btnChooseExcelFolderPath);
            this.Controls.Add(this.tbExcelFolderPath);
            this.Controls.Add(this.lbExcelFolderPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XlsxToLua GUI by 张齐";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbExcelFolderPath;
        private System.Windows.Forms.TextBox tbExcelFolderPath;
        private System.Windows.Forms.Button btnChooseExcelFolderPath;
        private System.Windows.Forms.Label lbExportLuaPath;
        private System.Windows.Forms.TextBox tbExportLuaFolderPath;
        private System.Windows.Forms.Button btnChooseExportLuaFolderPath;
        private System.Windows.Forms.Label lbClientFolderPath;
        private System.Windows.Forms.TextBox tbClientFolderPath;
        private System.Windows.Forms.Button btnChooseClientFolderPath;
        private System.Windows.Forms.Label lbLangFilePath;
        private System.Windows.Forms.TextBox tbLangFilePath;
        private System.Windows.Forms.Button btnChooseLangFilePath;
        private System.Windows.Forms.Label lbExtraParam;
        private System.Windows.Forms.CheckBox cbColumnInfo;
        private System.Windows.Forms.CheckBox cbUnchecked;
        private System.Windows.Forms.CheckBox cbPrintEmptyStringWhenLangNotMatching;
        private System.Windows.Forms.CheckBox cbExportMySQL;
        private System.Windows.Forms.CheckBox cbPart;
        private System.Windows.Forms.TextBox tbPartExcelNames;
        private System.Windows.Forms.Button btnChoosePartExcel;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnGenerateBat;
        private System.Windows.Forms.Label lbProgramPath;
        private System.Windows.Forms.TextBox tbProgramPath;
        private System.Windows.Forms.Button btnChooseProgramPath;
        private System.Windows.Forms.CheckBox cbAllowedNullNumber;
    }
}

