using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

public class MapStringCheckHelper
{
    /// <summary>
    /// 用于检查mapString型当逻辑上某列对应不同的类型取值时，其数据按是否要求含有或不允许有某些元素
    /// </summary>
    public static bool CheckMapString(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        if (fieldInfo.DataType != DataType.MapString)
        {
            errorString = string.Format("mapString型的内容检查只适用于mapString类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }

        MapStringCheckRule mapStringCheckRule = new MapStringCheckRule();

        // 解析检查规则
        const string CHECK_RULE_START_STRING = "mapString:";
        string checkRuleString = null;
        if (checkRule.CheckRuleString.Equals(CHECK_RULE_START_STRING, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = "mapString型的内容检查规则声明中必须含有具体的检查规则\n";
            return false;
        }
        else if (!checkRule.CheckRuleString.StartsWith(CHECK_RULE_START_STRING, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = string.Format("mapString型的内容检查规则声明错误，必须以\"{0}\"开头\n", CHECK_RULE_START_STRING);
            return false;
        }
        else
            checkRuleString = checkRule.CheckRuleString.Substring(CHECK_RULE_START_STRING.Length).Trim();

        // 通过|分隔不同条件下的内容检查规则
        string[] checkRuleList = checkRuleString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        // 解析每一种条件下对应的mapString内容
        for (int i = 0; i < checkRuleList.Length; ++i)
        {
            // 条件解析
            MapStringCondition mapStringCondition = new MapStringCondition();

            string oneCheckRule = checkRuleList[i].Trim();
            string tempCheckRule = oneCheckRule;
            if (string.IsNullOrEmpty(oneCheckRule))
            {
                errorString = "mapString型的内容检查规则声明错误，不允许含有空的规则声明，请检查是否含有多余的|分隔符\n";
                return false;
            }

            const string IF_CONDITION_START_STRING = "if(";
            if (oneCheckRule.Equals(IF_CONDITION_START_STRING, StringComparison.CurrentCultureIgnoreCase) || !oneCheckRule.StartsWith(IF_CONDITION_START_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                errorString = string.Format("mapString型的内容检查规则声明错误，必须在if后面的括号中声明其他字段需满足的条件。若无要求，请填写为\"if(all)\"，你填写的为{0}\n", oneCheckRule);
                return false;
            }
            tempCheckRule = tempCheckRule.Substring(IF_CONDITION_START_STRING.Length);
            int rightBracket = tempCheckRule.IndexOf(')');
            if (rightBracket == -1)
            {
                errorString = string.Format("mapString型的内容检查规则声明错误，if后面的右括号缺失，你填写的为{0}\n", oneCheckRule);
                return false;
            }
            if (rightBracket == tempCheckRule.Length - 1)
            {
                errorString = string.Format("mapString型的内容检查规则声明错误，在if后面的括号中声明其他字段需满足的条件之后，还需在方括号内声明对mapString型下属字段的要求\n", oneCheckRule);
                return false;
            }
            string ifConditionString = tempCheckRule.Substring(0, rightBracket).Trim();
            if (string.IsNullOrEmpty(ifConditionString))
            {
                errorString = "mapString型的内容检查规则声明错误，if后的括号中为空，若要设置为在任何条件下，请填写为\"if(all)\"\n";
                return false;
            }
            else if ("all".Equals(ifConditionString, StringComparison.CurrentCultureIgnoreCase))
            {
                Condition condition = new Condition();
                condition.FieldInfo = null;

                mapStringCondition.ConditionList.Add(condition);
            }
            else
            {
                // 通过英文逗号分隔要同时满足的条件
                string[] ifConditionStringList = ifConditionString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < ifConditionStringList.Length; j++)
                {
                    Condition condition = new Condition();

                    string oneKeyValuePairString = ifConditionStringList[j].Trim();
                    string fieldName = null;
                    StringBuilder fieldNameBuilder = new StringBuilder();
                    int charIndex = 0;
                    bool isFoundSignOfRelation = false;
                    for (charIndex = 0; charIndex < oneKeyValuePairString.Length; ++charIndex)
                    {
                        char c = oneKeyValuePairString[charIndex];
                        if (c == '=' || c == '>' || c == '<')
                        {
                            fieldName = fieldNameBuilder.ToString().Trim();
                            isFoundSignOfRelation = true;
                            break;
                        }
                        else
                            fieldNameBuilder.Append(c);
                    }
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，if后的括号中（{0}）未声明字段名，请按字段名、关系符、取值的形式进行声明，如type>1\n", oneKeyValuePairString);
                        return false;
                    }
                    else if (isFoundSignOfRelation == false)
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，if后的括号中（{0}）未声明关系符，请按字段名、关系符、取值的形式进行声明，如type>1\n", oneKeyValuePairString);
                        return false;
                    }
                    // 检查字段是否存在
                    FieldInfo targetFieldInfo = TableCheckHelper.GetFieldByIndexDefineString(fieldName, AppValues.TableInfo[fieldInfo.TableName], out errorString);
                    if (errorString != null)
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，无法根据索引字符串\"{0}\"在表格{1}找到要对应的字段，错误信息为：{2}\n", fieldName, fieldInfo.TableName, errorString);
                        return false;
                    }
                    // 检查字段类型是否符合要求
                    if (targetFieldInfo.DataType != DataType.Int && targetFieldInfo.DataType != DataType.Long && targetFieldInfo.DataType != DataType.Float && targetFieldInfo.DataType != DataType.Bool && targetFieldInfo.DataType != DataType.String)
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）的数据类型为{1}，而mapString型内容检查规则中，if条件字段只能为int、long、float、bool或string型\n", fieldName, targetFieldInfo.DataType);
                        return false;
                    }
                    condition.FieldInfo = targetFieldInfo;

                    // 解析填写的关系符
                    string signOfRelation = null;
                    StringBuilder signOfRelationBuilder = new StringBuilder();
                    if (charIndex == oneKeyValuePairString.Length)
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，if后的括号中（{0}）未声明取值，请按字段名、关系符、取值的形式进行声明，如type>1\n", oneKeyValuePairString);
                        return false;
                    }
                    for (; charIndex < oneKeyValuePairString.Length; ++charIndex)
                    {
                        char c = oneKeyValuePairString[charIndex];
                        if (c == '=' || c == '>' || c == '<')
                            signOfRelationBuilder.Append(c);
                        else
                            break;
                    }
                    signOfRelation = signOfRelationBuilder.ToString();
                    if (signOfRelation == "=")
                        condition.Relation = Relation.Equal;
                    else if (signOfRelation == ">")
                        condition.Relation = Relation.GreaterThan;
                    else if (signOfRelation == ">=")
                        condition.Relation = Relation.GreaterThanOrEqual;
                    else if (signOfRelation == "<")
                        condition.Relation = Relation.LessThan;
                    else if (signOfRelation == "<=")
                        condition.Relation = Relation.LessThanOrEqual;
                    else
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，关系符非法，只支持=、>、>=、<、<=，你填写的为{0}\n", oneKeyValuePairString);
                        return false;
                    }
                    // bool、string型只有关系符=
                    if (condition.Relation != Relation.Equal && (targetFieldInfo.DataType == DataType.Bool || targetFieldInfo.DataType == DataType.String))
                    {
                        errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）为{1}类型，只能进行等于判定，而你设置的关系符为{2}\n", fieldName, targetFieldInfo.DataType, condition.Relation);
                        return false;
                    }

                    // 解析填写的取值
                    string valueString = oneKeyValuePairString.Substring(charIndex).Trim();
                    if (targetFieldInfo.DataType == DataType.Int)
                    {
                        int value = 0;
                        if (int.TryParse(valueString, out value) == false)
                        {
                            errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）对应int型的取值（{1}）非法\n", fieldName, valueString);
                            return false;
                        }
                        else
                            condition.Value = value;
                    }
                    else if (targetFieldInfo.DataType == DataType.Long)
                    {
                        long value = 0;
                        if (long.TryParse(valueString, out value) == false)
                        {
                            errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）对应long型的取值（{1}）非法\n", fieldName, valueString);
                            return false;
                        }
                        else
                            condition.Value = value;
                    }
                    else if (targetFieldInfo.DataType == DataType.Float)
                    {
                        double value = 0;
                        if (double.TryParse(valueString, out value) == false)
                        {
                            errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）对应float型的取值（{1}）非法\n", fieldName, valueString);
                            return false;
                        }
                        else
                            condition.Value = value;
                    }
                    else if (targetFieldInfo.DataType == DataType.Bool)
                    {
                        if ("1".Equals(valueString) || "true".Equals(valueString, StringComparison.CurrentCultureIgnoreCase))
                            condition.Value = true;
                        else if ("0".Equals(valueString) || "false".Equals(valueString, StringComparison.CurrentCultureIgnoreCase))
                            condition.Value = true;
                        else
                        {
                            errorString = string.Format("mapString型的内容检查规则声明错误，条件字段（{0}）对应bool型的取值（{1}）非法，bool型的值应用数字1、0或true、false进行声明\n", fieldName, valueString);
                            return false;
                        }
                    }
                    else if (targetFieldInfo.DataType == DataType.String)
                        condition.Value = valueString;
                    else
                    {
                        errorString = "用CheckMapString函数处理非法的mapString型检查规则中定义的条件字段类型";
                        Utils.LogErrorAndExit(errorString);
                        return false;
                    }

                    mapStringCondition.ConditionList.Add(condition);
                }
            }

            // mapString型下属字段要求解析
            string mapStringRequiredString = tempCheckRule.Substring(rightBracket + 1).Trim();
            if (!mapStringRequiredString.StartsWith("[") || !mapStringRequiredString.EndsWith("]"))
            {
                errorString = string.Format("mapString型的内容检查规则声明错误，每条检查规则中的字段要求声明必须在if条件声明后，在方括号内声明，你填写的为{0}\n", mapStringRequiredString);
                return false;
            }
            mapStringRequiredString = mapStringRequiredString.Substring(1, mapStringRequiredString.Length - 2).Trim();
            MapStringRequiredInfo mapStringRequiredInfo = _GetMapStringRequiredInfo(mapStringRequiredString, fieldInfo, out errorString);
            if (errorString != null)
            {
                errorString = string.Format("mapString型的内容检查规则声明错误，输入的字段要求错误，你填写的为\"{0}\"，错误原因为：{1}\n", mapStringRequiredString, errorString);
                return false;
            }
            else
                mapStringCheckRule.CheckRuleList.Add(mapStringCondition, mapStringRequiredInfo);
        }

        // 按解析出的检查规则对mapString型进行检查
        StringBuilder errorStringBuilder = new StringBuilder();
        int ruleIndex = 0;
        foreach (var item in mapStringCheckRule.CheckRuleList)
        {
            MapStringCondition condition = item.Key;
            MapStringRequiredInfo requiredInfo = item.Value;
            StringBuilder oneConditionStringBuilder = new StringBuilder();

            // 先找出其他列满足检查条件的行
            // 记录符合检查条件的数据索引值（从0计），这里先将所有数据行加入，然后逐步排除不满足条件的数据行
            List<int> targetDataIndex = new List<int>();
            int dataCount = fieldInfo.Data.Count;
            for (int i = 0; i < dataCount; ++i)
                targetDataIndex.Add(i);

            List<int> emptyConditionRowIndex = new List<int>();
            for (int conditionIndex = 0; conditionIndex < condition.ConditionList.Count; ++conditionIndex)
            {
                Condition oneCondition = condition.ConditionList[conditionIndex];
                // 排除标为all的条件
                if (oneCondition.FieldInfo != null)
                {
                    List<int> tempEmptyConditionRowIndex = null;
                    _GetTargetDataIndex(oneCondition, targetDataIndex, oneCondition.FieldInfo, out tempEmptyConditionRowIndex);
                    if (tempEmptyConditionRowIndex != null && tempEmptyConditionRowIndex.Count > 0)
                    {
                        foreach (int rowIndex in tempEmptyConditionRowIndex)
                        {
                            if (!emptyConditionRowIndex.Contains(rowIndex))
                                emptyConditionRowIndex.Add(rowIndex);
                        }
                    }
                }

            }
            if (emptyConditionRowIndex.Count > 0)
            {
                string warningString = string.Format("警告：mapString型字段\"{0}\"（列号：{1}）的检查条件（{2}）中的字段，因为以下行中数据无效，视为不满足条件，不对对应行中的mapString进行检查：{3}\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), checkRuleList[ruleIndex], Utils.CombineString(emptyConditionRowIndex, ","));
                Utils.LogWarning(warningString);
            }

            // 对满足检查条件的数据行进行mapString内容检查
            List<int> emptyDataRowIndex = new List<int>();
            foreach (int index in targetDataIndex)
            {
                // 若为空值，跳过检查，但进行警告
                if (fieldInfo.Data[index] == null)
                {
                    emptyDataRowIndex.Add(index + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    continue;
                }

                JsonData jsonData = fieldInfo.Data[index] as JsonData;
                if (_CheckMapStringData(jsonData, requiredInfo, out errorString) == false)
                    oneConditionStringBuilder.AppendFormat("第{0}行填写的数据（{1}）：\n{2}\n", index + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.JsonString[index], errorString);
            }
            string oneConditionString = oneConditionStringBuilder.ToString();
            if (!string.IsNullOrEmpty(oneConditionString))
                errorStringBuilder.AppendFormat("以下行数据未通过检查规则\"{0}\"：\n{1}", checkRuleList[ruleIndex], oneConditionString);

            if (emptyDataRowIndex.Count > 0)
            {
                string warningString = string.Format("警告：在对mapString型字段\"{0}\"（列号：{1}）执行检查条件（{2}）时，因为以下行中数据无效，跳过对mapString的检查：{3}\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), checkRuleList[ruleIndex], Utils.CombineString(emptyDataRowIndex, ","));
                Utils.LogWarning(warningString);
            }

            ++ruleIndex;
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

    private static void _GetTargetDataIndex(Condition condition, List<int> targetDataIndex, FieldInfo fieldInfo, out List<int> emptyRowIndex)
    {
        emptyRowIndex = new List<int>();
        DataType dataType = condition.FieldInfo.DataType;
        int dataCount = fieldInfo.Data.Count;
        if (dataType == DataType.Int)
        {
            int conditionValue = (int)condition.Value;
            for (int i = 0; i < dataCount; ++i)
            {
                // 若为空值，视为不满足此条件，但进行警告
                if (fieldInfo.Data[i] == null)
                {
                    emptyRowIndex.Add(i + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);

                    continue;
                }

                if (condition.Relation == Relation.Equal)
                {
                    if ((int)fieldInfo.Data[i] != conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThan)
                {
                    if ((int)fieldInfo.Data[i] <= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThanOrEqual)
                {
                    if ((int)fieldInfo.Data[i] < conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThan)
                {
                    if ((int)fieldInfo.Data[i] >= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThanOrEqual)
                {
                    if ((int)fieldInfo.Data[i] > conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
            }
        }
        else if (dataType == DataType.Long)
        {
            long conditionValue = (long)condition.Value;
            for (int i = 0; i < dataCount; ++i)
            {
                // 若为空值，视为不满足此条件，但进行警告
                if (fieldInfo.Data[i] == null)
                {
                    emptyRowIndex.Add(i + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);

                    continue;
                }

                if (condition.Relation == Relation.Equal)
                {
                    if ((long)fieldInfo.Data[i] != conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThan)
                {
                    if ((long)fieldInfo.Data[i] <= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThanOrEqual)
                {
                    if ((long)fieldInfo.Data[i] < conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThan)
                {
                    if ((long)fieldInfo.Data[i] >= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThanOrEqual)
                {
                    if ((long)fieldInfo.Data[i] > conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
            }
        }
        else if (dataType == DataType.Float)
        {
            double conditionValue = (double)condition.Value;
            for (int i = 0; i < dataCount; ++i)
            {
                // 若为空值，视为不满足此条件，但进行警告
                if (fieldInfo.Data[i] == null)
                {
                    emptyRowIndex.Add(i + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);

                    continue;
                }

                if (condition.Relation == Relation.Equal)
                {
                    if ((double)fieldInfo.Data[i] != conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThan)
                {
                    if ((double)fieldInfo.Data[i] <= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.GreaterThanOrEqual)
                {
                    if ((double)fieldInfo.Data[i] < conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThan)
                {
                    if ((double)fieldInfo.Data[i] >= conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
                else if (condition.Relation == Relation.LessThanOrEqual)
                {
                    if ((double)fieldInfo.Data[i] > conditionValue)
                    {
                        int dataIndex = Utils.binarySearch(targetDataIndex, i);
                        if (dataIndex != -1)
                            targetDataIndex.RemoveAt(dataIndex);
                    }
                }
            }
        }
        else if (dataType == DataType.Bool)
        {
            bool conditionValue = (bool)condition.Value;
            for (int i = 0; i < dataCount; ++i)
            {
                // 若为空值，视为不满足此条件，但进行警告
                if (fieldInfo.Data[i] == null)
                {
                    emptyRowIndex.Add(i + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);

                    continue;
                }

                // bool、string型只有相等判定
                if ((bool)fieldInfo.Data[i] != conditionValue)
                {
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);
                }
            }
        }
        else if (dataType == DataType.String)
        {
            string conditionValue = (string)condition.Value;
            for (int i = 0; i < dataCount; ++i)
            {
                // 若为空值，视为不满足此条件，但进行警告
                if (fieldInfo.Data[i] == null)
                {
                    emptyRowIndex.Add(i + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);

                    continue;
                }

                // bool、string型只有相等判定
                if ((string)fieldInfo.Data[i] != conditionValue)
                {
                    int dataIndex = Utils.binarySearch(targetDataIndex, i);
                    if (dataIndex != -1)
                        targetDataIndex.RemoveAt(dataIndex);
                }
            }
        }
    }

    private static bool _CheckMapStringData(JsonData jsonData, MapStringRequiredInfo requiredInfo, out string errorString)
    {
        List<string> errorStringList = new List<string>();
        foreach (MapStringParamRequiredInfo paramRequiredInfo in requiredInfo.ParamRequiredInfo.Values)
        {
            if (_CheckMapStringParamData(jsonData, paramRequiredInfo, out errorString) == false)
                errorStringList.Add(errorString);
        }

        if (errorStringList.Count == 0)
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = Utils.CombineString(errorStringList, "\n");
            return false;
        }
    }

    private static bool _CheckMapStringParamData(JsonData jsonData, MapStringParamRequiredInfo paramRequiredInfo, out string errorString)
    {
        List<string> errorStringList = new List<string>();
        if (paramRequiredInfo.MapStringRequiredInfo == null)
        {
            if (paramRequiredInfo.ParamRequired == ParamRequired.MustHave)
            {
                if (jsonData.Keys.Contains(paramRequiredInfo.ParamName) == false)
                    errorStringList.Add(string.Format("要求必须存在的参数{0}未声明", paramRequiredInfo.ParamName));
            }
            else if (paramRequiredInfo.ParamRequired == ParamRequired.NotAllowed)
            {
                if (jsonData.Keys.Contains(paramRequiredInfo.ParamName) == true)
                    errorStringList.Add(string.Format("不允许存在的参数{0}被声明了", paramRequiredInfo.ParamName));
            }
        }
        else
        {
            if (jsonData.Keys.Contains(paramRequiredInfo.ParamName))
            {
                if (_CheckMapStringData(jsonData[paramRequiredInfo.ParamName], paramRequiredInfo.MapStringRequiredInfo, out errorString) == false)
                    errorStringList.Add(string.Format("参数{0}下属map未通过检查：{1}", paramRequiredInfo.ParamName, errorString));
            }
            else
                errorStringList.Add(string.Format("规则要求对mapString型参数{0}下属的子元素进行检查，而输入数据中不含此参数", paramRequiredInfo.ParamName));
        }

        if (errorStringList.Count == 0)
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = Utils.CombineString(errorStringList, "；");
            return false;
        }
    }

    private static MapStringRequiredInfo _GetMapStringRequiredInfo(string defineString, FieldInfo fieldInfo, out string errorString)
    {
        // 对定义字符串进行词法解析，得到解析后的token列表
        List<MapStringToken> tokenList = _AnalyzeMapStringRequiredInfo(defineString, out errorString);
        if (errorString != null)
        {
            errorString = string.Concat("定义错误：", errorString);
            return null;
        }

        if (tokenList.Count == 0)
        {
            errorString = "定义中未含有任何元素声明";
            return null;
        }

        // 因为整个定义相当于一个map，故将上面得到的token列表首尾加上map开始和结束标记
        tokenList.Insert(0, new MapStringToken(MapStringTokenType.StartMap, "("));
        tokenList.Add(new MapStringToken(MapStringTokenType.EndMap, ")"));

        MapStringCheckRuleParser parser = new MapStringCheckRuleParser();
        MapStringRequiredInfo requiredInfo = parser.GetMapStringCheckRule(tokenList, fieldInfo.MapStringFormatDefine, out errorString);
        if (errorString == null)
            return requiredInfo;
        else
        {
            errorString = string.Concat("定义错误：", errorString);
            return null;
        }
    }

    private static List<MapStringToken> _AnalyzeMapStringRequiredInfo(string defineString, out string errorString)
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
                errorString = "定义字符串中括号不匹配，存在\")\"没有前面与之对应的\"(\"";
                return null;
            }
        }
        if (leftBracketCount > 0)
        {
            errorString = string.Format("定义字符串中括号不匹配，有{0}个\"(\"没有与之对应的\")\"", leftBracketCount);
            return null;
        }

        List<MapStringToken> tokenList = new List<MapStringToken>();
        int nextCharIndex = 0;
        while (nextCharIndex < defineString.Length)
        {
            char c = defineString[nextCharIndex];
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
            else if (char.IsLetter(c) || c == '_')
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                ++nextCharIndex;
                while (nextCharIndex < defineString.Length)
                {
                    char temp = defineString[nextCharIndex];
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
                if (nextCharIndex == defineString.Length)
                    tokenList.Add(new MapStringToken(MapStringTokenType.String, stringBuilder.ToString()));
            }
            else if (char.IsNumber(c))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                ++nextCharIndex;
                while (nextCharIndex < defineString.Length)
                {
                    char temp = defineString[nextCharIndex];
                    if (char.IsNumber(temp))
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
                if (nextCharIndex == defineString.Length)
                    tokenList.Add(new MapStringToken(MapStringTokenType.Number, stringBuilder.ToString()));
            }
            else
            {
                errorString = string.Format("定义字符串中的\"{1}\"非法", c);
                return null;
            }
        }

        errorString = null;
        return tokenList;
    }
}

public class MapStringCheckRuleParser
{
    private Queue<MapStringToken> _tokenQueue = null;
    private MapStringInfo _formatDefine = null;

    public MapStringRequiredInfo GetMapStringCheckRule(List<MapStringToken> tokenList, MapStringInfo formatDefine, out string errorString)
    {
        _tokenQueue = new Queue<MapStringToken>(tokenList);
        _formatDefine = formatDefine;

        MapStringRequiredInfo requiredInfo = _GetMap(out errorString);
        if (errorString == null)
            return requiredInfo;
        else
            return null;
    }

    private MapStringRequiredInfo _GetMap(out string errorString)
    {
        MapStringRequiredInfo requiredInfo = new MapStringRequiredInfo();

        // 跳过map结构开始的{
        _tokenQueue.Dequeue();

        _GetMapElement(requiredInfo, _formatDefine, out errorString);
        if (errorString == null)
            return requiredInfo;
        else
        {
            errorString = string.Concat("解析map中的子元素错误：", errorString);
            return null;
        }
    }

    private void _GetMapElement(MapStringRequiredInfo requiredInfo, MapStringInfo formatDefine, out string errorString)
    {
        // 解析key
        if (_tokenQueue.Count == 0)
        {
            errorString = "定义不完整";
            return;
        }
        MapStringToken keyToken = _tokenQueue.Dequeue();
        if (keyToken.TokenType != MapStringTokenType.String)
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
        if (requiredInfo.ParamRequiredInfo.ContainsKey(key))
        {
            errorString = string.Format("定义中存在相同的key名（{0}）", key);
            return;
        }

        // 判断key名后是否为等号
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("定义不完整，map子元素中的key名（{0}）后缺失等号", key);
            return;
        }
        MapStringToken equalSignToken = _tokenQueue.Dequeue();
        if (equalSignToken.TokenType != MapStringTokenType.EqualSign)
        {
            errorString = string.Concat("map子元素中的key名（{0}）后应为等号，而输入值为{1}", key, equalSignToken.DefineString);
            return;
        }

        // 解析value值
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("定义不完整，map子元素中的key名（{0}）在等号后未声明参数要求，请用1声明为必须存在或用0声明为不允许存在", key);
            return;
        }
        MapStringToken valueToken = _tokenQueue.Dequeue();
        if (valueToken.TokenType == MapStringTokenType.Number)
        {
            if ("1".Equals(valueToken.DefineString))
            {
                MapStringParamRequiredInfo paramRequiredInfo = new MapStringParamRequiredInfo();
                paramRequiredInfo.ParamName = key;
                paramRequiredInfo.ParamRequired = ParamRequired.MustHave;

                requiredInfo.ParamRequiredInfo.Add(key, paramRequiredInfo);
            }
            else if ("0".Equals(valueToken.DefineString))
            {
                MapStringParamRequiredInfo paramRequiredInfo = new MapStringParamRequiredInfo();
                paramRequiredInfo.ParamName = key;
                paramRequiredInfo.ParamRequired = ParamRequired.NotAllowed;

                requiredInfo.ParamRequiredInfo.Add(key, paramRequiredInfo);
            }
            else
            {
                errorString = string.Format("map子元素中的key名（{0}）对应的参数要求非法，输入值为\"{1}\"，请用1或0分别设置该参数为必须含有或不允许含有", keyToken.DefineString);
                return;
            }
        }
        else if (valueToken.TokenType == MapStringTokenType.StartMap)
        {
            if (paramInfo.DataType == DataType.MapString)
            {
                MapStringRequiredInfo childRequiredInfo = new MapStringRequiredInfo();
                _GetMapElement(childRequiredInfo, formatDefine.ParamInfo[key].MapStringInfo, out errorString);
                if (errorString != null)
                {
                    errorString = string.Format("名为{0}的map下属的子元素错误：{1}", key, errorString);
                    return;
                }
                MapStringParamRequiredInfo paramRequiredInfo = new MapStringParamRequiredInfo();
                paramRequiredInfo.ParamName = key;
                paramRequiredInfo.ParamRequired = ParamRequired.None;
                paramRequiredInfo.MapStringRequiredInfo = childRequiredInfo;

                requiredInfo.ParamRequiredInfo.Add(key, paramRequiredInfo);
            }
            else
            {
                errorString = string.Format("mapString定义中要求key名（{0}）对应{1}类型的数据，而输入的检查规则却以mapString型进行设置，请不要对非mapString型使用括号声明", key, paramInfo.DataType);
                return;
            }
        }
        else
        {
            errorString = string.Format("map子元素中的key名（{0}）对应的参数要求非法，输入值为\"{1}\"，请用1或0分别设置该参数为必须含有或不允许含有", keyToken.DefineString);
            return;
        }

        // 解析后面的token
        if (_tokenQueue.Count == 0)
        {
            errorString = string.Format("定义不完整，map下的子元素{0}之后未声明用英文逗号分隔下一个子元素，也没有声明map的结束", key);
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
                errorString = string.Format("定义不完整，map下的子元素{0}之后的英文逗号后未声明下一个元素", key);
                return;
            }
            _GetMapElement(requiredInfo, formatDefine, out errorString);
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
/// mapString型内容检查规则
/// </summary>
public class MapStringCheckRule
{
    public Dictionary<MapStringCondition, MapStringRequiredInfo> CheckRuleList { get; set; }

    public MapStringCheckRule()
    {
        CheckRuleList = new Dictionary<MapStringCondition, MapStringRequiredInfo>();
    }
}

/// <summary>
/// mapString型内容检查规则中其他字段满足的条件
/// </summary>
public class MapStringCondition
{
    public List<Condition> ConditionList { get; set; }

    public MapStringCondition()
    {
        ConditionList = new List<Condition>();
    }
}

public class Condition
{
    // 如果为null表示任何条件下都满足
    public FieldInfo FieldInfo { get; set; }
    public Relation Relation { get; set; }

    public object Value { get; set; }
}

public enum Relation
{
    GreaterThan,
    GreaterThanOrEqual,
    Equal,
    LessThan,
    LessThanOrEqual,
}

public enum ParamRequired
{
    None,
    MustHave,
    NotAllowed,
}

/// <summary>
/// mapString型内容检查规则中mapString下属参数需满足的条件
/// </summary>
public class MapStringRequiredInfo
{
    public Dictionary<string, MapStringParamRequiredInfo> ParamRequiredInfo { get; set; }

    public MapStringRequiredInfo()
    {
        ParamRequiredInfo = new Dictionary<string, MapStringParamRequiredInfo>();
    }
}

public class MapStringParamRequiredInfo
{
    public string ParamName { get; set; }

    public ParamRequired ParamRequired { get; set; }

    public MapStringRequiredInfo MapStringRequiredInfo { get; set; }
}
