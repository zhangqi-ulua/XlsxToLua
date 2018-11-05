using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

public class TableExportToJsonHelper
{
    // 用于缩进json的字符串
    private static string _JSON_INDENTATION_STRING = "\t";

    public static bool ExportTableToJson(TableInfo tableInfo, out string errorString)
    {
        StringBuilder content = new StringBuilder();

        // 若生成为各行数据对应的json object包含在一个json array的形式
        if (AppValues.ExportJsonIsExportJsonArrayFormat == true)
        {
            // 生成json字符串开头，每行数据为一个json object，作为整张表json array的元素
            content.Append("[");

            // 逐行读取表格内容生成json
            List<FieldInfo> allField = tableInfo.GetAllClientFieldInfo();
            int dataCount = tableInfo.GetKeyColumnFieldInfo().Data.Count;
            int fieldCount = allField.Count;
            for (int row = 0; row < dataCount; ++row)
            {
                // 生成一行数据json object的开头
                content.Append("{");

                for (int column = 0; column < fieldCount; ++column)
                {
                    string oneFieldString = _GetOneField(allField[column], row, out errorString);
                    if (errorString != null)
                    {
                        errorString = string.Format("额外导出表格{0}为json文件失败，", tableInfo.TableName) + errorString;
                        return false;
                    }
                    else
                        content.Append(oneFieldString);
                }

                // 去掉本行最后一个字段后多余的英文逗号，json语法不像lua那样最后一个字段后的逗号可有可无
                content.Remove(content.Length - 1, 1);
                // 生成一行数据json object的结尾
                content.Append("}");
                // 每行的json object后加英文逗号
                content.Append(",");
            }

            // 去掉最后一行后多余的英文逗号，此处要特殊处理当表格中没有任何数据行时的情况
            if (content.Length > 1)
                content.Remove(content.Length - 1, 1);
            // 生成json字符串结尾
            content.Append("]");
        }
        else
        {
            // 生成json字符串开头，每行数据以表格主键列为key，各字段信息组成的json object为value，作为整张表json object的元素
            content.Append("{");

            // 逐行读取表格内容生成json
            List<FieldInfo> allField = tableInfo.GetAllClientFieldInfo();
            FieldInfo keyColumnInfo = tableInfo.GetKeyColumnFieldInfo();
            int dataCount = keyColumnInfo.Data.Count;
            int fieldCount = allField.Count;
            for (int row = 0; row < dataCount; ++row)
            {
                // 将主键列的值作为key
                string keyString = null;
                if (keyColumnInfo.DataType == DataType.String)
                {
                    keyString = _GetStringValue(keyColumnInfo, row);
                    content.Append(keyString);
                }
                else if (keyColumnInfo.DataType == DataType.Int || keyColumnInfo.DataType == DataType.Long)
                {
                    keyString = _GetNumberValue(keyColumnInfo, row);
                    content.Append("\"").Append(keyString).Append("\"");
                }
                else
                {
                    errorString = string.Format("ExportTableToJson函数中未定义{0}类型的主键数值导出至json文件的形式", keyColumnInfo.DataType);
                    Utils.LogErrorAndExit(errorString);
                    return false;
                }

                // 生成一行数据json object的开头
                content.Append(":{");

                int startColumn = (AppValues.ExportJsonIsExportJsonMapIncludeKeyColumnValue == true ? 0 : 1);
                for (int column = startColumn; column < fieldCount; ++column)
                {
                    string oneFieldString = _GetOneField(allField[column], row, out errorString);
                    if (errorString != null)
                    {
                        errorString = string.Format("额外导出表格{0}为json文件失败，", tableInfo.TableName) + errorString;
                        return false;
                    }
                    else
                        content.Append(oneFieldString);
                }

                // 去掉本行最后一个字段后多余的英文逗号，json语法不像lua那样最后一个字段后的逗号可有可无
                content.Remove(content.Length - 1, 1);
                // 生成一行数据json object的结尾
                content.Append("}");
                // 每行的json object后加英文逗号
                content.Append(",");
            }

            // 去掉最后一行后多余的英文逗号，此处要特殊处理当表格中没有任何数据行时的情况
            if (content.Length > 1)
                content.Remove(content.Length - 1, 1);
            // 生成json字符串结尾
            content.Append("}");
        }

        string exportString = content.ToString();

        // 如果声明了要整理为带缩进格式的形式
        if (AppValues.ExportJsonIsFormat == true)
            exportString = _FormatJson(exportString);

        // 保存为json文件
        if (Utils.SaveJsonFile(tableInfo.TableName, exportString) == true)
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存为json文件失败\n";
            return false;
        }
    }

    private static string _GetOneField(FieldInfo fieldInfo, int row, out string errorString)
    {
        StringBuilder content = new StringBuilder();
        errorString = null;

        // 变量名，注意array下属的子元素在json中不含key的声明
        if (!(fieldInfo.ParentField != null && fieldInfo.ParentField.DataType == DataType.Array))
        {
            content.Append("\"").Append(fieldInfo.FieldName).Append("\"");
            content.Append(":");
        }

        // 对应数据值
        string value = null;
        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Long:
            case DataType.Float:
                {
                    value = _GetNumberValue(fieldInfo, row);
                    break;
                }
            case DataType.String:
                {
                    value = _GetStringValue(fieldInfo, row);
                    break;
                }
            case DataType.Bool:
                {
                    value = _GetBoolValue(fieldInfo, row);
                    break;
                }
            case DataType.Lang:
                {
                    value = _GetLangValue(fieldInfo, row);
                    break;
                }
            case DataType.Date:
                {
                    value = _GetDateValue(fieldInfo, row);
                    break;
                }
            case DataType.Time:
                {
                    value = _GetTimeValue(fieldInfo, row);
                    break;
                }
            case DataType.Json:
                {
                    value = _GetJsonValue(fieldInfo, row);
                    break;
                }
            case DataType.TableString:
                {
                    value = _GetTableStringValue(fieldInfo, row, out errorString);
                    break;
                }
            case DataType.MapString:
                {
                    value = _GetMapStringValue(fieldInfo, row);
                    break;
                }
            case DataType.Dict:
                {
                    value = _GetDictValue(fieldInfo, row, out errorString);
                    break;
                }
            case DataType.Array:
                {
                    value = _GetArrayValue(fieldInfo, row, out errorString);
                    break;
                }
            default:
                {
                    errorString = string.Format("_GetOneField函数中未定义{0}类型数据导出至json文件的形式", fieldInfo.DataType);
                    Utils.LogErrorAndExit(errorString);
                    return null;
                }
        }

        if (errorString != null)
        {
            errorString = string.Format("第{0}行第{1}列的数据存在错误无法导出，", row + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1)) + errorString;
            return null;
        }

        content.Append(value);
        // 一个字段结尾加逗号
        content.Append(",");

        return content.ToString();
    }

    private static string _GetNumberValue(FieldInfo fieldInfo, int row)
    {
        if (fieldInfo.Data[row] == null)
            return "null";
        else
            return fieldInfo.Data[row].ToString();
    }

    private static string _GetStringValue(FieldInfo fieldInfo, int row)
    {
        StringBuilder content = new StringBuilder();

        content.Append("\"");
        content.Append(fieldInfo.Data[row].ToString().Replace("\n", "\\n").Replace("\"", "\\\""));
        content.Append("\"");

        return content.ToString();
    }

    private static string _GetBoolValue(FieldInfo fieldInfo, int row)
    {
        if ((bool)fieldInfo.Data[row] == true)
            return "true";
        else
            return "false";
    }

    private static string _GetLangValue(FieldInfo fieldInfo, int row)
    {
        StringBuilder content = new StringBuilder();

        if (fieldInfo.Data[row] != null)
        {
            content.Append("\"");
            content.Append(fieldInfo.Data[row].ToString().Replace("\n", "\\n").Replace("\"", "\\\""));
            content.Append("\"");
        }
        else
        {
            if (AppValues.IsPrintEmptyStringWhenLangNotMatching == true)
                content.Append("\"\"");
            else
                content.Append("null");
        }

        return content.ToString();
    }

    private static string _GetDateValue(FieldInfo fieldInfo, int row)
    {
        StringBuilder content = new StringBuilder();

        DateFormatType dateFormatType = TableAnalyzeHelper.GetDateFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_LUA_FORMAT].ToString());
        string exportFormatString = null;
        // 若date型声明toLua的格式为dateTable，则按input格式进行导出
        if (dateFormatType == DateFormatType.DataTable)
        {
            dateFormatType = TableAnalyzeHelper.GetDateFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_INPUT_FORMAT].ToString());
            exportFormatString = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_INPUT_FORMAT].ToString();
        }
        else
            exportFormatString = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_LUA_FORMAT].ToString();

        switch (dateFormatType)
        {
            case DateFormatType.FormatString:
                {
                    if (fieldInfo.Data[row] == null)
                        content.Append("null");
                    else
                        content.Append("\"").Append(((DateTime)(fieldInfo.Data[row])).ToString(exportFormatString)).Append("\"");

                    break;
                }
            case DateFormatType.ReferenceDateSec:
                {
                    if (fieldInfo.Data[row] == null)
                        content.Append("null");
                    else
                        content.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE_LOCAL).TotalSeconds);

                    break;
                }
            case DateFormatType.ReferenceDateMsec:
                {
                    if (fieldInfo.Data[row] == null)
                        content.Append("null");
                    else
                        content.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE_LOCAL).TotalMilliseconds);

                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit("错误：用_GetDateValue函数导出json文件的date型的DateFormatType非法");
                    break;
                }
        }

        return content.ToString();
    }

    private static string _GetTimeValue(FieldInfo fieldInfo, int row)
    {
        StringBuilder content = new StringBuilder();

        TimeFormatType timeFormatType = TableAnalyzeHelper.GetTimeFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString());
        switch (timeFormatType)
        {
            case TimeFormatType.FormatString:
                {
                    if (fieldInfo.Data[row] == null)
                        content.Append("null");
                    else
                        content.Append("\"").Append(((DateTime)(fieldInfo.Data[row])).ToString(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString())).Append("\"");

                    break;
                }
            case TimeFormatType.ReferenceTimeSec:
                {
                    if (fieldInfo.Data[row] == null)
                        content.Append("null");
                    else
                        content.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalSeconds);

                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit("错误：用_GetTimeValue函数导出json文件的time型的TimeFormatType非法");
                    break;
                }
        }

        return content.ToString();
    }

    private static string _GetJsonValue(FieldInfo fieldInfo, int row)
    {
        if (fieldInfo.Data[row] == null)
            return "null";
        else
        {
            //// 将json字符串进行格式整理，去除引号之外的所有空白字符
            //StringBuilder stringBuilder = new StringBuilder();
            //string inputJsonString = fieldInfo.JsonString[row];
            //bool isInQuotationMarks = false;
            //for (int i = 0; i < inputJsonString.Length; ++i)
            //{
            //    char c = inputJsonString[i];

            //    if (c == '"')
            //    {
            //        stringBuilder.Append('"');
            //        if (i > 0 && inputJsonString[i - 1] != '\\')
            //            isInQuotationMarks = !isInQuotationMarks;
            //    }
            //    else if (c == ' ')
            //    {
            //        if (isInQuotationMarks == true)
            //            stringBuilder.Append(' ');
            //    }
            //    else if (c != '\n' && c != '\r' && c != '\t')
            //        stringBuilder.Append(c);
            //}

            //return stringBuilder.ToString();

            return JsonMapper.ToJson(fieldInfo.Data[row]);
        }
    }

    private static string _GetMapStringValue(FieldInfo fieldInfo, int row)
    {
        if (fieldInfo.Data[row] == null)
            return "null";
        else
            return JsonMapper.ToJson(fieldInfo.Data[row]);
    }

    private static string _GetTableStringValue(FieldInfo fieldInfo, int row, out string errorString)
    {
        errorString = null;
        if (fieldInfo.Data[row] == null)
            return "null";

        StringBuilder content = new StringBuilder();
        string inputData = fieldInfo.Data[row].ToString();

        // tableString字符串中不允许出现英文引号、斜杠
        if (inputData.Contains("\"") || inputData.Contains("\\") || inputData.Contains("/"))
        {
            errorString = "tableString字符串中不允许出现英文引号、斜杠";
            return null;
        }

        // 若tableString的key为#seq，则生成json array，否则生成json object
        if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.Seq)
            content.Append("[");
        else
            content.Append("{");

        // 每组数据间用英文分号分隔
        string[] allDataString = inputData.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        // 记录每组数据中的key值（转为字符串后的），不允许出现相同的key（key：每组数据中的key值， value：第几组数据，从0开始记）
        Dictionary<string, int> stringKeys = new Dictionary<string, int>();
        for (int i = 0; i < allDataString.Length; ++i)
        {
            // 根据key的格式定义生成key
            switch (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType)
            {
                case TableStringKeyType.Seq:
                    break;
                case TableStringKeyType.DataInIndex:
                    {
                        string value = _GetDataInIndexType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine, allDataString[i], out errorString);
                        if (errorString == null)
                        {
                            if (fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.Int || fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.Long)
                            {
                                // 检查key是否在该组数据中重复
                                if (stringKeys.ContainsKey(value))
                                    errorString = string.Format("第{0}组数据与第{1}组数据均为相同的key（{2}）", stringKeys[value] + 1, i + 1, value);
                                else
                                {
                                    stringKeys.Add(value, i);
                                    content.AppendFormat("\"{0}\"", value);
                                }
                            }
                            else if (fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.String)
                            {
                                // string型的key不允许为空或纯空格且必须符合变量名的规范
                                value = value.Trim();
                                if (TableCheckHelper.CheckFieldName(value, out errorString))
                                {
                                    // 检查key是否在该组数据中重复
                                    if (stringKeys.ContainsKey(value))
                                        errorString = string.Format("第{0}组数据与第{1}组数据均为相同的key（{2}）", stringKeys[value] + 1, i + 1, value);
                                    else
                                    {
                                        stringKeys.Add(value, i);
                                        content.AppendFormat("\"{0}\"", value);
                                    }
                                }
                                else
                                    errorString = "string型的key不符合变量名定义规范，" + errorString;
                            }
                            else
                            {
                                Utils.LogErrorAndExit("错误：用_GetTableStringValue函数导出非int、long或string型的key值");
                                return null;
                            }
                        }

                        content.Append(":");

                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit("错误：用_GetTableStringValue函数导出未知类型的key");
                        return null;
                    }
            }
            if (errorString != null)
            {
                errorString = string.Format("tableString中第{0}组数据（{1}）的key数据存在错误，", i + 1, allDataString[i]) + errorString;
                return null;
            }

            // 根据value的格式定义生成value
            switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
            {
                case TableStringValueType.True:
                    {
                        content.Append("true");
                        break;
                    }
                case TableStringValueType.DataInIndex:
                    {
                        string value = _GetDataInIndexType(fieldInfo.TableStringFormatDefine.ValueDefine.DataInIndexDefine, allDataString[i], out errorString);
                        if (errorString == null)
                        {
                            DataType dataType = fieldInfo.TableStringFormatDefine.ValueDefine.DataInIndexDefine.DataType;
                            if (dataType == DataType.String || dataType == DataType.Lang)
                                content.AppendFormat("\"{0}\"", value);
                            else
                                content.Append(value);
                        }

                        break;
                    }
                case TableStringValueType.Table:
                    {
                        content.Append("{");

                        // 依次输出table中定义的子元素
                        foreach (TableElementDefine elementDefine in fieldInfo.TableStringFormatDefine.ValueDefine.TableValueDefineList)
                        {
                            content.AppendFormat("\"{0}\"", elementDefine.KeyName);
                            content.Append(":");
                            string value = _GetDataInIndexType(elementDefine.DataInIndexDefine, allDataString[i], out errorString);
                            if (errorString == null)
                            {
                                if (elementDefine.DataInIndexDefine.DataType == DataType.String || elementDefine.DataInIndexDefine.DataType == DataType.Lang)
                                    content.AppendFormat("\"{0}\"", value);
                                else
                                    content.Append(value);
                            }
                            content.Append(",");
                        }

                        // 去掉最后一个子元素后多余的英文逗号
                        content.Remove(content.Length - 1, 1);
                        content.Append("}");

                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit("错误：用_GetTableStringValue函数导出未知类型的value");
                        return null;
                    }
            }
            if (errorString != null)
            {
                errorString = string.Format("tableString中第{0}组数据（{1}）的value数据存在错误，", i + 1, allDataString[i]) + errorString;
                return null;
            }

            // 每组数据生成完毕后加逗号
            content.Append(",");
        }

        // 去掉最后一组后多余的英文逗号
        content.Remove(content.Length - 1, 1);
        if (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType == TableStringKeyType.Seq)
            content.Append("]");
        else
            content.Append("}");

        return content.ToString();
    }

    /// <summary>
    /// 将形如#1(int)的数据定义解析为要输出的字符串
    /// </summary>
    private static string _GetDataInIndexType(DataInIndexDefine define, string oneDataString, out string errorString)
    {
        // 一组数据中的子元素用英文逗号分隔
        string[] allElementString = oneDataString.Trim().Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        // 检查是否存在指定序号的数据
        if (allElementString.Length < define.DataIndex)
        {
            errorString = string.Format("解析#{0}({1})类型的数据错误，输入的数据中只有{2}个子元素", define.DataIndex, define.DataType.ToString(), allElementString.Length);
            return null;
        }
        // 检查是否为指定类型的合法数据
        string inputString = allElementString[define.DataIndex - 1];
        if (define.DataType != DataType.String)
            inputString = inputString.Trim();

        string value = _GetDataStringInTableString(inputString, define.DataType, out errorString);
        if (errorString != null)
        {
            errorString = string.Format("解析#{0}({1})类型的数据错误，", define.DataIndex, define.DataType.ToString()) + errorString;
            return null;
        }
        else
            return value;
    }

    /// <summary>
    /// 将tableString类型数据字符串中的某个所填数据转为需要输出的字符串
    /// </summary>
    private static string _GetDataStringInTableString(string inputData, DataType dataType, out string errorString)
    {
        string result = null;
        errorString = null;

        switch (dataType)
        {
            case DataType.Bool:
                {
                    if ("1".Equals(inputData) || "true".Equals(inputData, StringComparison.CurrentCultureIgnoreCase))
                        result = "true";
                    else if ("0".Equals(inputData) || "false".Equals(inputData, StringComparison.CurrentCultureIgnoreCase))
                        result = "false";
                    else
                        errorString = string.Format("输入的\"{0}\"不是合法的bool值，正确填写bool值方式为填1或true代表真，0或false代表假", inputData);

                    break;
                }
            case DataType.Int:
            case DataType.Long:
                {
                    long longValue;
                    bool isValid = long.TryParse(inputData, out longValue);
                    if (isValid)
                        result = longValue.ToString();
                    else
                        errorString = string.Format("输入的\"{0}\"不是合法的{1}类型的值", inputData, dataType);

                    break;
                }
            case DataType.Float:
                {
                    float floatValue;
                    bool isValid = float.TryParse(inputData, out floatValue);
                    if (isValid)
                        result = floatValue.ToString();
                    else
                        errorString = string.Format("输入的\"{0}\"不是合法的float类型的值", inputData);

                    break;
                }
            case DataType.String:
                {
                    result = inputData;
                    break;
                }
            case DataType.Lang:
                {
                    if (AppValues.LangData.ContainsKey(inputData))
                    {
                        string langValue = AppValues.LangData[inputData];
                        if (langValue.Contains("\"") || langValue.Contains("\\") || langValue.Contains("/") || langValue.Contains(",") || langValue.Contains(";"))
                            errorString = string.Format("tableString中的lang型数据中不允许出现英文引号、斜杠、逗号、分号，你输入的key（{0}）对应在lang文件中的值为\"{1}\"", inputData, langValue);
                        else
                            result = langValue;
                    }
                    else
                        errorString = string.Format("输入的lang型数据的key（{0}）在lang文件中找不到对应的value", inputData);

                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit(string.Format("错误：用_GetDataInTableString函数解析了tableString中不支持的数据类型{0}", dataType));
                    break;
                }
        }

        return result;
    }

    private static string _GetDictValue(FieldInfo fieldInfo, int row, out string errorString)
    {
        StringBuilder content = new StringBuilder();

        // 如果该dict数据用-1标为无效，则赋值为null
        if ((bool)fieldInfo.Data[row] == false)
            content.Append("null");
        else
        {
            // dict生成json object
            content.Append("{");

            // 逐个对子元素进行生成
            foreach (FieldInfo childField in fieldInfo.ChildField)
            {
                string oneFieldString = _GetOneField(childField, row, out errorString);
                if (errorString != null)
                    return null;
                else
                    content.Append(oneFieldString);
            }

            // 去掉最后一个子元素末尾多余的英文逗号
            content.Remove(content.Length - 1, 1);

            content.Append("}");
        }

        errorString = null;
        return content.ToString();
    }

    private static string _GetArrayValue(FieldInfo fieldInfo, int row, out string errorString)
    {
        StringBuilder content = new StringBuilder();

        // 如果该array数据用-1标为无效，则赋值为null
        if ((bool)fieldInfo.Data[row] == false)
            content.Append("null");
        else
        {
            // array生成json array
            content.Append("[");

            // 逐个对子元素进行生成
            bool hasValidChild = false;
            foreach (FieldInfo childField in fieldInfo.ChildField)
            {
                string oneFieldString = _GetOneField(childField, row, out errorString);
                if (errorString != null)
                    return null;

                // json array中不允许null元素
                if (!"null,".Equals(oneFieldString))
                {
                    content.Append(oneFieldString);
                    hasValidChild = true;
                }
            }

            // 去掉最后一个子元素末尾多余的英文逗号
            if (hasValidChild == true)
                content.Remove(content.Length - 1, 1);

            content.Append("]");
        }

        errorString = null;
        return content.ToString();
    }

    /// <summary>
    /// 将紧凑型的json字符串整理为带缩进和换行的形式，需注意string型值中允许含有括号和\"
    /// </summary>
    private static string _FormatJson(string json)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int level = 0;
        bool isInQuotationMarks = false;
        for (int i = 0; i < json.Length; ++i)
        {
            char c = json[i];

            if (c == '[' || c == '{')
            {
                stringBuilder.Append(c);
                if (isInQuotationMarks == false)
                {
                    stringBuilder.AppendLine();
                    ++level;
                    stringBuilder.Append(_GetJsonIndentation(level));
                }
            }
            else if (c == ']' || c == '}')
            {
                if (isInQuotationMarks == false)
                {
                    stringBuilder.AppendLine();
                    --level;
                    stringBuilder.Append(_GetJsonIndentation(level));
                }
                stringBuilder.Append(c);
            }
            else if (c == ',')
            {
                stringBuilder.Append(c);
                if (isInQuotationMarks == false)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(_GetJsonIndentation(level));
                }
            }
            else if (c == '"')
            {
                stringBuilder.Append('"');
                if (i > 0 && json[i - 1] != '\\')
                    isInQuotationMarks = !isInQuotationMarks;
            }
            else
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    private static string _GetJsonIndentation(int level)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < level; ++i)
            stringBuilder.Append(_JSON_INDENTATION_STRING);

        return stringBuilder.ToString();
    }
}
