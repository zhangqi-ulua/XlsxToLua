using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

public class TableCheckHelper
{
    public static bool CheckTable(TableInfo tableInfo, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();
        // 字段检查
        StringBuilder fieldErrorBuilder = new StringBuilder();
        foreach (FieldInfo fieldInfo in tableInfo.GetAllFieldInfo())
        {
            CheckOneField(fieldInfo, out errorString);
            if (errorString != null)
            {
                fieldErrorBuilder.Append(errorString);
                errorString = null;
            }
        }
        string fieldErrorString = fieldErrorBuilder.ToString();
        if (!string.IsNullOrEmpty(fieldErrorString))
            errorStringBuilder.Append("字段检查中发现以下错误：\n").Append(fieldErrorString);

        // 整表检查
        TableCheckHelper.CheckTableFunc(tableInfo, out errorString);
        if (errorString != null)
        {
            errorStringBuilder.Append("整表检查中发现以下错误：\n").Append(errorString);
            errorString = null;
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    private static bool CheckOneField(FieldInfo fieldInfo, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();

        // 解析检查规则
        List<FieldCheckRule> checkRules = GetCheckRules(fieldInfo.CheckRule, out errorString);
        if (errorString != null)
        {
            errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）填写的检查规则\"{2}\"不合法：{3}，不得不跳过对该字段的检查\n\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), fieldInfo.CheckRule, errorString);
            errorString = null;
        }
        else if (checkRules != null)
        {
            CheckByRules(checkRules, fieldInfo, out errorString);
            if (errorString != null)
            {
                errorStringBuilder.Append(errorString);
                errorString = null;
            }
        }

        // array或dict下属子元素字段的检查
        if (fieldInfo.DataType == DataType.Array || fieldInfo.DataType == DataType.Dict)
        {
            foreach (FieldInfo childField in fieldInfo.ChildField)
            {
                CheckOneField(childField, out errorString);
                if (errorString != null)
                {
                    errorStringBuilder.Append(errorString);
                    errorString = null;
                }
            }
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    public static bool CheckByRules(List<FieldCheckRule> checkRules, FieldInfo fieldInfo, out string errorString)
    {
        StringBuilder errorStingBuilder = new StringBuilder();
        errorString = null;

        foreach (FieldCheckRule checkRule in checkRules)
        {
            switch (checkRule.CheckType)
            {
                case TableCheckType.Unique:
                    {
                        CheckUnique(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.NotEmpty:
                    {
                        CheckNotEmpty(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Ref:
                    {
                        CheckRef(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Range:
                    {
                        CheckRange(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Effective:
                    {
                        CheckEffective(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Illegal:
                    {
                        CheckIllegal(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.GreaterThan:
                    {
                        CheckGreaterThan(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.File:
                    {
                        CheckFile(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Func:
                    {
                        CheckFunc(fieldInfo, checkRule, out errorString);
                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit(string.Format("用CheckTable函数解析出了检查规则，但没有对应的检查函数，检查规则类型为{0}", checkRule.CheckType));
                        break;
                    }
            }

            if (errorString != null)
            {
                errorStingBuilder.AppendFormat("字段\"{0}\"（列号：{1}）未通过\"{2}\"的检查规则\n{3}\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), checkRule.CheckRuleString, errorString);
                errorString = null;
            }
        }

        if (string.IsNullOrEmpty(errorStingBuilder.ToString()))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = errorStingBuilder.ToString();
            return false;
        }
    }

    /// <summary>
    /// 解析一个字段的所有表格检查规则
    /// </summary>
    public static List<FieldCheckRule> GetCheckRules(string checkRuleString, out string errorString)
    {
        if (string.IsNullOrEmpty(checkRuleString))
        {
            errorString = null;
            return null;
        }
        else
        {
            List<FieldCheckRule> checkRules = new List<FieldCheckRule>();
            errorString = null;

            // 不同检查规则通过&&分隔
            string[] ruleString = checkRuleString.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ruleString.Length; ++i)
            {
                string oneRule = ruleString[i].Trim();
                if (oneRule == string.Empty)
                    continue;

                List<FieldCheckRule> oneCheckRule = _GetOneCheckRule(oneRule, out errorString);
                if (errorString != null)
                    return null;
                else
                    checkRules.AddRange(oneCheckRule);
            }

            return checkRules;
        }
    }

    /// <summary>
    /// 解析一条表格检查规则
    /// 注意：要把config配置的规则彻底解析为TABLE_CHECK_TYPE定义的基本的检查规则，故要考虑如果是config配置的规则中继续嵌套config配置规则的情况
    /// </summary>
    private static List<FieldCheckRule> _GetOneCheckRule(string ruleString, out string errorString)
    {
        List<FieldCheckRule> oneCheckRule = new List<FieldCheckRule>();
        errorString = null;

        if (ruleString.StartsWith("notEmpty", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.NotEmpty;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("unique", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Unique;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("ref", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Ref;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith(">") || ruleString.StartsWith(">="))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.GreaterThan;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("func", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Func;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("file", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.File;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("$"))
        {
            // 到config文件中找到对应的检查规则
            if (AppValues.ConfigData.ContainsKey(ruleString))
            {
                string configRuleString = AppValues.ConfigData[ruleString];
                // 不同检查规则通过&&分隔
                string[] ruleStringInConfigRule = configRuleString.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ruleStringInConfigRule.Length; ++i)
                {
                    string oneRule = ruleStringInConfigRule[i].Trim();
                    if (oneRule.Equals(string.Empty))
                        continue;

                    // 递归调用自身，解析config中配置的检查规则
                    List<FieldCheckRule> configCheckRules = _GetOneCheckRule(oneRule, out errorString);
                    if (errorString != null)
                    {
                        errorString = string.Format("config文件中名为\"{0}\"的配置\"{1}\"有误：", ruleString, configRuleString) + errorString;
                        return null;
                    }
                    else
                        oneCheckRule.AddRange(configCheckRules);
                }
            }
            else
            {
                errorString = string.Format("config文件中找不到名为\"{0}\"的检查规则配置", ruleString);
                return null;
            }
        }
        else if (ruleString.StartsWith("!") && ruleString.IndexOf("{") > 0)
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Illegal;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("{"))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Effective;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("(") || ruleString.StartsWith("["))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Range;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else
        {
            errorString = "未知的检查规则";
            return null;
        }

        return oneCheckRule;
    }

    /// <summary>
    /// 用于输入数据的非空检查，适用于int、long、float、string、lang、date、time、json或tableString型
    /// 注意：string类型要求字符串不能为空但允许为连续空格字符串，如果也不允许为连续空格字符串，需要声明为notEmpty[trim]
    /// 注意：lang类型声明notEmpty[key]只检查是否填写了key值，声明notEmpty[value]只检查填写的key在相应的lang文件中能找到对应的value，声明notEmpty[key|value]或notEmpty则包含这两个要求
    /// </summary>
    public static bool CheckNotEmpty(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 存储检查出的空值对应的行号
        List<int> emptyDataLines = new List<int>();

        if (fieldInfo.DataType == DataType.String)
        {
            if (checkRule.CheckRuleString.Equals("notEmpty", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.Data, emptyDataLines, false);
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty[trim]", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.Data, emptyDataLines, true);
            }
            else
            {
                errorString = string.Format("数据非空检查规则用于string型的声明错误，输入的检查规则字符串为{0}。而string型声明notEmpty要求字符串不能为空但允许为连续空格字符串，如果也不允许为连续空格字符串，需要声明为notEmpty[trim]\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Lang)
        {
            if (checkRule.CheckRuleString.Equals("notEmpty[key]", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.LangKeys, emptyDataLines, true);
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty[value]", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 0; i < fieldInfo.LangKeys.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值，忽略未填写key的
                    if (fieldInfo.LangKeys[i] == null || string.IsNullOrEmpty(fieldInfo.LangKeys[i].ToString()))
                        continue;

                    if (fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty", StringComparison.CurrentCultureIgnoreCase) || checkRule.CheckRuleString.Equals("notEmpty[key|value]", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 0; i < fieldInfo.LangKeys.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值
                    if (fieldInfo.LangKeys[i] == null)
                        continue;

                    // 填写的key不能为空或连续空格字符串，且必须能在lang文件中找到对应的value
                    if (string.IsNullOrEmpty(fieldInfo.LangKeys[i].ToString().Trim()) || fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
            else
            {
                errorString = string.Format("数据非空检查规则用于lang型的声明错误，输入的检查规则字符串为{0}。而lang型声明notEmpty[key]只检查是否填写了key值，声明notEmpty[value]只检查填写的key在相应的lang文件中能找到对应的value，声明notEmpty[key|value]或notEmpty则包含这两个要求\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
        {
            if (AppValues.IsAllowedNullNumber == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    // 如果int、long、float型字段下取值为null，可能填写的为空值，也可能是父集合类型标为无效
                    if (fieldInfo.ParentField != null)
                    {
                        if ((bool)fieldInfo.ParentField.Data[i] == false)
                            continue;
                        else if (fieldInfo.ParentField.ParentField != null && (bool)fieldInfo.ParentField.ParentField.Data[i] == false)
                            continue;
                    }
                    else if (fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
        {
            for (int i = 0; i < fieldInfo.Data.Count; ++i)
            {
                // 如果date、time型字段下取值为null，可能填写的为空值，也可能是父集合类型标为无效
                if (fieldInfo.ParentField != null)
                {
                    if ((bool)fieldInfo.ParentField.Data[i] == false)
                        continue;
                    else if (fieldInfo.ParentField.ParentField != null && (bool)fieldInfo.ParentField.ParentField.Data[i] == false)
                        continue;
                }
                else if (fieldInfo.Data[i] == null)
                    emptyDataLines.Add(i);
            }
        }
        else if (fieldInfo.DataType == DataType.Json || fieldInfo.DataType == DataType.TableString)
        {
            for (int i = 0; i < fieldInfo.Data.Count; ++i)
            {
                // json、tableString类型必为独立字段
                if (fieldInfo.Data[i] == null)
                    emptyDataLines.Add(i);
            }
        }
        else
        {
            errorString = string.Format("数据非空检查规则只适用于int、long、float、string、lang、date、time、json或tableString类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }

        if (emptyDataLines.Count > 0)
        {
            StringBuilder errorStringBuild = new StringBuilder();
            errorStringBuild.Append("存在以下空数据，行号分别为：");
            string separator = ", ";
            foreach (int lineNum in emptyDataLines)
                errorStringBuild.AppendFormat("{0}{1}", lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1, separator);

            // 去掉末尾多余的", "
            errorStringBuild.Remove(errorStringBuild.Length - separator.Length, separator.Length);

            errorStringBuild.Append("\n");
            errorString = errorStringBuild.ToString();

            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 检查List中的string型的数据是否为空，传入的needTrim表示检查前是否对字符串进行trim操作
    /// </summary>
    private static bool _CheckInputDataNotEmpty(List<object> data, List<int> emptyDataLines, bool needTrim)
    {
        if (needTrim == true)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                // 忽略无效集合元素下属子类型的空值
                if (data[i] == null)
                    continue;

                if (string.IsNullOrEmpty(data[i].ToString().Trim()))
                    emptyDataLines.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < data.Count; ++i)
            {
                // 忽略无效集合元素下属子类型的空值
                if (data[i] == null)
                    continue;

                if (string.IsNullOrEmpty(data[i].ToString()))
                    emptyDataLines.Add(i);
            }
        }

        return data.Count == 0;
    }

    /// <summary>
    /// 用于数据唯一性检查，适用于string、int、long、float或lang类型
    /// 注意：string型、lang型如果填写或者找到的value为空字符串，允许出现多次为空的情况
    /// 注意：lang型默认只检查key不能重复，如果还想检查不同key对应的value也不能相同则需要声明为unique[value]
    /// </summary>
    public static bool CheckUnique(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String || fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
        {
            _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.Data, out errorString);
            if (errorString == null)
                return true;
            else
            {
                errorString = string.Format("数据类型为{0}的字段中，存在以下重复数据：\n", fieldInfo.DataType.ToString()) + errorString;
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Lang)
        {
            // 只检查key则与string、int、float型的操作相同
            if ("unique".Equals(checkRule.CheckRuleString, StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.LangKeys, out errorString);
                if (errorString == null)
                    return true;
                else
                {
                    errorString = "要求仅检查lang型数据的key值不重复，但存在以下重复key：\n" + errorString;
                    return false;
                }
            }
            else if ("unique[value]".Equals(checkRule.CheckRuleString, StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.Data, out errorString);
                if (errorString == null)
                    return true;
                else
                {
                    errorString = "要求检查lang型数据的key值与Value值均不重复，但存在以下重复数据：\n" + errorString;
                    return false;
                }
            }
            else
            {
                errorString = string.Format("唯一性检查规则用于lang型的声明错误，输入的检查规则字符串为{0}。而lang型声明unique仅检查key不重复，声明为unique[value]一并检查value不重复\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else
        {
            errorString = string.Format("唯一性检查规则只适用于string、int、long、float、lang、date或time类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }
    }

    /// <summary>
    /// 用于检查List中的数据（类型为int、long、reffloat或string）是否唯一
    /// 该函数需传入List而不直接传入FieldInfo是因为对于lang型的检查分为只检查key和一并检查value不能重复，传List则可针对两种情况灵活处理
    /// </summary>
    private static bool _CheckInputDataUnique(DataType dataType, List<object> data, out string errorString)
    {
        // 存储每个数据对应的index（key：data， value：index）
        Dictionary<object, int> dataToIndex = new Dictionary<object, int>();
        // 存储已经重复的数据所在的所有行数
        Dictionary<object, List<int>> repeatedDataInfo = new Dictionary<object, List<int>>();

        for (int i = 0; i < data.Count; ++i)
        {
            object oneData = data[i];
            // 如果值为null说明其属于标为无效集合的子数据，或者是数值型字段中的空值，跳过unique检查。如果值为string.Empty说明其属于string类型列中的空数据也跳过检查
            if (oneData == null || string.IsNullOrEmpty(oneData.ToString()))
                continue;

            if (dataToIndex.ContainsKey(oneData))
            {
                if (!repeatedDataInfo.ContainsKey(oneData))
                    repeatedDataInfo.Add(oneData, new List<int>());
                List<int> repeatedRowIndex = repeatedDataInfo[oneData];
                repeatedRowIndex.Add(i);
            }
            else
                dataToIndex.Add(oneData, i);
        }
        // 此时repeatedDataInfo存储的重复行索引中并不包含最早出现这个重复数据的行，需要从dataToIndex中找到
        foreach (var item in repeatedDataInfo)
        {
            var repeatedData = item.Key;
            var repeatedRowIndex = item.Value;
            int index = dataToIndex[repeatedData];
            repeatedRowIndex.Insert(0, index);
        }

        if (repeatedDataInfo.Count > 0)
        {
            StringBuilder repeatedLineInfo = new StringBuilder();
            foreach (var item in repeatedDataInfo)
            {
                if (dataType == DataType.Date)
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", ((DateTime)(item.Key)).ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                if (dataType == DataType.Time)
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", ((DateTime)(item.Key)).ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
                else
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", item.Key);

                List<int> lineIndex = item.Value;
                foreach (int lineNum in lineIndex)
                    repeatedLineInfo.Append(lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1 + ", ");

                // 去掉最后多余的", "
                repeatedLineInfo.Remove(repeatedLineInfo.Length - 2, 2);
                // 换行
                repeatedLineInfo.AppendLine();
            }

            errorString = repeatedLineInfo.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 用于int、long、float三种数值类型或date、time两种时间类型的范围检查
    /// </summary>
    public static bool CheckRange(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        bool isNumberDataType = fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float;
        bool isTimeDataType = fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time;
        if (isNumberDataType == false && isTimeDataType == false)
        {
            errorString = string.Format("值范围检查只能用于int、long、float三种数值类型或date、time两种时间类型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }
        // 检查填写的检查规则是否正确
        bool isIncludeFloor;
        bool isIncludeCeil;
        bool isCheckFloor;
        bool isCheckCeil;
        double floorValue = 0;
        double ceilValue = 0;
        DateTime floorDateTime = AppValues.REFERENCE_DATE;
        DateTime ceilDateTime = AppValues.REFERENCE_DATE;
        // 规则首位必须为方括号或者圆括号
        if (checkRule.CheckRuleString.StartsWith("("))
            isIncludeFloor = false;
        else if (checkRule.CheckRuleString.StartsWith("["))
            isIncludeFloor = true;
        else
        {
            errorString = "值范围检查定义错误：必须用英文(或[开头，表示有效范围是否包含等于下限的情况\n";
            return false;
        }
        // 规则末位必须为方括号或者圆括号
        if (checkRule.CheckRuleString.EndsWith(")"))
            isIncludeCeil = false;
        else if (checkRule.CheckRuleString.EndsWith("]"))
            isIncludeCeil = true;
        else
        {
            errorString = "值范围检查定义错误：必须用英文)或]结尾，表示有效范围是否包含等于上限的情况\n";
            return false;
        }
        // 去掉首尾的括号
        string temp = checkRule.CheckRuleString.Substring(1, checkRule.CheckRuleString.Length - 2);
        // 通过英文逗号分隔上下限
        string[] floorAndCeilString = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (floorAndCeilString.Length != 2)
        {
            errorString = "值范围检查定义错误：必须用一个英文逗号分隔值范围的上下限\n";
            return false;
        }
        string floorString = floorAndCeilString[0].Trim();
        string ceilString = floorAndCeilString[1].Trim();
        // 提取下限数值
        if (floorString.Equals("*"))
            isCheckFloor = false;
        else
        {
            isCheckFloor = true;
            if (isNumberDataType == true)
            {
                if (double.TryParse(floorString, out floorValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：下限不是合法的数字，你输入的为{0}\n", floorString);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.DataType == DataType.Date)
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;
                        floorDateTime = Convert.ToDateTime(floorString, dateTimeFormat);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：date型下限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_DATE_FORMAT, floorString);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortTimePattern = AppValues.APP_DEFAULT_TIME_FORMAT;
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(floorString, dateTimeFormat);
                        floorDateTime = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：time型下限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_TIME_FORMAT, floorString);
                        return false;
                    }
                }
            }
        }
        // 提取上限数值
        if (ceilString.Equals("*"))
            isCheckCeil = false;
        else
        {
            isCheckCeil = true;
            if (isNumberDataType == true)
            {
                if (double.TryParse(ceilString, out ceilValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：上限不是合法的数字，你输入的为{0}\n", ceilString);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.DataType == DataType.Date)
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;
                        ceilDateTime = Convert.ToDateTime(ceilString, dateTimeFormat);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：date型上限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_DATE_FORMAT, ceilString);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_TIME_FORMAT;
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(ceilString, dateTimeFormat);
                        ceilDateTime = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：time型上限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_TIME_FORMAT, ceilString);
                        return false;
                    }
                }
            }
        }
        // 判断上限是否大于下限
        if (isNumberDataType == true)
        {
            if (isCheckFloor == true && isCheckCeil == true && floorValue >= ceilValue)
            {
                errorString = string.Format("值范围检查定义错误：上限值必须大于下限值，你输入的下限为{0}，上限为{1}\n", floorString, ceilString);
                return false;
            }
        }
        else
        {
            if (isCheckFloor == true && isCheckCeil == true && floorDateTime >= ceilDateTime)
            {
                errorString = string.Format("值范围检查定义错误：上限值必须大于下限值，你输入的下限为{0}，上限为{1}\n", floorDateTime.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), ceilDateTime.ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                return false;
            }
        }

        // 进行检查
        // 存储检查出的非法值（key：数据索引， value：填写值）
        Dictionary<int, object> illegalValue = new Dictionary<int, object>();
        if (isCheckFloor == true && isCheckCeil == false)
        {
            if (isIncludeFloor == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值或本身为空值
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }
        else if ((isCheckFloor == false && isCheckCeil == true))
        {
            if (isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }
        else if ((isCheckFloor == true && isCheckCeil == true))
        {
            if (isIncludeFloor == false && isIncludeCeil == false)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue || inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime || inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == true && isIncludeCeil == false)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue || inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime || inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == false && isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue || inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime || inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == true && isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue || inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime || inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }

        if (illegalValue.Count > 0)
        {
            StringBuilder illegalValueInfo = new StringBuilder();
            if (isNumberDataType == true)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);
            }
            else if (fieldInfo.DataType == DataType.Date)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, ((DateTime)(item.Value)).ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
            }
            else if (fieldInfo.DataType == DataType.Time)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, ((DateTime)(item.Value)).ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
            }

            errorString = illegalValueInfo.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 获取某字段的所有数据中属于或不属于指定集合中取值的数据所在索引
    /// </summary>
    /// <param name="data">字段数据</param>
    /// <param name="dataType">字段数据类型</param>
    /// <param name="setValueDefineString">集合取值的定义字符串</param>
    /// <param name="isInSet">获取属于还是不属于集合中数据所在索引</param>
    /// <param name="repeatedSetValue">集合定义字符串中存在的重复定义值</param>
    /// <param name="errorDataIndex">不满足属于或不属于要求的数据所在索引</param>
    /// <param name="errorString">检查规则定义错误信息</param>
    private static void _GetValueIsInSet(List<object> data, DataType dataType, string setValueDefineString, bool isInSet, out List<object> repeatedSetValue, out List<int> errorDataIndex, out string errorString)
    {
        repeatedSetValue = new List<object>();
        errorDataIndex = new List<int>();
        errorString = null;

        if (dataType == DataType.Int || dataType == DataType.Long || dataType == DataType.Float || dataType == DataType.Date || dataType == DataType.Time)
        {
            // 去除首尾花括号后，通过英文逗号分隔每个集合值即可
            if (!(setValueDefineString.StartsWith("{") && setValueDefineString.EndsWith("}")))
            {
                errorString = "集合值定义错误：必须在首尾用一对花括号包裹整个定义内容";
                return;
            }
            string temp = setValueDefineString.Substring(1, setValueDefineString.Length - 2).Trim();
            if (string.IsNullOrEmpty(temp))
            {
                errorString = "集合值定义错误：至少需要输入一个值";
                return;
            }

            string[] values = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (dataType == DataType.Int || dataType == DataType.Long)
            {
                // 存储用户设置的取值集合（key：取值， value：恒为true）
                Dictionary<long, bool> setValues = new Dictionary<long, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    long oneValue;
                    if (long.TryParse(oneValueString, out oneValue) == true)
                    {
                        // 记录集合定义字符串中的重复值
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    else
                    {
                        errorString = string.Format("集合值定义中出现了非{0}型数据，其为\"{1}\"\n", dataType, oneValueString);
                        return;
                    }
                }
                // 对本列所有数据进行检查
                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        // 忽略无效集合元素下属子类型的空值或本身为空值
                        if (data[i] == null)
                            continue;

                        long inputData = Convert.ToInt64(data[i]);
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        long inputData = Convert.ToInt64(data[i]);
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Float)
            {
                Dictionary<float, bool> setValues = new Dictionary<float, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    float oneValue;
                    if (float.TryParse(oneValueString, out oneValue) == true)
                    {
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    else
                    {
                        errorString = string.Format("集合值定义中出现了非float型数据，其为\"{0}\"", oneValueString);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        float inputData = Convert.ToSingle(data[i]);
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        float inputData = Convert.ToSingle(data[i]);
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Date)
            {
                DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;

                Dictionary<DateTime, bool> setValues = new Dictionary<DateTime, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    try
                    {
                        DateTime oneValue = Convert.ToDateTime(oneValueString, dateTimeFormat);
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    catch
                    {
                        errorString = string.Format("集合值定义中出现了非法的date型数据，其为\"{0}\"，请按{1}的形式填写", oneValueString, AppValues.APP_DEFAULT_DATE_FORMAT);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Time)
            {
                DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                dateTimeFormat.ShortTimePattern = AppValues.APP_DEFAULT_TIME_FORMAT;

                Dictionary<DateTime, bool> setValues = new Dictionary<DateTime, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    try
                    {
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(oneValueString, dateTimeFormat);
                        DateTime oneValue = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    catch
                    {
                        errorString = string.Format("集合值定义中出现了非法的time型数据，其为\"{0}\"，请按{1}的形式填写", oneValueString, AppValues.APP_DEFAULT_TIME_FORMAT);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else
            {
                errorString = "用_GetValueIsInSet函数检查了非int、long、float、date、time型的字段";
                Utils.LogErrorAndExit(errorString);
                return;
            }
        }
        else if (dataType == DataType.String)
        {
            // 用于分隔集合值定义的字符，默认为英文逗号
            char separator = ',';
            // 去除首尾花括号后整个集合值定义内容
            string tempSetValueDefineString = setValueDefineString;

            // 右边花括号的位置
            int rightBraceIndex = setValueDefineString.LastIndexOf('}');
            if (rightBraceIndex == -1)
            {
                errorString = "string型集合值定义错误：必须用一对花括号包裹整个定义内容";
                return;
            }
            // 如果声明了分隔集合值的字符
            if (rightBraceIndex != setValueDefineString.Length - 1)
            {
                int leftBracketIndex = setValueDefineString.LastIndexOf('(');
                int rightBracketIndex = setValueDefineString.LastIndexOf(')');
                if (leftBracketIndex < rightBraceIndex || rightBracketIndex < leftBracketIndex)
                {
                    errorString = "string型集合值定义错误：需要在最后面的括号中声明分隔各个集合值的一个字符，如果使用默认的英文逗号作为分隔符，则不必在最后面用括号声明自定义分隔字符";
                    return;
                }
                string separatorString = setValueDefineString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                if (separatorString.Length > 1)
                {
                    errorString = string.Format("string型集合值定义错误：自定义集合值的分隔字符只能为一个字符，而你输入的为\"{0}\"", separatorString);
                    return;
                }
                separator = separatorString[0];

                // 取得前面用花括号包裹的集合值定义
                tempSetValueDefineString = setValueDefineString.Substring(0, rightBraceIndex + 1).Trim();
            }

            // 去除花括号
            tempSetValueDefineString = tempSetValueDefineString.Substring(1, tempSetValueDefineString.Length - 2);
            if (string.IsNullOrEmpty(tempSetValueDefineString))
            {
                errorString = "string型集合值定义错误：至少需要输入一个集合值";
                return;
            }

            string[] setValueDefine = tempSetValueDefineString.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (setValueDefine.Length == 0)
            {
                errorString = "string型集合值定义错误：至少需要输入一个集合值";
                return;
            }

            // 存储定义的集合值（key：集合值， value：恒为true）
            Dictionary<string, bool> setValues = new Dictionary<string, bool>();
            foreach (string setValue in setValueDefine)
            {
                if (setValues.ContainsKey(setValue))
                    repeatedSetValue.Add(setValue);
                else
                    setValues.Add(setValue, true);
            }

            // 对本列所有数据进行检查
            if (isInSet == true)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值
                    if (data[i] == null)
                        continue;

                    string inputData = data[i].ToString();
                    if (!setValues.ContainsKey(inputData))
                        errorDataIndex.Add(i);
                }
            }
            else
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    if (data[i] == null)
                        continue;

                    string inputData = data[i].ToString();
                    if (setValues.ContainsKey(inputData))
                        errorDataIndex.Add(i);
                }
            }
        }
        else
        {
            errorString = string.Format("该检查只能用于int、long、float、string、date或time型的字段，而该字段为{0}型", dataType);
            return;
        }
    }

    /// <summary>
    /// 用于int、long、float、string、date或time型取值必须为指定有效取值中的一个的检查
    /// </summary>
    public static bool CheckEffective(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        List<object> repeatedSetValue = null;
        List<int> errorDataIndex = null;
        _GetValueIsInSet(fieldInfo.Data, fieldInfo.DataType, checkRule.CheckRuleString, true, out repeatedSetValue, out errorDataIndex, out errorString);

        if (errorString == null)
        {
            if (repeatedSetValue.Count > 0)
            {
                foreach (object setValue in repeatedSetValue)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的值有效性检查规则定义中，出现了相同的有效值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), setValue));
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的值有效性检查规则定义中，出现了相同的有效值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT)));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的值有效性检查规则定义中，出现了相同的有效值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT)));
                    }
                }
            }
            if (errorDataIndex.Count > 0)
            {
                StringBuilder illegalValueInfo = new StringBuilder();
                foreach (int dataIndex in errorDataIndex)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不属于有效取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[dataIndex]);
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不属于有效取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不属于有效取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
                    }
                }

                errorString = illegalValueInfo.ToString();
                return false;
            }
            else
                return true;
        }
        else
        {
            errorString = errorString + "\n";
            return false;
        }
    }

    /// <summary>
    /// 用于int、long、float、string、date或time型取值不允许为指定集合值中的一个的检查
    /// </summary>
    public static bool CheckIllegal(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        List<object> repeatedSetValue = null;
        List<int> errorDataIndex = null;
        string setValueDefineString = checkRule.CheckRuleString.Substring(1).Trim();
        _GetValueIsInSet(fieldInfo.Data, fieldInfo.DataType, setValueDefineString, false, out repeatedSetValue, out errorDataIndex, out errorString);

        if (errorString == null)
        {
            if (repeatedSetValue.Count > 0)
            {
                foreach (object setValue in repeatedSetValue)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), setValue));
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT)));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT)));
                    }
                }
            }
            if (errorDataIndex.Count > 0)
            {
                StringBuilder illegalValueInfo = new StringBuilder();
                foreach (int dataIndex in errorDataIndex)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[dataIndex]);
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
                    }
                }

                errorString = illegalValueInfo.ToString();
                return false;
            }
            else
                return true;
        }
        else
        {
            errorString = errorString + "\n";
            return false;
        }
    }

    /// <summary>
    /// 通过索引定义字符串，找到某张表格中的指定字段，若字段为dict的子元素，用“.”进行层层索引，若为array的子元素，用进行[x]索引
    /// </summary>
    public static FieldInfo GetFieldByIndexDefineString(string indexDefineString, TableInfo tableInfo, out string errorString)
    {
        FieldInfo fieldInfo = null;
        if (string.IsNullOrEmpty(indexDefineString.Trim()))
        {
            errorString = "输入的索引定义字符串不允许为空";
            return null;
        }
        indexDefineString = indexDefineString.Trim();
        // 如果是独立字段
        if (indexDefineString.IndexOf('.') == -1 && indexDefineString.IndexOf('[') == -1)
        {
            fieldInfo = tableInfo.GetFieldInfoByFieldName(indexDefineString);
            if (fieldInfo == null)
            {
                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，若此字段为dict或array的子元素，请通过.或[x]的形式索引到该子字段", indexDefineString);
                return null;
            }
        }
        else
        {
            StringBuilder tempFieldNameBuilder = new StringBuilder();
            StringBuilder tempArrayIndexBuilder = new StringBuilder();
            bool isInBracket = false;
            bool isAfterDot = false;
            for (int i = 0; i < indexDefineString.Length; ++i)
            {
                char currentChar = indexDefineString[i];
                if (currentChar == '.')
                {
                    if (isInBracket == true)
                    {
                        errorString = string.Format("索引array的[]中不允许出现小数点，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    if (i - 1 >= 0 && indexDefineString[i - 1] == '.')
                    {
                        errorString = string.Format("索引定义字符串中出现了连续的小数点，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }

                    string tempFieldName = tempFieldNameBuilder.ToString();
                    // 处理array嵌套dict，形如rewardList[1].rewardType的情况
                    if (string.IsNullOrEmpty(tempFieldName))
                    {
                        if (fieldInfo == null)
                        {
                            errorString = string.Format("小数点必须声明在dict型字段的变量名之后，用于索引dict型字段，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                            return null;
                        }
                        if (fieldInfo.DataType != DataType.Dict)
                        {
                            if (fieldInfo.DataType == DataType.Array)
                                errorString = string.Format("用小数点只能索引dict型字段，而你索引的是array型字段\"{0}\"，请使用[]来索引array型字段", fieldInfo.FieldName);
                            else
                                errorString = string.Format("用小数点只能索引dict型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, fieldInfo.FieldName);

                            return null;
                        }
                    }
                    else
                    {
                        // 处理dict嵌套dict，形如systemConfigDict.audioConfigDict.isOpen的情况
                        if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                        {
                            bool isFoundDictChildField = false;
                            foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                            {
                                if (tempFieldName.Equals(dictChildField.FieldName))
                                {
                                    fieldInfo = dictChildField;
                                    isFoundDictChildField = true;
                                    break;
                                }
                            }
                            if (isFoundDictChildField == false)
                            {
                                errorString = string.Format("dict型字段中不存在名为\"{0}\"的子元素，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                        }
                        else
                        {
                            fieldInfo = tableInfo.GetFieldInfoByFieldName(tempFieldName);
                            if (fieldInfo == null)
                            {
                                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                            if (fieldInfo.DataType != DataType.Dict)
                            {
                                if (fieldInfo.DataType == DataType.Array)
                                    errorString = string.Format("用小数点只能索引dict型字段，而你索引的是array型字段\"{0}\"，请使用[]来索引array型字段", tempFieldName);
                                else
                                    errorString = string.Format("用小数点只能索引dict型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, tempFieldName);

                                return null;
                            }
                        }
                    }

                    isAfterDot = true;
                    tempFieldNameBuilder = new StringBuilder();
                }
                else if (currentChar == '[')
                {
                    if (isInBracket == true)
                    {
                        errorString = string.Format("索引定义中括号不匹配，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    // 排除array嵌套array，形如array[1][1]的情况
                    if (!(fieldInfo != null && fieldInfo.DataType == DataType.Array))
                    {
                        // 处理dict嵌套array，形如pveBattleConfig.eliteBattleConfig[1]的情况
                        if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                        {
                            string tempFieldName = tempFieldNameBuilder.ToString();
                            bool isFoundDictChildField = false;
                            foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                            {
                                if (tempFieldName.Equals(dictChildField.FieldName))
                                {
                                    fieldInfo = dictChildField;
                                    isFoundDictChildField = true;
                                    break;
                                }
                            }
                            if (isFoundDictChildField == false)
                            {
                                errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", fieldInfo.FieldName, tempFieldName);
                                return null;
                            }
                        }
                        else
                        {
                            string tempFieldName = tempFieldNameBuilder.ToString();
                            fieldInfo = tableInfo.GetFieldInfoByFieldName(tempFieldName);
                            if (fieldInfo == null)
                            {
                                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                            if (fieldInfo.DataType != DataType.Array)
                            {
                                if (fieldInfo.DataType == DataType.Dict)
                                    errorString = string.Format("用[]只能索引array型字段，而你索引的是dict型字段\"{0}\"，请使用小数点来索引dict型字段", tempFieldName);
                                else
                                    errorString = string.Format("用[]只能索引array型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, tempFieldName);

                                return null;
                            }
                        }
                    }

                    isInBracket = true;
                    isAfterDot = false;
                    tempFieldNameBuilder = new StringBuilder();
                }
                else if (currentChar == ']')
                {
                    if (isInBracket == false)
                    {
                        errorString = string.Format("索引定义中括号不匹配，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    string arrayIndexString = tempArrayIndexBuilder.ToString();
                    int arrayIndex = -1;
                    if (int.TryParse(arrayIndexString, out arrayIndex) == true)
                    {
                        if (arrayIndex > 0)
                        {
                            int arrayChildCount = fieldInfo.ChildField.Count;
                            if (arrayIndex > arrayChildCount)
                            {
                                errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法，其只有{1}个子元素，而你要取第{2}个子元素", fieldInfo.FieldName, arrayChildCount, arrayIndex);
                                return null;
                            }
                            else
                                fieldInfo = fieldInfo.ChildField[arrayIndex - 1];
                        }
                        else
                        {
                            errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法必须为大于0的数字，你输入的为{1}", fieldInfo.FieldName, arrayIndexString);
                            return null;
                        }
                    }
                    else
                    {
                        errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法，你输入的为{1}", fieldInfo.FieldName, arrayIndexString);
                        return null;
                    }

                    isInBracket = false;
                    tempArrayIndexBuilder = new StringBuilder();
                }
                else
                {
                    if (isInBracket == false)
                        tempFieldNameBuilder.Append(currentChar);
                    else
                        tempArrayIndexBuilder.Append(currentChar);
                }
            }
            if (isAfterDot == true)
            {
                string tempFieldName = tempFieldNameBuilder.ToString();
                // 处理最后通过小数点索引Dict中子元素的情况
                if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                {
                    bool isFoundDictChildField = false;
                    foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                    {
                        if (tempFieldName.Equals(dictChildField.FieldName))
                        {
                            fieldInfo = dictChildField;
                            isFoundDictChildField = true;
                            break;
                        }
                    }
                    if (isFoundDictChildField == false)
                    {
                        if (fieldInfo.ParentField != null && fieldInfo.ParentField.DataType == DataType.Array)
                            errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", string.Concat(fieldInfo.ParentField.FieldName, fieldInfo.FieldName), tempFieldName);
                        else
                            errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", fieldInfo.FieldName, tempFieldName);

                        return null;
                    }
                }
            }
            if (isInBracket == true)
            {
                errorString = string.Format("索引定义中括号不匹配，你输入的索引定义字符串为{0}", indexDefineString);
                return null;
            }
        }

        errorString = null;
        return fieldInfo;
    }

    /// <summary>
    /// 用于int、long、float、date或time型同一行某字段值必须大于等于或大于另一字段值的检查
    /// 注意：要进行比较的两个字段可以为同种数据类型，也可以任意比较int、long、float三种数值型大小
    /// </summary>
    public static bool CheckGreaterThan(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        bool isNumberDataType = false;
        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
            isNumberDataType = true;
        else if (fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
            isNumberDataType = false;
        else
        {
            errorString = string.Format("值大小比较检查规则只能用于int、long、float三种数值类型或date、time两种时间类型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }

        bool isContainsEqual = checkRule.CheckRuleString.StartsWith(">=");
        TableInfo tableInfo = AppValues.TableInfo[fieldInfo.TableName];
        string comparedFieldString = null;
        if (isContainsEqual == true)
            comparedFieldString = checkRule.CheckRuleString.Substring(2).Trim();
        else
            comparedFieldString = checkRule.CheckRuleString.Substring(1).Trim();

        // 根据索引字符串定义，找到要与其比较的字段
        FieldInfo comparedField = GetFieldByIndexDefineString(comparedFieldString, tableInfo, out errorString);
        if (errorString != null)
        {
            errorString = string.Format("值大小比较检查规则定义错误：{0}\n", errorString);
            return false;
        }
        // 检查与其比较的字段是否类型匹配
        if (comparedField.DataType == DataType.Int || comparedField.DataType == DataType.Long || comparedField.DataType == DataType.Float)
        {
            if (isNumberDataType == false)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，不支持数值型与时间型的比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
        }
        else if (comparedField.DataType == DataType.Date || comparedField.DataType == DataType.Time)
        {
            if (isNumberDataType == true)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，不支持数值型与时间型的比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
            if (comparedField.DataType != fieldInfo.DataType)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，date型无法与time型进行比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
        }
        // 对这两个字段中的每行数据进行值大小比较检查（任一字段中某行数据为无效数据则忽略对该行两字段数值的比较）
        // 记录检查出的不满足要求的数据，其中object数组含3个元素，分别为未通过检查的数据所在Excel的行号、该字段的值、与其比较的字段的值
        List<object[]> illegalValue = new List<object[]>();
        if (isNumberDataType == true)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    double fieldDataValue = Convert.ToDouble(fieldInfo.Data[i]);
                    double comparedFieldDataValue = Convert.ToDouble(comparedField.Data[i]);
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[i], comparedField.Data[i] });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    double fieldDataValue = Convert.ToDouble(fieldInfo.Data[i]);
                    double comparedFieldDataValue = Convert.ToDouble(comparedField.Data[i]);
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[i], comparedField.Data[i] });
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Date)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT) });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT) });
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Time)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT) });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT) });
                }
            }
        }

        if (illegalValue.Count > 0)
        {
            StringBuilder errorStringBuilder = new StringBuilder();
            errorStringBuilder.AppendFormat("以下行中数据不满足{0}的值大小比较检查规则\n", isContainsEqual == true ? ">=" : ">");
            for (int i = 0; i < illegalValue.Count; ++i)
            {
                object[] oneIllegalValue = illegalValue[i];
                errorStringBuilder.AppendFormat("第{0}行中，本字段所填值为\"{1}\"，与其比较的的字段所填值为\"{2}\"\n", oneIllegalValue[0], oneIllegalValue[1], oneIllegalValue[2]);
            }

            errorString = errorStringBuilder.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 用于检查string型的文件路径对应的文件是否存在
    /// </summary>
    public static bool CheckFile(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 先判断是否输入了Client目录的路径
        if (AppValues.ClientPath == null)
        {
            errorString = "文件存在性检查无法进行：必须在程序运行时传入Client目录的路径\n";
            return false;
        }
        else if (fieldInfo.DataType == DataType.String)
        {
            int colonIndex = checkRule.CheckRuleString.IndexOf(":");
            if (colonIndex == -1)
            {
                errorString = "文件存在性检查定义错误：必须在英文冒号后声明相对于Client目录的路径\n";
                return false;
            }
            else
            {
                // 存储不存在的文件信息（key：行号， value：输入的文件名）
                Dictionary<int, string> inexistFileInfo = new Dictionary<int, string>();
                // 存储含有\或/的非法文件名信息
                List<int> illegalFileNames = new List<int>();

                // 判断规则中填写的文件的路径与Client路径组合后是否为一个已存在路径
                string inputPath = checkRule.CheckRuleString.Substring(colonIndex + 1, checkRule.CheckRuleString.Length - colonIndex - 1).Trim();
                string pathString = Utils.CombinePath(AppValues.ClientPath, inputPath);
                if (!Directory.Exists(pathString))
                {
                    errorString = string.Format("文件存在性检查定义错误：声明的文件所在目录不存在，请检查定义的路径是否正确，最终拼得的路径为{0}\n", pathString);
                    return false;
                }
                // 提取file和冒号之间的字符串，判断是否声明扩展名
                const string START_STRING = "file";
                string extensionString = checkRule.CheckRuleString.Substring(START_STRING.Length, colonIndex - START_STRING.Length).Trim();
                // 如果声明了扩展名，则遍历出目标目录下所有该扩展名的文件，然后逐行判断文件是否存在
                if (!string.IsNullOrEmpty(extensionString))
                {
                    if (extensionString.StartsWith("(") && extensionString.EndsWith(")"))
                    {
                        // 提取括号中定义的扩展名
                        string extension = extensionString.Substring(1, extensionString.Length - 2).Trim();
                        // 判断扩展名是否合法（只能为数字或小写英文字母）
                        foreach (char c in extension)
                        {
                            if (!((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
                            {
                                errorString = string.Format("文件存在性检查定义错误：声明文件扩展名不合法，文件扩展名只能由小写英文字母和数字组成，你填写的为{0}\n", extension);
                                return false;
                            }
                        }

                        // 存储该目录下为该扩展名的文件的文件名
                        Dictionary<string, bool> existFileNames = new Dictionary<string, bool>();
                        DirectoryInfo folder = new DirectoryInfo(pathString);
                        foreach (FileInfo file in folder.GetFiles("*." + extension))
                        {
                            // 注意file.Name中包含扩展名，需要除去
                            int dotIndex = file.Name.LastIndexOf('.');
                            string fileNameWithoutExtension = file.Name.Substring(0, dotIndex);
                            existFileNames.Add(fileNameWithoutExtension, true);
                        }

                        for (int i = 0; i < fieldInfo.Data.Count; ++i)
                        {
                            // 忽略无效集合元素下属子类型的空值
                            if (fieldInfo.Data[i] == null)
                                continue;

                            // 文件名中不允许含有\或/，即不支持文件在填写路径的非同级目录
                            string inputFileName = fieldInfo.Data[i].ToString().Trim();
                            if (inputFileName.IndexOf('\\') != -1 || inputFileName.IndexOf('/') != -1)
                                illegalFileNames.Add(i);
                            else
                            {
                                if (!existFileNames.ContainsKey(inputFileName))
                                    inexistFileInfo.Add(i, inputFileName);
                            }
                        }
                    }
                    else
                    {
                        errorString = "文件存在性检查定义错误：如果要声明扩展名，\"file\"与英文冒号之间必须在英文括号内声明扩展名\n";
                        return false;
                    }
                }
                // 如果没有声明扩展名，则每行数据都用File.Exists判断是否存在
                else
                {
                    for (int i = 0; i < fieldInfo.Data.Count; ++i)
                    {
                        // 忽略无效集合元素下属子类型的空值
                        if (fieldInfo.Data[i] == null)
                            continue;

                        // 文件名中不允许含有\或/，即不支持文件在填写路径的非同级目录
                        string inputFileName = fieldInfo.Data[i].ToString().Trim();
                        if (inputFileName.IndexOf('\\') != -1 || inputFileName.IndexOf('/') != -1)
                            illegalFileNames.Add(i);
                        else
                        {
                            string path = Utils.CombinePath(pathString, inputFileName);
                            if (!File.Exists(path))
                                inexistFileInfo.Add(i, inputFileName);
                        }
                    }
                }

                if (inexistFileInfo.Count > 0 || illegalFileNames.Count > 0)
                {
                    StringBuilder errorStringBuild = new StringBuilder();
                    if (illegalFileNames.Count > 0)
                    {
                        errorStringBuild.Append("单元格中填写的文件名中不允许含有\"\\\"或\"/\"，即要求填写的文件必须在填写路径的同级目录，以下行对应的文件名不符合此规则：");
                        string separator = ", ";
                        foreach (int lineNum in illegalFileNames)
                            errorStringBuild.AppendFormat("{0}{1}", lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1, separator);

                        // 去掉末尾多余的", "
                        errorStringBuild.Remove(errorStringBuild.Length - separator.Length, separator.Length);

                        errorStringBuild.Append("\n");
                    }
                    if (inexistFileInfo.Count > 0)
                    {
                        errorStringBuild.AppendLine("存在以下找不到的文件：");
                        foreach (var item in inexistFileInfo)
                            errorStringBuild.AppendFormat("第{0}行数据，填写的文件名为{1}\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);
                    }

                    errorString = errorStringBuild.ToString();
                    return false;
                }
                else
                {
                    errorString = null;
                    return true;
                }
            }
        }
        else
        {
            errorString = string.Format("文件存在性检查定义只能用于string型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }
    }

    /// <summary>
    /// 用于int、long、float或string型取值必须在另一字段（可能还是这张表格也可能跨表）中有对应值的检查
    /// </summary>
    public static bool CheckRef(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 首先要求字段类型只能为int、long、float或string型
        if (!(fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String))
        {
            errorString = string.Format("值引用检查规则只适用于int、long、float或string类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }
        else
        {
            string tableName;
            string fieldIndexDefine;

            // 解析ref规则中目标列所在表格以及字段名
            const string START_STRING = "ref:";
            if (!checkRule.CheckRuleString.StartsWith(START_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                errorString = string.Format("值引用检查规则声明错误，必须以\"{0}\"开头，后面跟表格名-字段名\n", START_STRING);
                return false;
            }
            else
            {
                string temp = checkRule.CheckRuleString.Substring(START_STRING.Length).Trim();
                if (string.IsNullOrEmpty(temp))
                {
                    errorString = string.Format("值引用检查规则声明错误，\"{0}\"的后面必须跟表格名-字段名\n", START_STRING);
                    return false;
                }
                else
                {
                    // 判断是否在最后以(except{xx,xx})的格式声明无需ref规则检查的特殊值
                    List<object> exceptValues = new List<object>();
                    int leftBracketIndex = temp.IndexOf('(');
                    int rightBracketIndex = temp.LastIndexOf(')');
                    if (leftBracketIndex != -1 && rightBracketIndex > leftBracketIndex)
                    {
                        // 取出括号中的排除值声明
                        const string EXCEPT_DEFINE_START_STRING = "except";
                        string exceptDefineString = temp.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                        if (!exceptDefineString.StartsWith(EXCEPT_DEFINE_START_STRING, StringComparison.CurrentCultureIgnoreCase))
                        {
                            errorString = string.Format("值引用检查规则声明错误，若要声明ref检查所忽略的特殊值，需在最后以(except{xx,xx})的形式声明，而你在括号中声明为\"{0}\"\n", exceptDefineString);
                            return false;
                        }
                        else
                        {
                            // 检查排除值的声明（即有效值声明格式）是否合法
                            string exceptValuesDefine = exceptDefineString.Substring(EXCEPT_DEFINE_START_STRING.Length).Trim();
                            exceptValues = Utils.GetEffectiveValue(exceptValuesDefine, fieldInfo.DataType, out errorString);
                            if (errorString != null)
                            {
                                errorString = string.Format("值引用检查规则声明错误，排除值的声明非法，{0}\n", errorString);
                                return false;
                            }

                            // 将定义字符串去掉except声明部分
                            temp = temp.Substring(0, leftBracketIndex).Trim();
                        }
                    }
                    // 解析参考表名、列名声明
                    FieldInfo targetFieldInfo = null;
                    int hyphenIndex = temp.LastIndexOf('-');
                    if (hyphenIndex == -1)
                    {
                        tableName = temp;
                        fieldIndexDefine = null;
                    }
                    else
                    {
                        tableName = temp.Substring(0, hyphenIndex).Trim();
                        fieldIndexDefine = temp.Substring(hyphenIndex + 1, temp.Length - hyphenIndex - 1);
                    }

                    if (!AppValues.TableInfo.ContainsKey(tableName))
                    {
                        errorString = string.Format("值引用检查规则声明错误，找不到名为{0}的表格\n", START_STRING);
                        return false;
                    }
                    if (string.IsNullOrEmpty(fieldIndexDefine))
                        targetFieldInfo = AppValues.TableInfo[tableName].GetKeyColumnFieldInfo();
                    else
                    {
                        TableInfo targetTableInfo = AppValues.TableInfo[tableName];
                        targetFieldInfo = GetFieldByIndexDefineString(fieldIndexDefine, targetTableInfo, out errorString);
                        if (errorString != null)
                        {
                            errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中无法根据索引字符串\"{1}\"找到要参考的字段，错误信息为：{2}\n", tableName, fieldIndexDefine, errorString);
                            return false;
                        }
                    }
                    // 检查目标字段必须为相同的数据类型
                    if (fieldInfo.DataType != targetFieldInfo.DataType)
                    {
                        errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中通过索引字符串\"{1}\"找到的参考字段的数据类型为{2}，而要检查字段的数据类型为{3}，无法进行不同数据类型字段的引用检查\n", tableName, fieldIndexDefine, targetFieldInfo.DataType.ToString(), fieldInfo.DataType.ToString());
                        return false;
                    }
                    else
                    {
                        List<object> targetFieldData = targetFieldInfo.Data;
                        // 存储找不到引用对应关系的数据信息（key：行号， value：填写的数据）
                        Dictionary<int, object> unreferencedInfo = new Dictionary<int, object>();

                        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
                        {
                            for (int i = 0; i < fieldInfo.Data.Count; ++i)
                            {
                                // 忽略无效集合元素下属子类型的空值或本身为空值
                                if (fieldInfo.Data[i] == null)
                                    continue;
                                // 忽略不进行ref检查的排除值
                                else if (exceptValues.Contains(fieldInfo.Data[i]))
                                    continue;

                                if (!targetFieldData.Contains(fieldInfo.Data[i]))
                                    unreferencedInfo.Add(i, fieldInfo.Data[i]);
                            }
                        }
                        else if (fieldInfo.DataType == DataType.String)
                        {
                            for (int i = 0; i < fieldInfo.Data.Count; ++i)
                            {
                                // 忽略无效集合元素下属子类型的空值以及空字符串
                                if (fieldInfo.Data[i] == null || string.IsNullOrEmpty(fieldInfo.Data[i].ToString()))
                                    continue;
                                // 忽略不进行ref检查的排除值
                                else if (exceptValues.Contains(fieldInfo.Data[i]))
                                    continue;

                                if (!targetFieldData.Contains(fieldInfo.Data[i]))
                                    unreferencedInfo.Add(i, fieldInfo.Data[i]);
                            }
                        }

                        if (unreferencedInfo.Count > 0)
                        {
                            StringBuilder errorStringBuild = new StringBuilder();
                            errorStringBuild.AppendLine("存在以下未找到引用关系的数据行：");
                            foreach (var item in unreferencedInfo)
                                errorStringBuild.AppendFormat("第{0}行数据\"{1}\"在对应参考列不存在\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);

                            errorString = errorStringBuild.ToString();
                            return false;
                        }
                        else
                        {
                            errorString = null;
                            return true;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 用于将指定的字段用自定义函数进行检查
    /// </summary>
    public static bool CheckFunc(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 提取func:后声明的自定义函数名
        const string START_STRING = "func:";
        if (!checkRule.CheckRuleString.StartsWith(START_STRING, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = string.Format("自定义函数检查规则声明错误，必须以\"{0}\"开头，后面跟MyCheckFunction.cs中声明的函数名\n", START_STRING);
            return false;
        }
        else
        {
            string funcName = checkRule.CheckRuleString.Substring(START_STRING.Length, checkRule.CheckRuleString.Length - START_STRING.Length).Trim();
            Type myCheckFunctionClassType = typeof(MyCheckFunction);
            if (myCheckFunctionClassType != null)
            {
                MethodInfo dynMethod = myCheckFunctionClassType.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(FieldInfo), typeof(string).MakeByRefType() }, null);
                if (dynMethod == null)
                {
                    errorString = string.Format("自定义函数检查规则声明错误，{0}.cs中找不到符合要求的名为\"{1}\"的函数，函数必须形如public static bool funcName(FieldInfo fieldInfo, out string errorString)\n", myCheckFunctionClassType.Name, funcName);
                    return false;
                }
                else
                {
                    errorString = null;
                    object[] inputParams = new object[] { fieldInfo, errorString };
                    bool checkResult = true;
                    try
                    {
                        checkResult = (bool)dynMethod.Invoke(null, inputParams);
                    }
                    catch (Exception exception)
                    {
                        errorString = string.Format("运行自定义检查函数{0}错误，请修正代码后重试\n{1}", funcName, exception);
                        return false;
                    }
                    if (inputParams[1] != null)
                        errorString = inputParams[1].ToString();

                    if (checkResult == true)
                        return true;
                    else
                    {
                        errorString = string.Format("未通过自定义函数规则检查，存在以下错误：\n{0}\n", errorString);
                        return false;
                    }
                }
            }
            else
            {
                errorString = string.Format("自定义函数检查规则无法使用，找不到{0}类\n", myCheckFunctionClassType.Name);
                return false;
            }
        }
    }

    /// <summary>
    /// 用于将整张表格用自定义函数进行检查
    /// </summary>
    public static bool CheckTableFunc(TableInfo tableInfo, out string errorString)
    {
        if (tableInfo.TableConfig == null || !tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_CHECK_TABLE))
        {
            errorString = null;
            return true;
        }
        List<string> checkTableFuncNames = tableInfo.TableConfig[AppValues.CONFIG_NAME_CHECK_TABLE];
        if (checkTableFuncNames.Count < 1)
        {
            errorString = null;
            Utils.LogWarning(string.Format("警告：表格{0}中声明了整表检查参数但没有配置任何检查函数，请确认是否遗忘", tableInfo.TableName));
            return true;
        }
        Type myCheckFunctionClassType = typeof(MyCheckFunction);
        if (myCheckFunctionClassType == null)
        {
            errorString = string.Format("自定义函数检查规则无法使用，找不到{0}类\n", myCheckFunctionClassType.Name);
            return false;
        }
        StringBuilder errorStringBuilder = new StringBuilder();
        foreach (string funcName in checkTableFuncNames)
        {
            MethodInfo dynMethod = myCheckFunctionClassType.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(TableInfo), typeof(string).MakeByRefType() }, null);
            if (dynMethod == null)
            {
                errorStringBuilder.AppendFormat("自定义整表函数检查规则声明错误，{0}.cs中找不到符合要求的名为\"{1}\"的函数，函数必须形如public static bool funcName(TableInfo tableInfo, out string errorString)\n", myCheckFunctionClassType.Name, funcName);
                continue;
            }
            else
            {
                string tempErrorString = null;
                object[] inputParams = new object[] { tableInfo, tempErrorString };
                bool checkResult = true;
                try
                {
                    checkResult = (bool)dynMethod.Invoke(null, inputParams);
                }
                catch (Exception exception)
                {
                    errorString = string.Format("运行自定义整表检查函数{0}错误，请修正代码后重试\n{1}", funcName, exception);
                    return false;
                }
                if (inputParams[1] != null)
                    tempErrorString = inputParams[1].ToString();

                if (checkResult == false)
                    errorStringBuilder.AppendFormat("未通过自定义整表检查函数{0}的检查，存在以下错误：\n{1}\n", funcName, tempErrorString);
            }
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 用于检查按自定义索引字段导出lua文件的表格中相关索引字段的数据完整性
    /// </summary>
    public static bool CheckTableIntegrity(List<FieldInfo> indexField, Dictionary<object, object> data, List<string> integrityCheckRules, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();

        // 解析各个需要进行数据完整性检查的字段声明的有效值
        List<List<object>> effectiveValues = new List<List<object>>();
        for (int i = 0; i < integrityCheckRules.Count; ++i)
        {
            if (integrityCheckRules[i] == null)
                effectiveValues.Add(null);
            else
            {
                List<object> oneFieldEffectiveValues = Utils.GetEffectiveValue(integrityCheckRules[i], indexField[i].DataType, out errorString);
                if (errorString != null)
                {
                    errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）的数据完整性检查规则定义错误，{2}\n", indexField[i].FieldName, Utils.GetExcelColumnName(indexField[i].ColumnSeq + 1), errorString);
                    errorString = null;
                }
                else
                    effectiveValues.Add(oneFieldEffectiveValues);
            }
        }
        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
            errorString = null;
        else
            return false;

        // 进行数据完整性检查
        List<object> parentKeys = new List<object>();
        for (int i = 0; i < integrityCheckRules.Count; ++i)
            parentKeys.Add(null);

        int currentLevel = 0;
        _CheckIntegrity(indexField, data, parentKeys, effectiveValues, ref currentLevel, errorStringBuilder);
        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 用于递归对表格进行数据完整性检查
    /// </summary>
    public static void _CheckIntegrity(List<FieldInfo> indexField, Dictionary<object, object> parentDict, List<object> parentKeys, List<List<object>> effectiveValues, ref int currentLevel, StringBuilder errorStringBuilder)
    {
        if (effectiveValues[currentLevel] != null)
        {
            List<object> inputData = new List<object>(parentDict.Keys);
            foreach (object value in effectiveValues[currentLevel])
            {
                if (!inputData.Contains(value))
                {
                    if (currentLevel > 0)
                    {
                        StringBuilder parentKeyInfoBuilder = new StringBuilder();
                        for (int i = 0; i <= currentLevel - 1; ++i)
                            parentKeyInfoBuilder.AppendFormat("{0}={1},", indexField[i].FieldName, parentKeys[i]);

                        string parentKeyInfo = parentKeyInfoBuilder.ToString().Substring(0, parentKeyInfoBuilder.Length - 1);
                        errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）缺少在{2}情况下值为\"{3}\"的数据\n", indexField[currentLevel].FieldName, Utils.GetExcelColumnName(indexField[currentLevel].ColumnSeq + 1), parentKeyInfo, value);
                    }
                    else
                        errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）缺少值为\"{2}\"的数据\n", indexField[currentLevel].FieldName, Utils.GetExcelColumnName(indexField[currentLevel].ColumnSeq + 1), value);
                }
            }
        }

        if (currentLevel < effectiveValues.Count - 1)
        {
            foreach (var key in parentDict.Keys)
            {
                parentKeys[currentLevel] = key;
                ++currentLevel;
                _CheckIntegrity(indexField, (Dictionary<object, object>)(parentDict[key]), parentKeys, effectiveValues, ref currentLevel, errorStringBuilder);
                --currentLevel;
            }
        }
    }

    /// <summary>
    /// 检查字段名是否合法，要求必须以英文字母开头，只能为英文字母、数字或下划线，且不能为空或纯空格
    /// </summary>
    public static bool CheckFieldName(string fieldName, out string errorString)
    {
        if (string.IsNullOrEmpty(fieldName.Trim()))
        {
            errorString = "不能为空或纯空格";
            return false;
        }
        char firstLetter = fieldName[0];
        if (!((firstLetter >= 'a' && firstLetter <= 'z') || (firstLetter >= 'A' && firstLetter <= 'Z')))
        {
            errorString = string.Format("{0}不合法，必须以英文字母开头", fieldName);
            return false;
        }
        foreach (char c in fieldName)
        {
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_'))
            {
                errorString = string.Format("{0}不合法，含有非法字符\"{1}\"，只能由英文字母、数字或下划线组成", fieldName, c);
                return false;
            }
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查date型的输入格式定义
    /// </summary>
    public static bool CheckDateInputDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        DateFormatType formatType = TableAnalyzeHelper.GetDateFormatType(defineString);
        if (!(formatType == DateFormatType.FormatString || formatType == DateFormatType.ReferenceDateMsec || formatType == DateFormatType.ReferenceDateSec))
        {
            errorString = "不属于合法的date型输入格式类型";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查date型导出至lua文件的格式定义
    /// </summary>
    public static bool CheckDateToLuaDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查date型导出至MySQL数据库的格式定义
    /// </summary>
    public static bool CheckDateToDatabaseDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        DateFormatType formatType = TableAnalyzeHelper.GetDateFormatType(defineString);
        if (!(formatType == DateFormatType.FormatString || formatType == DateFormatType.ReferenceDateMsec || formatType == DateFormatType.ReferenceDateSec))
        {
            errorString = "不属于合法的date型导出至MySQL数据库的格式类型";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查time型的格式定义
    /// </summary>
    public static bool CheckTimeDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        TimeFormatType formatType = TableAnalyzeHelper.GetTimeFormatType(defineString);
        if (formatType == TimeFormatType.FormatString)
        {
            // 检查time型的格式字符串声明，不允许出现代表年月日的y、M、d
            List<string> errorInfo = new List<string>();
            if (defineString.IndexOf('y') != -1)
                errorInfo.Add("代表年的y");
            if (defineString.IndexOf('M') != -1)
                errorInfo.Add("代表月的M");
            if (defineString.IndexOf('d') != -1)
                errorInfo.Add("代表日的d");

            if (errorInfo.Count > 0)
            {
                errorString = string.Format("time类型的格式定义中不允许出现以下与年月日相关的日期型格式定义字符：{0}", Utils.CombineString(errorInfo, "，"));
                return false;
            }
        }

        errorString = null;
        return true;
    }
}

/// <summary>
/// 表格检查规则类型
/// </summary>
public enum TableCheckType
{
    Invalid,

    Range,        // 数值范围检查
    Effective,    // 值有效性检查（填写值必须是几个合法值中的一个）
    Illegal,      // 非法值检查（填写值不允许为几个非法值中的一个）
    NotEmpty,     // 值非空检查
    Unique,       // 值唯一性检查
    Ref,          // 值引用检查（某个数值必须为另一个表格中某字段中存在的值）
    GreaterThan,  // 值大小比较检查（同一行中某个字段的值必须大于另一字段的值）
    Func,         // 自定义检查函数
    File,         // 文件存在性检查
}

public struct FieldCheckRule
{
    public TableCheckType CheckType;
    public string CheckRuleString;
}
