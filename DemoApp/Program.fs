open HaoKangFramework
open System
open System.Net
open System.IO
open HaoKangFramework
open Utils

printfn "Supported:"
Spider.Spiders
|> Seq.iter (fun x -> printfn "%s" x.Key)
printfn ""

printf "Connecting..."
let usableSpiders =
    Spider.Spiders
    |> Seq.map (fun kv ->
        async {
            return (Spider.TestConnection kv.Value,kv.Key,kv.Value) })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.filter (fun (a,_,_) -> a = Ok ())
    |> Array.map (fun (_,b,c) -> b,c)
printfn "OK!"
printfn ""

printfn "Usable spiders:"
usableSpiders
|> Array.iteri (fun i (name,_) ->
    printfn "%d.%s" (i+1) name)
printfn "Select your spiders(split by space):"

let spiders =
    Console.ReadLine().Trim().Split ' '
    |> Array.map (fun x -> 
        usableSpiders.[x |> int |> (+) -1] |> snd)
printfn ""

printfn "Selected spiders:"
spiders
|> Array.iter (printfn "%A")
printfn ""

printfn "Input your search tags(split by space):"
let tags =
    Console.ReadLine().Trim()
let splitedTags =
    tags.Split ' '
    |> Array.toList

let logFile =
    match tags with
    | "" -> "Download/no_tags.log"
    | x -> "Download/"+x+".log"

File.Delete logFile
let Log (x:string) =
    use logFile = File.Open (logFile,FileMode.Append)
    use stream = new StreamWriter (logFile)
    stream.WriteLine x

try
    printfn "======================================="

    let dir =
        Directory.CreateDirectory "Download" |> ignore
        match tags with
        | "" ->
            Directory.CreateDirectory("Download/no_tags").FullName + "\\"
        | tags ->
            Directory.CreateDirectory("Download/" + tags).FullName + "\\"

    let DownloadPage (page:Result<Page,exn>) = 
        let DownloadPost post = 
            let DownloadContent content = async {
                try
                    printfn "Downloading %s" content.FileName

                    do! Async.SwitchToThreadPool()
                    match content.Data.Force() with
                    | Ok data ->
                        let fileName = 
                            let org = dir + (NormalizeFileName content.FileName)
                            if org.Length > 247 then
                                let newPath = 
                                    org.[0..100] + "." + content.FileExtName
                                if File.Exists newPath then
                                    (string (org.GetHashCode())) + "." + content.FileExtName
                                else
                                    newPath
                            else
                                org
                        File.WriteAllBytes (fileName,data)
                        printfn "Downloaded! %s" content.FileName
                    | Error e -> raise e
                with e ->
                    sprintf @"Error:
                    Page:%A
                    Post:%A
                    Content:%A
                    Exception:%A
                    "
                        page
                        post
                        content
                        e
                    |> Log }
        

            post.Content
            |> List.map DownloadContent
            |> Async.Parallel
            |> Async.Ignore
        


        match page with
        | Ok page ->
            page
            |> Seq.map DownloadPost
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore
        | Error e -> Log e.Message


    spiders
    |> Array.map (Spider.Search splitedTags)
    |> Array.map (fun spiderResult ->
        spiderResult
        |> Seq.takeWhile (fun pageResult ->
            match pageResult with
            | Ok x -> Seq.isEmpty x |> not
            | Error e -> 
                sprintf "Pages error:%A" e
                |> Log
                true))
    |> Seq.collect (fun x -> x)
    |> Seq.iter DownloadPage

with ex ->
    sprintf @"
    致命错误：
    %A"
        ex
    |> Log
