// For more information see https://aka.ms/fsharp-console-apps
open OOXML.Toolkit
open XPathFindOccurrences.SecTitle

open System.Xml.XPath
open System.Xml.Linq

let titleWR = 
    "/w:r[count(*) = 1]/w:t[count(*) = 0]"
    
let toBeHandledXPath = 
    ".//w:r[count(w:t) = 1 ]/w:t[count(*) = 0]"

let toBeEliminatedXPath =
  "w:r[(w:fldChar or w:instrText) and count(*) = 1]" 

let styleQuery = "w:pPr/w:pStyle/@w:val"

let unReplaceKeyWords str =
    let (pairs: (string*string) list) = 
        [("Wordprocessingml", "WordprocessingML");
        ("wordprocessingml", "WordprocessingML");
        ("Presentationml", "PresentationML");
        ("presentationml", "PresentationML");
        ("Spreadsheetml","SpreadsheetML");
        ("spreadsheetml","SpreadsheetML");
        ("Drawingml","DrawingML");
        ("drawingml","DrawingML");
        ("Shared mls","Shared MLs");
        ("shared mls","Shared MLs");
        ("Custom xml","Custom XML");
        ("custom xml","Custom XML");
        ("Html","HTML");
        ("html","HTML");
        ("Xsl ", "XSL ");
        ("xsl ", "XSL ");
        ("Vml", "VML");
        ("dos ", "DOS ")]
    pairs
    |> List.fold 
        (fun (x: string) (oldS, newS) ->  x.Replace(oldS, newS)) str


[<EntryPoint>]
let main argv =
    match argv with
    | [| docXFileName; outputFileName |] ->
        let doc = createXDocumentFromDocxFileName docXFileName
        let mgr = getManager doc
        let secTitleList = 
            doc.XPathSelectElements(secTitleQuery, mgr) 
        for secTitle in secTitleList do
            let WRs = secTitle.XPathSelectElements(toBeHandledXPath, mgr)
            if WRs |> Seq.length = 1 then
                let wr = WRs |> Seq.head
                let content = wr.Value
                content.Substring(0,1)+content.Substring(1).ToLower()
                |> unReplaceKeyWords 
                |>(fun newContent ->
                    let newXText= new XText(newContent)
                    wr.ReplaceNodes([newXText]))
        createDocXFromXDocument doc outputFileName
        0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1