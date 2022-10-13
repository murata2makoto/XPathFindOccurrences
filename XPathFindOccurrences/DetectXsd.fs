module XPathFindOccurrences.DetectXsd

let detectXsd (nergContents: string seq): bool = 
    if Seq.isEmpty nergContents then 
        failwith "empty contents"
    else
        nergContents
        |> Seq.exists 
            (fun x -> 
                x.Contains("W3C XML Schema definition"))