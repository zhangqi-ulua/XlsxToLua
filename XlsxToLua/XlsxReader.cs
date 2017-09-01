using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ExcelDataReader;

public class XlsxReader
{
	/// <summary>
	/// 将指定Excel文件的内容读取到DataSet中
	/// </summary>
	public static DataSet ReadXlsxFile(string filePath, out string errorString)
	{
		// 检查文件是否存在且没被打开
		FileState fileState = Utils.GetFileState(filePath);
		if (fileState == FileState.Inexist)
		{
			errorString = string.Format("{0}文件不存在", filePath);
			return null;
		}
		else if (fileState == FileState.IsOpen)
		{
			errorString = string.Format("{0}文件正在被其他软件打开，请关闭后重新运行本工具", filePath);
			return null;
		}

		DataSet ds = null;

		if (Utils.IsWin)
		{
			OleDbConnection conn = null;
			OleDbDataAdapter da = null;
			try
			{
				// 初始化连接并打开
				string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"";
				Utils.Log(string.Format("Connect: {0}", filePath), ConsoleColor.Green);

				conn = new OleDbConnection(connectionString);
				conn.Open();
				Utils.Log(string.Format("Connect finished"), ConsoleColor.Green);

				// 获取数据源的表定义元数据                       
				DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

				// 必须存在数据表
				bool isFoundDateSheet = false;
				// 可选配置表
				bool isFoundConfigSheet = false;

				for (int i = 0; i < dtSheet.Rows.Count; ++i)
				{
					string sheetName = dtSheet.Rows[i]["TABLE_NAME"].ToString();

					if (sheetName == AppValues.EXCEL_DATA_SHEET_NAME)
						isFoundDateSheet = true;
					else if (sheetName == AppValues.EXCEL_CONFIG_SHEET_NAME)
						isFoundConfigSheet = true;
				}
				if (!isFoundDateSheet)
				{
					errorString = string.Format("错误：{0}中不含有Sheet名为{1}的数据表", filePath, AppValues.EXCEL_DATA_SHEET_NAME.Replace("$", ""));
					return null;
				}

				// 初始化适配器
				da = new OleDbDataAdapter();
				da.SelectCommand = new OleDbCommand(String.Format("Select * FROM [{0}]", AppValues.EXCEL_DATA_SHEET_NAME), conn);

				ds = new DataSet();
				da.Fill(ds, AppValues.EXCEL_DATA_SHEET_NAME);

				// 删除表格末尾的空行
				DataRowCollection rows = ds.Tables[AppValues.EXCEL_DATA_SHEET_NAME].Rows;
				int rowCount = rows.Count;
				for (int i = rowCount - 1; i >= AppValues.DATA_FIELD_DATA_START_INDEX; --i)
				{
					if (string.IsNullOrEmpty(rows[i][0].ToString()))
						rows.RemoveAt(i);
					else
						break;
				}

				if (isFoundConfigSheet == true)
				{
					da.Dispose();
					da = new OleDbDataAdapter();
					da.SelectCommand = new OleDbCommand(String.Format("Select * FROM [{0}]", AppValues.EXCEL_CONFIG_SHEET_NAME), conn);
					da.Fill(ds, AppValues.EXCEL_CONFIG_SHEET_NAME);
				}
			}
			catch
			{
				errorString = "错误：连接Excel失败，你可能尚未安装Office数据连接组件: http://www.microsoft.com/en-US/download/details.aspx?id=23734 \n";
				return null;
			}
			finally
			{
				// 关闭连接
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
					// 由于C#运行机制，即便因为表格中没有Sheet名为data的工作簿而return null，也会继续执行finally，而此时da为空，故需要进行判断处理
					if (da != null)
						da.Dispose();
					conn.Dispose();
				}
			}
		}
		else {
			var file = new FileInfo(filePath);
			using (var stream = new FileStream(filePath, FileMode.Open))
			{
				IExcelDataReader reader = null;
				if (file.Extension == ".xls")
				{
					reader = ExcelReaderFactory.CreateBinaryReader(stream);
				}
				else if (file.Extension == ".xlsx")
				{
					reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
				}

				if (reader == null)
				{
					errorString = "Unexpected file extension" + file.Extension + "\n";
					return null;
				}



				ds = reader.AsDataSet();
				var removeDataTableList = new System.Collections.Generic.List<DataTable>();
				foreach (DataTable da in ds.Tables)
				{
					//Utils.Log(string.Format("Table: {0}", da.TableName), ConsoleColor.Cyan);
					if (da.TableName.Equals(AppValues.EXCEL_DATA_SHEET_NAME.Replace("$", "")))
					{
						da.TableName = AppValues.EXCEL_DATA_SHEET_NAME;
					}
					else if (da.TableName.Equals(AppValues.EXCEL_CONFIG_SHEET_NAME.Replace("$", "")))
					{
						da.TableName = AppValues.EXCEL_CONFIG_SHEET_NAME;
					}
					else {
						removeDataTableList.Add(da);
					}
				}
				foreach (DataTable da in removeDataTableList)
				{
					ds.Tables.Remove(da);
				}
				//foreach (DataTable da in ds.Tables)
				//{
				//	Utils.Log(string.Format("Table: {0}", da.TableName), ConsoleColor.Cyan);
				//}

			}

			// 删除表格末尾的空行
			DataRowCollection rows = ds.Tables[AppValues.EXCEL_DATA_SHEET_NAME].Rows;
			int rowCount = rows.Count;
			for (int i = rowCount - 1; i >= AppValues.DATA_FIELD_DATA_START_INDEX; --i)
			{
				if (string.IsNullOrEmpty(rows[i][0].ToString()))
					rows.RemoveAt(i);
				else
					break;
			}
		}


		errorString = null;
		return ds;
	}
}
