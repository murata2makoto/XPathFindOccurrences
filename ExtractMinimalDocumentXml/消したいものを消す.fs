module 消したいものを消す

open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open 共通XPath定義
open KeepUnwrapDelete
open System.Collections.Generic

let natable= "w:tbl[not(w:tr[1]/w:tc[1]/w:p[1]/w:r[1]/w:t[1 and text()=\"Attributes\"])]"
let atable=  "w:tbl[    w:tr[1]/w:tc[1]/w:p[1]/w:r[1]/w:t[1 and text()=\"Attributes\"] ]"
let atable内の消したい位置 = $"{atable}/w:tr[position()>1]/w:tc[position()>1]"
let natable内の消したい位置 = $"{natable}/w:tr[position()>1]/w:tc[position()>1]"

let 消したいもののpredicate_tbl = 
    $"not(descendant-or-self::w:tbl | descendant-or-self::{secTitleQuery} | descendant-or-self::w:sectPr)"
let 消したいものでbody直下_tbl = $"//w:body/w:*[{消したいもののpredicate_tbl}]"
let 消したいrでatable内_tbl = $"//{atable内の消したい位置}/w:p/w:r[{消したいもののpredicate_tbl}]"
let tableを含まないatable = $"//{atable}[not(.//w:tbl)]"

let 消したいもののpredicate_drawing = 
    $"not(descendant-or-self::w:drawing | descendant-or-self::{secTitleQuery} | descendant-or-self::w:sectPr)"
let 消したいものでbody直下_drawing = $"//w:body/w:*[{消したいもののpredicate_drawing}]"
let 消したいrでatable内_drawing = $"//{atable内の消したい位置}//w:r[{消したいもののpredicate_drawing}]"
let 消したいrでnatable内_drawing = $"//{natable内の消したい位置}//w:r[{消したいもののpredicate_drawing}]" 



let 消す_tbl (elem: XElement)  (man: XmlNamespaceManager) : XElement  =
    let xpath = 
        $"{消したいものでbody直下_tbl}|{消したいrでatable内_tbl}|{tableを含まないatable}" //|
    let withoutElemSet = elem.XPathSelectElements(xpath, man) |> (fun x -> new HashSet<XElement>(x))
    let toBeUnrapped (e:XElement) = false
    let toBeDeleted (e:XElement) = 
        withoutElemSet.Contains(e)
    copyKUD elem toBeUnrapped toBeDeleted
    (*
    for e in elemSeq do
       e.Remove()
    elem
    *)

let 消す_drawing (elem: XElement)  (man: XmlNamespaceManager) : XElement  =
    let xpath = 
        $"{消したいものでbody直下_drawing}|{消したいrでatable内_drawing}|{消したいrでnatable内_drawing}"
          //|{figureを含まないatable}
    let withoutElemSeq = elem.XPathSelectElements(xpath, man) |> (fun x -> new HashSet<XElement>(x))
    let toBeUnrapped (e:XElement) = false
    let toBeDeleted (e:XElement) = 
        withoutElemSeq.Contains(e)
    copyKUD elem toBeUnrapped toBeDeleted
    (*
    for e in elemSeq do
       printfn "%A" e
       e.Remove()
    elem
    *) 
