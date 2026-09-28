namespace ArrayT

open System
open System.Collections.Generic

#nowarn "44" //for opening the hidden but not Obsolete UtilArray module
open UtilArray
#warnon "44"


/// The main module for functions on Array<'T>.
/// This module provides additional functions to ones from FSharp.Core.Array module.
module Array =


    /// <summary>Gets an element from an Array. (Use Array.getNeg(i) function if you want to use negative indices too.)</summary>
    /// <param name="index">The input index.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The value of the Array at the given index.</returns>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the index is negative or the input Array does not contain enough elements.</exception>
    let inline get (index:int) (arr: 'T[]) : 'T =
        #if UNCHECKED
            getUnCkd index arr
        #else
            if isNull arr then nullExn "get"
            arr.Get index
        #endif

    /// <summary>Sets an element of a Array. (use Array.setNeg(i) function if you want to use negative indices too)</summary>
    /// <param name="index">The input index.</param>
    /// <param name="value">The input value.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when the index is negative or the input Array does not contain enough elements.</exception>
    let inline set (index:int) (value:'T) (arr: 'T[]) : unit =
        #if UNCHECKED
            setUnCkd index value arr
        #else
            if isNull arr then nullExn "set"
            arr.Set index value
        #endif

    /// <summary>Just Array.zeroCreate in .NET.
    /// In Fable, when the `UNCHECKED` symbol is defined, it emits `new Array(len)`
    /// without filling the array with the default value of the items.
    /// So the items are `undefined` in JavaScript until they are set.
    /// Only for reference types, Fable emits TypedArrays for numeric arrays.</summary>
    /// <param name="len">The length of the array to create.</param>
    /// <returns>The new array.</returns>
    let inline zeroCreateUndef<'T when 'T : not struct> (len:int) : 'T [] =
        #if UNCHECKED && (FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT)
            Fable.Core.JsInterop.emitJsExpr (len) "new Array($0)"
        #else
            Array.zeroCreate<'T> len
        #endif




    //---------------------------------------------------
    // functions added that are not in FSharp.Core Array module)
    //----------------------------------------------------

    /// <summary>Raises an ArgumentException if the Array is empty.
    /// (Useful for chaining)
    /// Returns the input Array</summary>
    /// <param name="errorMessage">The error message to include in the exception.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The input Array if not empty.</returns>
    let inline failIfEmpty (errorMessage: string) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "failIfEmpty"
        if arr.Length = 0 then raise <| ArgumentException("Array.FailIfEmpty: " + errorMessage)
        arr

    /// <summary>Raises an ArgumentException if the Array has less then count items.
    /// (Useful for chaining)
    /// Returns the input Array</summary>
    /// <param name="count">The minimum count required.</param>
    /// <param name="errorMessage">The error message to include in the exception.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The input Array if it has enough items.</returns>
    let failIfLessThan (count:int) (errorMessage: string) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "failIfLessThan"
        if arr.Length < count then raise <| ArgumentException($"Array.FailIfLessThan {count}: {errorMessage}")
        arr


    /// <summary>Gets an item in the Array by index.
    /// Allows for negative index too ( -1 is last item,  like Python)
    /// (With LangVersion preview, F# also supports indexing from the end with the '^' prefix, e.g. xs.[^0] for the last item.)</summary>
    /// <param name="index">The index to access (can be negative).</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The value at the specified index.</returns>
    let inline getNeg (index:int) (arr: 'T[]) : 'T =
        if isNull arr then nullExn "getNeg"
        arr.GetNeg index


    /// <summary>Sets an item in the Array by index.
    /// Allows for negative index too ( -1 is last item,  like Python)
    /// (With LangVersion preview, F# also supports indexing from the end with the '^' prefix, e.g. xs.[^0] for the last item.)</summary>
    /// <param name="index">The index to set (can be negative).</param>
    /// <param name="value">The value to set.</param>
    /// <param name="arr">The input Array.</param>
    let inline setNeg (index:int) (value:'T) (arr: 'T[]) : unit =
        if isNull arr then nullExn "setNeg"
        arr.SetNeg index value

    /// <summary>Any index will return a value.
    /// Array is treated as an endless loop in positive and negative direction</summary>
    /// <param name="index">The index to access (can be any integer).</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The value at the looped index.</returns>
    let inline getLooped (index:int) (arr: 'T[]) : 'T =
        if isNull arr then nullExn "getLooped"
        arr.GetLooped index

    /// <summary>Any index will set a value.
    /// Array is treated as an endless loop in positive and negative direction</summary>
    /// <param name="index">The index to set (can be any integer).</param>
    /// <param name="value">The value to set.</param>
    /// <param name="arr">The input Array.</param>
    let inline setLooped (index:int) (value:'T) (arr: 'T[]) : unit =
        if isNull arr then nullExn "setLooped"
        arr.SetLooped index value


    /// <summary>Gets the second last item in the Array.
    /// Same as this.[this.Length - 2]</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The second last item.</returns>
    let inline secondLast (arr: 'T[]) : 'T =
        if isNull arr then nullExn "secondLast"
        arr.SecondLast

    /// <summary>Gets the third last item in the Array.
    /// Same as this.[this.Length - 3]</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The third last item.</returns>
    let inline thirdLast (arr: 'T[]) : 'T =
        if isNull arr then nullExn "thirdLast"
        arr.ThirdLast

    /// <summary>Gets the first item in the Array.
    /// Same as this.[0]</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The first item.</returns>
    let inline first (arr: 'T[]) : 'T =
        if isNull arr then nullExn "first"
        arr.First

    /// <summary>Gets the only item in the Array.
    /// Fails if the Array does not have exactly one element.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The single item in the Array.</returns>
    let inline firstAndOnly (arr: 'T[]) : 'T =
        if isNull arr then nullExn "firstAndOnly"
        arr.FirstAndOnly

    /// <summary>Gets the second item in the Array.
    /// Same as this.[1]</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The second item.</returns>
    let inline second (arr: 'T[]) : 'T =
        if isNull arr then nullExn "second"
        arr.Second

    /// <summary>Gets the third item in the Array.
    /// Same as this.[2]</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The third item.</returns>
    let inline third (arr: 'T[]) : 'T =
        if isNull arr then nullExn "third"
        arr.Third

    /// <summary>Slice the Array given start and end index.
    /// Allows for negative indices too. ( -1 is last item, like Python)
    /// The resulting Array includes the end index.
    /// If the end index is one less than the start index an empty Array is returned.
    /// Raises an IndexOutOfRangeException if indices are out of range.
    /// If you don't want an exception to be raised for index overflow or overlap use Array.trim.
    /// To reject negative indices use Array.sliceIdx, to normalize any index with modulo use Array.sliceLooped.
    /// (With LangVersion preview, F# also supports slicing from the end with the '^' prefix, e.g. xs.[1..^1] skips the first and last item.)</summary>
    /// <param name="startIdx">The start index (inclusive, can be negative).</param>
    /// <param name="endIdx">The end index (inclusive, can be negative).</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new array containing the sliced elements.</returns>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when either index is out of range or the start index is after the end index.</exception>
    let sliceNeg (startIdx:int) (endIdx:int) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "sliceNeg"
        arr.SliceNeg(startIdx, endIdx)

    /// <summary>Use Array.sliceNeg instead.
    /// Slice the Array given an inclusive start and end index. Allows for negative indices too. ( -1 is last item, like Python)</summary>
    /// <param name="startIdx">The start index (inclusive, can be negative).</param>
    /// <param name="endIdx">The end index (inclusive, can be negative).</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new array containing the sliced elements.</returns>
    [<Obsolete("Use Array.sliceNeg instead. The name slice is avoided because in .NET the .Slice method of some collections, like List<'T> and Span<'T>, takes a start index and a length, not an inclusive end index.")>]
    let slice (startIdx:int) (endIdx:int) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "slice"
        arr.SliceNeg(startIdx, endIdx)

    /// <summary>Returns a new Array containing the elements between the specified inclusive start and end indices.
    /// This function rejects negative and out-of-bounds indices, while the F# slicing notation xs.[1..3] does not.
    /// To allow negative indices use Array.sliceNeg, to normalize any index with modulo use Array.sliceLooped.</summary>
    /// <param name="startIdx">The inclusive start index of the slice.</param>
    /// <param name="endIdx">The inclusive end index of the slice.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new Array containing the requested range.</returns>
    /// <exception cref="T:System.IndexOutOfRangeException">Thrown when either index is outside the Array or startIdx is greater than endIdx.</exception>
    let sliceIdx (startIdx:int) (endIdx:int) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "sliceIdx"
        arr.SliceIdx(startIdx, endIdx)

    /// <summary>Returns a new Array containing the elements between the specified start and end indices after normalizing both indices with modulo.
    /// Both indices are inclusive, and negative and out-of-range indices are allowed.
    /// If the normalized start index is greater than the normalized end index, an empty Array is returned.
    /// For an empty input Array, an empty Array is returned.</summary>
    /// <param name="startIdx">The inclusive start index to normalize.</param>
    /// <param name="endIdx">The inclusive end index to normalize.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new Array containing the requested range.</returns>
    let sliceLooped (startIdx:int) (endIdx:int) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "sliceLooped"
        arr.SliceLooped(startIdx, endIdx)


    /// <summary>Trim items from start and end.
    /// If the sum of fromStartCount and fromEndCount is equal to or greater than arr.Length, it returns an empty Array.
    /// If you want an exception to be raised for index overlap (total trimming is bigger than count) use Array.sliceNeg with a negative end index.</summary>
    /// <param name="fromStartCount">The number of items to remove from the start.</param>
    /// <param name="fromEndCount">The number of items to remove from the end.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new trimmed array.</returns>
    let trim (fromStartCount:int) (fromEndCount:int) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "trim"
        if fromStartCount < 0 then fail arr $"trim: fromStartCount can't be negative: {fromStartCount}"
        if fromEndCount < 0 then fail arr $"trim: fromEndCount can't be negative: {fromEndCount}"
        let c = arr.Length
        if fromStartCount >= c || fromEndCount >= c - fromStartCount then // not 'fromStartCount + fromEndCount >= c', that could overflow
            [||]
        else
            let len = c - fromStartCount - fromEndCount
            Array.init len (fun i -> arr.[fromStartCount+i])


    //------------------------------------------------------------------
    //---------------------prev-this-next ------------------------------
    //------------------------------------------------------------------
    // these functions below also exist on Seq module in FsEx project:


    /// <summary>Yields Seq from (first, second)  up to (second-last, last).
    /// Not looped.
    /// The resulting seq is one element shorter than the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive pairs.</returns>
    let windowed2 (arr: 'T[]) : seq<'T * 'T> =
        if isNull arr then nullExn "windowed2"
        if arr.Length < 2 then fail arr "windowed2: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
        }

    /// <summary>Yields looped Seq from (first, second)  up to (last, first).
    /// The resulting seq has the same element count as the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive pairs (looped).</returns>
    let thisNext (arr: 'T[]) : seq<'T * 'T> =
        if isNull arr then nullExn "thisNext"
        if arr.Length < 2 then fail arr "thisNext: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
            arr.[arr.Length - 1], arr.[0]
        }

    /// <summary>Yields looped Seq from (last,first)  up to (second-last, last).
    /// The resulting seq has the same element count as the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive pairs (looped).</returns>
    let prevThis (arr: 'T[]) : seq<'T * 'T> =
        if isNull arr then nullExn "prevThis"
        if arr.Length < 2 then fail arr "prevThis: input has less than two items"
        seq {
            arr.[arr.Length - 1], arr.[0]
            for i = 0 to arr.Length - 2 do
                arr.[i], arr.[i + 1]
        }

    /// <summary>Yields Seq from (first, second, third)  up to (third-last, second-last, last).
    /// Not looped.
    /// The resulting seq is two elements shorter than the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive triples.</returns>
    let windowed3 (arr: 'T[]) : seq<'T * 'T * 'T> =
        if isNull arr then nullExn "windowed3"
        if arr.Length < 3 then fail arr "windowed3: input has less than three items"
        seq {
            for i = 0 to arr.Length - 3 do
                arr.[i], arr.[i + 1], arr.[i + 2]
        }

    /// <summary>Yields looped Seq of  from (last, first, second)  up to (second-last, last, first).
    /// The resulting seq has the same element count as the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive triples (looped).</returns>
    let prevThisNext (arr: 'T[]) : seq<'T * 'T * 'T> =
        if isNull arr then nullExn "prevThisNext"
        if arr.Length < 3 then fail arr "prevThisNext: input has less than three items"
        seq {
            arr.[arr.Length - 1], arr.[0], arr.[1]
            for i = 0 to arr.Length - 3 do
                arr.[i], arr.[i + 1], arr.[i + 2]
            arr.[arr.Length - 2], arr.[arr.Length - 1], arr.[0]
        }

    /// <summary>Yields Seq from (0,first, second)  up to (lastIndex-1 , second-last, last).
    /// Not looped.
    /// The resulting seq is one element shorter than the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive indexed pairs.</returns>
    let windowed2i (arr: 'T[]) : seq<int * 'T * 'T> =
        if isNull arr then nullExn "windowed2i"
        if arr.Length < 2 then fail arr "windowed2i: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                i, arr.[i], arr.[i + 1]
        }

    /// <summary>Yields looped Seq  from (0,first, second)  up to (lastIndex, last, first).
    /// The resulting seq has the same element count as the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of indexed consecutive pairs (looped).</returns>
    let iThisNext (arr: 'T[]) : seq<int * 'T * 'T> =
        if isNull arr then nullExn "iThisNext"
        if arr.Length < 2 then fail arr "iThisNext: input has less than two items"
        seq {
            for i = 0 to arr.Length - 2 do
                i, arr.[i], arr.[i + 1]
            arr.Length - 1, arr.[arr.Length - 1], arr.[0]
        }

    /// <summary>Yields Seq from (1, first, second, third)  up to (lastIndex-1 , third-last, second-last, last).
    /// Not looped.
    /// The resulting seq is two elements shorter than the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of consecutive indexed triples.</returns>
    let windowed3i (arr: 'T[]) : seq<int * 'T * 'T * 'T> =
        if isNull arr then nullExn "windowed3i"
        if arr.Length < 3 then fail arr "windowed3i: input has less than three items"
        seq {
            for i = 0 to arr.Length - 3 do
                i + 1, arr.[i], arr.[i + 1], arr.[i + 2]
        }

    /// <summary>Yields looped Seq from (1, last, first, second)  up to (lastIndex, second-last, last, first)
    /// The resulting seq has the same element count as the input Array.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A sequence of indexed consecutive triples (looped).</returns>
    let iPrevThisNext (arr: 'T[]) : seq<int * 'T * 'T * 'T> =
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
    let inline singleton (value:'T) : 'T[] =
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
        if arr.Length > 0 then
            // Normalize first so subtracting Int32.MinValue cannot overflow.
            let normalizedAmount = negIdxLooped amount arr.Length
            for i = 0 to arr.Length - 1 do
                r.[i] <- arr.[negIdxLooped (i - normalizedAmount) arr.Length]
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
            Array.copy arr
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
            Array.copy arr
        else
            let rec findBackIdx i =
                if i = -1 then
                    fail arr "rotateUpTillLast: no item in the array meets the condition"
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
            Array.copy arr
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
        elif condition arr.[arr.Length - 1] then
            Array.copy arr
        else
            let k = arr.Length
            let rec findIdx i =
                if i = k then
                    fail arr "rotateDownTillLast: no item in the array meets the condition"
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


    /// <summary>Returns true if the given Array has just one item.
    /// Same as  Array.hasOne</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has exactly one item.</returns>
    let inline isSingleton (arr: 'T[]) : bool =
        if isNull arr then nullExn "isSingleton"
        arr.Length = 1

    /// <summary>Returns true if the given Array has just one item.
    /// Same as  Array.isSingleton</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has exactly one item.</returns>
    let inline hasOne (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasOne"
        arr.Length = 1

    /// <summary>Returns true if the given Array is not empty.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has at least one item.</returns>
    let inline isNotEmpty (arr: 'T[]) : bool =
        if isNull arr then nullExn "isNotEmpty"
        arr.Length <> 0

    /// <summary>Returns true if the given Array has count items.
    /// Unlike the HasItems extension property, this function tests for an exact count.</summary>
    /// <param name="count">The exact count to check for.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has exactly the specified count.</returns>
    let inline hasItems (count:int) (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasItems"
        arr.Length = count

    /// <summary>Returns true if the given Array has equal or more than count items.</summary>
    /// <param name="count">The minimum count required.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has at least the specified count.</returns>
    let inline hasMinimumItems (count:int) (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasMinimumItems"
        arr.Length >= count

    /// <summary>Returns true if the given Array has equal or less than count items.</summary>
    /// <param name="count">The maximum count allowed.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>True if the Array has at most the specified count.</returns>
    let inline hasMaximumItems (count:int) (arr: 'T[]) : bool =
        if isNull arr then nullExn "hasMaximumItems"
        arr.Length <= count

    /// <summary>Swap the values of two given indices in Array</summary>
    /// <param name="i">The first index to swap.</param>
    /// <param name="j">The second index to swap.</param>
    /// <param name="arr">The input Array.</param>
    let inline swap (i:int) (j:int) (arr: 'T[]) : unit =
        if isNull arr then nullExn "swap"
        if i < 0 || j < 0 || i >= arr.Length || j >= arr.Length then
            fail arr $"swap: index i={i} and j={j} can't be less than 0 or bigger than last index {arr.Length - 1}"
        if i <> j then
            let ti = arr.[i]
            arr.[i] <- arr.[j]
            arr.[j] <- ti


    // internal, only for finding MinMax values
    module private MinMax =

        // funcName is the name of the public function, for the error message
        let inline simple2 (funcName: string) cmpF (arr: 'T[]) : 'T * 'T =
            if arr.Length < 2 then fail arr $"{funcName}: Count must be at least two"
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


        // A stable sorting network for three values. Since cmp is strict (< or >),
        // equal values always stay in their original order.
        // Only cmp is used, not '=', so this also works for types whose equality disagrees with their comparison.
        let inline sort3 cmp a b c : 'T * 'T * 'T =
            if cmp b a then
                if cmp c b then c, b, a
                elif cmp c a then b, c, a
                else b, a, c
            else
                if cmp c a then c, a, b
                elif cmp c b then a, c, b
                else a, b, c

        // The index counterpart of sort3, comparing projected values while keeping
        // the original index order for equal keys.
        let inline indexOfSort3By f cmp aa bb cc : int * int * int =
            let a = f aa
            let b = f bb
            let c = f cc
            if cmp b a then
                if cmp c b then 2, 1, 0
                elif cmp c a then 1, 2, 0
                else 1, 0, 2
            else
                if cmp c a then 2, 0, 1
                elif cmp c b then 0, 2, 1
                else 0, 1, 2

        let inline simple3 (funcName: string) cmpF (arr: 'T[]) : 'T * 'T * 'T =
            if arr.Length < 3 then fail arr $"{funcName}: Count must be at least three"
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

        let inline indexByFun (funcName: string) cmpF func (arr: 'T[]) : int =
            if arr.Length < 1 then fail arr $"{funcName}: Count must be at least one"
            let mutable f = func arr.[0]
            let mutable mf = f
            let mutable ii = 0
            for i = 1 to arr.Length - 1 do
                f <- func arr.[i]
                if cmpF f mf then
                    ii <- i
                    mf <- f
            ii

        let inline index2ByFun (funcName: string) cmpF func (arr: 'T[]) : int * int =
            if arr.Length < 2 then fail arr $"{funcName}: Count must be at least two"
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

        let inline index3ByFun (funcName: string) (cmpOp: 'U -> 'U -> bool) (byFun: 'T -> 'U) (arr: 'T[]) : int * int * int =
            if arr.Length < 3 then fail arr $"{funcName}: Count must be at least three"
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

    /// <summary>Returns the index of the smallest of all elements of the Array, compared via Operators.min on the function result.
    /// If several elements are equally small, the index of the first one is returned.</summary>
    /// <param name="projection">The function to transform the elements into a type supporting comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.ArgumentException">Thrown when the input Array is empty.</exception>
    /// <returns>The index of the smallest element.</returns>
    let inline minIndexBy (projection: 'T -> 'Key) (arr: 'T[]) : int =
        if isNull arr then nullExn "minIndexBy"
        arr |> MinMax.indexByFun "minIndexBy" (<) projection

    /// <summary>Returns the index of the greatest of all elements of the Array, compared via Operators.max on the function result.
    /// If several elements are equally great, the index of the first one is returned.</summary>
    /// <param name="projection">The function to transform the elements into a type supporting comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.ArgumentException">Thrown when the input Array is empty.</exception>
    /// <returns>The index of the maximum element.</returns>
    let inline maxIndexBy (projection: 'T -> 'Key) (arr: 'T[]) : int =
        if isNull arr then nullExn "maxIndexBy"
        arr |> MinMax.indexByFun "maxIndexBy" (>) projection


    /// <summary>Returns the smallest and the second smallest element of the Array.
    /// If they are equal then the order is kept</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the smallest and second smallest elements.</returns>
    let inline min2 (arr: 'T[]) : 'T * 'T =
        if isNull arr then nullExn "min2"
        arr |> MinMax.simple2 "min2" (<)

    /// <summary>Returns the biggest and the second biggest element of the Array.
    /// If they are equal then the  order is kept</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the biggest and second biggest elements.</returns>
    let inline max2 (arr: 'T[]) : 'T * 'T =
        if isNull arr then nullExn "max2"
        arr |> MinMax.simple2 "max2" (>)



    /// <summary>Returns the smallest and the second smallest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the smallest and second smallest elements.</returns>
    let inline min2By (f: 'T -> 'Key) (arr: 'T[]) : 'T * 'T =
        if isNull arr then nullExn "min2By"
        let i, ii = arr |> MinMax.index2ByFun "min2By" (<) f
        arr.[i], arr.[ii]

    /// <summary>Returns the biggest and the second biggest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the biggest and second biggest elements.</returns>
    let inline max2By (f: 'T -> 'Key) (arr: 'T[]) : 'T * 'T =
        if isNull arr then nullExn "max2By"
        let i, ii = arr |> MinMax.index2ByFun "max2By" (>) f
        arr.[i], arr.[ii]

    /// <summary>Returns the indices of the smallest and the second smallest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the indices of the smallest and second smallest elements.</returns>
    let inline min2IndicesBy (f: 'T -> 'Key) (arr: 'T[]) : int * int =
        if isNull arr then nullExn "min2IndicesBy"
        arr |> MinMax.index2ByFun "min2IndicesBy" (<) f

    /// <summary>Returns the indices of the biggest and the second biggest element of the Array.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the indices of the biggest and second biggest elements.</returns>
    let inline max2IndicesBy (f: 'T -> 'Key) (arr: 'T[]) : int * int =
        if isNull arr then nullExn "max2IndicesBy"
        arr |> MinMax.index2ByFun "max2IndicesBy" (>) f


    /// <summary>Returns the smallest three elements of the Array.
    /// The first element is the smallest, the second is the second smallest and the third is the third smallest.
    /// If they are equal then the order is kept</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the three smallest elements.</returns>
    let inline min3 (arr: 'T[]) : 'T * 'T * 'T =
        if isNull arr then nullExn "min3"
        arr |> MinMax.simple3 "min3" (<)

    /// <summary>Returns the biggest three elements of the Array.
    /// The first element is the biggest, the second is the second biggest and the third is the third biggest.
    /// If they are equal then the order is kept</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the three biggest elements.</returns>
    let inline max3 (arr: 'T[]) : 'T * 'T * 'T =
        if isNull arr then nullExn "max3"
        arr |> MinMax.simple3 "max3" (>)

    /// <summary>Returns the smallest three elements of the Array.
    /// The first element is the smallest, the second is the second smallest and the third is the third smallest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the three smallest elements.</returns>
    let inline min3By (f: 'T -> 'Key) (arr: 'T[]) : 'T * 'T * 'T =
        if isNull arr then nullExn "min3By"
        let i, ii, iii = arr |> MinMax.index3ByFun "min3By" (<) f
        arr.[i], arr.[ii], arr.[iii]

    /// <summary>Returns the biggest three elements of the Array.
    /// The first element is the biggest, the second is the second biggest and the third is the third biggest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the three biggest elements.</returns>
    let inline max3By (f: 'T -> 'Key) (arr: 'T[]) : 'T * 'T * 'T =
        if isNull arr then nullExn "max3By"
        let i, ii, iii = arr |> MinMax.index3ByFun "max3By" (>) f
        arr.[i], arr.[ii], arr.[iii]

    /// <summary>Returns the indices of the three smallest elements of the Array.
    /// The first element is the index of the smallest, the second is the index of the second smallest and the third is the index of the third smallest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the indices of the three smallest elements.</returns>
    let inline min3IndicesBy (f: 'T -> 'Key) (arr: 'T[]) : int * int * int =
        if isNull arr then nullExn "min3IndicesBy"
        arr |> MinMax.index3ByFun "min3IndicesBy" (<) f

    /// <summary>Returns the indices of the three biggest elements of the Array.
    /// The first element is the index of the biggest, the second is the index of the second biggest and the third is the index of the third biggest.
    /// Elements are compared by applying the predicate function first.
    /// If they are equal after function is applied then the order is kept</summary>
    /// <param name="f">The function to transform elements for comparison.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of the indices of the three biggest elements.</returns>
    let inline max3IndicesBy (f: 'T -> 'Key) (arr: 'T[]) : int * int * int =
        if isNull arr then nullExn "max3IndicesBy"
        arr |> MinMax.index3ByFun "max3IndicesBy" (>) f


    /// <summary>Return the length or count of the collection.
    /// Same as Array.length</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The number of items in the Array.</returns>
    let inline count (arr: 'T[]) : int =
        if isNull arr then nullExn "count"
        arr.Length

    /// <summary>Counts for how many items of the collection the predicate returns true.
    /// Same as Array.filter and then Array.length</summary>
    /// <param name="predicate">The function to test each element.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The count of items for which the predicate returns true.</returns>
    let inline countIf (predicate: 'T -> bool) (arr: 'T[]) : int = //countBy is something else !!
        if isNull arr then nullExn "countIf"
        let mutable k = 0
        for i = 0 to arr.Length - 1 do
            if predicate arr.[i] then k <- k + 1
        k


    /// <summary>Builds a new Array from the given ResizeArray.</summary>
    /// <param name="rarr">The input ResizeArray.</param>
    /// <returns>A new array containing the elements from the ResizeArray.</returns>
    let inline ofResizeArray (rarr: ResizeArray<'T>) : 'T[] =
        if isNull rarr then nullExn "ofResizeArray"
        rarr.ToArray()


    /// <summary>an optimized alternative to the <code>toResizeArray</code> function for use in Fable (JavaScript).
    /// F# Array and ResizeArray are both represented as JavaScript arrays in Fable.
    /// So this function does not allocate a new ResizeArray but just casts the Array to a ResizeArray.
    /// In .NET runtime a new ResizeArray is still allocated and the elements are copied.</summary>
    /// <remarks>Numeric arrays are optimized as TypedArrays in Fable, so this function only works on reference types.</remarks>
    /// <param name="arr">The input Array.</param>
    /// <returns>A ResizeArray containing the same elements.</returns>
    let inline asResizeArray(arr: 'T[]) : ResizeArray<'T> when 'T : not struct =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            if isNull arr then nullExn "asResizeArray"
            unbox<ResizeArray<'T>> arr
        #else
            if isNull arr then nullExn "asResizeArray"
            ResizeArray<'T> arr
        #endif


    /// <summary>Builds a new ResizeArray that contains a copy of the elements of the input Array.
    /// This function always allocates a new ResizeArray and copies the elements.
    /// (In Fable, Array.asResizeArray avoids the copy for reference types.)</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>A ResizeArray containing a copy of the Array elements.</returns>
    let inline toResizeArray (arr: 'T[]) : ResizeArray<'T> =
        if isNull arr then nullExn "toResizeArray"
        ResizeArray<'T>(arr)


    /// <summary>Build a Array from the given IList Interface.</summary>
    /// <param name="arr">The input IList.</param>
    /// <returns>A new array containing the elements from the IList.</returns>
    let inline ofIList (arr: IList<'T>) : 'T[] =
        if isNull arr then nullExn "ofIList"
        let l = Array.zeroCreate (arr.Count)
        arr.CopyTo(l, 0)
        l


    /// <summary>Splits the collection into two collections, containing the elements for which the
    /// given function returns <c>Choice1Of2</c> or <c>Choice2Of2</c>, respectively. This function is similar to
    /// <c>Array.partition</c>, but it allows the returned collections to have different element types.</summary>
    /// <param name="partitioner">The function to transform and classify each input element into one of two output types.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of two Arrays. The first containing values from Choice1Of2 results and the second
    /// containing values from Choice2Of2 results.</returns>
    let inline partitionBy (partitioner: 'T -> Choice<'U1,'U2>) (arr: 'T[]) : 'U1[] * 'U2[] =
        if isNull arr then nullExn "partitionBy"
        let results1 = ResizeArray()
        let results2 = ResizeArray()
        for i = 0 to arr.Length - 1 do
            match partitioner arr.[i] with
            | Choice1Of2 value -> results1.Add value
            | Choice2Of2 value -> results2.Add value
        results1.ToArray(), results2.ToArray()

    /// <summary>Splits the collection into two Arrays, by applying the given partitioning function
    /// to each element. Returns Choice1Of2 elements in the first Array and
    /// Choice2Of2 elements in the second Array. Element order is preserved in both of the created Arrays.
    /// This is the same function as Array.partitionBy, provided under the name
    /// used by the F# core <c>Array</c> module since FSharp.Core 10.1.</summary>
    /// <param name="partitioner">The function to transform and classify each input element into one of two output types.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A tuple of two Arrays. The first containing values from Choice1Of2 results and the second
    /// containing values from Choice2Of2 results.</returns>
    let inline partitionWith (partitioner: 'T -> Choice<'U1, 'U2>) (arr: 'T[]) : 'U1[] * 'U2[] =
        if isNull arr then nullExn "partitionWith"
        partitionBy partitioner arr

    /// <summary>Splits the collection into three collections, containing the elements for which the
    /// given function returns <c>Choice1Of3</c>, <c>Choice2Of3</c> or <c>Choice3Of3</c>, respectively. This function is similar to
    /// <c>Array.partition3</c>, but it allows the returned collections to have different element types.</summary>
    /// <param name="partitioner">The function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Three Arrays.</returns>
    let partition3By (partitioner: 'T -> Choice<'U1,'U2,'U3>) (arr: 'T[]) : 'U1[] * 'U2[] * 'U3[] =
        if isNull arr then nullExn "partition3By"
        let results1 = ResizeArray()
        let results2 = ResizeArray()
        let results3 = ResizeArray()
        for i = 0 to arr.Length - 1 do
            match partitioner arr.[i] with
            | Choice1Of3 value -> results1.Add value
            | Choice2Of3 value -> results2.Add value
            | Choice3Of3 value -> results3.Add value
        results1.ToArray(), results2.ToArray(), results3.ToArray()

    /// <summary>Splits the collection into four collections, containing the elements for which the
    /// given function returns <c>Choice1Of4</c>, <c>Choice2Of4</c>, <c>Choice3Of4</c> or <c>Choice4Of4</c>, respectively. This function is similar to
    /// <c>Array.partition4</c>, but it allows the returned collections to have different element types.</summary>
    /// <param name="partitioner">The function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Four Arrays.</returns>
    let partition4By (partitioner: 'T -> Choice<'U1,'U2,'U3,'U4>) (arr: 'T[]) : 'U1[] * 'U2[] * 'U3[] * 'U4[] =
        if isNull arr then nullExn "partition4By"
        let results1 = ResizeArray()
        let results2 = ResizeArray()
        let results3 = ResizeArray()
        let results4 = ResizeArray()
        for i = 0 to arr.Length - 1 do
            match partitioner arr.[i] with
            | Choice1Of4 value -> results1.Add value
            | Choice2Of4 value -> results2.Add value
            | Choice3Of4 value -> results3.Add value
            | Choice4Of4 value -> results4.Add value
        results1.ToArray(), results2.ToArray(), results3.ToArray(), results4.ToArray()

    /// <summary>Splits the collection into five collections, containing the elements for which the
    /// given function returns <c>Choice1Of5</c>, <c>Choice2Of5</c>, <c>Choice3Of5</c>, <c>Choice4Of5</c> or <c>Choice5Of5</c>, respectively. This function is similar to
    /// <c>Array.partition5</c>, but it allows the returned collections to have different element types.</summary>
    /// <param name="partitioner">The function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Five Arrays.</returns>
    let partition5By (partitioner: 'T -> Choice<'U1,'U2,'U3,'U4,'U5>) (arr: 'T[]) : 'U1[] * 'U2[] * 'U3[] * 'U4[] * 'U5[] =
        if isNull arr then nullExn "partition5By"
        let results1 = ResizeArray()
        let results2 = ResizeArray()
        let results3 = ResizeArray()
        let results4 = ResizeArray()
        let results5 = ResizeArray()
        for i = 0 to arr.Length - 1 do
            match partitioner arr.[i] with
            | Choice1Of5 value -> results1.Add value
            | Choice2Of5 value -> results2.Add value
            | Choice3Of5 value -> results3.Add value
            | Choice4Of5 value -> results4.Add value
            | Choice5Of5 value -> results5.Add value
        results1.ToArray(), results2.ToArray(), results3.ToArray(), results4.ToArray(), results5.ToArray()

    /// <summary>Splits the collection into three collections,
    /// first containing the elements for which the given predicate1 returns <c>true</c>,
    /// second containing the elements for which the given predicate2 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// third the rest.</summary>
    /// <param name="predicate1">The first function to test the input elements.</param>
    /// <param name="predicate2">The second function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Three Arrays.</returns>
    let partition3 (predicate1: 'T -> bool) (predicate2: 'T -> bool) (arr: 'T[]) : 'T[] * 'T[] * 'T[] =
        if isNull arr then nullExn "partition3"
        let p1True = ResizeArray()
        let p2True = ResizeArray()
        let allFalse = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let el = arr.[i]
            if predicate1 el then p1True.Add el
            elif predicate2 el then p2True.Add el
            else allFalse.Add el
        p1True.ToArray(), p2True.ToArray(), allFalse.ToArray()

    /// <summary>Splits the collection into four collections,
    /// first containing the elements for which the given predicate1 returns <c>true</c>,
    /// second containing the elements for which the given predicate2 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// third containing the elements for which the given predicate3 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// fourth the rest.</summary>
    /// <param name="predicate1">The first function to test the input elements.</param>
    /// <param name="predicate2">The second function to test the input elements.</param>
    /// <param name="predicate3">The third function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Four Arrays.</returns>
    let partition4 (predicate1: 'T -> bool) (predicate2: 'T -> bool) (predicate3: 'T -> bool) (arr: 'T[]) : 'T[] * 'T[] * 'T[] * 'T[] =
        if isNull arr then nullExn "partition4"
        let p1True = ResizeArray()
        let p2True = ResizeArray()
        let p3True = ResizeArray()
        let allFalse = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let el = arr.[i]
            if predicate1 el then p1True.Add el
            elif predicate2 el then p2True.Add el
            elif predicate3 el then p3True.Add el
            else allFalse.Add el
        p1True.ToArray(), p2True.ToArray(), p3True.ToArray(), allFalse.ToArray()

    /// <summary>Splits the collection into five collections,
    /// first containing the elements for which the given predicate1 returns <c>true</c>,
    /// second containing the elements for which the given predicate2 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// third containing the elements for which the given predicate3 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// fourth containing the elements for which the given predicate4 returns <c>true</c> (and all previous predicates returned <c>false</c>),
    /// fifth the rest.</summary>
    /// <param name="predicate1">The first function to test the input elements.</param>
    /// <param name="predicate2">The second function to test the input elements.</param>
    /// <param name="predicate3">The third function to test the input elements.</param>
    /// <param name="predicate4">The fourth function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>Five Arrays.</returns>
    let partition5 (predicate1: 'T -> bool) (predicate2: 'T -> bool) (predicate3: 'T -> bool) (predicate4: 'T -> bool) (arr: 'T[]) : 'T[] * 'T[] * 'T[] * 'T[] * 'T[] =
        if isNull arr then nullExn "partition5"
        let p1True = ResizeArray()
        let p2True = ResizeArray()
        let p3True = ResizeArray()
        let p4True = ResizeArray()
        let allFalse = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let el = arr.[i]
            if predicate1 el then p1True.Add el
            elif predicate2 el then p2True.Add el
            elif predicate3 el then p3True.Add el
            elif predicate4 el then p4True.Add el
            else allFalse.Add el
        p1True.ToArray(), p2True.ToArray(), p3True.ToArray(), p4True.ToArray(), allFalse.ToArray()


    /// <summary>Applies a function to array
    /// If resulting array meets the resultPredicate it is returned, otherwise the original input is returned.</summary>
    /// <param name="resultPredicate">The predicate to test the result.</param>
    /// <param name="transform">The transformation function to apply.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The transformed array if it meets the predicate, otherwise the original array.</returns>
    let inline mapIfResult (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "mapIfResult"
        let r = transform arr
        if resultPredicate r then r else arr

    /// <summary>Applies a function to array if it meets the inputPredicate, otherwise just returns input.
    /// If resulting array meets the resultPredicate it is returned, otherwise original input is returned.</summary>
    /// <param name="inputPredicate">The predicate to test the input array.</param>
    /// <param name="resultPredicate">The predicate to test the result.</param>
    /// <param name="transform">The transformation function to apply.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The transformed array if conditions are met, otherwise the original array.</returns>
    let inline mapIfInputAndResult (inputPredicate: 'T[] -> bool) (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "mapIfInputAndResult"
        if inputPredicate arr then
            let r = transform arr
            if resultPredicate r then r else arr
        else
            arr


    [<Obsolete("Use mapIfResult instead")>]
    let inline applyIfResult (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "applyIfResult"
        let r = transform arr
        if resultPredicate r then r else arr

    [<Obsolete("Use mapIfInputAndResult instead")>]
    let inline applyIfInputAndResult (inputPredicate: 'T[] -> bool) (resultPredicate: 'T[] -> bool) (transform: 'T[] ->  'T[]) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "applyIfInputAndResult"
        if inputPredicate arr then
            let r = transform arr
            if resultPredicate r then r else arr
        else
            arr


    /// <summary>Returns all elements that exist more than once in Array.
    /// Each element that exists more than once is only returned once.
    /// The returned item is the second occurrence, where the duplicate is first detected.
    /// Returned order is by position of that second occurrence.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>An array of duplicate elements.</returns>
    let duplicates (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "duplicates"
        let h = HashSet<'T>()
        let t = HashSet<'T>()
        // first Add should be false, second Add true, to recognize the first occurrence of a duplicate:
        arr |> Array.filter (fun x -> if h.Add x then false else t.Add x)

    /// <summary>Returns all elements whose projected value exists more than once in Array.
    /// Each projected value that exists more than once is only returned once.
    /// The returned item is the second occurrence, where the duplicate is first detected.
    /// Returned order is by position of that second occurrence.</summary>
    /// <param name="f">The function to extract comparison value from each element.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>An array of duplicate elements.</returns>
    let duplicatesBy (f: 'T -> 'U) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "duplicatesBy"
        let h = HashSet<'U>()
        let t = HashSet<'U>()
        // first Add should be false, second Add true, to recognize the first occurrence of a duplicate:
        arr |> Array.filter (fun x -> let y = f x in if h.Add y then false else t.Add y)


    /// <summary>Checks if a given array matches the content in Array at a given index</summary>
    /// <param name="searchFor">The array pattern to search for.</param>
    /// <param name="atIdx">The index to start the search.</param>
    /// <param name="searchIn">The array to search in.</param>
    /// <returns>True if the pattern matches at the given index.</returns>
    let matches (searchFor:'T[]) (atIdx:int) (searchIn:'T[]) : bool =
        if isNull searchFor then nullExn "matches"
        if isNull searchIn then nullExn "matches"
        if searchFor.Length = 0 then fail searchIn "matches: the searchFor array is empty"
        if atIdx < 0               then fail searchIn <| sprintf "matches: atIdx Index is too small: %d for array of %d items" atIdx searchIn.Length
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


    /// <summary>Find first index where searchFor occurs in searchIn array.
    /// Give lower and upper bound index for search space.
    /// Returns -1 if not found</summary>
    /// <param name="searchFor">The value to search for.</param>
    /// <param name="fromIdx">The starting index for the search.</param>
    /// <param name="tillIdx">The ending index for the search.</param>
    /// <param name="searchIn">The array to search in.</param>
    /// <returns>The index of the first occurrence, or -1 if not found.</returns>
    let findValue (searchFor:'T) (fromIdx:int) (tillIdx:int) (searchIn:'T[]) : int =
        if isNull searchIn then nullExn "findValue"
        if fromIdx < 0                then fail searchIn <| sprintf "findValue: fromIdx Index is too small: %d for array of %d items" fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findValue: tillIdx Index is too big:   %d for array of %d items" tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findValue: tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" tillIdx fromIdx searchIn.Length
        let rec find i  =
            if  i > tillIdx  then -1 // not found!
            elif searchIn.[i] = searchFor  then i  // found,  exit !
            else find (i + 1)
        find fromIdx


    /// <summary>Find last index where searchFor occurs in searchIn array. Searching from end.
    /// Give lower and upper bound index for search space.
    /// Returns -1 if not found</summary>
    /// <param name="searchFor">The value to search for.</param>
    /// <param name="fromIdx">The starting index for the search.</param>
    /// <param name="tillIdx">The ending index for the search.</param>
    /// <param name="searchIn">The array to search in.</param>
    /// <returns>The index of the last occurrence, or -1 if not found.</returns>
    let findLastValue (searchFor:'T) (fromIdx:int) (tillIdx:int) (searchIn:'T[]) : int =
        if isNull searchIn then nullExn "findLastValue"
        if fromIdx < 0                then fail searchIn <| sprintf "findLastValue: fromIdx Index is too small: %d for array of %d items" fromIdx searchIn.Length
        if tillIdx >= searchIn.Length then fail searchIn <| sprintf "findLastValue: tillIdx Index is too big:   %d for array of %d items" tillIdx searchIn.Length
        if tillIdx < fromIdx          then fail searchIn <| sprintf "findLastValue: tillIdx Index %d is smaller than fromIdx Index %d for array of %d items" tillIdx fromIdx searchIn.Length
        let rec find i  =
            if  i < fromIdx  then -1 // not found!
            elif searchIn.[i] = searchFor  then i  // found,  exit !
            else find (i - 1)
        find tillIdx


    /// <summary>Find first index where searchFor array occurs in searchIn array.
    /// Give lower and upper bound index for search space.
    /// Returns index of first element or -1 if not found</summary>
    /// <param name="searchFor">The array pattern to search for.</param>
    /// <param name="fromIdx">The starting index for the search.</param>
    /// <param name="tillIdx">The ending index for the search.</param>
    /// <param name="searchIn">The array to search in.</param>
    /// <returns>The index of the first occurrence, or -1 if not found.</returns>
    let findArray (searchFor:'T[]) (fromIdx:int) (tillIdx:int) (searchIn:'T[]) : int =
        if isNull searchIn then nullExn "findArray"
        if isNull searchFor then nullExn "findArray"
        if searchFor.Length = 0 then fail searchIn "findArray: the searchFor array is empty"
        if fromIdx < 0               then fail searchIn <| sprintf "findArray (of %d items): fromIdx Index is too small: %d for array of %d items" searchFor.Length fromIdx searchIn.Length
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



    /// <summary>Find last index where searchFor array occurs in searchIn array. Searching from end.
    /// Give lower and upper bound index for search space.
    /// Returns index of first element  or -1 if not found</summary>
    /// <param name="searchFor">The array pattern to search for.</param>
    /// <param name="fromIdx">The starting index for the search.</param>
    /// <param name="tillIdx">The ending index for the search.</param>
    /// <param name="searchIn">The array to search in.</param>
    /// <returns>The index of the last occurrence, or -1 if not found.</returns>
    let findLastArray (searchFor:'T[]) (fromIdx:int) (tillIdx:int) (searchIn:'T[]) : int =
        if isNull searchIn then nullExn "findLastArray"
        if isNull searchFor then nullExn "findLastArray"
        if searchFor.Length = 0 then fail searchIn "findLastArray: the searchFor array is empty"
        if fromIdx < 0               then fail searchIn <| sprintf "findLastArray (of %d items): fromIdx Index is too small: %d for array of %d items" searchFor.Length fromIdx searchIn.Length
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
    /// for which the given predicate, called with the index and the element, returns <c>true</c>.</summary>
    /// <param name="predicate">The function to test the index and the element.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>An array containing the elements for which the given predicate returns true.</returns>
    let filteri (predicate: int -> 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "filteri"
        let res = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let t = arr.[i]
            if predicate i t then
                res.Add(t)
        res.ToArray()

    /// <summary>Returns the zero-based index of the first element in the Array that satisfies the given indexed predicate.</summary>
    /// <param name="predicate">The function to test each indexed element against.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The index of the first element that satisfies the predicate, or None if not found.</returns>
    let inline tryFindIndexi (predicate: int -> 'T -> bool) (arr: 'T[]) : option<int> =
        if isNull arr then nullExn "tryFindIndexi"
        let mutable i = 0
        let mutable result = None
        let k = arr.Length
        while i < k do
            if predicate i arr.[i] then
                result <- Some i
                i <- k // break the loop
            else
                i <- i + 1
        result

    /// <summary>Returns the zero-based index of the first element in the Array that satisfies the given indexed predicate.</summary>
    /// <param name="predicate">The function to test each indexed element against.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The index of the first element that satisfies the predicate.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown when no element satisfies the predicate.</exception>
    let findIndexi (predicate: int -> 'T -> bool) (arr: 'T[]) : int =
        if isNull arr then nullExn "findIndexi"
        match tryFindIndexi predicate arr with
        | Some i -> i
        | None -> failKey arr "findIndexi did not find for given predicate in"

    /// <summary>Treats the Array as a loop.
    /// For each element, it gets the Prev-This and the This-Next combination using the combineAdjacent function.
    /// Finally creates a new type using itself and the Prev-This and the This-Next combination for each element.
    /// The first element uses the last element as previous, and the last element uses the first element as next.
    /// On each element it caches the This-Next combination and uses it for the next element as Prev-This.</summary>
    /// <param name="combineAdjacent">The function to combine two adjacent elements.</param>
    /// <param name="mergePrevAndNextCombineResults">The function to create the result from the element, its Prev-This and its This-Next combination.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A new Array with one result per input element.</returns>
    let mapPrevNext (combineAdjacent: 'T -> 'T -> 'U) (mergePrevAndNextCombineResults: 'T -> 'U -> 'U -> 'V) (arr: 'T[]) : 'V[] =
        if isNull arr then nullExn "mapPrevNext"
        let len = arr.Length
        if len = 0 then
            [||]
        else
            let res = Array.zeroCreate len
            let mutable this = arr.[0]
            let mutable prevCombined = combineAdjacent arr.[len - 1] this
            for i = 1 to len - 1 do
                let next = arr.[i]
                let nextCombined = combineAdjacent this next
                res.[i - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
                this <- next
                prevCombined <- nextCombined
            // close the loop
            let nextCombined = combineAdjacent this arr.[0]
            res.[len - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
            res

    /// <summary>Returns a tuple of the first element of the Array and a new Array containing the remaining elements.</summary>
    /// <param name="arr">The input Array.</param>
    /// <returns>The first element and a new Array with the remaining elements.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when the input Array is empty.</exception>
    let inline headAndTail (arr: 'T[]) : 'T * 'T[] =
        if isNull arr then nullExn "headAndTail"
        if arr.Length = 0 then fail arr "headAndTail: input is empty"
        arr.[0], Array.sub arr 1 (arr.Length - 1)

    /// <summary>Tests if none of the elements of the Array satisfies the given predicate.
    /// The predicate is applied to the elements of the input Array. If any application
    /// returns true then the overall result is <c>false</c> and no further elements are tested.
    /// Otherwise, true is returned.</summary>
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns><c>false</c> if any result from <c>predicate</c> is <c>true</c>.</returns>
    let notExists (predicate: 'T -> bool) (arr: 'T[]) : bool =
        if isNull arr then nullExn "notExists"
        not (Array.exists predicate arr)

    /// <summary>Starting from last element going backwards. Applies the given function to successive elements, returning the first
    /// result where function returns <c>Some(x)</c> for some <c>x</c>. If the function
    /// never returns <c>Some(x)</c> then <see cref="T:System.Collections.Generic.KeyNotFoundException"/> is raised.</summary>
    /// <param name="chooser">The function to generate options from the elements.</param>
    /// <param name="arr">The input Array.</param>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown if every result from
    /// <c>chooser</c> is <c>None</c>.</exception>
    /// <returns>The first result. From the end of the Array.</returns>
    let pickBack (chooser: 'T -> 'U option) (arr: 'T[]) : 'U =
        if isNull arr then nullExn "pickBack"
        let mutable i = arr.Length - 1
        let mutable result = None
        while i >= 0 && result.IsNone do
            result <- chooser arr.[i]
            i <- i - 1
        match result with
        | Some res -> res
        | None -> failKey arr $"pickBack: Key not found in {arr.Length} elements"

    /// <summary>Applies the given function to successive elements from the end of the Array, returning the first
    /// result where function returns <c>Some(x)</c> for some <c>x</c>. If the function
    /// never returns <c>Some(x)</c> then <c>None</c> is returned.</summary>
    /// <param name="chooser">The function to transform the Array elements into options.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>The first transformed element searched from the end of the Array that is <c>Some(x)</c>.</returns>
    let tryPickBack (chooser: 'T -> 'U option) (arr: 'T[]) : 'U option =
        if isNull arr then nullExn "tryPickBack"
        let mutable i = arr.Length - 1
        let mutable result = None
        while i >= 0 && result.IsNone do
            result <- chooser arr.[i]
            i <- i - 1
        result

    /// <summary>Like Array.zip, but when one of the two input Arrays is exhausted, the getDefaultVal function is used for the rest of the output.</summary>
    /// <param name="getDefaultVal">A function that takes the current index and the current value of the longer Array and returns a default value for the shorter Array.</param>
    /// <param name="arr1">The first input Array.</param>
    /// <param name="arr2">The second input Array.</param>
    /// <returns>The Array of tupled elements, as long as the longer input Array.</returns>
    let zipDefault (getDefaultVal: int -> 'T -> 'T) (arr1: 'T[]) (arr2: 'T[]) : ('T * 'T)[] =
        if isNull arr1 then nullExn "zipDefault first"
        if isNull arr2 then nullExn "zipDefault second"
        let len1 = arr1.Length
        let len2 = arr2.Length
        Array.init (max len1 len2) (fun i ->
            if i >= len2 then // first is longer
                let x = arr1.[i]
                x, getDefaultVal i x
            elif i >= len1 then // second is longer
                let x = arr2.[i]
                getDefaultVal i x, x
            else
                arr1.[i], arr2.[i]
            )

    /// <summary>Applies a key-generating function to each element of an Array and yields a Dictionary of
    /// unique keys and respective elements that match to this key.
    /// Uses F# structural equality for grouping and dictionary lookups on both .NET and Fable.
    /// Keys must support equality. Element order within each group is preserved.
    /// As opposed to Array.groupBy the key may not be null or Option.None.</summary>
    /// <param name="projection">A function that transforms an element of the Array into a key supporting equality. Null and Option.None keys are rejected.</param>
    /// <param name="arr">The input Array.</param>
    /// <returns>A Dictionary containing each unique key and an Array of its matching elements.</returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown when the input Array is null or a projected key is null or Option.None.</exception>
    let groupByDict (projection: 'T -> 'Key) (arr: 'T[]) : Dictionary<'Key, 'T[]> =
        if isNull arr then nullExn "groupByDict"
        let comparer = HashIdentity.Structural<'Key>
        let groups = Dictionary<'Key, ResizeArray<'T>>(comparer)
        for i = 0 to arr.Length - 1 do
            let v = arr.[i]
            let k = projection v
            if isNull (box k) then
                raise (ArgumentNullException("projection", "Array.groupByDict: the projected key is null or None."))
            match groups.TryGetValue k with
            | true, r -> r.Add v
            | _ ->
                let r = ResizeArray()
                groups.[k] <- r
                r.Add v
        let dict = Dictionary<'Key, 'T[]>(groups.Count, comparer)
        for kv in groups do
            dict.[kv.Key] <- kv.Value.ToArray()
        dict




