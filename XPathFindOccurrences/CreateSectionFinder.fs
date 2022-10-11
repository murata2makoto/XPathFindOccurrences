module XPathFindOccurrences.CreateSectionFinder

open System.Xml.Linq
open SecTitle

let createTitleElemList (doc: XDocument) mgr part1P =
    let mutable titleElemList: (List<XElement * string> ) = []

    let addAction getNumber (e:XElement) (stackOfStack: int list list) =
        let topStackList = List.map (fun l -> List.head l) stackOfStack
        let printStr = getNumber topStackList
        titleElemList <- (e, printStr)::titleElemList 

    scanSecTitles2 doc mgr (addAction (getSubClauseNumber part1P))
    titleElemList |> List.rev

let createSectionFinder titleElemList =
//すべてのtitle elementから探すのではなく、前回見つけたもの以降しか探さない

    let mutable titleElemList: (List<XElement * string> ) = titleElemList

    let finder (p: XElement) =
        let mutable found = false
        let mutable previousHead = (null, "")
        let mutable head = List.head titleElemList
        let mutable remaining = List.tail titleElemList
        while not(found) do
            if (fst head).IsAfter(p) then found <- true
            else
                previousHead <- head
                head <- List.head remaining
                remaining <- List.tail remaining
        titleElemList <- previousHead::head::remaining
        snd previousHead

    finder