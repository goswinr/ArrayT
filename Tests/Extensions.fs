namespace Tests

module Extensions =

    open ArrayT


    #if FABLE_COMPILER
    open Fable.Mocha
    #else
    open Expecto
    #endif

    let tests : Test =
      testList "extensions Tests" [

        test "Intro: 9=9" {Expect.equal 9 9 "Intro"}

        test "DebugIndexer" {
            let aa = [|for i in 0 .. 9 ->  float i |]
            Expect.equal (aa.DebugIdx.[2]) 2.0 "DebugIndexer 2"
            Expect.throws (fun () -> aa.DebugIdx.[10] |> ignore ) "DebugIndexer 10"
            aa.DebugIdx.[2] <- 3.0
            Expect.equal (aa.[2]) 3.0 "DebugIndexer 2 Item"
        }

        let a = [|for i in 0 .. 9 ->  float i |]
        //let b = Array.init 10 (fun i -> float i)

        test "Get" {
            Expect.equal (a.Get 2) 2.0 "Get 2"
            Expect.equal (a.Get 2) a[2] "Get 2 Item"
            Expect.throws (fun () -> a.Get 10 |> ignore ) "Get 10"
            Expect.throws (fun () -> a.Get -1 |> ignore ) "Get -1"

        }
        test "Set" {
            let a = a.Duplicate()
            a.Set 2 3.0
            Expect.equal (a.Get 2) 3.0 "Set 2"
            a[2] <- 4.0
            Expect.equal (a.Get 2) 4.0 "Set 2 Item"
            Expect.throws (fun () -> a.Set 10 0.0 |> ignore ) "Set 10"
            Expect.throws (fun () -> a.Set -1 0.0 |> ignore ) "Set -1"
        }

        // -- xs.LastIndex --
        testCase "LastIndex doesn't raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let r =  xs.LastIndex
            Expect.equal -1 r "Expected -1"

        testCase "LastIndex returns Count - 1 on non-empty Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let lastIndex = xs.LastIndex
            Expect.equal lastIndex (xs.Length - 1) "Expected LastIndex to be equal to Count - 1"

        //---- xs.Last ----
        testCase "Last getter raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.Last |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "Last setter raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.Last <- 1
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "Last getter returns last item on non-empty Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let lastItem = xs.Last
            Expect.equal lastItem 5 "Expected Last to be equal to the last item in the Array"

        testCase "Last setter changes last item on non-empty Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.Last <- 6
            Expect.equal xs.Last 6 "Expected Last to be changed to the new value"

        //---- xs.SecondLast ----
        testCase "SecondLast getter raises exception on Array with less than 2 items" <| fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.SecondLast |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "SecondLast setter raises exception on Array with less than 2 items" <| fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.SecondLast <- 1
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "SecondLast getter returns second last item on Array with 2 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let secondLastItem = xs.SecondLast
            Expect.equal secondLastItem 4 "Expected SecondLast to be equal to the second last item in the Array"

        testCase "SecondLast setter changes second last item on Array with 2 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.SecondLast <- 6
            Expect.equal xs.SecondLast 6 "Expected SecondLast to be changed to the new value"

        //---- xs.ThirdLast ----
        testCase "ThirdLast getter raises exception on Array with less than 3 items" <| fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.ThirdLast |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "ThirdLast setter raises exception on Array with less than 3 items" <| fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.ThirdLast <- 1
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "ThirdLast getter returns third last item on Array with 3 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let thirdLastItem = xs.ThirdLast
            Expect.equal thirdLastItem 3 "Expected ThirdLast to be equal to the third last item in the Array"

        testCase "ThirdLast setter changes third last item on Array with 3 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.ThirdLast <- 6
            Expect.equal xs.ThirdLast 6 "Expected ThirdLast to be changed to the new value"

        //---- xs.First ----
        testCase "First getter raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.First |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "First setter raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.First <- 1
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "First getter returns first item on non-empty Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let firstItem = xs.First
            Expect.equal firstItem 1 "Expected First to be equal to the first item in the Array"

        testCase "First setter changes first item on non-empty Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.First <- 6
            Expect.equal xs.First 6 "Expected First to be changed to the new value"

        //---- xs.FirstAndOnly ----
        testCase "FirstAndOnly getter raises exception on empty Array" <| fun _ ->
            let xs = [||]
            let testCode = fun () -> xs.FirstAndOnly |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "FirstAndOnly getter raises exception on Array with more than one item" <| fun _ ->
            let xs = [| 1; 2|]
            let testCode = fun () -> xs.FirstAndOnly |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "FirstAndOnly getter returns the item on Array with exactly one item" <| fun _ ->
            let xs = [| 1|]
            let firstAndOnlyItem = xs.FirstAndOnly
            Expect.equal firstAndOnlyItem 1 "Expected FirstAndOnly to be equal to the only item in the Array"

        //---- xs.Second ----
        testCase "Second getter raises exception on Array with less than 2 items" <| fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.Second |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "Second setter raises exception on Array with less than 2 items" <| fun _ ->
            let xs = [| 1|]
            let testCode = fun () -> xs.Second <- 1
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "Second getter returns second item on Array with 2 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let secondItem = xs.Second
            Expect.equal secondItem 2 "Expected Second to be equal to the second item in the Array"

        testCase "Second setter changes second item on Array with 2 or more items" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            xs.Second <- 6
            Expect.equal xs.Second 6 "Expected Second to be changed to the new value"

        //---- xs.Third ----
        // Similar tests can be written for Third

        //---- xs.IsEmpty ----
        testCase "IsEmpty returns true for empty Array" <| fun _ ->
            let xs = [||]
            Expect.isTrue xs.IsEmpty "Expected IsEmpty to be true for an empty Array"

        testCase "IsEmpty returns false for non-empty Array" <| fun _ ->
            let xs = [| 1|]
            Expect.isFalse xs.IsEmpty "Expected IsEmpty to be false for a non-empty Array"

        //---- xs.IsSingleton ----
        testCase "IsSingleton returns true for Array with one item" <| fun _ ->
            let xs = [| 1|]
            Expect.isTrue xs.IsSingleton "Expected IsSingleton to be true for a Array with one item"

        testCase "IsSingleton returns false for Array with zero or more than one items" <| fun _ ->
            let xs = [| 1; 2|]
            Expect.isFalse xs.IsSingleton "Expected IsSingleton to be false for a Array with zero or more than one items"

        //---- xs.IsNotEmpty ----
        testCase "IsNotEmpty returns false for empty Array" <| fun _ ->
            let xs = [||]
            Expect.isFalse xs.IsNotEmpty "Expected IsNotEmpty to be false for an empty Array"

        testCase "IsNotEmpty returns true for non-empty Array" <| fun _ ->
            let xs = [| 1|]
            Expect.isTrue xs.IsNotEmpty "Expected IsNotEmpty to be true for a non-empty Array"


        //---- xs.GetNeg ----
        testCase "GetNeg gets an item in the Array by index, allowing for negative index" <| fun _ ->
            let xs = [| 1; 2; 3|]
            let item = xs.GetNeg -1
            Expect.equal item 3 "Expected GetNeg to get the last item in the Array when index is -1"

        //---- xs.SetNeg ----
        testCase "SetNeg sets an item in the Array by index, allowing for negative index" <| fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetNeg -1 4
            Expect.equal xs.Last 4 "Expected SetNeg to set the last item in the Array when index is -1"

        //---- xs.GetLooped ----
        testCase "GetLooped gets an item in the Array by index, treating the Array as an endless loop" <| fun _ ->
            let xs = [| 1; 2; 3|]
            let item = xs.GetLooped 3
            Expect.equal item 1 "Expected GetLooped to get the first item in the Array when index is equal to the count of the Array"

        //---- xs.SetLooped ----
        testCase "SetLooped sets an item in the Array by index, treating the Array as an endless loop" <| fun _ ->
            let xs = [| 1; 2; 3|]
            xs.SetLooped 3 4
            Expect.equal xs.First 4 "Expected SetLooped to set the first item in the Array when index is equal to the count of the Array"



        //---- xs.Duplicate ----
        testCase "Duplicate creates a shallow copy of the Array" <| fun _ ->
            let xs = [| 1; 2; 3|]
            let ys = xs.Duplicate()
            Expect.isTrue (ys = xs) "Expected Clone to create a shallow copy of the Array"


        //---- xs.GetSlice ----
        testCase "GetSlice gets a slice from the Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let slice = xs[1..3]
            Expect.isTrue (slice = [| 2; 3; 4|]) "Expected GetSlice to get a slice from the Array"

        //---- xs.SetSlice ----
        testCase "SetSlice sets a slice in the Array" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let newValues = [| 6; 7; 8|]
            xs[1..3] <-  newValues
            Expect.isTrue (xs = [| 1; 6; 7; 8; 5|]) "Expected SetSlice to set a slice in the Array"


        testCase "GetSlice raises exception when start index is out of range" <| fun _ ->
            let xs = [| 1; 2; 3; 4; 5|]
            let testCode = fun () -> xs.Slice(5,8) |> ignore
            Expect.throws testCode "Expected an IndexOutOfRangeException"

        testCase "toString entries" <| fun _ ->
            let a = [|1;2;3;4;5;6|]
            let s = a.ToString(3).Replace("\r\n", "\n").Trim()
            let expected = "array<Int32> with 6 items:\n  0: 1\n  1: 2\n  2: 3\n  ...\n  5: 6"
            Expect.equal s expected "toString entries"

    ]
