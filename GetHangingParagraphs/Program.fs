// For more information see https://aka.ms/fsharp-console-apps
open System.IO
open Toolkit.ReadWrite

let createTextReaderFromInputFileName inputFileName = 
    let enc = new System.Text.UTF8Encoding(true)
    let ifs = new FileStream(inputFileName, FileMode.Open)
    new StreamReader(ifs, enc)

let readPairs (sr: StreamReader) =
    [while not(sr.EndOfStream) do 
        let line = sr.ReadLine()
        match line.Split([|'\t'|], 2) with 
        | [|secNum; paraContent |] when paraContent.Trim() <>"" ->
            yield (secNum, paraContent)
        | _ -> printfn "strange line: %s" line   ]

let pairs2Triplets (pairsList: (string*string) list) =
    [for (heading, content) in pairsList do
        let tIndex = heading.IndexOf("T")
        if tIndex <> -1 then
            let secNum = heading.Substring(0, tIndex)
            let tfNum = heading.Substring(tIndex)
            yield
                secNum,
                tfNum, 
                content
        else yield heading, "", content]
    
let getSecNumSet (triplets: (string*string*string)list) =
    set [for (secNum, _, _) in triplets do yield secNum]

let getNested (triplets: (string*string*string)list) =
    let grouped = 
        List.groupBy (fun (secNum, _, _) -> secNum) triplets
    grouped 
    |> List.map (fun (k, l) -> k, List.map (fun (_,y,z) -> (y,z)) l)

let selectHangingParagraphs  (triplets: (string*string*string)list) =
    let secNumSet = getSecNumSet triplets
    let nested = getNested triplets
    nested 
    |> List.filter 
        (fun (k, vl) -> 
                if vl.Length = 1 then false
                elif k.EndsWith(".") then secNumSet.Contains(k+"1")
                else secNumSet.Contains(k+".1"))

let writeNestedTriplets (sw: StreamWriter) 
            (nestedTriplets: (string * (string * string) list) list )= 
    sw.WriteLine("Subclause number\tPara Content")
    for (secNum, tfNumAndContentList) in nestedTriplets do
        for tfNum, paraContent in tfNumAndContentList do
            if tfNum="" then
                fprintfn sw "%s\t\t%s" secNum paraContent
            else
                fprintfn sw "%s\t%s\t%s " secNum tfNum paraContent
        sw.WriteLine()

[<EntryPoint>]
let main argv =
    match argv with
    | [| secNumParaContentPairFileName; outputFileName |] ->
            use sw = createTextWriteFromOutputFileName outputFileName
            use sr = createTextReaderFromInputFileName secNumParaContentPairFileName
            let pairs = readPairs sr
            let triplets = pairs2Triplets pairs
            let hangingParagraphs = selectHangingParagraphs triplets
            writeNestedTriplets sw hangingParagraphs
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1