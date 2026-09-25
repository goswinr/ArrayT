namespace Tests

open ArrayT

open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open System
open System.Collections.Generic


module Module2 =
 open Exceptions

 let tests =
    testList ("Module Tests", [

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Basic Get/Set functions---------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.get returns item at index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.get 2 xs) (tag "get at index 2" >> isEqualTo 3)
            assertThat (Array.get 0 xs) (tag "get at index 0" >> isEqualTo 1)
            assertThat (Array.get 4 xs) (tag "get at index 4" >> isEqualTo 5)
        )

        test ("Array.get throws on negative index", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.get -1 xs |> ignore)
        )

        test ("Array.get throws on index out of range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.get 3 xs |> ignore)
        )

        test ("Array.get throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.get 0 xs |> ignore)
        )

        test ("Array.get does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.get 1 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.set sets item at index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.set 2 99 xs
            assertThat xs.[2] (tag "value should be set" >> isEqualTo 99)
        )

        test ("Array.set throws on negative index", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.set -1 99 xs)
        )

        test ("Array.set throws on index out of range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.set 3 99 xs)
        )

        test ("Array.set throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.set 0 99 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Negative indexing---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.getNeg returns item at negative index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.getNeg -1 xs) (tag "getNeg -1" >> isEqualTo 5)
            assertThat (Array.getNeg -2 xs) (tag "getNeg -2" >> isEqualTo 4)
            assertThat (Array.getNeg -5 xs) (tag "getNeg -5" >> isEqualTo 1)
        )

        test ("Array.getNeg works with positive index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.getNeg 0 xs) (tag "getNeg 0" >> isEqualTo 1)
            assertThat (Array.getNeg 2 xs) (tag "getNeg 2" >> isEqualTo 3)
        )

        test ("Array.getNeg throws on index out of range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.getNeg -4 xs |> ignore)
            throwsRange (fun () -> Array.getNeg 3 xs |> ignore)
        )

        test ("Array.getNeg throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getNeg -1 xs |> ignore)
        )

        test ("Array.getNeg does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.getNeg -1 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.setNeg sets item at negative index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg -1 99 xs
            assertThat xs.[4] (tag "setNeg -1 should set last item" >> isEqualTo 99)
        )

        test ("Array.setNeg works with positive index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.setNeg 0 99 xs
            assertThat xs.[0] (tag "setNeg 0 should set first item" >> isEqualTo 99)
        )

        test ("Array.setNeg throws on index out of range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsRange (fun () -> Array.setNeg -4 99 xs)
            throwsRange (fun () -> Array.setNeg 3 99 xs)
        )

        test ("Array.setNeg throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setNeg -1 99 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Looped indexing-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.getLooped returns item with looped positive index", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.getLooped 0 xs) (tag "getLooped 0" >> isEqualTo 1)
            assertThat (Array.getLooped 3 xs) (tag "getLooped 3" >> isEqualTo 1)
            assertThat (Array.getLooped 4 xs) (tag "getLooped 4" >> isEqualTo 2)
            assertThat (Array.getLooped 6 xs) (tag "getLooped 6" >> isEqualTo 1)
        )

        test ("Array.getLooped returns item with looped negative index", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.getLooped -1 xs) (tag "getLooped -1" >> isEqualTo 3)
            assertThat (Array.getLooped -2 xs) (tag "getLooped -2" >> isEqualTo 2)
            assertThat (Array.getLooped -3 xs) (tag "getLooped -3" >> isEqualTo 1)
            assertThat (Array.getLooped -4 xs) (tag "getLooped -4" >> isEqualTo 3)
        )

        test ("Array.getLooped throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.getLooped 0 xs |> ignore)
        )

        test ("Array.getLooped throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getLooped 0 xs |> ignore)
        )

        test ("Array.getLooped does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.getLooped 5 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.setLooped sets item with looped index", fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped 3 99 xs
            assertThat xs.[0] (tag "setLooped 3 should set first item" >> isEqualTo 99)
        )

        test ("Array.setLooped sets item with negative looped index", fun _ ->
            let xs = [|1; 2; 3|]
            Array.setLooped -1 99 xs
            assertThat xs.[2] (tag "setLooped -1 should set last item" >> isEqualTo 99)
        )

        test ("Array.setLooped throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.setLooped 0 99 xs)
        )

        test ("Array.setLooped throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setLooped 0 99 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Positional access---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.first returns first item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.first xs) (tag "first" >> isEqualTo 1)
        )

        test ("Array.first throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)
        )

        test ("Array.first throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.first xs |> ignore)
        )

        test ("Array.first does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.first xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.second returns second item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.second xs) (tag "second" >> isEqualTo 2)
        )

        test ("Array.second throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.second xs |> ignore)
        )

        test ("Array.second throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.second xs |> ignore)
        )

        test ("Array.third returns third item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.third xs) (tag "third" >> isEqualTo 3)
        )

        test ("Array.third throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.third xs |> ignore)
        )

        test ("Array.third throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.third xs |> ignore)
        )

        test ("Array.secondLast returns second last item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.secondLast xs) (tag "secondLast" >> isEqualTo 4)
        )

        test ("Array.secondLast throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsRange (fun () -> Array.secondLast xs |> ignore)
        )

        test ("Array.secondLast throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.secondLast xs |> ignore)
        )

        test ("Array.thirdLast returns third last item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.thirdLast xs) (tag "thirdLast" >> isEqualTo 3)
        )

        test ("Array.thirdLast throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.thirdLast xs |> ignore)
        )

        test ("Array.thirdLast throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thirdLast xs |> ignore)
        )

        test ("Array.firstAndOnly returns only item", fun _ ->
            let xs = [|42|]
            assertThat (Array.firstAndOnly xs) (tag "firstAndOnly" >> isEqualTo 42)
        )

        test ("Array.firstAndOnly throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.firstAndOnly xs |> ignore)
        )

        test ("Array.firstAndOnly throws on array with more than one item", fun _ ->
            let xs = [|1; 2|]
            throwsRange (fun () -> Array.firstAndOnly xs |> ignore)
        )

        test ("Array.firstAndOnly throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.firstAndOnly xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Slicing and trimming------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.slice returns slice with positive indices", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.slice 1 3 xs = [|2; 3; 4|]) (tag "slice 1 3" >> isTrue)
        )

        test ("Array.slice returns slice with negative indices", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.slice -2 -1 xs = [|4; 5|]) (tag "slice -2 -1" >> isTrue)
            assertThat (Array.slice 1 -2 xs = [|2; 3; 4|]) (tag "slice 1 -2" >> isTrue)
        )

        test ("Array.slice returns single item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.slice 2 2 xs = [|3|]) (tag "slice 2 2" >> isTrue)
        )

        test ("Array.slice throws on invalid range", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            throwsRange (fun () -> Array.slice 3 1 xs |> ignore)
        )

        test ("Array.slice returns empty array when end index is one less than start index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.slice 3 2 xs = [||]) (tag "slice 3 2" >> isTrue)
            assertThat (Array.slice 0 -6 xs = [||]) (tag "slice 0 -6, like trim 0 5" >> isTrue)
        )

        test ("Array.slice error messages name the offending index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            throwsWith ["Start index 3 is bigger than end index 1"] (fun () -> Array.slice 3 1 xs |> ignore)
            throwsWith ["End index -99 is out of range"] (fun () -> Array.slice 1 -99 xs |> ignore)
            throwsWith ["End index -6 is out of range"] (fun () -> Array.slice 1 -6 xs |> ignore)
            throwsWith ["End index 5 is out of range"] (fun () -> Array.slice 1 5 xs |> ignore)
            throwsWith ["Start index -6 is out of range"] (fun () -> Array.slice -6 2 xs |> ignore)
        )

        test ("Array.slice throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.slice 0 1 xs |> ignore)
        )

        test ("Array.slice does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.slice 1 3 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.trim trims from start and end", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.trim 1 1 xs = [|2; 3; 4|]) (tag "trim 1 1" >> isTrue)
        )

        test ("Array.trim returns empty array when trimming more than length", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.trim 2 2 xs = [||]) (tag "trim 2 2" >> isTrue)
        )

        test ("Array.trim returns same elements when trimming 0", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.trim 0 0 xs = [|1; 2; 3|]) (tag "trim 0 0" >> isTrue)
        )

        test ("Array.trim throws on negative trim values", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.trim -1 0 xs |> ignore)
            throwsArg (fun () -> Array.trim 0 -1 xs |> ignore)
        )

        test ("Array.trim error message includes the negative value", fun _ ->
            let xs = [|1; 2; 3|]
            throwsWith ["fromStartCount"; "-1"] (fun () -> Array.trim -1 0 xs |> ignore)
            throwsWith ["fromEndCount"; "-2"] (fun () -> Array.trim 0 -2 xs |> ignore)
        )

        test ("Array.trim throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.trim 1 1 xs |> ignore)
        )

        test ("Array.trim does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.trim 1 1 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Windowing functions (non-looped)------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.windowed2 returns pairs", fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.windowed2 xs |> Seq.toArray
            assertThat (result = [|(1,2); (2,3); (3,4)|]) (tag "windowed2" >> isTrue)
        )

        test ("Array.windowed2 throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.windowed2 xs |> ignore)
        )

        test ("Array.windowed2 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.windowed2 xs |> ignore)
        )

        test ("Array.windowed2 does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.windowed2 xs |> Seq.toArray
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.windowed3 returns triplets", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let result = Array.windowed3 xs |> Seq.toArray
            assertThat (result = [|(1,2,3); (2,3,4); (3,4,5)|]) (tag "windowed3" >> isTrue)
        )

        test ("Array.windowed3 throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.windowed3 xs |> ignore)
        )

        test ("Array.windowed3 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.windowed3 xs |> ignore)
        )

        test ("Array.windowed2i returns pairs with index", fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.windowed2i xs |> Seq.toArray
            assertThat (result = [|(0,'a','b'); (1,'b','c'); (2,'c','d')|]) (tag "windowed2i" >> isTrue)
        )

        test ("Array.windowed2i throws on array with less than 2 items", fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.windowed2i xs |> ignore)
        )

        test ("Array.windowed2i throws on null array", fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.windowed2i xs |> ignore)
        )

        test ("Array.windowed3i returns triplets with index", fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'; 'e'|]
            let result = Array.windowed3i xs |> Seq.toArray
            assertThat (result = [|(1,'a','b','c'); (2,'b','c','d'); (3,'c','d','e')|]) (tag "windowed3i" >> isTrue)
        )

        test ("Array.windowed3i throws on array with less than 3 items", fun _ ->
            let xs = [|'a'; 'b'|]
            throwsArg (fun () -> Array.windowed3i xs |> ignore)
        )

        test ("Array.windowed3i throws on null array", fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.windowed3i xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Windowing functions (looped)----------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.thisNext returns looped pairs", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.thisNext xs |> Seq.toArray
            assertThat (result = [|(1,2); (2,3); (3,1)|]) (tag "thisNext" >> isTrue)
        )

        test ("Array.thisNext throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.thisNext xs |> ignore)
        )

        test ("Array.thisNext throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.thisNext xs |> ignore)
        )

        test ("Array.prevThis returns looped pairs starting with last-first", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.prevThis xs |> Seq.toArray
            assertThat (result = [|(3,1); (1,2); (2,3)|]) (tag "prevThis" >> isTrue)
        )

        test ("Array.prevThis throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.prevThis xs |> ignore)
        )

        test ("Array.prevThis throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThis xs |> ignore)
        )

        test ("Array.prevThisNext returns looped triplets", fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.prevThisNext xs |> Seq.toArray
            assertThat (result = [|(4,1,2); (1,2,3); (2,3,4); (3,4,1)|]) (tag "prevThisNext" >> isTrue)
        )

        test ("Array.prevThisNext throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.prevThisNext xs |> ignore)
        )

        test ("Array.prevThisNext throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.prevThisNext xs |> ignore)
        )

        test ("Array.iThisNext returns looped pairs with index", fun _ ->
            let xs = [|'a'; 'b'; 'c'|]
            let result = Array.iThisNext xs |> Seq.toArray
            assertThat (result = [|(0,'a','b'); (1,'b','c'); (2,'c','a')|]) (tag "iThisNext" >> isTrue)
        )

        test ("Array.iThisNext throws on array with less than 2 items", fun _ ->
            let xs = [|'a'|]
            throwsArg (fun () -> Array.iThisNext xs |> ignore)
        )

        test ("Array.iThisNext throws on null array", fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.iThisNext xs |> ignore)
        )

        test ("Array.iPrevThisNext returns looped triplets with index", fun _ ->
            let xs = [|'a'; 'b'; 'c'; 'd'|]
            let result = Array.iPrevThisNext xs |> Seq.toArray
            assertThat (result = [|(0,'d','a','b'); (1,'a','b','c'); (2,'b','c','d'); (3,'c','d','a')|]) (tag "iPrevThisNext" >> isTrue)
        )

        test ("Array.iPrevThisNext throws on array with less than 3 items", fun _ ->
            let xs = [|'a'; 'b'|]
            throwsArg (fun () -> Array.iPrevThisNext xs |> ignore)
        )

        test ("Array.iPrevThisNext throws on null array", fun _ ->
            let xs : char[] = null
            throwsNull (fun () -> Array.iPrevThisNext xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Singleton creation--------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.singleton creates single item array", fun _ ->
            let result = Array.singleton 42
            assertThat (result = [|42|]) (tag "singleton" >> isTrue)
        )

        test ("Array.singleton allows null values", fun _ ->
            let result = Array.singleton (null: string)
            assertThat result.Length (tag "singleton null" >> isEqualTo 1)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Rotation tests------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.rotate", fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            assertThat (xs |> Array.rotate  2 = [|4; 5; 0; 1; 2; 3 |]) (tag "rotate 2" >> isTrue)
            assertThat (xs |> Array.rotate  1 = [|5; 0; 1; 2; 3; 4 |]) (tag "rotate 1" >> isTrue)
            assertThat (xs |> Array.rotate -2 = [|2; 3; 4; 5; 0; 1|]) (tag "rotate -2" >> isTrue)
            assertThat (xs |> Array.rotate -1 = [|1; 2; 3; 4; 5; 0|]) (tag "rotate -1" >> isTrue)
            assertThat (xs |> Array.rotate -6 = xs) (tag "rotate -6" >> isTrue)
            assertThat (xs |> Array.rotate  12 = xs) (tag "rotate 12" >> isTrue)
            assertThat (xs |> Array.rotate -12 = xs) (tag "rotate -12" >> isTrue)
            assertThat (xs |> Array.rotate -13 = (xs|> Array.rotate -1)) (tag "rotate -13" >> isTrue)
            assertThat (xs |> Array.rotate  13 = (xs|> Array.rotate  1)) (tag "rotate 13" >> isTrue)
        )

        test ("Array.rotate throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.rotate 1 xs |> ignore)
        )

        test ("Array.rotate does not modify input array", fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotate 2 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.rotateDownTill", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            assertThat (xs |> Array.rotateDownTill(fun i -> i = 7) = [|7; 2; 3; 7; 5; 0 |]) (tag "rotateDownTill" >> isTrue)
            throwsArg (fun () -> xs |> Array.rotateDownTill (fun i -> i = 99) |> ignore)
        )

        test ("Array.rotateDownTill does not modify input array", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTill (fun i -> i = 7) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.rotateDownTillLast", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            assertThat (xs |> Array.rotateDownTillLast(fun i -> i = 7) = [|2; 3; 7; 5; 0; 7 |]) (tag "rotateDownTillLast" >> isTrue)
            throwsArg (fun () -> xs |> Array.rotateDownTillLast (fun i -> i = 99) |> ignore)
        )

        test ("Array.rotateDownTillLast does not modify input array", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateDownTillLast (fun i -> i = 7) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.rotateUpTill", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            assertThat (xs |> Array.rotateUpTill(fun i -> i = 7) = [|7; 5; 0; 7; 2; 3 |]) (tag "rotateUpTill" >> isTrue)
            throwsArg (fun () -> xs |> Array.rotateUpTill (fun i -> i = 99) |> ignore)
        )

        test ("Array.rotateUpTill does not modify input array", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTill (fun i -> i = 7) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.rotateUpTillLast", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            assertThat (xs |> Array.rotateUpTillLast(fun i -> i = 7) = [|5; 0; 7; 2; 3; 7 |]) (tag "rotateUpTillLast" >> isTrue)
            throwsArg (fun () -> xs |> Array.rotateUpTillLast (fun i -> i = 99) |> ignore)
        )

        test ("Array.rotateUpTillLast does not modify input array", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Duplicate()
            let _ = Array.rotateUpTillLast (fun i -> i = 7) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Status check functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.isSingleton returns true for single item array", fun _ ->
            let xs = [|42|]
            assertThat (Array.isSingleton xs) (tag "isSingleton" >> isTrue)
        )

        test ("Array.isSingleton returns false for empty array", fun _ ->
            let xs : int[] = [||]
            assertThat (Array.isSingleton xs) (tag "isSingleton empty" >> isFalse)
        )

        test ("Array.isSingleton returns false for array with multiple items", fun _ ->
            let xs = [|1; 2|]
            assertThat (Array.isSingleton xs) (tag "isSingleton multiple" >> isFalse)
        )

        test ("Array.isSingleton throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isSingleton xs |> ignore)
        )

        test ("Array.hasOne returns true for single item array", fun _ ->
            let xs = [|42|]
            assertThat (Array.hasOne xs) (tag "hasOne" >> isTrue)
        )

        test ("Array.hasOne returns false for empty array", fun _ ->
            let xs : int[] = [||]
            assertThat (Array.hasOne xs) (tag "hasOne empty" >> isFalse)
        )

        test ("Array.hasOne throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasOne xs |> ignore)
        )

        test ("Array.isNotEmpty returns true for non-empty array", fun _ ->
            let xs = [|1|]
            assertThat (Array.isNotEmpty xs) (tag "isNotEmpty" >> isTrue)
        )

        test ("Array.isNotEmpty returns false for empty array", fun _ ->
            let xs : int[] = [||]
            assertThat (Array.isNotEmpty xs) (tag "isNotEmpty empty" >> isFalse)
        )

        test ("Array.isNotEmpty throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.isNotEmpty xs |> ignore)
        )

        test ("Array.hasItems returns true when count matches", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasItems 3 xs) (tag "hasItems 3" >> isTrue)
        )

        test ("Array.hasItems returns false when count does not match", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasItems 2 xs) (tag "hasItems 2" >> isFalse)
        )

        test ("Array.hasItems throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasItems 1 xs |> ignore)
        )

        test ("Array.hasMinimumItems returns true when count is sufficient", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasMinimumItems 2 xs) (tag "hasMinimumItems 2" >> isTrue)
            assertThat (Array.hasMinimumItems 3 xs) (tag "hasMinimumItems 3" >> isTrue)
        )

        test ("Array.hasMinimumItems returns false when count is insufficient", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasMinimumItems 4 xs) (tag "hasMinimumItems 4" >> isFalse)
        )

        test ("Array.hasMinimumItems throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMinimumItems 1 xs |> ignore)
        )

        test ("Array.hasMaximumItems returns true when count is not exceeded", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasMaximumItems 3 xs) (tag "hasMaximumItems 3" >> isTrue)
            assertThat (Array.hasMaximumItems 4 xs) (tag "hasMaximumItems 4" >> isTrue)
        )

        test ("Array.hasMaximumItems returns false when count is exceeded", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.hasMaximumItems 2 xs) (tag "hasMaximumItems 2" >> isFalse)
        )

        test ("Array.hasMaximumItems throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.hasMaximumItems 1 xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Min/Max functions---------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.min2 returns two smallest elements", fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b = Array.min2 xs
            assertThat a (tag "min2 first" >> isEqualTo 1)
            assertThat b (tag "min2 second" >> isEqualTo 2)
        )

        test ("Array.min2 keeps order for equal elements", fun _ ->
            let xs = [|3; 1; 1; 2|]
            let a, b = Array.min2 xs
            assertThat a (tag "min2 first equal" >> isEqualTo 1)
            assertThat b (tag "min2 second equal" >> isEqualTo 1)
        )

        test ("Array.min2 throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.min2 xs |> ignore)
        )

        test ("Array.min2 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.min2 xs |> ignore)
        )

        test ("Array.min2 does not modify input array", fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.min2 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.max2 returns two largest elements", fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b = Array.max2 xs
            assertThat a (tag "max2 first" >> isEqualTo 5)
            assertThat b (tag "max2 second" >> isEqualTo 4)
        )

        test ("Array.max2 keeps order for equal elements", fun _ ->
            let xs = [|3; 5; 5; 2|]
            let a, b = Array.max2 xs
            assertThat a (tag "max2 first equal" >> isEqualTo 5)
            assertThat b (tag "max2 second equal" >> isEqualTo 5)
        )

        test ("Array.max2 throws on array with less than 2 items", fun _ ->
            let xs = [|1|]
            throwsArg (fun () -> Array.max2 xs |> ignore)
        )

        test ("Array.max2 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max2 xs |> ignore)
        )

        test ("Array.min2By returns two smallest by projection", fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let a, b = Array.min2By String.length xs
            assertThat a (tag "min2By first" >> isEqualTo "be")
            assertThat b (tag "min2By second" >> isEqualTo "do")
        )

        test ("Array.min2By throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2By String.length xs |> ignore)
        )

        test ("Array.max2By returns two largest by projection", fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let a, b = Array.max2By String.length xs
            assertThat a (tag "max2By first" >> isEqualTo "elephant")
            assertThat b (tag "max2By second" >> isEqualTo "apple")
        )

        test ("Array.max2By throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2By String.length xs |> ignore)
        )

        test ("Array.min2IndicesBy returns indices of two smallest", fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"|]
            let i1, i2 = Array.min2IndicesBy String.length xs
            assertThat i1 (tag "min2IndicesBy first" >> isEqualTo 1)
            assertThat i2 (tag "min2IndicesBy second" >> isEqualTo 3)
        )

        test ("Array.min2IndicesBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min2IndicesBy String.length xs |> ignore)
        )

        test ("Array.max2IndicesBy returns indices of two largest", fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"|]
            let i1, i2 = Array.max2IndicesBy String.length xs
            assertThat i1 (tag "max2IndicesBy first" >> isEqualTo 3)
            assertThat i2 (tag "max2IndicesBy second" >> isEqualTo 1)
        )

        test ("Array.max2IndicesBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max2IndicesBy String.length xs |> ignore)
        )

        test ("Array.min3 returns three smallest elements", fun _ ->
            let xs = [|5; 1; 4; 2; 3|]
            let a, b, c = Array.min3 xs
            assertThat a (tag "min3 first" >> isEqualTo 1)
            assertThat b (tag "min3 second" >> isEqualTo 2)
            assertThat c (tag "min3 third" >> isEqualTo 3)
        )

        test ("Array.min3 throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.min3 xs |> ignore)
        )

        test ("Array.min3 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.min3 xs |> ignore)
        )

        test ("Array.max3 returns three largest elements", fun _ ->
            let xs = [|1; 5; 2; 4; 3|]
            let a, b, c = Array.max3 xs
            assertThat a (tag "max3 first" >> isEqualTo 5)
            assertThat b (tag "max3 second" >> isEqualTo 4)
            assertThat c (tag "max3 third" >> isEqualTo 3)
        )

        test ("Array.max3 throws on array with less than 3 items", fun _ ->
            let xs = [|1; 2|]
            throwsArg (fun () -> Array.max3 xs |> ignore)
        )

        test ("Array.max3 throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.max3 xs |> ignore)
        )

        test ("Array.min3By returns three smallest by projection", fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "elephant"|]
            let a, b, c = Array.min3By String.length xs
            assertThat a (tag "min3By first" >> isEqualTo "be")
            assertThat b (tag "min3By second" >> isEqualTo "do")
            assertThat c (tag "min3By third" >> isEqualTo "cat")
        )

        test ("Array.min3By throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3By String.length xs |> ignore)
        )

        test ("Array.max3By returns three largest by projection", fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let a, b, c = Array.max3By String.length xs
            assertThat a (tag "max3By first" >> isEqualTo "elephant")
            assertThat b (tag "max3By second" >> isEqualTo "apple")
            assertThat c (tag "max3By third" >> isEqualTo "cat")
        )

        test ("Array.max3By throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3By String.length xs |> ignore)
        )

        test ("Array.min3IndicesBy returns indices of three smallest", fun _ ->
            let xs = [|"apple"; "be"; "cat"; "do"; "e"|]
            let i1, i2, i3 = Array.min3IndicesBy String.length xs
            assertThat i1 (tag "min3IndicesBy first" >> isEqualTo 4)
            assertThat i2 (tag "min3IndicesBy second" >> isEqualTo 1)
            assertThat i3 (tag "min3IndicesBy third" >> isEqualTo 3)
        )

        test ("Array.min3IndicesBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.min3IndicesBy String.length xs |> ignore)
        )

        test ("Array.max3IndicesBy returns indices of three largest", fun _ ->
            let xs = [|"a"; "apple"; "be"; "elephant"; "cat"|]
            let i1, i2, i3 = Array.max3IndicesBy String.length xs
            assertThat i1 (tag "max3IndicesBy first" >> isEqualTo 3)
            assertThat i2 (tag "max3IndicesBy second" >> isEqualTo 1)
            assertThat i3 (tag "max3IndicesBy third" >> isEqualTo 4)
        )

        test ("Array.min3IndicesBy and max3IndicesBy keep the original order on ties", fun _ ->
            assertThat (Array.min3IndicesBy id [|1; 2; 2|]) (tag "min3IndicesBy 1,2,2" >> isEqualTo (0, 1, 2))
            assertThat (Array.min3IndicesBy id [|1; 2; 1|]) (tag "min3IndicesBy 1,2,1" >> isEqualTo (0, 2, 1))
            assertThat (Array.min3IndicesBy id [|2; 1; 1|]) (tag "min3IndicesBy 2,1,1" >> isEqualTo (1, 2, 0))
            assertThat (Array.min3IndicesBy id [|1; 1; 1; 1|]) (tag "min3IndicesBy all equal" >> isEqualTo (0, 1, 2))
            assertThat (Array.max3IndicesBy id [|2; 1; 1|]) (tag "max3IndicesBy 2,1,1" >> isEqualTo (0, 1, 2))
            assertThat (Array.max3IndicesBy id [|2; 1; 2|]) (tag "max3IndicesBy 2,1,2" >> isEqualTo (0, 2, 1))
        )

        test ("Array.min3By and max3By keep the original order on ties", fun _ ->
            let xs = [|(1, "a"); (2, "b"); (2, "c")|]
            assertThat (Array.min3By fst xs) (tag "min3By" >> isEqualTo ((1, "a"), (2, "b"), (2, "c")))
            let ys = [|(2, "a"); (1, "b"); (2, "c")|]
            assertThat (Array.max3By fst ys) (tag "max3By" >> isEqualTo ((2, "a"), (2, "c"), (1, "b")))
        )

        test ("Array.max3IndicesBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.max3IndicesBy String.length xs |> ignore)
        )

        test ("Array.minIndexBy returns index of smallest by projection", fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let i = Array.minIndexBy String.length xs
            assertThat i (tag "minIndexBy" >> isEqualTo 1)
        )

        test ("Array.minIndexBy throws on empty array", fun _ ->
            let xs : string[] = [||]
            throwsArg (fun () -> Array.minIndexBy String.length xs |> ignore)
        )

        test ("Array.minIndexBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.minIndexBy String.length xs |> ignore)
        )

        test ("Array.maxIndexBy returns index of largest by projection", fun _ ->
            let xs = [|"apple"; "be"; "elephant"|]
            let i = Array.maxIndexBy String.length xs
            assertThat i (tag "maxIndexBy" >> isEqualTo 2)
        )

        test ("Array.maxIndexBy throws on empty array", fun _ ->
            let xs : string[] = [||]
            throwsArg (fun () -> Array.maxIndexBy String.length xs |> ignore)
        )

        test ("Array.maxIndexBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.maxIndexBy String.length xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Swap function-------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.swap swaps two elements", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            Array.swap 1 3 xs
            assertThat (xs = [|1; 4; 3; 2; 5|]) (tag "swap 1 3" >> isTrue)
        )

        test ("Array.swap with same index does nothing", fun _ ->
            let xs = [|1; 2; 3|]
            Array.swap 1 1 xs
            assertThat (xs = [|1; 2; 3|]) (tag "swap same index" >> isTrue)
        )

        test ("Array.swap throws on negative index", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.swap -1 1 xs)
            throwsArg (fun () -> Array.swap 1 -1 xs)
        )

        test ("Array.swap throws on index out of range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.swap 0 3 xs)
            throwsArg (fun () -> Array.swap 3 0 xs)
        )

        test ("Array.swap error message includes the indices", fun _ ->
            let xs = [|1; 2; 3|]
            throwsWith ["i=0"; "j=7"; "last index 2"] (fun () -> Array.swap 0 7 xs)
        )

        test ("Array.swap throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.swap 0 1 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Count functions-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.count returns length", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.count xs) (tag "count" >> isEqualTo 5)
        )

        test ("Array.count returns 0 for empty array", fun _ ->
            let xs : int[] = [||]
            assertThat (Array.count xs) (tag "count empty" >> isEqualTo 0)
        )

        test ("Array.count throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.count xs |> ignore)
        )

        test ("Array.countIf counts matching items", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let count = Array.countIf (fun x -> x > 2) xs
            assertThat count (tag "countIf" >> isEqualTo 3)
        )

        test ("Array.countIf returns 0 when no items match", fun _ ->
            let xs = [|1; 2; 3|]
            let count = Array.countIf (fun x -> x > 10) xs
            assertThat count (tag "countIf none match" >> isEqualTo 0)
        )

        test ("Array.countIf returns 0 for empty array", fun _ ->
            let xs : int[] = [||]
            let count = Array.countIf (fun _ -> true) xs
            assertThat count (tag "countIf empty" >> isEqualTo 0)
        )

        test ("Array.countIf throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.countIf (fun _ -> true) xs |> ignore)
        )

        test ("Array.countIf does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.countIf (fun x -> x > 1) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Filter by index-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.filteri ", fun _ ->
            let arr = [|'a';'b';'c'|]
            let result = arr|> Array.filteri (fun i -> i % 2 = 0)
            assertThat (result = [|'a';'c'|]) (tag "filteri" >> isTrue)
            assertThat (Object.ReferenceEquals(arr, result)) (tag "filteri should return new array" >> isFalse)
        )

        test ("Array.filteri returns empty for all false predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> false) xs
            assertThat (result = [||]) (tag "filteri all false" >> isTrue)
        )

        test ("Array.filteri returns all for all true predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ -> true) xs
            assertThat (result = [|1; 2; 3|]) (tag "filteri all true" >> isTrue)
        )

        test ("Array.filteri throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.filteri (fun _ -> true) xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Collection conversion-----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.ofResizeArray creates array from ResizeArray", fun _ ->
            let ra = ResizeArray<int>([1; 2; 3])
            let result = Array.ofResizeArray ra
            assertThat (result = [|1; 2; 3|]) (tag "ofResizeArray" >> isTrue)
        )

        test ("Array.ofResizeArray throws on null", fun _ ->
            let ra : ResizeArray<int> = null
            throwsNull (fun () -> Array.ofResizeArray ra |> ignore)
        )

        test ("Array.toResizeArray creates ResizeArray from array", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.toResizeArray xs
            assertThat result.Count (tag "toResizeArray count" >> isEqualTo 3)
            assertThat result.[0] (tag "toResizeArray [0]" >> isEqualTo 1)
            assertThat result.[1] (tag "toResizeArray [1]" >> isEqualTo 2)
            assertThat result.[2] (tag "toResizeArray [2]" >> isEqualTo 3)
        )

        test ("Array.toResizeArray throws on null", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.toResizeArray xs |> ignore)
        )

        test ("Array.toResizeArray does not modify input array", fun _ ->
            let xs = [|1; 2; 3|]
            let original = xs.Duplicate()
            let _ = Array.toResizeArray xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.asResizeArray creates ResizeArray from array", fun _ ->
            let xs = [|"a"; "b"; "c"|]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "asResizeArray count" >> isEqualTo 3)
        )

        test ("Array.asResizeArray throws on null", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.asResizeArray xs |> ignore)
        )

        test ("Array.ofIList creates array from IList", fun _ ->
            let list : IList<int> = ResizeArray<int>([1; 2; 3])
            let result = Array.ofIList list
            assertThat (result = [|1; 2; 3|]) (tag "ofIList" >> isTrue)
        )

        test ("Array.ofIList throws on null", fun _ ->
            let list : IList<int> = null
            throwsNull (fun () -> Array.ofIList list |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Conditional transformation------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.mapIfResult applies transform when result meets predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            assertThat (result = [|2; 3; 4|]) (tag "mapIfResult applied" >> isTrue)
        )

        test ("Array.mapIfResult returns original when result does not meet predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfResult (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "mapIfResult not applied" >> isTrue)
        )

        test ("Array.mapIfResult throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfResult (fun _ -> true) id xs |> ignore)
        )

        test ("Array.mapIfInputAndResult applies transform when both predicates pass", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 0) (fun r -> r.Length > 0) (Array.map ((+) 1)) xs
            assertThat (result = [|2; 3; 4|]) (tag "mapIfInputAndResult applied" >> isTrue)
        )

        test ("Array.mapIfInputAndResult returns original when input predicate fails", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun a -> a.Length > 10) (fun _ -> true) (Array.map ((+) 1)) xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "mapIfInputAndResult input failed" >> isTrue)
        )

        test ("Array.mapIfInputAndResult returns original when result predicate fails", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.mapIfInputAndResult (fun _ -> true) (fun r -> r.Length > 10) (Array.map ((+) 1)) xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "mapIfInputAndResult result failed" >> isTrue)
        )

        test ("Array.mapIfInputAndResult throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.mapIfInputAndResult (fun _ -> true) (fun _ -> true) id xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Duplicates functions------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.duplicates returns duplicate elements", fun _ ->
            let xs = [|1; 2; 3; 2; 4; 3; 2|]
            let result = Array.duplicates xs
            assertThat (result = [|2; 3|]) (tag "duplicates" >> isTrue)
        )

        test ("Array.duplicates returns empty when no duplicates", fun _ ->
            let xs = [|1; 2; 3; 4|]
            let result = Array.duplicates xs
            assertThat (result = [||]) (tag "duplicates none" >> isTrue)
        )

        test ("Array.duplicates returns empty for empty array", fun _ ->
            let xs : int[] = [||]
            let result = Array.duplicates xs
            assertThat (result = [||]) (tag "duplicates empty" >> isTrue)
        )

        test ("Array.duplicates throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.duplicates xs |> ignore)
        )

        test ("Array.duplicates does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 2; 4|]
            let original = xs.Duplicate()
            let _ = Array.duplicates xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.duplicatesBy returns duplicates by projection", fun _ ->
            let xs = [|"apple"; "be"; "Art"; "big"|]
            let result = Array.duplicatesBy (fun (s:string) -> Char.ToLower(s.[0])) xs
            assertThat (result = [|"Art"; "big"|]) (tag "duplicatesBy" >> isTrue)
        )

        test ("Array.duplicatesBy returns empty when no duplicates", fun _ ->
            let xs = [|"apple"; "be"; "cat"|]
            let result = Array.duplicatesBy (fun (s:string) -> s.[0]) xs
            assertThat (result = [||]) (tag "duplicatesBy none" >> isTrue)
        )

        test ("Array.duplicatesBy throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.duplicatesBy String.length xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Error handling functions--------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.failIfEmpty returns array when not empty", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfEmpty "should not throw" xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "failIfEmpty returns same array" >> isTrue)
        )

        test ("Array.failIfEmpty throws on empty array", fun _ ->
            let xs : int[] = [||]
            assertThat (fun () -> Array.failIfEmpty "is empty" xs |> ignore) (tag "should throw on empty" >> throws)
        )

        test ("Array.failIfLessThan returns array when count is sufficient", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfLessThan 3 "should not throw" xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "failIfLessThan returns same array" >> isTrue)
        )

        test ("Array.failIfLessThan throws when count is insufficient", fun _ ->
            let xs = [|1; 2|]
            assertThat (fun () -> Array.failIfLessThan 3 "too few" xs |> ignore) (tag "should throw when too few" >> throws)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Search/Match functions----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.matches returns true when array matches at index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.matches [|2; 3|] 1 xs) (tag "matches at index 1" >> isTrue)
        )

        test ("Array.matches returns false when array does not match", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.matches [|2; 4|] 1 xs) (tag "does not match" >> isFalse)
        )

        test ("Array.matches returns false when not enough items remain", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.matches [|3; 4|] 2 xs) (tag "not enough items" >> isFalse)
        )

        test ("Array.matches throws on invalid index", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.matches [|1|] -1 xs |> ignore)
            throwsArg (fun () -> Array.matches [|1|] 3 xs |> ignore)
        )

        test ("Array.matches throws on null searchFor", fun _ ->
            let xs = [|1; 2; 3|]
            throwsNull (fun () -> Array.matches null 0 xs |> ignore)
        )

        test ("Array.matches does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = Array.matches [|2; 3|] 1 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.findValue finds value in range", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.findValue 3 0 4 xs) (tag "findValue" >> isEqualTo 2)
        )

        test ("Array.findValue returns -1 when not found", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.findValue 6 0 4 xs) (tag "findValue not found" >> isEqualTo -1)
        )

        test ("Array.findValue respects range bounds", fun _ ->
            let xs = [|1; 2; 3; 2; 5|]
            assertThat (Array.findValue 2 2 2 xs) (tag "findValue not in range" >> isEqualTo -1)
            assertThat (Array.findValue 2 2 4 xs) (tag "findValue in range" >> isEqualTo 3)
        )

        test ("Array.findValue throws on invalid range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findValue 1 -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findValue 1 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findValue 1 2 1 xs |> ignore)
        )

        test ("Array.findValue throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findValue 1 0 0 xs |> ignore)
        )

        test ("Array.findLastValue finds last value in range", fun _ ->
            let xs = [|1; 2; 3; 2; 5|]
            assertThat (Array.findLastValue 2 0 4 xs) (tag "findLastValue" >> isEqualTo 3)
        )

        test ("Array.findLastValue returns -1 when not found", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.findLastValue 6 0 4 xs) (tag "findLastValue not found" >> isEqualTo -1)
        )

        test ("Array.findLastValue throws on invalid range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findLastValue 1 -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findLastValue 1 0 3 xs |> ignore)
        )

        test ("Array.findLastValue throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findLastValue 1 0 0 xs |> ignore)
        )

        test ("findlast", fun _ ->
            let i =  "abcde".ToCharArray()
            let l =  i.LastIndex
            let ab =  "ab".ToCharArray()
            let de =  "de".ToCharArray()

            assertThat (
                ( 0 = Array.findArray ab 0 l i)
                && (-1 = Array.findArray ab 1 l i)
                && ( 0 = Array.findLastArray ab 0 l i)
                && (-1 = Array.findLastArray ab 1 l i)
                && (-1 = Array.findArray de 0 (l-1)  i)
                && ( 3 = Array.findArray de 0 l   i)
                && (-1 = Array.findLastArray de 0 (l-1)  i)
                && ( 3 = Array.findLastArray de 0 l   i)
            ) (tag "findArray and findLastArray" >> isTrue)
        )

        test ("Array.findArray throws on invalid range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findArray [|1|] -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findArray [|1|] 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findArray [|1|] 2 1 xs |> ignore)
        )

        test ("Array.findArray throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findArray [|1|] 0 0 xs |> ignore)
        )

        test ("Array.findLastArray throws on invalid range", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findLastArray [|1|] -1 2 xs |> ignore)
            throwsArg (fun () -> Array.findLastArray [|1|] 0 3 xs |> ignore)
            throwsArg (fun () -> Array.findLastArray [|1|] 2 1 xs |> ignore)
        )

        test ("Array.findLastArray throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.findLastArray [|1|] 0 0 xs |> ignore)
        )

    ])
