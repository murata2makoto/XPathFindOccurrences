module GetStartEndMarkerPairs

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

let private getAllIndexes (string: string) (substring: string) =
    let help (startIndex: int) =
        string.IndexOf(substring,startIndex)
    [let mutable startIndex = 0;
     let mutable flag = true;
     while flag do
        let pos = help startIndex
        if pos <> -1 then 
            yield pos
            startIndex <- pos + 1
        else flag <- false]

let getStartEndMarkerPairs (xpath: string) (contents: seq<string>) = 
    let help startMarker endMarker endMarkerLength =
        let startMarkers = 
            getAllIndexes (Seq.head contents) startMarker
        let endMarkers =
            let lastContent = Seq.last contents
            getAllIndexes lastContent endMarker
            |> List.map (fun pos -> pos + endMarkerLength - lastContent.Length)
        List.map2 (fun x y -> (x,y)) startMarkers endMarkers

    if xpath.Contains("Example",ignoreCase) then
        help "[Example:" "end example" 11
    elif xpath.Contains("Note", ignoreCase) then
        help "[Note:" "end note" 8
    elif xpath.Contains("Guidance", ignoreCase) then
        help "[Guidance:" "end guidance" 12
    elif xpath.Contains("Rationale", ignoreCase) then
        help "[Rationale:" "end rationale" 13
    else failwith "hen" 