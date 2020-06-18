module Codility99_PolygonConcavityIndexTests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open ReviseApp

let pci = new Codility99_PolygonConcavityIndex()
let toPoint (x,y) = Codility99_PolygonConcavityIndex.Point2D(x = x, y = y)
let isValid x = if (x = -1 || x > 0) then true else false

[<Property>]
let ``Check polygon concavitiy with random input`` (input : int * int list) =
  let arbPoints =   
   Gen.choose (-1_000_000_000, 1_000_000_000) 
   |> Gen.two
   |> Gen.listOf
   |> Arb.fromGen  

  Prop.forAll arbPoints (fun arb -> 
   let (input: Codility99_PolygonConcavityIndex.Point2D[]) = 
     arb 
     |> (fun x -> List.map toPoint x |> List.toArray)
   isValid (pci.PolygonConcavitiyIndex input))

[<Fact>]
let ``Examine failing test`` =
 let input = 
  [(158051708, 722339995); (116860879, -334412935); (760020111, -370696648);
 (276164837, 389319364)]
  |> List.map toPoint
  |> List.toArray

 Assert.NotEqual(0, pci.PolygonConcavitiyIndex input)
