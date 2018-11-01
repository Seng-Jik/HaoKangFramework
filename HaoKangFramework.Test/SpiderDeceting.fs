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
        |> Array.iter (printfn "%A")

    [<TestMethod>]
    member x.TestConnection () =
        HaoKangFramework.Spider.Spiders
        |> Array.iter (fun (name,spider) ->
            match spider.TestConnection () with
            | Ok () -> printfn "%s OK!" name
            | Error e -> printfn "%s failed:%s" name e.Message)
        
