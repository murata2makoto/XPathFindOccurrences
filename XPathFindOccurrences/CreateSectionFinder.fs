module XPathFindOccurrences.CreateSectionFinder

open System.Xml.Linq
open SecTitle
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
        snd previousHead

    (finder, reset)