module PrintAction

open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open SecTitle

let printAction sw getNumber (e:XElement) (stackOfStack: int list list) =
    let content = e.Value
    let topStackList = List.map (fun l -> List.head l) stackOfStack
    let printStr = getNumber topStackList
    fprintfn sw "%s %s" printStr content

