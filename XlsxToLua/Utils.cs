using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class Utils
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);
    private const int OF_READWRITE = 2;
    private const int OF_SHARE_DENY_NONE = 0x40;
    private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

    /// <summary>
    /// 获取某个文件的状态
    /// </summary>
    public static FILE_STATE GetFileState(string filePath)
    {
        if (File.Exists(filePath))
        {
            IntPtr vHandle = _lopen(filePath, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
                return FILE_STATE.IS_OPEN;

            CloseHandle(vHandle);
            return FILE_STATE.AVAILABLE;
        }
        else
            return FILE_STATE.INEXIST;
    }

    /// <summary>
    /// 将Excel中的列编号转为列名称（第1列为A，第28列为AB）
    /// </summary>
    public static string GetExcelColumnName(int columnNumber)
    {
        string columnName = string.Empty;

        if (columnNumber <= 26)
        {
            columnName = ((char)('A' + columnNumber - 1)).ToString();
        }
        else
        {
            int quotient = columnNumber / 26;
            int remainder = columnNumber % 26;
            char first = (char)('A' + quotient - 1);
            char second = (char)('A' + remainder - 1);
            columnName = string.Concat(first, second);
        }

        return columnName;
    }

    /// <summary>
    /// 将List中的所有数据用指定分隔符连接为一个新字符串
    /// </summary>
    public static string CombineString<T>(List<T> list, string separateString)
    {
        if (list == null || list.Count < 1)
            return null;
        else
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; ++i)
                builder.Append(list[i]).Append(separateString);

            string result = builder.ToString();
            // 去掉最后多加的一次分隔符
            if (separateString != null)
                return result.Substring(0, result.Length - separateString.Length);
            else
                return result;
        }
    }

    public static void Log(string logString)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(logString);
        AppValues.LogContent.AppendLine(logString);
    }

    public static void LogWarning(string warningString)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warningString);
        AppValues.LogContent.AppendLine(warningString);
    }

    public static void LogError(string errorString)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorString);
        AppValues.LogContent.AppendLine(errorString);
    }

    /// <summary>
    /// 输出错误信息并在用户按任意键后退出
    /// </summary>
    public static void LogErrorAndExit(string errorString)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorString);
        Console.WriteLine("程序被迫退出，请修正错误后重试");
        Console.ReadKey();
        Environment.Exit(0);
    }

    /// <summary>
    /// 将程序运行中检查出的所有错误保存到文本文件中，存储目录为本工具所在目录
    /// </summary>
    public static bool SaveErrorInfoToFile()
    {
        try
        {
            string fileName = string.Format("表格检查结果 {0}", string.Format("{0:yyyy年MM月dd日 HH时mm分ss秒}.txt", DateTime.Now));
            string savePath = Utils.CombinePath(System.Environment.CurrentDirectory, fileName);
            StreamWriter writer = new StreamWriter(savePath, false, new UTF8Encoding(false));
            writer.WriteLine(AppValues.LogContent.ToString().Replace("\n", System.Environment.NewLine));
            writer.Flush();
            writer.Close();

            Log(string.Format("全部错误信息已导出文本文件，文件名为\"{0}\"，存储在本工具所在目录下", fileName));
            return true;
        }
        catch
        {
            LogError("全部错误信息导出到文本文件失败");
            return false;
        }
    }

    /// <summary>
    /// 将某张Excel表格转换为lua table内容保存到文件
    /// </summary>
    public static bool SaveLuaFile(string tableName, string content)
    {
        try
        {
            string savePath = Utils.CombinePath(AppValues.ExportLuaFilePath, tableName + ".lua");
            StreamWriter writer = new StreamWriter(savePath, false, new UTF8Encoding(false));
            writer.Write(content);
            writer.Flush();
            writer.Close();
            return true;
        }
        catch
        {
            LogError(string.Format("导出表格{0}为lua文件失败", tableName));
            return false;
        }
    }

    /// <summary>
    /// 合并两个路径字符串，与.Net类库中的Path.Combine不同，本函数不会因为path2以目录分隔符开头就认为是绝对路径，然后直接返回path2
    /// </summary>
    public static string CombinePath(string path1, string path2)
    {
        path1 = path1.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        path2 = path2.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        if (path2.StartsWith(Path.DirectorySeparatorChar.ToString()))
            path2 = path2.Substring(1, path2.Length - 1);

        return Path.Combine(path1, path2);
    }
}

public enum FILE_STATE
{
    INEXIST,     // 不存在
    IS_OPEN,     // 已被打开
    AVAILABLE,   // 当前可用
}
