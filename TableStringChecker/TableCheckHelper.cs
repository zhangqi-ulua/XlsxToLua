using System;
using System.Collections.Generic;
using System.Text;

public class TableCheckHelper
{
    /// <summary>
    /// 检查字段名是否合法，要求必须以英文字母开头，只能为英文字母、数字或下划线，且不能为空或纯空格
    /// </summary>
    public static bool CheckFieldName(string fieldName, out string errorString)
    {
        if (string.IsNullOrEmpty(fieldName.Trim()))
        {
            errorString = "不能为空或纯空格";
            return false;
        }
        char firstLetter = fieldName[0];
        if (!((firstLetter >= 'a' && firstLetter <= 'z') || (firstLetter >= 'A' && firstLetter <= 'Z')))
        {
            errorString = string.Format("{0}不合法，必须以英文字母开头", fieldName);
            return false;
        }
        foreach (char c in fieldName)
        {
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_'))
            {
                errorString = string.Format("{0}不合法，含有非法字符\"{1}\"，只能由英文字母、数字或下划线组成", fieldName, c);
                return false;
            }
        }

        errorString = null;
        return true;
    }
}
