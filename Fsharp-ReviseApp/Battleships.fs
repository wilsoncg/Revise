namespace Fsharp_ReviseApp
open FSharp.Core
open System

module Battleships =
 let private rowToInt (c:char) =
    c.ToString() |> Int32.Parse
 
 let private ColumnLetter i =
    ['A'..'Z'] |> Seq.item i |> (fun f -> f.ToString())

 let private print sequence =
    sequence 
    |> Seq.map (fun c -> c.ToString()) 
    |> String.concat ""

// 1A 2B -> 1A 1B 2A 2B
 let ToFullShip (s:string) (n:int) = 
    let x0 = s.Chars 0
    let y0 = s.Chars 1
    let x1 = s.Chars 3
    let y1 = s.Chars 4

    let isRow = (x0 = x1) || (y0 = y1)
    if (isRow) 
    then 
        List.allPairs [x0..x1] [y0..y1] 
        |> Seq.map (fun (x,y) -> sprintf "%O%O" x y)
        |> String.concat " "
        //|> print
    else
        let row1 = rowToInt x0
        let row2 = rowToInt x1
        let r = 
         [x0; y0; ' '; x0; y1; ' '; x1; y0; ' '; x1; y1]
         |> print
        r