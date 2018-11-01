open HaoKangFramework
open System
open System.Net
open System.IO

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

printfn "======================================="


let dir =
    Directory.CreateDirectory "Download" |> ignore
    Directory.CreateDirectory("Download/" + tags).FullName + "\\"

