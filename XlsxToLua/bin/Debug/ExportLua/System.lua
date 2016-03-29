-- systemId                         int                              系统ID
-- systemName                       lang                             系统名称
-- help                             lang                             系统帮助信息
-- openCondition                    dict[3]                          开启条件（同时满足三个条件）
--    rankLimit                     int                              所需玩家等级（不限填-1）
--    vipLimit                      int                              所需Vip等级（不限填-1）
--    levelLimit                    int                              所需通关关卡（不限填-1）
-- openRewards                      array[dict[3]:2]                 开启系统后赠送物
--    [1]                           dict[3]                          奖励物1
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量
--    [2]                           dict[3]                          奖励物2
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量

return {
	[1] = {
		systemName = "普通关卡",
		help = "通过普通关卡可以获得一定几率掉落的道具和英雄经验，快来让你的英雄挑战吧！",
		openCondition = nil,
		openRewards = nil,
	},
	[2] = {
		systemName = "精英关卡",
		help = "通过精英关卡可以获得一定几率掉落的稀有英雄碎片用于英雄升阶，每天挑战次数有限哦！",
		openCondition = nil,
		openRewards = nil,
	},
	[3] = {
		systemName = "竞技场",
		help = "这里是各位英雄彼此切磋较量的场所，有丰富的奖励给予强大的英雄",
		openCondition = {
			rankLimit = 25,
			vipLimit = -1,
			levelLimit = -1,
		},
		openRewards = {
			[1] = {
				type = 1,
				id = 1,
				count = 10,
			},
			[2] = nil,
		},
	},
	[4] = {
		systemName = "商店",
		help = "在这里你可以花费金币或者钻石来兑换你想要的道具，商店会在每天9点、15点、21点进货新的一批商品哟，欢迎客官前来购买",
		openCondition = {
			rankLimit = 15,
			vipLimit = -1,
			levelLimit = -1,
		},
		openRewards = {
			[1] = {
				type = 1,
				id = 1,
				count = 5,
			},
			[2] = nil,
		},
	},
}
