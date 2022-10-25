// For more information see https://aka.ms/fsharp-console-apps

module XPathFindOccurrences.Program

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

open NERG
open NERGList
open Toolkit
open CreateSectionFinder
open GetStartEndMarkerPairs
open ForbiddenWords
open DetectXsd
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


let help2 nerg (pairs: (string * 
                    XElement * 
                    XElement * seq<string>) list)
          finder  sw  = 
    if nerg = Note then
        fprintfn sw "%s\t%s\t%s\t%s\t%s\t%s\t%s"
            "Subclause number"
            "Presense of normative modal verbs" 
            "Reference to schemas" 
            "Start Marker Position" 
            "End Marker Position"  
            "Content" 
            "Para Content" 
    else
        fprintfn sw "%s\t%s\t%s\t%s\t%s\t%s"
            "Subclause number"
            "Presense of normative modal verbs" 
            "Start Marker Position" 
            "End Marker Position"  
            "Content" 
            "Para Content" 
    for (xpath, startElem, _ , contents) in pairs do 
        let sectionNumber = finder startElem
        let msp_mep_content_triplets = 
            getStartEndMarkerPairs nerg xpath contents
        for (msp, mep, nergContents) in msp_mep_content_triplets do
            let forbiddenWordsFlag = hasForbiddenWords nergContents
            let xsdFlag = detectXsd nergContents
            if nerg = Note then
                fprintf sw "%s\t%b\t%b\t%d\t%d\t" 
                    sectionNumber forbiddenWordsFlag xsdFlag msp mep
            else
                fprintf sw "%s\t%b\t%d\t%d\t" sectionNumber forbiddenWordsFlag msp mep 
            Seq.head nergContents |> fprintf sw "%s" 
            for nergContent in Seq.tail nergContents do
                fprintf sw " %s" nergContent
            fprintf sw "\t"
            Seq.head contents |> fprintf sw "%s" 
            for content in Seq.tail contents do
                fprintf sw " %s" content
            sw.WriteLine()

let detectNerg (xpath: string) = 
    if xpath.Contains("Example",ignoreCase) then Example
    elif xpath.Contains("Note", ignoreCase) then Note
    elif xpath.Contains("Guidance", ignoreCase) then Guidance
    elif xpath.Contains("Rationale", ignoreCase) then Rationale
    else failwith "unrecognized nerg"

[<EntryPoint>]
let main argv =
    match argv with
    | [| xPathsFileName; docXFileName; outputFileName |] ->
            let xPaths = readXPaths xPathsFileName
            let doc = createXDocumentFromDocxFileName docXFileName
            let mgr = getManager doc
            let part1P = docXFileName.Contains("29500-1")
            let nerg = xPaths.Head |> detectNerg 
            let sectionFinder = 
                createSectionFinder 
                    (createTitleElemList doc mgr part1P)
            let nergList = 
                extractNERGList xPaths doc mgr 
            use sw = createTextWriteFromOutputFileName outputFileName
            help2 nerg nergList sectionFinder sw 
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1


