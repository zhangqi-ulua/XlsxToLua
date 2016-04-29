-- id                               int                              道具ID
-- type                             int                              道具类型 （1：经验道具，2：装备，3：装备碎片，4：英雄灵魂石）
-- subType                          int                              子类型
-- name                             lang                             名称
-- desc                             lang(itemDesc{id})               描述
-- quality                          int                              品质
-- icon                             string                           图标
-- sellPrice                        int                              铜币卖出价格（填-1表示不允许卖）

return {
	[100001] = {
		type = 1,
		subType = -1,
		name = "小号经验药水",
		desc = "使用增加英雄经验100点",
		quality = 1,
		icon = "item1",
		sellPrice = 500,
	},
	[100002] = {
		type = 1,
		subType = -1,
		name = "中号经验药水",
		desc = "使用增加英雄经验200点",
		quality = 2,
		icon = "item1",
		sellPrice = 800,
	},
	[200001] = {
		type = 2,
		subType = -1,
		name = "盾牌",
		desc = "盾牌",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200002] = {
		type = 2,
		subType = -1,
		name = "弓箭",
		desc = "弓箭",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200003] = {
		type = 2,
		subType = -1,
		name = "长矛",
		desc = "长矛",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200004] = {
		type = 2,
		subType = -1,
		name = "护甲",
		desc = "护甲",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200005] = {
		type = 2,
		subType = -1,
		name = "头盔",
		desc = "头盔",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200006] = {
		type = 2,
		subType = -1,
		name = "匕首",
		desc = "匕首",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200007] = {
		type = 2,
		subType = -1,
		name = "禅杖",
		desc = "禅杖",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
	[200008] = {
		type = 2,
		subType = -1,
		name = "大刀",
		desc = "大刀",
		quality = 1,
		icon = "item1",
		sellPrice = 800,
	},
}
