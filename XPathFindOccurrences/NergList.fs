module XPathFindOccurrences.NERGList

open System.Collections.Generic
open System.Xml.XPath
open System.Xml.Linq
open MergeSort

let private findPosition 
  (e: XElement) (hash: Dictionary<XElement, int list>) =
    let mutable countList = []
    for a in e.AncestorsAndSelf() do
        if hash.ContainsKey(a) then 
            countList <- hash.[a]
        else ()
    if countList.Length = 0  then failwith "empty!"
    countList

let private comparer 
    ((p1i, _, e1): int*string*XElement) 
    ((p2i, _, e2): int*string*XElement) =
    if e1.IsBefore(e2) then -1
    elif e2.IsBefore(e1) then 1
    elif p1i < p2i then -1
    elif p1i > p2i then 1
    elif p1i = p2i then 0
    else failwith "hen";;

let private findElementForXPaths 
        xpathList (doc: XDocument) manager = 
    let help i xpath =
        try
            [for x in doc.XPathSelectElements(xpath, manager) do
                yield i, xpath, x] 
        with 
            | :? System.Xml.XPath.XPathException as e ->
                printfn "%s: %s" (e.Message) xpath
                failwith "" 
    match xpathList with
    | [startMarkerXPath;endMarkerXPath] ->
        let startMarkers = help 0 startMarkerXPath
        let endMarkers = help 1 endMarkerXPath
        mergesort1 comparer startMarkers endMarkers
    |_ -> failwith "hen"
 
let private mergeable1 (e1: XElement) (e2: XElement) = 
    e1 = e2 || (Seq.contains e2 (e1.ElementsAfterSelf())) 


let private createPairs (l: (int*string* XElement ) list) = 
    let mutable (prevPindex, prevXpath, prevXelem) = 
        (-1,"", null)
    [for (pindex, xpath, xelem) in l do
        match prevPindex, pindex with 
        | -1, 0 ->
            prevPindex <- 0
            prevXpath <- xpath
            prevXelem <-xelem
        | -1, 1 ->
            printfn "Skipped: %s" xelem.Value
            ()
        | 0, 0 -> 
            printfn "Skipped: %s" prevXelem.Value
            prevPindex <- 0
            prevXpath <- xpath
            prevXelem <-xelem
        | 0, 1 -> 
            if mergeable1 prevXelem xelem then
                yield (prevXpath, prevXelem, xelem)
            prevPindex <- -1
        | _ -> failwith "hen"
        ]

let private addContent (l: (string*XElement * XElement ) list) =
    [for (xpath, xe1, xe2) in l do
        if xe1 = xe2 then
            let contents = 
                xe1.Value.Replace("\"", "\"\"") |> Seq.singleton
            yield xpath, xe1, xe2, contents
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
                yield xpath, xe1,  xe2, contents]

let extractNERGList xpathList (doc: XDocument) manager = 
    findElementForXPaths xpathList doc manager  
    |> createPairs  |> addContent 