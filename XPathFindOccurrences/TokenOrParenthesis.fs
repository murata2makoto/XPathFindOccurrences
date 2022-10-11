module XPathFindOccurrences.TokenOrParenthesis
open System.Xml.Linq

type TokenOrParenthesis<'a> =
    | Token of 'a
    | StartParenthesis
    | EndParenthesis

let private tokenSeq2tokenOrParenthesisSeq (l: seq<'a>) getLevel = 
    seq{let mutable currentLevel = 0
        for e in l do
          let nextLevel = getLevel e
          if nextLevel = currentLevel then
            yield Token(e)
          elif nextLevel = currentLevel + 1 then
            yield StartParenthesis
            yield Token(e)
          elif nextLevel < currentLevel then
            for i = 1 to currentLevel - nextLevel do
              yield EndParenthesis
            yield Token(e)
          else failwith "hen"
          currentLevel <- nextLevel;
        for j = 1 to currentLevel do yield EndParenthesis
        }

let nest (l: seq<'a>) getLevel action =
    let tpSeq = tokenSeq2tokenOrParenthesisSeq l getLevel
    let mutable stackOfStacks = []
    for tp in tpSeq do
        match tp with
        | Token(e) -> 
            let (topStack: int list) = List.head stackOfStacks
            let topStackNext = 
                if topStack.IsEmpty then [1]
                else (topStack.Head + 1)::topStack
            stackOfStacks <- topStackNext::(List.tail stackOfStacks)
            action e stackOfStacks
        | StartParenthesis -> 
            stackOfStacks <- []::stackOfStacks
        | EndParenthesis -> 
            stackOfStacks <- List.tail stackOfStacks