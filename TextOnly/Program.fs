open System.Collections.Generic
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open OOXML.Toolkit
open XPathFindOccurrences.CreateSectionFinder

let paraQuery = "//w:p[not(.//w:p)
                    ]"
 //                   and not (.//w:bookmarkStart) 
//                    and not (.//w:t[contains(text(), 
//                            \"Fundamentals and Markup Language Reference\")])]"

let scopeQuery = "//w:p//w:t[text()=\"Scope\"]"

let help2 (paras: XElement seq) finder scopePara sw  = 
    fprintfn sw "%s\t%s"
            "Subclause number"
            "Para Content" 
    for para in paras do 
        if para.IsAfter(scopePara) then
            let sectionNumber = finder para
            if sectionNumber <> "" then
                let paraContent = para.Value
                fprintf sw "%s\t%s" sectionNumber paraContent
                sw.WriteLine()


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
            let paras = doc.XPathSelectElements(paraQuery, mgr)
            let scopePara = doc.XPathSelectElement(scopeQuery, mgr)
            use sw = createTextWriteFromOutputFileName outputFileName
            resetSectionFinder()
            help2 paras sectionFinder scopePara sw 
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1