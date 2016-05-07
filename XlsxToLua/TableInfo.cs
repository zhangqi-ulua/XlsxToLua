using System;
using System.Collections.Generic;

/// <summary>
/// 将一张Excel表格解析为本工具所需的数据结构
/// </summary>
public class TableInfo
{
    public string TableName { get; set; }
    // 表格配置参数
    public Dictionary<string, List<string>> TableConfig { get; set; }
    // 存储每个字段的信息以及按字段存储的所有数据
    private List<FieldInfo> _fieldInfo = new List<FieldInfo>();
    // 用于将字段名对应到_fieldInfo中的下标位置，目的是当其他表格进行ref检查规则时无需遍历快速找到指定字段列信息和数据（key：fieldName， value：index），但忽略array或dict的子元素列
    private Dictionary<string, int> _indexForFieldNameToColumnSeq = new Dictionary<string, int>();

    public void AddField(FieldInfo fieldInfo)
    {
        _fieldInfo.Add(fieldInfo);
        _indexForFieldNameToColumnSeq.Add(fieldInfo.FieldName, _fieldInfo.Count - 1);
    }

    public FieldInfo GetFieldInfoByFieldName(string fieldName)
    {
        if (_indexForFieldNameToColumnSeq.ContainsKey(fieldName))
            return _fieldInfo[_indexForFieldNameToColumnSeq[fieldName]];
        else
            return null;
    }

    public List<FieldInfo> GetAllFieldInfo()
    {
        return _fieldInfo;
    }

    public FieldInfo GetKeyColumnFieldInfo()
    {
        if (_fieldInfo.Count > 0)
            return _fieldInfo[0];
        else
            return null;
    }
}

/// <summary>
/// 一张表格中一个字段的信息，包含这个字段名称、数据类型、检查规则的定义，以及所有行的数据
/// </summary>
public class FieldInfo
{
    // 该字段所在表格
    public string TableName { get; set; }
    // 字段名
    public string FieldName { get; set; }
    // 字段数据类型
    public DataType DataType { get; set; }
    // 声明字段数据类型的字符串
    public string DataTypeString { get; set; }
    // array类型的子元素的数据类型
    public DataType ArrayChildDataType { get; set; }
    // array类型的子元素的数据类型字符串
    public string ArrayChildDataTypeString { get; set; }
    // 字段描述
    public string Desc { get; set; }
    // 声明字段检查规则的字符串
    public string CheckRule { get; set; }
    // 该字段在表格中的列号（从0计）
    public int ColumnSeq { get; set; }
    // 如果该字段不是集合类型，直接依次存储该字段下的所有行的数据，否则存储每行定义的该集合数据是否有效
    public List<object> Data { get; set; }
    // 如果该字段为lang型，LangKeys中额外存储所填的所有key名，对应的键值则存储在Data中
    public List<object> LangKeys { get; set; }
    // 如果该字段为tableString型，存储解析之后的格式定义
    public TableStringFormatDefine TableStringFormatDefine { get; set; }
    // 如果该字段是array或dict类型，其下属的字段信息存放在该变量中
    public List<FieldInfo> ChildField { get; set; }
    // 如果该字段是array或dict的子元素，存储其父元素的引用
    public FieldInfo ParentField { get; set; }
    // 导出到数据库中对应的字段名
    public string DatabaseFieldName { get; set; }
    // 导出到数据库中对应的字段数据类型
    public string DatabaseFieldType { get; set; }
}

/// <summary>
/// 数据类型的定义
/// </summary>
public enum DataType
{
    Invalid,

    Int,
    Float,
    Bool,
    String,
    Lang,
    TableString,
    Array,
    Dict,
}

/// <summary>
/// tableString代表的table中的key类型
/// </summary>
public enum TABLE_STRING_KEY_TYPE
{
    SEQ,              // 按数据顺序自动编号
    DATA_IN_INDEX,    // 以数据组中指定索引位置的数据为key
}

/// <summary>
/// tableString代表的table中的value类型
/// </summary>
public enum TABLE_STRING_VALUE_TYPE
{
    TRUE,             // 值始终为true
    DATA_IN_INDEX,    // 以数据组中指定索引位置的数据为value
    TABLE,            // value是含有复杂数据的table
}

/// <summary>
/// 形如#1(int)这样对指定数据类型和索引位置的数据的定义
/// </summary>
public struct DataInIndexDefine
{
    // 数据类型
    public DataType DataType;
    // 数据所在数据组的索引位置
    public int DataIndex;
}

/// <summary>
/// 形如type=#1(int)这样对table型value中一个键值对的定义
/// </summary>
public struct TableElementDefine
{
    public string KeyName;
    public DataInIndexDefine DataInIndexDefine;
}

/// <summary>
/// tableString代表的table中对key的定义描述
/// </summary>
public struct TableStringKeyDefine
{
    // key的类型
    public TABLE_STRING_KEY_TYPE KeyType;
    // 如果key为DATA_IN_INDEX类型，存储其定义
    public DataInIndexDefine DataInIndexDefine;
}

/// <summary>
/// tableString代表的table中对value的定义描述
/// </summary>
public struct TableStringValueDefine
{
    // value的类型
    public TABLE_STRING_VALUE_TYPE ValueType;
    // 如果value为DATA_IN_INDEX类型，存储其定义
    public DataInIndexDefine DataInIndexDefine;
    // 如果value为TABLE类型，则需要存储table中每个元素的定义
    public List<TableElementDefine> TableValueDefineList;
}

/// <summary>
/// 将tableString定义的字符串转为的格式定义
/// </summary>
public struct TableStringFormatDefine
{
    public TableStringKeyDefine KeyDefine;
    public TableStringValueDefine ValueDefine;
}
