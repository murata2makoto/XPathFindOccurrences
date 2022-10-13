module XPathFindOccurrences.ForbiddenWords

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

let private help (str: string) =
    str.Contains("shall",ignoreCase) ||
    str.Contains("should",ignoreCase) ||
    str.Contains("may",ignoreCase) ||
    str.Contains("is required to",ignoreCase)||
    str.Contains("are required to",ignoreCase)

let hasForbiddenWords (nergContents: string seq) = 
    if Seq.isEmpty nergContents then 
        failwith "empty contents"
    else
        nergContents
        |> Seq.exists help
