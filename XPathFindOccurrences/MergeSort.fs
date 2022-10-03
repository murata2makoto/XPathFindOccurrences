module MergeSort

let mergesort1 (comparer : 'a -> 'a -> int)
                (l1: 'a list) (l2: 'a list) =
    let rec help toBePrepended (l1: 'a list) (l2: 'a list) =
        if l1.IsEmpty then List.append l2 toBePrepended 
        elif l2.IsEmpty then List.append l1 toBePrepended 
        else 
          let h1 = List.head l1
          let h2 = List.head l2
          match comparer h1 h2 with
          | -1  ->
            help (h1::toBePrepended) (List.tail l1) l2
          | 1 ->
            help (h2::toBePrepended) l1 (List.tail l2)
          | _ -> failwith "hen"
    help [] l1 l2
    |> List.rev
        
let mergesort (comparer : 'a -> 'a -> int)
                (l1: 'a list) (l2: 'a list) =
    let mutable t1 = l1
    let mutable t2 = l2
    let mutable merged = []
    while not (t1.IsEmpty || t2.IsEmpty) do
      let h1 = List.head t1
      let h2 = List.head t2
      match comparer h1 h2 with
      | -1 | 0 ->
        merged <- h1::merged
        t1 <- t1.Tail
      | 1 ->
        merged <- h2::merged
        t2 <- t2.Tail 
      | _ -> failwith "hen"
    if t1.IsEmpty then
      List.append (List.rev t2) merged 
      |> List.rev
    elif t2.IsEmpty then
      List.append (List.rev t1) merged 
      |> List.rev
    else failwith "hen"

let rec mergesortList (comparer : 'a -> 'a -> int)
                        (listOfLists: 'a list list) : 'a list =
  match listOfLists with 
  | [] -> []
  | [h] -> h
  | h::t -> mergesort1 comparer h (mergesortList comparer t) 
