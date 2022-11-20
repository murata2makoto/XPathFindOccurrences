module XPathFindOccurrences.NERGList
//NERG stands for Notes, Examples, Rationales, and Guidances.

open System.Collections.Generic
open System.Xml.XPath
open System.Xml.Linq

let private findElementForXPaths 
        xpathList (doc: XDocument) manager = 
    let help xpath =
        try
            doc.XPathSelectElements(xpath, manager)
            |> List.ofSeq
        with 
            | :? System.Xml.XPath.XPathException as e ->
                printfn "%s: %s" (e.Message) xpath
                failwith "" 
    match xpathList with
    | [startMarkerXPath;endMarkerXPath] ->
        let startMarkers = help startMarkerXPath
        let endMarkers = help endMarkerXPath
        startMarkers, endMarkers
    |_ -> failwith "hen"
 
let private mergeable1 (e1: XElement) (e2: XElement) = 
    e1 = e2 || (Seq.contains e2 (e1.ElementsAfterSelf())) 


let private createPairs2 sectionFinder 
                            (startMarkerList: XElement list, 
                            endMarkerList: XElement list) =
    let rec help startMarkerList endMarkerList revList =
        match startMarkerList, endMarkerList with
        | [], [] -> revList
        | [], _ -> 
            for (em: XElement) in endMarkerList do 
                let secNum = sectionFinder em
                printfn "Skipped: %s %s" secNum em.Value
            revList
        | _, [] -> 
            for (em: XElement) in startMarkerList do 
                let secNum = sectionFinder em
                printfn "Skipped: %s %s" secNum em.Value
            revList
        | stMarker::startMarkerTail, edMarker::endMarkerTail ->
            if mergeable1 stMarker edMarker then
                help startMarkerTail endMarkerTail ((stMarker, edMarker)::revList)
            elif stMarker.IsBefore(edMarker) then //skip stMarker
                let secNum = sectionFinder stMarker
                printfn "Skipped: %s %s" secNum stMarker.Value
                help startMarkerTail endMarkerList revList
            elif edMarker.IsBefore(stMarker) then //skip edMarker
                let secNum = sectionFinder edMarker
                printfn "Skipped: %s %s" secNum edMarker.Value
                help startMarkerList endMarkerTail revList
            else failwith "hen"
    help startMarkerList endMarkerList []
    |> List.rev

let private addContent (l: (XElement * XElement ) list) =
    [for (xe1, xe2) in l do
        if xe1 = xe2 then
            let contents = 
                xe1.Value.Replace("\"", "\"\"") |> Seq.singleton
            yield xe1, xe2, contents
        else     
            let elderSiblings = 
                xe1.NodesAfterSelf()
                |> Seq.filter (fun node -> 
                            match node with 
                            | :? XElement -> true 
                            | _ -> false)
                |> Seq.map (fun node -> node :?> XElement)
                |> Seq.append (Seq.singleton xe1) 
            let allElems = 
                elderSiblings |> 
                Seq.filter (fun sib -> sib = xe2 || sib.IsBefore(xe2))
            let rec allElems1 candidates  =
                if Seq.isEmpty candidates then Seq.empty
                else
                    let candHead = Seq.head candidates
                    if candHead = xe2 then Seq.singleton candHead
                    else Seq.append (seq{candHead})
                            (allElems1 (Seq.tail candidates))
            let contents = 
                allElems1 elderSiblings 
                |> Seq.map (fun xei ->  
                        xei.Value.Replace("\"", "\"\""))
            if xe1 = xe2 || xe1.NodesAfterSelf() |> Seq.contains xe2 then
                yield xe1,  xe2, contents]

let extractNERGList xpathList (doc: XDocument) manager sectionFinder = 
    findElementForXPaths xpathList doc manager  
    |> createPairs2 sectionFinder
    |> addContent 