// For more information see https://aka.ms/fsharp-console-apps

module XPathFindOccurrences.Program

open NERGList
open Toolkit
open CreateSectionFinder
open GetStartEndMarkerPairs
open ForbiddenWords
open System.IO
open System.Xml.Linq


let readXPaths inputFileName = 
    let enc = new System.Text.UTF8Encoding(true)
    let ifs = new FileStream(inputFileName, FileMode.Open)
    let sr = new StreamReader(ifs, enc)
    let xpaths =
        [while not(sr.EndOfStream) do 
            yield sr.ReadLine()]
    if xpaths.Length = 2 then xpaths
    else failwith "Specify two XPath expressions"


let help2 (pairs: (string * 
                    XElement * 
                    XElement * seq<string>) list)
          finder  sw  = 
    fprintfn sw "%s\t%s\t%s\t%s\t%s"
        "Subclasue number"
        "Presense of normative modal verbs" 
        "Start Marker Position" 
        "End Marker Position"  
        "Content"    
    for (xpath, startElem, _ , contents) in pairs do 
        let sectionNumber = finder startElem
        let msp_mep_pairs = 
            getStartEndMarkerPairs xpath contents
        for (msp, mep) in msp_mep_pairs do
            let flag = hasForbiddenWords contents msp mep 
            fprintf sw "%s\t%b\t%d\t%d\t" sectionNumber flag msp mep 
            contents |> Seq.iter (fun cont -> fprintf sw "%s " cont) 
            sw.WriteLine()

[<EntryPoint>]
let main argv =
    match argv with
    | [| xPathsFileName; docXFileName; outputFileName |] ->
            let xPaths = readXPaths xPathsFileName
            let doc = createXDocumentFromDocxFileName docXFileName
            let mgr = getManager doc
            let part1P = docXFileName.Contains("29500-1")
            //let hash = createHashContainingParagarphsAndTableRows doc mgr
            let sectionFinder = 
                createSectionFinder 
                    (createTitleElemList doc mgr part1P)
            let nergList = 
                extractNERGList xPaths doc mgr 
            use sw = createTextWriteFromOutputFileName outputFileName
            help2 nergList sectionFinder sw 
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1


