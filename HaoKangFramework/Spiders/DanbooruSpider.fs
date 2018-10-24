namespace HaoKangFramework.Spiders

open HaoKangFramework

module private BooruUtils =
    open System.Net
    open System.Xml

    let TestConnection (postXml:string) =
        try
            use web = new WebClient ()
            web.DownloadString postXml |> ignore
            true
        with x -> false

    
    let GetPosts postXmlUrl pageID tags spider =
        let xml = XmlDocument ()

        using (new WebClient ()) (fun web ->
            sprintf "%s?limit=25&page=%d&tags=%s" postXmlUrl pageID tags
            |> web.DownloadString
            |> xml.LoadXml)

        let posts = xml.SelectSingleNode "posts"

        let count = posts.Attributes.["count"].Value |> int

        let pageCount = 
            match count % 100 with
            | 0 -> count / 100
            | _ -> count / 100 + 1
        
        ([ for i in posts.ChildNodes -> 
            let url = i.Attributes.["file_url"].Value
            {
                ID = i.Attributes.["id"].Value |> uint64
                Preview = async { 
                    use web = new WebClient ()
                    return i.Attributes.["preview_url"].Value |> web.DownloadData }

                Content = [ async { 
                    use web = new WebClient ()
                    return url |> web.DownloadData } ]

                PostType = Image
                AgeGrading = 
                    match i.Attributes.["rating"].Value with
                    | "s" -> Everyone
                    | "q" -> R15
                    | "e" -> R18
                    | _ -> Unknown

                FileName = url.[url.LastIndexOf '/' + 1 ..]
                FileExtensionName = url.[url.LastIndexOf '.' + 1 ..]
                Author = i.Attributes.["author"].Value
                Tags = i.Attributes.["tags"].Value.Split ' '
                FromSpider = spider }]
        ,pageCount)

open BooruUtils

type BooruSpider (postXml) =
    interface ISpider with
        member this.Dispose(): unit = ()
        member x.TestConnection () =
            TestConnection postXml
        member x.Search param =
            let tags =
                param
                |> List.reduce (fun x y -> x + " " + y)
            let (firstPage,pageCount) = GetPosts postXml 1 tags x
            let nextPages = 
                Seq.init pageCount (fun i ->
                    GetPosts postXml (i+1) tags x |> fst |> List.toSeq)
            Seq.append (seq { yield firstPage }) nextPages

type Konachan () =
   inherit BooruSpider ("http://konachan.net/post.xml")

type Yandere () =
   inherit BooruSpider ("http://yande.re/post.xml")
