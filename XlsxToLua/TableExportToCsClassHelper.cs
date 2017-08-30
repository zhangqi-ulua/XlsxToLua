using System;
using System.Collections.Generic;
using System.Text;

public class TableExportToCsClassHelper
{
    private static string _SET_DATA_TYPE_USING_STRING = "System.Collections.Generic";
    private static string _CS_CLASS_GET_SET_STRING = "{ get; set; }";
    private static string _CS_CLASS_INDENTATION_STRING = "    ";

    public static bool ExportTableToCsClass(TableInfo tableInfo, out string errorString)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int level = 0;
        bool isAddNamespace = false;
        List<FieldInfo> allFieldInfo = tableInfo.GetAllClientFieldInfo();

        // 先生成using内容
        if (AppValues.ExportCsClassUsing != null)
        {
            for (int i = 0; i < AppValues.ExportCsClassUsing.Count; ++i)
            {
                string usingString = AppValues.ExportCsClassUsing[i];
                stringBuilder.AppendLine(string.Concat("using ", usingString, ";"));
            }

            isAddNamespace = true;
        }
        // 如果未声明对System.Collections.Generic的引用，但实际用到了List或Dictionary，将强制进行添加
        if (AppValues.ExportCsClassUsing == null || !AppValues.ExportCsClassUsing.Contains(_SET_DATA_TYPE_USING_STRING))
        {
            foreach (FieldInfo fieldInfo in allFieldInfo)
            {
                if (fieldInfo.DataType == DataType.Array || fieldInfo.DataType == DataType.Dict || fieldInfo.DataType == DataType.Json || fieldInfo.DataType == DataType.TableString)
                {
                    stringBuilder.AppendLine(string.Concat("using ", _SET_DATA_TYPE_USING_STRING, ";"));
                    isAddNamespace = true;
                    break;
                }
            }
        }

        // 命名空间与类定义行之间通常需要空一行
        if (isAddNamespace == true)
            stringBuilder.AppendLine();

        // 再生成命名空间
        if (AppValues.ExportCsClassNamespace != null)
        {
            stringBuilder.AppendLine(string.Concat("namespace ", AppValues.ExportCsClassNamespace));
            stringBuilder.AppendLine("{");
            ++level;
        }

        // 最后根据字段信息生成csv对应的C#类
        // 优先使用表格config中指定的C#类名，若未设置则自动命名
        string className = null;
        if (tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME) && tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME].Count > 0 && !string.IsNullOrEmpty(tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME][0].Trim()))
            className = tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_CSV_CLASS_NAME][0].Trim();
        if (string.IsNullOrEmpty(className))
        {
            StringBuilder classNameStringBuilder = new StringBuilder();
            if (tableInfo.TableName.IndexOf('_') != -1)
            {
                bool isUnderlineOfLastChar = false;
                for (int i = 0; i < tableInfo.TableName.Length; ++i)
                {
                    char c = tableInfo.TableName[i];
                    if (c == '_')
                    {
                        isUnderlineOfLastChar = true;
                        continue;
                    }
                    if (isUnderlineOfLastChar == true)
                    {
                        classNameStringBuilder.Append(c.ToString().ToUpper());
                        isUnderlineOfLastChar = false;
                    }
                    else
                        classNameStringBuilder.Append(c.ToString().ToLower());
                }

                className = classNameStringBuilder[0].ToString();
            }
            else
                className = tableInfo.TableName;

            // 若统一配置了前缀和后缀，需进行添加
            className = string.Concat(AppValues.ExportCsvClassClassNamePrefix != null ? AppValues.ExportCsvClassClassNamePrefix : string.Empty, className, AppValues.ExportCsvClassClassNamePostfix != null ? AppValues.ExportCsvClassClassNamePostfix : string.Empty);
        }
        // 类名首字母大写
        string firstLetter = className[0].ToString().ToUpper();
        className = string.Concat(firstLetter, className.Substring(1));
        stringBuilder.Append(_GetCsClassIndentation(level));
        stringBuilder.AppendLine(string.Concat("public class ", className));
        // 开始类定义
        stringBuilder.Append(_GetCsClassIndentation(level));
        stringBuilder.AppendLine("{");
        ++level;
        // 逐个生成类字段信息
        foreach (FieldInfo fieldInfo in allFieldInfo)
        {
            stringBuilder.Append(_GetCsClassIndentation(level));
            stringBuilder.Append("public ");
            stringBuilder.Append(_GetCsClassFieldDefine(fieldInfo));
            stringBuilder.AppendLine(string.Concat(" ", fieldInfo.FieldName, " ", _CS_CLASS_GET_SET_STRING));
        }
        --level;
        // 闭合类定义
        stringBuilder.Append(_GetCsClassIndentation(level));
        stringBuilder.AppendLine("}");
        --level;
        // 闭合命名空间
        if (AppValues.ExportCsClassNamespace != null)
        {
            stringBuilder.AppendLine("}");
            --level;
        }

        if (Utils.SaveCsClassFile(className, stringBuilder.ToString()) == true)
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存csv对应C#类文件失败\n";
            return false;
        }
    }

    private static string _GetCsClassFieldDefine(FieldInfo fieldInfo)
    {
        StringBuilder stringBuilder = new StringBuilder();
        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Long:
            case DataType.Float:
            case DataType.Bool:
            case DataType.String:
                {
                    stringBuilder.Append(fieldInfo.DataType.ToString().ToLower());
                    break;
                }
            case DataType.Lang:
                {
                    stringBuilder.Append("string");
                    break;
                }
            case DataType.Json:
                {
                    stringBuilder.Append("LitJson.JsonData");
                    break;
                }
            case DataType.Array:
                {
                    stringBuilder.Append("List<");
                    stringBuilder.Append(_GetCsClassFieldDefine(fieldInfo.ChildField[0]));
                    stringBuilder.Append(">");
                    break;
                }
            case DataType.Dict:
                {
                    stringBuilder.Append("Dictionary<string, object>");
                    break;
                }
            case DataType.Date:
            case DataType.Time:
                {
                    stringBuilder.Append("DateTime");
                    break;
                }
            case DataType.TableString:
                {
                    if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.Seq)
                    {
                        stringBuilder.Append("List<");
                        switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
                        {
                            case TableStringValueType.True:
                                {
                                    stringBuilder.Append("bool");
                                    break;
                                }
                            case TableStringValueType.Table:
                                {
                                    stringBuilder.Append("Dictionary<string, object>");
                                    break;
                                }
                            case TableStringValueType.DataInIndex:
                                {
                                    string valueDataTypeString = _GetCsClassTableStringDataType(fieldInfo.TableStringFormatDefine.ValueDefine.DataInIndexDefine.DataType);
                                    if (valueDataTypeString == null)
                                        Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的seq型key的dataInIndex型value的数据类型非法");
                                    else
                                        stringBuilder.Append(valueDataTypeString);

                                    break;
                                }
                            default:
                                {
                                    Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的seq型key的value类型非法");
                                    break;
                                }
                        }

                        stringBuilder.Append(">");
                    }
                    else if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.DataInIndex)
                    {
                        stringBuilder.Append("Dictionary<");
                        // key
                        string keyDataTypeString = _GetCsClassTableStringDataType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType);
                        if (keyDataTypeString == null)
                            Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的dataInIndex型key的数据类型非法");
                        else
                            stringBuilder.Append(keyDataTypeString);

                        stringBuilder.Append(" ,");
                        // value
                        switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
                        {
                            case TableStringValueType.True:
                                {
                                    stringBuilder.Append("bool");
                                    break;
                                }
                            case TableStringValueType.Table:
                                {
                                    stringBuilder.Append("Dictionary<string, object>");
                                    break;
                                }
                            case TableStringValueType.DataInIndex:
                                {
                                    string valueDataTypeString = _GetCsClassTableStringDataType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType);
                                    if (valueDataTypeString == null)
                                        Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的dataInIndex型key的dataInIndex型value的数据类型非法");
                                    else
                                        stringBuilder.Append(valueDataTypeString);

                                    break;
                                }
                            default:
                                {
                                    Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的dataInIndex型key的value类型非法");
                                    break;
                                }
                        }

                        stringBuilder.Append(">");
                    }
                    else
                        Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件中tableString型字段的key非法");

                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit("用_GetCsClassFieldDefine函数导出csv对应C#类文件的字段DataType非法");
                    break;
                }
        }

        return stringBuilder.ToString();
    }

    private static string _GetCsClassTableStringDataType(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Int:
            case DataType.Long:
            case DataType.Float:
            case DataType.String:
            case DataType.Bool:
                return dataType.ToString().ToLower();
            case DataType.Lang:
                return "string";
            default:
                return null;
        }
    }

    private static string _GetCsClassIndentation(int level)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < level; ++i)
            stringBuilder.Append(_CS_CLASS_INDENTATION_STRING);

        return stringBuilder.ToString();
    }
}
