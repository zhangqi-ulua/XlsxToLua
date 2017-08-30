using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

// 注意：要在本项目属性的“生成”选项卡中将“目标平台”由默认的“Any CPU”改为“x86”，
// 否则即便安装了AccessDatabaseEngine，在64位系统安装32位Office（Microsoft.ACE.OLEDB.12.0也就是32位的），然后64位的VS默认编译为64位程序仍将导致无法连接Excel，提示本机未注册Microsoft.ACE.OLEDB.12.0提供程序

public class Program
{
    /// <summary>
    /// 传入参数中，第1个必须为Excel表格所在目录，第2个必须为存放导出lua文件的目录，第3个参数为项目Client目录的路径（无需文件存在型检查规则则填-noClient），第4个参数为必须为lang文件路径（没有填-noLang）
    /// 可附加参数有：
    /// 1)  -columnInfo（在生成lua文件的最上方用注释形式显示列信息，默认不开启）
    /// 2)  -unchecked（不对表格进行查错，不推荐使用）
    /// 3)  -printEmptyStringWhenLangNotMatching（当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串即xx = ""，默认为输出nil）
    /// 4)  -exportMySQL（将表格数据导出到MySQL数据库中，默认不导出）
    /// 5)  -part（后面在英文小括号内声明本次要导出的Excel文件名，用|分隔，未声明的文件将被本工具忽略）
    /// 6)  -except（后面在英文小括号内声明本次要忽略导出的Excel文件名，用|分隔，注意不允许对同一张表格既声明-part又声明-except）
    /// 7)  -allowedNullNumber（允许int、long、float型字段下填写空值，默认不允许）
    /// 8)  声明要将指定的Excel表导出为csv文件需要以下2个参数：
    ///     -exportCsv（后面在英文小括号内声明本次要额外导出csv文件的Excel文件名，用|分隔，或者用$all表示全部。注意如果-part参数中未指定本次要导出某个Excel表，即便声明要导出csv文件也不会生效）
    ///     -exportCsvParam（可声明导出csv文件的参数）
    /// 9)  声明要将指定的Excel表导出为csv对应的C#文件需要以下2个参数：
    ///     -exportCsClass（后面在英文小括号内声明本次要额外导出csv对应C#类文件的Excel文件名，用|分隔，或者用$all表示全部。注意如果-part参数中未指定本次要导出某个Excel表，即便声明要导出csv文件也不会生效）
    ///     -exportCsClassParam（可声明导出csv对应C#类文件的参数）
    /// 10) 声明要将指定的Excel表导出为csv对应的Java文件需要以下2个参数：
    ///     -exportJavaClass（后面在英文小括号内声明本次要额外导出csv对应Java类文件的Excel文件名，用|分隔，或者用$all表示全部。注意如果-part参数中未指定本次要导出某个Excel表，即便声明要导出csv文件也不会生效）
    ///     -exportJavaClassParam（可声明导出csv对应Java类文件的参数）
    /// 11) -autoNameCsvClassParam（通过classNamePrefix、classNamePostfix两个子参数设置当导出csv对应的C#或Java类文件时，由本工具自动根据Excel名命名时统一追加前后缀）
    /// 12) 声明要将指定的Excel表导出为json文件需要以下2个参数：
    ///     -exportJson（后面在英文小括号内声明本次要额外导出json文件的Excel文件名，用|分隔，或者用$all表示全部。注意如果-part参数中未指定本次要导出某个Excel表，即便声明要导出json文件也不会生效）
    ///     -exportJsonParam（可声明导出json文件的参数）
    /// </summary>
    static void Main(string[] args)
    {
		try {
			
        // 检查第1个参数（Excel表格所在目录）是否正确
        if (args.Length < 1)
            Utils.LogErrorAndExit("错误：未输入Excel表格所在目录");
        if (!Directory.Exists(args[0]))
            Utils.LogErrorAndExit(string.Format("错误：输入的Excel表格所在目录不存在，路径为{0}", args[0]));

        AppValues.ExcelFolderPath = Path.GetFullPath(args[0]);
        Utils.Log(string.Format("选择的Excel所在路径：{0}", AppValues.ExcelFolderPath));

        // 记录目录中存在的所有Excel文件名（注意不能直接用File.Exists判断某个字符串代表的文件名是否存在，因为Windows会忽略声明的Excel文件名与实际文件名的大小写差异）
        List<string> existExcelFilePaths = new List<string>(Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"));
        List<string> existExcelFileNames = new List<string>();
        foreach (string filePath in existExcelFilePaths)
            existExcelFileNames.Add(Path.GetFileNameWithoutExtension(filePath));

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
        if (AppValues.NO_LANG_PARAM_STRING.Equals(args[3], StringComparison.CurrentCultureIgnoreCase))
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
            else if (param.StartsWith(AppValues.EXCEPT_EXPORT_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 解析声明的本次忽略导出的Excel名
                string errorString = null;
                string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                if (errorString != null)
                    Utils.LogErrorAndExit(string.Format("错误：声明忽略导出部分Excel表格的参数{0}后{1}", AppValues.EXCEPT_EXPORT_PARAM_STRING, errorString));
                else
                {
                    foreach (string fileName in fileNames)
                        AppValues.ExceptExportTableNames.Add(fileName.Trim());

                    // 检查要忽略导出的Excel文件是否存在
                    foreach (string exceptExportExcelFileName in AppValues.ExceptExportTableNames)
                    {
                        if (!existExcelFileNames.Contains(exceptExportExcelFileName))
                            Utils.LogErrorAndExit(string.Format("设置要忽略导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExcelFolderPath, string.Concat(exceptExportExcelFileName, ".xlsx"))));
                    }

                    Utils.LogWarning(string.Format("警告：本次将忽略以下Excel文件：\n{0}\n", Utils.CombineString(AppValues.ExceptExportTableNames, ", ")));
                }
            }
            else if (param.StartsWith(AppValues.PART_EXPORT_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 解析声明的本次要导出的Excel名
                string errorString = null;
                string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                if (errorString != null)
                    Utils.LogErrorAndExit(string.Format("错误：声明导出部分Excel表格的参数{0}后{1}", AppValues.PART_EXPORT_PARAM_STRING, errorString));
                else
                {
                    foreach (string fileName in fileNames)
                        AppValues.ExportTableNames.Add(fileName.Trim());

                    // 检查指定导出的Excel文件是否存在
                    foreach (string exportExcelFileName in AppValues.ExportTableNames)
                    {
                        if (!existExcelFileNames.Contains(exportExcelFileName))
                            Utils.LogErrorAndExit(string.Format("要求导出的Excel文件（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExcelFolderPath, string.Concat(exportExcelFileName, ".xlsx"))));
                    }

                    Utils.LogWarning(string.Format("警告：本次将仅检查并导出以下Excel文件：\n{0}\n", Utils.CombineString(AppValues.ExportTableNames, ", ")));
                }
            }
            // 对额外导出csv对应C#或Java类文件自动命名类名时统一添加的前后缀配置
            else if (param.StartsWith(AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                int leftBracketIndex = param.IndexOf('(');
                int rightBracketIndex = param.LastIndexOf(')');
                if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("错误：声明自动命名导出csv对应的C#或Java类名规则的参数{0}后必须在英文小括号内声明各个具体参数", AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING));
                else
                {
                    string paramString = param.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                    if (string.IsNullOrEmpty(paramString))
                        Utils.LogWarning(string.Format("警告：声明的{0}参数没有在小括号中一并声明下属的子参数{1}和{2}来配置自动命名导出csv对应的C#或Java类名时统一添加的前后缀，若不想设置，可以直接不配置此参数，而不是将参数值留空", AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, AppValues.AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_PREFIX_PARAM_STRING, AppValues.AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_POSTFIX_PARAM_STRING));
                    else
                    {
                        // 通过|分隔各个参数
                        string[] paramStringList = paramString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        // 解析各个具体参数
                        foreach (string oneParamString in paramStringList)
                        {
                            string[] keyAndValue = oneParamString.Split(new char[] { '=' });
                            if (keyAndValue.Length != 2)
                                Utils.LogErrorAndExit(string.Format("声明的{0}参数下属的参数字符串{1}错误，参数名和配置值之间应用=分隔", AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, oneParamString));
                            else
                            {
                                string key = keyAndValue[0].Trim();
                                string value = keyAndValue[1].Trim();
                                if (AppValues.AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_PREFIX_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    AppValues.ExportCsvClassClassNamePrefix = value;
                                    Utils.LogWarning(string.Format("导出csv对应的C#或Java类文件时，若进行类名的自动命名，将统一添加类名前缀为“{0}”", value));
                                }
                                else if (AppValues.AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_POSTFIX_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    AppValues.ExportCsvClassClassNamePostfix = value;
                                    Utils.LogWarning(string.Format("导出csv对应的C#或Java类文件时，若进行类名的自动命名，将统一添加类名后缀为“{0}”", value));
                                }
                                else
                                    Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}非法", AppValues.AUTO_NAME_CSV_CLASS_PARAM_STRING, key));
                            }
                        }
                    }
                }
            }
            // 注意：-exportCsv与-exportCsvParam均以-exportCsv开头，故要先判断-exportCsvParam分支。这里将-exportCsvParam的解析放到-exportCsv的解析之中是为了只有声明了进行csv文件导出时才解析导出参数
            else if (param.StartsWith(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                continue;
            else if (param.StartsWith(AppValues.EXPORT_CSV_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 首先解析并判断配置的csv文件导出参数是否正确
                string exportCsvParamString = null;
                for (int j = 4; j < args.Length; ++j)
                {
                    string tempParam = args[j];
                    if (tempParam.StartsWith(AppValues.EXPORT_CSV_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    {
                        exportCsvParamString = tempParam;
                        break;
                    }
                }
                if (exportCsvParamString != null)
                {
                    int leftBracketIndex = exportCsvParamString.IndexOf('(');
                    int rightBracketIndex = exportCsvParamString.LastIndexOf(')');
                    if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                        Utils.LogErrorAndExit(string.Format("错误：声明导出csv文件的参数{0}后必须在英文小括号内声明各个具体参数", AppValues.EXPORT_CSV_PARAM_PARAM_STRING));
                    else
                    {
                        string paramString = exportCsvParamString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                        // 通过|分隔各个参数，但因为用户设置的csv文件中的字段分隔符本身可能为|，本工具采用\|配置进行转义，故需要自行从头遍历查找真正的参数分隔符
                        // 记录参数分隔符的下标位置
                        List<int> splitParamCharIndex = new List<int>();
                        for (int index = 0; index < paramString.Length; ++index)
                        {
                            char c = paramString[index];
                            if (c == '|' && (index < 1 || (index > 1 && paramString[index - 1] != '\\')))
                                splitParamCharIndex.Add(index);
                        }
                        // 通过识别的参数分隔符，分隔各个参数
                        List<string> paramStringList = new List<string>();
                        int lastSplitParamChatIndex = -1;
                        foreach (int index in splitParamCharIndex)
                        {
                            paramStringList.Add(paramString.Substring(lastSplitParamChatIndex + 1, index - lastSplitParamChatIndex - 1));
                            lastSplitParamChatIndex = index;
                        }
                        // 解析各个具体参数
                        foreach (string oneParamString in paramStringList)
                        {
                            if (string.IsNullOrEmpty(oneParamString))
                                continue;

                            string[] keyAndValue = oneParamString.Split(new char[] { '=' });
                            if (keyAndValue.Length != 2)
                                Utils.LogErrorAndExit(string.Format("声明的{0}参数下属的参数字符串{1}错误，参数名和配置值之间应用=分隔", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, oneParamString));
                            else
                            {
                                string key = keyAndValue[0].Trim();
                                string value = keyAndValue[1];
                                if (AppValues.EXPORT_CSV_PARAM_EXPORT_PATH_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // 检查导出路径是否存在
                                    if (!Directory.Exists(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的csv文件导出路径不存在", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.EXPORT_CSV_PARAM_EXPORT_PATH_PARAM_STRING));
                                    else
                                        AppValues.ExportCsvPath = value;
                                }
                                else if (AppValues.EXPORT_CSV_PARAM_EXTENSION_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (string.IsNullOrEmpty(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的导出csv文件的扩展名不允许为空", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.EXPORT_CSV_PARAM_EXTENSION_PARAM_STRING));
                                    if (value.StartsWith("."))
                                        value = value.Substring(1);

                                    AppValues.ExportCsvExtension = value;
                                }
                                else if (AppValues.EXPORT_CSV_PARAM_SPLIT_STRING_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Replace("\\|", "|");
                                    AppValues.ExportCsvSplitString = value;
                                }
                                else if (AppValues.EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_NAME_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if ("true".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportCsvIsExportColumnName = true;
                                    else if ("false".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportCsvIsExportColumnName = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的值错误，必须为true或false", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_NAME_PARAM_STRING));
                                }
                                else if (AppValues.EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_DATA_TYPE_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if ("true".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportCsvIsExportColumnDataType = true;
                                    else if ("false".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportCsvIsExportColumnDataType = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的值错误，必须为true或false", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_DATA_TYPE_PARAM_STRING));
                                }
                                else
                                    Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}非法", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, key));
                            }
                        }
                        // 要求必须含有exportPath参数
                        if (AppValues.ExportCsvPath == null)
                            Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为csv文件，就必须同时在{0}参数下声明用于配置csv文件导出路径的参数{1}", AppValues.EXPORT_CSV_PARAM_PARAM_STRING, AppValues.EXPORT_CSV_PARAM_EXPORT_PATH_PARAM_STRING));
                    }
                }
                else
                    Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为csv文件，就必须同时声明用于配置csv文件导出参数的{0}", AppValues.EXPORT_CSV_PARAM_PARAM_STRING));

                // 解析配置的要额外导出csv文件的Excel文件名
                string errorString = null;
                // 先判断是否声明对所有文件进行导出
                int paramLeftBracketIndex = param.IndexOf('(');
                int paramRightBracketIndex = param.LastIndexOf(')');
                if (paramLeftBracketIndex == -1 || paramRightBracketIndex == -1 || paramLeftBracketIndex > paramRightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("必须在英文小括号内声明要导出为csv文件的Excel表格名，若要全部导出，请配置为{1}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING));

                string exportCsvFileWithoutBracket = param.Substring(paramLeftBracketIndex + 1, paramRightBracketIndex - paramLeftBracketIndex - 1).Trim();
                if (exportCsvFileWithoutBracket.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    AppValues.ExportCsvTableNames = AppValues.ExportTableNames;
                else
                {
                    string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(string.Format("错误：声明额外导出为csv文件的参数{0}后{1}", AppValues.EXPORT_CSV_PARAM_STRING, errorString));
                    else
                    {
                        // 检查指定导出的Excel文件是否存在
                        foreach (string fileName in fileNames)
                        {
                            if (!existExcelFileNames.Contains(fileName))
                                Utils.LogErrorAndExit(string.Format("要求额外导出为csv文件的Excel表（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExportCsvPath, string.Concat(fileName, ".xlsx"))));
                            else
                                AppValues.ExportCsvTableNames.Add(fileName);
                        }
                    }
                }
            }
            // 注意：-exportCsClass与-exportCsClassParam均以-exportCsClass开头，故要先判断-exportCsClassParam分支。这里将-exportCsClassParam的解析放到-exportCsClass的解析之中是为了只有声明了进行csv对应C#类文件导出时才解析导出参数
            else if (param.StartsWith(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                continue;
            else if (param.StartsWith(AppValues.EXPORT_CS_CLASS_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 首先解析并判断配置的csv对应C#类文件导出参数是否正确
                string exportCsClassParamString = null;
                for (int j = 4; j < args.Length; ++j)
                {
                    string tempParam = args[j];
                    if (tempParam.StartsWith(AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    {
                        exportCsClassParamString = tempParam;
                        break;
                    }
                }
                if (exportCsClassParamString != null)
                {
                    int leftBracketIndex = exportCsClassParamString.IndexOf('(');
                    int rightBracketIndex = exportCsClassParamString.LastIndexOf(')');
                    if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                        Utils.LogErrorAndExit(string.Format("错误：声明导出csv对应C#类文件的参数{0}后必须在英文小括号内声明各个具体参数", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING));
                    else
                    {
                        string paramString = exportCsClassParamString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                        // 通过|分隔各个参数
                        string[] paramStringList = paramString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        // 解析各个具体参数
                        foreach (string oneParamString in paramStringList)
                        {
                            string[] keyAndValue = oneParamString.Split(new char[] { '=' });
                            if (keyAndValue.Length != 2)
                                Utils.LogErrorAndExit(string.Format("声明的{0}参数下属的参数字符串{1}错误，参数名和配置值之间应用=分隔", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, oneParamString));
                            else
                            {
                                string key = keyAndValue[0].Trim();
                                string value = keyAndValue[1];
                                if (AppValues.EXPORT_CS_CLASS_PARAM_EXPORT_PATH_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // 检查导出路径是否存在
                                    if (!Directory.Exists(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的导出csv对应C#类文件导出路径不存在", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_CS_CLASS_PARAM_EXPORT_PATH_PARAM_STRING));
                                    else
                                        AppValues.ExportCsClassPath = value;
                                }
                                else if (AppValues.EXPORT_CS_CLASS_PARAM_NAMESPACE_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (string.IsNullOrEmpty(value))
                                        Utils.LogWarning(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应C#类文件中的命名空间为空，若不想设置命名空间，可以直接不配置此参数，而不是将参数值留空", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_CS_CLASS_PARAM_NAMESPACE_PARAM_STRING));
                                    else
                                        AppValues.ExportCsClassNamespace = value;
                                }
                                else if (AppValues.EXPORT_CS_CLASS_PARAM_USING_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (string.IsNullOrEmpty(value))
                                        Utils.LogWarning(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应C#类文件中的引用类库为空，若不想设置引用类库，可以直接不配置此参数，而不是将参数值留空", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_CS_CLASS_PARAM_USING_PARAM_STRING));
                                    else
                                    {
                                        // 通过英文逗号分隔各个引用类库
                                        string[] usingList = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        List<string> exportCsClassUsing = new List<string>();
                                        for (int index = 0; index < usingList.Length; ++index)
                                        {
                                            string usingString = usingList[index].Trim();
                                            if (!string.IsNullOrEmpty(usingString) && !exportCsClassUsing.Contains(usingString))
                                                exportCsClassUsing.Add(usingString);

                                            if (exportCsClassUsing.Count > 0)
                                                AppValues.ExportCsClassUsing = exportCsClassUsing;
                                            else
                                                Utils.LogWarning(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应C#类文件中的引用类库均为空，若不想设置引用类库，可以直接不配置此参数，而不是将参数值留空", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_CS_CLASS_PARAM_USING_PARAM_STRING));
                                        }
                                    }
                                }
                                else
                                    Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}非法", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, key));
                            }
                        }
                        // 要求必须含有exportPath参数
                        if (AppValues.ExportCsClassPath == null)
                            Utils.LogErrorAndExit(string.Format("错误：声明要额外导出csv对应C#类文件，就必须同时在{0}参数下声明用于配置导出路径的参数{1}", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_CS_CLASS_PARAM_EXPORT_PATH_PARAM_STRING));
                    }
                }
                else
                    Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为csv对应C#类文件，就必须同时声明用于配置C#类文件导出参数的{0}", AppValues.EXPORT_CS_CLASS_PARAM_PARAM_STRING));

                // 解析配置的要额外导出csv对应C#类文件的Excel文件名
                string errorString = null;
                // 先判断是否声明对所有文件进行导出
                int paramLeftBracketIndex = param.IndexOf('(');
                int paramRightBracketIndex = param.LastIndexOf(')');
                if (paramLeftBracketIndex == -1 || paramRightBracketIndex == -1 || paramLeftBracketIndex > paramRightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("必须在英文小括号内声明要导出为csv对应C#类文件的Excel表格名，若要全部导出，请配置为{1}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING));

                string exportCsClassFileWithoutBracket = param.Substring(paramLeftBracketIndex + 1, paramRightBracketIndex - paramLeftBracketIndex - 1).Trim();
                if (exportCsClassFileWithoutBracket.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    AppValues.ExportCsClassTableNames = AppValues.ExportTableNames;
                else
                {
                    string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(string.Format("错误：声明额外导出为csv对应C#类文件的参数{0}后{1}", AppValues.EXPORT_CS_CLASS_PARAM_STRING, errorString));
                    else
                    {
                        // 检查指定导出的Excel文件是否存在
                        foreach (string fileName in fileNames)
                        {
                            if (!existExcelFileNames.Contains(fileName))
                                Utils.LogErrorAndExit(string.Format("要求额外导出为csv对应C#类文件的Excel表（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExportCsClassPath, string.Concat(fileName, ".xlsx"))));
                            else
                                AppValues.ExportCsClassTableNames.Add(fileName);
                        }
                    }
                }
            }
            // 注意：-exportJavaClass与-exportJavaClassParam均以-exportJavaClass开头，故要先判断-exportJavaClassParam分支。这里将-exportJavaClassParam的解析放到-exportJavaClass的解析之中是为了只有声明了进行csv对应Java类文件导出时才解析导出参数
            else if (param.StartsWith(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                continue;
            else if (param.StartsWith(AppValues.EXPORT_JAVA_CLASS_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 首先解析并判断配置的csv对应Java类文件导出参数是否正确
                string exportJavaClassParamString = null;
                for (int j = 4; j < args.Length; ++j)
                {
                    string tempParam = args[j];
                    if (tempParam.StartsWith(AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    {
                        exportJavaClassParamString = tempParam;
                        break;
                    }
                }
                if (exportJavaClassParamString != null)
                {
                    int leftBracketIndex = exportJavaClassParamString.IndexOf('(');
                    int rightBracketIndex = exportJavaClassParamString.LastIndexOf(')');
                    if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                        Utils.LogErrorAndExit(string.Format("错误：声明导出csv对应Java类文件的参数{0}后必须在英文小括号内声明各个具体参数", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING));
                    else
                    {
                        string paramString = exportJavaClassParamString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                        // 通过|分隔各个参数
                        string[] paramStringList = paramString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        // 解析各个具体参数
                        foreach (string oneParamString in paramStringList)
                        {
                            string[] keyAndValue = oneParamString.Split(new char[] { '=' });
                            if (keyAndValue.Length != 2)
                                Utils.LogErrorAndExit(string.Format("声明的{0}参数下属的参数字符串{1}错误，参数名和配置值之间应用=分隔", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, oneParamString));
                            else
                            {
                                string key = keyAndValue[0].Trim();
                                string value = keyAndValue[1];
                                if (AppValues.EXPORT_JAVA_CLASS_PARAM_EXPORT_PATH_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // 检查导出路径是否存在
                                    if (!Directory.Exists(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件导出路径不存在", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_EXPORT_PATH_PARAM_STRING));
                                    else
                                        AppValues.ExportJavaClassPath = value;
                                }
                                else if (AppValues.EXPORT_JAVA_CLASS_PARAM_PACKAGE_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (!string.IsNullOrEmpty(value))
                                        AppValues.ExportJavaClassPackage = value;
                                }
                                else if (AppValues.EXPORT_JAVA_CLASS_PARAM_IMPORT_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (string.IsNullOrEmpty(value))
                                        Utils.LogWarning(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件中的引用类库为空，若不想设置引用类库，可以直接不配置此参数，而不是将参数值留空", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_IMPORT_PARAM_STRING));
                                    else
                                    {
                                        // 通过英文逗号分隔各个引用类库
                                        string[] importList = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        List<string> exportJavaClassPackage = new List<string>();
                                        for (int index = 0; index < importList.Length; ++index)
                                        {
                                            string importString = importList[index].Trim();
                                            if (!string.IsNullOrEmpty(importString) && !exportJavaClassPackage.Contains(importString))
                                                exportJavaClassPackage.Add(importString);

                                            if (exportJavaClassPackage.Count > 0)
                                                AppValues.ExportJavaClassImport = exportJavaClassPackage;
                                            else
                                                Utils.LogWarning(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件中的引用类库均为空，若不想设置引用类库，可以直接不配置此参数，而不是将参数值留空", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_PACKAGE_PARAM_STRING));
                                        }
                                    }
                                }
                                else if (AppValues.EXPORT_JAVA_CLASS_PARAM_IS_USE_DATE_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassIsUseDate = true;
                                    else if (value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassIsUseDate = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件中的时间型是否转为Date而不是Calendar型的选项值配置错误，必须配置为true或false", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_IS_USE_DATE_PARAM_STRING));
                                }
                                else if (AppValues.EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassisGenerateConstructorWithoutFields = true;
                                    else if (value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassisGenerateConstructorWithoutFields = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件是否生成无参构造函数的选项值配置错误，必须配置为true或false", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS_PARAM_STRING));
                                }
                                else if (AppValues.EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassIsGenerateConstructorWithAllFields = true;
                                    else if (value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJavaClassIsGenerateConstructorWithAllFields = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("警告：声明的{0}参数下属的参数{1}所配置的导出csv对应Java类文件是否生成含所有参数的构造函数的选项值配置错误，必须配置为true或false", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS_PARAM_STRING));
                                }
                                else
                                    Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}非法", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, key));
                            }
                        }
                        // 要求必须含有exportPath参数
                        if (AppValues.ExportJavaClassPath == null)
                            Utils.LogErrorAndExit(string.Format("错误：声明要额外导出csv对应Java类文件，就必须同时在{0}参数下声明用于配置导出路径的参数{1}", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_EXPORT_PATH_PARAM_STRING));
                        // 要求必须含有package参数
                        if (AppValues.ExportJavaClassPackage == null)
                            Utils.LogErrorAndExit(string.Format("错误：声明要额外导出csv对应Java类文件，就必须同时在{0}参数下声明用于配置包名的参数{1}", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING, AppValues.EXPORT_JAVA_CLASS_PARAM_PACKAGE_PARAM_STRING));
                    }
                }
                else
                    Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为csv对应Java类文件，就必须同时声明用于配置Java类文件导出参数的{0}", AppValues.EXPORT_JAVA_CLASS_PARAM_PARAM_STRING));

                // 解析配置的要额外导出csv对应Java类文件的Excel文件名
                string errorString = null;
                // 先判断是否声明对所有文件进行导出
                int paramLeftBracketIndex = param.IndexOf('(');
                int paramRightBracketIndex = param.LastIndexOf(')');
                if (paramLeftBracketIndex == -1 || paramRightBracketIndex == -1 || paramLeftBracketIndex > paramRightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("必须在英文小括号内声明要导出为csv对应Java类文件的Excel表格名，若要全部导出，请配置为{1}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING));

                string exportJavaClassFileWithoutBracket = param.Substring(paramLeftBracketIndex + 1, paramRightBracketIndex - paramLeftBracketIndex - 1).Trim();
                if (exportJavaClassFileWithoutBracket.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    AppValues.ExportJavaClassTableNames = AppValues.ExportTableNames;
                else
                {
                    string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(string.Format("错误：声明额外导出为csv对应Java类文件的参数{0}后{1}", AppValues.EXPORT_JAVA_CLASS_PARAM_STRING, errorString));
                    else
                    {
                        // 检查指定导出的Excel文件是否存在
                        foreach (string fileName in fileNames)
                        {
                            if (!existExcelFileNames.Contains(fileName))
                                Utils.LogErrorAndExit(string.Format("要求额外导出为csv对应Java类文件的Excel表（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExportJavaClassPath, string.Concat(fileName, ".xlsx"))));
                            else
                                AppValues.ExportJavaClassTableNames.Add(fileName);
                        }
                    }
                }
            }
            // 注意：-exportJson与-exportJsonParam均以-exportJson开头，故要先判断-exportJsonParam分支。这里将-exportJsonParam的解析放到-exportJson的解析之中是为了只有声明了进行json文件导出时才解析导出参数
            else if (param.StartsWith(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                continue;
            else if (param.StartsWith(AppValues.EXPORT_JSON_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                // 首先解析并判断配置的json文件导出参数是否正确
                string exportJsonParamString = null;
                for (int j = 4; j < args.Length; ++j)
                {
                    string tempParam = args[j];
                    if (tempParam.StartsWith(AppValues.EXPORT_JSON_PARAM_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    {
                        exportJsonParamString = tempParam;
                        break;
                    }
                }
                if (exportJsonParamString != null)
                {
                    int leftBracketIndex = exportJsonParamString.IndexOf('(');
                    int rightBracketIndex = exportJsonParamString.LastIndexOf(')');
                    if (leftBracketIndex == -1 || rightBracketIndex == -1 || leftBracketIndex > rightBracketIndex)
                        Utils.LogErrorAndExit(string.Format("错误：声明导出json文件的参数{0}后必须在英文小括号内声明各个具体参数", AppValues.EXPORT_JSON_PARAM_PARAM_STRING));
                    else
                    {
                        string paramString = exportJsonParamString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                        // 通过|分隔各个参数
                        string[] paramStringList = paramString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        // 解析各个具体参数
                        foreach (string oneParamString in paramStringList)
                        {
                            string[] keyAndValue = oneParamString.Split(new char[] { '=' });
                            if (keyAndValue.Length != 2)
                                Utils.LogErrorAndExit(string.Format("声明的{0}参数下属的参数字符串{1}错误，参数名和配置值之间应用=分隔", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, oneParamString));
                            else
                            {
                                string key = keyAndValue[0].Trim();
                                string value = keyAndValue[1];
                                if (AppValues.EXPORT_JSON_PARAM_EXPORT_PATH_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // 检查导出路径是否存在
                                    if (!Directory.Exists(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的json文件导出路径不存在", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_EXPORT_PATH_PARAM_STRING));
                                    else
                                        AppValues.ExportJsonPath = value;
                                }
                                else if (AppValues.EXPORT_JSON_PARAM_EXTENSION_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if (string.IsNullOrEmpty(value))
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的导出json文件的扩展名不允许为空", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_EXTENSION_PARAM_STRING));
                                    if (value.StartsWith("."))
                                        value = value.Substring(1);

                                    AppValues.ExportJsonExtension = value;
                                }
                                else if (AppValues.EXPORT_JSON_PARAM_IS_FORMAT_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if ("true".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsFormat = true;
                                    else if ("false".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsFormat = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的值错误，必须为true或false", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_IS_FORMAT_PARAM_STRING));
                                }
                                else if (AppValues.EXPORT_JSON_PARAM_IS_EXPORT_JSON_ARRAY_FORMAT_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if ("true".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsExportJsonArrayFormat = true;
                                    else if ("false".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsExportJsonArrayFormat = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的值错误，必须为true或false", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_IS_EXPORT_JSON_ARRAY_FORMAT_PARAM_STRING));
                                }
                                else if (AppValues.EXPORT_JSON_PARAM_IS_MAP_INCLUDE_KEY_COLUMN_VALUE_PARAM_STRING.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    value = value.Trim();
                                    if ("true".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsExportJsonMapIncludeKeyColumnValue = true;
                                    else if ("false".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                        AppValues.ExportJsonIsExportJsonMapIncludeKeyColumnValue = false;
                                    else
                                        Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}所配置的值错误，必须为true或false", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_IS_MAP_INCLUDE_KEY_COLUMN_VALUE_PARAM_STRING));
                                }
                                else
                                    Utils.LogErrorAndExit(string.Format("错误：声明的{0}参数下属的参数{1}非法", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, key));
                            }
                        }
                        // 要求必须含有exportPath参数
                        if (AppValues.ExportJsonPath == null)
                            Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为json文件，就必须同时在{0}参数下声明用于配置json文件导出路径的参数{1}", AppValues.EXPORT_JSON_PARAM_PARAM_STRING, AppValues.EXPORT_JSON_PARAM_EXPORT_PATH_PARAM_STRING));
                    }
                }
                else
                    Utils.LogErrorAndExit(string.Format("错误：声明要额外导出指定Excel文件为json文件，就必须同时声明用于配置json文件导出参数的{0}", AppValues.EXPORT_JSON_PARAM_PARAM_STRING));

                // 解析配置的要额外导出json文件的Excel文件名
                string errorString = null;
                // 先判断是否声明对所有文件进行导出
                int paramLeftBracketIndex = param.IndexOf('(');
                int paramRightBracketIndex = param.LastIndexOf(')');
                if (paramLeftBracketIndex == -1 || paramRightBracketIndex == -1 || paramLeftBracketIndex > paramRightBracketIndex)
                    Utils.LogErrorAndExit(string.Format("必须在英文小括号内声明要导出为json文件的Excel表格名，若要全部导出，请配置为{1}参数", AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING));

                string exportJsonFileWithoutBracket = param.Substring(paramLeftBracketIndex + 1, paramRightBracketIndex - paramLeftBracketIndex - 1).Trim();
                if (exportJsonFileWithoutBracket.Equals(AppValues.EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
                    AppValues.ExportJsonTableNames = AppValues.ExportTableNames;
                else
                {
                    string[] fileNames = Utils.GetExcelFileNames(param, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(string.Format("错误：声明额外导出为json文件的参数{0}后{1}", AppValues.EXPORT_JSON_PARAM_STRING, errorString));
                    else
                    {
                        // 检查指定导出的Excel文件是否存在
                        foreach (string fileName in fileNames)
                        {
                            if (!existExcelFileNames.Contains(fileName))
                                Utils.LogErrorAndExit(string.Format("要求额外导出为json文件的Excel表（{0}）不存在，请检查后重试并注意区分大小写", Utils.CombinePath(AppValues.ExportJsonPath, string.Concat(fileName, ".xlsx"))));
                            else
                                AppValues.ExportJsonTableNames.Add(fileName);
                        }
                    }
                }
            }
            else if (param.StartsWith(AppValues.ALLOWED_NULL_NUMBER_PARAM_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                AppValues.IsAllowedNullNumber = true;
                Utils.LogWarning("警告：你选择了允许int、long、float字段中存在空值，建议为逻辑上不允许为空的数值型字段声明使用notEmpty检查规则");
            }
            else
                Utils.LogErrorAndExit(string.Format("错误：未知的指令参数{0}", param));
        }

        // 如果设置了部分导出，则检查是否对同一个表格既声明了-part又声明了-except
        if (AppValues.ExportTableNames.Count > 0)
        {
            List<string> errorConfigTableNames = new List<string>();
            foreach (string tableName in AppValues.ExportTableNames)
            {
                if (AppValues.ExceptExportTableNames.Contains(tableName))
                    errorConfigTableNames.Add(tableName);
            }

            if (errorConfigTableNames.Count > 0)
                Utils.LogErrorAndExit(string.Format("错误：以下表格既声明要进行导出，又声明要忽略导出：{0}，请修正配置后重试", Utils.CombineString(errorConfigTableNames, ",")));
        }

        // 如果未指定导出部分Excel文件，则全部导出，但要排除设置了进行忽略的
        if (AppValues.ExportTableNames.Count == 0)
        {
            foreach (string filePath in Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith(AppValues.EXCEL_TEMP_FILE_FILE_NAME_START_STRING))
                    Utils.LogWarning(string.Format("目录中的{0}文件为Excel自动生成的临时文件，将被忽略处理", filePath));
                else if (!AppValues.ExceptExportTableNames.Contains(fileName))
                    AppValues.ExportTableNames.Add(Path.GetFileNameWithoutExtension(filePath));
            }
        }

        // 如果声明要额外导出为csv文件的Excel表本身在本次被忽略，需要进行警告
        List<string> warnExportCsvTableNames = new List<string>();
        foreach (string exportCsvTableName in AppValues.ExportCsvTableNames)
        {
            if (!AppValues.ExportTableNames.Contains(exportCsvTableName))
                warnExportCsvTableNames.Add(exportCsvTableName);
        }
        if (warnExportCsvTableNames.Count > 0)
        {
            Utils.LogWarning(string.Format("警告：以下Excel表声明为要额外导出为csv文件，但在{0}参数中未声明本次要对其进行导出，本工具将不对这些表格执行导出csv文件的操作\n{1}", AppValues.PART_EXPORT_PARAM_STRING, Utils.CombineString(warnExportCsvTableNames, ", ")));
            foreach (string tableName in warnExportCsvTableNames)
                AppValues.ExportCsvTableNames.Remove(tableName);
        }
        if (AppValues.ExportCsvTableNames.Count > 0)
            Utils.Log(string.Format("本次将以下Excel表额外导出为csv文件：\n{0}\n", Utils.CombineString(AppValues.ExportCsvTableNames, ", ")));

        // 如果声明要额外导出为json文件的Excel表本身在本次被忽略，需要进行警告
        List<string> warnExportJsonTableNames = new List<string>();
        foreach (string exportJsonTableName in AppValues.ExportJsonTableNames)
        {
            if (!AppValues.ExportTableNames.Contains(exportJsonTableName))
                warnExportJsonTableNames.Add(exportJsonTableName);
        }
        if (warnExportJsonTableNames.Count > 0)
        {
            Utils.LogWarning(string.Format("警告：以下Excel表声明为要额外导出为json文件，但在{0}参数中未声明本次要对其进行导出，本工具将不对这些表格执行导出json文件的操作\n{1}", AppValues.PART_EXPORT_PARAM_STRING, Utils.CombineString(warnExportJsonTableNames, ", ")));
            foreach (string tableName in warnExportJsonTableNames)
                AppValues.ExportJsonTableNames.Remove(tableName);
        }
        if (AppValues.ExportJsonTableNames.Count > 0)
            Utils.Log(string.Format("本次将以下Excel表额外导出为json文件：\n{0}\n", Utils.CombineString(AppValues.ExportJsonTableNames, ", ")));

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

        // 读取部分配置项并进行检查
        const string ERROR_STRING_FORMAT = "配置项\"{0}\"所设置的值\"{1}\"非法：{2}\n";
        StringBuilder errorStringBuilder = new StringBuilder();
        string tempErrorString = null;
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_DATE_INPUT_FORMAT))
        {
            AppValues.DefaultDateInputFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_DATE_INPUT_FORMAT].Trim();
            if (TableCheckHelper.CheckDateInputDefine(AppValues.DefaultDateInputFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_DATE_INPUT_FORMAT, AppValues.DefaultDateInputFormat, tempErrorString);
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_LUA_FORMAT))
        {
            AppValues.DefaultDateToLuaFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_LUA_FORMAT].Trim();
            if (TableCheckHelper.CheckDateToLuaDefine(AppValues.DefaultDateToLuaFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_LUA_FORMAT, AppValues.DefaultDateToLuaFormat, tempErrorString);
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_DATABASE_FORMAT))
        {
            AppValues.DefaultDateToDatabaseFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_DATABASE_FORMAT].Trim();
            if (TableCheckHelper.CheckDateToDatabaseDefine(AppValues.DefaultDateToDatabaseFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_DATE_TO_DATABASE_FORMAT, AppValues.DefaultDateToDatabaseFormat, tempErrorString);
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_TIME_INPUT_FORMAT))
        {
            AppValues.DefaultTimeInputFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_TIME_INPUT_FORMAT].Trim();
            if (TableCheckHelper.CheckTimeDefine(AppValues.DefaultTimeInputFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_TIME_INPUT_FORMAT, AppValues.DefaultTimeInputFormat, tempErrorString);
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_LUA_FORMAT))
        {
            AppValues.DefaultTimeToLuaFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_LUA_FORMAT].Trim();
            if (TableCheckHelper.CheckTimeDefine(AppValues.DefaultTimeToLuaFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_LUA_FORMAT, AppValues.DefaultTimeToLuaFormat, tempErrorString);
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_DATABASE_FORMAT))
        {
            AppValues.DefaultTimeToDatabaseFormat = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_DATABASE_FORMAT].Trim();
            if (TableCheckHelper.CheckTimeDefine(AppValues.DefaultTimeToDatabaseFormat, out tempErrorString) == false)
                errorStringBuilder.AppendFormat(ERROR_STRING_FORMAT, AppValues.APP_CONFIG_KEY_DEFAULT_TIME_TO_DATABASE_FORMAT, AppValues.DefaultTimeToDatabaseFormat, tempErrorString);
        }

        string errorConfigString = errorStringBuilder.ToString();
        if (!string.IsNullOrEmpty(errorConfigString))
        {
            errorConfigString = string.Concat("配置文件中存在以下错误，请修正后重试\n", errorConfigString);
            Utils.LogErrorAndExit(errorConfigString);
        }

        // 读取给定的Excel所在目录下的所有Excel文件，然后解析成本工具所需的数据结构
        Utils.Log("开始解析Excel所在目录下的所有Excel文件：");
        Stopwatch stopwatch = new Stopwatch();
        foreach (string filePath in Directory.GetFiles(AppValues.ExcelFolderPath, "*.xlsx"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.StartsWith(AppValues.EXCEL_TEMP_FILE_FILE_NAME_START_STRING))
                continue;

            Utils.Log(string.Format("解析表格\"{0}\"：", fileName), ConsoleColor.Green);
            stopwatch.Reset();
            stopwatch.Start();

            string errorString = null;
            DataSet ds = XlsxReader.ReadXlsxFile(filePath, out errorString);
            stopwatch.Stop();
            Utils.Log(string.Format("成功，耗时：{0}毫秒", stopwatch.ElapsedMilliseconds));
            if (string.IsNullOrEmpty(errorString))
            {
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

            foreach (string tableName in AppValues.ExportTableNames)
            {
                TableInfo tableInfo = AppValues.TableInfo[tableName];
                string errorString = null;
                Utils.Log(string.Format("检查表格\"{0}\"：", tableInfo.TableName), ConsoleColor.Green);
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
            foreach (string tableName in AppValues.ExportTableNames)
            {
                TableInfo tableInfo = AppValues.TableInfo[tableName];
                string errorString = null;
                Utils.Log(string.Format("导出表格\"{0}\"：", tableInfo.TableName), ConsoleColor.Green);
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
                // 判断是否要额外导出为csv文件
                if (AppValues.ExportCsvTableNames.Contains(tableName))
                {
                    TableExportToCsvHelper.ExportTableToCsv(tableInfo, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(errorString);
                    else
                        Utils.Log("额外导出为csv文件成功");
                }
                // 判断是否要额外导出为csv对应C#类文件
                if (AppValues.ExportCsClassTableNames.Contains(tableName))
                {
                    TableExportToCsClassHelper.ExportTableToCsClass(tableInfo, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(errorString);
                    else
                        Utils.Log("额外导出为csv对应C#类文件成功");
                }
                // 判断是否要额外导出为csv对应Java类文件
                if (AppValues.ExportJavaClassTableNames.Contains(tableName))
                {
                    TableExportToJavaClassHelper.ExportTableToJavaClass(tableInfo, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(errorString);
                    else
                        Utils.Log("额外导出为csv对应Java类文件成功");
                }
                // 判断是否要额外导出为json文件
                if (AppValues.ExportJsonTableNames.Contains(tableName))
                {
                    TableExportToJsonHelper.ExportTableToJson(tableInfo, out errorString);
                    if (errorString != null)
                        Utils.LogErrorAndExit(errorString);
                    else
                        Utils.Log("额外导出为json文件成功");
                }
            }

            Utils.Log("\n导出lua文件完毕\n");

            // 进行数据库导出
            if (AppValues.IsExportMySQL == true)
            {
				Utils.NotImplemntedOnOSX ("数据库导出");
                Utils.Log("\n导出表格数据到MySQL数据库\n");

                string errorString = null;
                TableExportToMySQLHelper.ConnectToDatabase(out errorString);
                if (!string.IsNullOrEmpty(errorString))
                    Utils.LogErrorAndExit(string.Format("无法连接至MySQL数据库：{0}\n导出至MySQL数据库被迫中止，请修正错误后重试\n", errorString));

                foreach (string tableName in AppValues.ExportTableNames)
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
		
		} catch (Exception e) {
			Utils.LogErrorAndExit (e.Message);
		}

        Utils.Log("\n按任意键退出本工具");
        Console.ReadKey();
    }
}
