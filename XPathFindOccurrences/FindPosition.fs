module XPathFindOccurrences.FindPosition

open System.Collections.Generic
open System.Xml.XPath
open System.Xml.Linq

let private findPosition 
  (e: XElement) (hash: Dictionary<XElement, int list>) =
    let mutable countList = []
    for a in e.AncestorsAndSelf() do
        if hash.ContainsKey(a) then 
            countList <- hash.[a]
        else ()
    countList

let private comparer 
    ((p1i, _, e1, _): int*string*XElement*'a) 
    ((p2i, _, e2, _): int*string*XElement*'a) =
    if e1.IsBefore(e2) then -1
    elif e2.IsBefore(e1) then 1
    elif p1i < p2i then -1
    elif p1i > p2i then 1
    elif p1i = p2i then 0
    else failwith "hen";;

let findElementForXPaths 
        xpathList (doc: XDocument) manager 
        (hash: Dictionary<XElement, int list>)= 
    let xpathIndex_elem_list = 
        xpathList 
        |> List.mapi (fun i xpath -> 
                        try
                            doc.XPathSelectElements(xpath, manager)
                            |> List.ofSeq
                            |> List.map (fun x -> i, xpath, x,                                          findPosition x hash)
                        with 
                        | :? System.Xml.XPath.XPathException as e ->
                            printfn "%s: %s" (e.Message) xpath
                            failwith "")   
    [for l in xpathIndex_elem_list do yield! l]
    |> List.sortWith comparer
    
let createPairs (l: (int*string* XElement * int list) list) = 
    let mutable (prevPindex, prevXpath, prevXelem, prevPosList) = 
        (-1,"", null, [])
    [for (pindex, xpath, xelem, posList) in l do
        match prevPindex, pindex with 
        | -1, 0 ->
            prevPindex <- 0
            prevXpath <- xpath
            prevXelem <-xelem
            prevPosList <- posList
        | -1, 1 ->
            ()
        | 0, 0 -> 
            prevPindex <- 0
            prevXpath <- xpath
            prevXelem <-xelem
            prevPosList <- posList
        | 0, 1 -> 
            yield (prevXpath, prevXelem, prevPosList, xelem, posList)
            prevPindex <- -1
        | _ -> failwith "hen"
        ]

let addContent (l: (string*XElement * int list * XElement * int list) list) =
    [for (xpath, xe1, posLst1, xe2, posLst2) in l do
        if xe1 = xe2 then
            let contents = 
                xe1.Value.Replace("\"", "\"\"") |> Seq.singleton
            yield xpath, xe1, posLst1, xe2, posLst2, contents
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
                yield xpath, xe1, posLst1, xe2, posLst2, contents]