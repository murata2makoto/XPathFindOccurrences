// For more information see https://aka.ms/fsharp-console-apps
open Toolkit.ReadWrite
open Toolkit.SecTitle

open System.Xml.XPath
open System.Xml.Linq
open System.IO
open System.IO.Compression

let tocStylesXPath = "//w:style[.//w:outlineLvl]/w:name"


[<EntryPoint>]
let main argv =
    match argv with
    | [| docXFileName |] ->
            let doc = createXDocumentFromDocxFileName docXFileName
                        "\\word\\styles.xml"
            let mgr = getManager doc
            for tocStyle in doc.XPathSelectElements(tocStylesXPath, mgr) do
                let valAtt = tocStyle.Attribute(wml + "val")
                printfn "%s" valAtt.Value
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1