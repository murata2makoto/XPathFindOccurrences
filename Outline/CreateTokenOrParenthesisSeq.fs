module CreateTokenOrParenthesisSeq

open TokenOrParenthesis

//トークン列に、開き括弧、閉じ括弧を挿入する。getRankで深さを判定
let  createTokenOrParenthesisSeq 
        (l: seq<'a>) (getRank: ('a -> int)) = 
    seq{let mutable currentLevel = 0
        for e in l do
          let nextLevel = getRank e
          if nextLevel = currentLevel then
            yield EndParenthesis
            yield StartParenthesis
            yield Token(e)
          elif nextLevel = currentLevel + 1 then
            yield StartParenthesis
            yield Token(e)
          elif nextLevel < currentLevel then
            for i = 0 to currentLevel - nextLevel do
              yield EndParenthesis
            yield StartParenthesis
            yield Token(e)
          else failwith "hen"
          currentLevel <- nextLevel;
        for j = 1 to currentLevel do yield EndParenthesis
        }
