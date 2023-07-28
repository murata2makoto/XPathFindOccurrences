module Toolkit.SecTitle

open System.Xml.XPath
open System.Xml.Linq

let wml = XNamespace.Get "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

//すべてのヘッディングを得るためのXPath検索式

let secTitleQuery = ".//w:p[
                  .//w:r and
                  (starts-with(w:pPr/w:pStyle/@w:val, \"Heading\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"ISOClause1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"21\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"31\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"41\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"50\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"51\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix2\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix3\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix4\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix5\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix6\")
                  )
                  ]"

//                  or
//                  (w:pPr/w:pStyle/@w:val = \"UnnumberedHeading\")

//ヘッディングの深さ判定

let getHeadingElementLevel mgr (e: XElement)   =
    let pStyle = e.XPathSelectElement("w:pPr/w:pStyle", mgr)
    let styleName = pStyle.Attribute(wml+"val").Value
    match styleName with
    | "ISOClause1" -> 1
    | "1" -> 1
    | "21" -> 2
    | "31" -> 3
    | "41" -> 4
    | "50" -> 5
    | "51" -> 5
    | "Heading1" -> 1
    | "Heading2" -> 2
    | "Heading3" -> 3
    | "Heading4" -> 4
    | "Heading5" -> 5
    | "Heading6" -> 6
    | "Appendix1" -> 1
    | "Appendix2" -> 2
    | "Appendix3" -> 3
    | "Appendix4" -> 4
    | "Appendix5" -> 5
    | "Appendix6" -> 6
    | "Appendix7" -> 7
 //   | "UnnumberedHeading" -> 1
    | _ -> failwith "hen"

let getSubClauseNumber part1P (indexStack : int list): string =
    match List.rev indexStack with 
    | clauseNumber::subClauseNumber ->
        let clauseString =
            if part1P then
                if clauseNumber <= 23 then clauseNumber.ToString() //Annex
                else (char (65 + clauseNumber - 24)).ToString()
            else 
                if clauseNumber <= 20 then clauseNumber.ToString() //Annex
                else (char (65 + clauseNumber - 21)).ToString()
        let subclauseString =
           List.fold 
                (fun st t  -> st + "." + t.ToString())
                ""
                subClauseNumber
        if subclauseString <> "" then 
            clauseString + subclauseString
        else clauseString + "."
    | _ -> failwith "hen"

