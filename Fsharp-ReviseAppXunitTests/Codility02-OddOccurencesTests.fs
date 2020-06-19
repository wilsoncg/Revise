module Codility02_OddOccurencesTests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open ReviseApp

let occurences = new Codility02_OddOccurences();
let isValid x = if (x = 0 || x > 0) then true else false

[<Property>]
let ``Check occurences with random input`` (size : int) =
  let arb =
   gen {
    let! x = Gen.choose (1, 1_000_000) |> Gen.listOfLength size
    let! y = Gen.choose (1, 1_000_000) |> Gen.listOfLength 1
    return List.concat [x; x; y] |> List.toArray
   } |> Arb.fromGen

  Prop.forAll arb (fun a -> 
   isValid (occurences.Occurences a)
  )
