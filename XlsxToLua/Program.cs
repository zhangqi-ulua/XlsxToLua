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
    /// 可附加参数为：-columnInfo（在生成lua文件的最上方用注释形式显示列信息，默认不开启），-unchecked（不对表格进行查错，不推荐使用），-printEmptyStringWhenLangNotMatching（当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串即xx = ""，默认为输出nil）
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
            switch (param)
            {
                case AppValues.UNCHECKED_PARAM_STRING:
                    {
                        AppValues.IsNeedCheck = false;
                        Utils.LogWarning("警告：你选择了不进行表格检查，请务必自己保证表格的正确性");
                        break;
                    }
                case AppValues.NEED_COLUMN_INFO_PARAM_STRING:
                    {
                        AppValues.IsNeedColumnInfo = true;
                        Utils.LogWarning("你选择了在生成的lua文件最上方用注释形式显示列信息");
                        break;
                    }
                case AppValues.LANG_NOT_MATCHING_PRINT_PARAM_STRING:
                    {
                        AppValues.IsPrintEmptyStringWhenLangNotMatching = true;
                        Utils.LogWarning("你选择了当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串");
                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit(string.Format("错误：未知的指令参数{0}", param));
                        break;
                    }
            }
        }
        // 解析本工具所在目录下的config文件
        if (File.Exists(AppValues.CONFIG_FILE_NAME))
        {
            string errorString = null;
            AppValues.ConfigData = TxtConfigReader.ParseTxtConfigFile(AppValues.CONFIG_FILE_NAME, ":", out errorString);
            if (!string.IsNullOrEmpty(errorString))
                Utils.LogErrorAndExit(errorString);
        }
        else
            Utils.LogWarning(string.Format("警告：找不到本工具所在路径下的{0}配置文件，请确定是否真的不需要自定义配置", AppValues.CONFIG_FILE_NAME));

        // 读取给定的Excel所在目录下的所有Excel文件，然后解析成本工具所需的数据结构
        foreach (var filePath in Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"))
        {
            string errorString = null;
            DataSet ds = XlsxReader.ReadXlsxFile(filePath, out errorString);
            if (string.IsNullOrEmpty(errorString))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                TableInfo tableInfo = TableAnalyzeHelper.AnalyzeTable(ds.Tables[AppValues.EXCEL_SHEET_NAME], fileName, out errorString);
                if (errorString != null)
                    Utils.LogErrorAndExit(string.Format("错误：解析{0}失败\n{1}", filePath, errorString));
                else
                {
                    // 如果有表格配置进行解析
                    if (ds.Tables[AppValues.EXCEL_CONFIG_NAME] != null)
                    {
                        tableInfo.TableConfig = TableAnalyzeHelper.GetTableConfig(ds.Tables[AppValues.EXCEL_CONFIG_NAME], out errorString);
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

            foreach (TableInfo tableInfo in AppValues.TableInfo.Values)
            {
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
            foreach (TableInfo tableInfo in AppValues.TableInfo.Values)
            {
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
                                Utils.LogErrorAndExit("导出失败：" + errorString);
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
