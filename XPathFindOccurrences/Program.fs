// For more information see https://aka.ms/fsharp-console-apps

module XPathFindOccurrences.Program

open EnumerateParagraphsAndTableRows
open FindPosition
open Toolkit
open CreateSectionFinder
open GetMarkerPosition
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
    else failwith "Specify two XPath expressios"


let help2 (pairs: (string * 
                    XElement * int list * 
                    XElement * int list * seq<string>) list)
          finder  sw  = 
    fprintfn sw "%s\t%s\t%s\t%s\t%s"
        "Subclasue number"
        "Presense of normative modal verbs" 
        "Start Marker Position" 
        "End Marker Position"  
        "Content"    
    for (xpath, starElem, _ ,_ ,_ , contents) in pairs do 
        let msp = 
            getMarkerPosition xpath true (Seq.head contents)
        let mep = 
            getMarkerPosition xpath false (Seq.last contents)
        let sectionNumber = finder starElem
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
            let hash = createHashContainingParagarphsAndTableRows doc mgr
            let sectionFinder = 
                createSectionFinder 
                    (createTitleElemList doc mgr part1P)
            use sw = createTextWriteFromOutputFileName outputFileName
            let pairs = 
                findElementForXPaths xPaths doc mgr hash
                |> createPairs  |> addContent 
            let mutable previousIndex = -1
            help2 pairs sectionFinder sw 
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1


