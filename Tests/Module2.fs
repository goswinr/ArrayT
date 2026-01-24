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
            Assert.AreEqual(3, Array.get 2 xs)
            Assert.AreEqual(1, Array.get 0 xs)
            Assert.AreEqual(5, Array.get 4 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.set sets item at index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.set 2 99 xs
            Assert.AreEqual(99, xs.[2])

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
            Assert.AreEqual(5, Array.getNeg -1 xs)
            Assert.AreEqual(4, Array.getNeg -2 xs)
            Assert.AreEqual(1, Array.getNeg -5 xs)

        testCase "Array.getNeg works with positive index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(1, Array.getNeg 0 xs)
            Assert.AreEqual(3, Array.getNeg 2 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.setNeg sets item at negative index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg -1 99 xs
            Assert.AreEqual(99, xs.[4])

        testCase "Array.setNeg works with positive index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg 0 99 xs
            Assert.AreEqual(99, xs.[0])

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
            Assert.AreEqual(1, Array.getLooped 0 xs)
            Assert.AreEqual(1, Array.getLooped 3 xs)
            Assert.AreEqual(2, Array.getLooped 4 xs)
            Assert.AreEqual(1, Array.getLooped 6 xs)

        testCase "Array.getLooped returns item with looped negative index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.AreEqual(3, Array.getLooped -1 xs)
            Assert.AreEqual(2, Array.getLooped -2 xs)
            Assert.AreEqual(1, Array.getLooped -3 xs)
            Assert.AreEqual(3, Array.getLooped -4 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.setLooped sets item with looped index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped 3 99 xs
            Assert.AreEqual(99, xs.[0])

        testCase "Array.setLooped sets item with negative looped index" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped -1 99 xs
            Assert.AreEqual(99, xs.[2])

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
            Assert.AreEqual(1, Array.first xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.second returns second item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(2, Array.second xs)

        testCase "Array.second throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.second xs |> ignore)

        testCase "Array.second throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.second xs |> ignore)

        testCase "Array.third returns third item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(3, Array.third xs)

        testCase "Array.third throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.third xs |> ignore)

        testCase "Array.third throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.third xs |> ignore)

        testCase "Array.secondLast returns second last item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(4, Array.secondLast xs)

        testCase "Array.secondLast throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.secondLast xs |> ignore)

        testCase "Array.secondLast throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.secondLast xs |> ignore)

        testCase "Array.thirdLast returns third last item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(3, Array.thirdLast xs)

        testCase "Array.thirdLast throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.thirdLast xs |> ignore)

        testCase "Array.thirdLast throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thirdLast xs |> ignore)

        testCase "Array.firstAndOnly returns only item" <| fun _ ->
            let xs = [|42|]
            Assert.AreEqual(42, Array.firstAndOnly xs)

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
            Assert.AreEqual([|2; 3; 4|], Array.slice 1 3 xs)

        testCase "Array.slice returns slice with negative indices" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual([|4; 5|], Array.slice -2 -1 xs)
            Assert.AreEqual([|2; 3; 4|], Array.slice 1 -2 xs)

        testCase "Array.slice returns single item" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual([|3|], Array.slice 2 2 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.trim trims from start and end" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual([|2; 3; 4|], Array.trim 1 1 xs)

        testCase "Array.trim returns empty array when trimming more than length" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.AreEqual([||], Array.trim 2 2 xs)

        testCase "Array.trim returns same elements when trimming 0" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.AreEqual([|1; 2; 3|], Array.trim 0 0 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Windowing functions (non-looped)------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.windowed2 returns pairs" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.windowed2 xs |> Seq.toArray
            Assert.AreEqual([|(1,2); (2,3); (3,4)|], result)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.windowed3 returns triplets" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let result = Array.windowed3 xs |> Seq.toArray
            Assert.AreEqual([|(1,2,3); (2,3,4); (3,4,5)|], result)

        testCase "Array.windowed3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.windowed3 xs |> ignore)

        testCase "Array.windowed3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.windowed3 xs |> ignore)

        testCase "Array.windowed2i returns pairs with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.windowed2i xs |> Seq.toArray
            Assert.AreEqual([|(0,'a','b'); (1,'b','c'); (2,'c','d')|], result)

        testCase "Array.windowed2i throws on array with less than 2 items" <| fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.windowed2i xs |> ignore)

        testCase "Array.windowed2i throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.windowed2i xs |> ignore)

        testCase "Array.windowed3i returns triplets with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'; 'e'|]
            let result = Array.windowed3i xs |> Seq.toArray
            Assert.AreEqual([|(1,'a','b','c'); (2,'b','c','d'); (3,'c','d','e')|], result)

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
            Assert.AreEqual([|(1,2); (2,3); (3,1)|], result)

        testCase "Array.thisNext throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.thisNext xs |> ignore)

        testCase "Array.thisNext throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thisNext xs |> ignore)

        testCase "Array.prevThis returns looped pairs starting with last-first" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.prevThis xs |> Seq.toArray
            Assert.AreEqual([|(3,1); (1,2); (2,3)|], result)

        testCase "Array.prevThis throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.prevThis xs |> ignore)

        testCase "Array.prevThis throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThis xs |> ignore)

        testCase "Array.prevThisNext returns looped triplets" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.prevThisNext xs |> Seq.toArray
            Assert.AreEqual([|(4,1,2); (1,2,3); (2,3,4); (3,4,1)|], result)

        testCase "Array.prevThisNext throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.prevThisNext xs |> ignore)

        testCase "Array.prevThisNext throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThisNext xs |> ignore)

        testCase "Array.iThisNext returns looped pairs with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'|]
            let result = Array.iThisNext xs |> Seq.toArray
            Assert.AreEqual([|(0,'a','b'); (1,'b','c'); (2,'c','a')|], result)

        testCase "Array.iThisNext throws on array with less than 2 items" <| fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.iThisNext xs |> ignore)

        testCase "Array.iThisNext throws on null array" <| fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.iThisNext xs |> ignore)

        testCase "Array.iPrevThisNext returns looped triplets with index" <| fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.iPrevThisNext xs |> Seq.toArray
            Assert.AreEqual([|(0,'d','a','b'); (1,'a','b','c'); (2,'b','c','d'); (3,'c','d','a')|], result)

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
            Assert.AreEqual([|42|], result)

        testCase "Array.singleton allows null values" <| fun _ ->
            let result = Array.singleton (null: string)
            Assert.AreEqual(1, result.Length)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Rotation tests------------------------------------------------------------
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

        testCase "Array.rotate throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.rotate 1 xs |> ignore)

        testCase "Array.rotate does not modify input array" <| fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotate 2 xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.rotateDownTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateDownTill(fun i -> i = 7)     , [|7; 2; 3; 7; 5; 0 |])
            throwsArg (fun () -> xs |> Array.rotateDownTill (fun i -> i = 99) |> ignore  )

        testCase "Array.rotateDownTill does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTill (fun i -> i = 7) xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.rotateDownTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateDownTillLast(fun i -> i = 7) , [|2; 3; 7; 5; 0; 7 |])
            throwsArg (fun () -> xs |> Array.rotateDownTillLast (fun i -> i = 99) |> ignore  )

        testCase "Array.rotateDownTillLast does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTillLast (fun i -> i = 7) xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.rotateUpTill" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateUpTill(fun i -> i = 7)       , [|7; 5; 0; 7; 2; 3 |])
            throwsArg (fun () -> xs |> Array.rotateUpTill (fun i -> i = 99) |> ignore  )

        testCase "Array.rotateUpTill does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTill (fun i -> i = 7) xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.rotateUpTillLast" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            Assert.AreEqual(xs |> Array.rotateUpTillLast(fun i -> i = 7)   , [|5; 0; 7; 2; 3; 7 |])
            throwsArg (fun () -> xs |> Array.rotateUpTillLast (fun i -> i = 99) |> ignore  )

        testCase "Array.rotateUpTillLast does not modify input array" <| fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTillLast (fun i -> i = 7) xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Status check functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.isSingleton returns true for single item array" <| fun _ ->
            let xs = [|42|]
            Assert.True(Array.isSingleton xs)

        testCase "Array.isSingleton returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Assert.False(Array.isSingleton xs)

        testCase "Array.isSingleton returns false for array with multiple items" <| fun _ ->
            let xs = [|1; 2|]
            Assert.False(Array.isSingleton xs)

        testCase "Array.isSingleton throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isSingleton xs |> ignore)

        testCase "Array.hasOne returns true for single item array" <| fun _ ->
            let xs = [|42|]
            Assert.True(Array.hasOne xs)

        testCase "Array.hasOne returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Assert.False(Array.hasOne xs)

        testCase "Array.hasOne throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasOne xs |> ignore)

        testCase "Array.isNotEmpty returns true for non-empty array" <| fun _ ->
            let xs = [|1|]
            Assert.True(Array.isNotEmpty xs)

        testCase "Array.isNotEmpty returns false for empty array" <| fun _ ->
            let xs : int[] = [||]
            Assert.False(Array.isNotEmpty xs)

        testCase "Array.isNotEmpty throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isNotEmpty xs |> ignore)

        testCase "Array.hasItems returns true when count matches" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.True(Array.hasItems 3 xs)

        testCase "Array.hasItems returns false when count does not match" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.False(Array.hasItems 2 xs)

        testCase "Array.hasItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasItems 1 xs |> ignore)

        testCase "Array.hasMinimumItems returns true when count is sufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.True(Array.hasMinimumItems 2 xs)
            Assert.True(Array.hasMinimumItems 3 xs)

        testCase "Array.hasMinimumItems returns false when count is insufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.False(Array.hasMinimumItems 4 xs)

        testCase "Array.hasMinimumItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMinimumItems 1 xs |> ignore)

        testCase "Array.hasMaximumItems returns true when count is not exceeded" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.True(Array.hasMaximumItems 3 xs)
            Assert.True(Array.hasMaximumItems 4 xs)

        testCase "Array.hasMaximumItems returns false when count is exceeded" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.False(Array.hasMaximumItems 2 xs)

        testCase "Array.hasMaximumItems throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMaximumItems 1 xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Min/Max functions---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.min2 returns two smallest elements" <| fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b = Array.min2 xs
            Assert.AreEqual(1, a)
            Assert.AreEqual(2, b)

        testCase "Array.min2 keeps order for equal elements" <| fun _ ->
            let xs = [|3; 1; 1; 2|]
            let a, b = Array.min2 xs
            Assert.AreEqual(1, a)
            Assert.AreEqual(1, b)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.max2 returns two largest elements" <| fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b = Array.max2 xs
            Assert.AreEqual(5, a)
            Assert.AreEqual(4, b)

        testCase "Array.max2 keeps order for equal elements" <| fun _ ->
            let xs = [|3; 5; 5; 2|]
            let a, b = Array.max2 xs
            Assert.AreEqual(5, a)
            Assert.AreEqual(5, b)

        testCase "Array.max2 throws on array with less than 2 items" <| fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.max2 xs |> ignore)

        testCase "Array.max2 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max2 xs |> ignore)

        testCase "Array.min2By returns two smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let a, b = Array.min2By String.length xs
            Assert.AreEqual("be", a)
            Assert.AreEqual("do", b)

        testCase "Array.min2By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2By String.length xs |> ignore)

        testCase "Array.max2By returns two largest by projection" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let a, b = Array.max2By String.length xs
            Assert.AreEqual("elephant", a)
            Assert.AreEqual("apple", b)

        testCase "Array.max2By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2By String.length xs |> ignore)

        testCase "Array.min2IndicesBy returns indices of two smallest" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let i1, i2 = Array.min2IndicesBy String.length xs
            Assert.AreEqual(1, i1)
            Assert.AreEqual(3, i2)

        testCase "Array.min2IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2IndicesBy String.length xs |> ignore)

        testCase "Array.max2IndicesBy returns indices of two largest" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let i1, i2 = Array.max2IndicesBy String.length xs
            Assert.AreEqual(3, i1)
            Assert.AreEqual(1, i2)

        testCase "Array.max2IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2IndicesBy String.length xs |> ignore)

        testCase "Array.min3 returns three smallest elements" <| fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b, c = Array.min3 xs
            Assert.AreEqual(1, a)
            Assert.AreEqual(2, b)
            Assert.AreEqual(3, c)

        testCase "Array.min3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.min3 xs |> ignore)

        testCase "Array.min3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.min3 xs |> ignore)

        testCase "Array.max3 returns three largest elements" <| fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b, c = Array.max3 xs
            Assert.AreEqual(5, a)
            Assert.AreEqual(4, b)
            Assert.AreEqual(3, c)

        testCase "Array.max3 throws on array with less than 3 items" <| fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.max3 xs |> ignore)

        testCase "Array.max3 throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max3 xs |> ignore)

        testCase "Array.min3By returns three smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "elephant"|]
            let a, b, c = Array.min3By String.length xs
            Assert.AreEqual("be", a)
            Assert.AreEqual("do", b)
            Assert.AreEqual("cat", c)

        testCase "Array.min3By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3By String.length xs |> ignore)

        testCase "Array.max3By returns three largest by projection" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let a, b, c = Array.max3By String.length xs
            Assert.AreEqual("elephant", a)
            Assert.AreEqual("apple", b)
            Assert.AreEqual("cat", c)

        testCase "Array.max3By throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3By String.length xs |> ignore)

        testCase "Array.min3IndicesBy returns indices of three smallest" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "e"|]
            let i1, i2, i3 = Array.min3IndicesBy String.length xs
            Assert.AreEqual(1, i1)
            Assert.AreEqual(3, i2)
            Assert.AreEqual(4, i3)

        testCase "Array.min3IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3IndicesBy String.length xs |> ignore)

        testCase "Array.max3IndicesBy returns indices of three largest" <| fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let i1, i2, i3 = Array.max3IndicesBy String.length xs
            Assert.AreEqual(3, i1)
            Assert.AreEqual(1, i2)
            Assert.AreEqual(4, i3)

        testCase "Array.max3IndicesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3IndicesBy String.length xs |> ignore)

        testCase "Array.minIndexBy returns index of smallest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let i = Array.minIndexBy String.length xs
            Assert.AreEqual(1, i)

        testCase "Array.minIndexBy throws on empty array" <| fun _ ->
            let xs : string[] = [||]
            throwsArg (fun () -> Array.minIndexBy String.length xs |> ignore)

        testCase "Array.minIndexBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.minIndexBy String.length xs |> ignore)

        testCase "Array.maxIndexBy returns index of largest by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "elephant"|]
            let i = Array.maxIndexBy String.length xs
            Assert.AreEqual(2, i)

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
            Assert.AreEqual([|1; 4; 3; 2; 5|], xs)

        testCase "Array.swap with same index does nothing" <| fun _ ->
            let xs = [|1; 2; 3|]
            Array.swap 1 1 xs
            Assert.AreEqual([|1; 2; 3|], xs)

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
            Assert.AreEqual(5, Array.count xs)

        testCase "Array.count returns 0 for empty array" <| fun _ ->
            let xs : int[] = [||]
            Assert.AreEqual(0, Array.count xs)

        testCase "Array.count throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.count xs |> ignore)

        testCase "Array.countIf counts matching items" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let count = Array.countIf (fun x -> x > 2) xs
            Assert.AreEqual(3, count)

        testCase "Array.countIf returns 0 when no items match" <| fun _ ->
            let xs = [|1; 2; 3|]
            let count = Array.countIf (fun x -> x > 10) xs
            Assert.AreEqual(0, count)

        testCase "Array.countIf returns 0 for empty array" <| fun _ ->
            let xs : int[] = [||]
            let count = Array.countIf (fun _ -> true) xs
            Assert.AreEqual(0, count)

        testCase "Array.countIf throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.countIf (fun _ -> true) xs |> ignore)

        testCase "Array.countIf does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.countIf (fun x -> x > 1) xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Filter by index-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.filteri " <| fun _ ->
            let arr = [|'a';'b';'c'|]
            let result = arr|> Array.filteri (fun i -> i % 2 = 0)
            Assert.AreEqual([|'a';'c'|] , result)
            Assert.AreNotEqual(arr, result,"input array should not be modified")

        testCase "Array.filteri returns empty for all false predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> false) xs
            Assert.AreEqual([||], result)

        testCase "Array.filteri returns all for all true predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> true) xs
            Assert.AreEqual([|1; 2; 3|], result)

        testCase "Array.filteri throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.filteri (fun _ -> true) xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Collection conversion-----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.ofResizeArray creates array from ResizeArray" <| fun _ ->
            let ra = ResizeArray<int>([1; 2; 3])
            let result = Array.ofResizeArray ra
            Assert.AreEqual([|1; 2; 3|], result)

        testCase "Array.ofResizeArray throws on null" <| fun _ ->
            let ra : ResizeArray<int> = null
            throwsNull (fun () -> Array.ofResizeArray ra |> ignore)

        testCase "Array.toResizeArray creates ResizeArray from array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.toResizeArray xs
            Assert.AreEqual(3, result.Count)
            Assert.AreEqual(1, result.[0])
            Assert.AreEqual(2, result.[1])
            Assert.AreEqual(3, result.[2])

        testCase "Array.toResizeArray throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.toResizeArray xs |> ignore)

        testCase "Array.toResizeArray does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.toResizeArray xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.asResizeArray creates ResizeArray from array" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.asResizeArray xs
            Assert.AreEqual(3, result.Count)

        testCase "Array.asResizeArray throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.asResizeArray xs |> ignore)

        testCase "Array.asArray creates array from ResizeArray" <| fun _ ->
            let ra = ResizeArray<int>([1; 2; 3])
            let result = Array.asArray ra
            Assert.AreEqual([|1; 2; 3|], result)

        testCase "Array.asArray throws on null" <| fun _ ->
            let ra : ResizeArray<int> = null
            throwsNull (fun () -> Array.asArray ra |> ignore)

        testCase "Array.ofIList creates array from IList" <| fun _ ->
            let list : IList<int> = [1; 2; 3] :> IList<int>
            let result = Array.ofIList list
            Assert.AreEqual([|1; 2; 3|], result)

        testCase "Array.ofIList throws on null" <| fun _ ->
            let list : IList<int> = null
            throwsNull (fun () -> Array.ofIList list |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Conditional transformation------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.mapIfResult applies transform when result meets predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            Assert.AreEqual([|2; 3; 4|], result)

        testCase "Array.mapIfResult returns original when result does not meet predicate" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            Assert.True(Object.ReferenceEquals(xs, result))

        testCase "Array.mapIfResult throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfResult (fun _ -> true) id xs |> ignore)

        testCase "Array.mapIfInputAndResult applies transform when both predicates pass" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 0) (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            Assert.AreEqual([|2; 3; 4|], result)

        testCase "Array.mapIfInputAndResult returns original when input predicate fails" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 10) (fun _ -> true) (Array.map ((+) 1)) xs
            Assert.True(Object.ReferenceEquals(xs, result))

        testCase "Array.mapIfInputAndResult returns original when result predicate fails" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun _ -> true) (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            Assert.True(Object.ReferenceEquals(xs, result))

        testCase "Array.mapIfInputAndResult throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfInputAndResult (fun _ -> true) (fun _ -> true) id xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Duplicates functions------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.duplicates returns duplicate elements" <| fun _ ->
            let xs = [|1; 2; 3; 2; 4; 3; 2|]
            let result = Array.duplicates xs
            Assert.AreEqual([|2; 3|], result)

        testCase "Array.duplicates returns empty when no duplicates" <| fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.duplicates xs
            Assert.AreEqual([||], result)

        testCase "Array.duplicates returns empty for empty array" <| fun _ ->
            let xs : int[] = [||]
            let result = Array.duplicates xs
            Assert.AreEqual([||], result)

        testCase "Array.duplicates throws on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.duplicates xs |> ignore)

        testCase "Array.duplicates does not modify input array" <| fun _ ->
            let xs = [|1; 2; 3; 2; 4|]
            let original = xs.Duplicate()
            let _ = Array.duplicates xs
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.duplicatesBy returns duplicates by projection" <| fun _ ->
            let xs = [|"apple"; "be"; "Art"; "big"|]
            let result = Array.duplicatesBy (fun (s:string) -> s.[0]) xs
            Assert.AreEqual([|"Art"; "big"|], result)

        testCase "Array.duplicatesBy returns empty when no duplicates" <| fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let result = Array.duplicatesBy (fun (s:string) -> s.[0]) xs
            Assert.AreEqual([||], result)

        testCase "Array.duplicatesBy throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.duplicatesBy String.length xs |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Error handling functions--------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.failIfEmpty returns array when not empty" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfEmpty "should not throw" xs
            Assert.True(Object.ReferenceEquals(xs, result))

        testCase "Array.failIfEmpty throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            Expect.throws (fun () -> Array.failIfEmpty "is empty" xs |> ignore) "should throw on empty"

        testCase "Array.failIfLessThan returns array when count is sufficient" <| fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfLessThan 3 "should not throw" xs
            Assert.True(Object.ReferenceEquals(xs, result))

        testCase "Array.failIfLessThan throws when count is insufficient" <| fun _ ->
            let xs = [|1; 2|]
            Expect.throws (fun () -> Array.failIfLessThan 3 "too few" xs |> ignore) "should throw when too few"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Search/Match functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.matches returns true when array matches at index" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.True(Array.matches [|2; 3|] 1 xs)

        testCase "Array.matches returns false when array does not match" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.False(Array.matches [|2; 4|] 1 xs)

        testCase "Array.matches returns false when not enough items remain" <| fun _ ->
            let xs = [|1; 2; 3|]
            Assert.False(Array.matches [|3; 4|] 2 xs)

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
            Assert.AreEqual(original, xs, "input array should not be modified")

        testCase "Array.findValue finds value in range" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(2, Array.findValue 3 0 4 xs)

        testCase "Array.findValue returns -1 when not found" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(-1, Array.findValue 6 0 4 xs)

        testCase "Array.findValue respects range bounds" <| fun _ ->
            let xs = [|1; 2; 3; 2; 5|]
            Assert.AreEqual(-1, Array.findValue 2 2 2 xs)
            Assert.AreEqual(3, Array.findValue 2 2 4 xs)

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
            Assert.AreEqual(3, Array.findLastValue 2 0 4 xs)

        testCase "Array.findLastValue returns -1 when not found" <| fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Assert.AreEqual(-1, Array.findLastValue 6 0 4 xs)

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
