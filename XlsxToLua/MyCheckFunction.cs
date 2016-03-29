using System.Collections.Generic;
using System.Text;

/// <summary>
/// 该类定义那些复杂非通用的检查条件所需要手工编写的检查函数
/// 注意：所定义函数必须形如public static bool funcName(FieldInfo fieldInfo, out string errorString)
/// </summary>
public static class MyCheckFunction
{
    /// <summary>
    /// 检查奖励列表是否配置正确，要求字段的数据结构必须为array[dict[3]:n]，定义一种奖励类型的三个int型字段分别叫type、id、count，每个奖励项的类型必须存在，除道具类型之外不允许奖励同一种类型，奖励数量必须大于0，如果是道具类型则道具id在道具表中要存在
    /// </summary>
    public static bool CheckRewardList(FieldInfo fieldInfo, out string errorString)
    {
        // 道具类型对应的type
        int PROP_TYPE;
        string propTypeConfigKey = "propType";
        if (AppValues.ConfigData.ContainsKey(propTypeConfigKey))
        {
            string configValue = AppValues.ConfigData[propTypeConfigKey];
            if (int.TryParse(configValue, out PROP_TYPE) == false)
            {
                errorString = string.Format("config配置中用于表示道具类型对应数字（名为\"{0}\"）的配置项所填值不是合法的数字（你填的为\"{1}\"），无法进行奖励列表字段的检查，请修正配置后再重试\n", propTypeConfigKey, configValue);
                return false;
            }
        }
        else
        {
            errorString = string.Format("config配置文件找不到名为\"{0}\"的表示道具类型对应数字的配置，无法进行奖励列表字段的检查，请填写配置后再重试\n", propTypeConfigKey);
            return false;
        }
        // 合法奖励类型检查规则
        List<FieldCheckRule> CHECK_TYPE_RULES;
        string typeConfigKey = "$type";
        if (AppValues.ConfigData.ContainsKey(typeConfigKey))
        {
            CHECK_TYPE_RULES = TableCheckHelper.GetCheckRules(AppValues.ConfigData[typeConfigKey], out errorString);
            if (errorString != null)
            {
                errorString = string.Format("config文件中用于检查奖励类型是否合法的规则\"{0}\"有误，{1}\n", typeConfigKey, errorString);
                return false;
            }
            if (CHECK_TYPE_RULES == null)
            {
                errorString = string.Format("config文件中用于检查奖励类型是否合法的规则\"{0}\"为空，无法进行奖励列表字段的检查，请填写配置后再重试\n", typeConfigKey, errorString);
                return false;
            }
        }
        else
        {
            errorString = string.Format("config配置文件找不到名为\"{0}\"的表示合法奖励类型对应数字数组的配置，无法进行奖励列表字段的检查，请填写配置后再重试\n", typeConfigKey);
            return false;
        }
        // 读取Prop表的主键id字段，用于填写道具id的值引用检查
        List<object> PROP_KEYS = null;
        string propTableName = "Prop";
        if (AppValues.TableInfo.ContainsKey(propTableName))
            PROP_KEYS = AppValues.TableInfo[propTableName].GetKeyColumnFieldInfo().Data;
        else
        {
            errorString = string.Format("找不到名为\"{0}\"用于配置道具属性的表格，无法进行奖励列表字段的检查\n", propTableName);
            return false;
        }

        // 要求字段的数据结构必须为array[dict[3]:n]
        if (!(fieldInfo.DataType == DataType.Array && fieldInfo.ArrayChildDataType == DataType.Dict))
        {
            errorString = "奖励列表字段的数据结构必须为array[dict[3]:n]";
            return false;
        }
        // 标识组成一个奖励项的三个字段的名称（type、id、count）
        List<string> fieldNames = new List<string>();
        fieldNames.Add("type");
        fieldNames.Add("id");
        fieldNames.Add("count");
        // 要求定义一种奖励类型的三个int型字段分别叫type、id、count
        foreach (FieldInfo childDictField in fieldInfo.ChildField)
        {
            if (childDictField.ChildField.Count != 3)
            {
                errorString = string.Format("一个奖励项的dict必须由type、id、count三个int型字段组成，而你填写的奖励项由{0}个字段组成，出错的dict列号为{1}", childDictField.ChildField.Count, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1));
                return false;
            }
            foreach (FieldInfo childBaseField in childDictField.ChildField)
            {
                if (!fieldNames.Contains(childBaseField.FieldName.ToLower()))
                {
                    errorString = string.Format("一个奖励项的dict必须由type、id、count三个int型字段组成，而你填写的奖励项中含有名为\"{0}\"的字段，出错的dict列号为{1}", childBaseField.FieldName, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1));
                    return false;
                }
                else if (childBaseField.DataType != DataType.Int)
                {
                    errorString = string.Format("一个奖励项的dict必须由type、id、count三个int型字段组成，而你填写的奖励项中的{0}字段的数据类型为{1}，出错的dict列号为{2}", childBaseField.DataType, childBaseField.FieldName, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1));
                    return false;
                }
            }
        }
        // 读取并检查每一行数据是否符合奖励列表的要求
        StringBuilder errorStringBuilder = new StringBuilder();
        for (int i = 0; i < fieldInfo.Data.Count; ++i)
        {
            // 如果本行定义的奖励列表声明为无效，直接跳过
            if ((bool)fieldInfo.Data[i] == false)
                continue;

            // 记录该行已配置的奖励类型（key：type， value：恒为true），但注意道具类型不计入因为允许奖励多种道具类型，只要id不同即可
            Dictionary<int, bool> rewardType = new Dictionary<int, bool>();
            // 记录该行已配置的奖励道具的id（key：道具id， value：恒为true）
            Dictionary<int, bool> rewardPropId = new Dictionary<int, bool>();

            //bool isCorrectForTheLineData = true;
            foreach (FieldInfo childDictField in fieldInfo.ChildField)
            {
                // 一旦读到用-1标识的无效数据，则不再读取array中后面的dict字段，即如果为array[dict[3]:4]并且第二个dict用-1标为无效数据，则认为此行仅配置了1个奖励项，不再读取判断第三四个奖励项是否配置
                if ((bool)childDictField.Data[i] == false)
                    break;

                int type = -1;
                int id = -1;
                int count = -1;
                foreach (FieldInfo field in childDictField.ChildField)
                {
                    if (field.FieldName.Equals("type", System.StringComparison.CurrentCultureIgnoreCase))
                        type = (int)field.Data[i];
                    else if (field.FieldName.Equals("id", System.StringComparison.CurrentCultureIgnoreCase))
                        id = (int)field.Data[i];
                    else if (field.FieldName.Equals("count", System.StringComparison.CurrentCultureIgnoreCase))
                        count = (int)field.Data[i];
                }

                // 对于道具类型，需检查奖励的道具id不能重复且Prop表中要存在该道具id
                if (type == PROP_TYPE)
                {
                    if (rewardPropId.ContainsKey(id))
                        errorStringBuilder.AppendFormat("第{0}行的奖励列表中含有同种道具（id={1}）类型\n", i + AppValues.FIELD_DATA_START_INDEX + 1, id);
                    else
                        rewardPropId.Add(id, true);

                    if (!PROP_KEYS.Contains(id))
                        errorStringBuilder.AppendFormat("第{0}行第{1}列的奖励项中所填的奖励道具（id={2}）在道具表中不存在\n", i + AppValues.FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1), id);
                }
                // 对于非道具类型，需检查奖励类型type不能重复
                else
                {
                    if (rewardType.ContainsKey(type))
                        errorStringBuilder.AppendFormat("第{0}行的奖励列表中含有同种奖励类型（{1}）\n", i + AppValues.FIELD_DATA_START_INDEX + 1, type);
                    else
                        rewardType.Add(type, true);
                }
                // 均要检查奖励count不能低于1
                if (count < 1)
                    errorStringBuilder.AppendFormat("第{0}行第{1}列的奖励项中所填的奖励数量低于1\n", i + AppValues.FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1), type);
            }
        }

        // 按字段检查所有type列所填的类型是否合法
        foreach (FieldInfo childDictField in fieldInfo.ChildField)
        {
            foreach (FieldInfo field in childDictField.ChildField)
            {
                if (field.FieldName.Equals("type", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    TableCheckHelper.CheckByRules(CHECK_TYPE_RULES, field, out errorString);
                    if (errorString != null)
                    {
                        errorStringBuilder.AppendFormat("检查type列发现以下无效type类型：\n{0}", errorString);
                        errorString = null;
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(errorStringBuilder.ToString()))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = errorStringBuilder.ToString();
            return false;
        }
    }
}
