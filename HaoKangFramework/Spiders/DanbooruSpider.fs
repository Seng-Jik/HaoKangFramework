module HaoKangFramework.Spiders.DanbooruSpider

open HaoKangFramework
open HaoKangFramework.Spiders.KonachanSpider
open KonachanSpiderUtils

module private DanbooruSpiderUtils =
    open HaoKangFramework.SpiderUtils

    let internal GetPage (xmlUrl:string) (pageIndex:int) tags format urlFixer spider=
        try
            let xmlDoc =
                ApplyUrl format xmlUrl pageIndex tags
                |> DownloadXml
            let posts = xmlDoc.SelectSingleNode "posts"

            seq {
                for i in posts.ChildNodes ->
                    let contentUrl = 
                        match i.SelectSingleNode "large-file-url" with
                        | null -> 
                            match i.SelectSingleNode "file-url" with
                            | null -> i.SelectSingleNode "source" |> UnwrapXmlText
                            | x -> x.InnerText
                        | x -> x.InnerText
                        |> urlFixer
                    {
                        ID = i.SelectSingleNode "id" |> UnwrapXmlText |> uint64
                        PreviewImage = i.SelectSingleNode "priview_file_url" |> UnwrapXmlText |> DownloadDataLazy
                        Content = [{
                            Data = contentUrl |> DownloadDataLazy
                            Url = contentUrl
                            FileName = contentUrl.[contentUrl.LastIndexOf '/' + 1 ..]
                            FileExtName = contentUrl.[contentUrl.LastIndexOf '.' + 1 ..] }]
                        AgeGrading = i.SelectSingleNode "rating" |> UnwrapXmlValue |> Rating
                        Author = i.SelectSingleNode "uploader-name" |> UnwrapXmlText
                        Tags = (i.SelectSingleNode "tag-string" |> UnwrapXmlText).Trim().Split ' '
                        FromSpider = spider }}
            |> Ok
        with e -> Error e

open DanbooruSpiderUtils

[<Spider>]
let Danbooru =
    new KonachanSpider ("Danbooru","https://danbooru.donmai.us/posts.xml",KonachanFormat,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let ATFBooru =
    new KonachanSpider ("ATFBooru","https://atfbooru.ninja/posts.xml",KonachanFormat,GetPage,NoFixer)
    :> ISpider