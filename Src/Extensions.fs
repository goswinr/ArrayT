namespace ArrayT

open System
open System.Collections.Generic
// open NiceString

#nowarn "44" //for opening the hidden but not Obsolete UtilArray module
open UtilArray


[<AutoOpen>]
module AutoOpenExtensions =

    type ``[]``<'T>  with //Generic Array

        /// Gets an item at index, same as this.[index] or this.Idx(index)
        /// Throws a descriptive Exception if the index is out of range.
        /// (Use this.GetNeg(i) member if you want to use negative indices too)
        member inline xs.Get index =
            if index < 0 then ArgumentOutOfRangeException.Raise "arr.Get(%d) failed for Array of %d items, use arr.GetNeg method if you want negative indices too:\r\n%s" index xs.Length xs.ToNiceStringLong
            if index >= xs.Length then ArgumentOutOfRangeException.Raise "arr.Get(%d) failed for Array of %d items:\r\n%s" index xs.Length xs.ToNiceStringLong
            xs.[index]

        /// Gets an item at index, same as this.[index] or this.Get(index)
        /// Throws a descriptive Exception if the index is out of range.
        /// (Use this.GetNeg(i) member if you want to use negative indices too)
        member inline xs.Idx index =
            if index < 0 then ArgumentOutOfRangeException.Raise "arr.Idx(%d) failed for Array of %d items, use arr.GetNeg method if you want negative indices too:\r\n%s" index xs.Length xs.ToNiceStringLong
            if index >= xs.Length then ArgumentOutOfRangeException.Raise "arr.Idx(%d) failed for Array of %d items:\r\n%s" index xs.Length xs.ToNiceStringLong
            xs.[index]


        /// Sets an item at index
        /// (Use this.SetNeg(i) member if you want to use negative indices too)
        member inline xs.Set index value =
            if index < 0 then ArgumentOutOfRangeException.Raise "The curried function arr.Set %d value, failed for negative number on Array of %d items, use arr.SetNeg method if you want top use negative indices too, for setting %s " index xs.Length (toNiceString value)
            if index >= xs.Length then ArgumentOutOfRangeException.Raise "tThe curried function arr.Set %d value, failed for Array of %d items. for setting %s " index xs.Length (toNiceString value)
            xs.[index] <- value


        /// Gets the index of the last item in the Array.
        /// Equal to this.Length - 1
        /// Returns -1 for empty Array.
        member inline xs.LastIndex =
            // don't fail so that a loop for i=0 to xs.LastIndex will work for empty Array
            //if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.LastIndex: Failed to get LastIndex of empty %s" xs.ToNiceStringLong // Array<%s>" (typeof<'T>).FullName
            xs.Length - 1

        /// Get (or set) the last item in the Array.
        /// Equal to this.[this.Length - 1]
        member inline xs.Last
            with get () =
                if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.Last: Failed to get last item of empty %s" xs.ToNiceStringLong // Array<%s>" (typeof<'T>).FullName
                xs.[xs.Length - 1]
            and set (v: 'T) =
                if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.Last: Failed to set last item of %s to %s" xs.ToNiceStringLong (toNiceString v)
                xs.[xs.Length - 1] <- v

        /// Get (or set) the second last item in the Array.
        /// Equal to this.[this.Length - 2]
        member inline xs.SecondLast
            with get () =
                if xs.Length < 2 then ArgumentOutOfRangeException.Raise "array.SecondLast: Failed to get second last item of %s" xs.ToNiceStringLong
                xs.[xs.Length - 2]
            and set (v: 'T) =
                if xs.Length < 2 then ArgumentOutOfRangeException.Raise "array.SecondLast: Failed to set second last item of %s to %s" xs.ToNiceStringLong (toNiceString v)
                xs.[xs.Length - 2] <- v


        /// Get (or set) the third last item in the Array.
        /// Equal to this.[this.Length - 3]
        member inline xs.ThirdLast
            with get () =
                if xs.Length < 3 then ArgumentOutOfRangeException.Raise "array.ThirdLast: Failed to get third last item of %s." xs.ToNiceStringLong
                xs.[xs.Length - 3]
            and set (v: 'T) =
                if xs.Length < 3 then ArgumentOutOfRangeException.Raise "array.ThirdLast: Failed to set third last item of %s to %s" xs.ToNiceStringLong (toNiceString v)
                xs.[xs.Length - 3] <- v

        /// Get (or set) the first item in the Array.
        /// Equal to this.[0]
        member inline xs.First
            with get () =
                if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.First: Failed to get first item of empty %s" xs.ToNiceStringLong // Array<%s>" (typeof<'T>).FullName
                xs.[0]
            and set (v: 'T) =
                if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.First: Failed to set first item of empty %s" xs.ToNiceStringLong // Array<%s> to %s" (typeof<'T>).FullName (toNiceString v)
                xs.[0] <- v

        /// Gets the the only item in the Array.
        /// Fails if the Array does not have exactly one element.
        member inline xs.FirstAndOnly =
            if xs.Length = 0 then ArgumentOutOfRangeException.Raise "array.FirstOnly: Failed to get first item of empty %s" xs.ToNiceStringLong // Array<%s>" (typeof<'T>).FullName
            if xs.Length > 1 then ArgumentOutOfRangeException.Raise "array.FirstOnly: Array is expected to have only one item but has %d Array: %s" xs.Length xs.ToNiceStringLong
            xs.[0]


        /// Get (or set) the second item in the Array.
        /// Equal to this.[1]
        member inline xs.Second
            with get () =
                if xs.Length < 2 then ArgumentOutOfRangeException.Raise "array.Second: Failed to get second item of %s" xs.ToNiceStringLong
                xs.[1]
            and set (v: 'T) =
                if xs.Length < 2 then ArgumentOutOfRangeException.Raise "array.Second: Failed to set second item of %s to %s" xs.ToNiceStringLong (toNiceString v)
                xs.[1] <- v

        /// Get (or set) the third item in the Array.
        /// Equal to this.[2]
        member inline xs.Third
            with get () =
                if xs.Length < 3 then ArgumentOutOfRangeException.Raise "array.Third: Failed to get third item of %s" xs.ToNiceStringLong
                xs.[2]
            and set (v: 'T) =
                if xs.Length < 3 then ArgumentOutOfRangeException.Raise "array.Third: Failed to set third item of %s to %s" xs.ToNiceStringLong (toNiceString v)
                xs.[2] <- v

        /// Checks if this.Length = 0
        member inline xs.IsEmpty = xs.Length = 0


        /// Checks if this.Length = 1
        member inline xs.IsSingleton = xs.Length = 1

        /// Checks if this.Length > 0
        /// Same as xs.HasItems
        member inline xs.IsNotEmpty = xs.Length > 0

        /// Checks if this.Length > 0
        /// Same as xs.IsNotEmpty
        member inline xs.HasItems = xs.Length > 0


        /// Gets an item in the Array by index.
        /// Allows for negative index too ( -1 is last item,  like Python)
        /// (From the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
        member inline xs.GetNeg index =
            let len = xs.Length
            let ii = if index < 0 then len + index else index
            if ii < 0 || ii >= len then ArgumentOutOfRangeException.Raise "arr.GetNeg: Failed to get (negative) index %d from Array of %d items: %s" index xs.Length xs.ToNiceStringLong
            xs.[ii]

        /// Sets an item in the Array by index.
        /// Allows for negative index too ( -1 is last item,  like Python)
        /// (from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
        member inline xs.SetNeg index value =
            let len = xs.Length
            let ii = if index < 0 then len + index else index
            if ii < 0 || ii >= len then ArgumentOutOfRangeException.Raise "arr.SetNeg: Failed to set (negative) index %d to %s in %s" index (toNiceString value) xs.ToNiceStringLong
            xs.[ii] <- value

        /// Any index will return a value.
        /// Array is treated as an endless loop in positive and negative direction
        member inline xs.GetLooped index =
            let len = xs.Length
            if len = 0 then ArgumentOutOfRangeException.Raise "arr.GetLooped: Failed to get (looped) index %d from Array of 0 items" index
            let t = index % len
            let ii = if t >= 0 then t else t + len
            xs.[ii]

        /// Any index will set a value.
        /// Array is treated as an endless loop in positive and negative direction
        member inline xs.SetLooped index value =
            let len = xs.Length
            if len = 0 then ArgumentOutOfRangeException.Raise "arr.SetLooped: Failed to set (looped) index %d to %s in Array of 0 items" index (toNiceString value)
            let t = index % len
            let ii = if t >= 0 then t else t + len
            xs.[ii] <- value

        /// Raises an Exception if the Array is empty
        /// (Useful for chaining)
        /// Returns the input Array
        member inline xs.FailIfEmpty (errorMessage: string) =
            if xs.Length = 0 then raise <| Exception("Array.FailIfEmpty: " + errorMessage)
            xs

        /// Raises an Exception if the Array has less then count items.
        /// (Useful for chaining)
        /// Returns the input Array
        member inline xs.FailIfLessThan(count, errorMessage: string) =
            if xs.Length < count then raise <| Exception($"Array.FailIfLessThan {count}: {errorMessage}")
            xs


        /// Allows for negative indices too. ( -1 is last item, like Python)
        /// The resulting array includes the end index.
        /// The built in slicing notation (e.g. a.[1..3]) for arrays does not allow for negative indices. (and can't be overwritten)
        /// Alternative: from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item.
        /// (this is an Extension Member from FsEx.ExtensionsArray)
        member inline this.Slice(startIdx:int , endIdx: int ) : 'T array=

            // overrides of existing methods are unfortunately silently ignored and not possible. see https://github.com/dotnet/fsharp/issues/3692#issuecomment-334297164
            // member inline this.GetSlice(startIdx, endIdx) =

            let count = this.Length
            let st  = if startIdx< 0 then count + startIdx        else startIdx
            let len = if endIdx  < 0 then count + endIdx - st + 1 else endIdx - st + 1

            if st < 0 || st > count - 1 then
                let err = sprintf "Array.Slice: Start index %d is out of range. Allowed values are -%d up to %d for Array of %d items" startIdx count (count-1)  count
                raise (IndexOutOfRangeException(err))

            if st+len > count then
                let err = sprintf "Array.Slice: End index %d is out of range. Allowed values are -%d up to %d for Array of %d items" startIdx count (count-1)  count
                raise (IndexOutOfRangeException(err))

            if len < 0 then
                let en = if endIdx<0 then count+endIdx else endIdx
                let err = sprintf "Array.Slice: Start index '%A' (= %d) is bigger than end index '%A'(= %d) for Array of %d items" startIdx st endIdx en  count
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


        /// A property like the ToString() method,
        /// But with richer formatting
        /// Listing includes the first 6 items
        member xs.ToNiceString = toNiceString xs

        /// A property like the ToString() method,
        /// But with richer formatting
        /// Listing includes the first 50 items
        member xs.ToNiceStringLong = toNiceStringLong xs
