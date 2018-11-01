module internal HaoKangFramework.SpiderUtils

open System.Net
open System.Xml

let DownloadString (url:string) =
    use web = new WebClient ()
    web.DownloadString url

let ReduceTags = function
| [] -> ""
| x -> List.reduce (fun x y -> x + " " + y) x

let DownloadDataLazy (url:string) =
    lazy (
        try
            match url.Trim () with
            | "" -> raise NoData
            | x ->
                use web = new WebClient ()
                web.DownloadData x
                |> Ok
        with e -> Error e)

let UnwrapXmlValue (x:XmlNode) = 
    match x with
    | null -> ""
    | x -> x.Value

let UnwrapXmlText (x:XmlNode) = 
    match x with
    | null -> ""
    | x -> x.InnerText

let DownloadXml (url:string) =
    let xmlDoc = XmlDocument ()
    DownloadString url
    |> xmlDoc.LoadXml
    xmlDoc

