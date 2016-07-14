using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class MySQLOperateHelper
{
    // MySQL支持的用于定义Schema名的参数名
    private static string[] _DEFINE_SCHEMA_NAME_PARAM = { "Database", "Initial Catalog" };

    private const string _SELECT_ALL_DATA_SQL = "SELECT * FROM {0}";
    private const string _SELECT_COLUMN_INFO_SQL = "SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'";

    private static MySqlConnection _conn = null;
    private static string _schemaName = null;

    // 数据库中存在的数据表名
    public static List<string> ExistTableNames { get; private set; }

    public static bool ConnectToDatabase(out string errorString)
    {
        if (AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_MYSQL_CONNECT_STRING))
        {
            // 提取MySQL连接字符串中的Schema名
            string connectString = AppValues.ConfigData[AppValues.APP_CONFIG_KEY_MYSQL_CONNECT_STRING];
            foreach (string legalSchemaNameParam in _DEFINE_SCHEMA_NAME_PARAM)
            {
                int defineStartIndex = connectString.IndexOf(legalSchemaNameParam, StringComparison.CurrentCultureIgnoreCase);
                if (defineStartIndex != -1)
                {
                    // 查找后面的等号
                    int equalSignIndex = -1;
                    for (int i = defineStartIndex + legalSchemaNameParam.Length; i < connectString.Length; ++i)
                    {
                        if (connectString[i] == '=')
                        {
                            equalSignIndex = i;
                            break;
                        }
                    }
                    if (equalSignIndex == -1 || equalSignIndex + 1 == connectString.Length)
                    {
                        errorString = string.Format("MySQL数据库连接字符串（\"{0}\"）中\"{1}\"后需要跟\"=\"进行Schema名声明", connectString, legalSchemaNameParam);
                        return false;
                    }
                    else
                    {
                        // 查找定义的Schema名，在参数声明的=后面截止到下一个分号或字符串结束
                        int semicolonIndex = -1;
                        for (int i = equalSignIndex + 1; i < connectString.Length; ++i)
                        {
                            if (connectString[i] == ';')
                            {
                                semicolonIndex = i;
                                break;
                            }
                        }
                        if (semicolonIndex == -1)
                            _schemaName = connectString.Substring(equalSignIndex + 1).Trim();
                        else
                            _schemaName = connectString.Substring(equalSignIndex + 1, semicolonIndex - equalSignIndex - 1).Trim();
                    }

                    break;
                }
            }
            if (_schemaName == null)
            {
                errorString = string.Format("MySQL数据库连接字符串（\"{0}\"）中不包含Schema名的声明，请在{1}中任选一个参数名进行声明", connectString, Utils.CombineString(_DEFINE_SCHEMA_NAME_PARAM, ","));
                return false;
            }

            try
            {
                _conn = new MySqlConnection(connectString);
                _conn.Open();
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    ExistTableNames = new List<string>();
                    DataTable schemaInfo = _conn.GetSchema(System.Data.SqlClient.SqlClientMetaDataCollectionNames.Tables);
                    foreach (DataRow info in schemaInfo.Rows)
                        ExistTableNames.Add(info.ItemArray[2].ToString());

                    errorString = null;
                    return true;
                }
                else
                {
                    errorString = "未知错误";
                    return true;
                }
            }
            catch (MySqlException exception)
            {
                errorString = exception.Message;
                return false;
            }
        }
        else
        {
            errorString = string.Format("未在config配置文件中以名为\"{0}\"的key声明连接MySQL的字符串", AppValues.APP_CONFIG_KEY_MYSQL_CONNECT_STRING);
            return false;
        }
    }

    public static DataTable ReadDatabaseTable(string tableName)
    {
        MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_ALL_DATA_SQL, _CombineDatabaseTableFullName(tableName)), _conn);
        return _ExecuteSqlCommand(cmd);
    }

    public static DataTable GetColumnInfo(string tableName)
    {
        MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_COLUMN_INFO_SQL, _schemaName, tableName), _conn);
        return _ExecuteSqlCommand(cmd);
    }

    private static DataTable _ExecuteSqlCommand(MySqlCommand cmd)
    {
        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    /// <summary>
    /// 将数据库的表名连同Schema名组成形如'SchemaName'.'tableName'的字符串
    /// </summary>
    private static string _CombineDatabaseTableFullName(string tableName)
    {
        return string.Format("`{0}`.`{1}`", _schemaName, tableName);
    }
}
