namespace HaoKangFramework

open System

type AgeGrading =
| Everyone
| R15
| R18
| Unknown

exception NoData

[<Struct>]
type Content = {
    Data : Result<byte[],exn> Lazy
    FileName : string
    FileExtName : string
    Url : string }


[<Struct>]
type Post = {
    ID : uint64
    PreviewImage : Result<byte[],exn> Lazy
    Content : Content list
    AgeGrading : AgeGrading
    Author : string
    Tags : string[]
    FromSpider : ISpider }

and Page = Post seq

and ISpider =
    inherit IDisposable
    abstract TestConnection : unit -> Result<unit,exn>
    abstract Search : tags : string list -> Result<Page,exn> seq

type Spider () =
    inherit System.Attribute ()

module public Utils =
    let inline NormalizeFileName (x : string) = 
        let mutable ret = x
        [":";"*";"!";"#";"?";"%";"<";">";"|";"\"";"\\";"/"]
        |> List.iter (fun c -> ret <- ret.Replace (c,""))
        ret.Trim()

module public Spider =
    open System.Reflection

    let inline TestConnection spider =
        (spider :> ISpider).TestConnection ()

    let inline Search param spider =
        (spider :> ISpider).Search param


    let Spiders =
        System
            .Reflection
            .Assembly
            .GetExecutingAssembly()
            .GetTypes()
        |> Array.filter (fun x ->
            FSharp.Reflection.FSharpType.IsModule x)
        |> Array.collect (fun x ->
            x.GetMembers ())
        |> Array.filter (fun x ->
            x.GetCustomAttributes()
            |> Seq.exists (fun x -> x :? Spider))
        |> Array.map (fun x ->
            x.Name,
            (x :?> PropertyInfo)
                .GetValue(null) 
                :?> ISpider)
        |> dict

