namespace HaoKangFramework

open System

type AgeGrading =
| Everyone
| R15
| R18
| Unknown

type PostType =
| Video
| Image

[<Struct>]
type Post = {
    ID : uint64
    Preview : byte[] Async
    Content : byte[] Async list
    PostType : PostType
    AgeGrading : AgeGrading
    FileName : string
    FileExtensionName : string
    Author : string
    Tags : string[]
    FromSpider : ISpider }

and Page = Post seq

and ISpider =
    inherit IDisposable
    abstract TestConnection : unit -> bool
    abstract Search : tags : string list -> Page seq

module Spider =
    let inline TestConnection spider =
        (spider :> ISpider).TestConnection ()
    let inline Search param spider =
        (spider :> ISpider).Search param


