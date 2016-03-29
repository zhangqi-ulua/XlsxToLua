using System.Collections.Generic;
using System.Text;

/// <summary>
/// 定义本程序中的常量以及供各个类之间读取的常量
/// </summary>
public class AppValues
{
    /// <summary>
    /// Excel文件中数据表的Sheet名，其余Sheet表可自定义内容，不会被本工具导出
    /// </summary>
    public const string EXCEL_SHEET_NAME = "data$";

    /// <summary>
    /// 声明在生成的lua文件开头以注释形式展示列信息的命令参数
    /// </summary>
    public const string NEED_COLUMN_INFO_PARAM_STRING = "-columnInfo";

    /// <summary>
    /// 声明不进行表格检查的命令参数
    /// </summary>
    public const string UNCHECKED_PARAM_STRING = "-unchecked";

    /// <summary>
    /// 声明不指定项目Client目录的命令参数
    /// </summary>
    public const string NO_CLIENT_PATH_STRING = "-noClient";

    /// <summary>
    /// 声明不含有lang文件的命令参数
    /// </summary>
    public const string NO_LONG_PARAM_STRING = "-noLong";

    /// <summary>
    /// 声明当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串的命令参数
    /// </summary>
    public const string LANG_NOT_MATCHING_PRINT_PARAM_STRING = "-printEmptyStringWhenLangNotMatching";

    /// <summary>
    /// 配置文件（配置自定义的检查规则）的路径
    /// </summary>
    public const string CONFIG_FILE_NAME = "config.txt";

    // 每个表格前四行分别声明字段描述、字段变量名、字段数据类型、字段检查规则（行编号从0开始）
    public const int FIELD_DESC_INDEX = 0;
    public const int FIELD_NAME_INDEX = 1;
    public const int FIELD_DATA_TYPE_INDEX = 2;
    public const int FIELD_CHECK_RULE_INDEX = 3;
    public const int FIELD_DATA_START_INDEX = 4;

    /// <summary>
    /// 用户输入的要导出的Excel文件所在目录
    /// </summary>
    public static string ExcelFolderPath = null;

    /// <summary>
    /// 用户输入的要生成的lua文件存放路径
    /// </summary>
    public static string ExportLuaFilePath = null;

    /// <summary>
    /// 用户输入的国际化文件所在路径
    /// </summary>
    public static string LangFilePath = null;

    /// <summary>
    /// 用户输入的Client目录所在路径
    /// </summary>
    public static string ClientPath = null;

    /// <summary>
    /// 用户输入的是否需要检查表格（默认为检查）
    /// </summary>
    public static bool IsNeedCheck = true;

    /// <summary>
    /// 当lang型数据key在lang文件中找不到对应值时，是否在lua文件输出字段值为空字符（默认为输出nil）
    /// </summary>
    public static bool IsPrintEmptyStringWhenLangNotMatching = false;

    /// <summary>
    /// 用户输入的是否需要在生成lua文件的最上方用注释形式显示列信息（默认为不需要）
    /// </summary>
    public static bool IsNeedColumnInfo = false;

    /// <summary>
    /// lang文件转为键值对形式（key：lang文件中的key名， value：对应的在指定语言下的翻译）
    /// </summary>
    public static Dictionary<string, string> LangData = new Dictionary<string, string>();

    /// <summary>
    /// config文件转为键值对形式（key：配置文件中的key名， value：对应的配置规则字符串）
    /// </summary>
    public static Dictionary<string, string> ConfigData = new Dictionary<string, string>();

    /// <summary>
    /// 存储每张Excel表格解析成的本程序所需的数据结构（key：表名）
    /// </summary>
    public static Dictionary<string, TableInfo> TableInfo = new Dictionary<string, TableInfo>();

    /// <summary>
    /// 存储运行时打印的所有信息，在程序运行完毕后输出为txt文件，从而解决如果输出内容过多控制台无法显示全部信息的问题
    /// </summary>
    public static StringBuilder LogContent = new StringBuilder();
}
