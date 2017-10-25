-- id                               int                              id
-- testDict                         dict[3]                          dict下的多重嵌套
--    childArray1                   array[dict[2]:2]                 dict下的array中嵌套dict
--       [1]                        dict[2]                          dict1
--          id                      int                              
--          price                   int                              
--       [2]                        dict[2]                          dict2
--          id                      int                              
--          price                   int                              
--    childArray2                   array[array[int:2]:2]            dict下的array中嵌套array
--       [1]                        array[int:2]                     array1
--          [1]                     int                              1
--          [2]                     int                              2
--       [2]                        array[int:2]                     array2
--          [1]                     int                              1
--          [2]                     int                              2
--    childDict1                    dict[7]                          dict下的dict中嵌套dict和array
--       childChildDict1            dict[3]                          dict下的dict中嵌套的dict
--          type                    int                              
--          id                      int                              
--          count                   int                              
--       childChildArray2           array[int:2]                     dict下的dict中嵌套的array
--          [1]                     int                              1
--          [2]                     int                              2
--       testTableString1           tableString[k:#1(int)|v:#true]   dict下的dict中嵌套tableString1
--       testTableString2           tableString[k:#seq|v:#1(int)]    dict下的dict中嵌套tableString2
--       testTableString3           tableString[k:#seq|v:#table(type=#1(int),id=#2(int),count=#3(int))]   dict下的dict中嵌套tableString3
--       testJson                   json                             dict下的dict中嵌套json
--       testMapString              mapString[content=string,position=(x=float,y=float),buttonName=string,isAutoHide=bool]   dict下的dict中嵌套mapString
-- testArray1                       array[dict[2]:2]                 array下的dict的多重嵌套
--    [1]                           dict[2]                          dict1
--       childDict1                 dict[5]                          array下的dict中嵌套dict1
--          testTableString1        tableString[k:#1(int)|v:#true]   array中的dict下的dict中嵌套tableString1
--          testTableString2        tableString[k:#seq|v:#1(int)]    array中的dict下的dict中嵌套tableString2
--          testTableString3        tableString[k:#seq|v:#table(type=#1(int),id=#2(int),count=#3(int))]   array中的dict下的dict中嵌套tableString3
--          testMapString           mapString[content=string,position=(x=float,y=float),buttonName=string,isAutoHide=bool]   array中的dict下的dict中嵌套mapString
--          childDict2              dict[2]                          array中的dict下的dict中嵌套dict
--             name                 string                           
--             age                  int                              
--       childDict2                 dict[3]                          array下的dict中嵌套dict2
--          testJson                json                             array中的dict下的dict中嵌套json
--          testMapString           mapString[content=string,position=(x=float,y=float),buttonName=string,isAutoHide=bool]   array中的dict下的dict中嵌套mapString
--          testArray               array[int:2]                     array中的dict下的dict中嵌套array
--             [1]                  int                              
--             [2]                  int                              
--    [2]                           dict[2]                          dict2
--       childArray1                array[int:3]                     array下的dict中嵌套array1
--          [1]                     int                              1
--          [2]                     int                              2
--          [3]                     int                              3
--       childArray2                array[string:2]                  array下的dict中嵌套array2
--          [1]                     string                           
--          [2]                     string                           
-- testArray2                       array[array[dict[2]:2]:2]        array下的array的多重嵌套
--    [1]                           array[dict[2]:2]                 childArray1
--       [1]                        dict[2]                          childArrayChildDict1
--          name                    string                           
--          score                   float                            
--       [2]                        dict[2]                          childArrayChildDict2
--          array1                  array[int:2]                     
--             [1]                  int                              1
--             [2]                  int                              2
--          array2                  array[string:2]                  
--             [1]                  string                           1
--             [2]                  string                           2
--    [2]                           array[dict[2]:2]                 childArray2
--       [1]                        dict[2]                          childArrayChildDict1
--          dict1                   dict[4]                          
--             testTableString1     tableString[k:#1(int)|v:#true]   array中的array下的dict中dict嵌套tableString1
--             testTableString2     tableString[k:#seq|v:#1(int)]    array中的array下的dict中dict嵌套tableString2
--             testTableString3     tableString[k:#seq|v:#table(type=#1(int),id=#2(int),count=#3(int))]   array中的array下的dict中dict嵌套tableString3
--             testMapString        mapString[content=string,position=(x=float,y=float),buttonName=string,isAutoHide=bool]   array中的array下的dict中dict嵌套mapString
--          testJson                json                             array中的array下的dict中dict嵌套json
--       [2]                        dict[2]                          childArrayChildDict2
--          id                      int                              
--          count                   int                              

return {
	[1] = {
		testDict = nil,
		testArray1 = nil,
		testArray2 = nil,
	},
	[2] = {
		testDict = {
			childArray1 = nil,
			childArray2 = nil,
			childDict1 = nil,
		},
		testArray1 = {
			[1] = nil,
			[2] = nil,
		},
		testArray2 = {
			[1] = nil,
			[2] = nil,
		},
	},
	[3] = {
		testDict = {
			childArray1 = {
				[1] = {
					id = 1,
					price = 2,
				},
				[2] = nil,
			},
			childArray2 = {
				[1] = {
					[1] = 5,
					[2] = 6,
				},
				[2] = nil,
			},
			childDict1 = {
				childChildDict1 = {
					type = 9,
					id = 10,
					count = 11,
				},
				childChildArray2 = nil,
				testTableString1 = nil,
				testTableString2 = nil,
				testTableString3 = nil,
				testJson = nil,
				testMapString = {
					buttonName = "继续",
					isAutoHide = true,
				},
			},
		},
		testArray1 = {
			[1] = {
				childDict1 = {
					testTableString1 = {
						[10001] = true,
						[10003] = true,
						[10021] = true,
					},
					testTableString2 = {
						[1] = 10001,
						[2] = 10003,
						[3] = 10021,
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
					testMapString = {
						buttonName = "继续",
						isAutoHide = false,
					},
					childDict2 = nil,
				},
				childDict2 = nil,
			},
			[2] = nil,
		},
		testArray2 = {
			[1] = {
				[1] = {
					name = "Tom",
					score = 95.5,
				},
				[2] = nil,
			},
			[2] = nil,
		},
	},
	[4] = {
		testDict = {
			childArray1 = {
				[1] = {
					id = 1,
					price = 2,
				},
				[2] = {
					id = 3,
					price = 4,
				},
			},
			childArray2 = {
				[1] = {
					[1] = 5,
					[2] = 6,
				},
				[2] = {
					[1] = 7,
					[2] = 8,
				},
			},
			childDict1 = {
				childChildDict1 = {
					type = 9,
					id = 10,
					count = 11,
				},
				childChildArray2 = {
					[1] = 12,
					[2] = 13,
				},
				testTableString1 = {
					[10001] = true,
					[10003] = true,
					[10021] = true,
				},
				testTableString2 = {
					[1] = 10001,
					[2] = 10003,
					[3] = 10021,
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
				testJson = {
					attack = {
						physical = 140,
						magic = 0,
						canCrit = true,
						hitRate = 0.9,
						ult = {
							[1] = {
								name = "CoupDeGrace",
								params = {
									[1] = "sector",
									[2] = 150,
									[3] = 0,
									[4] = true,
								},
								cd = 5,
							},
						},
					},
					defence = {
						physical = 40,
						magic = 0,
					},
					hp = 150,
					modelSize = {
						[1] = 3.5,
						[2] = 4,
						[3] = 8,
					},
					petPhrase = "Death to all who oppose me!",
				},
				testMapString = {
					position = {
						x = 0.5,
						y = 3,
					},
					content = "让我们继续冒险旅程吧",
				},
			},
		},
		testArray1 = {
			[1] = {
				childDict1 = {
					testTableString1 = {
						[10001] = true,
						[10003] = true,
						[10021] = true,
					},
					testTableString2 = {
						[1] = 10001,
						[2] = 10003,
						[3] = 10021,
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
					testMapString = {
						buttonName = "继续",
						isAutoHide = true,
					},
					childDict2 = {
						name = "Tom",
						age = 10,
					},
				},
				childDict2 = {
					testJson = {
						attack = {
							physical = 140,
							magic = 0,
							canCrit = true,
							hitRate = 0.9,
							ult = {
								[1] = {
									name = "CoupDeGrace",
									params = {
										[1] = "sector",
										[2] = 150,
										[3] = 0,
										[4] = true,
									},
									cd = 5,
								},
							},
						},
						defence = {
							physical = 40,
							magic = 0,
						},
						hp = 150,
						modelSize = {
							[1] = 3.5,
							[2] = 4,
							[3] = 8,
						},
						petPhrase = "Death to all who oppose me!",
					},
					testMapString = {
						position = {
							x = 0.5,
							y = 3,
						},
						content = "让我们继续冒险旅程吧",
					},
					testArray = {
						[1] = 14,
						[2] = 15,
					},
				},
			},
			[2] = {
				childArray1 = {
					[1] = 16,
					[2] = 17,
					[3] = 18,
				},
				childArray2 = {
					[1] = "hello",
					[2] = "hi",
				},
			},
		},
		testArray2 = {
			[1] = {
				[1] = {
					name = "Tom",
					score = 95.5,
				},
				[2] = {
					array1 = {
						[1] = 19,
						[2] = 20,
					},
					array2 = nil,
				},
			},
			[2] = {
				[1] = {
					dict1 = {
						testTableString1 = {
							[10001] = true,
							[10003] = true,
							[10021] = true,
						},
						testTableString2 = {
							[1] = 10001,
							[2] = 10003,
							[3] = 10021,
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
						testMapString = {
							position = {
								x = 0.5,
								y = 3,
							},
							content = "让我们继续冒险旅程吧",
						},
					},
					testJson = {
						attack = {
							physical = 140,
							magic = 0,
							canCrit = true,
							hitRate = 0.9,
							ult = {
								[1] = {
									name = "CoupDeGrace",
									params = {
										[1] = "sector",
										[2] = 150,
										[3] = 0,
										[4] = true,
									},
									cd = 5,
								},
							},
						},
						defence = {
							physical = 40,
							magic = 0,
						},
						hp = 150,
						modelSize = {
							[1] = 3.5,
							[2] = 4,
							[3] = 8,
						},
						petPhrase = "Death to all who oppose me!",
					},
				},
				[2] = {
					id = 21,
					count = 22,
				},
			},
		},
	},
}
