-- id                               int                              id
-- testBool                         bool                             测试bool
-- testLang                         lang                             测试lang
-- testLong                         long                             测试long
-- testSimpleDict                   dict[3]                          测试简单dict
--    dictChildString               string                           简单dict下的string
--    dictChildLang                 lang                             简单dict下的lang
--    dictChildLang2                lang(itemDesc{id})               简单dict下统一声明key的lang
-- testDictIncludeArray             dict[2]                          测试dict嵌套array
--    arrayInDict1                  array[string:2]                  dict下的array1
--       [1]                        string                           array下的元素1
--       [2]                        string                           array下的元素2
--    arrayInDict2                  array[int:2]                     dict下的array2
--       [1]                        int                              array下的元素1
--       [2]                        int                              array下的元素2
-- testFloat                        float                            测试float
-- testString                       string                           测试string
-- testSimpleArray                  array[lang:2]                    测试简单array
--    [1]                           lang                             1
--    [2]                           lang                             2
-- testSimpleArray2                 array[lang(itemName{id}):2]      测试简单array2，其中子元素为统一key名的lang型
--    [1]                           lang(itemName{id})               1
--    [2]                           lang(itemName{id})               2
-- testTableString1                 tableString[k:#1(int)|v:#true]   测试tableString1
-- testArrayIncludeArray            array[array[string:3]:2]         测试array嵌套array
--    [1]                           array[string:3]                  array下的array1
--       [1]                        string                           1
--       [2]                        string                           2
--       [3]                        string                           3
--    [2]                           array[string:3]                  array下的array2
--       [1]                        string                           1
--       [2]                        string                           2
--       [3]                        string                           3
-- testTableString2                 tableString[k:#seq|v:#1(int)]    测试tableString2
-- testArrayIncludeDict             array[dict[3]:2]                 测试array嵌套dict
--    [1]                           dict[3]                          reward1
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量
--    [2]                           dict[3]                          reward2
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量
-- testDictIncludeDict              dict[3]                          测试dict嵌套dict
--    dictInDict                    dict[2]                          dict中的dict
--       dictLang                   lang                             嵌套dict中的lang
--       dictFloat                  float                            嵌套dict中的float
--    floatInDict                   float                            dict中的float
--    boolInDict                    bool                             dict中的bool
-- testTableString3                 tableString[k:#seq|v:#table(type=#1(int),id=#2(int),count=#3(int))]   测试tableString3
-- testDate1                        date(input=yyyy年MM月dd日 HH时mm分ss秒|toLua=yyyy/MM/dd HH:mm:ss)   测试date（自定义日期输入、导出至lua文件的格式）
-- testDate2                        date(toLua=#dateTable)           测试date（使用默认的日期输入格式，以dateTable形式导出至lua文件）
-- testDate3                        date(input=#1970sec|toLua=yyyy年MM月dd日 HH时mm分ss秒)   测试date（以距离1970年秒数输入、导出至lua文件采用自定义日期格式）
-- testDate4                        date(input=#1970msec|toLua=#1970sec)   测试date（以距离1970年毫秒数输入、导出至lua文件采用距离1970年秒数）
-- testTime1                        time(input=HH时mm分ss秒|toLua=HH:mm:ss)   测试time（自定义时间输入、导出至lua文件的格式）
-- testTime2                        time(toLua=#sec)                 测试time（使用默认的时间输入格式，导出至lua文件采用距离0点的秒数）
-- testTime3                        time(input=#sec)                 测试date（以距离0点的秒数输入，导出至lua文件采用默认的时间格式）
-- testExportJsonSpecialString      dict[2]                          测试导出json时，字符串中含有括号、转义引号的特殊情况
--    testExportJsonArrayInDict     array[string:2]                  dict下的array
--       [1]                        string                           1
--       [2]                        string                           2
--    testExportJsonStringInDict    string                           普通string
-- testJson                         json                             测试json

return {
	[100001] = {
		testBool = false,
		testLang = "",
		testLong = -1,
		testSimpleDict = nil,
		testDictIncludeArray = nil,
		testFloat = 0,
		testString = "",
		testSimpleArray = nil,
		testSimpleArray2 = {
			[1] = "小号经验药水",
			[2] = "小号经验药水",
		},
		testTableString1 = nil,
		testArrayIncludeArray = nil,
		testTableString2 = nil,
		testArrayIncludeDict = nil,
		testDictIncludeDict = nil,
		testTableString3 = nil,
		testDate1 = "2016/07/01 00:00:00",
		testDate2 = os.date("!*t", 1467331200),
		testDate3 = "2016年07月01日 00时00分00秒",
		testDate4 = 1467331200,
		testTime1 = "00:00:00",
		testTime2 = 0,
		testTime3 = "00:00:00",
		testExportJsonSpecialString = {
			testExportJsonArrayInDict = {
				[1] = "正常1",
				[2] = "",
			},
			testExportJsonStringInDict = "正常2",
		},
		testJson = {
			testbool = true,
			testNumber1 = "12",
			testNumber2 = "-3.5",
		},
	},
	[100002] = {
		testBool = true,
		testLang = "使用增加英雄经验100点",
		testLong = 0,
		testSimpleDict = {
			dictChildString = "",
			dictChildLang = "使用增加英雄经验200点",
			dictChildLang2 = "使用增加英雄经验200点",
		},
		testDictIncludeArray = {
			arrayInDict1 = {
				[1] = "1",
				[2] = "2",
			},
			arrayInDict2 = nil,
		},
		testFloat = 1,
		testString = "1",
		testSimpleArray = {
			[1] = "",
			[2] = "",
		},
		testSimpleArray2 = {
			[1] = "中号经验药水",
			[2] = "中号经验药水",
		},
		testTableString1 = {
			[10001] = true,
		},
		testArrayIncludeArray = nil,
		testTableString2 = {
			[1] = 10001,
		},
		testArrayIncludeDict = {
			[1] = {
				type = 1,
				id = 2,
				count = 3,
			},
			[2] = nil,
		},
		testDictIncludeDict = {
			dictInDict = nil,
			floatInDict = 1,
			boolInDict = true,
		},
		testTableString3 = {
			[1] = {
				type = 1,
				id = 10001,
				count = 500,
			},
		},
		testDate1 = "2016/07/01 01:00:01",
		testDate2 = os.date("!*t", 1467334801),
		testDate3 = "2016年07月01日 01时00分01秒",
		testDate4 = 1467334801,
		testTime1 = "00:32:00",
		testTime2 = 1920,
		testTime3 = "00:32:00",
		testExportJsonSpecialString = {
			testExportJsonArrayInDict = {
				[1] = "{带括号]",
				[2] = "",
			},
			testExportJsonStringInDict = "[带括号}",
		},
		testJson = {
			[1] = "a",
			[2] = "b",
			[3] = "c",
			[4] = 12,
		},
	},
	[100003] = {
		testBool = true,
		testLang = "使用增加英雄经验200点",
		testLong = -1467331200000,
		testSimpleDict = {
			dictChildString = "1",
			dictChildLang = "",
			dictChildLang2 = "使用增加英雄经验500点",
		},
		testDictIncludeArray = {
			arrayInDict1 = {
				[1] = "",
				[2] = "",
			},
			arrayInDict2 = {
				[1] = 1,
				[2] = 2,
			},
		},
		testFloat = 1.5,
		testString = "a",
		testSimpleArray = {
			[1] = "使用增加英雄经验100点",
			[2] = "",
		},
		testSimpleArray2 = {
			[1] = "大号经验药水",
			[2] = "大号经验药水",
		},
		testTableString1 = {
			[10002] = true,
		},
		testArrayIncludeArray = {
			[1] = {
				[1] = "1",
				[2] = "2",
				[3] = "3",
			},
			[2] = nil,
		},
		testTableString2 = {
			[1] = 10002,
		},
		testArrayIncludeDict = {
			[1] = {
				type = 4,
				id = 5,
				count = 6,
			},
			[2] = nil,
		},
		testDictIncludeDict = {
			dictInDict = {
				dictLang = "",
				dictFloat = 2,
			},
			floatInDict = 2,
			boolInDict = true,
		},
		testTableString3 = {
			[1] = {
				type = 1,
				id = 10001,
				count = 500,
			},
		},
		testDate1 = "2016/10/10 00:00:00",
		testDate2 = os.date("!*t", 1476057600),
		testDate3 = "2016年10月10日 00时00分00秒",
		testDate4 = 1476057600,
		testTime1 = "10:10:00",
		testTime2 = 36600,
		testTime3 = "10:10:00",
		testExportJsonSpecialString = {
			testExportJsonArrayInDict = {
				[1] = "带引号\"",
				[2] = "",
			},
			testExportJsonStringInDict = "带\\\"引号",
		},
		testJson = {
			position = {
				x = 1,
				y = -0.5,
			},
			field = {
				[1] = "a",
				[2] = "b",
				[3] = "value3",
			},
		},
	},
	[100004] = {
		testBool = false,
		testLang = "使用增加英雄经验500点",
		testLong = 1467331200000,
		testSimpleDict = {
			dictChildString = "2",
			dictChildLang = "使用增加英雄经验1000点",
			dictChildLang2 = "使用增加英雄经验1000点",
		},
		testDictIncludeArray = {
			arrayInDict1 = {
				[1] = "3",
				[2] = "4",
			},
			arrayInDict2 = {
				[1] = 5,
				[2] = 6,
			},
		},
		testFloat = 1.25,
		testString = "ab",
		testSimpleArray = {
			[1] = "使用增加英雄经验200点",
			[2] = "使用增加英雄经验500点",
		},
		testSimpleArray2 = {
			[1] = "超级经验药水",
			[2] = "超级经验药水",
		},
		testTableString1 = {
			[10001] = true,
			[10003] = true,
			[10021] = true,
		},
		testArrayIncludeArray = {
			[1] = {
				[1] = "4",
				[2] = "",
				[3] = "6",
			},
			[2] = {
				[1] = "",
				[2] = "7",
				[3] = "8",
			},
		},
		testTableString2 = {
			[1] = 10001,
			[2] = 10003,
			[3] = 10021,
		},
		testArrayIncludeDict = {
			[1] = {
				type = 7,
				id = 8,
				count = 9,
			},
			[2] = {
				type = 10,
				id = 11,
				count = 12,
			},
		},
		testDictIncludeDict = {
			dictInDict = {
				dictLang = "使用增加英雄经验100点",
				dictFloat = 1.5,
			},
			floatInDict = 2.5,
			boolInDict = false,
		},
		testTableString3 = {
			[1] = {
				type = 1,
				id = 10001,
				count = 500,
			},
			[2] = {
				type = 2,
				id = 10002,
				count = 10,
			},
		},
		testDate1 = "2016/10/10 10:10:10",
		testDate2 = os.date("!*t", 1476094210),
		testDate3 = "2016年10月10日 10时10分10秒",
		testDate4 = 1476094210,
		testTime1 = "03:00:32",
		testTime2 = 10832,
		testTime3 = "03:00:32",
		testExportJsonSpecialString = {
			testExportJsonArrayInDict = {
				[1] = "混{{\"\\]合}",
				[2] = "",
			},
			testExportJsonStringInDict = "混{{\\\"]合}",
		},
		testJson = {
			testString1 = "测试\"引号\"",
			testString2 = "测试换行\n第二行",
		},
	},
}
