# HaoKangFramework
Excited Library!!!

已经移动到[KoKo](https://github.com/Seng-Jik/KoKo)，具有更好的性能、稳定性。


## 这是个什么玩意
这是个用于从壁纸网站上爬取图片的爬虫工具箱，本库采用F#语言编写，这个库可以帮助你从壁纸网站上爬取图片。
请注意，这个库遵守LGPL v3协议，以及不要用它做违法的事情，以此库造成的任何问题，均与库作者无关。

[在Nuget中安装这个库](https://www.nuget.org/packages/HaoKangFramework)

[![Build status](https://ci.appveyor.com/api/projects/status/i1kl78rex2ywocfn?svg=true)](https://ci.appveyor.com/project/SmallLuma/haokangframework)


## 如何使用它

### 搜索图像
1. 导入这个库到你的F#/C#项目中
2. open HaoKangFramework.Spider
3. Spiders是一个IDirectionary<string,ISpider>，这里面有这个库可以使用的所有爬虫。
4. 使用TestConnection测试爬虫是否连接良好，并且选出连接良好的爬虫。
5. 使用Search函数，输入要查询的关键字，即可返回搜索结果。

### 预览和下载
1. Search应当返回一个Result<Page,exn> seq，这是页面集合，每个页面都有一些图片或者其他的投稿，如果一个Page是空的，那么说明已经到底末页。
2. 遍历Page，Page本身是Post seq，每个Post都是一个投稿。
3. Post.PreviewImage是预览图像，Post.Content则是一组内容，你需要使用异步工作流来获取它，或者使用Post.ContentUrl获取URL，并且使用其他工具下载。

### 例子程序
这个例子程序演示了如何使用HaoKangFramework下载给定关键字的所有图片。    
[例子程序 DemoApp](DemoApp/Program.fs)

## 已经支持的爬虫
- Konachan
- Yandere
- Gelbooru
- Danbooru
- ATFBooru
- LoliBooru
- Rule34
- SafeBooru
- HypnoHub
- TheBigImageBoard

## 未来将要支持的爬虫
- iwara(Videos)
- ecchi.iwara(Videos)
- iwara(Images)
- ecchi.iwara(Images)
- Chan.SankakuComplex(参考https://github.com/CryShana/Sankaku-Channel-Downloader)
- Idol.SankakuComplex(参考https://github.com/CryShana/Sankaku-Channel-Downloader)
- Pixiv
- ExHentai
- Zerochan
- 3DBooru(Behoimi.org)
- AllGirl
- TheDoujin(http://thedoujin.com)


## 已经完成的工具包
- KonachanSpider（用于爬取Booru类网站，它对应的为Konachan这些版本较旧的网站）
- DanbooruSpider（用于爬取Booru类网站，它对应的为Danbooru这些版本较新的网站）

## 我希望加入开发

1. Fork这个项目
2. 解析你要爬取的网站
3. 在HaoKangFramework.Spiders下实现一个ISpider，然后使用这个ISpider在某个模块中创建属性为Spider的静态常量。
4. Pull Request，与世界分享你的成果

