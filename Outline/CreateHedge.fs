module CreateHedge

open TokenOrParenthesis
open TreeAndHedge

let rec nest (l:seq<TokenOrParenthesis<'a>>): 
        Tree<'a> * seq<TokenOrParenthesis<'a>> =

    let head, tail = Seq.head l, Seq.tail l
    match head with 
    | Token(e) ->
        let content, remainder =  sequence2Hedge tail
        Node (e,content), remainder
    | _ -> failwith "hen"

and sequence2Hedge (l:seq<TokenOrParenthesis<'a>>):
        Hedge<'a> * seq<TokenOrParenthesis<'a>> =

    if Seq.isEmpty l then
        Seq.empty, l
    else
        let head, tail = Seq.head l, Seq.tail l
        match head with
        | Token(e) ->
            failwith "Should not happen"
        | EndParenthesis -> 
            Seq.empty, tail
        | StartParenthesis -> 
            let subtree, remainder = nest tail
            let rest, remainder = sequence2Hedge remainder
            Seq.append (seq{subtree}) rest,
            remainder


