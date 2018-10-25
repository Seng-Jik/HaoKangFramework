open HaoKangFramework
open HaoKangFramework.Utils
open System.IO    

let searchParam = ["yuri";"uncensored"]

let searchResult =
    Spider.Spiders
    |> Array.filter (fun (x,_) -> x <> "Yandere" && x <> "Konachan")
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


let DownloadSite name (pageSeq : Page seq) = 
    let DownloadPage (page : Page) =
        page
        |> Seq.collect (fun post -> post.Content)
        |> Seq.map (fun content -> 
            async {
                printfn "Downloading %s" content.FileName
                let targetFile = name + "\\" + (NormalizeFileName content.FileName)

                match! content.Data with
                | Ok data ->
                    File.WriteAllBytes (targetFile,data)
                    printfn "Downloaded! %s" content.FileName
                | Error e -> printfn "%A %s" e content.FileName})
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously
    
    printfn "==== %s ====" name
    
    pageSeq
    |> Seq.takeWhile (fun x -> Seq.isEmpty x |> not)
    |> Seq.iteri (fun i page -> 
        printfn "-- Page %d" i
        DownloadPage page)

searchResult
|> Seq.iter (fun (name,x) -> DownloadSite name x)
