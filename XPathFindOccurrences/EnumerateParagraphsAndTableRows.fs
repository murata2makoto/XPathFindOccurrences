module XPathFindOccurrences.EnumerateParagraphsAndTableRows

open System.Collections.Generic
open System.Xml.XPath
open System.Xml.Linq
open MergeSort

let tblQ = "//w:body/w:tbl"
let pQ = "//w:body/w:p"
let sdtTblQ = "//w:body/w:sdt/w:sdtContent/w:tbl"
let sdtPQ = "//w:body/w:sdt/w:sdtContent/w:p"
let sdtSdtTblQ = "//w:body/w:sdt/w:sdtContent/w:sdt/w:sdtContent/w:tbl"
let sdtSdtPQ = "//w:body/w:sdt/w:sdtContent/w:sdt/w:sdtContent/w:p"
let trQ = "w:tr"

let private pOrTbl (doc: XDocument) manager =
  let find pathExp =
    doc.XPathSelectElements(pathExp, manager)
    |> List.ofSeq
  let xElemComparer (e1: XElement) (e2: XElement) =
    if e1 = e2 then 0
    elif e1.IsBefore(e2) then -1
    else  1 //e2.IsBefore(e1) 
  find tblQ
  |> mergesort1 xElemComparer (find pQ)
  |> mergesort1 xElemComparer (find sdtTblQ)
  |> mergesort1 xElemComparer (find sdtPQ)
  |> mergesort1 xElemComparer (find sdtSdtTblQ)
  |> mergesort1 xElemComparer (find sdtSdtPQ)


let private visitParagraph (e: XElement) nth (hash: Dictionary<_,_>) =
  hash.[e] <- [nth]

let private visitTbl (e: XElement) nth (hash: Dictionary<_,_>) manager =
  let trs = e.XPathSelectElements(trQ, manager)
  trs |> Seq.iteri (fun i trElem -> 
                    hash.[trElem] <- [nth; i])

let createHashContainingParagarphsAndTableRows (doc: XDocument) manager =
  let hashtable = new Dictionary<XElement, int list>()
  pOrTbl doc manager
  |> List.iteri 
    (fun i e -> 
        match e.Name.LocalName with 
        | "p" -> visitParagraph e i hashtable
        | "tbl" -> visitTbl e i hashtable manager
        | _ -> ()
    )
  hashtable