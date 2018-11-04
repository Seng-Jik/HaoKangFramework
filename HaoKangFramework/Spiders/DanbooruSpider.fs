module HaoKangFramework.Spiders.DanbooruSpider

open HaoKangFramework
open HaoKangFramework.Spiders.KonachanSpider
open KonachanSpiderUtils

module private DanbooruSpiderUtils =
    open HaoKangFramework.SpiderUtils
    open KonachanSpiderUtils

    module RequestFormats =
        let Danbooru : RequestFormat = "%s/posts.xml?limit=%d&page=%d&tags=%s"

    module PostUrlFormats = 
        let Danbooru : PostUrlFormat = "%s/posts/%u"

    let GetPage (url:string) (pageIndex:int) tags format urlFixer postUrlFormat spider=
        try
            let xmlDoc =
                ApplyUrl format url pageIndex tags
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
                    let id = i.SelectSingleNode "id" |> UnwrapXmlText |> uint64
                    {
                        ID = id
                        PreviewImage = i.SelectSingleNode "priview_file_url" |> UnwrapXmlText |> DownloadDataLazy
                        Content = [{
                            Data = contentUrl |> DownloadDataLazy
                            Url = contentUrl
                            FileName = contentUrl.[contentUrl.LastIndexOf '/' + 1 ..]
                            FileExtName = contentUrl.[contentUrl.LastIndexOf '.' + 1 ..] }]
                        PostUrl = sprintf postUrlFormat url id
                        Score = 
                            try
                                i.SelectSingleNode "score"
                                |> UnwrapXmlText
                                |> float
                                |> ValueSome
                            with _ -> ValueNone
                        AgeGrading = i.SelectSingleNode "rating" |> UnwrapXmlValue |> Rating
                        Author = i.SelectSingleNode "uploader-name" |> UnwrapXmlText
                        Tags = (i.SelectSingleNode "tag-string" |> UnwrapXmlText).Trim().Split ' '
                        FromSpider = spider }}
            |> Ok
        with e -> Error e

open DanbooruSpiderUtils

[<Spider>]
let Danbooru =
    new KonachanSpider ("Danbooru","https://danbooru.donmai.us",RequestFormats.Danbooru,PostUrlFormats.Danbooru,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let ATFBooru =
    new KonachanSpider ("ATFBooru","https://atfbooru.ninja",RequestFormats.Danbooru,PostUrlFormats.Danbooru,GetPage,NoFixer)
    :> ISpider