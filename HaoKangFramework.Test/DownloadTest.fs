namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders

[<TestClass>]
type DownloadTest () =
    [<TestMethod>]
    member x.DownloadTest () =
        Spider.Spiders
        |> Array.iter (fun (name,spider) ->
            Directory.CreateDirectory name |> ignore
            spider
            |> Spider.Search []
            |> Seq.head
            |> function
            | Ok a ->
                a
                |> Seq.take 3
                |> Seq.map (fun x -> 
                    async {
                        let content = x.Content |> List.head
                        let data = content.Data.Force ()
                        match data with
                        | Ok data ->
                            File.WriteAllBytes (name + "\\" + content.FileName,data)
                        | Error e -> printfn "%s download failed:%s" name e.Message })
                |> Async.Parallel
                |> Async.Ignore
                |> Async.RunSynchronously
            | Error e -> printfn "%s error:%s" name e.Message )
        ()
        
