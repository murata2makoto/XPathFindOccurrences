module TreeAndHedge

type Tree<'a> = Node of 'a * Hedge<'a>
and Hedge<'a> = Tree<'a> seq;;

//木の列のほうがプログラミングでは便利なことが多いし、
//数学的にも扱いやすい。なお、二分木のように子の数が
//決まっている場合ではなく、子の数が不定な場合の話である。