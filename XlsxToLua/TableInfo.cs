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

    /// <summary>
    /// 获取所有指定了字段名，需要进行客户端lua、csv、json等方式导出的字段
    /// </summary>
    public List<FieldInfo> GetAllClientFieldInfo()
    {
        List<FieldInfo> allClientFieldInfo = new List<FieldInfo>();
        foreach (FieldInfo fieldInfo in _fieldInfo)
        {
            if (fieldInfo.IsIgnoreClientExport == false)
                allClientFieldInfo.Add(fieldInfo);
        }

        return allClientFieldInfo;
    }

    public FieldInfo GetKeyColumnFieldInfo()
    {
        if (_fieldInfo.Count > 0)
            return _fieldInfo[0];
        else
            return null;
    }

    /// <summary>
    /// 获取依次排列的表格中各字段信息，但无视array、dict型的嵌套结构，将其下属子元素作为独立字段
    /// </summary>
    public List<FieldInfo> GetAllClientFieldInfoIgnoreSetDataStructure()
    {
        List<FieldInfo> allFieldInfo = new List<FieldInfo>();
        foreach (FieldInfo fieldInfo in _fieldInfo)
            _AddClientFieldInfoFromOneField(fieldInfo, allFieldInfo);

        return allFieldInfo;
    }

    public void _AddClientFieldInfoFromOneField(FieldInfo fieldInfo, List<FieldInfo> allFieldInfo)
    {
        if (fieldInfo.DataType == DataType.Array || fieldInfo.DataType == DataType.Dict)
        {
            allFieldInfo.Add(fieldInfo);
            foreach (FieldInfo childField in fieldInfo.ChildField)
                _AddClientFieldInfoFromOneField(childField, allFieldInfo);
        }
        else if (fieldInfo.IsIgnoreClientExport == false)
            allFieldInfo.Add(fieldInfo);
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
    // 如果该字段为json型，JsonString中存储所填的所有json字符串，对应解析后的JsonData存储在Data中
    public List<string> JsonString { get; set; }
    // 存储额外属性，比如date类型的输入、导出选项等
    public Dictionary<string, object> ExtraParam { get; set; }
    // 如果该字段为tableString型，存储解析之后的格式定义
    public TableStringFormatDefine TableStringFormatDefine { get; set; }
    // 如果该字段为mapString型，存储解析之后的格式定义
    public MapStringInfo MapStringFormatDefine { get; set; }
    // 如果该字段是array或dict类型，其下属的字段信息存放在该变量中
    public List<FieldInfo> ChildField { get; set; }
    // 如果该字段是array或dict的子元素，存储其父元素的引用
    public FieldInfo ParentField { get; set; }
    // 导出到数据库中对应的字段名
    public string DatabaseFieldName { get; set; }
    // 导出到数据库中对应的字段数据类型
    public string DatabaseFieldType { get; set; }
    // 是否忽略进行lua、csv、json等客户端方式导出（未填写字段名但填写了数据库导出信息的字段，仅进行数据库导出）
    public bool IsIgnoreClientExport { get; set; }

    public FieldInfo()
    {
        ExtraParam = new Dictionary<string, object>();
        IsIgnoreClientExport = false;
    }
}

/// <summary>
/// mapString类型的配置信息
/// </summary>
public class MapStringInfo
{
    // 下属参数配置信息（key：参数变量名， value：此参数的配置信息）
    public Dictionary<string, MapStringParamInfo> ParamInfo { get; set; }

    public MapStringInfo()
    {
        ParamInfo = new Dictionary<string, MapStringParamInfo>();
    }
}

/// <summary>
/// mapString类型下属的参数配置信息
/// </summary>
public class MapStringParamInfo
{
    // 参数名
    public string ParamName { get; set; }
    // 参数的数据类型
    public DataType DataType { get; set; }
    // 如果此参数是mapString型，存储下属参数信息
    public MapStringInfo MapStringInfo { get; set; }
}

/// <summary>
/// 数据类型的定义
/// </summary>
public enum DataType
{
    Invalid,

    Int,
    Long,
    Float,
    Bool,
    String,
    Lang,
    Date,
    Time,
    Json,
    TableString,
    Array,
    Dict,
    MapString,
}

/// <summary>
/// tableString代表的table中的key类型
/// </summary>
public enum TableStringKeyType
{
    Seq,             // 按数据顺序自动编号
    DataInIndex,     // 以数据组中指定索引位置的数据为key
}

/// <summary>
/// tableString代表的table中的value类型
/// </summary>
public enum TableStringValueType
{
    True,            // 值始终为true
    DataInIndex,     // 以数据组中指定索引位置的数据为value
    Table,           // value是含有复杂数据的table
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
    public TableStringKeyType KeyType;
    // 如果key为DATA_IN_INDEX类型，存储其定义
    public DataInIndexDefine DataInIndexDefine;
}

/// <summary>
/// tableString代表的table中对value的定义描述
/// </summary>
public struct TableStringValueDefine
{
    // value的类型
    public TableStringValueType ValueType;
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

/// <summary>
/// date型数据的输入导出格式
/// </summary>
public enum DateFormatType
{
    FormatString,          // 符合C#类库要求的标准时间格式
    ReferenceDateSec,      // 用距离1970年1月1日的秒数表示
    ReferenceDateMsec,     // 用距离1970年1月1日的毫秒数表示
    DataTable,             // 生成调用lua库函数os.date的代码形式
}

/// <summary>
/// time型数据的输入导出格式
/// </summary>
public enum TimeFormatType
{
    FormatString,          // 符合C#类库要求的标准时间格式
    ReferenceTimeSec,      // 用距离0点的秒数表示
}
