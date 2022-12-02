open OOXML.Toolkit
open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let countChildLocalNames (matches: XElement seq)  =
    matches
    |> Seq.countBy (fun e -> e.Name.LocalName)

let printResult (xPath: string) matches = 
    if xPath.EndsWith("*") then
        for (key, count) in countChildLocalNames matches do
            let childXPath = xPath.TrimEnd('*') + key
            printfn "Matches of %s: %d" childXPath count
    else 
        printfn "Matches of %s: %d" xPath (Seq.length matches)

[<EntryPoint>]
let main argv =
    match argv with
    | [| docXFileName; xPathsFileName|] ->
            let doc = createXDocumentFromDocxFileName docXFileName
            let mgr = getManager doc
            let xPaths = readXPaths xPathsFileName
            printfn "%s" docXFileName
            for xPath in xPaths do
                let matches = doc.XPathSelectElements(xPath, mgr)
                printResult xPath matches
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1