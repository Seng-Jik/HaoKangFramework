namespace HaoKangFramework.Test

open System
open HaoKangFramework.Spiders
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO

[<TestClass>]
type TestSpiders () =

    let spiders : ISpider list = [
        new KonachanSpider () ]

    [<TestMethod>]
    member this.TestConnection () =
        spiders
        |> List.map (fun spider ->
            spider.TestConnection)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
        
    [<TestMethod>]
    member this.DownloadImageByID () =
        let konachan = new KonachanSpider () :> ISpider
        
        async {
            let! post = konachan.FindPostByID 1UL
            let! data = 
                match post with
                | Some x ->
                    x.Data
                | None ->
                    failwith "No Data!" 
            File.WriteAllBytes ("OutDemo.png",data)
        }
        |> Async.RunSynchronously