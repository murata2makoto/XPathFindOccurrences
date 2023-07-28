module UnwrapSdt
open System.Xml
open System.Xml.Linq
open KeepUnwrapDelete

[<Literal>]
let WmlName = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

let private toBeUnwrapped (elem: XElement) =
    match elem.Name.NamespaceName with
    | WmlName ->
        match elem.Name.LocalName with
        | "sdt" | "sdtContent" -> true
        | _ -> false
    | _ -> false;;

let private toBeDeleted (elem: XElement) =
    match elem.Name.NamespaceName with
    | WmlName ->
        match elem.Name.LocalName with
      //  | "bookmarkStart" | "bookmarkEnd"
        | "sdtPr" | "sdtEndPr" -> true //sdtのしたにだけsdtPrとsdtEndPrはある。sdtはすべてunwrap
        | _ -> false
    | _ -> false;;

let unwrapSdt (elem: XElement)  : XElement  =
    copyKUD elem toBeUnwrapped toBeDeleted
