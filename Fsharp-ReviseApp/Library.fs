namespace Fsharp_ReviseApp

module Library =

 let fibs = 
  (0, 1) |> 
  Seq.unfold(fun (first, second) -> Some(first, (second, first + second)))

 let rec qsort (input) =
    match input with
    | [] -> []
    | x::xs -> List.partition(fun f -> f <= x) xs |> fun (ys,zs) -> qsort(ys) @ x :: qsort(zs)