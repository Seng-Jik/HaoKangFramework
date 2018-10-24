namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders

[<TestClass>]
type TestSpiders () =

    let knc = new Konachan()
    [<TestMethod>]
    member this.TestConnection () =
        assert (Spider.TestConnection knc)
        
    [<TestMethod>]
    member this.DownloadImageByID () =
        Spider.Search ["game_cg"] knc
        |> Seq.take 3
        |> Seq.iteri (fun i page ->
            let dir = Directory.CreateDirectory (sprintf "Page %d" i)
            
            page
            |> Seq.map (fun post ->
                async {
                    //let! prv_data = post.Preview
                    let! data = post.Content |> List.head
                    File.WriteAllBytes (dir.FullName + "\\" + post.FileName + "." + post.FileExtensionName,data)
                    //File.WriteAllBytes (dir.FullName + "\\" + post.FileName + "_pv." + post.FileExtensionName,prv_data)
                })
            |> Async.Parallel
            |> Async.RunSynchronously 
            |> ignore )
        