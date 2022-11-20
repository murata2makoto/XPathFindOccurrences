module XPathFindOccurrences.NERG

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

type NERG = Note | Example | Rationale | Guidance;;

let detectNerg (xpath: string) = 
    if xpath.Contains("Example",ignoreCase) then Example
    elif xpath.Contains("Note", ignoreCase) then Note
    elif xpath.Contains("Guidance", ignoreCase) then Guidance
    elif xpath.Contains("Rationale", ignoreCase) then Rationale
    else failwith "unrecognized nerg"