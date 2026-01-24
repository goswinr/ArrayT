namespace Tests

open ArrayT

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open System
open System.Collections.Generic


module Module2 =
 open Exceptions

 let tests =
    testList "Module Tests" [

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Basic Get/Set functions---------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.get returns item at index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.get 2 xs) 3 "get at index 2"
            Expect.equal (Array.get 0 xs) 1 "get at index 0"
            Expect.equal (Array.get 4 xs) 5 "get at index 4"

        testCase "Array.get throws on negative index" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.get -1 xs |> ignore)

        testCase "Array.get throws on index out of range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.get 3 xs |> ignore)

        testCase "Array.get throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.get 0 xs |> ignore)

        testCase "Array.get does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.get 1 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.set sets item at index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.set 2 99 xs
            Expect.equal xs.[2] 99 "value should be set"

        testCase "Array.set throws on negative index" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.set -1 99 xs)

        testCase "Array.set throws on index out of range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.set 3 99 xs)

        testCase "Array.set throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.set 0 99 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Negative indexing---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.getNeg returns item at negative index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.getNeg -1 xs) 5 "getNeg -1"
            Expect.equal (Array.getNeg -2 xs) 4 "getNeg -2"
            Expect.equal (Array.getNeg -5 xs) 1 "getNeg -5"

        testCase "Array.getNeg works with positive index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.getNeg 0 xs) 1 "getNeg 0"
            Expect.equal (Array.getNeg 2 xs) 3 "getNeg 2"

        testCase "Array.getNeg throws on index out of range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.getNeg -4 xs |> ignore)
            throwsRange (fun () -> Array.getNeg 3 xs |> ignore)

        testCase "Array.getNeg throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getNeg -1 xs |> ignore)

        testCase "Array.getNeg does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.getNeg -1 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.setNeg sets item at negative index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg -1 99 xs
            Expect.equal xs.[4] 99 "setNeg -1 should set last item"

        testCase "Array.setNeg works with positive index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg 0 99 xs
            Expect.equal xs.[0] 99 "setNeg 0 should set first item"

        testCase "Array.setNeg throws on index out of range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.setNeg -4 99 xs)
            throwsRange (fun () -> Array.setNeg 3 99 xs)

        testCase "Array.setNeg throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setNeg -1 99 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Looped indexing-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.getLooped returns item with looped positive index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.equal (Array.getLooped 0 xs) 1 "getLooped 0"
            Expect.equal (Array.getLooped 3 xs) 1 "getLooped 3"
            Expect.equal (Array.getLooped 4 xs) 2 "getLooped 4"
            Expect.equal (Array.getLooped 6 xs) 1 "getLooped 6"

        testCase "Array.getLooped returns item with looped negative index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.equal (Array.getLooped -1 xs) 3 "getLooped -1"
            Expect.equal (Array.getLooped -2 xs) 2 "getLooped -2"
            Expect.equal (Array.getLooped -3 xs) 1 "getLooped -3"
            Expect.equal (Array.getLooped -4 xs) 3 "getLooped -4"

        testCase "Array.getLooped throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.getLooped 0 xs |> ignore)

        testCase "Array.getLooped throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getLooped 0 xs |> ignore)

        testCase "Array.getLooped does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.getLooped 5 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.setLooped sets item with looped index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped 3 99 xs
            Expect.equal xs.[0] 99 "setLooped 3 should set first item"

        testCase "Array.setLooped sets item with negative looped index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped -1 99 xs
            Expect.equal xs.[2] 99 "setLooped -1 should set last item"

        testCase "Array.setLooped throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.setLooped 0 99 xs)

        testCase "Array.setLooped throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setLooped 0 99 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Positional access---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.first returns first item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.first xs) 1 "first"

        testCase "Array.first throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)

        testCase "Array.first throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.first xs |> ignore)

        testCase "Array.first does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.first xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.second returns second item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.second xs) 2 "second"

        testCase "Array.second throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.second xs |> ignore)

        testCase "Array.second throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.second xs |> ignore)

        testCase "Array.third returns third item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.third xs) 3 "third"

        testCase "Array.third throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.third xs |> ignore)

        testCase "Array.third throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.third xs |> ignore)

        testCase "Array.secondLast returns second last item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.secondLast xs) 4 "secondLast"

        testCase "Array.secondLast throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.secondLast xs |> ignore)

        testCase "Array.secondLast throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.secondLast xs |> ignore)

        testCase "Array.thirdLast returns third last item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.thirdLast xs) 3 "thirdLast"

        testCase "Array.thirdLast throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.thirdLast xs |> ignore)

        testCase "Array.thirdLast throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thirdLast xs |> ignore)

        testCase "Array.firstAndOnly returns only item" <| fun _ ->
            let xs = [|42|]
            Expect.equal (Array.firstAndOnly xs) 42 "firstAndOnly"

        testCase "Array.firstAndOnly throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.firstAndOnly xs |> ignore)

        testCase "Array.firstAndOnly throws on array with more than one item" <| fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.firstAndOnly xs |> ignore)

        testCase "Array.firstAndOnly throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.firstAndOnly xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Slicing and trimming------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.slice returns slice with positive indices" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isTrue (Array.slice 1 3 xs = [|2; 3; 4|]) "slice 1 3"

        testCase "Array.slice returns slice with negative indices" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isTrue (Array.slice -2 -1 xs = [|4; 5|]) "slice -2 -1"
            Expect.isTrue (Array.slice 1 -2 xs = [|2; 3; 4|]) "slice 1 -2"

        testCase "Array.slice returns single item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isTrue (Array.slice 2 2 xs = [|3|]) "slice 2 2"

        testCase "Array.slice throws on invalid range" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            throwsRange (fun () -> Array.slice 3 1 xs |> ignore)

        testCase "Array.slice throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.slice 0 1 xs |> ignore)

        testCase "Array.slice does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.slice 1 3 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.trim trims from start and end" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isTrue (Array.trim 1 1 xs = [|2; 3; 4|]) "trim 1 1"

        testCase "Array.trim returns empty array when trimming more than length" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isTrue (Array.trim 2 2 xs = [||]) "trim 2 2"

        testCase "Array.trim returns same elements when trimming 0" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isTrue (Array.trim 0 0 xs = [|1; 2; 3|]) "trim 0 0"

        testCase "Array.trim throws on negative trim values" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.trim -1 0 xs |> ignore)
            throwsArg (fun () -> Array.trim 0 -1 xs |> ignore)

        testCase "Array.trim throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.trim 1 1 xs |> ignore)

        testCase "Array.trim does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.trim 1 1 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Windowing functions (non-looped)------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.windowed2 returns pairs" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.windowed2 xs |> Seq.toArray
            Expect.isTrue (result = [|(1,2); (2,3); (3,4)|]) "windowed2"

        testCase "Array.windowed2 throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.windowed2 xs |> ignore)

        testCase "Array.windowed2 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.windowed2 xs |> ignore)

        testCase "Array.windowed2 does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.windowed2 xs |> Seq.toArray
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.windowed3 returns triplets" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let result = Array.windowed3 xs |> Seq.toArray
            Expect.isTrue (result = [|(1,2,3); (2,3,4); (3,4,5)|]) "windowed3"

        testCase "Array.windowed3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.windowed3 xs |> ignore)

        testCase "Array.windowed3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.windowed3 xs |> ignore)

        testCase "Array.windowed2i returns pairs with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.windowed2i xs |> Seq.toArray
            Expect.isTrue (result = [|(0,'a','b'); (1,'b','c'); (2,'c','d')|]) "windowed2i"

        testCase "Array.windowed2i throws on array with less than 2 items" <| fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.windowed2i xs |> ignore)

        testCase "Array.windowed2i throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.windowed2i xs |> ignore)

        testCase "Array.windowed3i returns triplets with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'; 'e'|]
            let result = Array.windowed3i xs |> Seq.toArray
            Expect.isTrue (result = [|(1,'a','b','c'); (2,'b','c','d'); (3,'c','d','e')|]) "windowed3i"

        testCase "Array.windowed3i throws on array with less than 3 items" <| fun _ ->
            let xs = [|'a'; 'b'|]
            throwsArg (fun () -> Array.windowed3i xs |> ignore)

        testCase "Array.windowed3i throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.windowed3i xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Windowing functions (looped)----------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.thisNext returns looped pairs" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.thisNext xs |> Seq.toArray
            Expect.isTrue (result = [|(1,2); (2,3); (3,1)|]) "thisNext"

        testCase "Array.thisNext throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.thisNext xs |> ignore)

        testCase "Array.thisNext throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thisNext xs |> ignore)

        testCase "Array.prevThis returns looped pairs starting with last-first" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.prevThis xs |> Seq.toArray
            Expect.isTrue (result = [|(3,1); (1,2); (2,3)|]) "prevThis"

        testCase "Array.prevThis throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.prevThis xs |> ignore)

        testCase "Array.prevThis throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThis xs |> ignore)

        testCase "Array.prevThisNext returns looped triplets" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.prevThisNext xs |> Seq.toArray
            Expect.isTrue (result = [|(4,1,2); (1,2,3); (2,3,4); (3,4,1)|]) "prevThisNext"

        testCase "Array.prevThisNext throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.prevThisNext xs |> ignore)

        testCase "Array.prevThisNext throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThisNext xs |> ignore)

        testCase "Array.iThisNext returns looped pairs with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'|]
            let result = Array.iThisNext xs |> Seq.toArray
            Expect.isTrue (result = [|(0,'a','b'); (1,'b','c'); (2,'c','a')|]) "iThisNext"

        testCase "Array.iThisNext throws on array with less than 2 items" <| fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.iThisNext xs |> ignore)

        testCase "Array.iThisNext throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.iThisNext xs |> ignore)

        testCase "Array.iPrevThisNext returns looped triplets with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.iPrevThisNext xs |> Seq.toArray
            Expect.isTrue (result = [|(0,'d','a','b'); (1,'a','b','c'); (2,'b','c','d'); (3,'c','d','a')|]) "iPrevThisNext"

        testCase "Array.iPrevThisNext throws on array with less than 3 items" <| fun _ ->
            let xs = [|'a'; 'b'|]
            throwsArg (fun () -> Array.iPrevThisNext xs |> ignore)

        testCase "Array.iPrevThisNext throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.iPrevThisNext xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Singleton creation--------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.singleton creates single item array" <| fun _ ->
            let result = Array.singleton 42
            Expect.isTrue (result = [|42|]) "singleton"

        testCase "Array.singleton allows null values" <| fun _ ->
            let result = Array.singleton (null: string)
            Expect.equal result.Length 1 "singleton null"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Rotation tests------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.rotate" <| fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            Expect.isTrue (xs |> Array.rotate  2 = [|4; 5; 0; 1; 2; 3 |]) "rotate 2"
            Expect.isTrue (xs |> Array.rotate  1 = [|5; 0; 1; 2; 3; 4 |]) "rotate 1"
            Expect.isTrue (xs |> Array.rotate -2 = [|2; 3; 4; 5; 0; 1|]) "rotate -2"
            Expect.isTrue (xs |> Array.rotate -1 = [|1; 2; 3; 4; 5; 0|]) "rotate -1"
            Expect.isTrue (xs |> Array.rotate -6 = xs) "rotate -6"
            Expect.isTrue (xs |> Array.rotate  12 = xs) "rotate 12"
            Expect.isTrue (xs |> Array.rotate -12 = xs) "rotate -12"
            Expect.isTrue (xs |> Array.rotate -13 = (xs|> Array.rotate -1)) "rotate -13"
            Expect.isTrue (xs |> Array.rotate  13 = (xs|> Array.rotate  1)) "rotate 13"

        testCase "Array.rotate throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.rotate 1 xs |> ignore)

        testCase "Array.rotate does not modify input array" <| fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotate 2 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.rotateDownTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Expect.isTrue (xs |> Array.rotateDownTill(fun i -> i = 7) = [|7; 2; 3; 7; 5; 0 |]) "rotateDownTill"
            throwsArg (fun () -> xs |> Array.rotateDownTill (fun i -> i = 99) |> ignore)

        testCase "Array.rotateDownTill does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTill (fun i -> i = 7) xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.rotateDownTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Expect.isTrue (xs |> Array.rotateDownTillLast(fun i -> i = 7) = [|2; 3; 7; 5; 0; 7 |]) "rotateDownTillLast"
            throwsArg (fun () -> xs |> Array.rotateDownTillLast (fun i -> i = 99) |> ignore)

        testCase "Array.rotateDownTillLast does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTillLast (fun i -> i = 7) xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.rotateUpTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Expect.isTrue (xs |> Array.rotateUpTill(fun i -> i = 7) = [|7; 5; 0; 7; 2; 3 |]) "rotateUpTill"
            throwsArg (fun () -> xs |> Array.rotateUpTill (fun i -> i = 99) |> ignore)

        testCase "Array.rotateUpTill does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTill (fun i -> i = 7) xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.rotateUpTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Expect.isTrue (xs |> Array.rotateUpTillLast(fun i -> i = 7) = [|5; 0; 7; 2; 3; 7 |]) "rotateUpTillLast"
            throwsArg (fun () -> xs |> Array.rotateUpTillLast (fun i -> i = 99) |> ignore)

        testCase "Array.rotateUpTillLast does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTillLast (fun i -> i = 7) xs
            Expect.isTrue (xs = original) "input array should not be modified"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Status check functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.isSingleton returns true for single item array" <| fun _ ->
            let xs = [|42|]
            Expect.isTrue (Array.isSingleton xs) "isSingleton"

        testCase "Array.isSingleton returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.isFalse (Array.isSingleton xs) "isSingleton empty"

        testCase "Array.isSingleton returns false for array with multiple items" <| fun _ ->
            let xs = [|1; 2|]
            Expect.isFalse (Array.isSingleton xs) "isSingleton multiple"

        testCase "Array.isSingleton throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isSingleton xs |> ignore)

        testCase "Array.hasOne returns true for single item array" <| fun _ ->
            let xs = [|42|]
            Expect.isTrue (Array.hasOne xs) "hasOne"

        testCase "Array.hasOne returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.isFalse (Array.hasOne xs) "hasOne empty"

        testCase "Array.hasOne throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasOne xs |> ignore)

        testCase "Array.isNotEmpty returns true for non-empty array" <| fun _ ->
            let xs = [|1|]
            Expect.isTrue (Array.isNotEmpty xs) "isNotEmpty"

        testCase "Array.isNotEmpty returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.isFalse (Array.isNotEmpty xs) "isNotEmpty empty"

        testCase "Array.isNotEmpty throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isNotEmpty xs |> ignore)

        testCase "Array.hasItems returns true when count matches" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isTrue (Array.hasItems 3 xs) "hasItems 3"

        testCase "Array.hasItems returns false when count does not match" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isFalse (Array.hasItems 2 xs) "hasItems 2"

        testCase "Array.hasItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasItems 1 xs |> ignore)

        testCase "Array.hasMinimumItems returns true when count is sufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isTrue (Array.hasMinimumItems 2 xs) "hasMinimumItems 2"
            Expect.isTrue (Array.hasMinimumItems 3 xs) "hasMinimumItems 3"

        testCase "Array.hasMinimumItems returns false when count is insufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isFalse (Array.hasMinimumItems 4 xs) "hasMinimumItems 4"

        testCase "Array.hasMinimumItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMinimumItems 1 xs |> ignore)

        testCase "Array.hasMaximumItems returns true when count is not exceeded" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isTrue (Array.hasMaximumItems 3 xs) "hasMaximumItems 3"
            Expect.isTrue (Array.hasMaximumItems 4 xs) "hasMaximumItems 4"

        testCase "Array.hasMaximumItems returns false when count is exceeded" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isFalse (Array.hasMaximumItems 2 xs) "hasMaximumItems 2"

        testCase "Array.hasMaximumItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMaximumItems 1 xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Min/Max functions---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.min2 returns two smallest elements" <| fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b = Array.min2 xs
            Expect.equal a 1 "min2 first"
            Expect.equal b 2 "min2 second"

        testCase "Array.min2 keeps order for equal elements" <| fun _ ->
            let xs = [|3; 1; 1; 2|]
            let a, b = Array.min2 xs
            Expect.equal a 1 "min2 first equal"
            Expect.equal b 1 "min2 second equal"

        testCase "Array.min2 throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.min2 xs |> ignore)

        testCase "Array.min2 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.min2 xs |> ignore)

        testCase "Array.min2 does not modify input array" <| fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.min2 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.max2 returns two largest elements" <| fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b = Array.max2 xs
            Expect.equal a 5 "max2 first"
            Expect.equal b 4 "max2 second"

        testCase "Array.max2 keeps order for equal elements" <| fun _ ->
            let xs = [|3; 5; 5; 2|]
            let a, b = Array.max2 xs
            Expect.equal a 5 "max2 first equal"
            Expect.equal b 5 "max2 second equal"

        testCase "Array.max2 throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.max2 xs |> ignore)

        testCase "Array.max2 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max2 xs |> ignore)

        testCase "Array.min2By returns two smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let a, b = Array.min2By String.length xs
            Expect.equal a "be" "min2By first"
            Expect.equal b "do" "min2By second"

        testCase "Array.min2By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2By String.length xs |> ignore)

        testCase "Array.max2By returns two largest by projection" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let a, b = Array.max2By String.length xs
            Expect.equal a "elephant" "max2By first"
            Expect.equal b "apple" "max2By second"

        testCase "Array.max2By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2By String.length xs |> ignore)

        testCase "Array.min2IndicesBy returns indices of two smallest" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let i1, i2 = Array.min2IndicesBy String.length xs
            Expect.equal i1 1 "min2IndicesBy first"
            Expect.equal i2 3 "min2IndicesBy second"

        testCase "Array.min2IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2IndicesBy String.length xs |> ignore)

        testCase "Array.max2IndicesBy returns indices of two largest" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let i1, i2 = Array.max2IndicesBy String.length xs
            Expect.equal i1 3 "max2IndicesBy first"
            Expect.equal i2 1 "max2IndicesBy second"

        testCase "Array.max2IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2IndicesBy String.length xs |> ignore)

        testCase "Array.min3 returns three smallest elements" <| fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b, c = Array.min3 xs
            Expect.equal a 1 "min3 first"
            Expect.equal b 2 "min3 second"
            Expect.equal c 3 "min3 third"

        testCase "Array.min3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.min3 xs |> ignore)

        testCase "Array.min3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.min3 xs |> ignore)

        testCase "Array.max3 returns three largest elements" <| fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b, c = Array.max3 xs
            Expect.equal a 5 "max3 first"
            Expect.equal b 4 "max3 second"
            Expect.equal c 3 "max3 third"

        testCase "Array.max3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.max3 xs |> ignore)

        testCase "Array.max3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max3 xs |> ignore)

        testCase "Array.min3By returns three smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "elephant"|]
            let a, b, c = Array.min3By String.length xs
            Expect.equal a "be" "min3By first"
            Expect.equal b "do" "min3By second"
            Expect.equal c "cat" "min3By third"

        testCase "Array.min3By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3By String.length xs |> ignore)

        testCase "Array.max3By returns three largest by projection" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let a, b, c = Array.max3By String.length xs
            Expect.equal a "elephant" "max3By first"
            Expect.equal b "apple" "max3By second"
            Expect.equal c "cat" "max3By third"

        testCase "Array.max3By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3By String.length xs |> ignore)

        testCase "Array.min3IndicesBy returns indices of three smallest" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "e"|]
            let i1, i2, i3 = Array.min3IndicesBy String.length xs
            Expect.equal i1 1 "min3IndicesBy first"
            Expect.equal i2 3 "min3IndicesBy second"
            Expect.equal i3 4 "min3IndicesBy third"

        testCase "Array.min3IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3IndicesBy String.length xs |> ignore)

        testCase "Array.max3IndicesBy returns indices of three largest" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let i1, i2, i3 = Array.max3IndicesBy String.length xs
            Expect.equal i1 3 "max3IndicesBy first"
            Expect.equal i2 1 "max3IndicesBy second"
            Expect.equal i3 4 "max3IndicesBy third"

        testCase "Array.max3IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3IndicesBy String.length xs |> ignore)

        testCase "Array.minIndexBy returns index of smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let i = Array.minIndexBy String.length xs
            Expect.equal i 1 "minIndexBy"

        testCase "Array.minIndexBy throws on empty array" <| fun _ ->
            let xs : string[] = [||]
            throwsArg (fun () -> Array.minIndexBy String.length xs |> ignore)

        testCase "Array.minIndexBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.minIndexBy String.length xs |> ignore)

        testCase "Array.maxIndexBy returns index of largest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "elephant"|]
            let i = Array.maxIndexBy String.length xs
            Expect.equal i 2 "maxIndexBy"

        testCase "Array.maxIndexBy throws on empty array" <| fun _ ->
            let xs : string[] = [||]
            throwsArg (fun () -> Array.maxIndexBy String.length xs |> ignore)

        testCase "Array.maxIndexBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.maxIndexBy String.length xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Swap function-------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.swap swaps two elements" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.swap 1 3 xs
            Expect.isTrue (xs = [|1; 4; 3; 2; 5|]) "swap 1 3"

        testCase "Array.swap with same index does nothing" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.swap 1 1 xs
            Expect.isTrue (xs = [|1; 2; 3|]) "swap same index"

        testCase "Array.swap throws on negative index" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.swap -1 1 xs)
            throwsArg (fun () -> Array.swap 1 -1 xs)

        testCase "Array.swap throws on index out of range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.swap 0 3 xs)
            throwsArg (fun () -> Array.swap 3 0 xs)

        testCase "Array.swap throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.swap 0 1 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Count functions-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.count returns length" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.count xs) 5 "count"

        testCase "Array.count returns 0 for empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.equal (Array.count xs) 0 "count empty"

        testCase "Array.count throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.count xs |> ignore)

        testCase "Array.countIf counts matching items" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let count = Array.countIf (fun x -> x > 2) xs
            Expect.equal count 3 "countIf"

        testCase "Array.countIf returns 0 when no items match" <| fun _ ->
            let xs = [|1; 2; 3|]
            let count = Array.countIf (fun x -> x > 10) xs
            Expect.equal count 0 "countIf none match"

        testCase "Array.countIf returns 0 for empty array" <| fun _ ->
            let xs : int[] = [||]
            let count = Array.countIf (fun _ -> true) xs
            Expect.equal count 0 "countIf empty"

        testCase "Array.countIf throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.countIf (fun _ -> true) xs |> ignore)

        testCase "Array.countIf does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.countIf (fun x -> x > 1) xs
            Expect.isTrue (xs = original) "input array should not be modified"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Filter by index-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.filteri " <| fun _ ->
            let arr = [|'a';'b';'c'|]
            let result = arr|> Array.filteri (fun i -> i % 2 = 0)
            Expect.isTrue (result = [|'a';'c'|]) "filteri"
            Expect.isFalse (Object.ReferenceEquals(arr, result)) "filteri should return new array"

        testCase "Array.filteri returns empty for all false predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> false) xs
            Expect.isTrue (result = [||]) "filteri all false"

        testCase "Array.filteri returns all for all true predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> true) xs
            Expect.isTrue (result = [|1; 2; 3|]) "filteri all true"

        testCase "Array.filteri throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.filteri (fun _ -> true) xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Collection conversion-----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.ofResizeArray creates array from ResizeArray" <| fun _ ->
            let ra = ResizeArray<int>([1; 2; 3])
            let result = Array.ofResizeArray ra
            Expect.isTrue (result = [|1; 2; 3|]) "ofResizeArray"

        testCase "Array.ofResizeArray throws on null" <| fun _ ->
            let ra : ResizeArray<int> = null
            throwsNull (fun () -> Array.ofResizeArray ra |> ignore)

        testCase "Array.toResizeArray creates ResizeArray from array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.toResizeArray xs
            Expect.equal result.Count 3 "toResizeArray count"
            Expect.equal result.[0] 1 "toResizeArray [0]"
            Expect.equal result.[1] 2 "toResizeArray [1]"
            Expect.equal result.[2] 3 "toResizeArray [2]"

        testCase "Array.toResizeArray throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.toResizeArray xs |> ignore)

        testCase "Array.toResizeArray does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.toResizeArray xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.asResizeArray creates ResizeArray from array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 3 "asResizeArray count"

        testCase "Array.asResizeArray throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.asResizeArray xs |> ignore)

        testCase "Array.asArray creates array from ResizeArray" <| fun _ ->
            let ra = ResizeArray<int>([1; 2; 3])
            let result = Array.asArray ra
            Expect.isTrue (result = [|1; 2; 3|]) "asArray"

        testCase "Array.asArray throws on null" <| fun _ ->
            let ra : ResizeArray<int> = null
            throwsNull (fun () -> Array.asArray ra |> ignore)

        testCase "Array.ofIList creates array from IList" <| fun _ ->
            let list : IList<int> = [1; 2; 3] :> IList<int>
            let result = Array.ofIList list
            Expect.isTrue (result = [|1; 2; 3|]) "ofIList"

        testCase "Array.ofIList throws on null" <| fun _ ->
            let list : IList<int> = null
            throwsNull (fun () -> Array.ofIList list |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Conditional transformation------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.mapIfResult applies transform when result meets predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            Expect.isTrue (result = [|2; 3; 4|]) "mapIfResult applied"

        testCase "Array.mapIfResult returns original when result does not meet predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "mapIfResult not applied"

        testCase "Array.mapIfResult throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfResult (fun _ -> true) id xs |> ignore)

        testCase "Array.mapIfInputAndResult applies transform when both predicates pass" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 0) (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            Expect.isTrue (result = [|2; 3; 4|]) "mapIfInputAndResult applied"

        testCase "Array.mapIfInputAndResult returns original when input predicate fails" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 10) (fun _ -> true) (Array.map ((+) 1)) xs
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "mapIfInputAndResult input failed"

        testCase "Array.mapIfInputAndResult returns original when result predicate fails" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun _ -> true) (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "mapIfInputAndResult result failed"

        testCase "Array.mapIfInputAndResult throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfInputAndResult (fun _ -> true) (fun _ -> true) id xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Duplicates functions------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.duplicates returns duplicate elements" <| fun _ ->
            let xs = [|1; 2; 3; 2; 4; 3; 2|]
            let result = Array.duplicates xs
            Expect.isTrue (result = [|2; 3|]) "duplicates"

        testCase "Array.duplicates returns empty when no duplicates" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.duplicates xs
            Expect.isTrue (result = [||]) "duplicates none"

        testCase "Array.duplicates returns empty for empty array" <| fun _ ->
            let xs : int[] = [||]
            let result = Array.duplicates xs
            Expect.isTrue (result = [||]) "duplicates empty"

        testCase "Array.duplicates throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.duplicates xs |> ignore)

        testCase "Array.duplicates does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3; 2; 4|]
            let original = xs.Duplicate()
            let _ = Array.duplicates xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.duplicatesBy returns duplicates by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "Art"; "big"|]
            let result = Array.duplicatesBy (fun (s:string) -> Char.ToLower(s.[0])) xs
            Expect.isTrue (result = [|"Art"; "big"|]) "duplicatesBy"

        testCase "Array.duplicatesBy returns empty when no duplicates" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let result = Array.duplicatesBy (fun (s:string) -> s.[0]) xs
            Expect.isTrue (result = [||]) "duplicatesBy none"

        testCase "Array.duplicatesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.duplicatesBy String.length xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Error handling functions--------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.failIfEmpty returns array when not empty" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfEmpty "should not throw" xs
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "failIfEmpty returns same array"

        testCase "Array.failIfEmpty throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.throws (fun () -> Array.failIfEmpty "is empty" xs |> ignore) "should throw on empty"

        testCase "Array.failIfLessThan returns array when count is sufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfLessThan 3 "should not throw" xs
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "failIfLessThan returns same array"

        testCase "Array.failIfLessThan throws when count is insufficient" <| fun _ ->
            let xs = [|1; 2|]
            Expect.throws (fun () -> Array.failIfLessThan 3 "too few" xs |> ignore) "should throw when too few"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Search/Match functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.matches returns true when array matches at index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isTrue (Array.matches [|2; 3|] 1 xs) "matches at index 1"

        testCase "Array.matches returns false when array does not match" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.isFalse (Array.matches [|2; 4|] 1 xs) "does not match"

        testCase "Array.matches returns false when not enough items remain" <| fun _ ->
            let xs = [|1; 2; 3|]
            Expect.isFalse (Array.matches [|3; 4|] 2 xs) "not enough items"

        testCase "Array.matches throws on invalid index" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.matches [|1|] -1 xs |> ignore)
            throwsArg (fun () -> Array.matches [|1|] 3 xs |> ignore)

        testCase "Array.matches throws on null searchFor" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsNull (fun () -> Array.matches null 0 xs |> ignore)

        testCase "Array.matches does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.matches [|2; 3|] 1 xs
            Expect.isTrue (xs = original) "input array should not be modified"

        testCase "Array.findValue finds value in range" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.findValue 3 0 4 xs) 2 "findValue"

        testCase "Array.findValue returns -1 when not found" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.findValue 6 0 4 xs) -1 "findValue not found"

        testCase "Array.findValue respects range bounds" <| fun _ ->
            let xs = [|1; 2; 3; 2; 5|]
            Expect.equal (Array.findValue 2 2 2 xs) -1 "findValue not in range"
            Expect.equal (Array.findValue 2 2 4 xs) 3 "findValue in range"

        testCase "Array.findValue throws on invalid range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findValue 1 -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findValue 1 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findValue 1 2 1 xs |> ignore)

        testCase "Array.findValue throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findValue 1 0 0 xs |> ignore)

        testCase "Array.findLastValue finds last value in range" <| fun _ ->
            let xs = [|1; 2; 3; 2; 5|]
            Expect.equal (Array.findLastValue 2 0 4 xs) 3 "findLastValue"

        testCase "Array.findLastValue returns -1 when not found" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Expect.equal (Array.findLastValue 6 0 4 xs) -1 "findLastValue not found"

        testCase "Array.findLastValue throws on invalid range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findLastValue 1 -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findLastValue 1 0 3 xs |> ignore)

        testCase "Array.findLastValue throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findLastValue 1 0 0 xs |> ignore)

        testCase "findlast" <| fun _ ->
            let i =  "abcde".ToCharArray()
            let l =  i.LastIndex
            let ab =  "ab".ToCharArray()
            let de =  "de".ToCharArray()

            Expect.isTrue (
                ( 0 = Array.findArray ab 0 l i)
                && (-1 = Array.findArray ab 1 l i)
                && ( 0 = Array.findLastArray ab 0 l i)
                && (-1 = Array.findLastArray ab 1 l i)
                && (-1 = Array.findArray de 0 (l-1)  i)
                && ( 3 = Array.findArray de 0 l   i)
                && (-1 = Array.findLastArray de 0 (l-1)  i)
                && ( 3 = Array.findLastArray de 0 l   i)
            ) "findArray and findLastArray"

        testCase "Array.findArray throws on invalid range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findArray [|1|] -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findArray [|1|] 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findArray [|1|] 2 1 xs |> ignore)

        testCase "Array.findArray throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findArray [|1|] 0 0 xs |> ignore)

        testCase "Array.findLastArray throws on invalid range" <| fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findLastArray [|1|] -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findLastArray [|1|] 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findLastArray [|1|] 2 1 xs |> ignore)

        testCase "Array.findLastArray throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findLastArray [|1|] 0 0 xs |> ignore)

    ]
