namespace HaoKangFramework.Test

open System
open HaoKangFramework.Spiders
open Microsoft.VisualStudio.TestTools.UnitTesting
open HaoKangFramework
open System.IO

[<TestClass>]
type TestSpiders () =

    let spiders : ISpider list = [
        new KonachanNetSpider () ]

    [<TestMethod>]
    member this.TestConnection () =
        ()
        
    [<TestMethod>]
    member this.DownloadImageByID () =
        ()