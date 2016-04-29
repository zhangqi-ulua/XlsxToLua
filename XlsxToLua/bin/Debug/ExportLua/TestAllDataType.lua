-- id                               int                              id
-- testBool                         bool                             测试bool
-- testLang                         lang                             测试lang
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
--    [1]                           lang(itemName{id})               测试tableString1
--    [2]                           lang(itemName{id})               测试array嵌套array
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

return {
	[100001] = {
		testBool = false,
		testLang = "",
		testSimpleDict = nil,
		testDictIncludeArray = nil,
		testFloat = 0,
		testString = "",
		testSimpleArray = nil,
		testSimpleArray2 = {
			[1] = "小号经验药水",
			[2] = "小号经验药水",
		},
		testTableString2 = {
		},
		testArrayIncludeDict = nil,
		testDictIncludeDict = nil,
		testTableString3 = {
		},
	},
	[100002] = {
		testBool = true,
		testLang = "使用增加英雄经验100点",
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
	},
	[100003] = {
		testBool = true,
		testLang = "使用增加英雄经验200点",
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
	},
	[100004] = {
		testBool = false,
		testLang = "使用增加英雄经验500点",
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
	},
}
