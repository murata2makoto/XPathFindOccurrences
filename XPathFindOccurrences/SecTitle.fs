module SecTitle

open System.Xml.XPath
open System.Xml.Linq
open TokenOrParenthesis

let wml = XNamespace.Get "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

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
                  (w:pPr/w:pStyle/@w:val = \"Appendix4\"))
                  ]"

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
    | "Appendix1" -> 1
    | "Appendix2" -> 2
    | "Appendix3" -> 3
    | "Appendix4" -> 4
    | _ -> failwith "hen"

let scanSecTitles2 (doc: XDocument) mgr action =
    let secTitleList = 
        doc.XPathSelectElements(secTitleQuery, mgr)           
    nest secTitleList (getHeadingElementLevel mgr) action


let getSubClauseNumber part1P (x : int list) =
    match List.rev x with 
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

