-- id                               int                              ID
-- type                             int                              类型
-- params                           mapString[content=string,position=(x=float,y=float),buttonName=string,isAutoHide=bool]   不同类型下所需的参数

return {
	[1] = {
		type = 1,
		params = {
			position = {
				x = 0.5,
				y = 3,
			},
			content = "测试引号\"以及换行\n,让我们继续冒险旅程吧",
		},
	},
	[2] = {
		type = 2,
		params = {
			buttonName = "继续",
			isAutoHide = true,
		},
	},
	[3] = {
		type = 2,
		params = {
			buttonName = "继续",
			isAutoHide = false,
		},
	},
	[4] = {
		type = 2,
		params = {
			buttonName = "继续",
			isAutoHide = true,
		},
	},
	[5] = {
		type = 2,
		params = {
			buttonName = "继续",
			isAutoHide = false,
		},
	},
}
