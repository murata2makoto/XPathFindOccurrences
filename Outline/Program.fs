// For more information see https://aka.ms/fsharp-console-apps

open TokenOrParenthesis
open CreateTokenOrParenthesisSeq
open TreeAndHedge
open CreateHedge
      
[<EntryPoint>]
let main argv =
    let test seq1 = 
        seq1 |> Seq.iter (fun x -> printf "%A " x)
        System.Console.WriteLine()
        let tpSeq = createTokenOrParenthesisSeq seq1 (fun x -> x)
        for e in tpSeq do
            match e with 
            | StartParenthesis -> printf "("
            | EndParenthesis -> printf ")"
            | Token(e) -> printf "%A" e
        System.Console.WriteLine()
        let hedge, _ = sequence2Hedge tpSeq
        for tr in hedge do
            printfn "%A" tr
        System.Console.WriteLine()
    seq {1;2;3;2;1;2} |> test 
    seq {1} |> test 
    seq {1;1} |> test 
    seq {1;1;1} |> test 
    seq {1;2} |> test 
    seq {1;2;2} |> test 
    seq {1;2;1} |> test 
    seq {1;2;3;4;1} |> test
    seq {1;2;3;4;2} |> test  
    seq {1;2;2;1} |> test 
    seq {1;2;2;1;2} |> test 
    seq {1;2;3;1;2} |> test 
    seq {1;2;3;4;3;4;3;4;3;4;1;2} |> test 
    seq {1;2;3;4;2;3;4;2;3;1} |> test 
    1