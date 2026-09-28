namespace Tests

open ArrayT

open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open System
open System.Collections.Generic


/// Compares by Value only, but Equals also checks the Name.
/// So items that compare as equal are not equal by '='.
[<CustomEquality; CustomComparison>]
type TieItem =
    { Value: int; Name: string }
    override this.Equals(o) =
        match o with
        | :? TieItem as i -> i.Value = this.Value && i.Name = this.Name
        | _ -> false
    override this.GetHashCode() = hash (this.Value, this.Name)
    interface IComparable with
        member this.CompareTo(o) = compare this.Value (o :?> TieItem).Value


module Module2 =
 open Exceptions

 /// Shows a float so that NaN, -0.0 and +0.0 can be told apart, since -0.0 = +0.0 is true.
 let private show (x: float) =
    if Double.IsNaN x then "NaN"
    elif x = 0.0 then (if 1.0 / x < 0.0 then "-0" else "+0")
    else string x

 let private show32 (x: float32) = show (float x)

 /// For a stable sort: NaN comes last, other values ascending. -0.0 and +0.0 are equal.
 let private nanLastAsc (a: float) (b: float) =
    match Double.IsNaN a, Double.IsNaN b with
    | true, true -> 0
    | true, false -> 1
    | false, true -> -1
    | false, false -> compare a b

 /// For a stable sort: NaN comes last, other values descending. -0.0 and +0.0 are equal.
 let private nanLastDesc (a: float) (b: float) =
    match Double.IsNaN a, Double.IsNaN b with
    | true, true -> 0
    | true, false -> 1
    | false, true -> -1
    | false, false -> compare b a

 /// Reference for the IEEE 754:2019 'minimumNumber': NaN is skipped and -0.0 is smaller than +0.0.
 let private refMinNumber (vs: float list) =
    match vs |> List.filter (fun v -> not (Double.IsNaN v)) with
    | [] -> nan
    | ns -> ns |> List.reduce (fun a b -> if b < a || (b = a && show b = "-0") then b else a)

 /// Reference for the IEEE 754:2019 'maximumNumber': NaN is skipped and +0.0 is bigger than -0.0.
 let private refMaxNumber (vs: float list) =
    match vs |> List.filter (fun v -> not (Double.IsNaN v)) with
    | [] -> nan
    | ns -> ns |> List.reduce (fun a b -> if b > a || (b = a && show b = "+0") then b else a)

 /// All lists of length 1 to 4 made of NaN, -0.0, +0.0, -1.0 and 1.0.
 let private nanInputs =
    let values = [nan; -0.0; 0.0; -1.0; 1.0]
    [ for a in values do
        [a]
        for b in values do
            [a; b]
            for c in values do
                [a; b; c]
                for d in values do
                    [a; b; c; d] ]

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
            let original = xs.Copy()
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
            let original = xs.Copy()
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
            let original = xs.Copy()
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
            let original = xs.Copy()
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

        test ("Array.sliceNeg returns slice with positive indices", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.sliceNeg 1 3 xs = [|2; 3; 4|]) (tag "sliceNeg 1 3" >> isTrue)
        )

        test ("Array.sliceNeg returns slice with negative indices", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.sliceNeg -2 -1 xs = [|4; 5|]) (tag "sliceNeg -2 -1" >> isTrue)
            assertThat (Array.sliceNeg 1 -2 xs = [|2; 3; 4|]) (tag "sliceNeg 1 -2" >> isTrue)
        )

        test ("Array.sliceNeg returns single item", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.sliceNeg 2 2 xs = [|3|]) (tag "sliceNeg 2 2" >> isTrue)
        )

        test ("Array.sliceNeg throws on invalid range", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            throwsRange (fun () -> Array.sliceNeg 3 1 xs |> ignore)
        )

        test ("Array.sliceNeg returns empty array when end index is one less than start index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.sliceNeg 3 2 xs = [||]) (tag "sliceNeg 3 2" >> isTrue)
            assertThat (Array.sliceNeg 0 -6 xs = [||]) (tag "sliceNeg 0 -6, like trim 0 5" >> isTrue)
        )

        test ("Array.sliceNeg error messages name the offending index", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            throwsWith ["Start index 3 is bigger than end index 1"] (fun () -> Array.sliceNeg 3 1 xs |> ignore)
            throwsWith ["End index -99 is out of range"] (fun () -> Array.sliceNeg 1 -99 xs |> ignore)
            throwsWith ["End index -6 is out of range"] (fun () -> Array.sliceNeg 1 -6 xs |> ignore)
            throwsWith ["End index 5 is out of range"] (fun () -> Array.sliceNeg 1 5 xs |> ignore)
            throwsWith ["Start index -6 is out of range"] (fun () -> Array.sliceNeg -6 2 xs |> ignore)
        )

        test ("Array.sliceNeg throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.sliceNeg 0 1 xs |> ignore)
        )

        test ("Array.sliceNeg does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Copy()
            let _ = Array.sliceNeg 1 3 xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.sliceIdx uses an inclusive end index", fun _ ->
            let xs = [|0 .. 4|]
            assertThat (Array.sliceIdx 0 0 xs) (tag "sliceIdx 0 0" >> isEqualTo [|0|])
            assertThat (Array.sliceIdx 1 3 xs) (tag "sliceIdx 1 3" >> isEqualTo [|1; 2; 3|])
            assertThat (Array.sliceIdx 2 4 xs) (tag "sliceIdx 2 4" >> isEqualTo [|2; 3; 4|])
        )

        test ("Array.sliceIdx rejects invalid ranges", fun _ ->
            let xs = [|0 .. 4|]
            throwsRange (fun () -> Array.sliceIdx -1 2 xs |> ignore)
            throwsRange (fun () -> Array.sliceIdx 0 5 xs |> ignore)
            throwsRange (fun () -> Array.sliceIdx 3 2 xs |> ignore)
            throwsRange (fun () -> Array.sliceIdx -3 -1 xs |> ignore)
            throwsNull  (fun () -> Array.sliceIdx 0 1 (null: int[]) |> ignore)
        )

        test ("Array.sliceLooped", fun _ ->
            let xs = [|1 .. 10|]
            assertThat (Array.sliceLooped -3 -1 xs) (tag "sliceLooped -3 -1" >> isEqualTo [|8; 9; 10|])
            assertThat (Array.sliceLooped 10 11 xs) (tag "sliceLooped 10 11" >> isEqualTo [|1; 2|])
            assertThat (Array.sliceLooped 5 4 xs) (tag "sliceLooped 5 4" >> isEqualTo [||])
            throwsNull (fun () -> Array.sliceLooped 0 1 (null: int[]) |> ignore)
        )

        test ("Array.trim trims from start and end", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            assertThat (Array.trim 1 1 xs = [|2; 3; 4|]) (tag "trim 1 1" >> isTrue)
        )

        test ("Array.trim returns empty array when trimming more than length", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.trim 2 2 xs = [||]) (tag "trim 2 2" >> isTrue)
        )

        test ("Array.trim returns empty array for Int32.MaxValue trim counts", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.trim Int32.MaxValue 1 xs = [||]) (tag "trim MaxValue 1" >> isTrue)
            assertThat (Array.trim 1 Int32.MaxValue xs = [||]) (tag "trim 1 MaxValue" >> isTrue)
            assertThat (Array.trim Int32.MaxValue Int32.MaxValue xs = [||]) (tag "trim MaxValue MaxValue" >> isTrue)
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
            let original = xs.Copy()
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
            let original = xs.Copy()
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

        test ("Array.rotate by Int32.MinValue and MaxValue does not overflow", fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            // Int32.MinValue % 6 = -2 and Int32.MaxValue % 6 = 1
            assertThat (xs |> Array.rotate Int32.MinValue) (tag "rotate MinValue" >> isEqualTo (xs |> Array.rotate -2))
            assertThat (xs |> Array.rotate Int32.MaxValue) (tag "rotate MaxValue" >> isEqualTo (xs |> Array.rotate 1))
            assertThat (Array.rotate Int32.MinValue ([||]: int[])) (tag "rotate empty" >> isEqualTo [||])
        )

        test ("Array.rotate throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.rotate 1 xs |> ignore)
        )

        test ("Array.rotate does not modify input array", fun _ ->
            let xs = [|0; 1; 2; 3; 4; 5|]
            let original = xs.Copy()
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
            let original = xs.Copy()
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
            let original = xs.Copy()
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
            let original = xs.Copy()
            let _ = Array.rotateUpTill (fun i -> i = 7) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.rotate...TillLast and iThisNext error messages name the right function", fun _ ->
            let xs = [|1; 2; 3|]
            throwsWith ["Array.rotateUpTillLast:"] (fun () -> Array.rotateUpTillLast (fun i -> i = 99) xs |> ignore)
            throwsWith ["Array.rotateDownTillLast:"] (fun () -> Array.rotateDownTillLast (fun i -> i = 99) xs |> ignore)
            throwsWith ["Array.iThisNext: input has less than two items"] (fun () -> Array.iThisNext [| 1 |] |> ignore)
        )

        test ("Array.rotateUpTillLast", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            assertThat (xs |> Array.rotateUpTillLast(fun i -> i = 7) = [|5; 0; 7; 2; 3; 7 |]) (tag "rotateUpTillLast" >> isTrue)
            throwsArg (fun () -> xs |> Array.rotateUpTillLast (fun i -> i = 99) |> ignore)
        )

        test ("Array.rotateUpTillLast does not modify input array", fun _ ->
            let xs = [|0; 7; 2; 3; 7; 5|]
            let original = xs.Copy()
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
            let original = xs.Copy()
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

        test ("Array min3 and max3 functions match a stable sort, also when equality disagrees with comparison", fun _ ->
            let values = [1; 2; 3]
            let inputs = [
                for a in values do
                    for b in values do
                        for c in values do
                            [a; b; c]
                            for d in values do
                                [a; b; c; d] ]
            for vs in inputs do
                let items = vs |> List.mapi (fun i v -> { Value = v; Name = string i }) |> Array.ofList
                // List.sortWith is a stable sort
                let stableIdx cmp = items |> List.ofArray |> List.indexed |> List.sortWith (fun (_, x) (_, y) -> cmp x y) |> List.map fst
                let asc  = stableIdx (fun (x: TieItem) y -> compare x.Value y.Value)
                let desc = stableIdx (fun (x: TieItem) y -> compare y.Value x.Value)
                let first3 (idx: int list) = idx.[0], idx.[1], idx.[2]
                let names (x: TieItem, y: TieItem, z: TieItem) = [x.Name; y.Name; z.Name]
                let expectedNames (idx: int list) = [ for i in idx.[0..2] -> string i ]
                assertThat (Array.min3IndicesBy id items) (tag $"min3IndicesBy {vs}" >> isEqualTo (first3 asc))
                assertThat (Array.max3IndicesBy id items) (tag $"max3IndicesBy {vs}" >> isEqualTo (first3 desc))
                assertThat (names (Array.min3 items))     (tag $"min3 {vs}"   >> isEqualTo (expectedNames asc))
                assertThat (names (Array.max3 items))     (tag $"max3 {vs}"   >> isEqualTo (expectedNames desc))
                assertThat (names (Array.min3By id items)) (tag $"min3By {vs}" >> isEqualTo (expectedNames asc))
                assertThat (names (Array.max3By id items)) (tag $"max3By {vs}" >> isEqualTo (expectedNames desc))
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

        test ("Min and max error messages name the public function", fun _ ->
            let empty : string[] = [||]
            throwsWith ["Array.minIndexBy: Count must be at least one"] (fun () -> Array.minIndexBy String.length empty |> ignore)
            throwsWith ["Array.max2: Count must be at least two"] (fun () -> Array.max2 [| 1 |] |> ignore)
            throwsWith ["Array.min2IndicesBy: Count must be at least two"] (fun () -> Array.min2IndicesBy id [| 1 |] |> ignore)
            throwsWith ["Array.min3: Count must be at least three"] (fun () -> Array.min3 [| 1; 2 |] |> ignore)
            throwsWith ["Array.max3By: Count must be at least three"] (fun () -> Array.max3By id [| 1; 2 |] |> ignore)
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

        test ("Array.min_IEEE754 and max_IEEE754 propagate NaN, minNumber and maxNumber skip it", fun _ ->
            assertThat (show -0.0) (tag "the -0.0 literal is negative zero" >> isEqualTo "-0")
            // the table from https://github.com/dotnet/fsharp/issues/13207#issuecomment-1194411950 , inputs in either order
            let table = [
                // input        min    max    minNumber maxNumber
                [3.0; 7.0],  ("3",   "7",   "3",   "7")
                [3.0; nan],  ("NaN", "NaN", "3",   "3")
                [nan; nan],  ("NaN", "NaN", "NaN", "NaN")
                [-0.0; 0.0], ("-0",  "+0",  "-0",  "+0") ]
            for input, (mi, ma, miN, maN) in table do
                for vs in [input; List.rev input] do
                    let xs = Array.ofList vs
                    assertThat (show (Array.min_IEEE754 xs)) (tag $"min_IEEE754 {vs}" >> isEqualTo mi)
                    assertThat (show (Array.max_IEEE754 xs)) (tag $"max_IEEE754 {vs}" >> isEqualTo ma)
                    assertThat (show (Array.minNumber xs))   (tag $"minNumber {vs}"   >> isEqualTo miN)
                    assertThat (show (Array.maxNumber xs))   (tag $"maxNumber {vs}"   >> isEqualTo maN)
                    let fs = Array.map float32 xs
                    assertThat (show32 (Array.min_IEEE754 fs)) (tag $"float32 min_IEEE754 {vs}" >> isEqualTo mi)
                    assertThat (show32 (Array.max_IEEE754 fs)) (tag $"float32 max_IEEE754 {vs}" >> isEqualTo ma)
                    assertThat (show32 (Array.minNumber fs))   (tag $"float32 minNumber {vs}"   >> isEqualTo miN)
                    assertThat (show32 (Array.maxNumber fs))   (tag $"float32 maxNumber {vs}"   >> isEqualTo maN)
        )

        test ("Array.min_IEEE754, max_IEEE754, minNumber and maxNumber match a reference with NaN, -0.0 and +0.0 at every position", fun _ ->
            for vs in nanInputs do
                let hasNaN = vs |> List.exists Double.IsNaN
                let minN = refMinNumber vs
                let maxN = refMaxNumber vs
                let xs = Array.ofList vs
                assertThat (show (Array.min_IEEE754 xs)) (tag $"min_IEEE754 {vs}" >> isEqualTo (if hasNaN then "NaN" else show minN))
                assertThat (show (Array.max_IEEE754 xs)) (tag $"max_IEEE754 {vs}" >> isEqualTo (if hasNaN then "NaN" else show maxN))
                assertThat (show (Array.minNumber xs))   (tag $"minNumber {vs}"   >> isEqualTo (show minN))
                assertThat (show (Array.maxNumber xs))   (tag $"maxNumber {vs}"   >> isEqualTo (show maxN))
                let fs = Array.map float32 xs
                assertThat (show32 (Array.min_IEEE754 fs)) (tag $"float32 min_IEEE754 {vs}" >> isEqualTo (if hasNaN then "NaN" else show minN))
                assertThat (show32 (Array.max_IEEE754 fs)) (tag $"float32 max_IEEE754 {vs}" >> isEqualTo (if hasNaN then "NaN" else show maxN))
                assertThat (show32 (Array.minNumber fs))   (tag $"float32 minNumber {vs}"   >> isEqualTo (show minN))
                assertThat (show32 (Array.maxNumber fs))   (tag $"float32 maxNumber {vs}"   >> isEqualTo (show maxN))
        )

        test ("Array.min_IEEE754, max_IEEE754, minNumber and maxNumber work on any comparable type, keep the first of equal items and check their input", fun _ ->
            assertThat (Array.min_IEEE754 [| 3; 1; 2 |]) (tag "min_IEEE754 int" >> isEqualTo 1)
            assertThat (Array.max_IEEE754 [| "b"; "c"; "a" |]) (tag "max_IEEE754 string" >> isEqualTo "c")
            assertThat (Array.minNumber [| 3; 1; 2 |]) (tag "minNumber int" >> isEqualTo 1)
            assertThat (Array.maxNumber [| "b"; "c"; "a" |]) (tag "maxNumber string" >> isEqualTo "c")
            let ties = [| { Value = 2; Name = "a" }; { Value = 1; Name = "b" }; { Value = 1; Name = "c" }; { Value = 2; Name = "d" } |]
            assertThat (Array.min_IEEE754 ties).Name (tag "min_IEEE754 ties" >> isEqualTo "b")
            assertThat (Array.max_IEEE754 ties).Name (tag "max_IEEE754 ties" >> isEqualTo "a")
            assertThat (Array.minNumber ties).Name   (tag "minNumber ties"   >> isEqualTo "b")
            assertThat (Array.maxNumber ties).Name   (tag "maxNumber ties"   >> isEqualTo "a")
            let empty : float[] = [||]
            let nullArr : float[] = null
            throwsWith ["Array.min_IEEE754: Count must be at least one"] (fun () -> Array.min_IEEE754 empty |> ignore)
            throwsWith ["Array.max_IEEE754: Count must be at least one"] (fun () -> Array.max_IEEE754 empty |> ignore)
            throwsWith ["Array.minNumber: Count must be at least one"] (fun () -> Array.minNumber empty |> ignore)
            throwsWith ["Array.maxNumber: Count must be at least one"] (fun () -> Array.maxNumber empty |> ignore)
            throwsWith ["Array.minNumberBy: Count must be at least one"] (fun () -> Array.minNumberBy id empty |> ignore)
            throwsWith ["Array.maxNumberBy: Count must be at least one"] (fun () -> Array.maxNumberBy id empty |> ignore)
            throwsNull (fun () -> Array.min_IEEE754 nullArr |> ignore)
            throwsNull (fun () -> Array.max_IEEE754 nullArr |> ignore)
            throwsNull (fun () -> Array.minNumber nullArr |> ignore)
            throwsNull (fun () -> Array.maxNumber nullArr |> ignore)
            throwsNull (fun () -> Array.minNumberBy id nullArr |> ignore)
            throwsNull (fun () -> Array.maxNumberBy id nullArr |> ignore)
        )

        test ("Array.minNumberBy, maxNumberBy and the other By functions ignore NaN keys at any position", fun _ ->
            let xs = [| nan; 3.0; nan; 1.0; 2.0; nan |]
            assertThat (Array.minNumberBy id xs)   (tag "minNumberBy"   >> isEqualTo 1.0)
            assertThat (Array.maxNumberBy id xs)   (tag "maxNumberBy"   >> isEqualTo 3.0)
            assertThat (Array.minIndexBy id xs)    (tag "minIndexBy"    >> isEqualTo 3)
            assertThat (Array.maxIndexBy id xs)    (tag "maxIndexBy"    >> isEqualTo 1)
            assertThat (Array.min2By id xs)        (tag "min2By"        >> isEqualTo (1.0, 2.0))
            assertThat (Array.max2IndicesBy id xs) (tag "max2IndicesBy" >> isEqualTo (1, 4))
            assertThat (Array.min3IndicesBy id xs) (tag "min3IndicesBy" >> isEqualTo (3, 4, 1))
            assertThat (Array.max3By id xs)        (tag "max3By"        >> isEqualTo (3.0, 2.0, 1.0))
            assertThat (Array.min3 xs)             (tag "min3"          >> isEqualTo (1.0, 2.0, 3.0))
            assertThat (Array.max2 xs)             (tag "max2"          >> isEqualTo (3.0, 2.0))
            // unlike min_IEEE754 and max_IEEE754, which propagate NaN
            assertThat (Double.IsNaN (Array.min_IEEE754 xs)) (tag "min_IEEE754 propagates NaN" >> isTrue)
            assertThat (Double.IsNaN (Array.max_IEEE754 xs)) (tag "max_IEEE754 propagates NaN" >> isTrue)
            // not enough keys that are not NaN: NaN keys come last, in their original order
            let ys = [| nan; 5.0; nan |]
            assertThat (Array.min3IndicesBy id ys) (tag "min3IndicesBy one number" >> isEqualTo (1, 0, 2))
            assertThat (Array.max2IndicesBy id ys) (tag "max2IndicesBy one number" >> isEqualTo (1, 0))
            // all keys are NaN: the first element
            let nans = [| ("a", nan); ("b", nan); ("c", nan) |]
            assertThat (Array.minNumberBy snd nans |> fst) (tag "minNumberBy all NaN" >> isEqualTo "a")
            assertThat (Array.maxNumberBy snd nans |> fst) (tag "maxNumberBy all NaN" >> isEqualTo "a")
            assertThat (Array.minIndexBy snd nans)         (tag "minIndexBy all NaN"  >> isEqualTo 0)
            // float32 keys
            let fs = [| nanf; 2.0f; nanf; -1.0f |]
            assertThat (Array.minNumberBy id fs) (tag "float32 minNumberBy" >> isEqualTo -1.0f)
            assertThat (Array.maxNumberBy id fs) (tag "float32 maxNumberBy" >> isEqualTo 2.0f)
            // like ResizeArray.minBy the projection is not called for a single element
            let calls = ref 0
            let one = Array.minNumberBy (fun (x: float) -> calls.Value <- calls.Value + 1; x) [| 7.0 |]
            assertThat (one, calls.Value) (tag "minNumberBy single element" >> isEqualTo (7.0, 0))
        )

        test ("Array By functions and min2, max2, min3, max3 rank NaN last, matching a stable sort", fun _ ->
            for vs in nanInputs do
                let n = vs.Length
                // the index is in the item, to see which of several equal keys was returned
                let items = vs |> List.mapi (fun i v -> (i, v)) |> Array.ofList
                let key (_: int, v: float) = v
                let idx (i: int, _: float) = i
                // List.sortWith is a stable sort
                let stableIdx cmp = [0 .. n - 1] |> List.sortWith (fun i j -> cmp vs.[i] vs.[j])
                let asc = stableIdx nanLastAsc
                let desc = stableIdx nanLastDesc
                let values (idxs: int list) = idxs |> List.map (fun i -> show vs.[i])
                assertThat (Array.minIndexBy key items)        (tag $"minIndexBy {vs}"  >> isEqualTo asc.[0])
                assertThat (Array.maxIndexBy key items)        (tag $"maxIndexBy {vs}"  >> isEqualTo desc.[0])
                assertThat (idx (Array.minNumberBy key items)) (tag $"minNumberBy {vs}" >> isEqualTo asc.[0])
                assertThat (idx (Array.maxNumberBy key items)) (tag $"maxNumberBy {vs}" >> isEqualTo desc.[0])
                let xs = Array.ofList vs
                if n >= 2 then
                    assertThat (Array.min2IndicesBy key items) (tag $"min2IndicesBy {vs}" >> isEqualTo (asc.[0], asc.[1]))
                    assertThat (Array.max2IndicesBy key items) (tag $"max2IndicesBy {vs}" >> isEqualTo (desc.[0], desc.[1]))
                    let a, b = Array.min2By key items
                    assertThat (idx a, idx b) (tag $"min2By {vs}" >> isEqualTo (asc.[0], asc.[1]))
                    let a, b = Array.max2By key items
                    assertThat (idx a, idx b) (tag $"max2By {vs}" >> isEqualTo (desc.[0], desc.[1]))
                    let a, b = Array.min2 xs
                    assertThat [show a; show b] (tag $"min2 {vs}" >> isEqualTo (values asc.[0..1]))
                    let a, b = Array.max2 xs
                    assertThat [show a; show b] (tag $"max2 {vs}" >> isEqualTo (values desc.[0..1]))
                if n >= 3 then
                    assertThat (Array.min3IndicesBy key items) (tag $"min3IndicesBy {vs}" >> isEqualTo (asc.[0], asc.[1], asc.[2]))
                    assertThat (Array.max3IndicesBy key items) (tag $"max3IndicesBy {vs}" >> isEqualTo (desc.[0], desc.[1], desc.[2]))
                    let a, b, c = Array.min3By key items
                    assertThat (idx a, idx b, idx c) (tag $"min3By {vs}" >> isEqualTo (asc.[0], asc.[1], asc.[2]))
                    let a, b, c = Array.max3By key items
                    assertThat (idx a, idx b, idx c) (tag $"max3By {vs}" >> isEqualTo (desc.[0], desc.[1], desc.[2]))
                    let a, b, c = Array.min3 xs
                    assertThat [show a; show b; show c] (tag $"min3 {vs}" >> isEqualTo (values asc.[0..2]))
                    let a, b, c = Array.max3 xs
                    assertThat [show a; show b; show c] (tag $"max3 {vs}" >> isEqualTo (values desc.[0..2]))
        )

        #if !FABLE_COMPILER
        test ("Array.minNumberBy and maxNumberBy also ignore keys that contain NaN, like a tuple", fun _ ->
            let xs = [| (nan, 0); (2.0, 1); (nan, 2); (1.0, 3) |]
            assertThat (Array.minNumberBy id xs) (tag "minNumberBy tuple" >> isEqualTo (1.0, 3))
            assertThat (Array.maxNumberBy id xs) (tag "maxNumberBy tuple" >> isEqualTo (2.0, 1))
            assertThat (Array.min3IndicesBy id xs) (tag "min3IndicesBy tuple" >> isEqualTo (3, 1, 0))
        )
        #endif

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
            let original = xs.Copy()
            let _ = Array.countIf (fun x -> x > 1) xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Filter by index-----------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.filteri ", fun _ ->
            let arr = [|'a';'b';'c'|]
            let result = arr|> Array.filteri (fun i _ -> i % 2 = 0)
            assertThat (result = [|'a';'c'|]) (tag "filteri" >> isTrue)
            assertThat (Object.ReferenceEquals(arr, result)) (tag "filteri should return new array" >> isFalse)
        )

        test ("Array.filteri passes the index and the element", fun _ ->
            let arr = [|10; 11; 12; 13|]
            let seen = ResizeArray()
            let result = arr |> Array.filteri (fun i x -> seen.Add((i, x)); i > 0 && x % 2 = 0)
            assertThat result (tag "filteri by index and element" >> isEqualTo [|12|])
            assertThat (seen.ToArray()) (tag "filteri called with index and element" >> isEqualTo [|(0, 10); (1, 11); (2, 12); (3, 13)|])
        )

        test ("Array.filteri returns empty for all false predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ _ -> false) xs
            assertThat (result = [||]) (tag "filteri all false" >> isTrue)
        )

        test ("Array.filteri returns all for all true predicate", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.filteri (fun _ _ -> true) xs
            assertThat (result = [|1; 2; 3|]) (tag "filteri all true" >> isTrue)
        )

        test ("Array.filteri throws on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.filteri (fun _ _ -> true) xs |> ignore)
        )

        test ("Array.tryFindIndexi and findIndexi use zero-based indices", fun _ ->
            let xs = [|10; 20; 30|]
            assertThat (xs |> Array.tryFindIndexi (fun i x -> i = 0 && x = 10)) (tag "tryFindIndexi first" >> isEqualTo (Some 0))
            assertThat (xs |> Array.tryFindIndexi (fun i x -> i = 2 && x = 30)) (tag "tryFindIndexi last" >> isEqualTo (Some 2))
            assertThat (xs |> Array.tryFindIndexi (fun i x -> i = x)) (tag "tryFindIndexi none" >> isEqualTo None)
            assertThat (([||]: int[]) |> Array.tryFindIndexi (fun _ _ -> true)) (tag "tryFindIndexi empty" >> isEqualTo None)
            assertThat (xs |> Array.findIndexi (fun i x -> i = 1 && x = 20)) (tag "findIndexi" >> isEqualTo 1)
            assertThat (xs |> Array.findIndexi (fun _ x -> x > 10)) (tag "findIndexi returns the first match" >> isEqualTo 1)
        )

        test ("Array.mapPrevNext", fun _ ->
            let combineAdjacent prev next = prev + "-" + next
            let merge current prevResult nextResult = prevResult + ":" + current + ":" + nextResult
            let result = Array.mapPrevNext combineAdjacent merge [|"a"; "b"; "c"; "d"|]
            assertThat result (tag "mapPrevNext loops" >> isEqualTo [|"d-a:a:a-b"; "a-b:b:b-c"; "b-c:c:c-d"; "c-d:d:d-a"|])
            assertThat (Array.mapPrevNext combineAdjacent merge [|"a"|]) (tag "mapPrevNext single" >> isEqualTo [|"a-a:a:a-a"|])
            assertThat (Array.mapPrevNext combineAdjacent merge [||]) (tag "mapPrevNext empty" >> isEqualTo [||])
            throwsNull (fun () -> Array.mapPrevNext combineAdjacent merge (null: string[]) |> ignore)
        )

        test ("Array.headAndTail", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.headAndTail xs) (tag "headAndTail" >> isEqualTo (1, [|2; 3|]))
            assertThat (Array.headAndTail [|7|]) (tag "headAndTail single" >> isEqualTo (7, [||]))
            throwsArg (fun () -> Array.headAndTail ([||]: int[]) |> ignore)
            throwsWith ["Array.headAndTail: input is empty"] (fun () -> Array.headAndTail ([||]: int[]) |> ignore)
            throwsNull (fun () -> Array.headAndTail (null: int[]) |> ignore)
        )

        test ("Array.notExists", fun _ ->
            let xs = [|1; 2; 3|]
            assertThat (Array.notExists (fun x -> x > 5) xs) (tag "notExists true" >> isTrue)
            assertThat (Array.notExists (fun x -> x = 2) xs) (tag "notExists false" >> isFalse)
            assertThat (Array.notExists (fun _ -> true) [||]) (tag "notExists empty" >> isTrue)
            throwsNull (fun () -> Array.notExists (fun _ -> true) (null: int[]) |> ignore)
        )

        test ("Array.pickBack and tryPickBack search from the end", fun _ ->
            let xs = [|1; 2; 3; 4|]
            let evenTimes10 x = if x % 2 = 0 then Some (x * 10) else None
            assertThat (Array.pickBack evenTimes10 xs) (tag "pickBack" >> isEqualTo 40)
            assertThat (Array.tryPickBack evenTimes10 xs) (tag "tryPickBack" >> isEqualTo (Some 40))
            assertThat (Array.tryPickBack evenTimes10 [|1; 3|]) (tag "tryPickBack none" >> isEqualTo None)
            CheckThrowsExn<KeyNotFoundException> (fun () -> Array.pickBack evenTimes10 [|1; 3|] |> ignore)
            throwsWith ["Array.pickBack: Key not found in 2 elements"] (fun () -> Array.pickBack evenTimes10 [|1; 3|] |> ignore)
            throwsNull (fun () -> Array.pickBack evenTimes10 (null: int[]) |> ignore)
            throwsNull (fun () -> Array.tryPickBack evenTimes10 (null: int[]) |> ignore)
        )

        test ("Array.zipDefault", fun _ ->
            let plusIndex i x = x + i
            assertThat (Array.zipDefault plusIndex [|1; 2; 10|] [|4; 5|]) (tag "first longer" >> isEqualTo [|(1, 4); (2, 5); (10, 12)|])
            assertThat (Array.zipDefault plusIndex [|4; 5|] [|1; 2; 10|]) (tag "second longer" >> isEqualTo [|(4, 1); (5, 2); (12, 10)|])
            assertThat (Array.zipDefault plusIndex [|1; 2; 3|] [|4; 5; 6|]) (tag "equal length" >> isEqualTo [|(1, 4); (2, 5); (3, 6)|])
            assertThat (Array.zipDefault (fun _ x -> x * 2) [|1; 2; 3|] [||]) (tag "second empty" >> isEqualTo [|(1, 2); (2, 4); (3, 6)|])
            assertThat (Array.zipDefault (fun _ x -> x) [||] [|4; 5|]) (tag "first empty" >> isEqualTo [|(4, 4); (5, 5)|])
            throwsNull (fun () -> Array.zipDefault plusIndex (null: int[]) [|1|] |> ignore)
            throwsNull (fun () -> Array.zipDefault plusIndex [|1|] (null: int[]) |> ignore)
        )

        test ("Array.groupByDict uses structural keys and preserves group order", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let mutable calls = 0
            let d = Array.groupByDict (fun x -> calls <- calls + 1; [|x % 2|]) xs
            assertThat d.Count (tag "structurally equal arrays form one group" >> isEqualTo 2)
            assertThat d.[[|1|]] (tag "fresh key lookup and input order" >> isEqualTo [|1; 3; 5|])
            assertThat d.[[|0|]] (tag "other group" >> isEqualTo [|2; 4|])
            assertThat calls (tag "projection runs once per item" >> isEqualTo xs.Length)
            let nested = Array.groupByDict (fun x -> Some ([|x % 2|], "key")) xs
            assertThat nested.[Some ([|1|], "key")] (tag "nested structural key" >> isEqualTo [|1; 3; 5|])
        )

        test ("Array.groupByDict rejects null and None keys", fun _ ->
            throwsNull (fun () -> Array.groupByDict (fun _ -> (null: string)) [|1|] |> ignore)
            throwsNull (fun () -> Array.groupByDict (fun _ -> (None: int option)) [|1|] |> ignore)
            throwsWith ["groupByDict"; "null or None"] (fun () -> Array.groupByDict (fun _ -> (None: int option)) [|1|] |> ignore)
        )

        test ("Array.groupByDict", fun _ ->
            let xs = [|"a"; "bb"; "c"; "dd"; "eee"|]
            let d = Array.groupByDict String.length xs
            assertThat d.Count (tag "groupByDict key count" >> isEqualTo 3)
            assertThat d.[1] (tag "groupByDict key 1" >> isEqualTo [|"a"; "c"|])
            assertThat d.[2] (tag "groupByDict key 2" >> isEqualTo [|"bb"; "dd"|])
            assertThat d.[3] (tag "groupByDict key 3" >> isEqualTo [|"eee"|])
            assertThat (Array.groupByDict String.length [||]).Count (tag "groupByDict empty" >> isEqualTo 0)
            throwsNull (fun () -> Array.groupByDict String.length (null: string[]) |> ignore)
        )

        test ("Array.findIndexi throws when not found or on null", fun _ ->
            let xs = [|10; 20; 30|]
            CheckThrowsExn<KeyNotFoundException> (fun () -> Array.findIndexi (fun i x -> i = x) xs |> ignore)
            throwsWith ["Array.findIndexi did not find"] (fun () -> Array.findIndexi (fun i x -> i = x) xs |> ignore)
            throwsNull (fun () -> Array.tryFindIndexi (fun _ _ -> true) (null: int[]) |> ignore)
            throwsNull (fun () -> Array.findIndexi (fun _ _ -> true) (null: int[]) |> ignore)
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
            let original = xs.Copy()
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
        //------------------------------------------Partitioning--------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.partitionBy and partitionWith", fun _ ->
            let classify x = if x % 2 = 0 then Choice1Of2 (x * 10) else Choice2Of2 (string x)
            let xs = [|1; 2; 3; 4; 5|]
            let evens, odds = Array.partitionBy classify xs
            assertThat evens (tag "partitionBy Choice1Of2 in order" >> isEqualTo [|20; 40|])
            assertThat odds (tag "partitionBy Choice2Of2 in order" >> isEqualTo [|"1"; "3"; "5"|])
            let evensW, oddsW = Array.partitionWith classify xs
            assertThat evensW (tag "partitionWith agrees with partitionBy" >> isEqualTo evens)
            assertThat oddsW (tag "partitionWith agrees with partitionBy 2" >> isEqualTo odds)
            let e1, e2 = Array.partitionBy classify [||]
            assertThat (e1.Length + e2.Length) (tag "partitionBy empty" >> isEqualTo 0)
            throwsNull (fun () -> Array.partitionBy classify (null: int[]) |> ignore)
            throwsNull (fun () -> Array.partitionWith classify (null: int[]) |> ignore)
        )

        test ("Array.partition3By, partition4By and partition5By", fun _ ->
            let xs = [|0 .. 9|]
            let a, b, c = xs |> Array.partition3By (fun x -> match x % 3 with 0 -> Choice1Of3 x | 1 -> Choice2Of3 (float x) | _ -> Choice3Of3 (string x))
            assertThat a (tag "partition3By 1" >> isEqualTo [|0; 3; 6; 9|])
            assertThat b (tag "partition3By 2" >> isEqualTo [|1.0; 4.0; 7.0|])
            assertThat c (tag "partition3By 3" >> isEqualTo [|"2"; "5"; "8"|])
            let a, b, c, d = xs |> Array.partition4By (fun x -> match x % 4 with 0 -> Choice1Of4 x | 1 -> Choice2Of4 x | 2 -> Choice3Of4 x | _ -> Choice4Of4 x)
            assertThat (a, b, c, d) (tag "partition4By" >> isEqualTo ([|0; 4; 8|], [|1; 5; 9|], [|2; 6|], [|3; 7|]))
            let a, b, c, d, e = xs |> Array.partition5By (fun x -> match x % 5 with 0 -> Choice1Of5 x | 1 -> Choice2Of5 x | 2 -> Choice3Of5 x | 3 -> Choice4Of5 x | _ -> Choice5Of5 x)
            assertThat (a, b, c, d, e) (tag "partition5By" >> isEqualTo ([|0; 5|], [|1; 6|], [|2; 7|], [|3; 8|], [|4; 9|]))
            throwsNull (fun () -> Array.partition3By (fun x -> Choice1Of3 x) (null: int[]) |> ignore)
            throwsNull (fun () -> Array.partition4By (fun x -> Choice1Of4 x) (null: int[]) |> ignore)
            throwsNull (fun () -> Array.partition5By (fun x -> Choice1Of5 x) (null: int[]) |> ignore)
        )

        test ("Array.partition3, partition4 and partition5 test the predicates in order", fun _ ->
            let xs = [|0 .. 9|]
            let isEven x = x % 2 = 0
            let isDiv3 x = x % 3 = 0
            let isDiv5 x = x % 5 = 0
            let isDiv7 x = x % 7 = 0
            assertThat (Array.partition3 isEven isDiv3 xs) (tag "partition3" >> isEqualTo ([|0; 2; 4; 6; 8|], [|3; 9|], [|1; 5; 7|]))
            assertThat (Array.partition4 isEven isDiv3 isDiv5 xs) (tag "partition4" >> isEqualTo ([|0; 2; 4; 6; 8|], [|3; 9|], [|5|], [|1; 7|]))
            assertThat (Array.partition5 isEven isDiv3 isDiv5 isDiv7 xs) (tag "partition5" >> isEqualTo ([|0; 2; 4; 6; 8|], [|3; 9|], [|5|], [|7|], [|1|]))
            throwsNull (fun () -> Array.partition3 isEven isDiv3 (null: int[]) |> ignore)
            throwsNull (fun () -> Array.partition4 isEven isDiv3 isDiv5 (null: int[]) |> ignore)
            throwsNull (fun () -> Array.partition5 isEven isDiv3 isDiv5 isDiv7 (null: int[]) |> ignore)
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
            let original = xs.Copy()
            let _ = Array.duplicates xs
            assertThat (xs = original) (tag "input array should not be modified" >> isTrue)
        )

        test ("Array.duplicatesBy returns duplicates by projection", fun _ ->
            let xs = [|"apple"; "be"; "Art"; "big"|]
            let result = Array.duplicatesBy (fun (s:string) -> Char.ToLower(s.[0])) xs
            assertThat (result = [|"Art"; "big"|]) (tag "duplicatesBy" >> isTrue)
        )

        test ("Array.duplicates and duplicatesBy return second occurrences in their order", fun _ ->
            assertThat (Array.duplicates [|3; 2; 2; 3|] = [|2; 3|]) (tag "duplicates order" >> isTrue)
            let r = Array.duplicatesBy String.length [| "hi"; "hey"; "go"; "bye" |]
            assertThat (r = [| "go"; "bye" |]) (tag "duplicatesBy returns second occurrences" >> isTrue)
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
            throwsArg (fun () -> Array.failIfEmpty "is empty" xs |> ignore)
        )

        test ("Array.failIfLessThan returns array when count is sufficient", fun _ ->
            let xs = [|1; 2; 3|]
            let result = Array.failIfLessThan 3 "should not throw" xs
            assertThat (Object.ReferenceEquals(xs, result)) (tag "failIfLessThan returns same array" >> isTrue)
        )

        test ("Array.failIfLessThan throws when count is insufficient", fun _ ->
            let xs = [|1; 2|]
            assertThat (fun () -> Array.failIfLessThan 3 "too few" xs |> ignore) (tag "should throw when too few" >> throws)
            throwsArg (fun () -> Array.failIfLessThan 3 "too few" xs |> ignore)
        )

        test ("Array.failIfEmpty and failIfLessThan throw on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.failIfEmpty "is null" xs |> ignore)
            throwsNull (fun () -> Array.failIfLessThan 3 "is null" xs |> ignore)
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

        test ("Array.matches throws on null searchIn", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.matches [|1|] 0 xs |> ignore)
        )

        test ("Array.matches throws a descriptive error on empty searchFor", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.matches [||] 0 xs |> ignore)
            throwsWith ["matches"; "searchFor array is empty"] (fun () -> Array.matches [||] 0 xs |> ignore)
        )

        test ("Array.matches does not modify input array", fun _ ->
            let xs = [|1; 2; 3; 4; 5|]
            let original = xs.Copy()
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

        test ("Array.findArray and findLastArray throw on null searchFor", fun _ ->
            let xs = [|1; 2; 3|]
            throwsNull (fun () -> Array.findArray null 0 2 xs |> ignore)
            throwsNull (fun () -> Array.findLastArray null 0 2 xs |> ignore)
        )

        test ("Array.findArray and findLastArray throw a descriptive error on empty searchFor", fun _ ->
            let xs = [|1; 2; 3|]
            throwsArg (fun () -> Array.findArray [||] 0 2 xs |> ignore)
            throwsArg (fun () -> Array.findLastArray [||] 0 2 xs |> ignore)
            throwsWith ["findArray"; "searchFor array is empty"] (fun () -> Array.findArray [||] 0 2 xs |> ignore)
            throwsWith ["findLastArray"; "searchFor array is empty"] (fun () -> Array.findLastArray [||] 0 2 xs |> ignore)
        )

    ])
