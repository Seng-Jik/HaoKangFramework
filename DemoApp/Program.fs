open System.IO    
open System
(*
let searchResult =
    let searchParam = 
        printfn "输入要查询的关键词，用空格分隔："
        Console.ReadLine().Split(' ')
        |> Array.toList

    let spiderSet =
        printfn ""
        Spider.Spiders
        |> Array.iteri (fun i (x,_) -> 
            printfn "%d. %s" i x)
        printfn "输入你要使用的爬虫的编号，用空格分隔："
        Console.ReadLine().Split(' ')
        |> Array.map int
        |> Array.map (fun i -> Spider.Spiders.[i])

    spiderSet
    |> Array.map (fun (name,factory) ->
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
*)