module XPathFindOccurrences.DetectXsd

let detectXsd (contents: string seq): bool = 
    if Seq.isEmpty contents then 
        failwith "empty contents"
    else
        let firstContent = Seq.head contents
        firstContent.Contains("W3C XML Schema definition")