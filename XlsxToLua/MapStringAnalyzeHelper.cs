using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

public class MapStringAnalyzeHelper
{
    /// <summary>
    /// 解析mapString类型的定义字符串，并以MapStringInfo形式返回
    /// </summary>
    public static MapStringInfo GetMapStringFormatDefine(string defineString, out string errorString)
    {
        // 对定义字符串进行词法解析，得到解析后的token列表
        List<MapStringToken> tokenList = _AnalyzeMapStringDefineToken(defineString, out errorString);
        if (errorString != null)
        {
            errorString = string.Concat("mapString类型的格式定义错误：", errorString);
            return null;
        }

        if (tokenList.Count == 0)
        {
            errorString = "mapString类型定义中未含有任何元素声明";
            return null;
        }

        // 因为整个定义相当于一个map，故将上面得到的token列表首尾加上map开始和结束标记
        tokenList.Insert(0, new MapStringToken(MapStringTokenType.StartMap, "("));
        tokenList.Add(new MapStringToken(MapStringTokenType.EndMap, ")"));

        MapStringDefineParser parser = new MapStringDefineParser();
        MapStringInfo mapStringInfo = parser.GetMapStringDefine(tokenList, out errorString);
        if (errorString == null)
            return mapStringInfo;
        else
        {
            errorString = string.Concat("mapString类型的格式定义错误：", errorString);
            return null;
        }
    }

    /// <summary>
    /// 解析mapString类型的数据字符串，并转为JsonData形式
    /// </summary>
    public static JsonData GetMapStringData(string dataString, MapStringInfo formatDefine, out string errorString)
    {
        // 对数据字符串进行词法解析，得到解析后的token列表
        List<MapStringToken> tokenList = _AnalyzeMapStringDataToken(dataString, out errorString);
        if (errorString != null)
        {
            errorString = string.Concat("mapString类型的数据定义错误：", errorString);
            return null;
        }

        // 因为整个数据相当于一个map，故将上面得到的token列表首尾加上map开始和结束标记
        tokenList.Insert(0, new MapStringToken(MapStringTokenType.StartMap, "("));
        tokenList.Add(new MapStringToken(MapStringTokenType.EndMap, ")"));

        MapStringDataParser parser = new MapStringDataParser();
        JsonData jsonData = parser.GetMapStringData(tokenList, formatDefine, out errorString);

        if (errorString == null)
            return jsonData;
        else
        {
            errorString = string.Concat("mapString类型的数据错误：", errorString);
            return null;
        }
    }

    /// <summary>
    /// 对mapString类型的定义字符串进行词法解析
    /// </summary>
    private static List<MapStringToken> _AnalyzeMapStringDefineToken(string defineString, out string errorString)
    {
        // 检查括号是否匹配
        int leftBracketCount = 0;
        for (int i = 0; i < defineString.Length; ++i)
        {
            char c = defineString[i];
            if (c == '(')
                ++leftBracketCount;
            else if (c == ')')
                --leftBracketCount;

            if (leftBracketCount < 0)
            {
                errorString = string.Format("格式定义字符串中括号不匹配，第{0}个字符处的\")\"没有前面与之对应的\"(\"", i + 1);
                return null;
            }
        }
        if (leftBracketCount > 0)
        {
            errorString = string.Format("格式定义字符串中括号不匹配，有{0}个\"(\"没有与之对应的\")\"", leftBracketCount);
            return null;
        }

        // 必须在mapString[]中声明格式
        const string DEFINE_START_STRING = "mapString[";
        if (!(defineString.StartsWith(DEFINE_START_STRING, StringComparison.CurrentCultureIgnoreCase) && defineString.EndsWith("]")))
        {
            errorString = "格式定义必须在mapString[]中声明，即以\"mapString[\"开头，以\"]\"结尾";
            return null;
        }
        // 去掉外面的mapString[]，取得中间定义内容
        int startIndex = DEFINE_START_STRING.Length;
        string formatString = defineString.Substring(startIndex, defineString.Length - startIndex - 1).Trim();

        List<MapStringToken> tokenList = new List<MapStringToken>();
        int nextCharIndex = 0;
        while (nextCharIndex < formatString.Length)
        {
            char c = formatString[nextCharIndex];
            if (c == ' ')
                ++nextCharIndex;
            else if (c == ',')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.Comma, ","));
                ++nextCharIndex;
            }
            else if (c == '=')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.EqualSign, "="));
                ++nextCharIndex;
            }
            else if (c == '(')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.StartMap, "("));
                ++nextCharIndex;
            }
            else if (c == ')')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.EndMap, ")"));
                ++nextCharIndex;
            }
            else if (char.IsLetterOrDigit(c) || c == '_')
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                ++nextCharIndex;
                while (nextCharIndex < formatString.Length)
                {
                    char temp = formatString[nextCharIndex];
                    if (char.IsLetterOrDigit(temp) || c == '_' || c == ' ')
                    {
                        stringBuilder.Append(temp);
                        ++nextCharIndex;
                    }
                    else
                    {
                        tokenList.Add(new MapStringToken(MapStringTokenType.String, stringBuilder.ToString()));
                        break;
                    }
                }
                if (nextCharIndex == formatString.Length)
                    tokenList.Add(new MapStringToken(MapStringTokenType.String, stringBuilder.ToString()));
            }
            else
            {
                errorString = string.Format("格式定义字符串中的第{0}个字符（{1}）非法", nextCharIndex + 1, c);
                return null;
            }
        }

        errorString = null;
        return tokenList;
    }

    /// <summary>
    /// 对mapString类型的数据字符串进行词法解析
    /// </summary>
    private static List<MapStringToken> _AnalyzeMapStringDataToken(string dataString, out string errorString)
    {
        // 检查括号、引号是否匹配
        int leftBracketCount = 0;
        bool hasLeftQuotationMark = false;
        for (int i = 0; i < dataString.Length; ++i)
        {
            char c = dataString[i];
            if (c == '(')
                ++leftBracketCount;
            else if (c == ')')
                --leftBracketCount;
            else if (c == '"' && i > 0 && dataString[i - 1] != '\\')
                hasLeftQuotationMark = !hasLeftQuotationMark;

            if (leftBracketCount < 0)
            {
                errorString = string.Format("数据字符串中括号不匹配，第{0}个字符处的\")\"没有前面与之对应的\"(\"", i + 1);
                return null;
            }
        }
        if (leftBracketCount > 0)
        {
            errorString = string.Format("数据字符串中括号不匹配，有{0}个\"(\"没有与之对应的\")\"", leftBracketCount);
            return null;
        }
        if (hasLeftQuotationMark == true)
        {
            errorString = "数据字符串中引号不匹配";
            return null;
        }

        List<MapStringToken> tokenList = new List<MapStringToken>();
        int nextCharIndex = 0;
        while (nextCharIndex < dataString.Length)
        {
            char c = dataString[nextCharIndex];
            if (c == ' ')
                ++nextCharIndex;
            else if (c == ',')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.Comma, ","));
                ++nextCharIndex;
            }
            else if (c == '=')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.EqualSign, "="));
                ++nextCharIndex;
            }
            else if (c == '(')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.StartMap, "("));
                ++nextCharIndex;
            }
            else if (c == ')')
            {
                tokenList.Add(new MapStringToken(MapStringTokenType.EndMap, ")"));
                ++nextCharIndex;
            }
            else if (c == '"')
            {
                StringBuilder stringBuilder = new StringBuilder();
                ++nextCharIndex;
                while (nextCharIndex < dataString.Length)
                {
                    char temp = dataString[nextCharIndex];
                    if (temp == '"' && nextCharIndex > 0 && dataString[nextCharIndex - 1] != '\\')
                    {
                        tokenList.Add(new MapStringToken(MapStringTokenType.StringWithQuotationMark, stringBuilder.ToString()));
                        ++nextCharIndex;
                        break;
                    }
                    else
                    {
                        stringBuilder.Append(temp);
                        ++nextCharIndex;
                    }
                }
            }
            else if (char.IsLetter(c) || c == '_')
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                ++nextCharIndex;
                while (nextCharIndex < dataString.Length)
                {
                    char temp = dataString[nextCharIndex];
                    if (char.IsLetterOrDigit(temp) || c == '_')
                    {
                        stringBuilder.Append(temp);
                        ++nextCharIndex;
                    }
                    else
                    {
                        tokenList.Add(new MapStringToken(MapStringTokenType.String, stringBuilder.ToString()));
                        break;
                    }
                }
                if (nextCharIndex == dataString.Length)
                    tokenList.Add(new MapStringToken(MapStringTokenType.String, stringBuilder.ToString()));
            }
            else if (char.IsNumber(c) || c == '-')
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                ++nextCharIndex;
                while (nextCharIndex < dataString.Length)
                {
                    char temp = dataString[nextCharIndex];
                    if (char.IsNumber(temp) || temp == '.')
                    {
                        stringBuilder.Append(temp);
                        ++nextCharIndex;
                    }
                    else
                    {
                        tokenList.Add(new MapStringToken(MapStringTokenType.Number, stringBuilder.ToString()));
                        break;
                    }
                }
                if (nextCharIndex == dataString.Length)
                    tokenList.Add(new MapStringToken(MapStringTokenType.Number, stringBuilder.ToString()));
            }
            else
            {
                errorString = string.Format("格式定义字符串中的第{0}个字符（{1}）非法", nextCharIndex + 1, c);
                return null;
            }
        }

        errorString = null;
        return tokenList;
    }
}

/// <summary>
/// 用于解析mapString类型的定义字符串
/// </summary>
public class MapStringDefineParser
{
    private Queue<MapStringToken> _tokenQueue = null;

    public MapStringInfo GetMapStringDefine(List<MapStringToken> tokenList, out string errorString)
    {
        _tokenQueue = new Queue<MapStringToken>(tokenList);

        MapStringInfo mapStringInfo = _GetMap(out errorString);
        if (errorString == null)
            return mapStringInfo;
        else
            return null;
    }

    private MapStringInfo _GetMap(out string errorString)
    {
        MapStringInfo mapStringInfo = new MapStringInfo();

        // 跳过map结构开始的{
        _tokenQueue.Dequeue();

        _GetMapElement(mapStringInfo, out errorString);
        if (errorString == null)
            return mapStringInfo;
        else
        {
            errorString = string.Concat("解析map中的子元素错误：", errorString);
            return null;
        }
    }

    private void _GetMapElement(MapStringInfo mapStringInfo, out string errorString)
    {
        // 解析key
        if (_tokenQueue.Count == 0)
        {
            errorString = "mapString定义不完整";
            return;
        }
        MapStringToken keyToken = _tokenQueue.Dequeue();
        if (keyToken.TokenType != MapStringTokenType.String)
        {
            errorString = string.Concat("map子元素中的key名非法，输入值为", keyToken.DefineString);
            return;
        }
        string key = keyToken.DefineString.Trim();
        // 检查输入的key名是否符合变量名要求
        if (TableCheckHelper.CheckFieldName(key, out errorString) == false)
        {
            errorString = string.Concat("map子元素中的key名不符合变量名要求，", errorString);
            return;
        }
        // 检查是否已存在此变量名
        if (mapStringInfo.ParamInfo.ContainsKey(key))
        {
            errorString = string.Format("map子元素中不允许存在相同key名（{0}）", key);
            return;
        }

        // 判断key名后是否为等号
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString定义不完整，map子元素中的key名（{0}）后缺失等号", key);
            return;
        }
        MapStringToken equalSignToken = _tokenQueue.Dequeue();
        if (equalSignToken.TokenType != MapStringTokenType.EqualSign)
        {
            errorString = string.Format("map子元素中的key名（{0}）后应为等号，而输入值为{1}", key, equalSignToken.DefineString);
            return;
        }

        // 解析value值
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString定义不完整，map子元素中的key名（{0}）在等号后未声明对应的数据类型", key);
            return;
        }
        MapStringToken valueToken = _tokenQueue.Dequeue();
        DataType valueDataType = DataType.Invalid;
        if (valueToken.TokenType == MapStringTokenType.String)
        {
            string dataTypeString = valueToken.DefineString.Trim();
            if ("int".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.Int;
            else if ("long".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.Long;
            else if ("float".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.Float;
            else if ("string".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.String;
            else if ("lang".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.Lang;
            else if ("bool".Equals(dataTypeString, StringComparison.CurrentCultureIgnoreCase))
                valueDataType = DataType.Bool;
            else
            {
                errorString = string.Format("map子元素中的key名（{0}）对应的数据类型（{1}）非法，只支持int、long、float、string、lang、bool这几种最基础类型", key, dataTypeString);
                return;
            }
            MapStringParamInfo paramInfo = new MapStringParamInfo();
            paramInfo.ParamName = key;
            paramInfo.DataType = valueDataType;
            mapStringInfo.ParamInfo.Add(key, paramInfo);
        }
        else if (valueToken.TokenType == MapStringTokenType.StartMap)
        {
            MapStringParamInfo paramInfo = new MapStringParamInfo();
            paramInfo.ParamName = key;
            paramInfo.DataType = DataType.MapString;
            paramInfo.MapStringInfo = new MapStringInfo();
            _GetMapElement(paramInfo.MapStringInfo, out errorString);
            if (errorString != null)
            {
                errorString = string.Format("名为{0}的map下属的子元素错误：{1}", key, errorString);
                return;
            }
            mapStringInfo.ParamInfo.Add(key, paramInfo);
        }
        else
        {
            errorString = string.Format("与变量名（{0}）对应的数据类型声明非法，输入值为{1}", key, valueToken.DefineString);
            return;
        }

        // 解析后面的token
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString定义不完整，map下的子元素{0}之后未声明用英文逗号分隔下一个子元素，也没有声明map的结束", key);
            return;
        }
        MapStringToken nextToken = _tokenQueue.Dequeue();
        if (nextToken.TokenType == MapStringTokenType.EndMap)
            return;
        else if (nextToken.TokenType == MapStringTokenType.Comma)
        {
            if (_tokenQueue.Count == 0)
            {
                errorString = string.Format("mapString定义不完整，map下的子元素{0}之后的英文逗号后未声明下一个元素", key);
                return;
            }
            _GetMapElement(mapStringInfo, out errorString);
            if (errorString != null)
            {
                errorString = string.Format("名为{0}的子元素后面的元素声明错误：{1}", key, errorString);
                return;
            }
        }
        else
        {
            errorString = string.Format("map下的子元素{0}之后未声明用英文逗号分隔下一个子元素，也没有声明map的结束，输入值为{1}", key, nextToken.DefineString);
            return;
        }
    }
}

/// <summary>
/// 用于解析mapString类型的数据字符串
/// </summary>
public class MapStringDataParser
{
    private Queue<MapStringToken> _tokenQueue = null;
    private MapStringInfo _formatDefine = null;

    public JsonData GetMapStringData(List<MapStringToken> tokenList, MapStringInfo formatDefine, out string errorString)
    {
        _tokenQueue = new Queue<MapStringToken>(tokenList);
        _formatDefine = formatDefine;

        JsonData jsonData = _GetMap(out errorString);
        if (errorString == null)
            return jsonData;
        else
            return null;
    }

    private JsonData _GetMap(out string errorString)
    {
        JsonData jsonData = new JsonData();

        // 跳过map结构开始的{
        _tokenQueue.Dequeue();

        _GetMapElement(jsonData, _formatDefine, out errorString);
        if (errorString == null)
            return jsonData;
        else
        {
            errorString = string.Concat("解析map中的子元素错误：", errorString);
            return null;
        }
    }

    private void _GetMapElement(JsonData jsonData, MapStringInfo formatDefine, out string errorString)
    {
        // 解析key
        if (_tokenQueue.Count == 0)
        {
            errorString = "mapString数据不完整";
            return;
        }
        MapStringToken keyToken = _tokenQueue.Dequeue();
        if (keyToken.TokenType != MapStringTokenType.String && keyToken.TokenType != MapStringTokenType.StringWithQuotationMark)
        {
            errorString = string.Concat("map子元素中的key名非法，输入值为", keyToken.DefineString);
            return;
        }
        string key = keyToken.DefineString.Trim();
        // 检查该变量名是否定义
        if (!formatDefine.ParamInfo.ContainsKey(key))
        {
            errorString = string.Format("mapString定义中不存在名为{0}的key", key);
            return;
        }
        // 检查是否已存在此变量名
        MapStringParamInfo paramInfo = formatDefine.ParamInfo[key];
        if (jsonData.Keys.Contains(key))
        {
            errorString = string.Format("mapString数据中存在相同的key名（{0}）", key);
            return;
        }

        // 判断key名后是否为等号
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString数据不完整，map子元素中的key名（{0}）后缺失等号", key);
            return;
        }
        MapStringToken equalSignToken = _tokenQueue.Dequeue();
        if (equalSignToken.TokenType != MapStringTokenType.EqualSign)
        {
            errorString = string.Format("map子元素中的key名（{0}）后应为等号，而输入值为{1}", key, equalSignToken.DefineString);
            return;
        }

        // 解析value值
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString数据不完整，map子元素中的key名（{0}）在等号后未声明对应的数据值", key);
            return;
        }
        MapStringToken valueToken = _tokenQueue.Dequeue();

        if (valueToken.TokenType == MapStringTokenType.StringWithQuotationMark)
        {
            if (paramInfo.DataType == DataType.String)
            {
                // 适应LitJson类库的实现，对换行和引号进行处理
                jsonData[key] = valueToken.DefineString.Replace("\\n", "\n").Replace("\\\"", "\"");
            }
            else if (paramInfo.DataType == DataType.Lang)
            {
                if (AppValues.LangData.ContainsKey(valueToken.DefineString))
                    jsonData[key] = AppValues.LangData[valueToken.DefineString];
                else
                {
                    errorString = string.Format("mapString数据中key名（{0}）对应的lang型数据的key（{1}）在lang文件中找不到对应的value", key, valueToken.DefineString);
                    return;
                }
            }
            else if (paramInfo.DataType == DataType.MapString)
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应mapString类型的数据，而输入的数据值为\"{1}\"，请按格式要求在英文括号内声明mapString类型的数据", key, valueToken.DefineString);
                return;
            }
            else
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而该类型数据不应该用英文引号包裹，只有string和lang型需要", key, paramInfo.DataType);
                return;
            }
        }
        else if (valueToken.TokenType == MapStringTokenType.String)
        {
            if (paramInfo.DataType == DataType.Bool)
            {
                if ("true".Equals(valueToken.DefineString, StringComparison.CurrentCultureIgnoreCase))
                    jsonData[key] = true;
                else if ("false".Equals(valueToken.DefineString, StringComparison.CurrentCultureIgnoreCase))
                    jsonData[key] = false;
                else
                {
                    errorString = string.Format("mapString定义中要求key名（{0}）对应bool类型的数据，而输入值（{1}）非法，请用数字1、0或true、false进行数据定义", key, valueToken.DefineString);
                    return;
                }
            }
            else if (paramInfo.DataType == DataType.String || paramInfo.DataType == DataType.Lang)
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而该类型数据必须用英文引号包裹", key, paramInfo.DataType);
                return;
            }
            else if (paramInfo.DataType == DataType.MapString)
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应mapString类型的数据，而输入的数据值为\"{1}\"，请按格式要求在英文括号内声明mapString类型的数据", key, valueToken.DefineString);
                return;
            }
            else
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而输入的数据值为\"{2}\"", key, paramInfo.DataType, valueToken.DefineString);
                return;
            }
        }
        else if (valueToken.TokenType == MapStringTokenType.Number)
        {
            if (paramInfo.DataType == DataType.Int)
            {
                int number = 0;
                if (int.TryParse(valueToken.DefineString, out number) == false)
                {
                    errorString = string.Format("mapString定义中要求key名（{0}）对应int类型的数据，而输入值（{1}）非法", key, valueToken.DefineString);
                    return;
                }
                jsonData[key] = number;
            }
            else if (paramInfo.DataType == DataType.Long)
            {
                long number = 0;
                if (long.TryParse(valueToken.DefineString, out number) == false)
                {
                    errorString = string.Format("mapString定义中要求key名（{0}）对应long类型的数据，而输入值（{1}）非法", key, valueToken.DefineString);
                    return;
                }
                jsonData[key] = number;
            }
            else if (paramInfo.DataType == DataType.Float)
            {
                double number = 0;
                if (double.TryParse(valueToken.DefineString, out number) == false)
                {
                    errorString = string.Format("mapString定义中要求key名（{0}）对应float类型的数据，而输入值（{1}）非法", key, valueToken.DefineString);
                    return;
                }
                jsonData[key] = number;
            }
            else if (paramInfo.DataType == DataType.Bool)
            {
                if ("1".Equals(valueToken.DefineString))
                    jsonData[key] = true;
                else if ("0".Equals(valueToken.DefineString))
                    jsonData[key] = false;
                else
                {
                    errorString = string.Format("mapString定义中要求key名（{0}）对应bool类型的数据，而输入值（{1}）非法，请用数字1、0或true、false进行数据定义", key, valueToken.DefineString);
                    return;
                }
            }
            else if (paramInfo.DataType == DataType.MapString)
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应mapString类型的数据，而输入的数据值为\"{1}\"，请按格式要求在英文括号内声明mapString类型的数据", key, valueToken.DefineString);
                return;
            }
            else
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而该类型数据必须用英文引号包裹", key, paramInfo.DataType);
                return;
            }
        }
        else if (valueToken.TokenType == MapStringTokenType.StartMap)
        {
            if (paramInfo.DataType == DataType.MapString)
            {
                JsonData childJsonData = new JsonData();
                _GetMapElement(childJsonData, formatDefine.ParamInfo[key].MapStringInfo, out errorString);
                if (errorString != null)
                {
                    errorString = string.Format("名为{0}的map下属的子元素错误：{1}", key, errorString);
                    return;
                }
                jsonData[key] = childJsonData;
            }
            else
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而输入数据为mapString型", key, paramInfo.DataType);
                return;
            }
        }
        else
        {
            errorString = string.Format("与变量名（{0}）对应的数据值声明非法，输入值为{1}", key, valueToken.DefineString);
            return;
        }

        // 解析后面的token
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("mapString数据不完整，map下的子元素{0}之后未声明用英文逗号分隔下一个子元素，也没有声明map的结束", key);
            return;
        }
        MapStringToken nextToken = _tokenQueue.Dequeue();
        if (nextToken.TokenType == MapStringTokenType.EndMap)
        {
            errorString = null;
            return;
        }
        else if (nextToken.TokenType == MapStringTokenType.Comma)
        {
            if (_tokenQueue.Count == 0)
            {
                errorString = string.Format("mapString数据不完整，map下的子元素{0}之后的英文逗号后未声明下一个元素", key);
                return;
            }
            _GetMapElement(jsonData, formatDefine, out errorString);
            if (errorString != null)
            {
                errorString = string.Format("名为{0}的子元素后面的元素声明错误：{1}", key, errorString);
                return;
            }
        }
        else
        {
            errorString = string.Format("map下的子元素{0}之后未声明用英文逗号分隔下一个子元素，也没有声明map的结束，输入值为{1}", key, nextToken.DefineString);
            return;
        }
    }
}

/// <summary>
/// mapString类型中的词法单元类型
/// </summary>
public enum MapStringTokenType
{
    StartMap,
    EndMap,
    String,
    StringWithQuotationMark,
    Number,
    Comma,
    EqualSign,
}

/// <summary>
/// mapString类型中的词法单元信息
/// </summary>
public class MapStringToken
{
    public MapStringTokenType TokenType { get; set; }
    public string DefineString { get; set; }

    public MapStringToken(MapStringTokenType tokenType, string defineString)
    {
        TokenType = tokenType;
        DefineString = defineString;
    }
}
