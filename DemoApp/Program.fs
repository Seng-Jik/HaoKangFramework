open HaoKangFramework
open System.IO
open System
open HaoKangFramework.Spiders

let DownloadPage name (page:Page) =
    page
    |> Seq.iter (fun post -> 
        async {
            try
                let! data = post.Content.Head
                if data <> null then
                    File.WriteAllBytes (name + "\\" + (post.ID |> string) + "." + post.FileExtensionName,data)
                    printfn "Downloaded %s" post.FileName
            with 
            | _ -> () } |> Async.RunSynchronously )

let DownloadPages name (page:Page seq) =
    printfn "Indexing %s" name
    
    let downloadList = 
        try 
            page
            |> Seq.take 100
        with _ -> page

    try
        downloadList
        |> Seq.iter (DownloadPage name)
    with _ -> ()


let searchParam = ["footjob"]

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
|> Array.map (fun (name,result) ->
    async { DownloadPages name result })
|> Async.Parallel
|> Async.Ignore
|> Async.RunSynchronously

        