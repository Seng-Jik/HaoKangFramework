namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders
open DanbooruSpider
open KonachanSpider
open HaoKangFramework

[<TestClass>]
type DownloadTest () =

    let Test name spider = 
        Directory.CreateDirectory name |> ignore
        spider
        |> Spider.Search []
        |> Seq.take 3
        |> Seq.iter (
            function
            | Ok a ->
                a
                |> Seq.take 3
                |> Seq.map (fun x -> 
                    async {
                        let content = x.Content |> List.head
                        let data = content.Data.Force ()
                        match data with
                        | Ok data ->
                            File.WriteAllBytes (name + "\\" + Utils.NormalizeFileName(content.FileName),data)
                        | Error e -> printfn "%s download failed:%s" name e.Message })
                |> Async.Parallel
                |> Async.Ignore
                |> Async.RunSynchronously
            | Error e -> printfn "%s error:%s" name e.Message)
        

    [<TestMethod>]
    member x.TestAll () =
        Spider.Spiders
        |> Seq.iter (fun x ->
             Test x.Key x.Value)

    [<TestMethod>]
    member x.Konachan () =
        Test "Konachan" Konachan

    [<TestMethod>]
    member x.Lolibooru () =
        Test "Lolibooru" Lolibooru

    [<TestMethod>]
    member x.HypnoHub () =
        Test "HypnoHub" HypnoHub

    [<TestMethod>]
    member x.Gelbooru () =
        Test "Gelbooru" Gelbooru

    [<TestMethod>]
    member x.Rule34 () =
        Test "Rule34" Rule34

    [<TestMethod>]
    member x.SafeBooru () =
        Test "SafeBooru" SafeBooru

    [<TestMethod>]
    member x.Yandere () =
        Test "Yandere" Yandere

    [<TestMethod>]
    member x.Danbooru () =
        Test "Danbooru" Danbooru

    [<TestMethod>]
    member x.ATFBooru () =
        Test "ATFBooru" ATFBooru
        
