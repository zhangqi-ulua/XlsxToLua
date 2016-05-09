using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace XlsxToLuaGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 部分文本框填入默认值
            tbClientFolderPath.Text = AppValues.NO_CLIENT_PATH_STRING;
            tbLangFilePath.Text = AppValues.NO_LONG_PARAM_STRING;
            // 查找本程序所在目录下是否含有XlsxToLua工具，如果有直接填写路径到“工具所在目录”文本框中
            string defaultPath = Utils.CombinePath(AppValues.PROGRAM_FOLDER_PATH, AppValues.PROGRAM_NAME);
            if (File.Exists(defaultPath))
                tbProgramPath.Text = defaultPath;
        }

        private void btnChooseExcelFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择Excel文件所在目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExcelFolderPath.Text = folderPath;
            }
        }

        private void btnChooseExportLuaFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择lua文件导出目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExportLuaFolderPath.Text = folderPath;
            }
        }

        private void btnChooseClientFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择项目Client所在目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbClientFolderPath.Text = folderPath;
            }
        }

        private void btnChooseLangFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择lang文件所在路径";
            dialog.Multiselect = false;
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                tbLangFilePath.Text = filePath;
            }
        }

        private void btnChoosePartExcel_Click(object sender, EventArgs e)
        {
            string excelFolderPath = tbExcelFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(excelFolderPath))
            {
                MessageBox.Show("请先指定Excel文件所在目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(excelFolderPath))
            {
                MessageBox.Show("指定Excel文件所在目录不存在，请重新设置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择要导出的Excel文件";
            dialog.InitialDirectory = excelFolderPath;
            dialog.Multiselect = true;
            dialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] filePaths = dialog.FileNames;
                // 检查选择的Excel文件是否在设置的Excel所在目录
                string checkFilePath = Path.GetDirectoryName(filePaths[0]);
                if (!checkFilePath.Equals(excelFolderPath, StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageBox.Show(string.Format("必须在指定的Excel文件所在目录中选择导出文件\n设置的Excel文件所在目录为：{0}\n而你选择的Excel所在目录为：{1}", excelFolderPath, checkFilePath), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                List<string> fileNames = new List<string>();
                foreach (string filePath in filePaths)
                    fileNames.Add(Path.GetFileNameWithoutExtension(filePath));

                string exportExcelFileParam = Utils.CombineString(fileNames, "|");
                tbPartExcelNames.Text = exportExcelFileParam;
            }
        }

        private void btnChooseProgramPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择XlsxToLua工具所在路径";
            dialog.Multiselect = false;
            dialog.Filter = "Exe files (*.exe)|*.exe";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                tbProgramPath.Text = filePath;
            }
        }

        private void cbUnchecked_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.CheckState == CheckState.Checked)
                cb.ForeColor = Color.Red;
            else
                cb.ForeColor = Color.Black;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (_CheckConfig() == true)
            {
                string programPath = tbProgramPath.Text.Trim();
                string batContent = _GetExecuteParamString();
                System.Diagnostics.Process.Start(programPath, batContent.Substring(batContent.IndexOf(programPath) + programPath.Length + 1));
            }
        }

        private void btnGenerateBat_Click(object sender, EventArgs e)
        {
            if (_CheckConfig() == true)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "请选择要生成的bat批处理脚本所在路径";
                dialog.InitialDirectory = Path.GetDirectoryName(tbProgramPath.Text.Trim());
                dialog.Filter = "Bat files (*.bat)|*.bat";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string batContent = _GetExecuteParamString();
                    string errorString = null;
                    Utils.SaveFile(filePath, batContent, out errorString);
                    if (string.IsNullOrEmpty(errorString))
                        MessageBox.Show("生成bat批处理脚本成功", "恭喜", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(string.Format("保存bat批处理脚本失败：{0}", errorString), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            if (_CheckConfig() == true)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "请选择配置文件的保存路径";
                dialog.InitialDirectory = AppValues.PROGRAM_FOLDER_PATH;
                dialog.Filter = "Config files (*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder configStringBuilder = new StringBuilder();

                    string programPath = tbProgramPath.Text.Trim();
                    string excelFolderPath = tbExcelFolderPath.Text.Trim();
                    string exportLuaFolderPath = tbExportLuaFolderPath.Text.Trim();
                    string clientFolderPath = tbClientFolderPath.Text.Trim();
                    string langFilePath = tbLangFilePath.Text.Trim();
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_PROGRAM_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(programPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(excelFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportLuaFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(clientFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_LANG_FILE_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(langFilePath);
                    string trueString = "true";
                    if (cbColumnInfo.CheckState == CheckState.Checked)
                        configStringBuilder.Append(AppValues.NEED_COLUMN_INFO_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(trueString);
                    if (cbUnchecked.CheckState == CheckState.Checked)
                        configStringBuilder.Append(AppValues.UNCHECKED_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(trueString);
                    if (cbPrintEmptyStringWhenLangNotMatching.CheckState == CheckState.Checked)
                        configStringBuilder.Append(AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(trueString);
                    if (cbExportMySQL.CheckState == CheckState.Checked)
                        configStringBuilder.Append(AppValues.EXPORT_MYSQL_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(trueString);
                    if (cbPart.CheckState == CheckState.Checked)
                        configStringBuilder.Append(AppValues.PART_EXPORT_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(tbPartExcelNames.Text.Trim());

                    string errorString = null;
                    Utils.SaveFile(dialog.FileName, configStringBuilder.ToString(), out errorString);
                    if (string.IsNullOrEmpty(errorString))
                        MessageBox.Show("保存配置文件成功", "恭喜", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(string.Format("保存配置文件失败：{0}", errorString), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择配置文件所在路径";
            dialog.InitialDirectory = AppValues.PROGRAM_FOLDER_PATH;
            dialog.Multiselect = false;
            dialog.Filter = "Config files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string errorString = null;
                Dictionary<string, string> config = TxtConfigReader.ParseTxtConfigFile(dialog.FileName, AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR, out errorString);
                if (!string.IsNullOrEmpty(errorString))
                {
                    MessageBox.Show(string.Format("打开配置文件失败：\n{0}", errorString), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (config.ContainsKey(AppValues.SAVE_CONFIG_KEY_PROGRAM_PATH))
                    tbProgramPath.Text = config[AppValues.SAVE_CONFIG_KEY_PROGRAM_PATH];
                if (config.ContainsKey(AppValues.SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH))
                    tbExcelFolderPath.Text = config[AppValues.SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH];
                if (config.ContainsKey(AppValues.SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH))
                    tbExportLuaFolderPath.Text = config[AppValues.SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH];
                if (config.ContainsKey(AppValues.SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH))
                    tbClientFolderPath.Text = config[AppValues.SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH];
                if (config.ContainsKey(AppValues.SAVE_CONFIG_KEY_LANG_FILE_PATH))
                    tbLangFilePath.Text = config[AppValues.SAVE_CONFIG_KEY_LANG_FILE_PATH];
                if (config.ContainsKey(AppValues.NEED_COLUMN_INFO_PARAM_STRING))
                    cbColumnInfo.CheckState = CheckState.Checked;
                if (config.ContainsKey(AppValues.UNCHECKED_PARAM_STRING))
                    cbUnchecked.CheckState = CheckState.Checked;
                if (config.ContainsKey(AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING))
                    cbPrintEmptyStringWhenLangNotMatching.CheckState = CheckState.Checked;
                if (config.ContainsKey(AppValues.EXPORT_MYSQL_PARAM_STRING))
                    cbExportMySQL.CheckState = CheckState.Checked;
                if (config.ContainsKey(AppValues.PART_EXPORT_PARAM_STRING))
                {
                    cbPart.CheckState = CheckState.Checked;
                    tbPartExcelNames.Text = config[AppValues.PART_EXPORT_PARAM_STRING];
                }

                MessageBox.Show("载入配置文件成功", "恭喜", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool _CheckConfig()
        {
            // 检查工具所在路径是否填写正确
            string programPath = tbProgramPath.Text.Trim();
            if (string.IsNullOrEmpty(programPath))
            {
                MessageBox.Show("必须指定XlsxToLua工具所在路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!File.Exists(programPath))
            {
                MessageBox.Show("指定的XlsxToLua工具所在路径不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!".exe".Equals(Path.GetExtension(programPath), StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("指定的XlsxToLua工具错误，不是一个有效的exe程序", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // 检查Excel文件所在目录是否填写正确
            string excelFolderPath = tbExcelFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(excelFolderPath))
            {
                MessageBox.Show("必须指定Excel文件所在目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Directory.Exists(excelFolderPath))
            {
                MessageBox.Show("指定的Excel文件所在目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // 检查lua文件导出目录是否填写正确
            string exportLuaFolderPath = tbExportLuaFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(exportLuaFolderPath))
            {
                MessageBox.Show("必须指定lua文件导出目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Directory.Exists(exportLuaFolderPath))
            {
                MessageBox.Show("指定的lua文件导出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // 检查项目Client所在目录是否填写正确
            string clientFolderPath = tbClientFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(clientFolderPath))
            {
                MessageBox.Show(string.Format("未指定Client所在目录（若无需指定，请填写{0}）", AppValues.NO_CLIENT_PATH_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbClientFolderPath.Text = AppValues.NO_CLIENT_PATH_STRING;
                return false;
            }
            if (!clientFolderPath.Equals(AppValues.NO_CLIENT_PATH_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!Directory.Exists(clientFolderPath))
                {
                    MessageBox.Show("指定的项目Client所在目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 检查lang文件所在路径是否填写正确
            string langFilePath = tbLangFilePath.Text.Trim();
            if (string.IsNullOrEmpty(langFilePath))
            {
                MessageBox.Show(string.Format("未指定lang文件所在路径（若无需指定，请填写{0}）", AppValues.NO_LONG_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbLangFilePath.Text = AppValues.NO_LONG_PARAM_STRING;
                return false;
            }
            if (!langFilePath.Equals(AppValues.NO_LONG_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!File.Exists(langFilePath))
                {
                    MessageBox.Show("指定的lang文件所在路径不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 若设置导出部分Excel文件，检查文件名声明是否正确
            if (cbPart.CheckState == CheckState.Checked)
            {
                string partExcelNames = tbPartExcelNames.Text.Trim();
                if (string.IsNullOrEmpty(partExcelNames))
                {
                    MessageBox.Show("勾选了导出部分Excel文件选项，就必须在文本框中填写要导出的Excel文件名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string[] fileNames = partExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> exportTableNames = new List<string>();
                foreach (string fileName in fileNames)
                    exportTableNames.Add(fileName.Trim());

                // 检查指定导出的Excel文件是否存在（注意不能直接用File.Exists判断是否存在，因为Windows会忽略声明的Excel文件名与实际文件名的大小写差异）
                List<string> existExcelFilePaths = new List<string>(Directory.GetFiles(excelFolderPath, "*.xlsx"));
                List<string> existExcelFileNames = new List<string>();
                foreach (string filePath in existExcelFilePaths)
                    existExcelFileNames.Add(Path.GetFileNameWithoutExtension(filePath));

                foreach (string exportExcelFileName in exportTableNames)
                {
                    if (!existExcelFileNames.Contains(exportExcelFileName))
                    {
                        MessageBox.Show(string.Format("指定要导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(excelFolderPath, string.Concat(exportExcelFileName, ".xlsx"))), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        private string _GetExecuteParamString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string programPath = tbProgramPath.Text.Trim();
            string excelFolderPath = tbExcelFolderPath.Text.Trim();
            string exportLuaFolderPath = tbExportLuaFolderPath.Text.Trim();
            string clientFolderPath = tbClientFolderPath.Text.Trim();
            string langFilePath = tbLangFilePath.Text.Trim();
            stringBuilder.AppendFormat("\"{0}\" ", programPath).AppendFormat("\"{0}\" ", excelFolderPath).AppendFormat("\"{0}\" ", exportLuaFolderPath).AppendFormat("\"{0}\" ", clientFolderPath).AppendFormat("\"{0}\" ", langFilePath);
            if (cbColumnInfo.CheckState == CheckState.Checked)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.NEED_COLUMN_INFO_PARAM_STRING);
            if (cbUnchecked.CheckState == CheckState.Checked)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.UNCHECKED_PARAM_STRING);
            if (cbPrintEmptyStringWhenLangNotMatching.CheckState == CheckState.Checked)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING);
            if (cbExportMySQL.CheckState == CheckState.Checked)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.EXPORT_MYSQL_PARAM_STRING);
            if (cbPart.CheckState == CheckState.Checked)
            {
                string partExcelNames = tbPartExcelNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.PART_EXPORT_PARAM_STRING, partExcelNames);
            }

            return stringBuilder.ToString();
        }
    }
}
