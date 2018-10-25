namespace HaoKangFramework.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO
open HaoKangFramework.Spiders

[<TestClass>]
type FrameworkTest () =
    [<TestMethod>]
    member x.GetSpiders () =
        HaoKangFramework.Spider.Spiders
        |> Array.map (fun (name,factory) -> (name,factory()))
        |> Array.iter (printfn "%A")
