# HaoKangFramework
Excited Library!!!


## 这是个什么玩意
这是个用于从壁纸网站上爬取图片的爬虫工具箱，本库采用F#语言编写，这个库可以帮助你从壁纸网站上爬取图片。
请注意，这个库遵守LGPL v3协议，以及不要用它做违法的事情，以此库造成的任何问题，均与库作者无关。

## 我如何使用它

### 搜索
1. 导入这个库到你的F#/C#项目中
2. open HaoKangFramework.Spider
3. Spiders是一个(string * ISpider) []，这里面有这个库可以使用的所有爬虫。
4. 使用TestConnection测试爬虫是否连接良好，并且选出连接良好的爬虫。
5. 使用Search函数，输入要查询的关键字，即可返回搜索结果。

### 预览和下载
1. Search应当返回一个Page seq，这是页面集合，每个页面都有一些图片或者其他的投稿，如果一个Page是空的，那么说明已经到底末页。
2. 遍历Page，Page本身是Post seq，每个Post都是一个投稿。
3. Post.Preview是预览图像，Post.Content则是一组内容，你需要使用异步工作流来获取它，或者使用Post.ContentUrl获取URL，并且使用其他工具下载。

## 已经支持的爬虫
- Konachan
- Yandere（由于网络问题尚未测试）
- Gelbooru
- Danbooru
- ATFBooru
- LoliBooru
- Rule34
- SafeBooru
- HypnoHub

## 未来将要支持的爬虫
- iwara(Videos)
- ecchi.iwara(Videos)
- iwara(Images)
- ecchi.iwara(Images)
- Chan.SankakuComplex
- Idol.SankakuComplex
- https://booru.org/top中的Boorus
- Pixiv
- ExHentai

## 已经完成的工具包
- BooruUtils（用于爬取Boorus类网站）

## 我希望加入开发

1. Fork这个项目
2. 解析你要爬取的网站
3. 在HaoKangFramework.Spiders下实现一个ISpider，并提供无参构造函数，HaoKangFramework将自动检测到这个类。
4. Pull Request，与世界分享你的成果

