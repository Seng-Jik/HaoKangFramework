namespace HaoKangFramework.Spiders

open System.Net
open HaoKangFramework
open System

module private Konachan =

    let Website = "http://konachan.net/"

    let GetPost (client:WebClient) spider postid =
        async {
            let url = sprintf "%spost/show/%d/" Website postid

            let html = client.DownloadString url

            let beginPoint = 
                let mark1 = "original-file-unchanged highres-show"
                let p = html.IndexOf mark1
                if p > 0 then
                    html.IndexOf ('\"',p + mark1.Length + 1) + 1
                else
                    let mark2 = "original-file-unchanged"
                    html.IndexOf ('\"',p + mark2.Length + 1) + 1

            if beginPoint < 0 then 
                return None
            else
                let endPoint = html.IndexOf ('\"',beginPoint + 1)
                let imageUrl = html.[beginPoint..endPoint - 1]
                let extensionNamePoint = (imageUrl.LastIndexOf '.')

                return Some({
                    ID = postid
                    Thumbnail = async { return Array.empty }
                    Data = async { return client.DownloadData imageUrl }
                    PostType = Image
                    AgeGrading = Unknown
                    Title = null
                    FileName = imageUrl.[(imageUrl.LastIndexOf '/')+1..extensionNamePoint-1].Replace ("%20"," ")
                    FileExtensionName = imageUrl.[extensionNamePoint+1..]
                    UpdateTime = DateTime.Now
                    Uploader = null
                    Tags = null
                    FromSpider = spider }) }

open Konachan

type KonachanSpider() =
    
    let client = new WebClient ()

    interface ISpider with
        member this.Dispose() = 
            client.Dispose ()

        member this.FindPostByID id =
            GetPost client this id

        member this.TestConnection = 
            async {
                let uri = Uri Website
                let html = client.DownloadString uri
                assert (html.Length > 0) }

        member this.Search arg = raise (System.NotImplementedException())

        member x.WebsiteName = "Konachan"

