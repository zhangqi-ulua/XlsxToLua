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
    public static FileState GetFileState(string filePath)
    {
        if (File.Exists(filePath))
        {
            IntPtr vHandle = _lopen(filePath, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
                return FileState.IsOpen;

            CloseHandle(vHandle);
            return FileState.Available;
        }
        else
            return FileState.Inexist;
    }

    /// <summary>
    /// 将Excel中的列编号转为列名称（第1列为A，第28列为AB）
    /// </summary>
    public static string GetExcelColumnName(int columnNumber)
    {
        string columnName = string.Empty;

        if (columnNumber <= 26)
            columnName = ((char)('A' + columnNumber - 1)).ToString();
        else
        {
            int quotient = columnNumber / 26;
            int remainder = columnNumber % 26;
            char first;
            char second;
            if (remainder == 0)
            {
                first = (char)('A' + quotient - 2);
                second = 'Z';
            }
            else
            {
                first = (char)('A' + quotient - 1);
                second = (char)('A' + remainder - 1);
            }

            columnName = string.Concat(first, second);
        }

        return columnName;
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

    /// <summary>
    /// 将List中的所有数据用指定分隔符连接为一个新字符串
    /// </summary>
    public static string CombineString(IList<string> list, string separateString)
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
    }

    public static void LogWarning(string warningString)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warningString);
    }

    public static void LogErrorAndExit(string errorString)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorString);
        Console.WriteLine("程序被迫退出，请修正错误后重试");
        Console.ReadKey();
        Environment.Exit(0);
    }
}

public enum FileState
{
    Inexist,     // 不存在
    IsOpen,      // 已被打开
    Available,   // 当前可用
}
