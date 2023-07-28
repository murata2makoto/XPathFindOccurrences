module CreateCsv

open System.IO
open System.Collections.Generic
open System.Xml.Linq
open Toolkit.ReadWrite
open System.Xml.XPath
open XPathFindOccurrences.CreateSectionFinder

let createFigureCsv (doc: XDocument) man (sectionFinder: XElement -> string) sw =
    let drawings = doc.XPathSelectElements("//w:drawing", man)
    for drawing in drawings do
        let secnum = sectionFinder drawing
        fprintfn sw "%s" secnum

let createTableCsv (doc: XDocument) man (sectionFinder: XElement -> string) sw = 
    let atableXpath=  "//w:tbl[    w:tr[1]/w:tc[1]/w:p[1]/w:r[1]/w:t[1 and text()=\"Attributes\"] ]"
    let tableXpath=  "//w:tbl"
    let tables = doc.XPathSelectElements(tableXpath, man)
    let aTablesSet = doc.XPathSelectElements(atableXpath, man) |> (fun x -> new HashSet<XElement>(x))
    for table in tables do
        if aTablesSet.Contains(table) |> not then
            let secnum = sectionFinder table
            let strValue = table.Value
            let shortenedStrValue = strValue.Substring(0, min 100 (strValue.Length - 1) )
            fprintfn sw "%s\tTable\t%s" secnum shortenedStrValue


let createCsv doc tableOrFigure part1P (outputCsvFileName: string) = 

    let man = getManager doc

    let sectionFinder, _ = 
        createSectionFinder  doc man part1P

    use sw = new StreamWriter(outputCsvFileName)
    match tableOrFigure with
    | "figure" -> createFigureCsv doc man sectionFinder sw
    | "table"  -> createTableCsv  doc man sectionFinder sw
    | _ -> failwith "hen"
    ()