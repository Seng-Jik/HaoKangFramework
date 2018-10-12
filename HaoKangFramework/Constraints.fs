namespace HaoKangFramework

open System

type AgeGrading =
| Everyone
| R15
| R18

type PostType =
| Video
| Image

type SearchParam = {
    Tags : string list
    AgeGrading : AgeGrading
    Type : PostType list
    Uploader : string voption 
    Title : string voption
    UpdateTime : (DateTime * TimeSpan) voption }

type IPost =
    inherit IDisposable
    abstract Thumbnail : byte[] Async
    abstract Data : byte[] Async
    abstract PostType : PostType
    abstract AgeGrading : AgeGrading
    abstract Title : string
    abstract FileName : string
    abstract FileExtensionName : string
    abstract UpdateTime : DateTime
    abstract Uploader : string
    abstract Tags : string[]
    abstract FromSpider : ISpider

and ISpider =
    inherit IDisposable
    abstract TestConnection : unit Async
    abstract Search : SearchParam -> IPost seq
