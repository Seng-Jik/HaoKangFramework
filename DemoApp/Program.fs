open HaoKangFramework
open System.IO
open System
open HaoKangFramework.Spiders

let DownloadPage name (page:Page) =
    page
    |> Seq.map (fun post -> 
        async {
            try
                printfn "Downloading %s %s" name post.FileName
                let! data = post.Content.Head
                if data <> null then
                    File.WriteAllBytes (name + "\\" + post.FileName,data)
            with 
            | _ -> () })
    |> Async.Parallel
    |> Async.Ignore

let DownloadPages name (page:Page seq) =
    page
    |> Seq.head |> DownloadPage name
    |> Async.Ignore

let searchParam = ["masturbation";"uncensored"]

let searchResult =
    Spider.Spiders
    |> Array.filter (fun (x,_) -> x <> "Yandere")
    |> Array.map (fun (name,factory) ->
        printfn "Spider Deceted:%s" name
        (name,factory ()))
    |> Array.filter (fun (name,x) ->
        let test = Spider.TestConnection x
        if test then
            printfn "Spider Connected:%s" name
        test)
    |> Array.map (fun (name,spider) ->
        Directory.CreateDirectory name |> ignore
        (name,spider))
    |> Array.map (fun (name,x) -> async { return name,(x |> Spider.Search searchParam) })
    |> Async.Parallel
    |> Async.RunSynchronously

searchResult
|> Array.map (fun (name,x) -> 
    DownloadPages name x)
|> Async.Parallel
|> Async.Ignore
|> Async.RunSynchronously

        