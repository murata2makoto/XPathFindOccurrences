open System.IO;
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open System.Collections.Generic



let  copyWithout (docElement:  XElement) (toBeDropped: HashSet<XElement>) (toBeKept: HashSet<XElement>) =
    let rec help (elem:  XElement) =
        let shallowCopy = new XElement(elem.Name)
        for att in  elem.Attributes() do
            shallowCopy.SetAttributeValue(att.Name, att.Value)
        let xtextList = 
            [ for n in elem.Nodes() do 
                if n.NodeType = XmlNodeType.Text then
                    let xtext = n :?> XText
                    if xtext.Value.Trim() <> "" then
                        yield xtext]
        if not(xtextList.IsEmpty) && not(elem.Elements() |> Seq.isEmpty) then failwith "hen"
        for xtext in xtextList do shallowCopy.Add(xtext)
        let children =
            [for child in elem.Elements() do
                if not(toBeDropped.Contains(child)) || toBeKept.Contains(child)  then
                    yield help child  ]
        shallowCopy.Add(children)
        shallowCopy
    help docElement

let extractMinimalDocumentXml (doc: XDocument) (man: XmlNamespaceManager)  = 
    let toBeKept = getHeadings doc man |> List.map (fun (_, _, x) -> x)
    let toBeKeptSet = new HashSet<XElement>(toBeKept)
    let toBeDropped = 
        seq{ for path in stopXPathList do
                yield! doc.XPathSelectElements(path, man) } 
    let toBeDroppedSet =  new HashSet<XElement>(toBeDropped)
    printfn "length %d" (Seq.length toBeDropped)
    new XDocument(copyWithout doc.Root toBeDroppedSet toBeKeptSet)