using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

// 注意：要在本项目属性的“生成”选项卡中将“目标平台”由默认的“Any CPU”改为“x86”，
// 否则即便安装了AccessDatabaseEngine，在64位系统安装32位Office（Microsoft.ACE.OLEDB.12.0也就是32位的），然后64位的VS默认编译为64位程序仍将导致无法连接Excel，提示本机未注册Microsoft.ACE.OLEDB.12.0提供程序

public class Program
{
    /// <summary>
    /// 传入参数中，第1个必须为Excel表格所在目录，第2个必须为存放导出lua文件的目录，第3个参数为项目Client目录的路径（无需文件存在型检查规则则填-noClient），第4个参数为必须为lang文件路径（没有填-noLang）
    /// 可附加参数有：
    /// 1) -columnInfo（在生成lua文件的最上方用注释形式显示列信息，默认不开启）
    /// 2) -unchecked（不对表格进行查错，不推荐使用）
    /// 3) -printEmptyStringWhenLangNotMatching（当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串即xx = ""，默认为输出nil）
    /// 4) -exportMySQL（将表格数据导出到MySQL数据库中，默认不导出）
    /// 5) -part（后面在英文小括号内声明本次要导出的Excel文件名，用|分隔，未声明的文件将被本工具忽略）
    /// </summary>
    static void Main(string[] args)
    {
        // 检查第1个参数（Excel表格所在目录）是否正确
        if (args.Length < 1)
            Utils.LogErrorAndExit("错误：未输入Excel表格所在目录");
        if (!Directory.Exists(args[0]))
            Utils.LogErrorAndExit(string.Format("错误：输入的Excel表格所在目录不存在，路径为{0}", args[0]));

        AppValues.ExcelFolderPath = Path.GetFullPath(args[0]);
        Utils.Log(string.Format("选择的Excel所在路径：{0}", AppValues.ExcelFolderPath));
        // 检查第2个参数（存放导出lua文件的目录）是否正确
        if (args.Length < 2)
            Utils.LogErrorAndExit("错误：未输入要将生成lua文件存放的路径");
        if (!Directory.Exists(args[1]))
            Utils.LogErrorAndExit(string.Format("错误：输入的lua文件导出路径不存在，路径为{0}", args[1]));

        AppValues.ExportLuaFilePath = Path.GetFullPath(args[1]);
        Utils.Log(string.Format("选择的lua文件导出路径：{0}", AppValues.ExportLuaFilePath));
        // 检查第3个参数（项目Client目录的路径）是否正确
        if (args.Length < 3)
            Utils.LogErrorAndExit("错误：未输入项目Client目录的路径，如果不需要请输入参数-noClient");
        if (AppValues.NO_CLIENT_PATH_STRING.Equals(args[2], StringComparison.CurrentCultureIgnoreCase))
        {
            Utils.LogWarning("警告：你选择了不指定Client文件夹路径，则本工具无法检查表格中填写的图片路径等对应的文件是否存在");
            AppValues.ClientPath = null;
        }
        else if (Directory.Exists(args[2]))
        {
            AppValues.ClientPath = Path.GetFullPath(args[2]);
            Utils.Log(string.Format("Client目录完整路径：{0}", AppValues.ClientPath));
        }
        else
            Utils.LogErrorAndExit(string.Format("错误：请检查输入的Client路径是否正确{0}", args[2]));

        // 检查第4个参数（lang文件路径）是否正确
        if (args.Length < 4)
            Utils.LogErrorAndExit("错误：未输入lang文件路径或未声明不含lang文件（使用-noLang）");
        if (AppValues.NO_LONG_PARAM_STRING.Equals(args[3], StringComparison.CurrentCultureIgnoreCase))
        {
            AppValues.LangFilePath = null;
            Utils.Log("选择的lang文件路径：无");
        }
        else if (File.Exists(args[3]))
        {
            AppValues.LangFilePath = Path.GetFullPath(args[3]);
            Utils.Log(string.Format("选择的lang文件路径：{0}", AppValues.LangFilePath));

            // 解析lang文件
            string errorString = null;
            AppValues.LangData = TxtConfigReader.ParseTxtConfigFile(AppValues.LangFilePath, ":", out errorString);
            if (!string.IsNullOrEmpty(errorString))
                Utils.LogErrorAndExit(errorString);
        }
        else
            Utils.LogErrorAndExit(string.Format("错误：输入的lang文件不存在，路径为{0}", args[3]));

        // 检查其他参数
        for (int i = 4; i < args.Length; ++i)
        {
            string param = args[i];

            if (param.Equals(AppValues.UNCHECKED_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                AppValues.IsNeedCheck = false;
                Utils.LogWarning("警告：你选择了不进行表格检查，请务必自己保证表格的正确性");
            }
            else if (param.Equals(AppValues.NEED_COLUMN_INFO_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                AppValues.IsNeedColumnInfo = true;
                Utils.LogWarning("你选择了在生成的lua文件最上方用注释形式显示列信息");
            }
            else if (param.Equals(AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                AppValues.IsPrintEmptyStringWhenLangNotMatching = true;
                Utils.LogWarning("你选择了当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串");
            }
            else if (param.Equals(AppValues.EXPORT_MYSQL_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                AppValues.IsExportMySQL = true;
                Utils.LogWarning("你选择了导出表格数据到MySQL数据库");
            }
            else if (param.StartsWith(AppValues.PART_EXPORT_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 解析声明的本次要导出的Excel名
                int leftBracketIndex = param.IndexOf('(');
                int rightBracketIndex = param.LastIndexOf(')');
                if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("错误：声明导出部分Excel表格的参数{0}后必须在英文小括号内声明Excel文件名", AppValues.PART_EXPORT_PARAM_STRING));
                else
                {
                    string fileNameString = param.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                    string[] fileNames = fileNameString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fileNames.Length < 1)
                        Utils.LogErrorAndExit(string.Format("错误：声明导出部分Excel表格的参数{0}后必须在英文小括号内声明至少一个Excel文件名", AppValues.PART_EXPORT_PARAM_STRING));

                    foreach (string fileName in fileNames)
                        AppValues.exportTableNames.Add(fileName.Trim());

                    // 检查指定导出的Excel文件是否存在（注意不能直接用File.Exists判断是否存在，因为Windows会忽略声明的Excel文件名与实际文件名的大小写差异）
                    List<string> existExcelFilePaths = new List<string>(Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"));
                    List<string> existExcelFileNames = new List<string>();
                    foreach (string filePath in existExcelFilePaths)
                        existExcelFileNames.Add(Path.GetFileNameWithoutExtension(filePath));

                    foreach (string exportExcelFileName in AppValues.exportTableNames)
                    {
                        if (!existExcelFileNames.Contains(exportExcelFileName))
                            Utils.LogErrorAndExit(string.Format("要求导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExcelFolderPath, string.Concat(exportExcelFileName, ".xlsx"))));
                    }

                    Utils.LogWarning(string.Format("警告：本次将仅检查并导出以下Excel文件：\n{0}\n", Utils.CombineString(AppValues.exportTableNames, ", ")));
                }
            }
            else
                Utils.LogErrorAndExit(string.Format("错误：未知的指令参数{0}", param));
        }

        // 如果未指定导出部分Excel文件，则全部导出
        if (AppValues.exportTableNames.Count == 0)
        {
            foreach (string filePath in Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"))
                AppValues.exportTableNames.Add(Path.GetFileNameWithoutExtension(filePath));
        }

        // 解析本工具所在目录下的config文件
        string configFilePath = Utils.CombinePath(AppValues.PROGRAM_FOLDER_PATH, AppValues.CONFIG_FILE_NAME);
        if (File.Exists(configFilePath))
        {
            string errorString = null;
            AppValues.ConfigData = TxtConfigReader.ParseTxtConfigFile(configFilePath, ":", out errorString);
            if (!string.IsNullOrEmpty(errorString))
                Utils.LogErrorAndExit(errorString);
        }
        else
            Utils.LogWarning(string.Format("警告：找不到本工具所在路径下的{0}配置文件，请确定是否真的不需要自定义配置", AppValues.CONFIG_FILE_NAME));

        // 读取给定的Excel所在目录下的所有Excel文件，然后解析成本工具所需的数据结构
        foreach (string filePath in Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"))
        {
            string errorString = null;
            DataSet ds = XlsxReader.ReadXlsxFile(filePath, out errorString);
            if (string.IsNullOrEmpty(errorString))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                TableInfo tableInfo = TableAnalyzeHelper.AnalyzeTable(ds.Tables[AppValues.EXCEL_DATA_SHEET_NAME], fileName, out errorString);
                if (errorString != null)
                    Utils.LogErrorAndExit(string.Format("错误：解析{0}失败\n{1}", filePath, errorString));
                else
                {
                    // 如果有表格配置进行解析
                    if (ds.Tables[AppValues.EXCEL_CONFIG_SHEET_NAME] != null)
                    {
                        tableInfo.TableConfig = TableAnalyzeHelper.GetTableConfig(ds.Tables[AppValues.EXCEL_CONFIG_SHEET_NAME], out errorString);
                        if (!string.IsNullOrEmpty(errorString))
                            Utils.LogErrorAndExit(string.Format("错误：解析表格{0}的配置失败\n{1}", fileName, errorString));
                    }

                    AppValues.TableInfo.Add(tableInfo.TableName, tableInfo);
                }
            }
            else
                Utils.LogErrorAndExit(string.Format("错误：读取{0}失败\n{1}", filePath, errorString));
        }

        // 进行表格检查
        bool isTableAllRight = true;
        if (AppValues.IsNeedCheck == true)
        {
            Utils.Log("\n下面开始进行表格检查：");

            foreach (string tableName in AppValues.exportTableNames)
            {
                TableInfo tableInfo = AppValues.TableInfo[tableName];
                string errorString = null;
                Utils.Log(string.Format("检查表格\"{0}\"：", tableInfo.TableName));
                TableCheckHelper.CheckTable(tableInfo, out errorString);
                if (errorString != null)
                {
                    Utils.LogError(errorString);
                    isTableAllRight = false;
                }
                else
                    Utils.Log("正确");
            }
        }
        if (isTableAllRight == true)
        {
            Utils.Log("\n表格检查完毕，没有发现错误，开始导出为lua文件\n");
            // 进行表格导出
            foreach (string tableName in AppValues.exportTableNames)
            {
                TableInfo tableInfo = AppValues.TableInfo[tableName];
                string errorString = null;
                Utils.Log(string.Format("导出表格\"{0}\"：", tableInfo.TableName));
                bool isNeedExportOriginalTable = true;
                // 判断是否设置了特殊导出规则
                if (tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT))
                {
                    List<string> inputParams = tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT];
                    if (inputParams.Contains(AppValues.CONFIG_PARAM_NOT_EXPORT_ORIGINAL_TABLE))
                    {
                        isNeedExportOriginalTable = false;
                        if (inputParams.Count == 1)
                            Utils.LogWarning(string.Format("警告：你设置了不对表格\"{0}\"按默认方式进行导出，而又没有指定任何其他自定义导出规则，本工具对此表格不进行任何导出，请确认是否真要如此", tableInfo.TableName));
                        else
                            Utils.Log("你设置了不对此表进行默认规则导出");
                    }
                    // 执行设置的特殊导出规则
                    foreach (string param in inputParams)
                    {
                        if (!AppValues.CONFIG_PARAM_NOT_EXPORT_ORIGINAL_TABLE.Equals(param, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Utils.Log(string.Format("对此表格按\"{0}\"自定义规则进行导出：", param));
                            TableExportToLuaHelper.SpecialExportTableToLua(tableInfo, param, out errorString);
                            if (errorString != null)
                                Utils.LogErrorAndExit(string.Format("导出失败：\n{0}\n", errorString));
                            else
                                Utils.Log("成功");
                        }
                    }
                }
                // 对表格按默认方式导出（除非通过参数设置不执行此操作）
                if (isNeedExportOriginalTable == true)
                {
                    TableExportToLuaHelper.ExportTableToLua(tableInfo, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(errorString);
                    else
                        Utils.Log("按默认方式导出成功");
                }
            }

            Utils.Log("\n导出lua文件完毕\n");

            // 进行数据库导出
            if (AppValues.IsExportMySQL == true)
            {
                Utils.Log("\n导出表格数据到MySQL数据库\n");

                string errorString = null;
                TableExportToMySQLHelper.ConnectToDatabase(out errorString);
                if (!string.IsNullOrEmpty(errorString))
                    Utils.LogErrorAndExit(string.Format("无法连接至MySQL数据库：{0}\n导出至MySQL数据库被迫中止，请修正错误后重试\n", errorString));

                foreach (string tableName in AppValues.exportTableNames)
                {
                    TableInfo tableInfo = AppValues.TableInfo[tableName];
                    TableExportToMySQLHelper.ExportTableToDatabase(tableInfo, out errorString);
                    if (!string.IsNullOrEmpty(errorString))
                        Utils.LogErrorAndExit(string.Format("导出失败：{0}\n导出至MySQL数据库被迫中止，请修正错误后重试\n", errorString));
                }

                Utils.Log("\n导出到数据库完毕\n");
            }
        }
        else
        {
            Utils.LogError("\n表格检查完毕，但存在上面所列错误，必须全部修正后才可以进行表格导出\n");
            // 将错误信息全部输出保存到txt文件中
            Utils.SaveErrorInfoToFile();
        }

        Utils.Log("\n按任意键退出本工具");
        Console.ReadKey();
    }
}
