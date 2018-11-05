# XlsxToLua
####Excel表格数据导出为Lua table形式的工具，兼带数据检查功能

希望这个工具能为今后ulua手机游戏项目带来便利，使得策划可以用Excel进行配表，从而享受Excel各种强大的功能，而程序可以直接读取lua table定义的数据使用，省去如果用CSV作为数据表所导致的每个文件都得专门去写解析文件并构造数据结构的麻烦。同时各种预设检查规则以及可以自己扩展的检查规则可以帮助策划、程序进行数据导出前的查错，避免因为数据填写错误导致的上线事故<br/>

如果在使用中您觉得有些地方需要优化或者您有更好的方案，欢迎交流 我的QQ及对应邮箱是：2246549866@qq.com    我所有开源项目的反馈交流QQ群为：132108644<br/>

推荐大家到ulua社区 [http://www.ulua.org/](http://www.ulua.org/) 下载使用非常棒的ulua热更新方案<br/>
感谢蒙大神的 [<b>CSToLua</b>](https://github.com/topameng/CsToLua)(https://github.com/topameng/CsToLua) 以及新推出的[<b>tolua#</b>](https://github.com/topameng/tolua)(https://github.com/topameng/tolua) 对ulua开发unity游戏带来的巨大效率提升<br/>
感谢众多为ulua社区做出贡献的大神 (http://bbs.ulua.org/article/ulua/uluacstoluagongxianyingxiongbang.html)<br/>
______________________________________________________________________________<br/>

感谢mimicryryu fork出了替换微软读取Excel类库为开源的ExcelDataReader，从而支持在mac系统上运行XlsxToLua工具<br/>

地址为：https://github.com/mimicryryu/XlsxToLua<br/>
______________________________________________________________________________<br/><br/>
####更新日志<br/>
这里仅列举版本新增功能的大致介绍，详细使用请参考PDF版本的使用说明<br/><br/>

======= V7.2 2018.11.4 ======<br/>
1、XlsxToLua增加返回值，0为成功，-1为发现错误，从而方便bat调用时通过%errorlevel%判断导出是否成功从而进一步处理（感谢“游~”建议）<br/>
2、导出C#和Java类时，时间型如果声明导出为时间戳形式，则将字段设为数值型，而不再是统一设为时间型。导出的时间戳改为以本地时区计，而不再是UTC<br/>
3、导出C#和Java类时，dict型如果下属字段为同一类型，可以将Dictionary、HashMap型的值类型设为具体的类型，而不是统一用object<br/>
4、XlsxToLuaGUI工具支持拖拽文件或文件夹到输入框来填写路径<br/>

======= V7.1 2017.12.5 ======<br/>
1、支持导出指定Excel文件夹下所有子文件夹中的Excel文件，并支持将生成的文件按原目录结构存储<br/>
2、针对上面新增功能，同步修改XlsxToLuaGUI工具，通过文件选择窗口进行各功能指定Excel文件选择后，将以新增方式追加到配置文本框中，而不再是覆盖方式<br/>
3、值范围检查规则适用于string、lang型，用于确保字符串长度在要求的范围内<br/>

======= V7.0 2017.10.25 ======<br/>
1、增加用于灵活配置不定参数的mspString类型以及对应的表格检查规则<br/>
2、修改LitJson类库，使得输出json中的中文等字符原样显示，而不是以Unicode编码形式显示<br/>

======= V6.7 2017.9.18 ======<br/>
1、取消array、dict类型最多只能嵌套两层的限制，取消dict型下属子元素类型不能为json和tableString的限制<br/>
2、导出csv文件中，如果array、dict型字段的某个数据用-1设为无效，其下属array、dict型子字段的数据值也会被输出为-1<br/>

======= V6.6 2017.8.18 ======<br/>
1、增加另一种导出json文件的形式，即为各行数据以主键列值为key，各字段信息为value，包含在一个json object<br/>

======= V6.5 2017.8.17 ======<br/>
1、增加导出csv对应Java类文件的功能<br/>
2、对导出csv对应C#类文件功能的运行参数进行调整<br/>

======= V6.4 2017.3.20 ======<br/>
1、支持某字段仅进行客户端或服务器导出<br/>

======= V6.4 2017.3.20 ======<br/>
1、支持某字段仅进行客户端或服务器导出<br/>

======= V6.3 2017.2.20 ======<br/>
1、增加导出csv对应C#类文件的功能<br/>

======= V6.2 2017.2.14 ======<br/>
1、增加新的运行参数-except用于配置本次要忽略导出的Excel文件名<br/>
2、配置额外导出csv、json文件时，可用$all声明对本次要导出为lua的表格均进行导出<br/>
3、XlsxToLuaGUI工具可以选择使用相对路径进行配置<br/>

======= V6.1 2016.11.11 ======<br/>
1、Excel表格的config配置表中增加exportDatabaseWriteNullForEmptyString选项，可设置string型字段中的空单元格导出至MySQL数据库时为NULL，而不是默认的空字符串<br/>
2、增加显示用OleDb方式读取各张Excel表所用时间的功能<br/>
3、读取Excel表时，自动忽略表格末尾所有主键列为空的行<br/>
4、float型字段在C#中改为实际用double型存储，float型仅表明是小数类型<br/>
5、忽略对Excel自动生成的以~$开头的临时文件的读取<br/>

======= V6.0 2016.10.8 ======<br/>
1、增加将Excel文件额外导出为csv文件的功能<br/>
2、增加将Excel文件额外导出为json文件的功能<br/>

======= V5.2 2016.9.19 ======<br/>

1、增加json这种新的数据类型<br/>
2、Excel表格配置中增加addKeyToLuaTable选项，默认为false，如果为true则表格在导出为lua table时，将主键列也作为table中的元素<br/>

======= V5.1 2016.9.8 ======<br/>

1、导出Excel文件到MySQL数据库创建表格时允许自定义额外参数，如设定Charset等<br/>
2、Excel文件中增加exportDatabaseTableComment配置，可将说明信息作为导出的MySQL数据库表格的Comment。MySQLToExcel工具同步增加直接将数据库表Comment导出为exportDatabaseTableComment配置的功能<br/>
3、增加非法值检查规则，与有效值检查相反<br/>

======= V5.0 2016.7.14 ======<br/>

1、ref检查规则允许声明不进行ref检查的排除值（现在在进行ref检查时，可以很方便地排除那些具有特殊含义的不需要进行检查的数值）<br/>
2、增加date、time两种时间相关的数据类型，并扩展检查函数对这两种新类型的支持，扩展MySQLToExcel工具对导出为date、time型的支持<br/>
3、增加long数据类型，比int型可以表示更大的数值，并扩展检查函数对这种新类型的支持<br/>
4、增加检测一行数据中某个字段的值是否大于或等于另一个字段的值的检查规则（支持与array、dict下属字段进行比较）<br/>
5、扩展ref检查规则，使其支持与array、dict下属字段进行比较<br/>
6、对MySQLToExcel工具生成的Excel文件进行美化，设置每列的最大宽度，对于在同一行中显示内容会超过最大列宽的列强制设为最大列宽，而使内容换行显示<br/>

======= V4.1 2016.6.6 ======<br/>

1、增加一个功能开关设置是否强制要求int、float型字段中所填数据不能为空，不再像以前一样强制不能为空，以适应更多项目的实际需要。选项包含以下2种：强制不能为空，可以为空且对应值赋值为nil。但即便是开启了允许空值的参数仍旧可以通过notEmpty进行非空检查。同时本工具为适应此改变，修改notEmpty等检查规则并更新XlsxToLuaGUI辅助工具使其支持此新增参数<br/>
2、bool型字段的值除了之前允许的0和1外，也允许直接填写英文false和true<br/>
3、优化从MySQL数据库导出为XlsxToLua工具支持的Excel表格形式以及通过XlsxToLua工具导出到数据库中，一些特殊类型字段以及空值的处理<br/>
======= V4.0 2016.5.30 ======<br/>

1、string类型声明时增加string(trim)声明方式，此列中所填写内容将被自动去掉首尾空白字符<br/>
2、增加MySQL数据表导出为本工具所要求的Excel文件格式的辅助工具，对于之前使用数据库作为数据表存储方式的项目，可以更快地转换到XlsxToLua工具所支持的Excel配表方式。本工具默认还会进行以下格式设置：对各列设置为配置的背景色、用于声明字段信息的前五行设置实现边框并进行窗口冻结等，项目可根据自己喜好修改配置或者代码实现其他格式风格<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/v4.0/MySQLToExcel.png)<br/>
======= V3.5 2016.5.9 =======<br/>

1、新增只导出指定的部分Excel表格的功能。考虑到很多情况下，数值策划一次仅修改一张表格的数据，这时通过本工具进行所有表格的检查、导出lua文件、导出到MySQL数据库便显得没有必要，故增加部分导出功能适应这种普遍情况<br/>
2、新增GUI操作本程序并可创建bat批处理脚本，这样不懂程序的人员可以通过图形化程序自己设置运行参数<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/v3.5/gui.png)<br/>
======= V3.0 2016.5.7 =======<br/>

1、新增数据完整性检查规则，上一版本中增加了整表检查规则，使得表格中的不同行或不同字段间的逻辑关系可以通过自定义检查函数实现查错。但之前提到的HeroEquipment表中要进行检查是否填写了英雄在所有品阶下四件装备的穿戴信息。之前2.0版本中只能自己写函数检查。但类似的检查要求可能很普遍，所以我将这样的检查要求抽象成一个新的检查规则，使得只需进行规则配置，无需再写去相应的整表检查规则<br/>
2、新增将Excel表格数据导出到MySQL数据库的功能，这样数值策划维护Excel数据表，然后通过本工具不仅可以生成相应的lua table供前端程序使用，也能直接同步配置表格数据到服务器端的数据库中<br/>

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
