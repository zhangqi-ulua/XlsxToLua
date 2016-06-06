using System;
using System.Collections.Generic;
using System.Text;

public class AppValues
{
    public const string PROGRAM_NAME = "XlsxToLua.exe";

    public const string NEED_COLUMN_INFO_PARAM_STRING = "-columnInfo";
    public const string UNCHECKED_PARAM_STRING = "-unchecked";
    public const string NO_CLIENT_PATH_STRING = "-noClient";
    public const string NO_LONG_PARAM_STRING = "-noLong";
    public const string LANG_NOT_MATCHING_PRINT_PARAM_STRING = "-printEmptyStringWhenLangNotMatching";
    public const string EXPORT_MYSQL_PARAM_STRING = "-exportMySQL";
    public const string PART_EXPORT_PARAM_STRING = "-part";
    public const string ALLOWED_NULL_NUMBER_PARAM_STRING = "-allowedNullNumber";

    public const string SAVE_CONFIG_KEY_PROGRAM_PATH = "programPath";
    public const string SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH = "excelFolderPath";
    public const string SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH = "exportLuaFolderPath";
    public const string SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH = "clientFolderPath";
    public const string SAVE_CONFIG_KEY_LANG_FILE_PATH = "langFilePath";

    public const string SAVE_CONFIG_KEY_VALUE_SEPARATOR = ":";

    public static string PROGRAM_FOLDER_PATH = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
}
