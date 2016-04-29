# XlsxToLua
####Excel表格数据导出为Lua table形式的工具，兼带数据检查功能

希望这个工具能为今后ulua手机游戏项目带来便利，使得策划可以用Excel进行配表，从而享受Excel各种强大的功能，而程序可以直接读取lua table定义的数据使用，省去如果用CSV作为数据表所导致的每个文件都得专门去写解析文件并构造数据结构的麻烦。同时各种预设检查规则以及可以自己扩展的检查规则可以帮助策划、程序进行数据导出前的查错，避免因为数据填写错误导致的上线事故<br/>

如果在使用中您觉得有些地方需要优化或者您有更好的方案，欢迎交流 我的QQ及对应邮箱是：2246549866@qq.com <br/>

推荐大家到ulua社区 [http://www.ulua.org/](http://www.ulua.org/) 下载使用非常棒的ulua热更新方案<br/>
感谢蒙大神的 [<b>CSToLua</b>](https://github.com/topameng/CsToLua)(https://github.com/topameng/CsToLua) 以及新推出的[<b>tolua#</b>](https://github.com/topameng/tolua)(https://github.com/topameng/tolua) 对ulua开发unity游戏带来的巨大效率提升<br/>
感谢众多为ulua社区做出贡献的大神 (http://bbs.ulua.org/article/ulua/uluacstoluagongxianyingxiongbang.html)<br/>
______________________________________________________________________________<br/>
####更新日志<br/>
这里仅列举版本新增功能的大致介绍，详细使用请参考“使用说明.txt”<br/>
======= V2.0 2016.4.29 =======<br/>

1.lang型字段增加一种新的数据声明方式：之前版本只能手工在每个单元格中填写具体的lang.txt中的key名。但考虑到key名一般是有规律的，往往是一个固定的前缀后面跟本行数据中某个字段的填写值（一般是id值）。本工具在此版本新增lang(itemDesc{id})声明方式，由程序自动组合成key值，不再需要在单元格中逐个手工填写。当然，不仅是前缀，也支持后缀，并且支持读取任意字段的值来拼成key名<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/v2.0/lang%E6%96%B0%E5%A2%9E%E5%A3%B0%E6%98%8E%E6%96%B9%E5%BC%8F.png)<br/>
2.新增整表检查功能，之前版本只能对字段按指定规则进行检查，而一张表格中往往存在字段之间、数据行之间的逻辑关系，这需要整表的检查规则来保证数据完整、逻辑关联正确等，详见样例表格HeroEquipment<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/v2.0/%E6%95%B4%E8%A1%A8%E6%A3%80%E6%9F%A5%E8%A7%84%E5%88%99.png)<br/>
3.新增按自定义索引方式导出lua文件的功能，之前版本只能是逐字段的导出，但程序在使用数据时往往需要自行组织数据的索引嵌套结构使得程序运行时能快速查找指定数据。本版本提供这样的功能，使得生成的lua文件中的table就是按一系列指定索引嵌套层次生成好的，直接读取使用即可，再也不用手工写代码实现<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/v2.0/%E8%87%AA%E5%AE%9A%E4%B9%89%E7%B4%A2%E5%BC%95%E5%AF%BC%E5%87%BA%E8%A7%84%E5%88%99.png)<br/>
______________________________________________________________________________<br/>
####运行截图<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic1.jpg)<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic2.png)<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic3.png)<br/>
