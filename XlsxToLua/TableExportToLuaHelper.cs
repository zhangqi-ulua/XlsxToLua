using System.Collections.Generic;
using System.Text;

public class TableExportToLuaHelper
{
    // 用于缩进lua table的字符
    private static char _LUA_TABLE_INDENTATION_CHAR = '\t';

    // 生成lua文件上方字段描述的配置
    // 每行开头的lua注释声明
    private static string _COMMENT_OUT_STRING = "-- ";
    // 变量名、数据类型、描述声明之间的间隔字符串
    private static string _DEFINE_INDENTATION_STRING = "   ";
    // dict子元素相对于父dict变量名声明的缩进字符串
    private static string _DICT_CHILD_INDENTATION_STRING = "   ";
    // 变量名声明所占的最少字符数
    private static int _FIELD_NAME_MIN_LENGTH = 30;
    // 数据类型声明所占的最少字符数
    private static int _FIELD_DATA_TYPE_MIN_LENGTH = 30;

    public static bool ExportTableToLua(TableInfo tableInfo, out string errorString)
    {
        StringBuilder content = new StringBuilder();

        // 生成数据内容开头
        content.AppendLine("return {");

        // 当前缩进量
        int currentLevel = 1;

        // 逐行读取表格内容生成lua table
        for (int row = 0; row < tableInfo.GetKeyColumnFieldInfo().Data.Count; ++row)
        {
            List<FieldInfo> allField = tableInfo.GetAllFieldInfo();

            // 将主键列作为key生成
            content.Append(_GetLuaTableIndentation(currentLevel));
            FieldInfo keyColumnField = allField[0];
            if (keyColumnField.DataType == DataType.Int)
                content.AppendFormat("[{0}]", keyColumnField.Data[row]);
            else if (keyColumnField.DataType == DataType.String)
                content.Append(keyColumnField.Data[row]);

            content.AppendLine(" = {");
            ++currentLevel;

            // 将其他列依次作为value生成
            for (int column = 1; column < allField.Count; ++column)
            {
                string oneFieldString = _GetOneField(allField[column], row, currentLevel, out errorString);
                if (errorString != null)
                {
                    errorString = string.Format("导出表格{0}失败，", tableInfo.TableName) + errorString;
                    return false;
                }
                else
                    content.Append(oneFieldString);
            }

            // 一行数据生成完毕后添加右括号结尾等
            --currentLevel;
            content.Append(_GetLuaTableIndentation(currentLevel));
            content.AppendLine("},");
        }

        // 生成数据内容结尾
        content.AppendLine("}");

        string exportString = content.ToString();
        if (AppValues.IsNeedColumnInfo == true)
            exportString = _GetColumnInfo(tableInfo) + exportString;

        // 保存为lua文件
        Utils.SaveLuaFile(tableInfo.TableName, exportString);

        errorString = null;
        return true;
    }

    /// <summary>
    /// 生成要在lua文件最上方以注释形式展示的列信息
    /// </summary>
    private static string _GetColumnInfo(TableInfo tableInfo)
    {
        // 变量名前的缩进量
        int level = 0;

        StringBuilder content = new StringBuilder();
        foreach (FieldInfo fieldInfo in tableInfo.GetAllFieldInfo())
            content.Append(_GetOneFieldColumnInfo(fieldInfo, level));

        content.Append(System.Environment.NewLine);
        return content.ToString();
    }

    private static string _GetOneFieldColumnInfo(FieldInfo fieldInfo, int level)
    {
        StringBuilder content = new StringBuilder();
        content.AppendFormat("{0}{1, -" + _FIELD_NAME_MIN_LENGTH + "}{2}{3, -" + _FIELD_DATA_TYPE_MIN_LENGTH + "}{4}{5}\n", _COMMENT_OUT_STRING, _GetFieldNameIndentation(level) + fieldInfo.FieldName, _DEFINE_INDENTATION_STRING, fieldInfo.DataTypeString, _DEFINE_INDENTATION_STRING, fieldInfo.Desc);
        if (fieldInfo.DataType == DataType.Dict || fieldInfo.DataType == DataType.Array)
        {
            ++level;
            foreach (FieldInfo childFieldInfo in fieldInfo.ChildField)
                content.Append(_GetOneFieldColumnInfo(childFieldInfo, level));

            --level;
        }

        return content.ToString();
    }

    /// <summary>
    /// 生成columnInfo变量名前的缩进字符串
    /// </summary>
    private static string _GetFieldNameIndentation(int level)
    {
        string indentationString = string.Empty;
        for (int i = 0; i < level; ++i)
            indentationString += _DICT_CHILD_INDENTATION_STRING;

        return indentationString;
    }

    private static string _GetOneField(FieldInfo fieldInfo, int row, int level, out string errorString)
    {
        StringBuilder content = new StringBuilder();
        errorString = null;

        // 变量名前的缩进
        content.Append(_GetLuaTableIndentation(level));
        // 变量名
        content.Append(fieldInfo.FieldName);
        content.Append(" = ");
        // 对应数据值
        string value = null;
        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Float:
            case DataType.String:
            case DataType.Lang:
            case DataType.Bool:
                {
                    value = _GetBaseValue(fieldInfo, row, level);
                    break;
                }
            case DataType.TableString:
                {
                    value = _GetTableStringValue(fieldInfo, row, level, out errorString);
                    break;
                }
            case DataType.Dict:
            case DataType.Array:
                {
                    value = _GetSetValue(fieldInfo, row, level);
                    break;
                }
        }

        if (errorString != null)
        {
            errorString = string.Format("第{0}行第{1}列的数据存在错误无法导出，", row + AppValues.FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1)) + errorString;
            return null;
        }

        content.Append(value);
        // 一个字段结尾加逗号并换行
        content.AppendLine(",");

        return content.ToString();
    }

    private static string _GetBaseValue(FieldInfo fieldInfo, int row, int level)
    {
        StringBuilder content = new StringBuilder();

        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Float:
                {
                    content.Append(fieldInfo.Data[row]);
                    break;
                }
            case DataType.String:
                {
                    content.Append("\"");
                    // 将单元格中填写的英文引号进行转义，使得单元格中填写123"456时，最终生成的lua文件中为xx = "123\"456"
                    content.Append(fieldInfo.Data[row].ToString().Replace("\"", "\\\""));
                    content.Append("\"");
                    break;
                }
            case DataType.Lang:
                {
                    if (fieldInfo.Data[row] != null)
                    {
                        content.Append("\"");
                        content.Append(fieldInfo.Data[row].ToString().Replace("\"", "\\\""));
                        content.Append("\"");
                    }
                    else
                    {
                        if (AppValues.IsPrintEmptyStringWhenLangNotMatching == true)
                            content.Append("\"\"");
                        else
                            content.Append("nil");
                    }

                    break;
                }
            case DataType.Bool:
                {
                    if ((bool)fieldInfo.Data[row] == true)
                        content.Append("true");
                    else
                        content.Append("false");

                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit("错误：用_WriteBaseValue函数解析了非基础类型的数据");
                    break;
                }
        }

        return content.ToString();
    }

    private static string _GetSetValue(FieldInfo fieldInfo, int row, int level)
    {
        StringBuilder content = new StringBuilder();
        string errorString = null;

        // 如果该dict或array数据用-1标为无效，则赋值为nil
        if ((bool)fieldInfo.Data[row] == false)
            content.Append("nil");
        else
        {
            // 包裹dict或array所生成table的左括号
            content.AppendLine("{");
            ++level;
            // 逐个对子元素进行生成
            foreach (FieldInfo childField in fieldInfo.ChildField)
            {
                // 因为只有tableString型数据在导出时才有可能出现错误，而dict或array子元素不可能为tableString型，故这里不会出错
                string oneFieldString = _GetOneField(childField, row, level, out errorString);
                content.Append(oneFieldString);
            }
            // 包裹dict或array所生成table的右括号
            --level;
            content.Append(_GetLuaTableIndentation(level));
            content.Append("}");
        }

        return content.ToString();
    }

    private static string _GetTableStringValue(FieldInfo fieldInfo, int row, int level, out string errorString)
    {
        StringBuilder content = new StringBuilder();
        errorString = null;

        string inputData = fieldInfo.Data[row].ToString();

        // tableString字符串中不允许出现英文引号、斜杠
        if (inputData.Contains("\"") || inputData.Contains("\\") || inputData.Contains("/"))
        {
            errorString = "tableString字符串中不允许出现英文引号、斜杠";
            return null;
        }

        // 包裹tableString所生成table的左括号
        content.AppendLine("{");
        ++level;

        // 每组数据间用英文分号分隔，最终每组数据会生成一个lua table
        string[] allDataString = inputData.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        // 记录每组数据中的key值（转为字符串后的），不允许出现相同的key（key：每组数据中的key值， value：第几组数据，从0开始记）
        Dictionary<string, int> stringKeys = new Dictionary<string, int>();
        for (int i = 0; i < allDataString.Length; ++i)
        {
            content.Append(_GetLuaTableIndentation(level));

            // 根据key的格式定义生成key
            switch (fieldInfo.TableStringFormatDefine.KeyDefine.KeyType)
            {
                case TABLE_STRING_KEY_TYPE.SEQ:
                    {
                        content.AppendFormat("[{0}]", i + 1);
                        break;
                    }
                case TABLE_STRING_KEY_TYPE.DATA_IN_INDEX:
                    {
                        string value = _GetDataInIndexType(fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine, allDataString[i], out errorString);
                        if (errorString == null)
                        {
                            if (fieldInfo.TableStringFormatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.Int)
                            {
                                // 检查key是否在该组数据中重复
                                if (stringKeys.ContainsKey(value))
                                    errorString = string.Format("第{0}组数据与第{1}组数据均为相同的key（{2}）", stringKeys[value] + 1, i + 1, value);
                                else
                                {
                                    stringKeys.Add(value, i);
                                    content.AppendFormat("[{0}]", value);
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
                                        content.Append(value);
                                    }
                                }
                                else
                                    errorString = "string型的key不符合变量名定义规范，" + errorString;
                            }
                            else
                            {
                                Utils.LogErrorAndExit("错误：用_WriteTableStringValue函数导出非int或string型的key值");
                                return null;
                            }
                        }

                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit("错误：用_WriteTableStringValue函数导出未知类型的key");
                        return null;
                    }
            }
            if (errorString != null)
            {
                errorString = string.Format("tableString中第{0}组数据（{1}）的key数据存在错误，", i + 1, allDataString[i]) + errorString;
                return null;
            }

            content.Append(" = ");

            // 根据value的格式定义生成value
            switch (fieldInfo.TableStringFormatDefine.ValueDefine.ValueType)
            {
                case TABLE_STRING_VALUE_TYPE.TRUE:
                    {
                        content.Append("true");
                        break;
                    }
                case TABLE_STRING_VALUE_TYPE.DATA_IN_INDEX:
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
                case TABLE_STRING_VALUE_TYPE.TABLE:
                    {
                        content.AppendLine("{");
                        ++level;

                        // 依次输出table中定义的子元素
                        foreach (TableElementDefine elementDefine in fieldInfo.TableStringFormatDefine.ValueDefine.TableValueDefineList)
                        {
                            content.Append(_GetLuaTableIndentation(level));
                            content.Append(elementDefine.KeyName);
                            content.Append(" = ");
                            string value = _GetDataInIndexType(elementDefine.DataInIndexDefine, allDataString[i], out errorString);
                            if (errorString == null)
                            {
                                if (elementDefine.DataInIndexDefine.DataType == DataType.String || elementDefine.DataInIndexDefine.DataType == DataType.Lang)
                                    content.AppendFormat("\"{0}\"", value);
                                else
                                    content.Append(value);
                            }
                            content.AppendLine(",");
                        }
                        --level;
                        content.Append(_GetLuaTableIndentation(level));
                        content.Append("}");

                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit("错误：用_WriteTableStringValue函数导出未知类型的value");
                        return null;
                    }
            }
            if (errorString != null)
            {
                errorString = string.Format("tableString中第{0}组数据（{1}）的value数据存在错误，", i + 1, allDataString[i]) + errorString;
                return null;
            }

            // 每组数据生成完毕后加逗号并换行
            content.AppendLine(",");
        }

        // 包裹tableString所生成table的右括号
        --level;
        content.Append(_GetLuaTableIndentation(level));
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
                    if ("1".Equals(inputData))
                        result = "true";
                    else if ("0".Equals(inputData))
                        result = "false";
                    else
                        errorString = string.Format("输入的\"{0}\"不是合法的bool值，正确填写bool值方式为填1代表true，0代表false", inputData);

                    break;
                }
            case DataType.Int:
                {
                    int intValue;
                    bool isValid = int.TryParse(inputData, out intValue);
                    if (isValid)
                        result = intValue.ToString();
                    else
                        errorString = string.Format("输入的\"{0}\"不是合法的int类型的值", inputData);

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
                    Utils.LogErrorAndExit("错误：用_GetDataInTableString函数解析了tableString中不支持的数据类型");
                    break;
                }
        }

        return result;
    }

    private static string _GetLuaTableIndentation(int level)
    {
        return new string(_LUA_TABLE_INDENTATION_CHAR, level);
    }
}
