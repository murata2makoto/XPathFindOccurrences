module SecHeaderAndNumber

open System.Collections.Generic
open System.Xml
open System.Xml.XPath
open System.Xml.Linq

let wml = XNamespace.Get "http://schemas.openxmlformats.org/wordprocessingml/2006/main"


let bookmarkQuery = ".//w:p[w:hyperlink[@w:anchor]]"
let secTitleQuery = ".//w:p[w:bookmarkStart and .//w:t ]"
let previousSecTitleQuery = 
    "preceding-sibling::w:p[w:bookmarkStart and .//w:t ]"

let private createBookMarkHash (body:XElement) (manager:XmlNamespaceManager) =
    let titleElemHash = new Dictionary<string, string>()
    let paras = body.XPathSelectElements(bookmarkQuery, manager)
    for para in paras do
        let hyperLinks = para.XPathSelectElements(".//w:hyperlink", manager)
        for hl in hyperLinks do
            let tocId = hl.Attribute(wml + "anchor").Value
            let secNum = hl.XPathSelectElement(".//w:t", manager).Value
            if not(tocId.StartsWith("xsd") || 
                   tocId.StartsWith("XSD")) then
              titleElemHash.Add(tocId, secNum)
    titleElemHash


let createTitleElemList (doc: XDocument) mgr =
    let mutable titleElemList = []
    let bkmrkHash = createBookMarkHash (doc.Root.Element(wml+"body")) mgr
    let secTitleList = doc.XPathSelectElements(secTitleQuery, mgr)
    for secTitle in secTitleList do 
        for bkmkStrt in secTitle.Elements(wml + "bookmarkStart") do
              let bookmark = bkmkStrt.Attribute(wml+"name").Value
              if bkmrkHash.ContainsKey(bookmark) then 
                let secNum = bkmrkHash.[bookmark]
                titleElemList <- (secTitle, secNum)::titleElemList
    titleElemList |> List.rev

let createSectionFinder (doc: XDocument) mgr =
//すべてのtitle elementから探すのではなく、前回見つけたもの以降しか探さない
    let mutable titleElemList = createTitleElemList doc mgr
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