-- id                               int                              ID
-- testExportDateToDatetime         date(input=#1970sec)             测试导出date型为MySQL的datetime类型
-- testExportDateToReferenceSec     date(input=yyyy/MM/dd HH:mm:ss|toDatabase=#1970sec)   测试导出date型为距1970年的秒数
-- testExportDateToReferenceMsec    date(input=yyyy/MM/dd HH:mm:ss|toDatabase=#1970msec)   测试导出date型为距1970年的毫秒数
-- testExportDateToDate             date(input=yyyy/MM/dd HH:mm:ss)   测试导出无时分秒的date型为MySQL的date类型
-- testExportDateToVarchar          date(toDatabase=yyyy年MM月dd日 HH时mm分ss秒)   测试导出date型为MySQL的varchar类型
-- testExportTimeToTime             time(input=#sec)                 测试导出time型为MySQL的time类型
-- testExportTimeToReferenceSec     time(input=HH时mm分ss秒|toDatabase=#sec)   测试导出time型为距0点的秒数
-- testExportTimeToVarchar          time(toDatabase=HH时mm分ss秒)       测试导出time型为MySQL的varchar类型

return {
	[1] = {
		testExportDateToDatetime = "2016-07-01 00:00:00",
		testExportDateToReferenceSec = "2016-07-01 00:00:00",
		testExportDateToReferenceMsec = "2016-07-01 00:00:00",
		testExportDateToDate = "2016-07-01 00:00:00",
		testExportDateToVarchar = "2016-07-01 00:00:00",
		testExportTimeToTime = "00:00:00",
		testExportTimeToReferenceSec = "00:00:00",
		testExportTimeToVarchar = "00:00:00",
	},
	[2] = {
		testExportDateToDatetime = "2016-07-01 01:00:01",
		testExportDateToReferenceSec = "2016-07-01 00:00:00",
		testExportDateToReferenceMsec = "2016-07-01 00:00:00",
		testExportDateToDate = "2016-07-01 00:00:00",
		testExportDateToVarchar = "2016-07-01 00:00:00",
		testExportTimeToTime = "00:32:00",
		testExportTimeToReferenceSec = "00:32:00",
		testExportTimeToVarchar = "00:32:00",
	},
	[3] = {
		testExportDateToDatetime = "2016-10-10 00:00:00",
		testExportDateToReferenceSec = "2016-07-01 10:10:00",
		testExportDateToReferenceMsec = "2016-07-01 10:10:00",
		testExportDateToDate = "2016-07-01 10:10:00",
		testExportDateToVarchar = "2016-07-01 10:10:00",
		testExportTimeToTime = "10:10:00",
		testExportTimeToReferenceSec = "10:10:00",
		testExportTimeToVarchar = "10:10:00",
	},
	[4] = {
		testExportDateToDatetime = "2016-10-10 10:10:10",
		testExportDateToReferenceSec = "2016-10-10 00:00:10",
		testExportDateToReferenceMsec = "2016-10-10 00:00:10",
		testExportDateToDate = "2016-10-10 00:00:10",
		testExportDateToVarchar = "2016-10-10 00:00:10",
		testExportTimeToTime = "03:00:32",
		testExportTimeToReferenceSec = "03:00:32",
		testExportTimeToVarchar = "03:00:32",
	},
}
