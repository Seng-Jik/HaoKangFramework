namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders

module private Downloader =
    let inline Download3Pages spider =
        assert (Spider.TestConnection spider)
        let result = 
            let pages = Spider.Search [] spider
            try
                pages
                |> Seq.take 3
            with _ -> pages
        
        result
        |> Seq.iteri (fun i page ->
            let spiderName = spider |> string
            let dir = Directory.CreateDirectory (sprintf "%s Page %d" spiderName.[spiderName.LastIndexOf '.' + 1 ..] i)
            
            page
            |> Seq.map (fun post ->
                async {
                    post.Content
                    |> List.map (fun x -> async {
                        match! x.Data with
                        | Ok data -> File.WriteAllBytes (dir.FullName + "\\" + (Utils.NormalizeFileName x.FileName),data)
                        | Error _ -> ()})
                    |> Async.Parallel
                    |> Async.Ignore
                    |> Async.RunSynchronously })
            |> Async.Parallel
            |> Async.RunSynchronously 
            |> ignore )

open Downloader

[<TestClass>]
type TestSpiders () =
        
    [<TestMethod>]
    member this.Konachan () =
        Download3Pages (new Konachan ())

    [<TestMethod>]
    member this.Yandere () =
        Download3Pages (new Yandere ())

    [<TestMethod>]
    member this.Danbooru () =
        Download3Pages (new Danbooru ())

    [<TestMethod>]
    member this.Gelbooru () =
        Download3Pages (new Gelbooru ())

    [<TestMethod>]
    member this.ATFBooru () =
        Download3Pages (new ATFBooru ())

    [<TestMethod>]
    member this.LoliBooru () =
        Download3Pages (new LoliBooru ())

    [<TestMethod>]
    member this.Rule34 () =
        Download3Pages (new Rule34 ())

    [<TestMethod>]
    member this.SafeBooru () =
        Download3Pages (new SafeBooru ())

    [<TestMethod>]
    member this.HypnoHub () =
        Download3Pages (new HypnoHub ())

        