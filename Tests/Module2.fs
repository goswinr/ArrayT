namespace Tests

open ArrayT

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
// open System.Linq
// open System.Runtime.InteropServices
#endif

open System
open System.Collections.Generic


module Module2 =
 open Exceptions

 let tests : Test =
    testList "extensions Tests" [



        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------tests vor custom functions-------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------


        testCase "Array.rotate" <| fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            Assert.AreEqual(xs |> Array.rotate  2 , [|4; 5; 0; 1; 2; 3 |])
            Assert.AreEqual(xs |> Array.rotate  1 , [|5; 0; 1; 2; 3; 4 |])
            Assert.AreEqual(xs |> Array.rotate -2 , [|2; 3; 4; 5; 0; 1|])
            Assert.AreEqual(xs |> Array.rotate -1 , [|1; 2; 3; 4; 5; 0|])
            Assert.AreEqual(xs |> Array.rotate -6 , xs)
            Assert.AreEqual(xs |> Array.rotate  12 , xs)
            Assert.AreEqual(xs |> Array.rotate -12 , xs)
            Assert.AreEqual(xs |> Array.rotate -13 , xs|> Array.rotate -1)
            Assert.AreEqual(xs |> Array.rotate  13 , xs|> Array.rotate  1)


        testCase "Array.rotateDownTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateDownTill(fun i -> i = 7)     , [|7; 2; 3; 7; 5; 0 |])
            throwsArg (fun () -> xs |> Array.rotateDownTill (fun i -> i = 99) |> ignore  )



        testCase "Array.rotateDownTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateDownTillLast(fun i -> i = 7) , [|2; 3; 7; 5; 0; 7 |])
            throwsArg (fun () -> xs |> Array.rotateDownTillLast (fun i -> i = 99) |> ignore  )



        testCase "Array.rotateUpTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateUpTill(fun i -> i = 7)       , [|7; 5; 0; 7; 2; 3 |])
            throwsArg (fun () -> xs |> Array.rotateUpTill (fun i -> i = 99) |> ignore  )



        testCase "Array.rotateUpTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateUpTillLast(fun i -> i = 7)   , [|5; 0; 7; 2; 3; 7 |])
            throwsArg (fun () -> xs |> Array.rotateUpTillLast (fun i -> i = 99) |> ignore  )



        testCase "Array.filteri " <| fun _ ->
            let arr = [|'a';'b';'c'|]
            let result = arr|> Array.filteri (fun i -> i % 2 = 0)
            Assert.AreEqual([|'a';'c'|] , result)
            Assert.AreNotEqual(arr, result,"input array should not be modified")


        testCase "findlast" <| fun _ ->
            let i =  "abcde".ToCharArray()
            let l =  i.LastIndex
            let ab =  "ab".ToCharArray()
            let de =  "de".ToCharArray()    //

            Assert.True (
                ( 0 = Array.findArray ab 0 l i)
                && (-1 = Array.findArray ab 1 l i)
                && ( 0 = Array.findLastArray ab 0 l i)
                && (-1 = Array.findLastArray ab 1 l i)
                && (-1 = Array.findArray de 0 (l-1)  i)
                && ( 3 = Array.findArray de 0 l   i)
                && (-1 = Array.findLastArray de 0 (l-1)  i)
                && ( 3 = Array.findLastArray de 0 l   i)
            )

    ]