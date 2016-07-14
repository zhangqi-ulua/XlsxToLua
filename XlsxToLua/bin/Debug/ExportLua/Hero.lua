-- heroId                           int                              英雄ID
-- name                             lang                             英雄名称
-- rare                             int                              稀有度（11-13）
-- type                             int                              英雄职业（1：法师，2：战士，3：牧师，4：勇士）
-- defaultStar                      int                              英雄初始星数
-- isOpen                           bool                             当前是否在游戏中开放（即可在英雄图鉴看到，可以被抽卡抽到）

return {
	[1] = {
		name = "英雄法师",
		rare = 11,
		type = 1,
		defaultStar = 1,
		isOpen = true,
	},
	[2] = {
		name = "英雄战士",
		rare = 11,
		type = 2,
		defaultStar = 1,
		isOpen = true,
	},
	[3] = {
		name = "英雄牧师",
		rare = 11,
		type = 3,
		defaultStar = 1,
		isOpen = false,
	},
	[4] = {
		name = "英雄勇士",
		rare = 11,
		type = 4,
		defaultStar = 1,
		isOpen = false,
	},
}
