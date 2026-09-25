namespace Tests

open ArrayT

open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open System
open System.Collections.Generic


/// Tests to ensure JS (Fable) and .NET runtimes behave the same way
/// for all functions that contain FABLE_COMPILER_JAVASCRIPT directives.
module FableParity =
 open Exceptions

#nowarn "44" // to test the obsolete asString alias
 let private obsoleteAsString (xs: int[]) : string = xs.asString
#warnon "44"

 let tests =
    testList ("Fable Parity Tests", [

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------asResizeArray-------------------------------------------------------------
        // In Fable JS: unbox cast (same underlying JS array)
        // In .NET: new ResizeArray is allocated and elements copied
        //--------------------------------------------------------------------------------------------------------------------

        test ("asResizeArray with reference type array", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 3" >> isEqualTo 3)
            assertThat result.[0] (tag "first element" >> isEqualTo "a")
            assertThat result.[1] (tag "second element" >> isEqualTo "b")
            assertThat result.[2] (tag "third element" >> isEqualTo "c")
        )

        test ("asResizeArray with empty reference type array", fun _ ->
            let xs : string[] = [||]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "empty array should give empty ResizeArray" >> isEqualTo 0)
        )

        test ("asResizeArray with single element", fun _ ->
            let xs = [| "only" |]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 1" >> isEqualTo 1)
            assertThat result.[0] (tag "single element" >> isEqualTo "only")
        )

        test ("asResizeArray with null strings in array", fun _ ->
            let xs = [| "a"; null; "c" |]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 3" >> isEqualTo 3)
            assertThat result.[0] (tag "first element" >> isEqualTo "a")
            assertThat result.[1] (tag "null element should be preserved" >> isNull)
            assertThat result.[2] (tag "third element" >> isEqualTo "c")
        )

        test ("asResizeArray with duplicate reference elements", fun _ ->
            let xs = [| "dup"; "dup"; "other"; "dup" |]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 4" >> isEqualTo 4)
            assertThat result.[0] (tag "first dup" >> isEqualTo "dup")
            assertThat result.[1] (tag "second dup" >> isEqualTo "dup")
            assertThat result.[3] (tag "fourth dup" >> isEqualTo "dup")
        )

        test ("asResizeArray throws on null array", fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.asResizeArray xs |> ignore)
        )

        test ("asResizeArray preserves element order", fun _ ->
            let xs = [| "z"; "a"; "m"; "b" |]
            let result = Array.asResizeArray xs
            assertThat result.[0] (tag "order[0]" >> isEqualTo "z")
            assertThat result.[1] (tag "order[1]" >> isEqualTo "a")
            assertThat result.[2] (tag "order[2]" >> isEqualTo "m")
            assertThat result.[3] (tag "order[3]" >> isEqualTo "b")
        )

        test ("asResizeArray with large reference array", fun _ ->
            let xs = Array.init 1000 (fun i -> sprintf "item%d" i)
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 1000" >> isEqualTo 1000)
            assertThat result.[0] (tag "first" >> isEqualTo "item0")
            assertThat result.[999] (tag "last" >> isEqualTo "item999")
        )

        test ("asResizeArray result is iterable", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            let mutable sum = ""
            for item in result do
                sum <- sum + item
            assertThat sum (tag "iteration should work" >> isEqualTo "abc")
        )

        test ("asResizeArray result supports Add", fun _ ->
            let xs = [| "a"; "b" |]
            let result = Array.asResizeArray xs
            result.Add("c")
            assertThat result.Count (tag "count after Add" >> isEqualTo 3)
            assertThat result.[2] (tag "added element" >> isEqualTo "c")
        )

        test ("asResizeArray result supports Remove", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            let removed = result.Remove("b")
            assertThat removed (tag "Remove should return true" >> isTrue)
            assertThat result.Count (tag "count after Remove" >> isEqualTo 2)
        )

        test ("asResizeArray result supports IndexOf", fun _ ->
            let xs = [| "first"; "second"; "third" |]
            let result = Array.asResizeArray xs
            assertThat (result.IndexOf("second")) (tag "IndexOf second" >> isEqualTo 1)
            assertThat (result.IndexOf("missing")) (tag "IndexOf missing" >> isEqualTo -1)
        )

        test ("asResizeArray result supports Contains", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            assertThat (result.Contains("b")) (tag "Contains existing" >> isTrue)
            assertThat (result.Contains("d")) (tag "Contains missing" >> isFalse)
        )

        test ("asResizeArray with all null elements", fun _ ->
            let xs : string[] = [| null; null; null |]
            let result = Array.asResizeArray xs
            assertThat result.Count (tag "count should be 3" >> isEqualTo 3)
            assertThat result.[0] (tag "null[0]" >> isNull)
            assertThat result.[1] (tag "null[1]" >> isNull)
            assertThat result.[2] (tag "null[2]" >> isNull)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------toResizeArray (for comparison)---------------------------------------------
        // toResizeArray always allocates a copy; verify same observable behavior as asResizeArray
        //--------------------------------------------------------------------------------------------------------------------

        test ("toResizeArray with reference type array gives same results as asResizeArray", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let fromTo = Array.toResizeArray xs
            let fromAs = Array.asResizeArray xs
            assertThat fromTo.Count (tag "counts should match" >> isEqualTo fromAs.Count)
            for i in 0 .. xs.Length - 1 do
                assertThat fromTo.[i] (tag (sprintf "element[%d]" i) >> isEqualTo fromAs.[i])
        )

        test ("toResizeArray with empty reference type array", fun _ ->
            let xs : string[] = [||]
            let result = Array.toResizeArray xs
            assertThat result.Count (tag "empty" >> isEqualTo 0)
        )

        test ("toResizeArray with null strings", fun _ ->
            let xs = [| null; "b"; null |]
            let result = Array.toResizeArray xs
            assertThat result.[0] (tag "null[0]" >> isNull)
            assertThat result.[1] (tag "b[1]" >> isEqualTo "b")
            assertThat result.[2] (tag "null[2]" >> isNull)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------badGetExn (tested via Get/Idx extensions)----------------------------------
        // In Fable JS: uses "'T" string for type name
        // In .NET: uses typeof<'T>.Name
        // Both should throw IndexOutOfRangeException with same behavior
        //--------------------------------------------------------------------------------------------------------------------

        test ("Get throws on empty int array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Get 0 |> ignore)
        )

        test ("Get throws on empty string array", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Get 0 |> ignore)
        )

        test ("Get throws on out of range for value types", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Get 3 |> ignore)
            throwsRange (fun () -> xs.Get -1 |> ignore)
            throwsRange (fun () -> xs.Get 100 |> ignore)
        )

        test ("Get throws on out of range for reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            throwsRange (fun () -> xs.Get 3 |> ignore)
            throwsRange (fun () -> xs.Get -1 |> ignore)
            throwsRange (fun () -> xs.Get 100 |> ignore)
        )

        test ("Get works for value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (xs.Get 0) (tag "int[0]" >> isEqualTo 10)
            assertThat (xs.Get 1) (tag "int[1]" >> isEqualTo 20)
            assertThat (xs.Get 2) (tag "int[2]" >> isEqualTo 30)
        )

        test ("Get works for reference types", fun _ ->
            let xs = [| "hello"; "world" |]
            assertThat (xs.Get 0) (tag "string[0]" >> isEqualTo "hello")
            assertThat (xs.Get 1) (tag "string[1]" >> isEqualTo "world")
        )

        test ("Get works for float (value type)", fun _ ->
            let xs = [| 1.5; 2.5; 3.5 |]
            assertThat (xs.Get 0) (tag "float[0]" >> isEqualTo 1.5)
            assertThat (xs.Get 2) (tag "float[2]" >> isEqualTo 3.5)
        )

        test ("Get works for bool (value type)", fun _ ->
            let xs = [| true; false; true |]
            assertThat (xs.Get 0) (tag "bool[0]" >> isEqualTo true)
            assertThat (xs.Get 1) (tag "bool[1]" >> isEqualTo false)
        )

        test ("Get with single element array", fun _ ->
            let xs = [| 42 |]
            assertThat (xs.Get 0) (tag "single element" >> isEqualTo 42)
            throwsRange (fun () -> xs.Get 1 |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Error message contents-----------------------------------------------------
        // The descriptive messages are the main feature of this library, so check their content on .NET and in Fable.
        // (In Fable the type name is shown as 'T, so only the item count is checked.)
        //--------------------------------------------------------------------------------------------------------------------

        test ("Get and Idx error messages include the bad index, the item count and the content", fun _ ->
            let xs = [| 0 .. 88 |]
            throwsWith ["Array.Get: Can't get index 99 from:"; "with 89 items:"; "  0: 0"; "  ..."; "  88: 88"] (fun () -> xs.Get 99 |> ignore)
            throwsWith ["Array.Idx: Can't get index -1 from:"; "with 89 items:"] (fun () -> xs.Idx -1 |> ignore)
        )

        test ("Set error message includes the bad index, the value and the item count", fun _ ->
            let xs = [| 0 .. 88 |]
            throwsWith ["Array.Set: Can't set index 99 to 7 on:"; "with 89 items:"] (fun () -> xs.Set 99 7)
        )

        test ("DebugIdx error messages include the bad index and the item count", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsWith ["Array.DebugIdx.[i]: Can't get index 3 from:"; "with 3 items:"] (fun () -> xs.DebugIdx.[3] |> ignore)
            throwsWith ["Array.DebugIdx.[i]: Can't set index -1 to 9 on:"; "with 3 items:"] (fun () -> xs.DebugIdx.[-1] <- 9)
        )

        test ("GetNeg, GetLooped, Last and SecondLast error messages include the bad index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsWith ["Array.GetNeg: Can't get index -4 from:"; "with 3 items:"] (fun () -> xs.GetNeg -4 |> ignore)
            let empty : int[] = [||]
            throwsWith ["Array.GetLooped: Can't get index 5 from:"; "empty array"] (fun () -> empty.GetLooped 5 |> ignore)
            throwsWith ["Array.Last: Can't get index -1 from:"; "empty array"] (fun () -> empty.Last |> ignore)
            throwsWith ["Array.SecondLast: Can't get index -1 from:"; "with 1 item:"] (fun () -> [| 1 |].SecondLast |> ignore)
        )

        test ("Array.get and Array.set error messages include the bad index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsWith ["Array.Get: Can't get index 3 from:"; "with 3 items:"] (fun () -> Array.get 3 xs |> ignore)
            throwsWith ["Array.Set: Can't set index 3 to 0 on:"; "with 3 items:"] (fun () -> Array.set 3 0 xs)
        )

        test ("null input error message names the function", fun _ ->
            let xs : int[] = null
            throwsWith ["Array.first: input is null!"] (fun () -> Array.first xs |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------badSetExn (tested via Set extension)--------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Set throws on empty int array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Set 0 99)
        )

        test ("Set throws on empty string array", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Set 0 "x")
        )

        test ("Set throws on out of range for value types", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Set 3 99)
            throwsRange (fun () -> xs.Set -1 99)
        )

        test ("Set throws on out of range for reference types", fun _ ->
            let xs = [| "a"; "b" |]
            throwsRange (fun () -> xs.Set 2 "x")
            throwsRange (fun () -> xs.Set -1 "x")
        )

        test ("Set works for value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.Set 1 99
            assertThat xs.[1] (tag "set int" >> isEqualTo 99)
        )

        test ("Set works for reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.Set 1 "z"
            assertThat xs.[1] (tag "set string" >> isEqualTo "z")
        )

        test ("Set null to reference type array", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.Set 1 null
            assertThat xs.[1] (tag "set null" >> isNull)
        )

        test ("Set with single element array", fun _ ->
            let xs = [| 42 |]
            xs.Set 0 99
            assertThat xs.[0] (tag "set single element" >> isEqualTo 99)
            throwsRange (fun () -> xs.Set 1 99)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Idx extension (uses badGetExn)---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Idx throws on empty array value type", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Idx 0 |> ignore)
        )

        test ("Idx throws on empty array reference type", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Idx 0 |> ignore)
        )

        test ("Idx works for value and reference types", fun _ ->
            let ints = [| 1; 2; 3 |]
            assertThat (ints.Idx 0) (tag "int Idx 0" >> isEqualTo 1)
            assertThat (ints.Idx 2) (tag "int Idx 2" >> isEqualTo 3)
            let strs = [| "x"; "y" |]
            assertThat (strs.Idx 0) (tag "string Idx 0" >> isEqualTo "x")
            assertThat (strs.Idx 1) (tag "string Idx 1" >> isEqualTo "y")
        )

        test ("Idx out of range", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Idx 3 |> ignore)
            throwsRange (fun () -> xs.Idx -1 |> ignore)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------DebugIndexer (uses badGetExn/badSetExn)------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("DebugIdx get on empty value type array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] |> ignore)
        )

        test ("DebugIdx get on empty reference type array", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] |> ignore)
        )

        test ("DebugIdx set on empty value type array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] <- 1)
        )

        test ("DebugIdx set on empty reference type array", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] <- "x")
        )

        test ("DebugIdx get works for value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (xs.DebugIdx.[0]) (tag "int DebugIdx[0]" >> isEqualTo 10)
            assertThat (xs.DebugIdx.[2]) (tag "int DebugIdx[2]" >> isEqualTo 30)
        )

        test ("DebugIdx get works for reference types", fun _ ->
            let xs = [| "hello"; "world" |]
            assertThat (xs.DebugIdx.[0]) (tag "string DebugIdx[0]" >> isEqualTo "hello")
            assertThat (xs.DebugIdx.[1]) (tag "string DebugIdx[1]" >> isEqualTo "world")
        )

        test ("DebugIdx set works for value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.DebugIdx.[1] <- 99
            assertThat xs.[1] (tag "set int via DebugIdx" >> isEqualTo 99)
        )

        test ("DebugIdx set works for reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.DebugIdx.[1] <- "z"
            assertThat xs.[1] (tag "set string via DebugIdx" >> isEqualTo "z")
        )

        test ("DebugIdx set null on reference type", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.DebugIdx.[1] <- null
            assertThat xs.[1] (tag "set null via DebugIdx" >> isNull)
        )

        test ("DebugIdx out of range negative index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.DebugIdx.[-1] |> ignore)
            throwsRange (fun () -> xs.DebugIdx.[-1] <- 99)
        )

        test ("DebugIdx out of range positive index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.DebugIdx.[3] |> ignore)
            throwsRange (fun () -> xs.DebugIdx.[3] <- 99)
        )

        test ("DebugIdx with single element", fun _ ->
            let xs = [| "only" |]
            assertThat (xs.DebugIdx.[0]) (tag "single element get" >> isEqualTo "only")
            xs.DebugIdx.[0] <- "changed"
            assertThat xs.[0] (tag "single element set" >> isEqualTo "changed")
            throwsRange (fun () -> xs.DebugIdx.[1] |> ignore)
        )

        test ("DebugIdx Length property", fun _ ->
            let xs = [| 1; 2; 3 |]
            assertThat xs.DebugIdx.Length (tag "DebugIdx.Length" >> isEqualTo 3)
            let empty : int[] = [||]
            assertThat empty.DebugIdx.Length (tag "DebugIdx.Length empty" >> isEqualTo 0)
        )

        test ("DebugIdx Array property", fun _ ->
            let xs = [| 1; 2; 3 |]
            let arr = xs.DebugIdx.Array
            assertThat (Object.ReferenceEquals(xs, arr)) (tag "DebugIdx.Array should return same array" >> isTrue)
        )

        test ("DebugIdx with duplicates", fun _ ->
            let xs = [| 5; 5; 5 |]
            assertThat (xs.DebugIdx.[0]) (tag "dup[0]" >> isEqualTo 5)
            assertThat (xs.DebugIdx.[1]) (tag "dup[1]" >> isEqualTo 5)
            assertThat (xs.DebugIdx.[2]) (tag "dup[2]" >> isEqualTo 5)
            xs.DebugIdx.[1] <- 99
            assertThat xs.[0] (tag "dup unchanged[0]" >> isEqualTo 5)
            assertThat xs.[1] (tag "dup changed[1]" >> isEqualTo 99)
            assertThat xs.[2] (tag "dup unchanged[2]" >> isEqualTo 5)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------fail function (tested via module functions)--------------------------------
        // fail is used internally by various Array module functions
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.first throws on empty int array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)
        )

        test ("Array.first throws on empty string array", fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)
        )

        test ("Array.first throws on null", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.first xs |> ignore)
        )

        test ("Array.first works for value types", fun _ ->
            assertThat (Array.first [| 10; 20; 30 |]) (tag "first int" >> isEqualTo 10)
        )

        test ("Array.first works for reference types", fun _ ->
            assertThat (Array.first [| "a"; "b" |]) (tag "first string" >> isEqualTo "a")
        )

        test ("Array.first with single element", fun _ ->
            assertThat (Array.first [| 42 |]) (tag "first single int" >> isEqualTo 42)
            assertThat (Array.first [| "only" |]) (tag "first single string" >> isEqualTo "only")
        )

        test ("Array.first with null element at position 0", fun _ ->
            let xs = [| null; "b" |]
            assertThat (Array.first xs) (tag "first null element" >> isNull)
        )

        test ("Array.secondLast throws on arrays with less than 2 items", fun _ ->
            throwsRange (fun () -> Array.secondLast ([||] : int[]) |> ignore)
            throwsRange (fun () -> Array.secondLast [| 1 |] |> ignore)
        )

        test ("Array.secondLast works for value and reference types", fun _ ->
            assertThat (Array.secondLast [| 1; 2; 3 |]) (tag "secondLast int" >> isEqualTo 2)
            assertThat (Array.secondLast [| "a"; "b"; "c" |]) (tag "secondLast string" >> isEqualTo "b")
        )

        test ("Array.thirdLast throws on arrays with less than 3 items", fun _ ->
            throwsRange (fun () -> Array.thirdLast ([||] : int[]) |> ignore)
            throwsRange (fun () -> Array.thirdLast [| 1 |] |> ignore)
            throwsRange (fun () -> Array.thirdLast [| 1; 2 |] |> ignore)
        )

        test ("Array.thirdLast works for value and reference types", fun _ ->
            assertThat (Array.thirdLast [| 1; 2; 3; 4 |]) (tag "thirdLast int" >> isEqualTo 2)
            assertThat (Array.thirdLast [| "a"; "b"; "c" |]) (tag "thirdLast string" >> isEqualTo "a")
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------GetNeg/SetNeg extensions--------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("GetNeg on value types", fun _ ->
            let xs = [| 10; 20; 30; 40; 50 |]
            assertThat (xs.GetNeg -1) (tag "GetNeg -1" >> isEqualTo 50)
            assertThat (xs.GetNeg -2) (tag "GetNeg -2" >> isEqualTo 40)
            assertThat (xs.GetNeg -5) (tag "GetNeg -5" >> isEqualTo 10)
            assertThat (xs.GetNeg 0) (tag "GetNeg 0" >> isEqualTo 10)
            assertThat (xs.GetNeg 4) (tag "GetNeg 4" >> isEqualTo 50)
        )

        test ("GetNeg on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            assertThat (xs.GetNeg -1) (tag "GetNeg -1 ref" >> isEqualTo "c")
            assertThat (xs.GetNeg -3) (tag "GetNeg -3 ref" >> isEqualTo "a")
            assertThat (xs.GetNeg 0) (tag "GetNeg 0 ref" >> isEqualTo "a")
        )

        test ("GetNeg throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.GetNeg 0 |> ignore)
            throwsRange (fun () -> xs.GetNeg -1 |> ignore)
        )

        test ("GetNeg throws on too-large negative index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.GetNeg -4 |> ignore)
        )

        test ("GetNeg throws on too-large positive index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.GetNeg 3 |> ignore)
        )

        test ("GetNeg with single element", fun _ ->
            let xs = [| 42 |]
            assertThat (xs.GetNeg 0) (tag "GetNeg 0 single" >> isEqualTo 42)
            assertThat (xs.GetNeg -1) (tag "GetNeg -1 single" >> isEqualTo 42)
            throwsRange (fun () -> xs.GetNeg -2 |> ignore)
            throwsRange (fun () -> xs.GetNeg 1 |> ignore)
        )

        test ("SetNeg on value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetNeg -1 99
            assertThat xs.[2] (tag "SetNeg -1" >> isEqualTo 99)
            xs.SetNeg -3 88
            assertThat xs.[0] (tag "SetNeg -3" >> isEqualTo 88)
            xs.SetNeg 1 77
            assertThat xs.[1] (tag "SetNeg 1" >> isEqualTo 77)
        )

        test ("SetNeg on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.SetNeg -1 "z"
            assertThat xs.[2] (tag "SetNeg -1 ref" >> isEqualTo "z")
        )

        test ("SetNeg throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.SetNeg 0 1)
            throwsRange (fun () -> xs.SetNeg -1 1)
        )

        test ("SetNeg throws on too-large negative index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.SetNeg -4 99)
        )

        test ("SetNeg throws on too-large positive index", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.SetNeg 3 99)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------GetLooped/SetLooped extensions---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("GetLooped on value types wraps around", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (xs.GetLooped 0) (tag "GetLooped 0" >> isEqualTo 10)
            assertThat (xs.GetLooped 3) (tag "GetLooped 3 (wraps)" >> isEqualTo 10)
            assertThat (xs.GetLooped 4) (tag "GetLooped 4" >> isEqualTo 20)
            assertThat (xs.GetLooped 5) (tag "GetLooped 5" >> isEqualTo 30)
            assertThat (xs.GetLooped 6) (tag "GetLooped 6 (wraps again)" >> isEqualTo 10)
        )

        test ("GetLooped on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            assertThat (xs.GetLooped 3) (tag "GetLooped wraps ref" >> isEqualTo "a")
            assertThat (xs.GetLooped 4) (tag "GetLooped wraps ref+1" >> isEqualTo "b")
        )

        test ("GetLooped with negative indices", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (xs.GetLooped -1) (tag "GetLooped -1" >> isEqualTo 30)
            assertThat (xs.GetLooped -2) (tag "GetLooped -2" >> isEqualTo 20)
            assertThat (xs.GetLooped -3) (tag "GetLooped -3" >> isEqualTo 10)
            assertThat (xs.GetLooped -4) (tag "GetLooped -4 (wraps)" >> isEqualTo 30)
        )

        test ("GetLooped throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.GetLooped 0 |> ignore)
        )

        test ("GetLooped with single element", fun _ ->
            let xs = [| 42 |]
            assertThat (xs.GetLooped 0) (tag "GetLooped single 0" >> isEqualTo 42)
            assertThat (xs.GetLooped 1) (tag "GetLooped single 1" >> isEqualTo 42)
            assertThat (xs.GetLooped -1) (tag "GetLooped single -1" >> isEqualTo 42)
            assertThat (xs.GetLooped 1000) (tag "GetLooped single 1000" >> isEqualTo 42)
        )

        test ("SetLooped on value types wraps around", fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetLooped 3 99
            assertThat xs.[0] (tag "SetLooped 3 wraps to 0" >> isEqualTo 99)
        )

        test ("SetLooped with negative index", fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetLooped -1 99
            assertThat xs.[2] (tag "SetLooped -1" >> isEqualTo 99)
        )

        test ("SetLooped throws on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.SetLooped 0 99)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Last/First/Second/Third extensions with various types----------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Last on reference types with null elements", fun _ ->
            let xs = [| "a"; null |]
            assertThat xs.Last (tag "Last should be null" >> isNull)
        )

        test ("First on reference types with null elements", fun _ ->
            let xs = [| null; "b" |]
            assertThat xs.First (tag "First should be null" >> isNull)
        )

        test ("Last/First on float array", fun _ ->
            let xs = [| 1.1; 2.2; 3.3 |]
            assertThat xs.First (tag "First float" >> isEqualTo 1.1)
            assertThat xs.Last (tag "Last float" >> isEqualTo 3.3)
        )

        test ("Last/First on bool array", fun _ ->
            let xs = [| true; false |]
            assertThat xs.First (tag "First bool" >> isEqualTo true)
            assertThat xs.Last (tag "Last bool" >> isEqualTo false)
        )

        test ("Last/First setters with reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.First <- "z"
            xs.Last <- "y"
            assertThat xs.[0] (tag "First set ref" >> isEqualTo "z")
            assertThat xs.[2] (tag "Last set ref" >> isEqualTo "y")
        )

        test ("Last/First setters with null", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.First <- null
            xs.Last <- null
            assertThat xs.[0] (tag "First set null" >> isNull)
            assertThat xs.[2] (tag "Last set null" >> isNull)
        )

        test ("SecondLast/ThirdLast on reference types", fun _ ->
            let xs = [| "a"; "b"; "c"; "d" |]
            assertThat xs.SecondLast (tag "SecondLast ref" >> isEqualTo "c")
            assertThat xs.ThirdLast (tag "ThirdLast ref" >> isEqualTo "b")
        )

        test ("Second/Third on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            assertThat xs.Second (tag "Second ref" >> isEqualTo "b")
            assertThat xs.Third (tag "Third ref" >> isEqualTo "c")
        )

        test ("FirstAndOnly on value types", fun _ ->
            assertThat [| 42 |].FirstAndOnly (tag "FirstAndOnly int" >> isEqualTo 42)
            throwsRange (fun () -> [||].FirstAndOnly |> ignore)
            throwsRange (fun () -> [| 1; 2 |].FirstAndOnly |> ignore)
        )

        test ("FirstAndOnly error message says exactly one item is expected", fun _ ->
            throwsWith ["Array.FirstAndOnly: Expected exactly one item in:"; "2 items"] (fun () -> [| 1; 2 |].FirstAndOnly |> ignore)
            let empty : int[] = [||]
            throwsWith ["Array.FirstAndOnly: Expected exactly one item in:"; "empty array"] (fun () -> empty.FirstAndOnly |> ignore)
        )

        test ("FirstAndOnly on reference types", fun _ ->
            assertThat [| "only" |].FirstAndOnly (tag "FirstAndOnly string" >> isEqualTo "only")
            let xs : string[] = [||]
            throwsRange (fun () -> xs.FirstAndOnly |> ignore)
        )

        test ("FirstAndOnly with null element", fun _ ->
            let xs : string[] = [| null |]
            assertThat xs.FirstAndOnly (tag "FirstAndOnly null" >> isNull)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Slice with various types---------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Slice on reference type array", fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(1, 3)
            assertThat (result = [| "b"; "c"; "d" |]) (tag "Slice ref" >> isTrue)
        )

        test ("Slice with negative indices on reference types", fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(-3, -1)
            assertThat (result = [| "c"; "d"; "e" |]) (tag "Slice neg ref" >> isTrue)
        )

        test ("Slice single element", fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(1, 1)
            assertThat (result = [| 2 |]) (tag "Slice single element" >> isTrue)
        )

        test ("Slice full array", fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(0, 2)
            assertThat (result = [| 1; 2; 3 |]) (tag "Slice full" >> isTrue)
        )

        test ("Slice with negative start positive end", fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(-2, 4)
            assertThat (result = [| "d"; "e" |]) (tag "Slice mixed indices ref" >> isTrue)
        )

        test ("Slice throws on invalid indices", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Slice(3, 4) |> ignore)
            throwsRange (fun () -> xs.Slice(0, 5) |> ignore)
        )

        test ("Slice with start equal to end returns single element", fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(2, 2)
            assertThat (result = [| 3 |]) (tag "Slice single at end" >> isTrue)
        )

        test ("Slice throws when start is clearly after end", fun _ ->
            let xs = [| 1; 2; 3; 4; 5 |]
            throwsRange (fun () -> xs.Slice(3, 1) |> ignore)
        )

        test ("Slice with null elements in reference array", fun _ ->
            let xs = [| "a"; null; "c"; null; "e" |]
            let result = xs.Slice(1, 3)
            assertThat result.Length (tag "Slice null elements count" >> isEqualTo 3)
            assertThat result.[0] (tag "Slice null[0]" >> isNull)
            assertThat result.[1] (tag "Slice null[1]" >> isEqualTo "c")
            assertThat result.[2] (tag "Slice null[2]" >> isNull)
        )

        test ("Slice with duplicates", fun _ ->
            let xs = [| 5; 5; 5; 5; 5 |]
            let result = xs.Slice(1, 3)
            assertThat (result = [| 5; 5; 5 |]) (tag "Slice duplicates" >> isTrue)
        )

        test ("Slice does not modify original", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let original = Array.copy xs
            let result = xs.Slice(0, 1)
            result.[0] <- "z"
            assertThat xs.[0] (tag "original not modified by Slice mutation" >> isEqualTo "a")
            assertThat (xs = original) (tag "original unchanged" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Array module Get/Set (use badGetExn/badSetExn)-----------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.get on value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (Array.get 0 xs) (tag "module get int[0]" >> isEqualTo 10)
            assertThat (Array.get 2 xs) (tag "module get int[2]" >> isEqualTo 30)
        )

        test ("Array.get on reference types", fun _ ->
            let xs = [| "hello"; "world" |]
            assertThat (Array.get 0 xs) (tag "module get string[0]" >> isEqualTo "hello")
        )

        test ("Array.get on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.get 0 xs |> ignore)
        )

        test ("Array.get on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.get 0 xs |> ignore)
            let ys : string[] = null
            throwsNull (fun () -> Array.get 0 ys |> ignore)
        )

        test ("Array.set on value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            Array.set 1 99 xs
            assertThat xs.[1] (tag "module set int" >> isEqualTo 99)
        )

        test ("Array.set on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.set 1 "z" xs
            assertThat xs.[1] (tag "module set string" >> isEqualTo "z")
        )

        test ("Array.set null on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.set 1 null xs
            assertThat xs.[1] (tag "module set null" >> isNull)
        )

        test ("Array.set on empty array", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.set 0 99 xs)
        )

        test ("Array.set on null array", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.set 0 99 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Array.getNeg/setNeg (module level)------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.getNeg on value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            assertThat (Array.getNeg -1 xs) (tag "getNeg -1" >> isEqualTo 30)
            assertThat (Array.getNeg -3 xs) (tag "getNeg -3" >> isEqualTo 10)
            assertThat (Array.getNeg 0 xs) (tag "getNeg 0" >> isEqualTo 10)
            assertThat (Array.getNeg 2 xs) (tag "getNeg 2" >> isEqualTo 30)
        )

        test ("Array.getNeg on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            assertThat (Array.getNeg -1 xs) (tag "getNeg -1 ref" >> isEqualTo "c")
            assertThat (Array.getNeg 0 xs) (tag "getNeg 0 ref" >> isEqualTo "a")
        )

        test ("Array.getNeg throws on empty", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.getNeg 0 xs |> ignore)
        )

        test ("Array.getNeg throws on null", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getNeg 0 xs |> ignore)
        )

        test ("Array.getNeg throws out of range", fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> Array.getNeg -4 xs |> ignore)
            throwsRange (fun () -> Array.getNeg 3 xs |> ignore)
        )

        test ("Array.setNeg on value types", fun _ ->
            let xs = [| 10; 20; 30 |]
            Array.setNeg -1 99 xs
            assertThat xs.[2] (tag "setNeg -1" >> isEqualTo 99)
        )

        test ("Array.setNeg on reference types", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.setNeg -1 "z" xs
            assertThat xs.[2] (tag "setNeg -1 ref" >> isEqualTo "z")
        )

        test ("Array.setNeg throws on empty", fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.setNeg 0 99 xs)
        )

        test ("Array.setNeg throws on null", fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setNeg 0 99 xs)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Duplicate/Copy (should produce independent copies)-------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Duplicate on reference type array creates independent copy", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let dup = xs.Duplicate()
            dup.[0] <- "z"
            assertThat xs.[0] (tag "original not modified" >> isEqualTo "a")
            assertThat dup.[0] (tag "dup modified" >> isEqualTo "z")
        )

        test ("Duplicate on value type array creates independent copy", fun _ ->
            let xs = [| 1; 2; 3 |]
            let dup = xs.Duplicate()
            dup.[0] <- 99
            assertThat xs.[0] (tag "original not modified" >> isEqualTo 1)
        )

        test ("Duplicate on empty array", fun _ ->
            let xs : int[] = [||]
            let dup = xs.Duplicate()
            assertThat dup.Length (tag "dup empty" >> isEqualTo 0)
        )

        test ("Copy on reference type array creates independent copy", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let cp = xs.Copy()
            cp.[0] <- "z"
            assertThat xs.[0] (tag "original not modified" >> isEqualTo "a")
        )

        test ("Duplicate preserves null elements", fun _ ->
            let xs = [| "a"; null; "c" |]
            let dup = xs.Duplicate()
            assertThat dup.[1] (tag "null preserved in dup" >> isNull)
        )

        test ("Duplicate with duplicates", fun _ ->
            let xs = [| 5; 5; 5 |]
            let dup = xs.Duplicate()
            assertThat (dup = xs) (tag "dup equals original" >> isTrue)
            dup.[0] <- 99
            assertThat xs.[0] (tag "original not modified" >> isEqualTo 5)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------FailIfEmpty/FailIfLessThan with types--------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("FailIfEmpty on reference type array", fun _ ->
            let xs = [| "a"; "b" |]
            let result = xs.FailIfEmpty("ok")
            assertThat (Object.ReferenceEquals(xs, result)) (tag "returns same ref" >> isTrue)
        )

        test ("FailIfEmpty on empty reference type array", fun _ ->
            let xs : string[] = [||]
            assertThat (fun () -> xs.FailIfEmpty("empty") |> ignore) (tag "throws on empty ref" >> throws)
        )

        test ("FailIfEmpty on empty value type array", fun _ ->
            let xs : float[] = [||]
            assertThat (fun () -> xs.FailIfEmpty("empty") |> ignore) (tag "throws on empty float" >> throws)
        )

        test ("FailIfLessThan on reference type array", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = xs.FailIfLessThan(2, "ok")
            assertThat (Object.ReferenceEquals(xs, result)) (tag "returns same ref" >> isTrue)
        )

        test ("FailIfLessThan on insufficient reference type array", fun _ ->
            let xs = [| "a" |]
            assertThat (fun () -> xs.FailIfLessThan(3, "too few") |> ignore) (tag "throws on too few ref" >> throws)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------AsString / ToString------------------------------------------------------
        // Both are inline so that the type name can be resolved by reflection in Fable too.
        // Both should produce the same output format on .NET and in Fable.
        //--------------------------------------------------------------------------------------------------------------------

        test ("AsString on int array", fun _ ->
            let xs = [| 1; 2; 3 |]
            let s = xs.AsString
            assertThat (s.Contains("3")) (tag "should contain count" >> isTrue)
        )

        test ("AsString on empty int array", fun _ ->
            let xs : int[] = [||]
            let s = xs.AsString
            assertThat (s.Contains("empty")) (tag "should contain empty" >> isTrue)
        )

        test ("AsString on string array", fun _ ->
            let xs = [| "hello"; "world" |]
            let s = xs.AsString
            assertThat (s.Contains("2")) (tag "should contain count" >> isTrue)
        )

        test ("AsString on single element", fun _ ->
            let xs = [| 42 |]
            let s = xs.AsString
            assertThat (s.Contains("1")) (tag "should contain count 1" >> isTrue)
        )

        test ("obsolete asString alias gives the same result as AsString", fun _ ->
            let xs = [| 1; 2; 3; 4; 5; 6; 7 |]
            assertThat (obsoleteAsString xs) (tag "asString = AsString" >> isEqualTo xs.AsString)
        )

        test ("ToString(n) on int array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 |]
            let s = xs.ToString(3).Replace("\r\n", "\n").Trim()
            assertThat (s.Contains("10 items")) (tag "should mention 10 items" >> isTrue)
            assertThat (s.Contains("0: 1")) (tag "should contain first entry" >> isTrue)
            assertThat (s.Contains("...")) (tag "should contain ellipsis for truncated" >> isTrue)
        )

        test ("ToString(Int32.MaxValue) prints all entries once", fun _ ->
            let xs = [| 1; 2; 3 |]
            let s = xs.ToString(Int32.MaxValue).Replace("\r\n", "\n").Trim()
            assertThat s (tag "all entries, no ellipsis" >> isEqualTo "array<Int32> with 3 items:\n  0: 1\n  1: 2\n  2: 3")
        )

        test ("ToString(n) on empty array", fun _ ->
            let xs : int[] = [||]
            let s = xs.ToString(5)
            assertThat (s.Contains("empty")) (tag "should contain empty" >> isTrue)
        )

        test ("ToString(n) on string array", fun _ ->
            let xs = [| "alpha"; "beta"; "gamma" |]
            let s = xs.ToString(10).Replace("\r\n", "\n").Trim()
            assertThat (s.Contains("3 items")) (tag "should mention 3 items" >> isTrue)
            assertThat (s.Contains("alpha")) (tag "should contain first" >> isTrue)
            assertThat (s.Contains("gamma")) (tag "should contain last" >> isTrue)
        )

        test ("ToString(0) shows no entries", fun _ ->
            let xs = [| 1; 2; 3 |]
            let s = xs.ToString(0)
            assertThat (s.Contains("0:")) (tag "should not show entries" >> isFalse)
            assertThat (s.Contains("3 items")) (tag "should mention count" >> isTrue)
        )

        test ("ToString(n) on single element", fun _ ->
            let xs = [| 42 |]
            let s = xs.ToString(5).Replace("\r\n", "\n").Trim()
            assertThat (s.Contains("1 item")) (tag "should mention 1 item" >> isTrue)
            assertThat (s.Contains("42")) (tag "should contain the value" >> isTrue)
        )

        test ("ToString(n) with null elements", fun _ ->
            let xs = [| "a"; null; "c" |]
            let s = xs.ToString(5).Replace("\r\n", "\n")
            assertThat (s.Contains("3 items")) (tag "should mention count" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------IsEmpty/IsNotEmpty/HasItems/IsSingleton with types-------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("IsEmpty on reference type empty array", fun _ ->
            let xs : string[] = [||]
            assertThat xs.IsEmpty (tag "IsEmpty ref" >> isTrue)
        )

        test ("IsEmpty on reference type non-empty array", fun _ ->
            let xs = [| "a" |]
            assertThat xs.IsEmpty (tag "not IsEmpty ref" >> isFalse)
        )

        test ("IsNotEmpty on reference type", fun _ ->
            assertThat [| "a" |].IsNotEmpty (tag "IsNotEmpty ref" >> isTrue)
            let xs : string[] = [||]
            assertThat xs.IsNotEmpty (tag "not IsNotEmpty ref" >> isFalse)
        )

        test ("HasItems on reference type", fun _ ->
            assertThat [| "a" |].HasItems (tag "HasItems ref" >> isTrue)
            let xs : string[] = [||]
            assertThat xs.HasItems (tag "not HasItems ref" >> isFalse)
        )

        test ("IsSingleton on reference type", fun _ ->
            assertThat [| "a" |].IsSingleton (tag "IsSingleton ref" >> isTrue)
            let xs : string[] = [||]
            assertThat xs.IsSingleton (tag "not IsSingleton empty ref" >> isFalse)
            assertThat [| "a"; "b" |].IsSingleton (tag "not IsSingleton multi ref" >> isFalse)
        )

        test ("IsEmpty on float array", fun _ ->
            let xs : float[] = [||]
            assertThat xs.IsEmpty (tag "IsEmpty float" >> isTrue)
        )

        test ("IsSingleton on float array", fun _ ->
            assertThat [| 1.0 |].IsSingleton (tag "IsSingleton float" >> isTrue)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------LastIndex with various types-----------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("LastIndex on reference type array", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            assertThat xs.LastIndex (tag "LastIndex ref" >> isEqualTo 2)
        )

        test ("LastIndex on empty reference type array", fun _ ->
            let xs : string[] = [||]
            assertThat xs.LastIndex (tag "LastIndex empty ref" >> isEqualTo -1)
        )

        test ("LastIndex on float array", fun _ ->
            let xs = [| 1.0; 2.0 |]
            assertThat xs.LastIndex (tag "LastIndex float" >> isEqualTo 1)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Arrays with duplicate elements---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Get with duplicate elements returns correct one", fun _ ->
            let xs = [| 5; 5; 5 |]
            assertThat (xs.Get 0) (tag "dup get[0]" >> isEqualTo 5)
            assertThat (xs.Get 1) (tag "dup get[1]" >> isEqualTo 5)
            assertThat (xs.Get 2) (tag "dup get[2]" >> isEqualTo 5)
        )

        test ("Set with duplicate elements sets correct one", fun _ ->
            let xs = [| 5; 5; 5 |]
            xs.Set 1 99
            assertThat xs.[0] (tag "dup set unchanged[0]" >> isEqualTo 5)
            assertThat xs.[1] (tag "dup set changed[1]" >> isEqualTo 99)
            assertThat xs.[2] (tag "dup set unchanged[2]" >> isEqualTo 5)
        )

        test ("GetNeg with duplicate reference elements", fun _ ->
            let xs = [| "x"; "x"; "x" |]
            assertThat (xs.GetNeg -1) (tag "dup GetNeg -1" >> isEqualTo "x")
            assertThat (xs.GetNeg -3) (tag "dup GetNeg -3" >> isEqualTo "x")
        )

        test ("Slice with all-same elements", fun _ ->
            let xs = [| 7; 7; 7; 7; 7 |]
            let result = xs.Slice(1, 3)
            assertThat (result = [| 7; 7; 7 |]) (tag "Slice all same" >> isTrue)
        )

        test ("Last/First/Second/Third with duplicates", fun _ ->
            let xs = [| 5; 5; 5; 5 |]
            assertThat xs.First (tag "First dup" >> isEqualTo 5)
            assertThat xs.Second (tag "Second dup" >> isEqualTo 5)
            assertThat xs.Third (tag "Third dup" >> isEqualTo 5)
            assertThat xs.Last (tag "Last dup" >> isEqualTo 5)
        )

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Module-level functions with reference types---------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        test ("Array.getNeg with null elements in reference array", fun _ ->
            let xs = [| null; "b"; null |]
            assertThat (Array.getNeg 0 xs) (tag "getNeg null[0]" >> isNull)
            assertThat (Array.getNeg -1 xs) (tag "getNeg null[-1]" >> isNull)
            assertThat (Array.getNeg 1 xs) (tag "getNeg non-null" >> isEqualTo "b")
        )

        test ("Array.setNeg null on reference array", fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.setNeg -1 null xs
            assertThat xs.[2] (tag "setNeg null ref" >> isNull)
        )

        test ("Array.get with null elements", fun _ ->
            let xs = [| null; "b"; null |]
            assertThat (Array.get 0 xs) (tag "get null[0]" >> isNull)
            assertThat (Array.get 1 xs) (tag "get non-null[1]" >> isEqualTo "b")
        )

        test ("Array.set null element", fun _ ->
            let xs = [| "a"; "b" |]
            Array.set 0 null xs
            assertThat xs.[0] (tag "set null via module" >> isNull)
        )

    ])
