using System;
using System.Collections.Generic;
using System.Text;

public class AppValues
{
    public const string PROGRAM_NAME = "XlsxToLua.exe";

    public const string NEED_COLUMN_INFO_PARAM_STRING = "-columnInfo";
    public const string UNCHECKED_PARAM_STRING = "-unchecked";
    public const string NO_CLIENT_PATH_STRING = "-noClient";
    public const string NO_LANG_PARAM_STRING = "-noLang";
    public const string LANG_NOT_MATCHING_PRINT_PARAM_STRING = "-printEmptyStringWhenLangNotMatching";
    public const string EXPORT_MYSQL_PARAM_STRING = "-exportMySQL";
    public const string PART_EXPORT_PARAM_STRING = "-part";
    public const string EXCEPT_EXPORT_PARAM_STRING = "-except";
    public const string ALLOWED_NULL_NUMBER_PARAM_STRING = "-allowedNullNumber";
    public const string EXPORT_CSV_PARAM_STRING = "-exportCsv";
    public const string EXPORT_CSV_PARAM_PARAM_STRING = "-exportCsvParam";
    public const string EXPORT_JSON_PARAM_STRING = "-exportJson";
    public const string EXPORT_JSON_PARAM_PARAM_STRING = "-exportJsonParam";

    public const string EXPORT_CSV_PARAM_SUBTYPE_EXPORT_PATH = "exportPath";
    public const string EXPORT_CSV_PARAM_SUBTYPE_EXTENSION = "extension";
    public const string EXPORT_CSV_PARAM_SUBTYPE_SPLIT_STRING = "splitString";
    public const string EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_NAME = "isExportColumnName";
    public const string EXPORT_CSV_PARAM_SUBTYPE_IS_EXPORT_COLUMN_DATA_TYPE = "isExportColumnDataType";

    public const string EXPORT_JSON_PARAM_SUBTYPE_EXPORT_PATH = "exportPath";
    public const string EXPORT_JSON_PARAM_SUBTYPE_EXTENSION = "extension";
    public const string EXPORT_JSON_PARAM_SUBTYPE_IS_FORMAT = "isFormat";

    public const string EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING = "$all";

    public const string SAVE_CONFIG_KEY_PROGRAM_PATH = "programPath";
    public const string SAVE_CONFIG_KEY_EXCEL_FOLDER_PATH = "excelFolderPath";
    public const string SAVE_CONFIG_KEY_EXPORT_LUA_FOLDER_PATH = "exportLuaFolderPath";
    public const string SAVE_CONFIG_KEY_CLIENT_FOLDER_PATH = "clientFolderPath";
    public const string SAVE_CONFIG_KEY_LANG_FILE_PATH = "langFilePath";

    public const string SAVE_CONFIG_KEY_IS_CHECKED_PART = "isCheckedPart";
    public const string SAVE_CONFIG_KEY_IS_CHECKED_EXCEPT = "isCheckedExcept";
    public const string SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_CSV = "isCheckedExportCsv";
    public const string SAVE_CONFIG_KEY_IS_CHECKED_EXPORT_JSON = "isCheckedExportJson";
    public const string SAVE_CONFIG_KEY_IS_CHECKED_USE_RELATIVE_PATH = "isCheckedUseRelativePath";

    public const string SAVE_CONFIG_PARAM_SUBTYPE_SEPARATOR = "_";
    public const string SAVE_CONFIG_KEY_VALUE_SEPARATOR = ":";

    public static string PROGRAM_FOLDER_PATH = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
    public static string PROGRAM_PATH = System.Windows.Forms.Application.ExecutablePath;
}
