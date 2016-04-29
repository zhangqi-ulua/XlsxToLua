using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 该类定义那些复杂非通用的检查条件所需要手工编写的检查函数
/// 注意：自定义字段检查函数必须形如public static bool funcName(FieldInfo fieldInfo, out string errorString),建议函数名为CheckXxxField
/// 注意：自定义整表检查函数必须形如public static bool funcName(TableInfo tableInfo, out string errorString),建议函数名为CheckXxxTable
/// </summary>
public static class MyCheckFunction
{
    /// <summary>
    /// 检查奖励列表字段是否配置正确，要求字段的数据结构必须为array[dict[3]:n]，定义一种奖励类型的三个int型字段分别叫type、id、count，每个奖励项的类型必须存在，除道具类型之外不允许奖励同一种类型，奖励数量必须大于0，如果是道具类型则道具id在道具表中要存在
    /// </summary>
    public static bool CheckRewardListField(FieldInfo fieldInfo, out string errorString)
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
                        errorStringBuilder.AppendFormat("第{0}行的奖励列表中含有同种道具（id={1}）类型\n", i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, id);
                    else
                        rewardPropId.Add(id, true);

                    if (!PROP_KEYS.Contains(id))
                        errorStringBuilder.AppendFormat("第{0}行第{1}列的奖励项中所填的奖励道具（id={2}）在道具表中不存在\n", i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1), id);
                }
                // 对于非道具类型，需检查奖励类型type不能重复
                else
                {
                    if (rewardType.ContainsKey(type))
                        errorStringBuilder.AppendFormat("第{0}行的奖励列表中含有同种奖励类型（{1}）\n", i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, type);
                    else
                        rewardType.Add(type, true);
                }
                // 均要检查奖励count不能低于1
                if (count < 1)
                    errorStringBuilder.AppendFormat("第{0}行第{1}列的奖励项中所填的奖励数量低于1\n", i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(childDictField.ColumnSeq + 1), type);
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

    /// <summary>
    /// 检查HeroEquipment表，凡是填写的英雄都必须填写在所有品阶下四个槽位可穿戴装备信息
    /// </summary>
    public static bool CheckHeroEquipmentTable(TableInfo tableInfo, out string errorString)
    {
        // 从配置文件中读取英雄的所有品阶（key：品阶， value：恒为true）
        Dictionary<int, bool> HERO_QUALITY = new Dictionary<int, bool>();
        // 每品阶英雄所需穿戴的装备数量
        int HERO_EQUIPMENTCOUNT = 4;

        string heroQualityConfigKey = "$heroQuality";
        if (AppValues.ConfigData.ContainsKey(heroQualityConfigKey))
        {
            string configString = AppValues.ConfigData[heroQualityConfigKey];
            // 去除首尾花括号后，通过英文逗号分隔每个有效值即可
            if (!(configString.StartsWith("{") && configString.EndsWith("}")))
            {
                errorString = string.Format("表示英雄所有品阶的配置{0}错误，必须在首尾用一对花括号包裹整个定义内容，请修正后重试\n", heroQualityConfigKey);
                return false;
            }
            string temp = configString.Substring(1, configString.Length - 2).Trim();
            string[] values = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length < 1)
            {
                errorString = string.Format("表示英雄所有品阶的配置{0}错误，不允许为空值，请修正后重试\n", heroQualityConfigKey);
                return false;
            }

            for (int i = 0; i < values.Length; ++i)
            {
                string oneValueString = values[i].Trim();
                int oneValue;
                if (int.TryParse(oneValueString, out oneValue) == true)
                {
                    if (HERO_QUALITY.ContainsKey(oneValue))
                        Utils.LogWarning(string.Format("警告：表示英雄所有品阶的配置{0}错误，出现了相同的品阶\"{1}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", heroQualityConfigKey, oneValue));
                    else
                        HERO_QUALITY.Add(oneValue, true);
                }
                else
                {
                    errorString = string.Format("表示英雄所有品阶的配置{0}错误，出现了非int型有效值的规则定义，其为\"{1}\"，请修正后重试\n", heroQualityConfigKey, oneValueString);
                    return false;
                }
            }
        }
        else
        {
            errorString = string.Format("config配置文件找不到名为\"{0}\"的表示英雄所有品阶的配置，无法进行HeroEquipment整表的检查，请填写配置后再重试\n", heroQualityConfigKey);
            return false;
        }

        // 获取检查涉及的字段数据
        string HERO_ID_FIELD_NAME = "heroId";
        string HERO_QUALITY_FIELD_NAME = "heroQuality";
        string SEQ_FIELD_NAME = "seq";
        List<object> heroIdList = null;
        List<object> heroQualityList = null;
        List<object> equipmentSeqList = null;
        if (tableInfo.GetFieldInfoByFieldName(HERO_ID_FIELD_NAME) != null)
            heroIdList = tableInfo.GetFieldInfoByFieldName(HERO_ID_FIELD_NAME).Data;
        else
        {
            errorString = string.Format("HeroEquipment表中找不到名为{0}的字段，无法进行整表检查，请修正后重试\n", HERO_ID_FIELD_NAME);
            return false;
        }
        if (tableInfo.GetFieldInfoByFieldName(HERO_QUALITY_FIELD_NAME) != null)
            heroQualityList = tableInfo.GetFieldInfoByFieldName(HERO_QUALITY_FIELD_NAME).Data;
        else
        {
            errorString = string.Format("HeroEquipment表中找不到名为{0}的字段，无法进行整表检查，请修正后重试\n", HERO_QUALITY_FIELD_NAME);
            return false;
        }
        if (tableInfo.GetFieldInfoByFieldName(SEQ_FIELD_NAME) != null)
            equipmentSeqList = tableInfo.GetFieldInfoByFieldName(SEQ_FIELD_NAME).Data;
        else
        {
            errorString = string.Format("HeroEquipment表中找不到名为{0}的字段，无法进行整表检查，请修正后重试\n", SEQ_FIELD_NAME);
            return false;
        }

        // 记录实际填写的信息（key：从外层到内层依次表示heroId、quality、seq，最内层value为从0开始计的数据行序号）
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> inputData = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        int dataCount = tableInfo.GetKeyColumnFieldInfo().Data.Count;
        StringBuilder errorStringBuilder = new StringBuilder();
        for (int i = 0; i < dataCount; ++i)
        {
            int heroId = (int)heroIdList[i];
            int heroQuality = (int)heroQualityList[i];
            int seq = (int)equipmentSeqList[i];
            if (!inputData.ContainsKey(heroId))
                inputData.Add(heroId, new Dictionary<int, Dictionary<int, int>>());

            Dictionary<int, Dictionary<int, int>> qualityInfo = inputData[heroId];
            if (!qualityInfo.ContainsKey(heroQuality))
                qualityInfo.Add(heroQuality, new Dictionary<int, int>());

            Dictionary<int, int> seqInfo = qualityInfo[heroQuality];
            if (seqInfo.ContainsKey(seq))
                errorStringBuilder.AppendFormat("第{0}行与第{1}行完全重复\n", i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, seqInfo[seq] + AppValues.DATA_FIELD_DATA_START_INDEX + 1);
            else
                seqInfo.Add(seq, i);
        }

        string repeatedLineErrorString = errorStringBuilder.ToString();
        if (!string.IsNullOrEmpty(repeatedLineErrorString))
        {
            errorString = string.Format("HeroEquipment表中以下行中的heroId、heroQuality、seq字段与其他行存在完全重复的错误:\n{0}\n", repeatedLineErrorString);
            return false;
        }

        // 检查配置的每个英雄是否都含有所有品阶下四个槽位的可穿戴装备信息
        foreach (var heroInfo in inputData)
        {
            var qualityInfo = heroInfo.Value;
            List<int> qualityList = new List<int>(qualityInfo.Keys);
            List<int> LEGAL_QUALITY_LIST = new List<int>(HERO_QUALITY.Keys);
            foreach (int quality in LEGAL_QUALITY_LIST)
            {
                // 检查是否含有所有品阶
                if (!qualityList.Contains(quality))
                    errorStringBuilder.AppendFormat("英雄（heroId={0}）缺少品质为{1}的装备配置\n", heroInfo.Key, quality);
            }
            // 检查每个品阶下是否配全了四个槽位的装备信息
            foreach (var oneQualityInfo in qualityInfo)
            {
                var seqInfo = oneQualityInfo.Value;
                List<int> seqList = new List<int>(seqInfo.Keys);
                for (int seq = 1; seq <= HERO_EQUIPMENTCOUNT; ++seq)
                {
                    if (!seqList.Contains(seq) && LEGAL_QUALITY_LIST.Contains(oneQualityInfo.Key))
                        errorStringBuilder.AppendFormat("英雄（heroId={0}）在品质为{1}下缺少第{2}个槽位的装备配置\n", heroInfo.Key, oneQualityInfo.Key, seq);
                }
            }
        }

        string lackDataErrorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(lackDataErrorString))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = string.Format("HeroEquipment表中存在以下配置缺失:\n{0}\n", lackDataErrorString);
            return false;
        }
    }
}
