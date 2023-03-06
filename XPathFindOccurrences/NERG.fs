module XPathFindOccurrences.NERG

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase
let private respectCase = System.StringComparison.CurrentCulture

type NERG = Note | Example | Rationale | Guidance | ExampleBlock;;

let detectNerg (xpath: string) = 
    if xpath.Contains("EXAMPLE",respectCase) then ExampleBlock
    elif xpath.Contains("Example",ignoreCase) then Example
    elif xpath.Contains("Note", ignoreCase) then Note
    elif xpath.Contains("Guidance", ignoreCase) then Guidance
    elif xpath.Contains("Rationale", ignoreCase) then Rationale
    else failwith "unrecognized nerg"