using System;
using System.Collections.Generic;
using System.Text;

public class TableAnalyzeHelper
{
    /// <summary>
    /// 将填写的数据类型字符串解析为DataType的枚举
    /// </summary>
    private static DataType _AnalyzeDataType(string inputTypeString)
    {
        if (string.IsNullOrEmpty(inputTypeString))
            return DataType.Invalid;

        string typeString = inputTypeString.Trim();

        if (typeString.StartsWith("int", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Int;
        else if (typeString.StartsWith("float", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Float;
        else if (typeString.StartsWith("string", StringComparison.CurrentCultureIgnoreCase))
            return DataType.String;
        else if (typeString.StartsWith("lang", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Lang;
        else if (typeString.StartsWith("bool", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Bool;
        else if (typeString.StartsWith("tableString", StringComparison.CurrentCultureIgnoreCase))
            return DataType.TableString;
        else if (typeString.StartsWith("array", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Array;
        else if (typeString.StartsWith("dict", StringComparison.CurrentCultureIgnoreCase))
            return DataType.Dict;
        else
            return DataType.Invalid;
    }

    public static TableStringFormatDefine GetTableStringFormatDefine(string dataTypeString, out string errorString)
    {
        TableStringFormatDefine formatDefine = new TableStringFormatDefine();

        // 必须在tableString[]中声明格式
        string defineStartString = "tableString[";
        if (!(dataTypeString.StartsWith(defineStartString, StringComparison.CurrentCultureIgnoreCase) && dataTypeString.EndsWith("]")))
        {
            errorString = "必须在tableString[]中声明，即以\"tableString[\"开头，以\"]\"结尾";
            return formatDefine;
        }
        // 去掉外面的tableString[]，取得中间定义内容
        int startIndex = defineStartString.Length;
        string formatString = dataTypeString.Substring(startIndex, dataTypeString.Length - startIndex - 1).Trim();
        // 通过|分离key和value的声明
        string[] keyAndValueFormatString = formatString.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyAndValueFormatString.Length != 2)
        {
            errorString = "必须用|分隔key和value";
            return formatDefine;
        }
        // 解析key声明
        string keyFormatString = keyAndValueFormatString[0].Trim();
        string keyStartString = "k:";
        if (!keyFormatString.StartsWith(keyStartString, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = "key的声明必须以k:开头";
            return formatDefine;
        }
        else
        {
            // 去除开头的k:
            keyFormatString = keyFormatString.Substring(keyStartString.Length).Trim();

            // 按数据顺序自动编号
            if (keyFormatString.Equals("#seq", StringComparison.CurrentCultureIgnoreCase))
            {
                formatDefine.KeyDefine.KeyType = TABLE_STRING_KEY_TYPE.SEQ;
            }
            // 以数据组中指定索引位置的数据为key
            else if (keyFormatString.StartsWith("#"))
            {
                formatDefine.KeyDefine.KeyType = TABLE_STRING_KEY_TYPE.DATA_IN_INDEX;
                formatDefine.KeyDefine.DataInIndexDefine = _GetDataInIndexDefine(keyFormatString, out errorString);
                if (errorString != null)
                {
                    errorString = "key的声明未符合形如#1(int)\n" + errorString;
                    return formatDefine;
                }
                // 只有int型或string型数据才能作为key
                if (!(formatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.Int || formatDefine.KeyDefine.DataInIndexDefine.DataType == DataType.String))
                {
                    errorString = string.Format("key只允许为int或string型，你定义的类型为{0}\n", formatDefine.KeyDefine.DataInIndexDefine.DataType.ToString());
                    return formatDefine;
                }
            }
            else
            {
                errorString = "key声明非法";
                return formatDefine;
            }
        }

        // 解析value声明
        string valueFormatString = keyAndValueFormatString[1].Trim();
        string valueStartString = "v:";
        if (!valueFormatString.StartsWith(valueStartString, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = "value的声明必须以v:开头";
            return formatDefine;
        }
        else
        {
            // 去除开头的v:
            valueFormatString = valueFormatString.Substring(valueStartString.Length).Trim();

            // value始终为true
            if (valueFormatString.Equals("#true", StringComparison.CurrentCultureIgnoreCase))
            {
                formatDefine.ValueDefine.ValueType = TABLE_STRING_VALUE_TYPE.TRUE;
            }
            // value为table类型
            else if (valueFormatString.StartsWith("#table", StringComparison.CurrentCultureIgnoreCase))
            {
                formatDefine.ValueDefine.ValueType = TABLE_STRING_VALUE_TYPE.TABLE;
                // 判断是否形如#table(xxx)
                int leftBracketIndex = valueFormatString.IndexOf('(');
                int rightBracketIndex = valueFormatString.LastIndexOf(')');
                if (leftBracketIndex == -1 || rightBracketIndex != valueFormatString.Length - 1)
                {
                    errorString = "table类型value格式声明错误，必须形如#table(xxx)";
                    return formatDefine;
                }
                else
                {
                    // 去掉#table(xxx)外面，只保留括号中的内容
                    string tableDefineString = valueFormatString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                    // table中每个键值对的声明用英文逗号隔开
                    string[] tableElementDefine = tableDefineString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    // 解析每个键值对的定义，形如type=#1(int)
                    formatDefine.ValueDefine.TableValueDefineList = new List<TableElementDefine>();
                    // 记录每个键值对的key，不允许重复（key：key名， value：第几组键值对，从0开始记）
                    Dictionary<string, int> tableKeys = new Dictionary<string, int>();
                    for (int i = 0; i < tableElementDefine.Length; ++i)
                    {
                        TableElementDefine oneTableElementDefine = _GetTablelementDefine(tableElementDefine[i].Trim(), out errorString);
                        if (errorString != null)
                        {
                            errorString = string.Format("table类型值声明错误，无法解析{0}，", tableElementDefine[i].Trim()) + errorString;
                            return formatDefine;
                        }
                        else
                        {
                            // 检查定义的key是否重复
                            if (tableKeys.ContainsKey(oneTableElementDefine.KeyName))
                            {
                                errorString = string.Format("table类型的第{0}个与第{1}个子元素均为相同的key（{2}）", tableKeys[oneTableElementDefine.KeyName] + 1, i + 1, oneTableElementDefine.KeyName);
                                return formatDefine;
                            }
                            else
                            {
                                tableKeys.Add(oneTableElementDefine.KeyName, i + 1);
                                formatDefine.ValueDefine.TableValueDefineList.Add(oneTableElementDefine);
                            }
                        }
                    }
                }
            }
            // 以数据组中指定索引位置的数据为value
            else if (valueFormatString.StartsWith("#"))
            {
                formatDefine.ValueDefine.ValueType = TABLE_STRING_VALUE_TYPE.DATA_IN_INDEX;
                formatDefine.ValueDefine.DataInIndexDefine = _GetDataInIndexDefine(valueFormatString, out errorString);
                if (errorString != null)
                {
                    errorString = "value的声明未符合形如#1(int)\n" + errorString;
                    return formatDefine;
                }
            }
            else
            {
                errorString = "value声明非法";
                return formatDefine;
            }
        }

        errorString = null;
        return formatDefine;
    }

    /// <summary>
    /// 将形如type=#1(int)的格式定义字符串转为TableElementDefine定义
    /// </summary>
    private static TableElementDefine _GetTablelementDefine(string tableElementDefine, out string errorString)
    {
        TableElementDefine elementDefine = new TableElementDefine();

        // 判断是否用=分隔key与value定义
        string[] keyAndValueString = tableElementDefine.Trim().Split(new char[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyAndValueString.Length != 2)
        {
            errorString = "必须用=分隔键值对";
            return elementDefine;
        }
        else
        {
            // 取得并检查key名
            elementDefine.KeyName = keyAndValueString[0].Trim();
            TableCheckHelper.CheckFieldName(elementDefine.KeyName, out errorString);
            if (errorString != null)
            {
                errorString = "键值对中键名非法，" + errorString;
                return elementDefine;
            }
            // 解析value的定义
            elementDefine.DataInIndexDefine = _GetDataInIndexDefine(keyAndValueString[1].Trim(), out errorString);
            if (errorString != null)
            {
                errorString = "键值对中value的声明未符合形如#1(int)\n" + errorString;
                return elementDefine;
            }
        }

        errorString = null;
        return elementDefine;
    }

    /// <summary>
    /// 将形如#1(int)的格式定义字符串转为DataInIndexDefine定义
    /// </summary>
    private static DataInIndexDefine _GetDataInIndexDefine(string defineString, out string errorString)
    {
        DataInIndexDefine dataInIndexDefine = new DataInIndexDefine();

        // 检查#后是否跟合法数字，且数字后面用括号注明类型
        defineString = defineString.Substring(1).Trim();
        // 检查类型是否合法
        int leftBracketIndex = defineString.IndexOf('(');
        int rightBracketIndex = defineString.LastIndexOf(')');
        if (leftBracketIndex == -1 || rightBracketIndex != defineString.Length - 1)
        {
            errorString = "未注明格式类型，需要形如：#1(int)";
            return dataInIndexDefine;
        }
        else
        {
            string dataTypeString = defineString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
            dataInIndexDefine.DataType = _AnalyzeDataType(dataTypeString);
            if (!(dataInIndexDefine.DataType == DataType.Int || dataInIndexDefine.DataType == DataType.Float || dataInIndexDefine.DataType == DataType.Bool || dataInIndexDefine.DataType == DataType.String || dataInIndexDefine.DataType == DataType.Lang))
            {
                errorString = "格式类型非法，只支持int、float、bool、string、lang这几种类型";
                return dataInIndexDefine;
            }
        }
        // 检查数据索引值是否合法
        string dataIndexString = defineString.Substring(0, leftBracketIndex).Trim();
        int dataIndex;
        if (int.TryParse(dataIndexString, out dataIndex))
        {
            if (dataIndex > 0)
                dataInIndexDefine.DataIndex = dataIndex;
            else
            {
                errorString = "数据索引值编号最小要从1开始";
                return dataInIndexDefine;
            }
        }
        else
        {
            errorString = string.Format("数据索引值不是合法数字，你的输入值为:{0}", dataIndexString);
            return dataInIndexDefine;
        }

        errorString = null;
        return dataInIndexDefine;
    }
}
