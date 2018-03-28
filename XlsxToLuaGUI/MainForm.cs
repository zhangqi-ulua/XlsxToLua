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
            tbLangFilePath.Text = AppValues.NO_LANG_PARAM_STRING;
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
            string errorString = null;
            string chooseExcelNames = _GetChoosePartExcelFile(tbPartExcelNames.Text, out errorString);
            if (errorString == null)
            {
                if (chooseExcelNames != null)
                    tbPartExcelNames.Text = chooseExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnChooseExceptExcel_Click(object sender, EventArgs e)
        {
            string errorString = null;
            string exceptExcelNames = _GetChoosePartExcelFile(tbExceptExcelNames.Text, out errorString);
            if (errorString == null)
            {
                if (exceptExcelNames != null)
                    tbExceptExcelNames.Text = exceptExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _WarnWhenChooseDangerousParam(cb);
        }

        private void cbAllowedNullNumber_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            _WarnWhenChooseDangerousParam(cb);
        }

        private void btnChooseExportCsvFile_Click(object sender, EventArgs e)
        {
            string errorString = null;
            string chooseExcelNames = _GetChoosePartExcelFile(tbExportCsvTableNames.Text, out errorString);
            if (errorString == null)
            {
                if (chooseExcelNames != null)
                    tbExportCsvTableNames.Text = chooseExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnChooseExportCsvFilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择csv文件导出目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExportCsvFilePath.Text = folderPath;
            }
        }

        private void btnChooseExportCsClassFile_Click(object sender, EventArgs e)
        {
            string errorString = null;
            string chooseExcelNames = _GetChoosePartExcelFile(tbExportCsClassTableNames.Text, out errorString);
            if (errorString == null)
            {
                if (chooseExcelNames != null)
                    tbExportCsClassTableNames.Text = chooseExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnChooseExportCsClassFilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择csv对应C#类文件导出目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExportCsClassFilePath.Text = folderPath;
            }
        }

        private void btnChooseExportJavaClassFile_Click(object sender, EventArgs e)
        {
            string errorString = null;
            string chooseExcelNames = _GetChoosePartExcelFile(tbExportJavaClassTableNames.Text, out errorString);
            if (errorString == null)
            {
                if (chooseExcelNames != null)
                    tbExportJavaClassTableNames.Text = chooseExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnChooseExportJavaClassFilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择csv对应Java类文件导出目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExportJavaClassFilePath.Text = folderPath;
            }
        }

        private void btnChooseExportJsonFile_Click(object sender, EventArgs e)
        {
            string errorString = null;
            string chooseExcelNames = _GetChoosePartExcelFile(tbExportJsonTableNames.Text, out errorString);
            if (errorString == null)
            {
                if (chooseExcelNames != null)
                    tbExportJsonTableNames.Text = chooseExcelNames;
            }
            else
                MessageBox.Show(errorString, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnChooseExportJsonFilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择json文件导出目录";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;
                tbExportJsonFilePath.Text = folderPath;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (_CheckConfig() == true)
            {
                string programPath = tbProgramPath.Text.Trim();
                string batContent = _GetExecuteParamString();
                // System.Diagnostics.Process.Start函数无法识别用/分层的相对路径，故需进行转换
                System.Diagnostics.Process.Start(programPath.Replace('/', '\\'), batContent.Substring(batContent.IndexOf(programPath) + programPath.Length + 1));
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
                    Uri programUri = new Uri(AppValues.PROGRAM_PATH);

                    string programPath = tbProgramPath.Text.Trim();
                    string excelFolderPath = tbExcelFolderPath.Text.Trim();
                    string exportLuaFolderPath = tbExportLuaFolderPath.Text.Trim();
                    string clientFolderPath = tbClientFolderPath.Text.Trim();
                    string langFilePath = tbLangFilePath.Text.Trim();
                    if (cbIsUseRelativePath.Checked == true)
                    {
                        if (Path.IsPathRooted(programPath))
                            programPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(programPath)).ToString());
                        if (Path.IsPathRooted(excelFolderPath))
                            excelFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(excelFolderPath)).ToString());
                        if (Path.IsPathRooted(exportLuaFolderPath))
                            exportLuaFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportLuaFolderPath)).ToString());
                        if (!clientFolderPath.Equals(AppValues.NO_CLIENT_PATH_STRING, StringComparison.CurrentCultureIgnoreCase) && Path.IsPathRooted(clientFolderPath))
                            clientFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(clientFolderPath)).ToString());

                        if (!langFilePath.Equals(AppValues.NO_LANG_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase) && Path.IsPathRooted(langFilePath))
                            langFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(langFilePath)).ToString());
                    }
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_PROGRAM_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(programPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(excelFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportLuaFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(clientFolderPath);
                    configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_LANG_FILE_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(langFilePath);

                    string partExcelNames = tbPartExcelNames.Text.Trim();
                    if (!string.IsNullOrEmpty(partExcelNames))
                        configStringBuilder.Append(AppValues.PART_EXPORT_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(partExcelNames);

                    string exceptExcelNames = tbExceptExcelNames.Text.Trim();
                    if (!string.IsNullOrEmpty(exceptExcelNames))
                        configStringBuilder.Append(AppValues.EXCEPT_EXPORT_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exceptExcelNames);

                    string exportCsvExcelNames = tbExportCsvTableNames.Text.Trim();
                    if (!string.IsNullOrEmpty(exportCsvExcelNames))
                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportCsvExcelNames);

                    string exportCsvFilePath = tbExportCsvFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(exportCsvFilePath))
                    {
                        if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportCsvFilePath))
                            exportCsvFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportCsvFilePath)).ToString());

                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXPORT_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportCsvFilePath);
                    }

                    string csvFileExtension = tbCsvFileExtension.Text.Trim();
                    if (!string.IsNullOrEmpty(csvFileExtension))
                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXTENSION).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csvFileExtension);

                    string csvFileSplitString = tbCsvFileSplitString.Text;
                    if (!string.IsNullOrEmpty(csvFileSplitString))
                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CSV_PARAM_SUBTYPE_SPLIT_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csvFileSplitString);

                    string exportCsClassExcelNames = tbExportCsClassTableNames.Text.Trim();
                    if (!string.IsNullOrEmpty(exportCsClassExcelNames))
                        configStringBuilder.Append(AppValues.EXPORT_CS_CLASS_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportCsClassExcelNames);

                    string exportCsClassFilePath = tbExportCsClassFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(exportCsClassFilePath))
                    {
                        if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportCsClassFilePath))
                            exportCsClassFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportCsClassFilePath)).ToString());

                        configStringBuilder.Append(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_EXPORT_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportCsClassFilePath);
                    }

                    string csClassNamespace = tbExportCsClassNamespace.Text.Trim();
                    if (!string.IsNullOrEmpty(csClassNamespace))
                        configStringBuilder.Append(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_NAMESPACE).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csClassNamespace);

                    string csClassUsing = tbExportCsClassUsing.Text.Trim();
                    if (!string.IsNullOrEmpty(csClassUsing))
                        configStringBuilder.Append(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_USING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csClassUsing);

                    string exportJavaClassExcelNames = tbExportJavaClassTableNames.Text.Trim();
                    if (!string.IsNullOrEmpty(exportJavaClassExcelNames))
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportJavaClassExcelNames);

                    string exportJavaClassFilePath = tbExportJavaClassFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(exportJavaClassFilePath))
                    {
                        if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportJavaClassFilePath))
                            exportJavaClassFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportJavaClassFilePath)).ToString());

                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_EXPORT_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportJavaClassFilePath);
                    }

                    string javaClassPackage = tbExportJavaClassPackage.Text.Trim();
                    if (!string.IsNullOrEmpty(javaClassPackage))
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_PACKAGE).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(javaClassPackage);

                    string javaClassImport = tbExportJavaClassImport.Text.Trim();
                    if (!string.IsNullOrEmpty(javaClassImport))
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IMPORT).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(javaClassImport);

                    string csvClassNamePrefix = tbExportCsvClassNamePrefix.Text.Trim();
                    if (!string.IsNullOrEmpty(csvClassNamePrefix))
                        configStringBuilder.Append(AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_PREFIX).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csvClassNamePrefix);

                    string csvClassNamePostfix = tbExportCsvClassNamePostfix.Text.Trim();
                    if (!string.IsNullOrEmpty(csvClassNamePostfix))
                        configStringBuilder.Append(AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_POSTFIX).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(csvClassNamePostfix);

                    string exportJsonExcelNames = tbExportJsonTableNames.Text.Trim();
                    if (!string.IsNullOrEmpty(exportJsonExcelNames))
                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportJsonExcelNames);

                    string exportJsonFilePath = tbExportJsonFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(exportJsonFilePath))
                    {
                        if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportJsonFilePath))
                            exportJsonFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportJsonFilePath)).ToString());

                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXPORT_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(exportJsonFilePath);
                    }

                    string jsonFileExtension = tbJsonFileExtension.Text.Trim();
                    if (!string.IsNullOrEmpty(jsonFileExtension))
                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXTENSION).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(jsonFileExtension);

                    const string TRUE_STRING = "true";
                    if (cbExportIncludeSubfolder.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_INCLUDE_SUBFOLDER_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportKeepDirectoryStructure.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_KEEP_DIRECTORY_STRUCTURE_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbColumnInfo.Checked == true)
                        configStringBuilder.Append(AppValues.NEED_COLUMN_INFO_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbUnchecked.Checked == true)
                        configStringBuilder.Append(AppValues.UNCHECKED_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbPrintEmptyStringWhenLangNotMatching.Checked == true)
                        configStringBuilder.Append(AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportMySQL.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_MYSQL_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbAllowedNullNumber.Checked == true)
                        configStringBuilder.Append(AppValues.ALLOWED_NULL_NUMBER_PARAM_STRING).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);

                    if (cbPart.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_PART).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExcept.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXCEPT).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportCsv.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_CSV).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsExportCsvColumnName.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_NAME).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsExportCsvColumnDataType.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_CSV_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_DATA_TYPE).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportCsClass.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_CS_CLASS).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportJavaClass.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_JAVA_CLASS).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportJson.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_JSON).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsExportJsonWithFormat.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_FORMAT).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsExportJsonArrayFormat.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_EXPORT_JSON_ARRAY_FORMAT).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsExportJsonMapIncludeKeyColumnValue.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JSON_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_MAP_INCLUDE_KEY_COLUMN_VALUE).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportJavaClassIsUseDate.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_USE_DATE).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportJavaClassIsGenerateConstructorWithoutFields.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbExportJavaClassIsGenerateConstructorWithAllFields.Checked == true)
                        configStringBuilder.Append(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING).Append(AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR).Append(AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);
                    if (cbIsUseRelativePath.Checked == true)
                        configStringBuilder.Append(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_USE_RELATIVE_PATH).Append(AppValues.SAVE_CONFIG_KEY_VALUE_SEPARATOR).AppendLine(TRUE_STRING);

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
                if (config.ContainsKey(AppValues.EXPORT_INCLUDE_SUBFOLDER_PARAM_STRING))
                    cbExportIncludeSubfolder.Checked = true;
                if (config.ContainsKey(AppValues.EXPORT_KEEP_DIRECTORY_STRUCTURE_PARAM_STRING))
                    cbExportKeepDirectoryStructure.Checked = true;
                if (config.ContainsKey(AppValues.NEED_COLUMN_INFO_PARAM_STRING))
                    cbColumnInfo.Checked = true;
                if (config.ContainsKey(AppValues.UNCHECKED_PARAM_STRING))
                    cbUnchecked.Checked = true;
                if (config.ContainsKey(AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING))
                    cbPrintEmptyStringWhenLangNotMatching.Checked = true;
                if (config.ContainsKey(AppValues.EXPORT_MYSQL_PARAM_STRING))
                    cbExportMySQL.Checked = true;
                if (config.ContainsKey(AppValues.ALLOWED_NULL_NUMBER_PARAM_STRING))
                    cbAllowedNullNumber.Checked = true;
                if (config.ContainsKey(AppValues.PART_EXPORT_PARAM_STRING))
                    tbPartExcelNames.Text = config[AppValues.PART_EXPORT_PARAM_STRING];
                if (config.ContainsKey(AppValues.EXCEPT_EXPORT_PARAM_STRING))
                    tbExceptExcelNames.Text = config[AppValues.EXCEPT_EXPORT_PARAM_STRING];
                if (config.ContainsKey(AppValues.EXPORT_CSV_PARAM_STRING))
                    tbExportCsvTableNames.Text = config[AppValues.EXPORT_CSV_PARAM_STRING];
                if (config.ContainsKey(AppValues.EXPORT_CS_CLASS_PARAM_STRING))
                    tbExportCsClassTableNames.Text = config[AppValues.EXPORT_CS_CLASS_PARAM_STRING];
                if (config.ContainsKey(AppValues.EXPORT_JAVA_CLASS_PARAM_STRING))
                    tbExportJavaClassTableNames.Text = config[AppValues.EXPORT_JAVA_CLASS_PARAM_STRING];
                if (config.ContainsKey(AppValues.EXPORT_JSON_PARAM_STRING))
                    tbExportJsonTableNames.Text = config[AppValues.EXPORT_JSON_PARAM_STRING];

                string exportCsvFilePathKey = string.Concat(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXPORT_PATH);
                if (config.ContainsKey(exportCsvFilePathKey))
                    tbExportCsvFilePath.Text = config[exportCsvFilePathKey];

                string csvFileExtensionKey = string.Concat(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXTENSION);
                if (config.ContainsKey(csvFileExtensionKey))
                    tbCsvFileExtension.Text = config[csvFileExtensionKey];

                string csvFileSplitStringKey = string.Concat(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CSV_PARAM_SUBTYPE_SPLIT_STRING);
                if (config.ContainsKey(csvFileSplitStringKey))
                    tbCsvFileSplitString.Text = config[csvFileSplitStringKey];

                string exportCsClassFilePathKey = string.Concat(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_EXPORT_PATH);
                if (config.ContainsKey(exportCsClassFilePathKey))
                    tbExportCsClassFilePath.Text = config[exportCsClassFilePathKey];

                string exportCsClassNamespace = string.Concat(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_NAMESPACE);
                if (config.ContainsKey(exportCsClassNamespace))
                    tbExportCsClassNamespace.Text = config[exportCsClassNamespace];

                string exportCsClassUsing = string.Concat(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_USING);
                if (config.ContainsKey(exportCsClassUsing))
                    tbExportCsClassUsing.Text = config[exportCsClassUsing];

                string exportJavaClassFilePathKey = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_EXPORT_PATH);
                if (config.ContainsKey(exportJavaClassFilePathKey))
                    tbExportJavaClassFilePath.Text = config[exportJavaClassFilePathKey];

                string exportJavaClassPackage = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_PACKAGE);
                if (config.ContainsKey(exportJavaClassPackage))
                    tbExportJavaClassPackage.Text = config[exportJavaClassPackage];

                string exportJavaClassImport = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IMPORT);
                if (config.ContainsKey(exportJavaClassImport))
                    tbExportJavaClassImport.Text = config[exportJavaClassImport];

                string exportCsvClassNamePrefix = string.Concat(AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_PREFIX);
                if (config.ContainsKey(exportCsvClassNamePrefix))
                    tbExportCsvClassNamePrefix.Text = config[exportCsvClassNamePrefix];

                string exportCsvClassNamePostfix = string.Concat(AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_POSTFIX);
                if (config.ContainsKey(exportCsvClassNamePostfix))
                    tbExportCsvClassNamePostfix.Text = config[exportCsvClassNamePostfix];

                string exportJsonFilePathKey = string.Concat(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXPORT_PATH);
                if (config.ContainsKey(exportJsonFilePathKey))
                    tbExportJsonFilePath.Text = config[exportJsonFilePathKey];

                string jsonFileExtensionKey = string.Concat(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXTENSION);
                if (config.ContainsKey(jsonFileExtensionKey))
                    tbJsonFileExtension.Text = config[jsonFileExtensionKey];

                cbPart.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_PART);
                cbExcept.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXCEPT);
                cbExportCsv.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_CSV);
                cbExportCsClass.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_CS_CLASS);
                cbExportJavaClass.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_JAVA_CLASS);
                cbExportJson.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_JSON);
                cbIsUseRelativePath.Checked = config.ContainsKey(AppValues.SAVE_CONFIG_KEY_IS_CHECKED_USE_RELATIVE_PATH);
                string isExportCsvColumnNameKey = string.Concat(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_NAME);
                cbIsExportCsvColumnName.Checked = config.ContainsKey(isExportCsvColumnNameKey);
                string isExportCsvColumnDataTypeKey = string.Concat(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_DATA_TYPE);
                cbIsExportCsvColumnDataType.Checked = config.ContainsKey(isExportCsvColumnDataTypeKey);
                string isExportJsonWithFormatKey = string.Concat(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_FORMAT);
                cbIsExportJsonWithFormat.Checked = config.ContainsKey(isExportJsonWithFormatKey);
                string isExportJsonArrayFormat = string.Concat(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_EXPORT_JSON_ARRAY_FORMAT);
                cbIsExportJsonArrayFormat.Checked = config.ContainsKey(isExportJsonArrayFormat);
                string isExportJsonMapIncludeKeyColumnValue = string.Concat(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_MAP_INCLUDE_KEY_COLUMN_VALUE);
                cbIsExportJsonMapIncludeKeyColumnValue.Checked = config.ContainsKey(isExportJsonMapIncludeKeyColumnValue);
                string exportJavaClassIsUseDate = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_USE_DATE);
                cbExportJavaClassIsUseDate.Checked = config.ContainsKey(exportJavaClassIsUseDate);
                string exportJavaClassIsGenerateConstructorWithoutFields = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS);
                cbExportJavaClassIsGenerateConstructorWithoutFields.Checked = config.ContainsKey(exportJavaClassIsGenerateConstructorWithoutFields);
                string exportJavaClassIsGenerateConstructorWithAllFields = string.Concat(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS);
                cbExportJavaClassIsGenerateConstructorWithAllFields.Checked = config.ContainsKey(exportJavaClassIsGenerateConstructorWithAllFields);

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
            if (programPath.Contains(AppValues.GUI_PROGRAM_NAME))
            {
                MessageBox.Show(string.Format("需要指定的是{0}所在路径而不是{1}所在路径", AppValues.PROGRAM_NAME, AppValues.GUI_PROGRAM_NAME), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // 检查Excel文件夹及其下属子文件夹中是否存在同名文件
            // 记录Excel文件夹及其子文件夹中的文件名对应的所在路径（key：表名， value：文件所在路径）
            Dictionary<string, string> tableNameAndPath = new Dictionary<string, string>();
            // 记录重名文件所在目录
            Dictionary<string, List<string>> sameExcelNameInfo = new Dictionary<string, List<string>>();
            if (cbExportIncludeSubfolder.Checked == true)
            {
                foreach (string filePath in Directory.GetFiles(excelFolderPath, "*.xlsx", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (fileName.StartsWith(AppValues.EXCEL_TEMP_FILE_FILE_NAME_START_STRING))
                        continue;

                    if (tableNameAndPath.ContainsKey(fileName))
                    {
                        if (!sameExcelNameInfo.ContainsKey(fileName))
                        {
                            sameExcelNameInfo.Add(fileName, new List<string>());
                            sameExcelNameInfo[fileName].Add(tableNameAndPath[fileName]);
                        }

                        sameExcelNameInfo[fileName].Add(filePath);
                    }
                    else
                        tableNameAndPath.Add(fileName, filePath);
                }

                if (sameExcelNameInfo.Count > 0)
                {
                    StringBuilder sameExcelNameErrorStringBuilder = new StringBuilder();
                    sameExcelNameErrorStringBuilder.AppendLine("错误：Excel文件夹及其子文件夹中不允许出现同名文件，重名文件如下：");
                    foreach (var item in sameExcelNameInfo)
                    {
                        string fileName = item.Key;
                        List<string> filePath = item.Value;
                        sameExcelNameErrorStringBuilder.AppendFormat("以下路径中存在同名文件（{0}）：\n", fileName);
                        foreach (string oneFilePath in filePath)
                            sameExcelNameErrorStringBuilder.AppendLine(oneFilePath);
                    }

                    MessageBox.Show(sameExcelNameErrorStringBuilder.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 检查如果设置了-exportKeepDirectoryStructure参数，是否也设置了-exportIncludeSubfolder参数
            if (cbExportKeepDirectoryStructure.Checked == true && cbExportIncludeSubfolder.Checked == false)
            {
                MessageBox.Show(string.Format("只有通过设置{0}参数，将要导出的Excel文件夹下的各级子文件夹中的Excel文件也进行导出时，指定{1}参数设置将生成的文件按原Excel文件所在的目录结构进行存储才有意义，请检查是否遗漏设置{0}参数", AppValues.EXPORT_INCLUDE_SUBFOLDER_PARAM_STRING, AppValues.EXPORT_KEEP_DIRECTORY_STRUCTURE_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(string.Format("未指定lang文件所在路径（若无需指定，请填写{0}）", AppValues.NO_LANG_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbLangFilePath.Text = AppValues.NO_LANG_PARAM_STRING;
                return false;
            }
            if (!langFilePath.Equals(AppValues.NO_LANG_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!File.Exists(langFilePath))
                {
                    MessageBox.Show("指定的lang文件所在路径不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // 若设置导出部分Excel文件，检查文件名声明是否正确
            List<string> exportTableNames = new List<string>();
            if (cbPart.Checked == true)
            {
                string partExcelNames = tbPartExcelNames.Text.Trim();
                if (string.IsNullOrEmpty(partExcelNames))
                {
                    MessageBox.Show("勾选了导出部分Excel文件的选项，就必须在文本框中填写要导出的Excel文件名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string[] fileNames = partExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fileName in fileNames)
                    exportTableNames.Add(fileName.Trim());

                // 检查指定导出的Excel文件是否存在
                foreach (string exportExcelFileName in exportTableNames)
                {
                    if (!tableNameAndPath.ContainsKey(exportExcelFileName))
                    {
                        MessageBox.Show(string.Format("指定要导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", string.Concat(exportExcelFileName, ".xlsx")), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            // 若设置忽略导出部分Excel文件，检查文件名声明是否正确
            List<string> exceptTableNames = new List<string>();
            if (cbExcept.Checked == true)
            {
                string exceptExcelNames = tbExceptExcelNames.Text.Trim();
                if (string.IsNullOrEmpty(exceptExcelNames))
                {
                    MessageBox.Show("勾选了忽略导出部分Excel文件的选项，就必须在文本框中填写要忽略的Excel文件名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string[] fileNames = exceptExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fileName in fileNames)
                    exceptTableNames.Add(fileName.Trim());

                // 检查指定忽略导出的Excel文件是否存在
                foreach (string exceptTableName in exceptTableNames)
                {
                    if (!tableNameAndPath.ContainsKey(exceptTableName))
                    {
                        MessageBox.Show(string.Format("指定要忽略导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", string.Concat(exceptTableName, ".xlsx")), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            // 同一张表格不能既设置为-part又设置为-except
            foreach (string exportTableName in exportTableNames)
            {
                if (exceptTableNames.Contains(exportTableName))
                {
                    MessageBox.Show(string.Format("对表格{0}既设置了-part参数，又设置了-except参数", exportTableNames), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // 若设置额外导出部分Excel表格为csv文件，检查声明的各种参数是否正确
            if (cbExportCsv.Checked == true)
            {
                // 检查设置的要额外导出为csv文件的Excel表格名是否正确
                string exportCsvExcelNames = tbExportCsvTableNames.Text.Trim();
                if (string.IsNullOrEmpty(exportCsvExcelNames))
                {
                    MessageBox.Show(string.Format("勾选了额外导出部分Excel表格为csv文件的选项，就必须指定要额外导出为csv文件的Excel文件名，若要全部导出，请指定{0}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!exportCsvExcelNames.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] fileNames = exportCsvExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> exportCsvTableNames = new List<string>();
                    foreach (string fileName in fileNames)
                        exportCsvTableNames.Add(fileName.Trim());

                    // 检查指定要额外导出为csv文件的Excel表格是否存在
                    foreach (string exportCsvExcelFileName in exportCsvTableNames)
                    {
                        if (!tableNameAndPath.ContainsKey(exportCsvExcelFileName))
                        {
                            MessageBox.Show(string.Format("指定要额外导出出csv文件的Excel表格（{0}）不存在，请检查后重试并注意区分大小写", string.Concat(exportCsvExcelFileName, ".xlsx")), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                // 检查设置的导出csv文件的存储路径是否正确
                string exportCsvFilePath = tbExportCsvFilePath.Text.Trim();
                if (string.IsNullOrEmpty(exportCsvFilePath))
                {
                    MessageBox.Show("勾选了额外导出部分Excel表格为csv文件的选项，就必须指定导出csv文件的存储路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!Directory.Exists(exportCsvFilePath))
                {
                    MessageBox.Show("指定的csv文件导出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 检查设置的导出csv文件的扩展名是否正确
                string csvFileExtension = tbCsvFileExtension.Text.Trim();
                if (string.IsNullOrEmpty(csvFileExtension))
                {
                    MessageBox.Show("要额外导出为csv文件，就必须指定csv文件的扩展名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                // 检查设置的导出csv文件的字段分隔符是否正确
                string csvFileSplitString = tbCsvFileSplitString.Text;
                if (string.IsNullOrEmpty(csvFileSplitString))
                {
                    MessageBox.Show("要额外导出为csv文件，就必须指定csv文件的字段分隔符", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 若设置额外导出部分Excel表格为csv对应C#类文件，检查声明的各种参数是否正确
            if (cbExportCsClass.Checked == true)
            {
                // 检查设置的要额外导出为csv对应C#类文件的Excel表格名是否正确
                string exportCsClassExcelNames = tbExportCsClassTableNames.Text.Trim();
                if (string.IsNullOrEmpty(exportCsClassExcelNames))
                {
                    MessageBox.Show(string.Format("勾选了额外导出部分Excel表格为csv对应C#类文件的选项，就必须指定要额外导出为csv对应C#类文件的Excel文件名，若要全部导出，请指定{0}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!exportCsClassExcelNames.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] fileNames = exportCsClassExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> exportCsClassTableNames = new List<string>();
                    foreach (string fileName in fileNames)
                        exportCsClassTableNames.Add(fileName.Trim());

                    // 检查指定要额外导出为csv对应C#类文件的Excel表格是否存在
                    foreach (string exportCsClassExcelFileName in exportCsClassTableNames)
                    {
                        if (!tableNameAndPath.ContainsKey(exportCsClassExcelFileName))
                        {
                            MessageBox.Show(string.Format("指定要额外导出csv对应C#类文件的Excel表格（{0}）不存在，请检查后重试并注意区分大小写", string.Concat(exportCsClassExcelFileName, ".xlsx")), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                // 检查设置的导出csv对应C#类文件的存储路径是否正确
                string exportCsClassFilePath = tbExportCsClassFilePath.Text.Trim();
                if (string.IsNullOrEmpty(exportCsClassFilePath))
                {
                    MessageBox.Show("勾选了额外导出部分Excel表格为csv对应C#类文件的选项，就必须指定导出csv对应C#类文件的存储路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!Directory.Exists(exportCsClassFilePath))
                {
                    MessageBox.Show("指定的csv对应C#类文件导出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 若设置额外导出部分Excel表格为csv对应Java类文件，检查声明的各种参数是否正确
            if (cbExportJavaClass.Checked == true)
            {
                // 检查设置的要额外导出为csv对应Java类文件的Excel表格名是否正确
                string exportJavaClassExcelNames = tbExportJavaClassTableNames.Text.Trim();
                if (string.IsNullOrEmpty(exportJavaClassExcelNames))
                {
                    MessageBox.Show(string.Format("勾选了额外导出部分Excel表格为csv对应Java类文件的选项，就必须指定要额外导出为csv对应Java类文件的Excel文件名，若要全部导出，请指定{0}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!exportJavaClassExcelNames.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] fileNames = exportJavaClassExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> exportJavaClassTableNames = new List<string>();
                    foreach (string fileName in fileNames)
                        exportJavaClassTableNames.Add(fileName.Trim());

                    // 检查指定要额外导出为csv对应C#类文件的Excel表格是否存在
                    foreach (string exportJavaClassExcelFileName in exportJavaClassTableNames)
                    {
                        if (!tableNameAndPath.ContainsKey(exportJavaClassExcelFileName))
                        {
                            MessageBox.Show(string.Format("指定要额外导出csv对应Java类文件的Excel表格（{0}）不存在，请检查后重试并注意区分大小写", string.Concat(exportJavaClassExcelFileName, ".xlsx")), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                // 检查设置的导出csv对应Java类文件的存储路径是否正确
                string exportJavaClassFilePath = tbExportJavaClassFilePath.Text.Trim();
                if (string.IsNullOrEmpty(exportJavaClassFilePath))
                {
                    MessageBox.Show("勾选了额外导出部分Excel表格为csv对应Java类文件的选项，就必须指定导出csv对应Java类文件的存储路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!Directory.Exists(exportJavaClassFilePath))
                {
                    MessageBox.Show("指定的csv对应Java类文件导出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 检查导出csv对应Java类文件的包名是否填写
                string exportJavaClassPackage = tbExportJavaClassPackage.Text.Trim();
                if (string.IsNullOrEmpty(exportJavaClassPackage))
                {
                    MessageBox.Show("勾选了额外导出部分Excel表格为csv对应Java类文件的选项，就必须指定导出csv对应Java类文件的包名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            // 若设置额外导出部分Excel表格为json文件，检查声明的各种参数是否正确
            if (cbExportJson.Checked == true)
            {
                // 检查设置的要额外导出为json文件的Excel表格名是否正确
                string exportJsonExcelNames = tbExportJsonTableNames.Text.Trim();
                if (string.IsNullOrEmpty(exportJsonExcelNames))
                {
                    MessageBox.Show(string.Format("勾选了额外导出部分Excel表格为json文件的选项，就必须指定要额外导出为json文件的Excel文件名，若要全部导出，请指定{0}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!exportJsonExcelNames.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] fileNames = exportJsonExcelNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> exportJsonTableNames = new List<string>();
                    foreach (string fileName in fileNames)
                        exportJsonTableNames.Add(fileName.Trim());

                    // 检查指定要额外导出为json文件的Excel表格是否存在
                    foreach (string exportJsonExcelFileName in exportJsonTableNames)
                    {
                        if (!tableNameAndPath.ContainsKey(exportJsonExcelFileName))
                        {
                            MessageBox.Show(string.Format("指定要额外导出出json文件的Excel表格（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(excelFolderPath, string.Concat(exportJsonExcelFileName, ".xlsx"))), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                // 检查设置的导出json文件的存储路径是否正确
                string exportJsonFilePath = tbExportJsonFilePath.Text.Trim();
                if (string.IsNullOrEmpty(exportJsonFilePath))
                {
                    MessageBox.Show("勾选了额外导出部分Excel表格为json文件的选项，就必须指定导出json文件的存储路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!Directory.Exists(exportJsonFilePath))
                {
                    MessageBox.Show("指定的json文件导出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 检查设置的导出json文件的扩展名是否正确
                string jsonFileExtension = tbJsonFileExtension.Text.Trim();
                if (string.IsNullOrEmpty(jsonFileExtension))
                {
                    MessageBox.Show("要额外导出为json文件，就必须指定json文件的扩展名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private string _GetExecuteParamString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Uri programUri = new Uri(AppValues.PROGRAM_PATH);

            string programPath = tbProgramPath.Text.Trim();
            string excelFolderPath = tbExcelFolderPath.Text.Trim();
            string exportLuaFolderPath = tbExportLuaFolderPath.Text.Trim();
            string clientFolderPath = tbClientFolderPath.Text.Trim();
            string langFilePath = tbLangFilePath.Text.Trim();
            if (cbIsUseRelativePath.Checked == true)
            {
                if (Path.IsPathRooted(programPath))
                    programPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(programPath)).ToString());
                if (Path.IsPathRooted(excelFolderPath))
                    excelFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(excelFolderPath)).ToString());
                if (Path.IsPathRooted(exportLuaFolderPath))
                    exportLuaFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportLuaFolderPath)).ToString());
                if (!clientFolderPath.Equals(AppValues.NO_CLIENT_PATH_STRING, StringComparison.CurrentCultureIgnoreCase) && Path.IsPathRooted(clientFolderPath))
                    clientFolderPath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(clientFolderPath)).ToString());

                if (!langFilePath.Equals(AppValues.NO_LANG_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase) && Path.IsPathRooted(langFilePath))
                    langFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(langFilePath)).ToString());
            }
            stringBuilder.AppendFormat("\"{0}\" ", programPath).AppendFormat("\"{0}\" ", excelFolderPath).AppendFormat("\"{0}\" ", exportLuaFolderPath).AppendFormat("\"{0}\" ", clientFolderPath).AppendFormat("\"{0}\" ", langFilePath);
            if (cbExportIncludeSubfolder.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.EXPORT_INCLUDE_SUBFOLDER_PARAM_STRING);
            if (cbExportKeepDirectoryStructure.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.EXPORT_KEEP_DIRECTORY_STRUCTURE_PARAM_STRING);
            if (cbColumnInfo.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.NEED_COLUMN_INFO_PARAM_STRING);
            if (cbUnchecked.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.UNCHECKED_PARAM_STRING);
            if (cbPrintEmptyStringWhenLangNotMatching.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING);
            if (cbExportMySQL.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.EXPORT_MYSQL_PARAM_STRING);
            if (cbAllowedNullNumber.Checked == true)
                stringBuilder.AppendFormat("\"{0}\" ", AppValues.ALLOWED_NULL_NUMBER_PARAM_STRING);
            if (cbPart.Checked == true)
            {
                string partExcelNames = tbPartExcelNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.PART_EXPORT_PARAM_STRING, partExcelNames);
            }
            if (cbExcept.Checked == true)
            {
                string exceptExcelNames = tbExceptExcelNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXCEPT_EXPORT_PARAM_STRING, exceptExcelNames);
            }
            if (cbExportCsv.Checked == true)
            {
                // 声明要额外导出为csv文件的Excel表格
                string exportCsvTableNames = tbExportCsvTableNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_CSV_PARAM_STRING, exportCsvTableNames);

                // 声明csv文件的导出参数
                List<string> exportCsvParamList = new List<string>();
                // 导出路径
                const string KEY_AND_VALUE_FORMAT = "{0}={1}";
                string exportCsvFilePath = tbExportCsvFilePath.Text.Trim();
                if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportCsvFilePath))
                    exportCsvFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportCsvFilePath)).ToString());

                exportCsvParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXPORT_PATH, exportCsvFilePath));
                // csv文件扩展名
                string csvFileExtension = tbCsvFileExtension.Text.Trim();
                exportCsvParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CSV_PARAM_SUBTYPE_EXTENSION, csvFileExtension));
                // csv文件字段分隔符
                string csvFileSplitString = tbCsvFileSplitString.Text.Replace("|", "\\|");
                exportCsvParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CSV_PARAM_SUBTYPE_SPLIT_STRING, csvFileSplitString));
                // 是否在首行列举字段名称
                bool isExportColumnName = cbIsExportCsvColumnName.Checked;
                exportCsvParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_NAME, isExportColumnName == true ? "true" : "false"));
                // 是否在其后列举字段数据类型
                bool isExportCsvColumnDataType = cbIsExportCsvColumnDataType.Checked;
                exportCsvParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_DATA_TYPE, isExportCsvColumnDataType == true ? "true" : "false"));

                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, Utils.CombineString(exportCsvParamList, "|"));
            }
            // 自动对导出csv对应的C#或Java类文件命名时添加的前后缀
            string csvClassNamePrefix = tbExportCsvClassNamePrefix.Text.Trim();
            string csvClassNamePostfix = tbExportCsvClassNamePostfix.Text.Trim();
            if (!string.IsNullOrEmpty(csvClassNamePrefix) || !string.IsNullOrEmpty(csvClassNamePostfix))
            {
                List<string> autoNameCsvClassParamList = new List<string>();
                const string KEY_AND_VALUE_FORMAT = "{0}={1}";
                if (!string.IsNullOrEmpty(csvClassNamePrefix))
                    autoNameCsvClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_PREFIX, csvClassNamePrefix));
                if (!string.IsNullOrEmpty(csvClassNamePostfix))
                    autoNameCsvClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.AUTO_NAME_CSV_CLASS_PARAM_SUBTYPE_CLASS_NAME_POSTFIX, csvClassNamePostfix));

                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, Utils.CombineString(autoNameCsvClassParamList, "|"));
            }
            if (cbExportCsClass.Checked == true)
            {
                // 声明要额外导出为csv对应C#类文件的Excel表格
                string exportCsClassTableNames = tbExportCsClassTableNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_CS_CLASS_PARAM_STRING, exportCsClassTableNames);

                // 声明csv对应C#类文件的导出参数
                List<string> exportCsClassParamList = new List<string>();
                // 导出路径
                const string KEY_AND_VALUE_FORMAT = "{0}={1}";
                string exportCsClassFilePath = tbExportCsClassFilePath.Text.Trim();
                if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportCsClassFilePath))
                    exportCsClassFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportCsClassFilePath)).ToString());

                exportCsClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_EXPORT_PATH, exportCsClassFilePath));
                // C#类文件命名空间
                string csvClassNamespace = tbExportCsClassNamespace.Text.Trim();
                exportCsClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_NAMESPACE, csvClassNamespace));
                // C#类文件引用类库
                string csvClassUsing = tbExportCsClassUsing.Text.Trim();
                exportCsClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_CS_CLASS_PARAM_SUBTYPE_USING, csvClassUsing));

                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, Utils.CombineString(exportCsClassParamList, "|"));
            }
            if (cbExportJavaClass.Checked == true)
            {
                // 声明要额外导出为csv对应Java类文件的Excel表格
                string exportJavaClassTableNames = tbExportJavaClassTableNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_JAVA_CLASS_PARAM_STRING, exportJavaClassTableNames);

                // 声明csv对应Java类文件的导出参数
                List<string> exportJavaClassParamList = new List<string>();
                // 导出路径
                const string KEY_AND_VALUE_FORMAT = "{0}={1}";
                string exportJavaClassFilePath = tbExportJavaClassFilePath.Text.Trim();
                if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportJavaClassFilePath))
                    exportJavaClassFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportJavaClassFilePath)).ToString());

                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_EXPORT_PATH, exportJavaClassFilePath));
                // Java类文件的包名
                string javaClassPackage = tbExportJavaClassPackage.Text.Trim();
                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_PACKAGE, javaClassPackage));
                // Java类文件引用类库
                string javaClassImport = tbExportJavaClassImport.Text.Trim();
                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IMPORT, javaClassImport));
                // 是否对Java类文件中的时间型使用Date而不是Calendar
                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_USE_DATE, cbExportJavaClassIsUseDate.Checked == true ? "true" : "false"));
                // 是否为Java类文件生成无参构造函数
                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS, cbExportJavaClassIsGenerateConstructorWithoutFields.Checked == true ? "true" : "false"));
                // 是否为Java类文件生成含全部参数的构造函数
                exportJavaClassParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JAVA_CLASS_PARAM_SUBTYPE_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS, cbExportJavaClassIsGenerateConstructorWithAllFields.Checked == true ? "true" : "false"));

                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, Utils.CombineString(exportJavaClassParamList, "|"));
            }
            if (cbExportJson.Checked == true)
            {
                // 声明要额外导出为json文件的Excel表格
                string exportJsonTableNames = tbExportJsonTableNames.Text.Trim();
                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_JSON_PARAM_STRING, exportJsonTableNames);

                // 声明json文件的导出参数
                List<string> exportJsonParamList = new List<string>();
                // 导出路径
                const string KEY_AND_VALUE_FORMAT = "{0}={1}";
                string exportJsonFilePath = tbExportJsonFilePath.Text.Trim();
                if (cbIsUseRelativePath.Checked == true && Path.IsPathRooted(exportJsonFilePath))
                    exportJsonFilePath = Uri.UnescapeDataString(programUri.MakeRelativeUri(new Uri(exportJsonFilePath)).ToString());

                exportJsonParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXPORT_PATH, exportJsonFilePath));
                // json文件扩展名
                string jsonFileExtension = tbJsonFileExtension.Text.Trim();
                exportJsonParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JSON_PARAM_SUBTYPE_EXTENSION, jsonFileExtension));
                // 是否将生成的json字符串整理为带缩进格式的形式
                bool isFormat = cbIsExportJsonWithFormat.Checked;
                exportJsonParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_FORMAT, isFormat == true ? "true" : "false"));
                // 是否生成为各行数据对应的json object包含在一个json array的形式
                bool isExportJsonArrayFormat = cbIsExportJsonArrayFormat.Checked;
                exportJsonParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_EXPORT_JSON_ARRAY_FORMAT, isExportJsonArrayFormat == true ? "true" : "false"));
                // 若生成包含在一个json object的形式，是否使每行字段信息对应的json object中包含主键列对应的键值对
                bool isExportJsonMapIncludeKeyColumnValue = cbIsExportJsonMapIncludeKeyColumnValue.Checked;
                exportJsonParamList.Add(string.Format(KEY_AND_VALUE_FORMAT, AppValues.EXPORT_JSON_PARAM_SUBTYPE_IS_MAP_INCLUDE_KEY_COLUMN_VALUE, isExportJsonMapIncludeKeyColumnValue == true ? "true" : "false"));

                stringBuilder.AppendFormat("\"{0}({1})\" ", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, Utils.CombineString(exportJsonParamList, "|"));
            }

            return stringBuilder.ToString();
        }

        private void _WarnWhenChooseDangerousParam(CheckBox cb)
        {
            if (cb.Checked == true)
                cb.ForeColor = Color.Red;
            else
                cb.ForeColor = Color.Black;
        }

        /// <summary>
        /// 弹出文件选择对话框，选择部分要进行操作的Excel文件
        /// </summary>
        private string _GetChoosePartExcelFile(string originalText, out string errorString)
        {
            string excelFolderPath = tbExcelFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(excelFolderPath))
            {
                errorString = "请先指定Excel文件所在目录";
                return null;
            }
            if (!Directory.Exists(excelFolderPath))
            {
                errorString = "指定Excel文件所在目录不存在，请重新设置";
                return null;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择Excel表格";
            dialog.InitialDirectory = excelFolderPath;
            dialog.Multiselect = true;
            dialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] filePaths = dialog.FileNames;
                // 检查选择的Excel文件是否在设置的Excel所在目录
                string checkFilePath = Path.GetDirectoryName(filePaths[0]);
                if (cbExportIncludeSubfolder.Checked == true)
                {
                    if (!Path.GetFullPath(checkFilePath).StartsWith(Path.GetFullPath(excelFolderPath), StringComparison.CurrentCultureIgnoreCase))
                    {
                        errorString = string.Format("必须在指定的Excel文件所在目录或子目录中选择导出文件\n设置的Excel文件所在根目录为：{0}\n而你选择的Excel所在目录为：{1}", Path.GetFullPath(excelFolderPath), Path.GetFullPath(checkFilePath));
                        return null;
                    }
                }
                else
                {
                    if (!Path.GetFullPath(checkFilePath).Equals(Path.GetFullPath(excelFolderPath), StringComparison.CurrentCultureIgnoreCase))
                    {
                        errorString = string.Format("必须在指定的Excel文件所在目录中选择导出文件\n设置的Excel文件所在目录为：{0}\n而你选择的Excel所在目录为：{1}", Path.GetFullPath(excelFolderPath), Path.GetFullPath(checkFilePath));
                        return null;
                    }
                }

                List<string> fileNames = new List<string>();
                foreach (string filePath in filePaths)
                    fileNames.Add(Path.GetFileNameWithoutExtension(filePath));

                originalText = originalText.Trim();
                if (!string.IsNullOrEmpty(originalText))
                {
                    string[] originalInputExcelFile = originalText.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < originalInputExcelFile.Length; ++i)
                    {
                        string oneOriginalInputExcelFile = originalInputExcelFile[i].Trim();
                        if (!string.IsNullOrEmpty(oneOriginalInputExcelFile) && !fileNames.Contains(oneOriginalInputExcelFile))
                            fileNames.Add(oneOriginalInputExcelFile);
                    }
                }

                errorString = null;
                return Utils.CombineString(fileNames, "|");
            }
            else
            {
                errorString = null;
                return null;
            }
        }
    }
}
