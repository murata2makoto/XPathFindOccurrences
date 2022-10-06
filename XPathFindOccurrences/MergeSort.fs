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
