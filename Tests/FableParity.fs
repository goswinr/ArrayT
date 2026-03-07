namespace Tests

open ArrayT

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open Fable.Mocha
#else
open Expecto
#endif

open System
open System.Collections.Generic


/// Tests to ensure JS (Fable) and .NET runtimes behave the same way
/// for all functions that contain FABLE_COMPILER_JAVASCRIPT directives.
module FableParity =
 open Exceptions

 let tests =
    testList "Fable Parity Tests" [

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------asResizeArray-------------------------------------------------------------
        // In Fable JS: unbox cast (same underlying JS array)
        // In .NET: new ResizeArray is allocated and elements copied
        //--------------------------------------------------------------------------------------------------------------------

        testCase "asResizeArray with reference type array" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 3 "count should be 3"
            Expect.equal result.[0] "a" "first element"
            Expect.equal result.[1] "b" "second element"
            Expect.equal result.[2] "c" "third element"

        testCase "asResizeArray with empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 0 "empty array should give empty ResizeArray"

        testCase "asResizeArray with single element" <| fun _ ->
            let xs = [| "only" |]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 1 "count should be 1"
            Expect.equal result.[0] "only" "single element"

        testCase "asResizeArray with null strings in array" <| fun _ ->
            let xs = [| "a"; null; "c" |]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 3 "count should be 3"
            Expect.equal result.[0] "a" "first element"
            Expect.isTrue (isNull result.[1]) "null element should be preserved"
            Expect.equal result.[2] "c" "third element"

        testCase "asResizeArray with duplicate reference elements" <| fun _ ->
            let xs = [| "dup"; "dup"; "other"; "dup" |]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 4 "count should be 4"
            Expect.equal result.[0] "dup" "first dup"
            Expect.equal result.[1] "dup" "second dup"
            Expect.equal result.[3] "dup" "fourth dup"

        testCase "asResizeArray throws on null array" <| fun _ ->
            let xs : string[] = null
            throwsNull (fun () -> Array.asResizeArray xs |> ignore)

        testCase "asResizeArray preserves element order" <| fun _ ->
            let xs = [| "z"; "a"; "m"; "b" |]
            let result = Array.asResizeArray xs
            Expect.equal result.[0] "z" "order[0]"
            Expect.equal result.[1] "a" "order[1]"
            Expect.equal result.[2] "m" "order[2]"
            Expect.equal result.[3] "b" "order[3]"

        testCase "asResizeArray with large reference array" <| fun _ ->
            let xs = Array.init 1000 (fun i -> sprintf "item%d" i)
            let result = Array.asResizeArray xs
            Expect.equal result.Count 1000 "count should be 1000"
            Expect.equal result.[0] "item0" "first"
            Expect.equal result.[999] "item999" "last"

        testCase "asResizeArray result is iterable" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            let mutable sum = ""
            for item in result do
                sum <- sum + item
            Expect.equal sum "abc" "iteration should work"

        testCase "asResizeArray result supports Add" <| fun _ ->
            let xs = [| "a"; "b" |]
            let result = Array.asResizeArray xs
            result.Add("c")
            Expect.equal result.Count 3 "count after Add"
            Expect.equal result.[2] "c" "added element"

        testCase "asResizeArray result supports Remove" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            let removed = result.Remove("b")
            Expect.isTrue removed "Remove should return true"
            Expect.equal result.Count 2 "count after Remove"

        testCase "asResizeArray result supports IndexOf" <| fun _ ->
            let xs = [| "first"; "second"; "third" |]
            let result = Array.asResizeArray xs
            Expect.equal (result.IndexOf("second")) 1 "IndexOf second"
            Expect.equal (result.IndexOf("missing")) -1 "IndexOf missing"

        testCase "asResizeArray result supports Contains" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = Array.asResizeArray xs
            Expect.isTrue (result.Contains("b")) "Contains existing"
            Expect.isFalse (result.Contains("d")) "Contains missing"

        testCase "asResizeArray with all null elements" <| fun _ ->
            let xs : string[] = [| null; null; null |]
            let result = Array.asResizeArray xs
            Expect.equal result.Count 3 "count should be 3"
            Expect.isTrue (isNull result.[0]) "null[0]"
            Expect.isTrue (isNull result.[1]) "null[1]"
            Expect.isTrue (isNull result.[2]) "null[2]"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------toResizeArray (for comparison)---------------------------------------------
        // toResizeArray always allocates a copy; verify same observable behavior as asResizeArray
        //--------------------------------------------------------------------------------------------------------------------

        testCase "toResizeArray with reference type array gives same results as asResizeArray" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let fromTo = Array.toResizeArray xs
            let fromAs = Array.asResizeArray xs
            Expect.equal fromTo.Count fromAs.Count "counts should match"
            for i in 0 .. xs.Length - 1 do
                Expect.equal fromTo.[i] fromAs.[i] (sprintf "element[%d]" i)

        testCase "toResizeArray with empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            let result = Array.toResizeArray xs
            Expect.equal result.Count 0 "empty"

        testCase "toResizeArray with null strings" <| fun _ ->
            let xs = [| null; "b"; null |]
            let result = Array.toResizeArray xs
            Expect.isTrue (isNull result.[0]) "null[0]"
            Expect.equal result.[1] "b" "b[1]"
            Expect.isTrue (isNull result.[2]) "null[2]"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------badGetExn (tested via Get/Idx extensions)----------------------------------
        // In Fable JS: uses "'T" string for type name
        // In .NET: uses typeof<'T>.Name
        // Both should throw IndexOutOfRangeException with same behavior
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Get throws on empty int array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Get 0 |> ignore)

        testCase "Get throws on empty string array" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Get 0 |> ignore)

        testCase "Get throws on out of range for value types" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Get 3 |> ignore)
            throwsRange (fun () -> xs.Get -1 |> ignore)
            throwsRange (fun () -> xs.Get 100 |> ignore)

        testCase "Get throws on out of range for reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            throwsRange (fun () -> xs.Get 3 |> ignore)
            throwsRange (fun () -> xs.Get -1 |> ignore)
            throwsRange (fun () -> xs.Get 100 |> ignore)

        testCase "Get works for value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (xs.Get 0) 10 "int[0]"
            Expect.equal (xs.Get 1) 20 "int[1]"
            Expect.equal (xs.Get 2) 30 "int[2]"

        testCase "Get works for reference types" <| fun _ ->
            let xs = [| "hello"; "world" |]
            Expect.equal (xs.Get 0) "hello" "string[0]"
            Expect.equal (xs.Get 1) "world" "string[1]"

        testCase "Get works for float (value type)" <| fun _ ->
            let xs = [| 1.5; 2.5; 3.5 |]
            Expect.equal (xs.Get 0) 1.5 "float[0]"
            Expect.equal (xs.Get 2) 3.5 "float[2]"

        testCase "Get works for bool (value type)" <| fun _ ->
            let xs = [| true; false; true |]
            Expect.equal (xs.Get 0) true "bool[0]"
            Expect.equal (xs.Get 1) false "bool[1]"

        testCase "Get with single element array" <| fun _ ->
            let xs = [| 42 |]
            Expect.equal (xs.Get 0) 42 "single element"
            throwsRange (fun () -> xs.Get 1 |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------badSetExn (tested via Set extension)--------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Set throws on empty int array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Set 0 99)

        testCase "Set throws on empty string array" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Set 0 "x")

        testCase "Set throws on out of range for value types" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Set 3 99)
            throwsRange (fun () -> xs.Set -1 99)

        testCase "Set throws on out of range for reference types" <| fun _ ->
            let xs = [| "a"; "b" |]
            throwsRange (fun () -> xs.Set 2 "x")
            throwsRange (fun () -> xs.Set -1 "x")

        testCase "Set works for value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.Set 1 99
            Expect.equal xs.[1] 99 "set int"

        testCase "Set works for reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.Set 1 "z"
            Expect.equal xs.[1] "z" "set string"

        testCase "Set null to reference type array" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.Set 1 null
            Expect.isTrue (isNull xs.[1]) "set null"

        testCase "Set with single element array" <| fun _ ->
            let xs = [| 42 |]
            xs.Set 0 99
            Expect.equal xs.[0] 99 "set single element"
            throwsRange (fun () -> xs.Set 1 99)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Idx extension (uses badGetExn)---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Idx throws on empty array value type" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.Idx 0 |> ignore)

        testCase "Idx throws on empty array reference type" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.Idx 0 |> ignore)

        testCase "Idx works for value and reference types" <| fun _ ->
            let ints = [| 1; 2; 3 |]
            Expect.equal (ints.Idx 0) 1 "int Idx 0"
            Expect.equal (ints.Idx 2) 3 "int Idx 2"
            let strs = [| "x"; "y" |]
            Expect.equal (strs.Idx 0) "x" "string Idx 0"
            Expect.equal (strs.Idx 1) "y" "string Idx 1"

        testCase "Idx out of range" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Idx 3 |> ignore)
            throwsRange (fun () -> xs.Idx -1 |> ignore)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------DebugIndexer (uses badGetExn/badSetExn)------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "DebugIdx get on empty value type array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] |> ignore)

        testCase "DebugIdx get on empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] |> ignore)

        testCase "DebugIdx set on empty value type array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] <- 1)

        testCase "DebugIdx set on empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> xs.DebugIdx.[0] <- "x")

        testCase "DebugIdx get works for value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (xs.DebugIdx.[0]) 10 "int DebugIdx[0]"
            Expect.equal (xs.DebugIdx.[2]) 30 "int DebugIdx[2]"

        testCase "DebugIdx get works for reference types" <| fun _ ->
            let xs = [| "hello"; "world" |]
            Expect.equal (xs.DebugIdx.[0]) "hello" "string DebugIdx[0]"
            Expect.equal (xs.DebugIdx.[1]) "world" "string DebugIdx[1]"

        testCase "DebugIdx set works for value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.DebugIdx.[1] <- 99
            Expect.equal xs.[1] 99 "set int via DebugIdx"

        testCase "DebugIdx set works for reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.DebugIdx.[1] <- "z"
            Expect.equal xs.[1] "z" "set string via DebugIdx"

        testCase "DebugIdx set null on reference type" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.DebugIdx.[1] <- null
            Expect.isTrue (isNull xs.[1]) "set null via DebugIdx"

        testCase "DebugIdx out of range negative index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.DebugIdx.[-1] |> ignore)
            throwsRange (fun () -> xs.DebugIdx.[-1] <- 99)

        testCase "DebugIdx out of range positive index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.DebugIdx.[3] |> ignore)
            throwsRange (fun () -> xs.DebugIdx.[3] <- 99)

        testCase "DebugIdx with single element" <| fun _ ->
            let xs = [| "only" |]
            Expect.equal (xs.DebugIdx.[0]) "only" "single element get"
            xs.DebugIdx.[0] <- "changed"
            Expect.equal xs.[0] "changed" "single element set"
            throwsRange (fun () -> xs.DebugIdx.[1] |> ignore)

        testCase "DebugIdx Length property" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            Expect.equal xs.DebugIdx.Length 3 "DebugIdx.Length"
            let empty : int[] = [||]
            Expect.equal empty.DebugIdx.Length 0 "DebugIdx.Length empty"

        testCase "DebugIdx Array property" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let arr = xs.DebugIdx.Array
            Expect.isTrue (Object.ReferenceEquals(xs, arr)) "DebugIdx.Array should return same array"

        testCase "DebugIdx with duplicates" <| fun _ ->
            let xs = [| 5; 5; 5 |]
            Expect.equal (xs.DebugIdx.[0]) 5 "dup[0]"
            Expect.equal (xs.DebugIdx.[1]) 5 "dup[1]"
            Expect.equal (xs.DebugIdx.[2]) 5 "dup[2]"
            xs.DebugIdx.[1] <- 99
            Expect.equal xs.[0] 5 "dup unchanged[0]"
            Expect.equal xs.[1] 99 "dup changed[1]"
            Expect.equal xs.[2] 5 "dup unchanged[2]"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------fail function (tested via module functions)--------------------------------
        // fail is used internally by various Array module functions
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.first throws on empty int array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)

        testCase "Array.first throws on empty string array" <| fun _ ->
            let xs : string[] = [||]
            throwsRange (fun () -> Array.first xs |> ignore)

        testCase "Array.first throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.first xs |> ignore)

        testCase "Array.first works for value types" <| fun _ ->
            Expect.equal (Array.first [| 10; 20; 30 |]) 10 "first int"

        testCase "Array.first works for reference types" <| fun _ ->
            Expect.equal (Array.first [| "a"; "b" |]) "a" "first string"

        testCase "Array.first with single element" <| fun _ ->
            Expect.equal (Array.first [| 42 |]) 42 "first single int"
            Expect.equal (Array.first [| "only" |]) "only" "first single string"

        testCase "Array.first with null element at position 0" <| fun _ ->
            let xs = [| null; "b" |]
            Expect.isTrue (isNull (Array.first xs)) "first null element"

        testCase "Array.secondLast throws on arrays with less than 2 items" <| fun _ ->
            throwsRange (fun () -> Array.secondLast ([||] : int[]) |> ignore)
            throwsRange (fun () -> Array.secondLast [| 1 |] |> ignore)

        testCase "Array.secondLast works for value and reference types" <| fun _ ->
            Expect.equal (Array.secondLast [| 1; 2; 3 |]) 2 "secondLast int"
            Expect.equal (Array.secondLast [| "a"; "b"; "c" |]) "b" "secondLast string"

        testCase "Array.thirdLast throws on arrays with less than 3 items" <| fun _ ->
            throwsRange (fun () -> Array.thirdLast ([||] : int[]) |> ignore)
            throwsRange (fun () -> Array.thirdLast [| 1 |] |> ignore)
            throwsRange (fun () -> Array.thirdLast [| 1; 2 |] |> ignore)

        testCase "Array.thirdLast works for value and reference types" <| fun _ ->
            Expect.equal (Array.thirdLast [| 1; 2; 3; 4 |]) 2 "thirdLast int"
            Expect.equal (Array.thirdLast [| "a"; "b"; "c" |]) "a" "thirdLast string"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------GetNeg/SetNeg extensions (use negIdx from Util.fs)-------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "GetNeg on value types" <| fun _ ->
            let xs = [| 10; 20; 30; 40; 50 |]
            Expect.equal (xs.GetNeg -1) 50 "GetNeg -1"
            Expect.equal (xs.GetNeg -2) 40 "GetNeg -2"
            Expect.equal (xs.GetNeg -5) 10 "GetNeg -5"
            Expect.equal (xs.GetNeg 0) 10 "GetNeg 0"
            Expect.equal (xs.GetNeg 4) 50 "GetNeg 4"

        testCase "GetNeg on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Expect.equal (xs.GetNeg -1) "c" "GetNeg -1 ref"
            Expect.equal (xs.GetNeg -3) "a" "GetNeg -3 ref"
            Expect.equal (xs.GetNeg 0) "a" "GetNeg 0 ref"

        testCase "GetNeg throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.GetNeg 0 |> ignore)
            throwsRange (fun () -> xs.GetNeg -1 |> ignore)

        testCase "GetNeg throws on too-large negative index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.GetNeg -4 |> ignore)

        testCase "GetNeg throws on too-large positive index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.GetNeg 3 |> ignore)

        testCase "GetNeg with single element" <| fun _ ->
            let xs = [| 42 |]
            Expect.equal (xs.GetNeg 0) 42 "GetNeg 0 single"
            Expect.equal (xs.GetNeg -1) 42 "GetNeg -1 single"
            throwsRange (fun () -> xs.GetNeg -2 |> ignore)
            throwsRange (fun () -> xs.GetNeg 1 |> ignore)

        testCase "SetNeg on value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetNeg -1 99
            Expect.equal xs.[2] 99 "SetNeg -1"
            xs.SetNeg -3 88
            Expect.equal xs.[0] 88 "SetNeg -3"
            xs.SetNeg 1 77
            Expect.equal xs.[1] 77 "SetNeg 1"

        testCase "SetNeg on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.SetNeg -1 "z"
            Expect.equal xs.[2] "z" "SetNeg -1 ref"

        testCase "SetNeg throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.SetNeg 0 1)
            throwsRange (fun () -> xs.SetNeg -1 1)

        testCase "SetNeg throws on too-large negative index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.SetNeg -4 99)

        testCase "SetNeg throws on too-large positive index" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.SetNeg 3 99)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------GetLooped/SetLooped extensions---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "GetLooped on value types wraps around" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (xs.GetLooped 0) 10 "GetLooped 0"
            Expect.equal (xs.GetLooped 3) 10 "GetLooped 3 (wraps)"
            Expect.equal (xs.GetLooped 4) 20 "GetLooped 4"
            Expect.equal (xs.GetLooped 5) 30 "GetLooped 5"
            Expect.equal (xs.GetLooped 6) 10 "GetLooped 6 (wraps again)"

        testCase "GetLooped on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Expect.equal (xs.GetLooped 3) "a" "GetLooped wraps ref"
            Expect.equal (xs.GetLooped 4) "b" "GetLooped wraps ref+1"

        testCase "GetLooped with negative indices" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (xs.GetLooped -1) 30 "GetLooped -1"
            Expect.equal (xs.GetLooped -2) 20 "GetLooped -2"
            Expect.equal (xs.GetLooped -3) 10 "GetLooped -3"
            Expect.equal (xs.GetLooped -4) 30 "GetLooped -4 (wraps)"

        testCase "GetLooped throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.GetLooped 0 |> ignore)

        testCase "GetLooped with single element" <| fun _ ->
            let xs = [| 42 |]
            Expect.equal (xs.GetLooped 0) 42 "GetLooped single 0"
            Expect.equal (xs.GetLooped 1) 42 "GetLooped single 1"
            Expect.equal (xs.GetLooped -1) 42 "GetLooped single -1"
            Expect.equal (xs.GetLooped 1000) 42 "GetLooped single 1000"

        testCase "SetLooped on value types wraps around" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetLooped 3 99
            Expect.equal xs.[0] 99 "SetLooped 3 wraps to 0"

        testCase "SetLooped with negative index" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            xs.SetLooped -1 99
            Expect.equal xs.[2] 99 "SetLooped -1"

        testCase "SetLooped throws on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> xs.SetLooped 0 99)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Last/First/Second/Third extensions with various types----------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Last on reference types with null elements" <| fun _ ->
            let xs = [| "a"; null |]
            Expect.isTrue (isNull xs.Last) "Last should be null"

        testCase "First on reference types with null elements" <| fun _ ->
            let xs = [| null; "b" |]
            Expect.isTrue (isNull xs.First) "First should be null"

        testCase "Last/First on float array" <| fun _ ->
            let xs = [| 1.1; 2.2; 3.3 |]
            Expect.equal xs.First 1.1 "First float"
            Expect.equal xs.Last 3.3 "Last float"

        testCase "Last/First on bool array" <| fun _ ->
            let xs = [| true; false |]
            Expect.equal xs.First true "First bool"
            Expect.equal xs.Last false "Last bool"

        testCase "Last/First setters with reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.First <- "z"
            xs.Last <- "y"
            Expect.equal xs.[0] "z" "First set ref"
            Expect.equal xs.[2] "y" "Last set ref"

        testCase "Last/First setters with null" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            xs.First <- null
            xs.Last <- null
            Expect.isTrue (isNull xs.[0]) "First set null"
            Expect.isTrue (isNull xs.[2]) "Last set null"

        testCase "SecondLast/ThirdLast on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c"; "d" |]
            Expect.equal xs.SecondLast "c" "SecondLast ref"
            Expect.equal xs.ThirdLast "b" "ThirdLast ref"

        testCase "Second/Third on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Expect.equal xs.Second "b" "Second ref"
            Expect.equal xs.Third "c" "Third ref"

        testCase "FirstAndOnly on value types" <| fun _ ->
            Expect.equal [| 42 |].FirstAndOnly 42 "FirstAndOnly int"
            throwsRange (fun () -> [||].FirstAndOnly |> ignore)
            throwsRange (fun () -> [| 1; 2 |].FirstAndOnly |> ignore)

        testCase "FirstAndOnly on reference types" <| fun _ ->
            Expect.equal [| "only" |].FirstAndOnly "only" "FirstAndOnly string"
            let xs : string[] = [||]
            throwsRange (fun () -> xs.FirstAndOnly |> ignore)

        testCase "FirstAndOnly with null element" <| fun _ ->
            let xs : string[] = [| null |]
            Expect.isTrue (isNull xs.FirstAndOnly) "FirstAndOnly null"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Slice with various types---------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Slice on reference type array" <| fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(1, 3)
            Expect.isTrue (result = [| "b"; "c"; "d" |]) "Slice ref"

        testCase "Slice with negative indices on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(-3, -1)
            Expect.isTrue (result = [| "c"; "d"; "e" |]) "Slice neg ref"

        testCase "Slice single element" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(1, 1)
            Expect.isTrue (result = [| 2 |]) "Slice single element"

        testCase "Slice full array" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(0, 2)
            Expect.isTrue (result = [| 1; 2; 3 |]) "Slice full"

        testCase "Slice with negative start positive end" <| fun _ ->
            let xs = [| "a"; "b"; "c"; "d"; "e" |]
            let result = xs.Slice(-2, 4)
            Expect.isTrue (result = [| "d"; "e" |]) "Slice mixed indices ref"

        testCase "Slice throws on invalid indices" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> xs.Slice(3, 4) |> ignore)
            throwsRange (fun () -> xs.Slice(0, 5) |> ignore)

        testCase "Slice with start equal to end returns single element" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let result = xs.Slice(2, 2)
            Expect.isTrue (result = [| 3 |]) "Slice single at end"

        testCase "Slice throws when start is clearly after end" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5 |]
            throwsRange (fun () -> xs.Slice(3, 1) |> ignore)

        testCase "Slice with null elements in reference array" <| fun _ ->
            let xs = [| "a"; null; "c"; null; "e" |]
            let result = xs.Slice(1, 3)
            Expect.equal result.Length 3 "Slice null elements count"
            Expect.isTrue (isNull result.[0]) "Slice null[0]"
            Expect.equal result.[1] "c" "Slice null[1]"
            Expect.isTrue (isNull result.[2]) "Slice null[2]"

        testCase "Slice with duplicates" <| fun _ ->
            let xs = [| 5; 5; 5; 5; 5 |]
            let result = xs.Slice(1, 3)
            Expect.isTrue (result = [| 5; 5; 5 |]) "Slice duplicates"

        testCase "Slice does not modify original" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let original = Array.copy xs
            let result = xs.Slice(0, 1)
            result.[0] <- "z"
            Expect.equal xs.[0] "a" "original not modified by Slice mutation"
            Expect.isTrue (xs = original) "original unchanged"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Array module Get/Set (use badGetExn/badSetExn)-----------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.get on value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (Array.get 0 xs) 10 "module get int[0]"
            Expect.equal (Array.get 2 xs) 30 "module get int[2]"

        testCase "Array.get on reference types" <| fun _ ->
            let xs = [| "hello"; "world" |]
            Expect.equal (Array.get 0 xs) "hello" "module get string[0]"

        testCase "Array.get on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.get 0 xs |> ignore)

        testCase "Array.get on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.get 0 xs |> ignore)
            let ys : string[] = null
            throwsNull (fun () -> Array.get 0 ys |> ignore)

        testCase "Array.set on value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Array.set 1 99 xs
            Expect.equal xs.[1] 99 "module set int"

        testCase "Array.set on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.set 1 "z" xs
            Expect.equal xs.[1] "z" "module set string"

        testCase "Array.set null on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.set 1 null xs
            Expect.isTrue (isNull xs.[1]) "module set null"

        testCase "Array.set on empty array" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.set 0 99 xs)

        testCase "Array.set on null array" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.set 0 99 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Array.getNeg/setNeg (module level)------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.getNeg on value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Expect.equal (Array.getNeg -1 xs) 30 "getNeg -1"
            Expect.equal (Array.getNeg -3 xs) 10 "getNeg -3"
            Expect.equal (Array.getNeg 0 xs) 10 "getNeg 0"
            Expect.equal (Array.getNeg 2 xs) 30 "getNeg 2"

        testCase "Array.getNeg on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Expect.equal (Array.getNeg -1 xs) "c" "getNeg -1 ref"
            Expect.equal (Array.getNeg 0 xs) "a" "getNeg 0 ref"

        testCase "Array.getNeg throws on empty" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.getNeg 0 xs |> ignore)

        testCase "Array.getNeg throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.getNeg 0 xs |> ignore)

        testCase "Array.getNeg throws out of range" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            throwsRange (fun () -> Array.getNeg -4 xs |> ignore)
            throwsRange (fun () -> Array.getNeg 3 xs |> ignore)

        testCase "Array.setNeg on value types" <| fun _ ->
            let xs = [| 10; 20; 30 |]
            Array.setNeg -1 99 xs
            Expect.equal xs.[2] 99 "setNeg -1"

        testCase "Array.setNeg on reference types" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.setNeg -1 "z" xs
            Expect.equal xs.[2] "z" "setNeg -1 ref"

        testCase "Array.setNeg throws on empty" <| fun _ ->
            let xs : int[] = [||]
            throwsRange (fun () -> Array.setNeg 0 99 xs)

        testCase "Array.setNeg throws on null" <| fun _ ->
            let xs : int[] = null
            throwsNull (fun () -> Array.setNeg 0 99 xs)

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Duplicate/Copy (should produce independent copies)-------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Duplicate on reference type array creates independent copy" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let dup = xs.Duplicate()
            dup.[0] <- "z"
            Expect.equal xs.[0] "a" "original not modified"
            Expect.equal dup.[0] "z" "dup modified"

        testCase "Duplicate on value type array creates independent copy" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let dup = xs.Duplicate()
            dup.[0] <- 99
            Expect.equal xs.[0] 1 "original not modified"

        testCase "Duplicate on empty array" <| fun _ ->
            let xs : int[] = [||]
            let dup = xs.Duplicate()
            Expect.equal dup.Length 0 "dup empty"

        testCase "Copy on reference type array creates independent copy" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let cp = xs.Copy()
            cp.[0] <- "z"
            Expect.equal xs.[0] "a" "original not modified"

        testCase "Duplicate preserves null elements" <| fun _ ->
            let xs = [| "a"; null; "c" |]
            let dup = xs.Duplicate()
            Expect.isTrue (isNull dup.[1]) "null preserved in dup"

        testCase "Duplicate with duplicates" <| fun _ ->
            let xs = [| 5; 5; 5 |]
            let dup = xs.Duplicate()
            Expect.isTrue (dup = xs) "dup equals original"
            dup.[0] <- 99
            Expect.equal xs.[0] 5 "original not modified"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------FailIfEmpty/FailIfLessThan with types--------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "FailIfEmpty on reference type array" <| fun _ ->
            let xs = [| "a"; "b" |]
            let result = xs.FailIfEmpty("ok")
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "returns same ref"

        testCase "FailIfEmpty on empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            Expect.throws (fun () -> xs.FailIfEmpty("empty") |> ignore) "throws on empty ref"

        testCase "FailIfEmpty on empty value type array" <| fun _ ->
            let xs : float[] = [||]
            Expect.throws (fun () -> xs.FailIfEmpty("empty") |> ignore) "throws on empty float"

        testCase "FailIfLessThan on reference type array" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            let result = xs.FailIfLessThan(2, "ok")
            Expect.isTrue (Object.ReferenceEquals(xs, result)) "returns same ref"

        testCase "FailIfLessThan on insufficient reference type array" <| fun _ ->
            let xs = [| "a" |]
            Expect.throws (fun () -> xs.FailIfLessThan(3, "too few") |> ignore) "throws on too few ref"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------AsString / ToString (different names in Fable)-----------------------------
        // In Fable: member name is AsString (capital A) and inline
        // In .NET: member name is asString (lowercase a) and not inline
        // Both should produce similar output format
        //--------------------------------------------------------------------------------------------------------------------

        testCase "AsString/asString on int array" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            let s = xs.AsString
            #else
            let s = xs.asString
            #endif
            Expect.isTrue (s.Contains("3")) "should contain count"

        testCase "AsString/asString on empty int array" <| fun _ ->
            let xs : int[] = [||]
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            let s = xs.AsString
            #else
            let s = xs.asString
            #endif
            Expect.isTrue (s.Contains("empty")) "should contain empty"

        testCase "AsString/asString on string array" <| fun _ ->
            let xs = [| "hello"; "world" |]
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            let s = xs.AsString
            #else
            let s = xs.asString
            #endif
            Expect.isTrue (s.Contains("2")) "should contain count"

        testCase "AsString/asString on single element" <| fun _ ->
            let xs = [| 42 |]
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            let s = xs.AsString
            #else
            let s = xs.asString
            #endif
            Expect.isTrue (s.Contains("1")) "should contain count 1"

        testCase "ToString(n) on int array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 |]
            let s = xs.ToString(3).Replace("\r\n", "\n").Trim()
            Expect.isTrue (s.Contains("10 items")) "should mention 10 items"
            Expect.isTrue (s.Contains("0: 1")) "should contain first entry"
            Expect.isTrue (s.Contains("...")) "should contain ellipsis for truncated"

        testCase "ToString(n) on empty array" <| fun _ ->
            let xs : int[] = [||]
            let s = xs.ToString(5)
            Expect.isTrue (s.Contains("empty")) "should contain empty"

        testCase "ToString(n) on string array" <| fun _ ->
            let xs = [| "alpha"; "beta"; "gamma" |]
            let s = xs.ToString(10).Replace("\r\n", "\n").Trim()
            Expect.isTrue (s.Contains("3 items")) "should mention 3 items"
            Expect.isTrue (s.Contains("alpha")) "should contain first"
            Expect.isTrue (s.Contains("gamma")) "should contain last"

        testCase "ToString(0) shows no entries" <| fun _ ->
            let xs = [| 1; 2; 3 |]
            let s = xs.ToString(0)
            Expect.isFalse (s.Contains("0:")) "should not show entries"
            Expect.isTrue (s.Contains("3 items")) "should mention count"

        testCase "ToString(n) on single element" <| fun _ ->
            let xs = [| 42 |]
            let s = xs.ToString(5).Replace("\r\n", "\n").Trim()
            Expect.isTrue (s.Contains("1 item")) "should mention 1 item"
            Expect.isTrue (s.Contains("42")) "should contain the value"

        testCase "ToString(n) with null elements" <| fun _ ->
            let xs = [| "a"; null; "c" |]
            let s = xs.ToString(5).Replace("\r\n", "\n")
            Expect.isTrue (s.Contains("3 items")) "should mention count"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------IsEmpty/IsNotEmpty/HasItems/IsSingleton with types-------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "IsEmpty on reference type empty array" <| fun _ ->
            let xs : string[] = [||]
            Expect.isTrue xs.IsEmpty "IsEmpty ref"

        testCase "IsEmpty on reference type non-empty array" <| fun _ ->
            let xs = [| "a" |]
            Expect.isFalse xs.IsEmpty "not IsEmpty ref"

        testCase "IsNotEmpty on reference type" <| fun _ ->
            Expect.isTrue [| "a" |].IsNotEmpty "IsNotEmpty ref"
            let xs : string[] = [||]
            Expect.isFalse xs.IsNotEmpty "not IsNotEmpty ref"

        testCase "HasItems on reference type" <| fun _ ->
            Expect.isTrue [| "a" |].HasItems "HasItems ref"
            let xs : string[] = [||]
            Expect.isFalse xs.HasItems "not HasItems ref"

        testCase "IsSingleton on reference type" <| fun _ ->
            Expect.isTrue [| "a" |].IsSingleton "IsSingleton ref"
            let xs : string[] = [||]
            Expect.isFalse xs.IsSingleton "not IsSingleton empty ref"
            Expect.isFalse [| "a"; "b" |].IsSingleton "not IsSingleton multi ref"

        testCase "IsEmpty on float array" <| fun _ ->
            let xs : float[] = [||]
            Expect.isTrue xs.IsEmpty "IsEmpty float"

        testCase "IsSingleton on float array" <| fun _ ->
            Expect.isTrue [| 1.0 |].IsSingleton "IsSingleton float"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------LastIndex with various types-----------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "LastIndex on reference type array" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Expect.equal xs.LastIndex 2 "LastIndex ref"

        testCase "LastIndex on empty reference type array" <| fun _ ->
            let xs : string[] = [||]
            Expect.equal xs.LastIndex -1 "LastIndex empty ref"

        testCase "LastIndex on float array" <| fun _ ->
            let xs = [| 1.0; 2.0 |]
            Expect.equal xs.LastIndex 1 "LastIndex float"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Arrays with duplicate elements---------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Get with duplicate elements returns correct one" <| fun _ ->
            let xs = [| 5; 5; 5 |]
            Expect.equal (xs.Get 0) 5 "dup get[0]"
            Expect.equal (xs.Get 1) 5 "dup get[1]"
            Expect.equal (xs.Get 2) 5 "dup get[2]"

        testCase "Set with duplicate elements sets correct one" <| fun _ ->
            let xs = [| 5; 5; 5 |]
            xs.Set 1 99
            Expect.equal xs.[0] 5 "dup set unchanged[0]"
            Expect.equal xs.[1] 99 "dup set changed[1]"
            Expect.equal xs.[2] 5 "dup set unchanged[2]"

        testCase "GetNeg with duplicate reference elements" <| fun _ ->
            let xs = [| "x"; "x"; "x" |]
            Expect.equal (xs.GetNeg -1) "x" "dup GetNeg -1"
            Expect.equal (xs.GetNeg -3) "x" "dup GetNeg -3"

        testCase "Slice with all-same elements" <| fun _ ->
            let xs = [| 7; 7; 7; 7; 7 |]
            let result = xs.Slice(1, 3)
            Expect.isTrue (result = [| 7; 7; 7 |]) "Slice all same"

        testCase "Last/First/Second/Third with duplicates" <| fun _ ->
            let xs = [| 5; 5; 5; 5 |]
            Expect.equal xs.First 5 "First dup"
            Expect.equal xs.Second 5 "Second dup"
            Expect.equal xs.Third 5 "Third dup"
            Expect.equal xs.Last 5 "Last dup"

        //--------------------------------------------------------------------------------------------------------------------
        //------------------------------------------Module-level functions with reference types---------------------------------
        //--------------------------------------------------------------------------------------------------------------------

        testCase "Array.getNeg with null elements in reference array" <| fun _ ->
            let xs = [| null; "b"; null |]
            Expect.isTrue (isNull (Array.getNeg 0 xs)) "getNeg null[0]"
            Expect.isTrue (isNull (Array.getNeg -1 xs)) "getNeg null[-1]"
            Expect.equal (Array.getNeg 1 xs) "b" "getNeg non-null"

        testCase "Array.setNeg null on reference array" <| fun _ ->
            let xs = [| "a"; "b"; "c" |]
            Array.setNeg -1 null xs
            Expect.isTrue (isNull xs.[2]) "setNeg null ref"

        testCase "Array.get with null elements" <| fun _ ->
            let xs = [| null; "b"; null |]
            Expect.isTrue (isNull (Array.get 0 xs)) "get null[0]"
            Expect.equal (Array.get 1 xs) "b" "get non-null[1]"

        testCase "Array.set null element" <| fun _ ->
            let xs = [| "a"; "b" |]
            Array.set 0 null xs
            Expect.isTrue (isNull xs.[0]) "set null via module"

    ]
