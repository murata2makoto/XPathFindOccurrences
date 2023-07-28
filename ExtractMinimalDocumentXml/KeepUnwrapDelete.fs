module KeepUnwrapDelete


open System.Xml
open System.Xml.Linq

//結果の長さが1
let rec private copyElem (elem: XElement) toBeUnwrapped toBeDeleted: XElement  = 
    let newElem = new XElement(elem.Name)
    for att in elem.Attributes() do
        newElem.SetAttributeValue(att.Name, att.Value)
    let xtextList = 
        [ for n in elem.Nodes() do 
            if n.NodeType = XmlNodeType.Text then
                let xtext = n :?> XText
                if xtext.Value <> "" then
                    yield xtext]
    if not(xtextList.IsEmpty) && elem.HasElements then //mixed content
        failwith "hen"
    elif not(xtextList.IsEmpty) then
        for xtext in xtextList do newElem.Add(xtext)
    elif elem.HasElements then
        let children =
            [for child in elem.Elements() do 
                yield! createElemList child toBeUnwrapped toBeDeleted ]
        newElem.Add(children)
    else () //empty element
    newElem

//結果の長さが不定
and private createElemList (elem: XElement) toBeUnwrapped toBeDeleted : (XElement list) =
    if toBeUnwrapped elem then //toBeUnwrappedと判定されたものは、スキップされるが、子孫は処理対象となる。
        [for child in elem.Elements() do
            yield! createElemList child toBeUnwrapped toBeDeleted]
    elif toBeDeleted elem then //toBeDeletedと判定されたものは、削除される。子孫は見ない。
        []
    else
        [copyElem elem toBeUnwrapped toBeDeleted]

let copyKUD (elem: XElement)  toBeUnwrapped toBeDeleted  : XElement  =
    createElemList elem  toBeUnwrapped toBeDeleted  |> List.head
