namespace ArrayT

open System
open System.Collections.Generic
//open NiceString

#nowarn "44" //for opening the hidden but not Obsolete UtilArray module
open UtilArray

/// The main module for functions on Array<'T>.
/// This module provides additional functions to ones from FSharp.Core.Array module.
module Array =


    /// <summary>Gets an element from an Array. (Use Array.getNeg(i) function if you want to use negative indices too.)</summary>
    /// <param name="arr">The input Array.</param>
    /// <param name="index">The input index.</param>
    /// <returns>The value of the Array at the given index.</returns>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the index is negative or the input Array does not contain enough elements.</exception>
    let inline get (arr: 'T[]) index =
        if isNull arr then nullExn "get"
        arr.Get index


    /// <summary>Sets an element of a Array. (use Array.setNeg(i) function if you want to use negative indices too)</summary>
    /// <param name="arr">The input Array.</param>
    /// <param name="index">The input index.</param>
    /// <param name="value">The input value.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the index is negative or the input Array does not contain enough elements.</exception>
    let inline set (arr: 'T[]) index value =
        if isNull arr then nullExn "set"
        arr.Set index value


    //---------------------------------------------------
    // functions added  (not in FSharp.Core Array module)
    //----------------------------------------------------

    /// Raises an Exception if the Array is empty.
    /// (Useful for chaining)
    /// Returns the input Array
    let inline failIfEmpty (errorMessage: string) (arr: 'T[]) =
        if arr.Length = 0 then raise <| Exception("Array.FailIfEmpty: " + errorMessage)
        arr

    /// Raises an Exception if the Array has less then count items.
    /// (Useful for chaining)
    /// Returns the input Array
    let failIfLessThan (count) (errorMessage: string) (arr: 'T[]) =
        if arr.Length < count then raise <| Exception($"Array.FailIfLessThan {count}: {errorMessage}")
        arr


    /// Gets an item in the Array by index.
    /// Allows for negative index too ( -1 is last item,  like Python)
    /// (a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
    let inline getNeg index (arr: 'T[]) =
        if isNull arr then nullExn "getNeg"
        arr.GetNeg index


    /// Sets an item in the Array by index.
    /// Allows for negative index too ( -1 is last item,  like Python)
    /// (a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
    let inline setNeg index value (arr: 'T[]) =
        if isNull arr then nullExn "setNeg"
        arr.SetNeg index value

    /// Any index will return a value.
    /// Array is treated as an endless loop in positive and negative direction
    let inline getLooped index (arr: 'T[]) =
        if isNull arr then nullExn "getLooped"
        arr.GetLooped index

    /// Any index will set a value.
    /// Array is treated as an endless loop in positive and negative direction
    let inline setLooped index value (arr: 'T[]) =
        if isNull arr then nullExn "setLooped"
        arr.SetLooped index value


    /// Gets the second last item in the Array.
    /// Same as this.[this.Length - 2]
    let inline secondLast (arr: 'T[]) =
        if isNull arr then nullExn "secondLast"
        arr.SecondLast

    /// Gets the third last item in the Array.
    /// Same as this.[this.Length - 3]
    let inline thirdLast (arr: 'T[]) =
        if isNull arr then nullExn "thirdLast"
        arr.ThirdLast

    /// Gets the first item in the Array.
    /// Same as this.[0]
    let inline first (arr: 'T[]) =
        if isNull arr then nullExn "first"
        arr.First

    /// Gets the the only item in the Array.
    /// Fails if the Array does not have exactly one element.
    let inline firstAndOnly (arr: 'T[]) =
        if isNull arr then nullExn "firstAndOnly"
        arr.FirstAndOnly

    /// Gets the second item in the Array.
    /// Same as this.[1]
    let inline second (arr: 'T[]) =
        if isNull arr then nullExn "second"
        arr.Second

    /// Gets the third item in the Array.
    /// Same as this.[2]
    let inline third (arr: 'T[]) =
        if isNull arr then nullExn "third"
        arr.Third

    /// Slice the Array given start and end index.
    /// Allows for negative indices too. ( -1 is last item, like Python)
    /// The resulting Array includes the end index.
    /// Raises an IndexOutOfRangeException if indices are out of range.
    /// If you don't want an exception to be raised for index overflow or overlap use Array.trim.
    /// (A negative index can also be done with '^' prefix. E.g. ^0 for the last item, when F# Language preview features are enabled.)
    let slice startIdx endIdx (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "slice"
        arr.Slice(startIdx, endIdx)


    /// Trim items from start and end.
    /// If the sum of fromStartCount and fromEndCount is bigger than arr.Length it returns an empty Array.
    /// If you want an exception to be raised for index overlap (total trimming is bigger than count) use Array.slice with negative end index.
    let trim fromStartCount fromEndCount (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "trim"
        if fromStartCount < 0 then fail arr "trim: fromStartCount can't be negative: %d" fromStartCount
        if fromEndCount < 0 then fail arr "trim: fromEndCount can't be negative: %d" fromEndCount
        let c = arr.Length
        if fromStartCount + fromEndCount >= c then
            [||]
        else
            let len = c - fromStartCount - fromEndCount
            Array.init len (fun i -> arr.[fromStartCount+i])


    //------------------------------------------------------------------
    //---------------------prev-this-next ------------------------------
    //------------------------------------------------------------------
    // these functions below also exist on Seq module in FsEx:

    /// Yields Seq from (first, second)  up to (second-last, last).
    /// Not looped.
    /// The resulting seq is one element shorter than the input Array.
    let windowed2 (arr: 'T[]) =
        if isNull arr then nullExn "windowed2"
        if arr.Length < 2 then fail arr "windowed2: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
        }

    /// Yields looped Seq from (first, second)  up to (last, first).
    /// The resulting seq has the same element count as the input Array.
    let thisNext (arr: 'T[]) =
        if isNull arr then nullExn "thisNext"
        if arr.Length < 2 then fail arr "thisNext: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
            arr.[arr.Length - 1], arr.[0]
        }

    /// Yields looped Seq from (last,first)  up to (second-last, last).
    /// The resulting seq has the same element count as the input Array.
    let prevThis (arr: 'T[]) =
        if isNull arr then nullExn "prevThis"
        if arr.Length < 2 then fail arr "prevThis: input has less than two items"
        seq {
            arr.[arr.Length - 1], arr.[0]
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
        }

    /// Yields Seq from (first, second, third)  up to (third-last, second-last, last).
    /// Not looped.
    /// The resulting seq is two elements shorter than the input Array.
    let windowed3 (arr: 'T[]) =
        if isNull arr then nullExn "windowed3"
        if arr.Length < 3 then fail arr "windowed3: input has less than three items"
        seq {
            for i = 0 to arr.Length - 3 do
                arr.[i], arr.[i + 1], arr.[i + 2]
        }

    /// Yields looped Seq of  from (last, first, second)  up to (second-last, last, first).
    /// The resulting seq has the same element count as the input Array.
    let prevThisNext (arr: 'T[]) =
        if isNull arr then nullExn "prevThisNext"
        if arr.Length < 3 then fail arr "prevThisNext: input has less than three items"
        seq {
            arr.[arr.Length - 1], arr.[0], arr.[1]
            for i = 0 to arr.Length - 3 do
                arr.[i], arr.[i + 1], arr.[i + 2]
            arr.[arr.Length - 2], arr.[arr.Length - 1], arr.[0]
        }

    /// Yields Seq from (0,first, second)  up to (lastIndex-1 , second-last, last).
    /// Not looped.
    /// The resulting seq is one element shorter than the input Array.
    let windowed2i (arr: 'T[]) =
        if isNull arr then nullExn "windowed2i"
        if arr.Length < 2 then fail arr "windowed2i: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                i, arr.[i], arr.[i + 1]
        }

    /// Yields looped Seq  from (0,first, second)  up to (lastIndex, last, first).
    /// The resulting seq has the same element count as the input Array.
    let iThisNext (arr: 'T[]) =
        if isNull arr then nullExn "iThisNext"
        if arr.Length < 2 then fail arr "iThisNext input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                i, arr.[i], arr.[i + 1]
            arr.Length - 1, arr.[arr.Length - 1], arr.[0]
        }

    /// Yields Seq from (1, first, second, third)  up to (lastIndex-1 , third-last, second-last, last).
    /// Not looped.
    /// The resulting seq is two elements shorter than the input Array.
    let windowed3i (arr: 'T[]) =
        if isNull arr then nullExn "windowed3i"
        if arr.Length < 3 then fail arr "windowed3i: input has less than three items"
        seq {
            for i = 0 to arr.Length - 3 do
                i + 1, arr.[i], arr.[i + 1], arr.[i + 2]
        }

    /// Yields looped Seq from (1, last, first, second)  up to (lastIndex, second-last, last, first)
    /// The resulting seq has the same element count as the input Array.
    let iPrevThisNext (arr: 'T[]) =
        if isNull arr then nullExn "iPrevThisNext"
        if arr.Length < 3 then fail arr "iPrevThisNext: input has less than three items"
        seq {
            0, arr.[arr.Length - 1], arr.[0], arr.[1]

            for i = 0 to arr.Length - 3 do
                i + 1, arr.[i], arr.[i + 1], arr.[i + 2]

            arr.Length - 1, arr.[arr.Length - 2], arr.[arr.Length - 1], arr.[0]
        }

    /// <summary>Returns a Array that contains one item only.</summary>
    /// <param name="value">The input item.</param>
    /// <returns>The result Array of one item.</returns>
    let inline singleton value =
        // allow null values so that Array.singleton [] is valid
        // allow null values so that Array.singleton None is valid
        [| value |]

    /// <summary>Considers array circular and move elements up for positive integers or down for negative integers.
    /// e.g.: rotate +1 [ a, b, c, d] = [ d, a, b, c]
    /// e.g.: rotate -1 [ a, b, c, d] = [ b, c, d, a]
    /// the amount can even be bigger than the array's size. I will just rotate more than one loop.</summary>
    /// <param name="amount">How many elements to shift forward. Or backward if number is negative</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The new result Array.</returns>
    let inline rotate amount (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "rotate"
        let r = Array.zeroCreate (arr.Length)
        for i = 0 to arr.Length - 1 do
            r.[i] <- arr.[negIdxLooped (i - amount) arr.Length]
        r

    /// <summary>Considers array circular and move elements up till condition is met for the first item.
    /// The algorithm takes elements from the end and put them at the start till the first element in the array meets the condition.
    /// If the first element in the input meets the condition no changes are made. But still a shallow copy is returned.</summary>
    /// <param name="condition">The condition to meet.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The new result Array.</returns>
    let inline rotateUpTill (condition: 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "rotateUpTill"
        if arr.Length = 0 then
            arr
        elif condition arr.[0] then
            arr.Duplicate()
        else
            let rec findBackIdx i =
                if i = -1 then
                    fail arr "rotateUpTill: no item in the array meets the condition"
                elif condition arr.[i] then
                    i
                else
                    findBackIdx (i - 1)

            let fi = findBackIdx (arr.Length - 1)
            let r = Array.zeroCreate(arr.Length)
            let mutable j = 0
            for i = fi to arr.Length - 1 do
                r.[j] <-arr.[i]
                j <- j + 1
            for i = 0 to fi - 1 do
                r.[j] <-  arr.[i]
                j <- j + 1
            r

    /// <summary>Considers array circular and move elements up till condition is met for the last item.
    /// The algorithm takes elements from the end and put them at the start till the last element in the array meets the condition.
    /// If the last element in the input meets the condition no changes are made. But still a shallow copy is returned.</summary>
    /// <param name="condition">The condition to meet.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The new result Array.</returns>
    let inline rotateUpTillLast (condition: 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "rotateUpTillLast"
        if arr.Length = 0 then
            arr
        elif condition arr.[arr.Length - 1] then
            arr.Duplicate()
        else
            let rec findBackIdx i =
                if i = -1 then
                    fail arr "rotateUpTill: no item in the array meets the condition"
                elif condition arr.[i] then
                    i
                else
                    findBackIdx (i - 1)

            let fi = findBackIdx (arr.Length - 1)
            let r = Array.zeroCreate (arr.Length)
            let mutable j = 0
            for i = fi + 1 to arr.Length - 1 do
                r.[j ] <-  arr.[i]
                j <- j + 1

            for i = 0 to fi do
                r.[j ] <-  arr.[i]
                j <- j + 1

            r

    /// <summary>Considers array circular and move elements down till condition is met for the first item.
    /// The algorithm takes elements from the start and put them at the end till the first element in the array meets the condition.
    /// If the first element in the input meets the condition no changes are made. But still a shallow copy is returned.</summary>
    /// <param name="condition">The condition to meet.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The new result Array.</returns>
    let inline rotateDownTill (condition: 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "rotateDownTill"
        if arr.Length = 0 then
            arr
        elif condition arr.[0] then
            arr.Duplicate()
        else
            let k = arr.Length
            let rec findIdx i =
                if i = k then
                    fail arr "rotateDownTill: no item in the array meets the condition"
                elif condition arr.[i] then
                    i
                else
                    findIdx (i + 1)

            let fi = findIdx (0)
            let r = Array.zeroCreate (arr.Length)
            let mutable j = 0
            for i = fi to arr.Length - 1 do
                r.[j ] <-  arr.[i]
                j <- j + 1
            for i = 0 to fi - 1 do
                r.[ j] <-  arr.[i]
                j <- j + 1
            r

    /// <summary>Considers array circular and move elements down till condition is met for the last item.
    /// The algorithm takes elements from the start and put them at the end till the last element in the array meets the condition.
    /// If the last element in the input meets the condition no changes are made. But still a shallow copy is returned.</summary>
    /// <param name="condition">The condition to meet.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The new result Array.</returns>
    let inline rotateDownTillLast (condition: 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "rotateDownTillLast"
        if arr.Length = 0 then
            arr
        elif condition arr.[0] then
            arr.Duplicate()
        else
            let k = arr.Length
            let rec findIdx i =
                if i = k then
                    fail arr "rotateDownTill: no item in the array meets the condition"
                elif condition arr.[i] then
                    i
                else
                    findIdx (i + 1)

            let fi = findIdx (0)
            let r = Array.zeroCreate (arr.Length)
            let mutable j = 0
            for i = fi + 1 to arr.Length - 1 do
                r.[j ] <-  arr.[i]
                j <- j + 1
            for i = 0 to fi do
                r.[j ] <-  arr.[i]
                j <- j + 1
            r


    /// Returns true if the given Array has just one item.
    /// Same as  Array.hasOne
    let inline isSingleton (arr: 'T[]) : bool =
        if isNull arr then nullExn "isSingleton"
        arr.Length = 1

    /// Returns true if the given Array has just one item.
    /// Same as  Array.isSingleton
    let inline hasOne (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasOne"
        arr.Length = 1

    /// Returns true if the given Array is not empty.
    let inline isNotEmpty (arr: 'T[]) : bool =
        if isNull arr then nullExn "isNotEmpty"
        arr.Length <> 0

    /// Returns true if the given Array has count items.
    let inline hasItems count (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasItems"
        arr.Length = count

    /// Returns true if the given Array has equal or more than count items.
    let inline hasMinimumItems count (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasMinimumItems"
        arr.Length >= count

    /// Returns true if the given Array has equal or less than count items.
    let inline hasMaximumItems count (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasMaximumItems"
        arr.Length <= count

    /// Swap the values of two given indices in Array
    let inline swap i j (arr: 'T[]) : unit =
        if isNull arr then nullExn "swap"
        if i < 0 || j < 0 || i >= arr.Length || j >= arr.Length then
            fail arr "swap: index i=%d and j=%d can't be less than 0 or bigger than last index %d " i j (arr.Length - 1)
        if i <> j then
            let ti = arr.[i]
            arr.[i] <- arr.[j]
            arr.[j] <- ti


    // internal, only for finding MinMax values
    module private MinMax =
        //TODO test keeping of order if equal !

        (*
        let inline simple cmpF (arr:Array<'T>) =
            if arr.Length < 1 then fail arr "MinMax.simple: Count must be at least one: %s"  arr.ToNiceStringLong
            let mutable m = arr.[0]
            for i=1 to arr.Length-1 do
                if cmpF arr.[i] m then m <- arr.[i]
            m
        *)

        let inline simple2 cmpF (arr: 'T[]) =
            if arr.Length < 2 then fail arr "MinMax.simple2: Count must be at least two"
            let mutable m1 = arr.[0]
            let mutable m2 = arr.[1]
            for i = 1 to arr.Length - 1 do
                let this = arr.[i]
                if cmpF this m1 then
                    m2 <- m1
                    m1 <- this
                elif cmpF this m2 then
                    m2 <- this
            m1, m2


        /// If any are equal then the  order is kept by using ( a=b || ) since the compare operate does not include the equal test
        let inline sort3 cmp a b c =
            if a = b || cmp a b then
                if cmp b c then a, b, c
                else if cmp a c then a, c, b
                else c, a, b
            else if a = c || cmp a c then
                b, a, c
            else if b = c || cmp b c then
                b, c, a
            else
                c, b, a


        /// If any are equal then the  order is kept by using ( a=b || ) since the compare operate does not include the equal test
        let inline indexOfSort3By f cmp aa bb cc =
            let a = f aa
            let b = f bb
            let c = f cc
            if a = b || cmp a b then
                if cmp b c then 0, 1, 2
                else if cmp a c then 0, 2, 1
                else 2, 0, 1
            else if a = c || cmp a c then
                1, 0, 2
            else if b = c || cmp b c then
                1, 2, 0
            else
                2, 1, 0

        let inline simple3 cmpF (arr: 'T[]) =
            if arr.Length < 3 then fail arr "MinMax.simple3: Count must be at least three"
            let e1 = arr.[0]
            let e2 = arr.[1]
            let e3 = arr.[2]
            // sort first 3
            let mutable m1, m2, m3 = sort3 cmpF e1 e2 e3 // otherwise would fail on sorting first 3, test on Array([5;6;3;1;2;0])|> Array.max3
            for i = 3 to arr.Length - 1 do
                let this = arr.[i]
                if cmpF this m1 then
                    m3 <- m2
                    m2 <- m1
                    m1 <- this
                elif cmpF this m2 then
                    m3 <- m2
                    m2 <- this
                elif cmpF this m3 then
                    m3 <- this
            m1, m2, m3

        let inline indexByFun cmpF func (arr: 'T[]) =
            if arr.Length < 1 then fail arr "MinMax.indexByFun: Count must be at least one"
            let mutable f = func arr.[0]
            let mutable mf = f
            let mutable ii = 0
            for i = 1 to arr.Length - 1 do
                f <- func arr.[i]
                if cmpF f mf then
                    ii <- i
                    mf <- f
            ii

        let inline index2ByFun cmpF func (arr: 'T[]) =
            if arr.Length < 2 then fail arr "MinMax.index2ByFun: Count must be at least two"
            let mutable i1 = 0
            let mutable i2 = 1
            let mutable mf1 = func arr.[i1]
            let mutable mf2 = func arr.[i2]
            let mutable f = mf1 // placeholder
            for i = 1 to arr.Length - 1 do
                f <- func arr.[i]
                if cmpF f mf1 then
                    i2 <- i1
                    i1 <- i
                    mf2 <- mf1
                    mf1 <- f
                elif cmpF f mf2 then
                    i2 <- i
                    mf2 <- f
            i1, i2

        let inline index3ByFun (cmpOp: 'U -> 'U -> bool) (byFun: 'T -> 'U) (arr: 'T[]) =
            if arr.Length < 3 then fail arr "MinMax.index3ByFun: Count must be at least three"
            // sort first 3
            let mutable i1, i2, i3 = indexOfSort3By byFun cmpOp arr.[0] arr.[1] arr.[2] // otherwise would fail on sorting first 3, test on Array([5;6;3;1;2;0])|> Array.max3
            let mutable e1 = byFun arr.[i1]
            let mutable e2 = byFun arr.[i2]
            let mutable e3 = byFun arr.[i3]
            let mutable f = e1 // placeholder
            for i = 3 to arr.Length - 1 do
                f <- byFun arr.[i]
                if cmpOp f e1 then
                    i3 <- i2
                    i2 <- i1
                    i1 <- i
                    e3 <- e2
                    e2 <- e1
                    e1 <- f
                elif cmpOp f e2 then
                    i3 <- i2
                    i2 <- i
                    e3 <- e2
                    e2 <- f
                elif cmpOp f e3 then
                    i3 <- i
                    e3 <- f
            i1, i2, i3

    (* covered by part copied from Array module:
        let min arr =     arr |> MinMax.simple (<)
        let max arr =     arr |> MinMax.simple (>)
        let minBy f arr = let i = arr |> MinMax.indexByFun (<) f in arr.[i]
        let maxBy f arr = let i = arr |> MinMax.indexByFun (>) f in arr.[i]
        *)


    /// <summary>Returns the index of the smallest of all elements of the Array, compared via Operators.max on the function result.</summary>
    /// <param name="projection">The function to transform the elements into a type supporting comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the input Array is empty.</exception>
    /// <returns>The index of the smallest element.</returns>
    let inline minIndexBy (projection: 'T -> 'Key) (arr: 'T[]) : int =
        if isNull arr then nullExn "minIndexBy"
        arr |> MinMax.indexByFun (<) projection

    /// <summary>Returns the index of the greatest of all elements of the Array, compared via Operators.max on the function result.</summary>
    /// <param name="projection">The function to transform the elements into a type supporting comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the input Array is empty.</exception>
    /// <returns>The index of the maximum element.</returns>
    let inline maxIndexBy (projection: 'T -> 'Key) (arr: 'T[]) : int =
        if isNull arr then nullExn "maxIndexBy"
        arr |> MinMax.indexByFun (>) projection


    /// Returns the smallest and the second smallest element of the Array.
    /// If they are equal then the order is kept
    let inline min2 arr =
        if isNull arr then nullExn "min2"
        arr |> MinMax.simple2 (<)

    /// Returns the biggest and the second biggest element of the Array.
    /// If they are equal then the  order is kept
    let inline max2 arr =
        if isNull arr then nullExn "max2"
        arr |> MinMax.simple2 (>)

    // TODO make consistent xml docstring on below functions:

    /// Returns the smallest and the second smallest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline min2By f arr =
        if isNull arr then nullExn "min2By"
        let i, ii = arr |> MinMax.index2ByFun (<) f
        arr.[i], arr.[ii]

    /// Returns the biggest and the second biggest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline max2By f arr =
        if isNull arr then nullExn "max2By"
        let i, ii = arr |> MinMax.index2ByFun (>) f
        arr.[i], arr.[ii]

    /// Returns the indices of the smallest and the second smallest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline min2IndicesBy f arr =
        if isNull arr then nullExn "min2IndicesBy"
        arr |> MinMax.index2ByFun (<) f

    /// Returns the indices of the biggest and the second biggest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline max2IndicesBy f arr =
        if isNull arr then nullExn "max2IndicesBy"
        arr |> MinMax.index2ByFun (>) f


    /// Returns the smallest three elements of the Array.
    /// The first element is the smallest, the second is the second smallest and the third is the third smallest.
    /// If they are equal then the order is kept
    let inline min3 arr =
        if isNull arr then nullExn "min3"
        arr |> MinMax.simple3 (<)

    /// Returns the biggest three elements of the Array.
    /// The first element is the biggest, the second is the second biggest and the third is the third biggest.
    /// If they are equal then the order is kept
    let inline max3 arr =
        if isNull arr then nullExn "max3"
        arr |> MinMax.simple3 (>)

    /// Returns the smallest three elements of the Array.
    /// The first element is the smallest, the second is the second smallest and the third is the third smallest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline min3By f arr =
        if isNull arr then nullExn "min3By"
        let i, ii, iii = arr |> MinMax.index3ByFun (<) f
        arr.[i], arr.[ii], arr.[iii]

    /// Returns the biggest three elements of the Array.
    /// The first element is the biggest, the second is the second biggest and the third is the third biggest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline max3By f arr =
        if isNull arr then nullExn "max3By"
        let i, ii, iii = arr |> MinMax.index3ByFun (>) f
        arr.[i], arr.[ii], arr.[iii]

    /// Returns the indices of the three smallest elements of the Array.
    /// The first element is the index of the smallest, the second is the index of the second smallest and the third is the index of the third smallest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline min3IndicesBy f arr =
        if isNull arr then nullExn "min3IndicesBy"
        arr |> MinMax.index3ByFun (<) f

    /// Returns the indices of the three biggest elements of the Array.
    /// The first element is the index of the biggest, the second is the index of the second biggest and the third is the index of the third biggest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept
    let inline max3IndicesBy f arr =
        if isNull arr then nullExn "max3IndicesBy"
        arr |> MinMax.index3ByFun (>) f


    /// Return the length or count of the collection.
    /// Same as Array.length
    let inline count (arr: 'T[]) : int =
        if isNull arr then nullExn "count"
        arr.Length

    /// Counts for how many items of the collection the predicate returns true.
    /// Same as Array.filter and then Array.length
    let inline countIf (predicate: 'T -> bool) (arr: 'T[]) : int = //countBy is something else !!
        if isNull arr then nullExn "countIf"
        let mutable k = 0
        for i = 0 to arr.Length - 1 do
            if predicate arr.[i] then k <- k + 1
        k


    /// Builds a new Array from the given ResizeArray.
    /// (Use the asArray function if you want to just cast an ResizeArray to a Array in Fable-JavaScript)
    let inline ofResizeArray (rarr: ResizeArray<'T>) : 'T[] =
        if isNull rarr then nullExn "ofResizeArray"
        rarr.ToArray()

    /// Return a fixed-length Array containing the elements of the input ResizeArray as a copy.
    /// When this function is used in Fable (JavaScript) the ResizeArray is just casted to an Array.
    /// In .NET a new Array is still allocated and the elements are copied.
    let inline asArray (arr: ResizeArray<'T>) : 'T[]=
        if isNull arr then nullExn "asArray"
        #if FABLE_COMPILER_JAVASCRIPT
        unbox arr
        #else
        arr.ToArray()
        #endif

    /// Builds a new Array from the given ResizeArray.
    /// In Fable-JavaScript the ResizeArray is just casted to an Array without allocating a new ResizeArray.
    let inline asResizeArray(arr: 'T[]) : ResizeArray<'T> =
        if isNull arr then nullExn "asResizeArray"
        #if FABLE_COMPILER_JAVASCRIPT
        unbox arr
        #else
        let l = ResizeArray<'T>(arr.Length)
        for i = 0 to arr.Length - 1 do
            l.Add arr.[i]
        l
        #endif


    /// Return a fixed-length Array containing the elements of the input Array as a copy.
    /// This function always allocates a new ResizeArray and copies the elements.
    /// (Use the asArray function if you want to just cast a Array to an Array in Fable-JavaScript)
    let inline toResizeArray (arr: 'T[]) : ResizeArray<'T> =
        if isNull arr then nullExn "toArray"
        let l = ResizeArray<'T>(arr.Length)
        for i = 0 to arr.Length - 1 do
            l.Add arr.[i]
        l

    /// Build a Array from the given IList Interface.
    let inline ofIList (arr: IList<'T>) : 'T[] =
        if isNull arr then nullExn "ofIList"
        let l = Array.zeroCreate (arr.Count)
        arr.CopyTo(l, 0)
        l



    /// Applies a function to array
    /// If resulting array meets the resultPredicate it is returned, otherwise the original input is returned.
    let inline mapIfResult (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "mapIfResult"
        let r = transform arr
        if resultPredicate r then r else arr

    /// Applies a function to array if it meets the inputPredicate, otherwise just returns input.
    /// If resulting array meets the resultPredicate it is returned, otherwise original input is returned.
    let inline mapIfInputAndResult (inputPredicate: 'T[] -> bool) (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "mapIfInputAndResult"
        if inputPredicate arr then
            let r = transform arr
            if resultPredicate r then r else arr
        else
            arr

    /// Applies a function to array
    /// If resulting array meets the resultPredicate it is returned, otherwise the original input is returned.
    [<Obsolete("Use mapIfResult instead")>]
    let inline applyIfResult (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "applyIfResult"
        let r = transform arr
        if resultPredicate r then r else arr

    /// Applies a function to array if it meets the inputPredicate, otherwise just returns input.
    /// If resulting array meets the resultPredicate it is returned, otherwise original input is returned.
    [<Obsolete("Use mapIfInputAndResult instead")>]
    let inline applyIfInputAndResult (inputPredicate: 'T[] -> bool) (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "applyIfInputAndResult"
        if inputPredicate arr then
            let r = transform arr
            if resultPredicate r then r else arr
        else
            arr



    /// Returns all elements that exists more than once in Array.
    /// Each element that exists more than once is only returned once.
    /// Returned order is by first occurrence of first duplicate.
    let duplicates (arr: 'T[]) =
        if isNull arr then nullExn "duplicates"
        let h = HashSet<'T>()
        let t = HashSet<'T>()
        // first Add should be false, second Add true, to recognize the first occurrence of a duplicate:
        //arr.FindAll(System.Predicate(fun x -> if h.Add x then false else t.Add x))
        arr |> Array.filter (fun x -> if h.Add x then false else t.Add x)

    /// Returns all elements that exists more than once in Array.
    /// Each element that exists more than once is only returned once.
    /// Returned order is by first occurrence of first duplicate.
    let duplicatesBy (f: 'T -> 'U) (arr: 'T[]) =
        if isNull arr then nullExn "duplicatesBy"
        let h = HashSet<'U>()
        let t = HashSet<'U>()
        // first Add should be false, second Add true, to recognize the first occurrence of a duplicate:
        //arr.FindAll(System.Predicate(fun x -> let y = f x in if h.Add y then false else t.Add y))
        arr |> Array.filter (fun x -> let y = f x in if h.Add y then false else t.Add y)


    /// Checks if a given array matches the content in Array at a given index
    let matches (searchFor:'T[]) atIdx (searchIn:'T[]) :bool =
        if isNull searchFor then nullExn "matches"
        if atIdx < 0                then fail searchIn <| sprintf "matches: atIdx Index is too small: %d for array of %d items" atIdx searchIn.Length
        if atIdx >= searchIn.Length then fail searchIn <| sprintf "matches: atIdx Index is too big: %d for array of %d items" atIdx searchIn.Length
        let fLast = searchFor.Length - 1
        let iLen = searchIn.Length
        let rec find i f = // index in searchIn ,  index in searchFor
            if  i = iLen then false // not found! not enough items left in searchIn array
            elif searchIn.[i] = searchFor.[f]  then
                if f = fLast then true // found,  exit !
                else find (i + 1) (f + 1)
            else false //exit
        find atIdx 0


    /// Find first index where searchFor occurs in searchIn array.
    /// Give lower and upper bound index for search space.
    /// Returns -1 if not found
    let findValue (searchFor:'T) fromIdx  tillIdx (searchIn:'T[])  :int =
        if isNull searchIn then nullExn "findValue"
        if fromIdx < 0                then fail searchIn <| sprintf "findValue: fromIdx Index is too small: %d for array of %d items" fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findValue: tillIdx Index is too big:   %d for array of %d items" tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findValue: tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" tillIdx fromIdx searchIn.Length
        let rec find i  =
            if  i > tillIdx  then -1 // not found!
            elif searchIn.[i] = searchFor  then i  // found,  exit !
            else find (i + 1)
        find fromIdx


    /// Find last index where searchFor occurs in searchIn array. Searching from end.
    /// Give lower and upper bound index for search space.
    /// Returns -1 if not found
    let findLastValue (searchFor:'T) fromIdx  tillIdx (searchIn:'T[])   :int =
        if isNull searchIn then nullExn "findLastValue"
        if fromIdx < 0                then fail searchIn <| sprintf "findLastValue: fromIdx Index is too small: %d for array of %d items" fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findLastValue: tillIdx Index is too big:   %d for array of %d items" tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findLastValue: tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" tillIdx fromIdx searchIn.Length
        let rec find i  =
            if  i < fromIdx  then -1 // not found!
            elif searchIn.[i] = searchFor  then i  // found,  exit !
            else find (i - 1)
        find tillIdx


    /// Find first index where searchFor array occurs in searchIn array.
    /// Give lower and upper bound index for search space.
    /// Returns index of first element or -1 if not found
    let findArray (searchFor:'T[]) fromIdx  tillIdx (searchIn:'T[])   :int =
        if isNull searchIn then nullExn "findArray`"
        if fromIdx < 0                then fail searchIn <| sprintf "findArray (of %d items): fromIdx Index is too small: %d for array of %d items" searchFor.Length fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findArray (of %d items): tillIdx Index is too big:   %d for array of %d items" searchFor.Length tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findArray (of %d items): tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" searchFor.Length tillIdx fromIdx searchIn.Length
        let fLast = searchFor.Length - 1
        let rec find i f = // index in searchIn ,  index in searchFor
            if  i > tillIdx - fLast + f  then -1 // not found! not enough items left in searchIn array
            elif searchIn.[i] = searchFor.[f]  then
                if f = fLast then i - fLast  // found,  exit !
                else find (i + 1) (f + 1)
            else find (i + 1 - f) 0  // set back search to i+1 before first match
        find fromIdx 0



    /// Find last index where searchFor array occurs in searchIn array. Searching from end.
    /// Give lower and upper bound index for search space.
    /// Returns index of first element  or -1 if not found
    let findLastArray (searchFor:'T[]) fromIdx  tillIdx (searchIn:'T[])   :int =
        if isNull searchIn then nullExn "findLastArray"
        if fromIdx < 0                then fail searchIn <| sprintf "findLastArray (of %d items): fromIdx Index is too small: %d for array of %d items" searchFor.Length fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findLastArray (of %d items): tillIdx Index is too big:   %d for array of %d items" searchFor.Length tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findLastArray (of %d items): tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" searchFor.Length tillIdx fromIdx searchIn.Length
        let fLast = searchFor.Length - 1
        let rec find i f = // index in searchIn ,  index in searchFor
            if  i - f < fromIdx  then -1 // not found! not enough items left in searchIn array
            elif searchIn.[i] = searchFor.[f]  then
                if f = 0 then i  // found ,  exit!
                else find (i - 1) (f - 1)
            else find (i - 1 + fLast - f) fLast  // set back search to i-1 before first match
        find tillIdx fLast



    /// <summary>Returns a new collection containing only the elements of the collection
    /// for which the given predicate run on the index returns <c>true</c>.</summary>
    /// <param name="predicate">The function to test the current index.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>An array containing the elements for which the given predicate returns true.</returns>
    let filteri (predicate: int -> bool) (arr: 'T[]) =
        if isNull arr then nullExn "filteri"
        let res = ResizeArray()
        for i = 0 to arr.Length - 1 do
            if predicate i then
                res.Add(arr.[i])
        asArray res

