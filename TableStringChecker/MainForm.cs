using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TableStringChecker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            tbFormat.Text = "tableString[在这里填写格式定义]";

            // 导入lang文件内容
            if (File.Exists("lang.txt"))
            {
                string errorString = null;
                AppValues.LangData = TxtConfigReader.ParseTxtConfigFile("lang.txt", ":", out errorString);
                if (!string.IsNullOrEmpty(errorString))
                {
                    errorString = "lang.txt文件中存在错误，请修正后重新运行，程序被迫退出" + System.Environment.NewLine + errorString;
                    MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            else
                MessageBox.Show("未找到本程序所在目录下的lang.txt文件，将无法使用lang数据类型", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            string inputFormatString = tbFormat.Text.Trim();
            string inputDataString = tbData.Text;

            if (string.IsNullOrEmpty(inputFormatString))
            {
                MessageBox.Show("未输入格式定义", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(inputDataString))
            {
                MessageBox.Show("未输入数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 解析格式定义
            string errorString = null;
            TableStringFormatDefine formatDefine = TableAnalyzeHelper.GetTableStringFormatDefine(inputFormatString, out errorString);
            if (errorString != null)
            {
                tbOutput.Text = "格式声明错误，" + errorString;
                return;
            }
            // 生成lua table
            string exportString = TableExportToLuaHelper.GetTableStringValue(formatDefine, inputDataString, 0, out  errorString);
            if (errorString != null)
                tbOutput.Text = "输入数据错误，" + errorString;
            else
                tbOutput.Text = exportString;
        }
    }
}
