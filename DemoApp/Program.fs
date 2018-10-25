open HaoKangFramework
open System.IO

let searchParam = ["masturbation";"uncensored"]

let searchResults = 
    Spider.Spiders
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
    |> Seq.map (fun (name,pages) ->
        try 
            printfn "Indexing:%s" name
            let take100 =
                pages
                |> Seq.take 100
            name,take100
        with x -> name,pages)
    |> Seq.fold (fun s t -> t::s) []

        