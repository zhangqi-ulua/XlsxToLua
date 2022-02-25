# XlsxToLua
#### Excel表格数据导出为Lua table形式的工具，兼带数据检查功能

希望这个工具能为今后ulua手机游戏项目带来便利，使得策划可以用Excel进行配表，从而享受Excel各种强大的功能，而程序可以直接读取lua table定义的数据使用，省去如果用CSV作为数据表所导致的每个文件都得专门去写解析文件并构造数据结构的麻烦。同时各种预设检查规则以及可以自己扩展的检查规则可以帮助策划、程序进行数据导出前的查错，避免因为数据填写错误导致的上线事故<br/>

如果在使用中您觉得有些地方需要优化或者您有更好的方案，欢迎交流 我的QQ及对应邮箱是：2246549866@qq.com    我所有开源项目的反馈交流QQ群为：132108644<br/>

推荐大家到ulua社区 [http://www.ulua.org/](http://www.ulua.org/) 下载使用非常棒的ulua热更新方案<br/>
感谢蒙大神新推出的[<b>tolua#</b>](https://github.com/topameng/tolua)(https://github.com/topameng/tolua) 对ulua开发unity游戏带来的巨大效率提升<br/>
感谢众多为ulua社区做出贡献的大神 (http://bbs.ulua.org/article/ulua/uluacstoluagongxianyingxiongbang.html)<br/>
______________________________________________________________________________<br/>

#### 更新日志<br/>
这里仅列举最近一个版本的更新说明，全部版本更新记录，请查看“版本更新.md”<br/><br/>

======= V7.2 2018.11.4 ======<br/>
1、XlsxToLua增加返回值，0为成功，-1为发现错误，从而方便bat调用时通过%errorlevel%判断导出是否成功从而进一步处理（感谢“游~”建议）<br/>
2、导出C#和Java类时，时间型如果声明导出为时间戳形式，则将字段设为数值型，而不再是统一设为时间型。导出的时间戳改为以本地时区计，而不再是UTC<br/>
3、导出C#和Java类时，dict型如果下属字段为同一类型，可以将Dictionary、HashMap型的值类型设为具体的类型，而不是统一用object<br/>
4、XlsxToLuaGUI工具支持拖拽文件或文件夹到输入框来填写路径<br/>


#### 运行截图<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic1.jpg)<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic2.png)<br/>
![](https://github.com/zhangqi-ulua/XlsxToLua/blob/master/screenshots/pic3.png)<br/>

## 赞助
如果您觉得软件还不错，并且愿意请作者喝杯咖啡的话，欢迎打赏<br/>
<img src="https://github.com/zhangqi-ulua/FiddlerHeadConvertor/blob/main/%E4%BD%BF%E7%94%A8%E8%AF%B4%E6%98%8E/wechat.jpg" width="40%">
<img src="https://github.com/zhangqi-ulua/FiddlerHeadConvertor/blob/main/%E4%BD%BF%E7%94%A8%E8%AF%B4%E6%98%8E/alipay.jpg" width="40%">
