module XPathFindOccurrences.ForbiddenWords

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

let private help (str: string) =
    str.Contains("shall",ignoreCase) ||
    str.Contains("should",ignoreCase) ||
    str.Contains("may",ignoreCase) ||
    str.Contains("is required to",ignoreCase)||
    str.Contains("are required to",ignoreCase)

let hasForbiddenWords (contents: seq<string>) msp mep = 
    match contents |> Seq.length with
    | 0 ->
        failwith "hen"
    | 1 ->
        let onlyContent = Seq.head contents
        let choppedContent = 
            onlyContent.Substring(msp, onlyContent.Length + mep - msp)
        help choppedContent
    | _ ->
        let firstContent = Seq.head contents
        let choppedFirstContent = firstContent.Substring(msp)
        if help choppedFirstContent then true
        else 
            let lastContent = Seq.last contents    
            let choppedLastContent = 
                lastContent.Substring(0, lastContent.Length + mep)
            if help choppedLastContent then true
            else 
                let contentsInBetween = 
                    Seq.tail contents 
                    |> Seq.rev |> Seq.tail |> Seq.rev
                Seq.exists help contentsInBetween



