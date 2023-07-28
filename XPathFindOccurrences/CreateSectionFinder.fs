module XPathFindOccurrences.CreateSectionFinder

open System.Xml.Linq
open Toolkit.SecTitle
open System.Xml.XPath
open TokenOrParenthesis

let private createTitleElemList (doc: XDocument) mgr part1P =
    let mutable titleElemList: (List<XElement * string> ) = []

    let addAction  (e:XElement) (stack: int list) =
        let printStr = getSubClauseNumber part1P stack
        titleElemList <- (e, printStr)::titleElemList 

    let secTitleList = 
        doc.XPathSelectElements(secTitleQuery, mgr) 

    nest secTitleList (getHeadingElementLevel mgr) addAction

    titleElemList |> List.rev

let getSectionNumberPossiblyFollowedByTableNumberAndRowNumber 
        (head: XElement * string) (p: XElement) mgr =

    let ancestorTableXPath =
        "ancestor::w:tbl[not(ancestor::w:tbl)]"
    let ancestorTableRowXPath =
        "ancestor::w:tr[not(ancestor::w:tr)]"
    let tbl = p.XPathSelectElement(ancestorTableXPath, mgr)
    let tr = p.XPathSelectElement(ancestorTableRowXPath, mgr)
    if tbl = null then
        snd head
    else 
        let rowNumberObj = 
            tr.XPathEvaluate("count(preceding-sibling::w:tr)", mgr)
        let rowNumber = 
            int (rowNumberObj :?> double)
        let followingTables = 
            (fst head).XPathSelectElements("following
            ::w:tbl[not(ancestor::w:tbl)]", mgr)
        let tableNumber = 
            (followingTables |> Seq.findIndex (fun xe -> xe = tbl))
            + 1
        sprintf "%sT%dR%d" (snd head) tableNumber rowNumber

let createSectionFinder (doc: XDocument) mgr part1P =
//すべてのtitle elementから探すのではなく、前回見つけたもの以降しか探さない

    let mutable titleElemList: (List<XElement * string> ) = 
        createTitleElemList doc mgr part1P 

    let mutable backupTitleElemList = titleElemList

    let reset() = 
        titleElemList <- backupTitleElemList

    let finder (p: XElement) =
        let mutable found = false
        let mutable previousHead = (null, "")
        let mutable head = List.head titleElemList
        let mutable remaining = List.tail titleElemList
        let mutable reachedEnd = false
        while not(found) do
            if (fst head).IsAfter(p) then found <- true
            elif remaining.IsEmpty then
                reachedEnd <- true
                found <- true
            else
                previousHead <- head
                head <- List.head remaining
                remaining <- List.tail remaining
        if not reachedEnd && snd previousHead <> "" then
            titleElemList <- previousHead::head::remaining
        
        if reachedEnd then "Bibliography" else
        getSectionNumberPossiblyFollowedByTableNumberAndRowNumber previousHead p mgr

    (finder, reset)