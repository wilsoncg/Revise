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

 let rec qsortList (input) =
    match input with
    | [] -> []
    | x::xs -> List.partition(fun f -> f <= x) xs |> fun (ys,zs) -> qsortList(ys) @ x :: qsortList(zs)

type FizzBuzzGenerator() =
    member __.Apply (list:seq<int>) =
     let test n =
        match (n%3, n%5) with
        | 0,0 -> "FizzBuzz"
        | 0,_ -> "Fizz"
        | _,0 -> "Buzz"
        | _,_ -> string n    
     list |> Seq.map test

type IGetListLength = abstract member Invoke<'a> : 'a List -> int