module TokenOrParenthesis

type TokenOrParenthesis<'a> =
    | Token of 'a
    | StartParenthesis
    | EndParenthesis

//括弧の種類を増やすことは考えられる。