open System.Collections.Generic
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open Toolkit.ReadWrite
open XPathFindOccurrences.CreateSectionFinder

let paraQuery = "//w:p[not(.//w:p)
                    ]"
let drawingQuery = "//w:drawing"
 //                   and not (.//w:bookmarkStart) 
//                    and not (.//w:t[contains(text(), 
//                            \"Fundamentals and Markup Language Reference\")])]"

let scopeQuery = "//w:p//w:t[text()=\"Scope\"]"

let help2 (paraOrDrawings: XElement seq) finder scopePara sw  = 
    fprintfn sw "%s\t%s"
            "Subclause number"
            "Para Content" 
    for paraOrDrawing in paraOrDrawings do 
        if paraOrDrawing.IsAfter(scopePara) then
            let sectionNumber = finder paraOrDrawing
            if sectionNumber <> "" then
                match paraOrDrawing.Name.LocalName with
                | "p" ->
                    let paraContent = paraOrDrawing.Value.Trim()
                    if paraContent.Length > 0 then
                        fprintfn sw "%s\t%s" sectionNumber paraContent
                | "drawing" -> 
                    fprintfn sw "%s\t##Figure##" sectionNumber
                | _ -> failwith "hen"


[<EntryPoint>]
let main argv =
    match argv with
    | [| docXFileName; outputFileName |] ->
            let doc = createXDocumentFromDocxFileName docXFileName
                        "\\word\\document.xml"
            let part1P = docXFileName.Contains("29500-1")
            let mgr = getManager doc
            let sectionFinder, resetSectionFinder = 
                createSectionFinder  doc mgr part1P
            let parasOrFigures = doc.XPathSelectElements(paraQuery+"|"+drawingQuery, mgr)
            let scopePara = doc.XPathSelectElement(scopeQuery, mgr)
            use sw = createTextWriteFromOutputFileName outputFileName
            resetSectionFinder()
            help2 parasOrFigures sectionFinder scopePara sw 
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1