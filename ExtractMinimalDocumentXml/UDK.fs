module UDK
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open System.Collections.Generic
open Toolkit.SecTitle


//使ってない

[<Literal>]
let WmlName = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

type Mode = DefaultDelete | DefaultKeep | DefaultUnwrap

let tobeKept_table_xpath = 
    ".//w:tbl[not(.//w:t[normalize-space(text())=\"Attributes\"])"

let tobeKept_figure_xpath = 
    ".//w:p[.//w:drawing]"

let toBeUnwrapped (elem: XElement) =
    match elem.Name.NamespaceName with
    | WmlName ->
        match elem.Name.LocalName with
        | "sdt" | "sdtContent" -> true
        | _ -> false
    | _ -> false;;

let toBeDeleted (elem: XElement) =
    match elem.Name.NamespaceName with
    | WmlName ->
        match elem.Name.LocalName with
        | "bookmarkStart" | "bookmarkEnd"
        | "sdtPr" | "sdtEndPr" -> true //sdtのしたにだけsdtPrとsdtEndPrはある。sdtはすべてunwrap
        | _ -> false
    | _ -> false;;

let minimize (elem: XElement) (toBeKept: XElement -> bool) 
    : XElement  =

    let rec createCopy (elem: XElement): XElement  = 
        let newElem = new XElement(elem.Name)
        for att in elem.Attributes() do
            newElem.SetAttributeValue(att.Name, att.Value)
        let xtextList = 
            [ for n in elem.Nodes() do 
                if n.NodeType = XmlNodeType.Text then
                    let xtext = n :?> XText
                    if xtext.Value.Trim() <> "" then
                        yield xtext]
        if not(xtextList.IsEmpty) && elem.HasElements then
            failwith "hen"
        elif not(xtextList.IsEmpty) then
            for xtext in xtextList do newElem.Add(xtext)
        elif elem.HasElements then
            let children =
                let childMode = //bodyの直下のときだけDefaultDiscardにする。あとはDefaultKeep。
                    if elem.Name.LocalName = "body" then DefaultDelete else DefaultKeep
                [for child in elem.Elements() do yield! help child childMode ]
            newElem.Add(children)
        else ()
        newElem
    and help (elem: XElement) mode: (XElement list) =
        if toBeUnwrapped elem then //toBeUnwrappedと判定されたものは、スキップされるが、子孫は処理対象となる。
            printfn "%A" elem.Name
            [for child in elem.Elements() do
                yield! help child mode]
        elif toBeDeleted elem then //toBeDeletedと判定されたものは、削除される。子孫は見ない。
            []
        elif toBeKept elem then //toBeKeptと判定されるなら、この要素は保持するが、子孫については再帰的に処理する。
            [createCopy elem]
        elif mode = DefaultUnwrap then //DefaultUnwrapモードなら削除する。
            printfn "%A" elem.Name
            [for child in elem.Elements() do
                yield! help child mode]
        elif mode = DefaultDelete then //DefaultDeleteモードなら削除する。
            []
        elif mode = DefaultKeep then //DefaultKeepモードなら、この要素は保持するが、子孫については再帰的に処理する。
            [createCopy elem]
        else
            failwith "Hen"
    help elem DefaultKeep |> List.head

let tobeKeptGen (doc: XDocument) (man: XmlNamespaceManager) xpathList =
    let hsList = 
        [for xpath in xpathList do
            let sq = doc.XPathSelectElements(xpath, man) 
            let hs = new HashSet<XElement>(sq)
            yield hs]
    (fun (elem: XElement) -> 
        hsList |> 
        List.exists (fun hs -> hs.Contains(elem)))

let extractMinimalDocumentXml (doc: XDocument) (man: XmlNamespaceManager) (xpathList: string list) = 
    let toBeKept  = 
        tobeKeptGen doc man xpathList
    new XDocument(minimize doc.Root  toBeKept  )

let xpathList tableOrFigure = 
    match tableOrFigure with
    |"table" ->  [secTitleQuery; tobeKept_table_xpath]
    |"figure" -> [secTitleQuery;  tobeKept_figure_xpath]
    | _ -> failwith "hen"