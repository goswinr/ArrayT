namespace ArrayT

open System
open System.Collections.Generic

#nowarn "44" //for opening the hidden but not Obsolete UtilArray module
open UtilArray
#warnon "44"

/// Extension methods for Array<'T>.
/// Like .Last , .IsNotEmpty ,..
/// This module is automatically opened when the namespace ArrayT is opened.
[<AutoOpen>]
module AutoOpenArrayTExtensions =

    type ``[]``<'T>  with //Generic Array

        /// Use for Debugging index get/set operations.
        /// Just replace 'myArray.[3]' with 'myArray.DebugIdx.[3]'
        /// Throws a nice descriptive Exception if the index is out of range
        /// including the bad index and the array content.
        member xs.DebugIdx : DebugIndexer<'T> =
            new DebugIndexer<'T>(xs)

        /// <summary>Gets an item at index, same as this.[index] or this.Idx(index)
        /// Throws a descriptive Exception if the index is out of range
        /// including the bad index and the Array content.
        /// (Use this.GetNeg(i) member if you want to use negative indices too)</summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The value at the specified index.</returns>
        member inline xs.Get index : 'T =
            #if UNCHECKED
                getUnCkd index xs
            #else
                if index < 0 || index >= xs.Length then badGetExn index xs "Get"
                xs.[index]
            #endif

        /// <summary>Gets an item at index, same as this.[index] or this.Get(index)
        /// Throws a descriptive Exception if the index is out of range.
        /// (Use this.GetNeg(i) member if you want to use negative indices too)</summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The value at the specified index.</returns>
        member inline xs.Idx index : 'T =
            #if UNCHECKED
                getUnCkd index xs
            #else
                if index < 0 || index >= xs.Length then badGetExn index xs "Idx"
                xs.[index]
            #endif

        /// <summary>Sets an item at index
        /// (Use this.SetNeg(i) member if you want to use negative indices too)</summary>
        /// <param name="index">The index to set.</param>
        /// <param name="value">The value to set.</param>
        member inline xs.Set index value : unit =
            #if UNCHECKED
                setUnCkd index value xs
            #else
                if index < 0 || index >= xs.Length then badSetExn index xs "Set" value
                xs.[index] <- value
            #endif


        /// Gets the index of the last item in the Array.
        /// Equal to this.Length - 1
        /// Returns -1 for empty Array.
        member inline xs.LastIndex : int =
            // don't fail so that a loop for i=0 to xs.LastIndex will work for empty Array
            //if xs.Length = 0 then IndexOutOfRangeException.Raise "array.LastIndex: Failed to get LastIndex of empty %s" xs.ToNiceStringLong // Array<%s>" (typeof<'T>).FullName
            xs.Length - 1

        /// <summary>Get (or set) the last item in the Array.
        /// Equal to this.[this.Length - 1]</summary>
        member inline xs.Last
            with get () : 'T =
                if xs.Length = 0 then badGetExn xs.LastIndex xs "Last"
                xs.[xs.Length - 1]
            and set (v: 'T) : unit =
                if xs.Length = 0 then badSetExn xs.LastIndex xs "Last" v
                xs.[xs.Length - 1] <- v

        /// <summary>Get (or set) the second last item in the Array.
        /// Equal to this.[this.Length - 2]</summary>
        member inline xs.SecondLast
            with get () : 'T =
                if xs.Length < 2 then badGetExn (xs.Length - 2) xs "SecondLast"
                xs.[xs.Length - 2]
            and set (v: 'T) : unit =
                if xs.Length < 2 then badSetExn (xs.Length - 2) xs "SecondLast" v
                xs.[xs.Length - 2] <- v


        /// <summary>Get (or set) the third last item in the Array.
        /// Equal to this.[this.Length - 3]</summary>
        member inline xs.ThirdLast
            with get () : 'T =
                if xs.Length < 3 then badGetExn (xs.Length - 3) xs "ThirdLast"
                xs.[xs.Length - 3]
            and set (v: 'T) : unit =
                if xs.Length < 3 then badSetExn (xs.Length - 3) xs "ThirdLast" v
                xs.[xs.Length - 3] <- v

        /// <summary>Get (or set) the first item in the Array.
        /// Equal to this.[0]</summary>
        member inline xs.First
            with get () : 'T =
                if xs.Length = 0 then badGetExn 0 xs "First"
                xs.[0]
            and set (v: 'T) : unit =
                if xs.Length = 0 then badSetExn 0 xs "First" v
                xs.[0] <- v

        /// Gets the the only item in the Array.
        /// Fails if the Array does not have exactly one element.
        member inline xs.FirstAndOnly : 'T =
            if xs.Length = 0 then badGetExn 0 xs "FirstAndOnly"
            if xs.Length > 1 then badGetExn 1 xs "FirstAndOnly, Array is expected to have exactly one item."
            xs.[0]


        /// <summary>Get (or set) the second item in the Array.
        /// Equal to this.[1]</summary>
        member inline xs.Second
            with get () : 'T =
                if xs.Length < 2 then badGetExn 1 xs "Second"
                xs.[1]
            and set (v: 'T) : unit =
                if xs.Length < 2 then badSetExn 1 xs "Second" v
                xs.[1] <- v

        /// <summary>Get (or set) the third item in the Array.
        /// Equal to this.[2]</summary>
        member inline xs.Third
            with get () : 'T =
                if xs.Length < 3 then badGetExn 2 xs "Third"
                xs.[2]
            and set (v: 'T) : unit =
                if xs.Length < 3 then badSetExn 2 xs "Third" v
                xs.[2] <- v

        /// Checks if this.Length = 0
        member inline xs.IsEmpty : bool =
            xs.Length = 0


        /// Checks if this.Length = 1
        member inline xs.IsSingleton : bool =
            xs.Length = 1

        /// Checks if this.Length > 0
        /// Same as xs.HasItems
        member inline xs.IsNotEmpty : bool =
            xs.Length > 0

        /// Checks if this.Length > 0
        /// Same as xs.IsNotEmpty
        member inline xs.HasItems : bool =
            xs.Length > 0


        /// <summary>Gets an item in the Array by index.
        /// Allows for negative index too ( -1 is last item,  like Python)
        /// (From the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)</summary>
        /// <param name="index">The index to access (can be negative).</param>
        /// <returns>The value at the specified index.</returns>
        member inline xs.GetNeg index : 'T =
            let len = xs.Length
            let ii = if index < 0 then len + index else index
            if ii < 0 || ii >= len then badGetExn index xs "GetNeg"
            xs.[ii]

        /// <summary>Sets an item in the Array by index.
        /// Allows for negative index too ( -1 is last item,  like Python)
        /// (from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)</summary>
        /// <param name="index">The index to set (can be negative).</param>
        /// <param name="value">The value to set.</param>
        member inline xs.SetNeg index value : unit =
            let len = xs.Length
            let ii = if index < 0 then len + index else index
            if ii < 0 || ii >= len then badSetExn index xs "SetNeg" value
            xs.[ii] <- value

        /// <summary>Any index will return a value.
        /// Array is treated as an endless loop in positive and negative direction</summary>
        /// <param name="index">The index to access (can be any integer).</param>
        /// <returns>The value at the looped index.</returns>
        member inline xs.GetLooped index : 'T =
            let len = xs.Length
            if len = 0 then badGetExn index xs "GetLooped"
            let t = index % len
            let ii = if t >= 0 then t else t + len
            xs.[ii]

        /// <summary>Any index will set a value.
        /// Array is treated as an endless loop in positive and negative direction</summary>
        /// <param name="index">The index to set (can be any integer).</param>
        /// <param name="value">The value to set.</param>
        member inline xs.SetLooped index value : unit =
            let len = xs.Length
            if len = 0 then badSetExn index xs "SetLooped" value
            let t = index % len
            let ii = if t >= 0 then t else t + len
            xs.[ii] <- value

        /// <summary>Raises an Exception if the Array is empty
        /// (Useful for chaining)
        /// Returns the input Array</summary>
        /// <param name="errorMessage">The error message to include in the exception.</param>
        /// <returns>The input Array if not empty.</returns>
        member inline xs.FailIfEmpty (errorMessage: string) : 'T[] =
            if xs.Length = 0 then raise <| Exception("Array.FailIfEmpty: " + errorMessage)
            xs

        /// <summary>Raises an Exception if the Array has less than count items.
        /// (Useful for chaining)
        /// Returns the input Array</summary>
        /// <param name="count">The minimum count required.</param>
        /// <param name="errorMessage">The error message to include in the exception.</param>
        /// <returns>The input Array if it has enough items.</returns>
        member inline xs.FailIfLessThan(count, errorMessage: string) : 'T[] =
            if xs.Length < count then raise <| Exception($"Array.FailIfLessThan {count}: {errorMessage}")
            xs


        /// <summary>Allows for negative indices too. ( -1 is last item, like Python)
        /// The resulting array includes the end index.
        /// The built in slicing notation (e.g. a.[1..3]) for arrays does not allow for negative indices. (and can't be overwritten)
        /// Alternative: from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item.</summary>
        /// <param name="startIdx">The start index (inclusive, can be negative).</param>
        /// <param name="endIdx">The end index (inclusive, can be negative).</param>
        /// <returns>A new array containing the sliced elements.</returns>
        member this.Slice(startIdx:int , endIdx: int ) : 'T array=

            // overrides of existing methods are unfortunately silently ignored and not possible. see https://github.com/dotnet/fsharp/issues/3692#issuecomment-334297164
            // member inline this.GetSlice(startIdx, endIdx) =

            let count = this.Length
            let st  = if startIdx< 0 then count + startIdx        else startIdx
            let len = if endIdx  < 0 then count + endIdx - st + 1 else endIdx - st + 1

            if st < 0 || st > count - 1 then
                let err = $"Array.Slice: Start index {startIdx} is out of range. Allowed values are -{count} up to {count-1} for Array of {count} items"
                raise (IndexOutOfRangeException(err))

            if st+len > count then
                let err = $"Array.Slice: End index {endIdx} is out of range. Allowed values are -{count} up to {count-1} for Array of {count} items"
                raise (IndexOutOfRangeException(err))

            if len < 0 then
                // let en = if endIdx<0 then count+endIdx else endIdx
                // let err = sprintf "Array.Slice: Start index '%A' (= %d) is bigger than end index '%A'(= %d) for Array of %d items" startIdx st endIdx en  count
                let err = $"Array.Slice: Start index {startIdx} is bigger than end index {endIdx} for Array of {count} items"
                raise (IndexOutOfRangeException(err))

            Array.init len (fun i -> this.[st+i])


        /// Creates a new Array with the same items as the input Array.
        /// Shallow copy only.
        /// this.Clone() :?> 'T array
        member inline this.Duplicate(): 'T array =
            #if FABLE_COMPILER
            Array.init this.Length (fun i -> this.[i])
            #else
            this.Clone() :?> 'T array
            #endif



        /// <summary>A string representation of the Array including the count of entries and the first 5 entries.
        /// When used in Fable this member is inlined for reflection to work.</summary>
        /// <returns>A formatted string representation of the array.</returns>
        #if FABLE_COMPILER
        member inline arr.AsString : string =  // inline needed for Fable reflection
        #else
        member arr.asString  :string =  // on .NET inline fails because it's using internal DefaultDictUtil
        #endif
            let t = toStringInline arr
            $"{t}{contentAsString 5 arr}"


        /// <summary>A string representation of the Array including the count of entries
        /// and the specified amount of entries.
        /// When used in Fable this member is inlined for reflection to work.</summary>
        /// <param name="entriesToPrint">The number of entries to display in the string representation.</param>
        /// <returns>A formatted string representation of the array.</returns>
        #if FABLE_COMPILER
        member inline arr.ToString (entriesToPrint)  : string =  // inline needed for Fable reflection
        #else
        member arr.ToString (entriesToPrint)  : string  = // on .NET inline fails because it's using internal DefaultDictUtil
        #endif
            let t = toStringInline arr
            $"{t}{contentAsString entriesToPrint arr}"
