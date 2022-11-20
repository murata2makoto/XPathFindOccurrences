module XPathFindOccurrences.TokenOrParenthesis
open System.Xml.Linq

//Parenthesis grammarを用いて、フラットな列から階層構造を作り出す

type TokenOrParenthesis<'a> =
    | Token of 'a
    | StartParenthesis
    | EndParenthesis

//トークン列に、開き括弧、閉じ括弧を挿入する。getLevelで深さを判定
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

//actionは、各項目とその番号列（1.3.2を表す[2;3;1])を受け取る関数引数
let nest (l: seq<'a>) getLevel action =
    let tpSeq = tokenSeq2tokenOrParenthesisSeq l getLevel
    let mutable indexStack = []
    for tp in tpSeq do
        match tp with
        | Token(e) -> 
            let stackTop = List.head indexStack
            indexStack <- (stackTop + 1)::(List.tail indexStack)
            action e indexStack
        | StartParenthesis -> 
            indexStack <- 0::indexStack
        | EndParenthesis -> 
            indexStack <- List.tail indexStack