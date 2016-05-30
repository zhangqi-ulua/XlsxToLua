using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

public class ExcelOperateHelper
{
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

    public static bool ExportToExcel(string tableName)
    {
        Excel.Application application = new Excel.Application();
        // 不显示Excel窗口
        application.Visible = false;
        // 不显示警告对话框
        application.DisplayAlerts = false;
        // 禁止屏幕刷新
        application.ScreenUpdating = false;
        // 编辑非空单元格时不进行警告提示
        application.AlertBeforeOverwriting = false;

        string savePath = System.IO.Path.GetFullPath(Utils.CombinePath(AppValues.ExportExcelPath, tableName + ".xlsx"));
        // 检查要导出的Excel文件是否已存在且正被其他程序使用
        if (Utils.GetFileState(savePath) == FILE_STATE.IS_OPEN)
        {
            Utils.LogErrorAndExit("错误：导出目标路径存在该Excel文件且该文件正被其他程序打开，请关闭后重试");
            return false;
        }

        // 新建Excel工作簿
        Excel.Workbook workbook = application.Workbooks.Add();
        // 在名为data的Sheet表中填充数据
        Excel.Worksheet dataWorksheet = workbook.Sheets[1] as Excel.Worksheet;
        dataWorksheet.Name = AppValues.EXCEL_DATA_SHEET_NAME;
        dataWorksheet.Tab.ColorIndex = (XlColorIndex)AppValues.DataSheetTabColorIndex;

        System.Data.DataTable data = MySQLOperateHelper.ReadDatabaseTable(tableName);
        System.Data.DataTable columnInfo = MySQLOperateHelper.GetColumnInfo(tableName);
        int columnCount = columnInfo.Rows.Count;
        // 按XlsxToLua工具规定的格式导出配置参数及数据
        // 注意Excel中左上角单元格下标为[1,1]，而DataTable中为[0,0]
        for (int columnIndex = 1; columnIndex <= columnCount; ++columnIndex)
        {
            // 第一行为字段描述
            string desc = columnInfo.Rows[columnIndex - 1]["COLUMN_COMMENT"].ToString();
            dataWorksheet.Cells[AppValues.DATA_FIELD_DESC_INDEX, columnIndex] = desc;
            // 第二行为字段变量名
            string fileName = columnInfo.Rows[columnIndex - 1]["COLUMN_NAME"].ToString();
            dataWorksheet.Cells[AppValues.DATA_FIELD_NAME_INDEX, columnIndex] = fileName;
            // 第三行为字段数据类型
            string databaseDataType = columnInfo.Rows[columnIndex - 1]["DATA_TYPE"].ToString();
            string maxLength = columnInfo.Rows[columnIndex - 1]["CHARACTER_MAXIMUM_LENGTH"] as string;
            string dataType = _ConvertDataType(databaseDataType, maxLength);
            string fullDatabaseDataType = columnInfo.Rows[columnIndex - 1]["COLUMN_TYPE"].ToString();
            if (string.IsNullOrEmpty(dataType))
            {
                Utils.LogErrorAndExit(string.Format("错误：数据表中名为{0}的列数据类型为{1}，而_ConvertDataType函数中未定义该数据库数据类型对应XlsxToLua工具要求的数据类型，请补全转换关系后重试", fileName, fullDatabaseDataType));
                return false;
            }
            else
                dataWorksheet.Cells[AppValues.DATA_FIELD_DATA_TYPE_INDEX, columnIndex] = dataType;
            // 第四行为字段检查规则
            string checkRule = _GetCheckRuleByDatabaseColumnInfo(columnInfo.Rows[columnIndex - 1], dataType);
            dataWorksheet.Cells[AppValues.DATA_FIELD_CHECK_RULE_INDEX, columnIndex] = checkRule;
            // 第五行为导出到数据库中的字段名及类型
            dataWorksheet.Cells[AppValues.DATA_FIELD_EXPORT_DATABASE_FIELD_INFO, columnIndex] = fullDatabaseDataType;
            // 从第六行开始导入数据库中数据表所填写的数据
            int dataCount = data.Rows.Count;
            for (int i = 0; i < dataCount; ++i)
                dataWorksheet.Cells[AppValues.DATA_FIELD_DATA_START_INDEX + i, columnIndex] = data.Rows[i][columnIndex - 1];

            // 将每列的背景色按配置进行设置
            if (AppValues.ColumnBackgroundColorIndex != null)
                dataWorksheet.get_Range(Utils.GetExcelColumnName(columnIndex) + "1").EntireColumn.Interior.ColorIndex = AppValues.ColumnBackgroundColorIndex[(columnIndex - 1) % AppValues.ColumnBackgroundColorIndex.Count];
        }
        // 设置表格中所有单元格均自动换行
        dataWorksheet.Cells.WrapText = true;
        // 对前五行配置列执行窗口冻结
        Excel.Range excelRange = dataWorksheet.get_Range(dataWorksheet.Cells[AppValues.DATA_FIELD_DATA_START_INDEX, 1], dataWorksheet.Cells[AppValues.DATA_FIELD_DATA_START_INDEX, 1]);
        excelRange.Select();
        application.ActiveWindow.FreezePanes = true;
        // 先对整表设置虚线边框
        dataWorksheet.Cells.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlHairline;
        dataWorksheet.Cells.Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlHairline;
        dataWorksheet.Cells.Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlHairline;
        dataWorksheet.Cells.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlHairline;
        dataWorksheet.Cells.Borders[XlBordersIndex.xlInsideHorizontal].Weight = XlBorderWeight.xlHairline;
        dataWorksheet.Cells.Borders[XlBordersIndex.xlInsideVertical].Weight = XlBorderWeight.xlHairline;
        // 再对前五行配置列添加内外实线边框
        excelRange = dataWorksheet.get_Range(dataWorksheet.Cells[AppValues.DATA_FIELD_DESC_INDEX, 1], dataWorksheet.Cells[AppValues.DATA_FIELD_EXPORT_DATABASE_FIELD_INFO, columnCount]);
        excelRange.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlThin;
        excelRange.Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlThin;
        excelRange.Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlThin;
        excelRange.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThin;
        excelRange.Borders[XlBordersIndex.xlInsideHorizontal].Weight = XlBorderWeight.xlThin;
        excelRange.Borders[XlBordersIndex.xlInsideVertical].Weight = XlBorderWeight.xlThin;

        dataWorksheet.SaveAs(savePath);
        workbook.SaveAs(savePath);

        // 新建名为config的Sheet表
        Excel.Worksheet configWorksheet = workbook.Sheets.Add(Type.Missing, dataWorksheet) as Excel.Worksheet;
        configWorksheet.Name = AppValues.EXCEL_CONFIG_SHEET_NAME;
        configWorksheet.Tab.ColorIndex = (XlColorIndex)AppValues.ConfigSheetTabColorIndex;
        // 写入导出到数据库中的字段名及类型配置
        configWorksheet.Cells[1, 1] = AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_NAME;
        configWorksheet.Cells[2, 1] = tableName;
        // 设置表格中所有单元格均自动换行
        configWorksheet.Cells.WrapText = true;
        // 设置列背景色
        if (AppValues.ColumnBackgroundColorIndex != null)
            configWorksheet.get_Range("A1").EntireColumn.Interior.ColorIndex = AppValues.ColumnBackgroundColorIndex[0];
        // 对第一行配置名行执行窗口冻结
        excelRange = configWorksheet.get_Range("A2");
        excelRange.Select();
        application.ActiveWindow.FreezePanes = true;

        configWorksheet.SaveAs(savePath);

        // 设置默认显示的Sheet为data表
        dataWorksheet.Select();
        workbook.SaveAs(savePath);

        workbook.Close(false);
        application.Workbooks.Close();
        application.Quit();
        //System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
        _KillExcelProcess(application, tableName);

        return true;
    }

    /// <summary>
    /// 自定义MySQL数据库中的数据类型对应转换到XlsxToLua工具要求的数据类型
    /// </summary>
    private static string _ConvertDataType(string databaseDataType, string maxLength)
    {
        switch (databaseDataType)
        {
            case "int":
                return "int";
            case "tinyint":
                {
                    if ("1".Equals(maxLength))
                        return "bool";
                    else
                        return "int";
                }
            case "float":
            case "double":
                return "float";
            case "char":
            case "varchar":
            case "date":
            case "datetime":
                return "string";
            default:
                return null;
        }
    }

    /// <summary>
    /// 根据数据库中某列所设置的属性得到对应XlsxToLua工具支持的字段检查规则，因数据库列所提供信息有限，只能分析出unique和notEmpty检查规则且无法细分比如string型的notEmpty规则是否应包含trim参数
    /// </summary>
    private static string _GetCheckRuleByDatabaseColumnInfo(System.Data.DataRow columnInfo, string xlsxToLuaDataType)
    {
        List<string> checkRules = new List<string>();

        // notEmpty检查规则
        if ("string".Equals(xlsxToLuaDataType) || "lang".Equals(xlsxToLuaDataType))
        {
            string isNullable = columnInfo["IS_NULLABLE"].ToString();
            if ("NO".Equals(isNullable, System.StringComparison.CurrentCultureIgnoreCase))
                checkRules.Add("notEmpty");
        }
        // unique检查规则
        if ("int".Equals(xlsxToLuaDataType) || "float".Equals(xlsxToLuaDataType) || "string".Equals(xlsxToLuaDataType) || "lang".Equals(xlsxToLuaDataType))
        {
            string columnKey = columnInfo["COLUMN_KEY"] as string;
            if ("UNI".Equals(columnKey))
                checkRules.Add("unique");
        }

        return Utils.CombineString(checkRules, " && ");
    }

    private static void _KillExcelProcess(Excel.Application application, string tableName)
    {
        try
        {
            IntPtr hwnd = new IntPtr(application.Hwnd);
            int id;
            GetWindowThreadProcessId(hwnd, out id);
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(id);
            process.Kill();
        }
        catch (Exception e)
        {
            Utils.LogWarning(string.Format("警告：无法关闭Excel表格\"{0}\"对应的Excel进程，请在本工具运行完毕后手工在Windows任务管理器中杀死相应进程。错误信息：{1}", tableName, e.Message));
        }
    }
}
