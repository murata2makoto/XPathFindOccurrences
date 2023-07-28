open Toolkit.ReadWrite
open Toolkit.OOXMLNamespaces
open Toolkit.SecTitle
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open System.IO
open System.Collections.Generic
open CreateCsv
open UnwrapSdt
open 消したいものを消す


[<EntryPoint>]
let main argv =
    let inputDocxFileName = argv.[0]
    let tableOrFigure = argv.[1]
    let outputMinimialDocxFileName = argv.[2]
    let outputCsvFileName = argv.[3]

    let part1P = inputDocxFileName.Contains("29500-1")

    extractDocx inputDocxFileName
    let doc = createXDocumentFromExtractedDocx "\\word\\document.xml"
    let man = getManager doc

    let unwrappedRoot = 
        doc.Root
        |> unwrapSdt //ここでsdtとsdtContentをunwrapしている
    let result = 
        match tableOrFigure with 
            |"table" -> 消す_tbl unwrappedRoot man
            |"figure" -> 消す_drawing unwrappedRoot man
            | _ -> failwith "hen" 
    //printfn "%A" result
    let minimalDocumentXml = new XDocument(result)
    replaceXDocumentInExtractedDocx "\\word\\document.xml" minimalDocumentXml 
    createDocxFromExtractedDocx outputMinimialDocxFileName 

    createCsv minimalDocumentXml tableOrFigure part1P outputCsvFileName
    0 // return an integer exit code
