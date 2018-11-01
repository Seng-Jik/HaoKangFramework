namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders

[<TestClass>]
type SpiderDeceting () =
    [<TestMethod>]
    member x.GetSpiders () =
        HaoKangFramework.Spider.Spiders
        |> Seq.iter (printfn "%A")

    [<TestMethod>]
    member x.TestConnection () =
        HaoKangFramework.Spider.Spiders
        |> Seq.iter (fun x ->
            match x.Value.TestConnection () with
            | Ok () -> printfn "%s OK!" x.Key
            | Error e -> printfn "%s failed:%A" x.Key x.Value)
        
