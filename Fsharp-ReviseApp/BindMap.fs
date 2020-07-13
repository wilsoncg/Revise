module BindMap

let div y x = 
 try 
  Ok(x / y)
 with ex -> Error ex

let mul x y =
 x * y 

let apply f x =
 f |> Result.bind (fun a -> 
  let map = Result.bind (a >> Ok)
  map x)

let (<*>) = apply
let (<!>) = Result.map

let lift f x =
 f <!> x
let lift2 f x y =
 f <!> x <*> y

let unwrap r = 
 match r with
 | Ok a -> a
 | Error e -> 0

let calc = 
 let r = mul 10 10 |> div 2 // 10*10 / 2
 let r1 = 10 |> div 2 |> lift2 mul (Ok 10) // 10/2 * (Ok 10)
 let r2 = Ok 10 |> Result.bind (div 2) |> lift2 mul (Ok 10)
 
 let liftMul a b = lift2 mul a b
 let r3 = Ok 10 |> liftMul (Ok 10)
 let r4 = Ok 10 |> liftMul (Ok 10) |> Result.bind (div 2)
 let r5 = Ok 10 |> Result.bind (div 2) |> liftMul (Ok 10)

 [ r; r1; r2; r3; r4; r5 ]
 |> List.map unwrap
 |> List.iter (printfn "%i")

open FSharpPlus
module BindMapPlus =
 let mul x y = x * y
 let tryDiv x y = 
  try Ok(x/y)
  with ex -> Error ex

 //let res = lift mul