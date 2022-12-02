// For more information see https://aka.ms/fsharp-console-apps
open System.IO
open OOXML.Toolkit

let createTextReaderFromInputFileName inputFileName = 
    let enc = new System.Text.UTF8Encoding(true)
    let ifs = new FileStream(inputFileName, FileMode.Open)
    new StreamReader(ifs, enc)

let readPairs (sr: StreamReader) =
    [while not(sr.EndOfStream) do 
        let line = sr.ReadLine()
        match line.Split([|'\t'|], 2) with 
        | [|secNum; paraContent |] ->
            yield (secNum, paraContent)
        | _ -> printfn "strange line: %s" line   ]

let getSecNumSet (pairs: (string*string)list) =
    set [for (secNum, paraContent) in pairs do yield secNum]

let getNested (pairs: (string*string)list) =
    let grouped = 
        List.groupBy (fun (secNum, paraContent) -> secNum) pairs
    grouped 
    |> List.map (fun (k, l) -> k, List.map snd l)

let selectHangingParagraphs  (pairs: (string*string)list) =
    let secNumSet = getSecNumSet pairs
    let nested = getNested pairs
    nested 
    |> List.filter 
        (fun (k, vl) -> 
                if vl.Length = 1 then false
                else secNumSet.Contains(k+".1"))

let writeNestedPairs (sw: StreamWriter) 
            (nestedPairs: (string * string list) list )= 
    sw.WriteLine("Subclause number\tPara Content")
    for (secNum, paraContentList) in nestedPairs do
        sw.Write(secNum+ "\t")
        for paraContent in paraContentList do
            sw.Write(paraContent+" ")
        sw.WriteLine()

[<EntryPoint>]
let main argv =
    match argv with
    | [| secNumParaContentPairFileName; outputFileName |] ->
            use sw = createTextWriteFromOutputFileName outputFileName
            use sr = createTextReaderFromInputFileName secNumParaContentPairFileName
            let pairs = readPairs sr
            let hangingParagraphs = selectHangingParagraphs pairs
            writeNestedPairs sw hangingParagraphs
            sw.Close()
            0
    | _ -> 
        printfn "Illegal parameter %A" argv
        1