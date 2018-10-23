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
type SearchParam = {
    Tags : string list
    AgeGrading : AgeGrading list
    PostType : PostType list
    Uploader : string voption 
    Title : string voption
    UpdateTime : (DateTime * TimeSpan) voption }

[<Struct>]
type Post = {
    ID : uint64
    Thumbnail : byte[] Async
    Content : byte[] Async list
    PostType : PostType
    AgeGrading : AgeGrading
    Title : string
    FileName : string
    FileExtensionName : string
    UpdateTime : DateTime
    Uploader : string
    Tags : string[]
    FromSpider : ISpider }

and Page = Post seq

and ISpider =
    inherit IDisposable
    abstract TestConnection : unit -> bool
    abstract Search : SearchParam -> Page seq



