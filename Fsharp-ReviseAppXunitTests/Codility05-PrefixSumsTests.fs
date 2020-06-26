module Codility05_PrefixSumsTests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open ReviseApp

let sums = new Codility05_PrefixSums();
let isValid x = if (x = 0 || x > 0) then true else false

[<Property>]
let ``Check GenomicRange with random input`` (size : int) =
  let dna = "ACGT";
  let input =
   gen {
    let! sequence = 
     Gen.choose (0,3)
     |> Gen.map (fun x -> dna.Chars x) 
     |> Gen.listOfLength size   
     |> Gen.map (List.map string >> String.concat "")
    let! qs = 
     Gen.choose (0, sequence.Length - 1)
     |> Gen.arrayOfLength size
    let ps = 
     Array.map (fun q -> //q / 2
      if q > 2 then q / 2 else q
      ) qs

    return (sequence, ps, qs) 
   } |> Arb.fromGen

  let arb = input
  Prop.forAll arb (fun a -> 
   (sums.GenomicRange_PreCompute a) = (sums.GenomicRange_Naive a)
  )