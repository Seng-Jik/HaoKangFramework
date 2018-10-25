namespace HaoKangFramework

open System

exception DownloadFailed
exception NoData

type AgeGrading =
| Everyone
| R15
| R18
| Unknown

[<Struct>]
type Content = {
    Data : Result<byte[],exn> Async
    FileName : string
    FileExtName : string
    Url : string }


[<Struct>]
type Post = {
    ID : uint64
    Preview : Result<byte[],exn> Async
    Content : Content list
    AgeGrading : AgeGrading
    Author : string
    Tags : string[]
    FromSpider : ISpider }

and Page = Post seq

and ISpider =
    inherit IDisposable
    abstract TestConnection : unit -> bool
    abstract Search : tags : string list -> Page seq

module public Utils =
    let inline NormalizeFileName (x : string) = 
        x.Trim(':','*','!','#','?','%','<','>','|','\"','\\','/').Trim()

module public Spider =
    let inline TestConnection spider =
        (spider :> ISpider).TestConnection ()

    let inline Search param spider =
        (spider :> ISpider).Search param

    let Spiders = 
        System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
        |> Array.filter (fun x ->
            (x |> string).StartsWith "HaoKangFramework.Spiders.")
        |> Array.filter (fun x ->
            x.IsClass && 
            x.IsPublic &&
            x.GetInterfaces() |> Array.contains typeof<ISpider> &&
            x.GetConstructors() |> Array.exists (fun x -> x.GetParameters() |> Array.isEmpty))
        |> Array.map (fun x ->
            (x.Name,(fun () -> x.Assembly.CreateInstance(x.FullName) :?> ISpider)))

