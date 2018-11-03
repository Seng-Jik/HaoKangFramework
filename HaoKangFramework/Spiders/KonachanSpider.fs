module HaoKangFramework.Spiders.KonachanSpider

open HaoKangFramework

module internal KonachanSpiderUtils =
    open SpiderUtils

    let PageLimit = 100

    let Rating = function
    | "s" -> Everyone
    | "q" -> R15
    | "e" -> R18
    | _ -> Unknown

    type RequestFormat =
        Printf.StringFormat<(string -> int -> int -> string -> string)>

    type PostUrlFormat = Printf.StringFormat<(string->uint64->string)>

    type UrlFixer = string -> string

    let ApplyUrl format xmlUrl pageIndex tags =
        sprintf format xmlUrl PageLimit pageIndex (ReduceTags tags)

    let GetPage (url:string) (pageIndex:int) tags format urlFixer postUrlFormat spider =
        try
            let xmlDoc =
                ApplyUrl format url pageIndex tags
                |> DownloadXml

            let posts = xmlDoc.SelectSingleNode "posts"

            seq { 
                for i in posts.ChildNodes ->
                    let url = i.Attributes.["file_url"] |> UnwrapXmlValue |> urlFixer
                    let id = i.Attributes.["id"] |> UnwrapXmlValue |> uint64
                    {
                        ID = id
                        PreviewImage = i.Attributes.["preview_url"] 
                                        |> UnwrapXmlValue 
                                        |> urlFixer 
                                        |> DownloadDataLazy
                        Content = [{
                            Data = url |> DownloadDataLazy
                            FileName =  url.[url.LastIndexOf '/' + 1 ..]
                            FileExtName = url.[url.LastIndexOf '.' + 1 ..]
                            Url = url }]
                        Score = 
                            try
                                i.Attributes.["score"]
                                |> UnwrapXmlValue
                                |> float
                                |> ValueSome
                            with _ -> ValueNone
                        PostUrl = sprintf postUrlFormat url id
                        AgeGrading = i.Attributes.["rating"] |> UnwrapXmlValue |> Rating
                        Author = i.Attributes.["author"] |> UnwrapXmlValue
                        Tags = (i.Attributes.["tags"] |> UnwrapXmlValue).Trim().Split ' '
                        FromSpider = spider }}
            |> Ok
        with e -> Error e

    module RequestFormats =
        let Konachan : RequestFormat = "%s/post.xml?limit=%d&page=%d&tags=%s"
        let HypnoHub : RequestFormat = "%s/post/index.xml?limit=%d&page=%d&tags=%s"
        let Gelbooru : RequestFormat = "%s/index.php?page=dapi&s=post&q=index&&limit=%d&pid=%d&tags=%s"
    
    module PostUrlFormats =
        let Konachan : PostUrlFormat = "%s/post/show/%u"
        let Gelbooru : PostUrlFormat = "%s/index.php?page=post&s=view&id=%u"


    let NoFixer x = x
    let HttpsFixer x = "https:" + x
    
    type KonachanSpider (spiderName,xmlUrl,requestFormat,postUrlFormat,pageGetter,urlFixer) =
        inherit obj ()
        override x.ToString () =
            spiderName + " Spider"

        interface ISpider with
            member x.Dispose () = ()
            member x.TestConnection () =
                try
                    ApplyUrl requestFormat xmlUrl 1 []
                    |> DownloadString
                    |> ignore
                    Ok ()
                with e -> Error e
            
            member x.Search(tags:string list): Result<Page,exn> seq = 
                Seq.initInfinite (fun i ->
                    pageGetter xmlUrl i tags requestFormat urlFixer postUrlFormat x)


open KonachanSpiderUtils

[<Spider>]
let Konachan =
    new KonachanSpider ("Konachan","http://konachan.net",RequestFormats.Konachan,PostUrlFormats.Konachan,GetPage,NoFixer) 
    :> ISpider

[<Spider>]
let Lolibooru =
    new KonachanSpider ("Lolibooru","https://lolibooru.moe",RequestFormats.Konachan,PostUrlFormats.Konachan,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let HypnoHub =
    new KonachanSpider ("HypnoHub","https://hypnohub.net",RequestFormats.HypnoHub,PostUrlFormats.Konachan,GetPage,HttpsFixer)
    :> ISpider

[<Spider>]
let Gelbooru =
    new KonachanSpider ("Gelbooru","https://www.youhate.us",RequestFormats.Gelbooru,PostUrlFormats.Gelbooru,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let Rule34 =
    new KonachanSpider ("Rule34","https://rule34.xxx",RequestFormats.Gelbooru,PostUrlFormats.Gelbooru,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let SafeBooru =
    new KonachanSpider ("SafeBooru","https://safebooru.org",RequestFormats.Gelbooru,PostUrlFormats.Gelbooru,GetPage,HttpsFixer)
    :> ISpider

[<Spider>]
let Yandere =
    new KonachanSpider ("Yandere","https://yande.re",RequestFormats.Konachan,PostUrlFormats.Konachan,GetPage,NoFixer)
    :> ISpider

[<Spider>]
let Behoimi =
    new KonachanSpider ("Behoimi","http://behoimi.org",RequestFormats.HypnoHub,PostUrlFormats.Konachan,GetPage,NoFixer)
