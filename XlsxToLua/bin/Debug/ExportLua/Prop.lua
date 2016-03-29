-- id                               int                              道具ID
-- type                             int                              道具类型 （1：经验道具，2：装备，3：装备碎片，4：英雄灵魂石）
-- subType                          int                              子类型
-- name                             lang                             名称
-- desc                             lang                             描述
-- quality                          int                              品质
-- icon                             string                           图标
-- sellPrice                        int                              铜币卖出价格（填-1表示不允许卖）

return {
	[1] = {
		type = 1,
		subType = -1,
		name = "小号经验药水",
		desc = "使用增加英雄经验100点",
		quality = 1,
		icon = "item1",
		sellPrice = 500,
	},
	[2] = {
		type = 1,
		subType = -1,
		name = "中号经验药水",
		desc = "使用增加英雄经验200点",
		quality = 2,
		icon = "item1",
		sellPrice = 800,
	},
}
