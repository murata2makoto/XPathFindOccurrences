module XPathFindOccurrences.DetectXsd

let detectXsd (nergContents: string seq): bool = 
    nergContents
    |> Seq.exists 
         (fun x -> 
           x.Contains("W3C XML Schema definition"))