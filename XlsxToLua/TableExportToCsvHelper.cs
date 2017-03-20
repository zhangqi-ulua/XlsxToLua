using System;
using System.Collections.Generic;
using System.Text;

public class TableExportToCsvHelper
{
    public static bool ExportTableToCsv(TableInfo tableInfo, out string errorString)
    {
        // 存储每一行数据生成的csv文件内容
        List<StringBuilder> rowContentList = new List<StringBuilder>();

        // 生成主键列的同时，对每行的StringBuilder进行初始化，主键列只能为int、long或string型，且值不允许为空，直接转为字符串即可
        FieldInfo keyColumnFieldInfo = tableInfo.GetKeyColumnFieldInfo();
        int rowCount = keyColumnFieldInfo.Data.Count;
        for (int row = 0; row < rowCount; ++row)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(keyColumnFieldInfo.Data[row]);
            rowContentList.Add(stringBuilder);
        }
        // 生成其他列的内容（将array、dict这样的集合类型下属字段作为独立字段处理）
        List<FieldInfo> allFieldInfoIgnoreSetDataStructure = tableInfo.GetAllClientFieldInfoIgnoreSetDataStructure();
        for (int i = 1; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            _GetOneFieldCsvContent(allFieldInfoIgnoreSetDataStructure[i], rowContentList);

        // 如果声明了要在首行列举字段名称
        if (AppValues.ExportCsvIsExportColumnName == true)
        {
            StringBuilder columnNameStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                columnNameStringBuilder.Append(AppValues.ExportCsvSplitString);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                // 如果是array下属的子元素，字段名生成格式为“array字段名[从1开始的下标序号]”。dict下属的子元素，生成格式为“dict字段名.下属字段名”
                if (fieldInfo.ParentField != null)
                {
                    String fieldName = fieldInfo.FieldName;
                    FieldInfo tempField = fieldInfo;
                    while (tempField.ParentField != null)
                    {
                        if (tempField.ParentField.DataType == DataType.Array)
                            fieldName = string.Concat(tempField.ParentField.FieldName, fieldName);
                        else if (tempField.ParentField.DataType == DataType.Dict)
                            fieldName = string.Concat(tempField.ParentField.FieldName, ".", fieldName);

                        tempField = tempField.ParentField;
                    }

                    columnNameStringBuilder.Append(fieldName);
                }
                else
                    columnNameStringBuilder.Append(fieldInfo.FieldName);
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, columnNameStringBuilder.Remove(0, AppValues.ExportCsvSplitString.Length));
        }

        // 如果声明了要在其后列举字段数据类型
        if (AppValues.ExportCsvIsExportColumnDataType == true)
        {
            StringBuilder columnDataTypeStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                columnDataTypeStringBuilder.Append(AppValues.ExportCsvSplitString);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                columnDataTypeStringBuilder.Append(fieldInfo.DataType);
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(AppValues.ExportCsvIsExportColumnName == true ? 1 : 0, columnDataTypeStringBuilder.Remove(0, AppValues.ExportCsvSplitString.Length));
        }

        // 保存为csv文件
        if (Utils.SaveCsvFile(tableInfo.TableName, rowContentList))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存为csv文件失败\n";
            return false;
        }
    }

    private static void _GetOneFieldCsvContent(FieldInfo fieldInfo, List<StringBuilder> rowContentList)
    {
        int rowCount = fieldInfo.Data.Count;

        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Long:
            case DataType.Float:
            case DataType.String:
            case DataType.Lang:
            case DataType.TableString:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        // 先增加与上一字段间的分隔符
                        stringBuilder.Append(AppValues.ExportCsvSplitString);
                        // 再生成本行对应的内容
                        if (fieldInfo.Data[row] != null)
                            stringBuilder.Append(fieldInfo.Data[row].ToString());
                    }
                    break;
                }
            case DataType.Bool:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportCsvSplitString);
                        if (fieldInfo.Data[row] != null)
                        {
                            if ((bool)fieldInfo.Data[row] == true)
                                stringBuilder.Append("true");
                            else
                                stringBuilder.Append("false");
                        }
                    }
                    break;
                }
            case DataType.Json:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportCsvSplitString);
                        if (fieldInfo.Data[row] != null)
                            stringBuilder.Append(fieldInfo.JsonString[row]);
                    }
                    break;
                }
            case DataType.Date:
                {
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
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportCsvSplitString);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row])).ToString(exportFormatString));
                                }
                                break;
                            }
                        case DateFormatType.ReferenceDateSec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportCsvSplitString);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalSeconds);
                                }
                                break;
                            }
                        case DateFormatType.ReferenceDateMsec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportCsvSplitString);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalMilliseconds);
                                }
                                break;
                            }
                        default:
                            {
                                Utils.LogErrorAndExit("用_GetOneFieldCsvContent函数导出csv文件的date型的DateFormatType非法");
                                break;
                            }
                    }
                    break;
                }
            case DataType.Time:
                {
                    TimeFormatType timeFormatType = TableAnalyzeHelper.GetTimeFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString());
                    switch (timeFormatType)
                    {
                        case TimeFormatType.FormatString:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportCsvSplitString);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row])).ToString(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString()));
                                }
                                break;
                            }
                        case TimeFormatType.ReferenceTimeSec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportCsvSplitString);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalSeconds);
                                }
                                break;
                            }
                        default:
                            {
                                Utils.LogErrorAndExit("错误：用_GetOneFieldCsvContent函数导出csv文件的time型的TimeFormatType非法");
                                break;
                            }
                    }
                    break;
                }
            case DataType.Array:
            case DataType.Dict:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportCsvSplitString);
                        if ((bool)fieldInfo.Data[row] == false)
                            stringBuilder.Append("-1");
                    }
                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit(string.Format("_GetOneFieldCsvContent函数中未定义{0}类型数据导出至csv文件的形式", fieldInfo.DataType));
                    break;
                }
        }
    }
}
