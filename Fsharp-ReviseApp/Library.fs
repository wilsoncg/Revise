namespace Fsharp_ReviseApp

module Library =

 let fibs = 
  (0, 1) |> 
  Seq.unfold(fun (first, second) -> Some(first, (second, first + second)))

 let rec qsort input = seq {
    match input with
    | [] -> yield! Seq.empty
    | x::xs -> 
        let smaller, larger = List.partition ((>=) x) xs
        yield! qsort smaller; yield x; yield! qsort larger
}