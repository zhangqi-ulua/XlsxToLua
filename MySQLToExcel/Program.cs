using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        // 读取本工具所在路径下的config配置文件
        string errorString = null;
        string configFilePath = Utils.CombinePath(AppValues.PROGRAM_FOLDER_PATH, AppValues.CONFIG_FILE_NAME);
        AppValues.ConfigData = TxtConfigReader.ParseTxtConfigFile(configFilePath, ":", out errorString);
        if (!string.IsNullOrEmpty(errorString))
            Utils.LogErrorAndExit(errorString);

        // 检查填写的Excel导出目录是否存在
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_EXPORT_EXCEL_PATH))
        {
            AppValues.ExportExcelPath = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_EXPORT_EXCEL_PATH].Trim();
            if (!Directory.Exists(AppValues.ExportExcelPath))
                Utils.LogErrorAndExit(string.Format("config配置文件中声明的导出Excel文件的存放目录不存在，你填写的为\"{0}\"", AppValues.ExportExcelPath));
        }
        else
            Utils.LogErrorAndExit(string.Format("未在config配置文件中以名为\"{0}\"的key声明导出Excel文件的存放目录", AppValues.APP_CONFIG_KEY_EXPORT_EXCEL_PATH));

        // 获取设置的Excel文件中列的最大宽度
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_EXCEL_COLUMN_MAX_WIDTH))
        {
            double inputExcelColumnMaxWidth = -1;
            string inputParam = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_EXCEL_COLUMN_MAX_WIDTH];
            if (double.TryParse(inputParam, out inputExcelColumnMaxWidth) == true)
            {
                if (inputExcelColumnMaxWidth > 0)
                    AppValues.ExcelColumnMaxWidth = inputExcelColumnMaxWidth;
                else
                    Utils.LogErrorAndExit(string.Format("config配置文件中声明的Excel文件中列的最大宽度非法，必须大于0，你输入的为\"{0}\"", inputParam));
            }
            else
                Utils.LogErrorAndExit(string.Format("config配置文件中声明的Excel文件中列的最大宽度不是一个合法数字，你输入的为\"{0}\"", inputParam));
        }
        else
            Utils.LogWarning(string.Format("警告：未在config配置文件中以名为\"{0}\"的key声明生成的Excel文件中列的最大宽度，本工具将不对生成的Excel文件进行美化", AppValues.APP_CONFIG_KEY_EXCEL_COLUMN_MAX_WIDTH));

        const int COLOR_INDEX_MIN = 0;
        const int COLOR_INDEX_MAX = 56;

        // 获取设置的每列背景色，如果进行了设置需检查ColorIndex是否正确
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_COLUMN_BACKGROUND_COLOR))
        {
            AppValues.ColumnBackgroundColorIndex = new List<int>();
            string inputParam = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_COLUMN_BACKGROUND_COLOR];

            string[] colorIndexString = inputParam.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (colorIndexString.Length > 0)
            {
                foreach (string oneColorIndexString in colorIndexString)
                {
                    int oneColorIndex = -1;
                    if (int.TryParse(oneColorIndexString, out oneColorIndex) == true)
                    {
                        if (oneColorIndex >= COLOR_INDEX_MIN && oneColorIndex <= COLOR_INDEX_MAX)
                            AppValues.ColumnBackgroundColorIndex.Add(oneColorIndex);
                        else
                            Utils.LogErrorAndExit(string.Format("config配置文件中声明的颜色索引值\"{0}\"非法，必须介于{1}到{2}之间", oneColorIndex, COLOR_INDEX_MIN, COLOR_INDEX_MAX));
                    }
                    else
                        Utils.LogErrorAndExit(string.Format("config配置文件中声明的颜色索引值\"{0}\"不是一个合法数字", oneColorIndexString));
                }
            }
            else
                Utils.LogErrorAndExit(string.Format("config配置文件中以名为\"{0}\"的key声明各列背景色不允许为空", AppValues.APP_CONFIG_KEY_COLUMN_BACKGROUND_COLOR));
        }

        // 获取设置的data、config两张Sheet表的标签按钮颜色
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_DATA_SHEET_TAB_COLOR))
        {
            string colorIndexString = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_DATA_SHEET_TAB_COLOR];
            int colorIndex = -1;
            if (int.TryParse(colorIndexString, out colorIndex) == true)
            {
                if (colorIndex >= COLOR_INDEX_MIN && colorIndex <= COLOR_INDEX_MAX)
                    AppValues.DataSheetTabColorIndex = colorIndex;
                else
                    Utils.LogErrorAndExit(string.Format("config配置文件中声明的data表标签按钮颜色索引值\"{0}\"非法，必须介于{1}到{2}之间", colorIndex, COLOR_INDEX_MIN, COLOR_INDEX_MAX));
            }
            else
                Utils.LogErrorAndExit(string.Format("config配置文件中声明的data表标签按钮颜色索引值\"{0}\"不是一个合法数字", colorIndexString));
        }
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_CONFIG_SHEET_TAB_COLOR))
        {
            string colorIndexString = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_CONFIG_SHEET_TAB_COLOR];
            int colorIndex = -1;
            if (int.TryParse(colorIndexString, out colorIndex) == true)
            {
                if (colorIndex >= COLOR_INDEX_MIN && colorIndex <= COLOR_INDEX_MAX)
                    AppValues.ConfigSheetTabColorIndex = colorIndex;
                else
                    Utils.LogErrorAndExit(string.Format("config配置文件中声明的config表标签按钮颜色索引值\"{0}\"非法，必须介于{1}到{2}之间", colorIndex, COLOR_INDEX_MIN, COLOR_INDEX_MAX));
            }
            else
                Utils.LogErrorAndExit(string.Format("config配置文件中声明的config表标签按钮颜色索引值\"{0}\"不是一个合法数字", colorIndexString));
        }

        // 获取要导出的数据表名
        List<string> exportTableName = new List<string>();
        // 在这里自定义获取要导出的数据表名的方法（这里以在config.txt中进行配置为例，还可以采用类似表名特殊前缀标识等方式实现）
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_EXPORT_DATA_TABLE_NAMES))
        {
            string exportTableNameString = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_EXPORT_DATA_TABLE_NAMES].Trim();
            if (string.IsNullOrEmpty(exportTableNameString))
                Utils.LogErrorAndExit(string.Format("config配置文件中以名为\"{0}\"的key声明要导出的数据表名不允许为空", AppValues.APP_CONFIG_KEY_EXPORT_DATA_TABLE_NAMES));
            else
            {
                string[] tableNames = exportTableNameString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tableName in tableNames)
                    exportTableName.Add(tableName.Trim());
            }
        }
        else
            Utils.LogErrorAndExit(string.Format("未在config配置文件中以名为\"{0}\"的key声明要导出的数据表名", AppValues.APP_CONFIG_KEY_EXPORT_DATA_TABLE_NAMES));

        // 连接MySQL数据库
        MySQLOperateHelper.ConnectToDatabase(out errorString);
        if (!string.IsNullOrEmpty(errorString))
            Utils.LogErrorAndExit(string.Format("无法连接到MySQL数据库，{0}", errorString));

        // 检查声明的要导出的数据库表格是否存在，若存在导出到Excel
        foreach (string tableName in exportTableName)
        {
            if (MySQLOperateHelper.ExistTableNames.Contains(tableName))
            {
                Utils.Log(string.Format("导出数据表{0}：", tableName));
                ExcelOperateHelper.ExportToExcel(tableName);
                Utils.Log("成功");
            }
            else
                Utils.LogErrorAndExit(string.Format("\n错误：数据库中不存在名为{0}的数据表，请检查配置中声明的导出数据表名与数据库是否对应", tableName));
        }

        Utils.Log("\n按任意键退出本工具");
        Console.ReadKey();
    }
}
