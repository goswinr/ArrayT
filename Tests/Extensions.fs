namespace Tests

module Extensions =

    open ArrayT


    open Scriptorium.Nib.Assertion
    open type Scriptorium.Quill.Test
    open Exceptions

    let tests =
      testList ("extensions Tests", [

        test ("Intro: 9=9", fun _ -> assertThat 9 (tag "Intro" >> isEqualTo 9))

        test ("DebugIndexer", fun _ ->
            let aa = [|for i in 0 .. 9 ->  float i |]
            assertThat (aa.DebugIdx.[2]) (tag "DebugIndexer 2" >> isEqualTo 2.0)
            assertThat (fun () -> aa.DebugIdx.[10] |> ignore ) (tag "DebugIndexer 10" >> throws)
            aa.DebugIdx.[2] <- 3.0
            assertThat (aa.[2]) (tag "DebugIndexer 2 Item" >> isEqualTo 3.0)
        )

        let a = [|for i in 0 .. 9 ->  float i |]
        //let b = Array.init 10 (fun i -> float i)

        test ("Get", fun _ ->
            assertThat (a.Get 2) (tag "Get 2" >> isEqualTo 2.0)
            assertThat (a.Get 2) (tag "Get 2 Item" >> isEqualTo (a[2]))
            assertThat (fun () -> a.Get 10 |> ignore ) (tag "Get 10" >> throws)
            assertThat (fun () -> a.Get -1 |> ignore ) (tag "Get -1" >> throws)

        )
        test ("Set", fun _ ->
            let a = a.Duplicate()
            a.Set 2 3.0
            assertThat (a.Get 2) (tag "Set 2" >> isEqualTo 3.0)
            a[2] <- 4.0
            assertThat (a.Get 2) (tag "Set 2 Item" >> isEqualTo 4.0)
            assertThat (fun () -> a.Set 10 0.0 |> ignore ) (tag "Set 10" >> throws)
            assertThat (fun () -> a.Set -1 0.0 |> ignore ) (tag "Set -1" >> throws)
        )

        // -- xs.LastIndex --
        test ("LastIndex doesn't raises exception on empty Array", fun _ ->
            let xs = [||]
            let r =  xs.LastIndex
            assertThat -1 (tag "Expected -1" >> isEqualTo r)
        )

        test ("LastIndex returns Count - 1 on non-empty Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let lastIndex = xs.LastIndex
            assertThat lastIndex (tag "Expected LastIndex to be equal to Count - 1" >> isEqualTo (xs.Length - 1))
        )

        //---- xs.Last ----
        test ("Last getter raises exception on empty Array", fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.Last |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Last setter raises exception on empty Array", fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.Last <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Last getter returns last item on non-empty Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let lastItem = xs.Last
            assertThat lastItem (tag "Expected Last to be equal to the last item in the Array" >> isEqualTo 5)
        )

        test ("Last setter changes last item on non-empty Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.Last <- 6
            assertThat xs.Last (tag "Expected Last to be changed to the new value" >> isEqualTo 6)
        )

        //---- xs.SecondLast ----
        test ("SecondLast getter raises exception on Array with less than 2 items", fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.SecondLast |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("SecondLast setter raises exception on Array with less than 2 items", fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.SecondLast <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("SecondLast getter returns second last item on Array with 2 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let secondLastItem = xs.SecondLast
            assertThat secondLastItem (tag "Expected SecondLast to be equal to the second last item in the Array" >> isEqualTo 4)
        )

        test ("SecondLast setter changes second last item on Array with 2 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.SecondLast <- 6
            assertThat xs.SecondLast (tag "Expected SecondLast to be changed to the new value" >> isEqualTo 6)
        )

        //---- xs.ThirdLast ----
        test ("ThirdLast getter raises exception on Array with less than 3 items", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.ThirdLast |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("ThirdLast setter raises exception on Array with less than 3 items", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.ThirdLast <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("ThirdLast getter returns third last item on Array with 3 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let thirdLastItem = xs.ThirdLast
            assertThat thirdLastItem (tag "Expected ThirdLast to be equal to the third last item in the Array" >> isEqualTo 3)
        )

        test ("ThirdLast setter changes third last item on Array with 3 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.ThirdLast <- 6
            assertThat xs.ThirdLast (tag "Expected ThirdLast to be changed to the new value" >> isEqualTo 6)
        )

        //---- xs.First ----
        test ("First getter raises exception on empty Array", fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.First |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("First setter raises exception on empty Array", fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.First <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("First getter returns first item on non-empty Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let firstItem = xs.First
            assertThat firstItem (tag "Expected First to be equal to the first item in the Array" >> isEqualTo 1)
        )

        test ("First setter changes first item on non-empty Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.First <- 6
            assertThat xs.First (tag "Expected First to be changed to the new value" >> isEqualTo 6)
        )

        //---- xs.FirstAndOnly ----
        test ("FirstAndOnly getter raises exception on empty Array", fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.FirstAndOnly |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("FirstAndOnly getter raises exception on Array with more than one item", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.FirstAndOnly |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("FirstAndOnly getter returns the item on Array with exactly one item", fun _ ->
            let xs = [| 1|]
            let firstAndOnlyItem = xs.FirstAndOnly
            assertThat firstAndOnlyItem (tag "Expected FirstAndOnly to be equal to the only item in the Array" >> isEqualTo 1)
        )

        //---- xs.Second ----
        test ("Second getter raises exception on Array with less than 2 items", fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.Second |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Second setter raises exception on Array with less than 2 items", fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.Second <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Second getter returns second item on Array with 2 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let secondItem = xs.Second
            assertThat secondItem (tag "Expected Second to be equal to the second item in the Array" >> isEqualTo 2)
        )

        test ("Second setter changes second item on Array with 2 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.Second <- 6
            assertThat xs.Second (tag "Expected Second to be changed to the new value" >> isEqualTo 6)
        )

        //---- xs.Third ----
        test ("Third getter raises exception on Array with less than 3 items", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.Third |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Third setter raises exception on Array with less than 3 items", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.Third <- 1
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Third getter returns third item on Array with 3 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let thirdItem = xs.Third
            assertThat thirdItem (tag "Expected Third to be equal to the third item in the Array" >> isEqualTo 3)
        )

        test ("Third setter changes third item on Array with 3 or more items", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.Third <- 6
            assertThat xs.Third (tag "Expected Third to be changed to the new value" >> isEqualTo 6)
        )

        test ("Third getter on empty Array raises exception", fun _ ->
            let xs : int[] = [||]
            let testCode = fun () -> xs.Third |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        //---- xs.IsEmpty ----
        test ("IsEmpty returns true for empty Array", fun _ ->
            let xs = [||]
            assertThat xs.IsEmpty (tag "Expected IsEmpty to be true for an empty Array" >> isTrue)
        )

        test ("IsEmpty returns false for non-empty Array", fun _ ->
            let xs = [| 1|]
            assertThat xs.IsEmpty (tag "Expected IsEmpty to be false for a non-empty Array" >> isFalse)
        )

        //---- xs.IsSingleton ----
        test ("IsSingleton returns true for Array with one item", fun _ ->
            let xs = [| 1|]
            assertThat xs.IsSingleton (tag "Expected IsSingleton to be true for a Array with one item" >> isTrue)
        )

        test ("IsSingleton returns false for Array with zero or more than one items", fun _ ->
            let xs = [| 1; 2|]
            assertThat xs.IsSingleton (tag "Expected IsSingleton to be false for a Array with zero or more than one items" >> isFalse)
        )

        //---- xs.IsNotEmpty ----
        test ("IsNotEmpty returns false for empty Array", fun _ ->
            let xs = [||]
            assertThat xs.IsNotEmpty (tag "Expected IsNotEmpty to be false for an empty Array" >> isFalse)
        )

        test ("IsNotEmpty returns true for non-empty Array", fun _ ->
            let xs = [| 1|]
            assertThat xs.IsNotEmpty (tag "Expected IsNotEmpty to be true for a non-empty Array" >> isTrue)
        )


        //---- xs.GetNeg ----
        test ("GetNeg gets an item in the Array by index, allowing for negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            let item = xs.GetNeg -1
            assertThat item (tag "Expected GetNeg to get the last item in the Array when index is -1" >> isEqualTo 3)
        )

        //---- xs.SetNeg ----
        test ("SetNeg sets an item in the Array by index, allowing for negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetNeg -1 4
            assertThat xs.Last (tag "Expected SetNeg to set the last item in the Array when index is -1" >> isEqualTo 4)
        )

        //---- xs.GetLooped ----
        test ("GetLooped gets an item in the Array by index, treating the Array as an endless loop", fun _ ->
            let xs = [| 1; 2; 3|]
            let item = xs.GetLooped 3
            assertThat item (tag "Expected GetLooped to get the first item in the Array when index is equal to the count of the Array" >> isEqualTo 1)
        )

        //---- xs.SetLooped ----
        test ("SetLooped sets an item in the Array by index, treating the Array as an endless loop", fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetLooped 3 4
            assertThat xs.First (tag "Expected SetLooped to set the first item in the Array when index is equal to the count of the Array" >> isEqualTo 4)
        )



        //---- xs.Duplicate ----
        test ("Duplicate creates a shallow copy of the Array", fun _ ->
            let xs = [| 1; 2; 3|]
            let ys = xs.Duplicate()
            assertThat (ys = xs) (tag "Expected Clone to create a shallow copy of the Array" >> isTrue)
        )


        //---- xs.GetSlice ----
        test ("GetSlice gets a slice from the Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let slice = xs[1..3]
            assertThat (slice = [| 2; 3; 4|]) (tag "Expected GetSlice to get a slice from the Array" >> isTrue)
        )

        //---- xs.SetSlice ----
        test ("SetSlice sets a slice in the Array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let newValues = [| 6; 7; 8|]
            xs[1..3] <-  newValues
            assertThat (xs = [| 1; 6; 7; 8; 5|]) (tag "Expected SetSlice to set a slice in the Array" >> isTrue)
        )


        test ("GetSlice raises exception when start index is out of range", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let testCode = fun () -> xs.Slice(5,8) |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("toString entries", fun _ ->
            let a = [|1;2;3;4;5;6|]
            let s = a.ToString(3).Replace("\r\n", "\n").Trim()
            let expected = "array<Int32> with 6 items:\n  0: 1\n  1: 2\n  2: 3\n  ...\n  5: 6"
            assertThat s (tag "toString entries" >> isEqualTo expected)
        )

        //---- xs.FailIfEmpty ----
        test ("FailIfEmpty returns array when not empty", fun _ ->
            let xs = [| 1; 2; 3|]
            let result = xs.FailIfEmpty("should not throw")
            assertThat xs (tag "Expected same array to be returned" >> isEqualTo result)
        )

        test ("FailIfEmpty throws on empty Array", fun _ ->
            let xs : int[] = [||]
            let testCode = fun () -> xs.FailIfEmpty("is empty") |> ignore
            assertThat testCode (tag "Expected an Exception on empty array" >> throws)
            throwsArg testCode
            throwsWith ["Array.FailIfEmpty: is empty"] testCode
        )

        //---- xs.FailIfLessThan ----
        test ("FailIfLessThan returns array when count is sufficient", fun _ ->
            let xs = [| 1; 2; 3|]
            let result = xs.FailIfLessThan(3, "should not throw")
            assertThat xs (tag "Expected same array to be returned" >> isEqualTo result)
        )

        test ("FailIfLessThan throws when count is insufficient", fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.FailIfLessThan(3, "too few") |> ignore
            assertThat testCode (tag "Expected an Exception when array has too few items" >> throws)
            throwsArg testCode
            throwsWith ["Array.FailIfLessThan 3: too few"] testCode
        )

        test ("FailIfLessThan returns array when count exceeds minimum", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let result = xs.FailIfLessThan(3, "should not throw")
            assertThat xs (tag "Expected same array to be returned" >> isEqualTo result)
        )

        //---- xs.HasItems ----
        test ("HasItems returns true for non-empty Array", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat xs.HasItems (tag "Expected HasItems to be true for a non-empty Array" >> isTrue)
        )

        test ("HasItems returns false for empty Array", fun _ ->
            let xs : int[] = [||]
            assertThat xs.HasItems (tag "Expected HasItems to be false for an empty Array" >> isFalse)
        )

        //---- Immutability tests for extension members ----
        test ("Get does not modify input array", fun _ ->
            let xs = [| 1; 2; 3|]
            let original = xs.Duplicate()
            let _ = xs.Get 1
            assertThat (xs = original) (tag "Get should not modify input array" >> isTrue)
        )

        test ("GetNeg does not modify input array", fun _ ->
            let xs = [| 1; 2; 3|]
            let original = xs.Duplicate()
            let _ = xs.GetNeg -1
            assertThat (xs = original) (tag "GetNeg should not modify input array" >> isTrue)
        )

        test ("GetLooped does not modify input array", fun _ ->
            let xs = [| 1; 2; 3|]
            let original = xs.Duplicate()
            let _ = xs.GetLooped 5
            assertThat (xs = original) (tag "GetLooped should not modify input array" >> isTrue)
        )

        test ("Duplicate creates independent copy", fun _ ->
            let xs = [| 1; 2; 3|]
            let dup = xs.Duplicate()
            dup.[0] <- 99
            assertThat xs.[0] (tag "Original array should not be modified when duplicate is changed" >> isEqualTo 1)
        )

        test ("Slice does not modify input array", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let original = xs.Duplicate()
            let _ = xs.Slice(1, 3)
            assertThat (xs = original) (tag "Slice should not modify input array" >> isTrue)
        )

        test ("First getter does not modify input array", fun _ ->
            let xs = [| 1; 2; 3|]
            let original = xs.Duplicate()
            let _ = xs.First
            assertThat (xs = original) (tag "First getter should not modify input array" >> isTrue)
        )

        test ("Last getter does not modify input array", fun _ ->
            let xs = [| 1; 2; 3|]
            let original = xs.Duplicate()
            let _ = xs.Last
            assertThat (xs = original) (tag "Last getter should not modify input array" >> isTrue)
        )

        //---- Additional edge cases ----
        test ("DebugIndexer throws on negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            let testCode = fun () -> xs.DebugIdx.[-1] |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Idx gets item at index", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat (xs.Idx 1) (tag "Idx should return the item at index" >> isEqualTo 2)
        )

        test ("Idx throws on invalid index", fun _ ->
            let xs = [| 1; 2; 3|]
            let testCode = fun () -> xs.Idx 3 |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("GetNeg with boundary negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat (xs.GetNeg -3) (tag "GetNeg -3 should return first item" >> isEqualTo 1)
        )

        test ("GetNeg throws when negative index is too large", fun _ ->
            let xs = [| 1; 2; 3|]
            let testCode = fun () -> xs.GetNeg -4 |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("SetNeg with boundary negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetNeg -3 99
            assertThat xs.[0] (tag "SetNeg -3 should set first item" >> isEqualTo 99)
        )

        test ("SetNeg throws when negative index is too large", fun _ ->
            let xs = [| 1; 2; 3|]
            let testCode = fun () -> xs.SetNeg -4 99
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("GetLooped with large positive index", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat (xs.GetLooped 100) (tag "GetLooped should wrap large positive index" >> isEqualTo (xs.[100 % 3]))
        )

        test ("GetLooped with large negative index", fun _ ->
            let xs = [| 1; 2; 3|]
            let expected = xs.[((-100 % 3) + 3) % 3]
            assertThat (xs.GetLooped -100) (tag "GetLooped should wrap large negative index" >> isEqualTo expected)
        )

        test ("SetLooped with large positive index", fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetLooped 100 99
            assertThat xs.[100 % 3] (tag "SetLooped should wrap large positive index" >> isEqualTo 99)
        )

        test ("Slice with negative start and positive end", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let result = xs.Slice(-3, 4)
            assertThat (result = [|3; 4; 5|]) (tag "Slice should work with mixed indices" >> isTrue)
        )

        test ("Slice throws when start is after end", fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let testCode = fun () -> xs.Slice(3, 1) |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("Slice throws when indices are out of bounds", fun _ ->
            let xs = [| 1; 2; 3|]
            let testCode1 = fun () -> xs.Slice(5, 6) |> ignore
            let testCode2 = fun () -> xs.Slice(0, 5) |> ignore
            assertThat testCode1 (tag "Expected an IndexOutOfRangeException for start out of bounds" >> throws)
            assertThat testCode2 (tag "Expected an IndexOutOfRangeException for end out of bounds" >> throws)
        )

        test ("IsSingleton returns false for empty Array", fun _ ->
            let xs : int[] = [||]
            assertThat xs.IsSingleton (tag "Expected IsSingleton to be false for an empty Array" >> isFalse)
        )

        test ("LastIndex returns correct value for various arrays", fun _ ->
            assertThat [|1|].LastIndex (tag "Single item array should have LastIndex 0" >> isEqualTo 0)
            assertThat [|1;2;3|].LastIndex (tag "Three item array should have LastIndex 2" >> isEqualTo 2)
        )

        test ("FirstAndOnly fails on empty Array", fun _ ->
            let xs : int[] = [||]
            let testCode = fun () -> xs.FirstAndOnly |> ignore
            assertThat testCode (tag "Expected an IndexOutOfRangeException" >> throws)
        )

        test ("SecondLast on two item Array", fun _ ->
            let xs = [| 1; 2|]
            assertThat xs.SecondLast (tag "SecondLast on two item array should return first item" >> isEqualTo 1)
        )

        test ("ThirdLast on three item Array", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat xs.ThirdLast (tag "ThirdLast on three item array should return first item" >> isEqualTo 1)
        )

        test ("Second on two item Array", fun _ ->
            let xs = [| 1; 2|]
            assertThat xs.Second (tag "Second on two item array should return second item" >> isEqualTo 2)
        )

        test ("Third on three item Array", fun _ ->
            let xs = [| 1; 2; 3|]
            assertThat xs.Third (tag "Third on three item array should return third item" >> isEqualTo 3)
        )

    ])
