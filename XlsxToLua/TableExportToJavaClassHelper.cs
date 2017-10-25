using System;
using System.Collections.Generic;
using System.Text;

public class TableExportToJavaClassHelper
{
    private static string _ARRAY_DATA_TYPE_IMPORT_STRING = "java.util.ArrayList";
    private static string _DICT_DATA_TYPE_IMPORT_STRING = "java.util.HashMap";
    private static string _DATE_DATA_TYPE_TO_DATE_IMPORT_STRING = "java.util.Date";
    private static string _DATE_DATA_TYPE_TO_CALENDAR_IMPORT_STRING = "java.util.Calendar";
    private static string _JAVA_CLASS_INDENTATION_STRING = "\t";

    public static bool ExportTableToJavaClass(TableInfo tableInfo, out string errorString)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int level = 0;
        bool isAddImport = false;
        List<FieldInfo> allFieldInfo = tableInfo.GetAllClientFieldInfo();

        // 先生成package
        stringBuilder.AppendLine(string.Concat("package ", AppValues.ExportJavaClassPackage, ";"));
        // package与下面内容空一行
        stringBuilder.AppendLine();

        // 再生成import内容
        if (AppValues.ExportJavaClassImport != null)
        {
            for (int i = 0; i < AppValues.ExportJavaClassImport.Count; ++i)
            {
                string importString = AppValues.ExportJavaClassImport[i];
                stringBuilder.AppendLine(string.Concat("import ", importString, ";"));
            }

            isAddImport = true;
        }
        // 如果实际用到了array、dict或时间型，但没有声明对相应类库的引用，就要强制添加
        if (AppValues.ExportJavaClassImport == null || !AppValues.ExportJavaClassImport.Contains(_ARRAY_DATA_TYPE_IMPORT_STRING))
        {
            foreach (FieldInfo fieldInfo in allFieldInfo)
            {
                if (fieldInfo.DataType == DataType.Array)
                {
                    stringBuilder.AppendLine(string.Concat("import ", _ARRAY_DATA_TYPE_IMPORT_STRING, ";"));
                    isAddImport = true;
                    break;
                }
            }
        }
        if (AppValues.ExportJavaClassImport == null || !AppValues.ExportJavaClassImport.Contains(_DICT_DATA_TYPE_IMPORT_STRING))
        {
            foreach (FieldInfo fieldInfo in allFieldInfo)
            {
                if (fieldInfo.DataType == DataType.Dict)
                {
                    stringBuilder.AppendLine(string.Concat("import ", _DICT_DATA_TYPE_IMPORT_STRING, ";"));
                    isAddImport = true;
                    break;
                }
            }
        }
        bool isNeedDateTypeImport = false;
        foreach (FieldInfo fieldInfo in allFieldInfo)
        {
            if (fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
            {
                isNeedDateTypeImport = true;
                break;
            }
        }
        if (isNeedDateTypeImport == true && AppValues.ExportJavaClassIsUseDate == true && (AppValues.ExportJavaClassImport == null || !AppValues.ExportJavaClassImport.Contains(_DATE_DATA_TYPE_TO_DATE_IMPORT_STRING)))
        {
            stringBuilder.AppendLine(string.Concat("import ", _DATE_DATA_TYPE_TO_DATE_IMPORT_STRING, ";"));
            isAddImport = true;
        }
        if (isNeedDateTypeImport == true && AppValues.ExportJavaClassIsUseDate == false && (AppValues.ExportJavaClassImport == null || !AppValues.ExportJavaClassImport.Contains(_DATE_DATA_TYPE_TO_CALENDAR_IMPORT_STRING)))
        {
            stringBuilder.AppendLine(string.Concat("import ", _DATE_DATA_TYPE_TO_CALENDAR_IMPORT_STRING, ";"));
            isAddImport = true;
        }
        // package与类定义行之间通常需要空一行
        if (isAddImport == true)
            stringBuilder.AppendLine();

        // 最后根据字段信息生成csv对应的Java类
        // 优先使用表格config中指定的Java类名，若未设置则自动命名
        string className = null;
        if (tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME) && tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME].Count > 0 && !string.IsNullOrEmpty(tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME][0].Trim()))
            className = tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME][0].Trim();
        if (string.IsNullOrEmpty(className))
        {
            // 自动命名采用驼峰式
            className = Utils.GetCamelCaseString(tableInfo.TableName);

            // 若统一配置了前缀和后缀，需进行添加（但注意如果配置了前缀，上面生成的驼峰式类名首字母要改为大写）
            if (string.IsNullOrEmpty(AppValues.ExportCsvClassClassNamePrefix) == false)
                className = string.Concat(AppValues.ExportCsvClassClassNamePrefix, char.ToUpper(className[0]), className.Substring(1));
            if (string.IsNullOrEmpty(AppValues.ExportCsvClassClassNamePostfix) == false)
                className = className + AppValues.ExportCsvClassClassNamePostfix;
        }

        // 类名首字母大写
        char firstLetter = char.ToUpper(className[0]);
        className = string.Concat(firstLetter, className.Substring(1));
        stringBuilder.AppendLine(string.Concat("public class ", className, " {"));
        // 逐个生成类字段信息
        ++level;
        foreach (FieldInfo fieldInfo in allFieldInfo)
        {
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.Append("private ");
            stringBuilder.Append(_GetJavaClassFieldDefine(fieldInfo));
            stringBuilder.AppendLine(string.Concat(" ", fieldInfo.FieldName, ";"));
        }
        // 字段声明与下面内容空一行
        stringBuilder.AppendLine();
        // 为每个字段生成get、set方法
        foreach (FieldInfo fieldInfo in allFieldInfo)
        {
            stringBuilder.Append(_GetJavaClassIndentation(level));
            // get方法
            stringBuilder.Append("public ");
            stringBuilder.Append(_GetJavaClassFieldDefine(fieldInfo));
            stringBuilder.AppendLine(string.Concat(" get", fieldInfo.FieldName.Substring(0, 1).ToUpper(), fieldInfo.FieldName.Substring(1), "() {"));
            ++level;
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine(string.Concat("return ", fieldInfo.FieldName, ";"));
            --level;
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine("}");
            // 与set函数空一行
            stringBuilder.AppendLine();
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.Append("public void ");
            stringBuilder.AppendLine(string.Concat("set", fieldInfo.FieldName.Substring(0, 1).ToUpper(), fieldInfo.FieldName.Substring(1), "(", _GetJavaClassFieldDefine(fieldInfo), " ", fieldInfo.FieldName, ") {"));
            ++level;
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine(string.Concat("this.", fieldInfo.FieldName, " = ", fieldInfo.FieldName, ";"));
            --level;
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine("}");
        }
        // 无参构造函数
        if (AppValues.ExportJavaClassisGenerateConstructorWithoutFields == true)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine(string.Concat("public ", className, "() {"));
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine("}");
        }
        // 含全部参数的构造函数
        if (AppValues.ExportJavaClassIsGenerateConstructorWithAllFields == true)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(_GetJavaClassIndentation(level));
            List<string> allFieldDataTypeAndName = new List<string>();
            foreach (FieldInfo fieldInfo in allFieldInfo)
            {
                allFieldDataTypeAndName.Add(string.Concat(_GetJavaClassFieldDefine(fieldInfo), " ", fieldInfo.FieldName));
            }

            stringBuilder.AppendLine(string.Concat("public ", className, "(", Utils.CombineString(allFieldDataTypeAndName, ", "), ") {"));
            ++level;
            foreach (FieldInfo fieldInfo in allFieldInfo)
            {
                stringBuilder.Append(_GetJavaClassIndentation(level));
                stringBuilder.AppendLine(string.Concat("this.", fieldInfo.FieldName, " = ", fieldInfo.FieldName, ";"));
            }
            --level;
            stringBuilder.Append(_GetJavaClassIndentation(level));
            stringBuilder.AppendLine("}");
        }

        --level;
        // 闭合类定义
        stringBuilder.AppendLine("}");

        if (Utils.SaveJavaClassFile(className, stringBuilder.ToString()) == true)
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存csv对应Java类文件失败\n";
            return false;
        }
    }

    private static string _GetJavaClassFieldDefine(FieldInfo fieldInfo)
    {
        StringBuilder stringBuilder = new StringBuilder();
        switch (fieldInfo.DataType)
        {
            case DataType.Int:
                {
                    stringBuilder.Append("Integer");
                    break;
                }
            case DataType.Long:
                {
                    stringBuilder.Append("Long");
                    break;
                }
            case DataType.Float:
                {
                    stringBuilder.Append("Float");
                    break;
                }
            case DataType.Bool:
                {
                    stringBuilder.Append("Boolean");
                    break;
                }
            case DataType.String:
            case DataType.Lang:
                {
                    stringBuilder.Append("String");
                    break;
                }
            case DataType.Json:
                {
                    stringBuilder.Append("Object");
                    break;
                }
            case DataType.Array:
                {
                    stringBuilder.Append("ArrayList<");
                    stringBuilder.Append(_GetJavaClassFieldDefine(fieldInfo.ChildField[0]));
                    stringBuilder.Append(">");
                    break;
                }
            case DataType.Dict:
                {
                    stringBuilder.Append("HashMap<String, Object>");
                    break;
                }
            case DataType.Date:
            case DataType.Time:
                {
                    if (AppValues.ExportJavaClassIsUseDate == true)
                        stringBuilder.Append("Date");
                    else
                        stringBuilder.Append("Calendar");

                    break;
                }
            case DataType.TableString:
                {
                    if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.Seq)
                    {
                        stringBuilder.Append("ArrayList<");
                        switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
                        {
                            case TableStringValueType.True:
                                {
                                    stringBuilder.Append("Boolean");
                                    break;
                                }
                            case TableStringValueType.Table:
                                {
                                    stringBuilder.Append("HashMap<String, Object>");
                                    break;
                                }
                            case TableStringValueType.DataInIndex:
                                {
                                    string valueDataTypeString = _GetJavaClassTableStringDataType(fieldInfo.TableStringFormatDefine.ValueDefine.DataInIndexDefine.DataType);
                                    if (valueDataTypeString == null)
                                        Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的seq型key的dataInIndex型value的数据类型非法");
                                    else
                                        stringBuilder.Append(valueDataTypeString);

                                    break;
                                }
                            default:
                                {
                                    Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的seq型key的value类型非法");
                                    break;
                                }
                        }

                        stringBuilder.Append(">");
                    }
                    else if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.DataInIndex)
                    {
                        stringBuilder.Append("HashMap<");
                        // key
                        string keyDataTypeString = _GetJavaClassTableStringDataType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType);
                        if (keyDataTypeString == null)
                            Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的dataInIndex型key的数据类型非法");
                        else
                            stringBuilder.Append(keyDataTypeString);

                        stringBuilder.Append(" ,");
                        // value
                        switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
                        {
                            case TableStringValueType.True:
                                {
                                    stringBuilder.Append("Boolean");
                                    break;
                                }
                            case TableStringValueType.Table:
                                {
                                    stringBuilder.Append("HashMap<String, Object>");
                                    break;
                                }
                            case TableStringValueType.DataInIndex:
                                {
                                    string valueDataTypeString = _GetJavaClassTableStringDataType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType);
                                    if (valueDataTypeString == null)
                                        Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的dataInIndex型key的dataInIndex型value的数据类型非法");
                                    else
                                        stringBuilder.Append(valueDataTypeString);

                                    break;
                                }
                            default:
                                {
                                    Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的dataInIndex型key的value类型非法");
                                    break;
                                }
                        }

                        stringBuilder.Append(">");
                    }
                    else
                        Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件中tableString型字段的key非法");

                    break;
                }
            case DataType.MapString:
                {
                    stringBuilder.Append("Object");
                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit("用_GetJavaClassFieldDefine函数导出csv对应Java类文件的字段DataType非法");
                    break;
                }
        }

        return stringBuilder.ToString();
    }

    private static string _GetJavaClassTableStringDataType(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Int:
                return "Integer";
            case DataType.Long:
                return "Long";
            case DataType.Float:
                return "Float";
            case DataType.Bool:
                return "Boolean";
            case DataType.String:
            case DataType.Lang:
                return "String";
            default:
                return null;
        }
    }

    private static string _GetJavaClassIndentation(int level)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < level; ++i)
            stringBuilder.Append(_JAVA_CLASS_INDENTATION_STRING);

        return stringBuilder.ToString();
    }
}
