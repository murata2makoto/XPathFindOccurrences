module XPathFindOccurrences.GetStartEndMarkerPairs
open NERG

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

let private getAllIndexes (string: string) (substring: string) =
    [let mutable startIndex = 0;
     let mutable flag = true;
     while flag do
        let pos = string.IndexOf(substring,startIndex)
        if pos <> -1 then 
            yield pos
            startIndex <- pos + 1
        else flag <- false]

let getStartEndMarkerPairs nerg (xpath: string) (contents: seq<string>) = 
    let help startMarker endMarker endMarkerLength =
        let startMarkers = 
            getAllIndexes (Seq.head contents) startMarker
        let endMarkers =
            let lastContent = Seq.last contents
            getAllIndexes lastContent endMarker
            |> List.map (fun pos -> pos + endMarkerLength - lastContent.Length)
        (startMarkers, endMarkers)
        ||> List.map2 (fun x y -> 
                        match Seq.length contents with
                        | 0 -> failwith "hen"
                        | 1 ->
                            let onlyContent = Seq.head contents
                            let nergContent = 
                                onlyContent.Substring(x, onlyContent.Length - x + y)
                            (x,y,Seq.singleton nergContent) 
                        | _ -> 
                            contents 
                            |> Seq.mapi 
                                (fun i content ->
                                    if i = 0 then
                                        content.Substring(x)
                                    elif i = Seq.length contents - 1 then
                                        content.Substring(0, content.Length + y)
                                    else content)
                            |> (fun z -> x, y, z)
                                )
    match nerg with 
    | Example ->
        help "[Example:" "end example]" 12
    | Note ->
        help "[Note:" "end note]" 9
    | Guidance ->
        help "[Guidance:" "end guidance]" 13
    | Rationale ->
        help "[Rationale:" "end rationale]" 14