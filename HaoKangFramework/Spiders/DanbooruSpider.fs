namespace HaoKangFramework.Spiders

open HaoKangFramework

module private BooruUtils =
    open System.Net
    open System.Xml
    open System.Net.Http
    open System

    let TestConnection (postXml:string) =
        try
            use web = new HttpClient ()
            web.Timeout <- TimeSpan(0,0,10)
            (web.GetAsync postXml).Result |> ignore
            true
        with x -> false

    let private Rating = function
        | "s" -> Everyone
        | "q" -> R15
        | "e" -> R18
        | _ -> Unknown

    type PageFormat = Printf.StringFormat<(string -> int -> string -> string)>

    let private GetPostXml (pageFormat:PageFormat) postXmlUrl pageID tags =
        let xml = XmlDocument ()

        using (new WebClient ()) (fun web ->
            sprintf pageFormat postXmlUrl pageID tags
            |> web.DownloadString
            |> xml.LoadXml )
        xml

    let private DownloadAsync (url : string) = async { 
        use web = new WebClient ()
        let realUrl =
            if not (url.StartsWith "http") then
                "https:" + url
            else
                url
        try
            return realUrl |> web.DownloadData
        with _ -> return null }

    let GetPostsDanbooru (pageFormat : PageFormat) postXmlUrl pageID tags spider =
        let xml = GetPostXml pageFormat postXmlUrl pageID tags
        let posts = xml.SelectSingleNode "posts"

        [ for i in posts.ChildNodes ->
            let priviewUrl = 
                match i.SelectSingleNode("priview_file_url") with
                | null -> null
                | x -> x.InnerText

            let contentUrl = 
                match i.SelectSingleNode("large-file-url") with
                | null -> 
                    match i.SelectSingleNode("file-url") with
                    | null -> i.SelectSingleNode("source").InnerText
                    | x -> x.InnerText
                | x -> x.InnerText

            {
                ID = i.SelectSingleNode("id").InnerText |> uint64
                Preview = 
                    if System.String.IsNullOrWhiteSpace priviewUrl then
                        ValueNone
                    else 
                        DownloadAsync priviewUrl |> ValueSome

                Content = [ DownloadAsync contentUrl ]
                ContentUrl = [ contentUrl ]
                AgeGrading = Rating (i.SelectSingleNode("rating").InnerText)
                FileName = contentUrl.[contentUrl.LastIndexOf '/' + 1 ..]
                FileExtensionName = contentUrl.[contentUrl.LastIndexOf '.' + 1 ..]
                Author = i.SelectSingleNode("uploader-name").InnerText
                Tags = i.SelectSingleNode("tag-string").InnerText.Split(' ')
                FromSpider = spider }]

    let GetPostsKonachan pageFormat postXmlUrl pageID tags spider =
        let xml = GetPostXml pageFormat postXmlUrl pageID tags
        let posts = xml.SelectSingleNode "posts"

        let count = posts.Attributes.["count"].Value |> int
        let pageCount = 
            match count % 5 with
            | 0 -> count / 5
            | _ -> count / 5 + 1
        
        ([ for i in posts.ChildNodes -> 
            let url = i.Attributes.["file_url"].Value
            {
                ID = 
                    match i.Attributes.["id"] with
                    | null -> 0UL
                    | x -> x.Value |> uint64

                Preview = 
                    match i.Attributes.["preview_url"] with
                    | null -> ValueNone
                    | x -> DownloadAsync x.Value |> ValueSome

                Content = [ DownloadAsync url ]
                ContentUrl = [ url ]
                AgeGrading = Rating i.Attributes.["rating"].Value
                FileName = url.[url.LastIndexOf '/' + 1 ..]
                FileExtensionName = url.[url.LastIndexOf '.' + 1 ..]
                Author = 
                    match i.Attributes.["author"] with
                    | null -> ""
                    | x -> x.Value

                Tags = i.Attributes.["tags"].Value.Split ' '
                FromSpider = spider }]
        ,pageCount)

    let ReduceTags = function 
    | [] -> ""
    | x -> List.reduce (fun x y -> x + " " + y) x
    
    let KonachanFormat : PageFormat = "%s?limit=5&page=%d&tags=%s"
    let GelbooruFormat : PageFormat = "%s?page=dapi&s=post&q=index&&limit=5&pid=%d&tags=%s"

open BooruUtils

type KonachanLikeBooruSpider (postXml,pageFormat : PageFormat) =
    interface ISpider with
        member this.Dispose() = ()
        member x.TestConnection () =
            TestConnection postXml
        member x.Search param =
            let tags = ReduceTags param
            let (firstPage,pageCount) = GetPostsKonachan pageFormat postXml 1 tags x
            let nextPages = 
                Seq.init pageCount (fun i ->
                    GetPostsKonachan pageFormat postXml (i+2) tags x |> fst |> List.toSeq)
            Seq.append (seq { yield firstPage }) nextPages

type DanbooruLikeBooruSpider (postXml,pageFormat) =
    interface ISpider with
        member this.Dispose () = ()
        member x.TestConnection () = 
            TestConnection postXml
        member x.Search param =
            let tags = ReduceTags param
            Seq.initInfinite (fun i ->
                GetPostsDanbooru pageFormat postXml i tags x |> List.toSeq)

type Konachan () =
   inherit KonachanLikeBooruSpider ("http://konachan.net/post.xml",KonachanFormat)

type Yandere () =
   inherit KonachanLikeBooruSpider ("https://yande.re/post.xml",KonachanFormat)

type Gelbooru () =
    inherit KonachanLikeBooruSpider ("https://www.youhate.us/index.php",GelbooruFormat)

type Danbooru () =
    inherit DanbooruLikeBooruSpider ("https://danbooru.donmai.us/posts.xml",KonachanFormat)

type ATFBooru () =
    inherit DanbooruLikeBooruSpider ("https://atfbooru.ninja/posts.xml",KonachanFormat)

type LoliBooru () =
    inherit KonachanLikeBooruSpider ("https://lolibooru.moe/post/index.xml",KonachanFormat)

type Rule34 () =
    inherit KonachanLikeBooruSpider ("https://rule34.xxx/index.php",GelbooruFormat)

type SafeBooru () =
    inherit KonachanLikeBooruSpider ("https://safebooru.org/index.php",GelbooruFormat)

type HypnoHub () =
    inherit KonachanLikeBooruSpider ("https://hypnohub.net/post/index.xml",KonachanFormat)


(* Next Works : https://booru.org/top *)
