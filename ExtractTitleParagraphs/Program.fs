// For more information see https://aka.ms/fsharp-console-apps
open XPathFindOccurrences.Program
open XPathFindOccurrences.Toolkit
open System.Collections.Generic
open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open SecTitle
open PrintAction
let subQuery = "./w:pPr/w:pStyle/@w:val"

let paraStyleQuery = ".//w:p/w:pPr/w:pStyle"


let paraContainingDoubtfulParaStyleQuery = 
    ".//w:p[w:pPr/w:pStyle/@w:val=\"50\"]"




let createParaStyleList (doc: XDocument) mgr =
    let mutable titleElemList = []
    let paraStyleList = doc.XPathSelectElements(paraStyleQuery, mgr)
    let styleSeq = 
        seq {for secTitle in paraStyleList do 
                     yield secTitle.Attribute(wml + "val").Value}
    let hash = new Dictionary<string, int>()
    for style in styleSeq do
        if hash.ContainsKey(style) then
            hash.[style] <- hash.[style] + 1
        else
            hash.[style] <- 1
    for x in hash do
        let style = x.Key
        let count = x.Value
        printfn "%s: %d" style count


    
[<EntryPoint>]
let main argv =
    match argv with
    | [|  docXFileName; part1ps; outputFileName |] ->
//        try
            use sw = createTextWriteFromOutputFileName outputFileName
            let doc = createXDocumentFromDocxFileName docXFileName
            let mgr = getManager doc
            let part1P = part1ps = "1"
 //           createTitleElemList doc mgr
 //           createTitleElemList2 doc mgr sw
 //           createParaStyleList doc mgr
            scanSecTitles2 doc mgr (printAction sw (getNumber part1P))
            0
//        with 
//        | :? System.Exception as e -> printfn "%s" (e.Message); 1
    | _ -> 
        printfn "Illegal parameter %A" argv
        1

