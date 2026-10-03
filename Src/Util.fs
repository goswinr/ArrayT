namespace ArrayT

open System

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open Fable.Core.JsInterop
#endif


/// Internal utilities for array operations and exception handling.
[<Obsolete("Not Obsolete but hidden, needs to be visible for inlining")>]
module UtilArray =

    /// <summary>Gets the value at index i, skipping bounds check in compiled JS code.</summary>
    /// <param name="i">The index to access.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>The value at the specified index.</returns>
    let inline getUnCkd (i:int) (arr:'T[]) : 'T =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            emitJsExpr (arr,i) "$0[$1]"
        #else
            arr.[i]
        #endif


    /// <summary>Sets the value at index i, skipping bounds check in compiled JS code.</summary>
    /// <param name="i">The index to set.</param>
    /// <param name="v">The value to set.</param>
    /// <param name="arr">The input array.</param>
    let inline setUnCkd (i:int) (v:'T) (arr:'T[]) : unit =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            emitJsStatement (arr,i,v) "$0[$1] = $2"
        #else
            arr.[i] <- v
        #endif


    /// <summary>Any int will give a valid index for given collection size.
    /// Converts negative indices to positive ones and loops to start after last index is reached.
    /// Returns a valid index for a collection of 'length' items for any integer.
    /// Requires length > 0.</summary>
    /// <param name="i">The index to convert (can be any integer).</param>
    /// <param name="length">The length of the array (must be > 0).</param>
    /// <returns>A valid looped index.</returns>
    let inline negIdxLooped i length : int =
        let t = i % length
        if t >= 0 then t else t + length


    /// <summary>Converts an array to a string representation showing its type and item count.</summary>
    /// <param name="ofType">The type name as a string.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>A string describing the array.</returns>
    let inline toStringCore ofType (arr:'T[]) : string = // inline needed for Fable reflection
        if isNull arr then
            "null array"
        else
            if arr.Length = 0 then
                $"empty array<{ofType}>"
            elif arr.Length = 1 then
                $"array<{ofType}> with 1 item"
            else
                $"array<{ofType}> with {arr.Length} items"

    /// <summary>Converts an array to a string representation using runtime type information.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>A string describing the array.</returns>
    let inline toStringInline (arr:'T[]) : string = // inline needed for Fable reflection
        let t = (typeof<'T>).Name //  Fable reflection works only inline
        toStringCore t arr

    // -------------------------------------------------------------
    // IEEE 754:2019 minimum and maximum of float and float32
    // -------------------------------------------------------------
    // IEEE 754:2019 defines two different operations for the minimum and the maximum:
    // 'minimum' and 'maximum' propagate NaN, 'minimumNumber' and 'maximumNumber' skip NaN.
    // All four treat -0.0 as smaller than +0.0.
    // See https://github.com/dotnet/fsharp/issues/13207#issuecomment-1194411950
    // Array.min_IEEE754, max_IEEE754, minNumber and maxNumber use these functions when 'T is float or float32.
    // They are not inline, so that the call sites of those inline functions stay small.

    /// <summary>Casts x to 'U. Only use it when 'T and 'U are known to be the same type at runtime.</summary>
    /// <param name="x">The value to cast.</param>
    /// <returns>The same value, typed as 'U.</returns>
    let inline retype (x: 'T) : 'U = unbox<'U> (box x)

    /// <summary>True for -0.0. For float and float32.</summary>
    /// <param name="x">The float to test.</param>
    /// <returns>True if x is -0.0.</returns>
    let inline isNegZero (x: ^F) : bool =
        x = LanguagePrimitives.GenericZero && LanguagePrimitives.GenericOne / x < LanguagePrimitives.GenericZero

    /// <summary>The IEEE 754:2019 'minimum' of two floats: NaN propagates and -0.0 is smaller than +0.0.</summary>
    /// <param name="a">The first float.</param>
    /// <param name="b">The second float.</param>
    /// <returns>The smaller float, or NaN.</returns>
    let inline minimumOf (a: ^F) (b: ^F) : ^F =
        if a < b then a
        elif b < a then b
        elif a = b then (if isNegZero b then b else a) // -0.0 = +0.0 is true
        elif a <> a then a // a is NaN
        else b // b is NaN

    /// <summary>The IEEE 754:2019 'maximum' of two floats: NaN propagates and +0.0 is bigger than -0.0.</summary>
    /// <param name="a">The first float.</param>
    /// <param name="b">The second float.</param>
    /// <returns>The bigger float, or NaN.</returns>
    let inline maximumOf (a: ^F) (b: ^F) : ^F =
        if a > b then a
        elif b > a then b
        elif a = b then (if isNegZero a then b else a)
        elif a <> a then a
        else b

    /// <summary>The IEEE 754:2019 'minimumNumber' of two floats: NaN is skipped and -0.0 is smaller than +0.0.
    /// Returns NaN only if both are NaN.</summary>
    /// <param name="a">The first float.</param>
    /// <param name="b">The second float.</param>
    /// <returns>The smaller float that is not NaN.</returns>
    let inline minimumNumberOf (a: ^F) (b: ^F) : ^F =
        if a < b then a
        elif b < a then b
        elif a = b then (if isNegZero b then b else a)
        elif b <> b then a // b is NaN
        else b // a is NaN

    /// <summary>The IEEE 754:2019 'maximumNumber' of two floats: NaN is skipped and +0.0 is bigger than -0.0.
    /// Returns NaN only if both are NaN.</summary>
    /// <param name="a">The first float.</param>
    /// <param name="b">The second float.</param>
    /// <returns>The bigger float that is not NaN.</returns>
    let inline maximumNumberOf (a: ^F) (b: ^F) : ^F =
        if a > b then a
        elif b > a then b
        elif a = b then (if isNegZero a then b else a)
        elif b <> b then a
        else b

    let inline private reduceFloats ([<InlineIfLambda>] op: ^F -> ^F -> ^F) (arr: ^F[]) : ^F =
        let mutable acc = arr.[0]
        for i = 1 to arr.Length - 1 do
            acc <- op acc arr.[i]
        acc

    /// <summary>The IEEE 754:2019 'minimum' of a non-empty float array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The smallest float, or NaN.</returns>
    let minimumFloat (arr: float[]) : float = reduceFloats minimumOf arr

    /// <summary>The IEEE 754:2019 'minimum' of a non-empty float32 array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The smallest float32, or NaN.</returns>
    let minimumFloat32 (arr: float32[]) : float32 = reduceFloats minimumOf arr

    /// <summary>The IEEE 754:2019 'maximum' of a non-empty float array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The biggest float, or NaN.</returns>
    let maximumFloat (arr: float[]) : float = reduceFloats maximumOf arr

    /// <summary>The IEEE 754:2019 'maximum' of a non-empty float32 array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The biggest float32, or NaN.</returns>
    let maximumFloat32 (arr: float32[]) : float32 = reduceFloats maximumOf arr

    /// <summary>The IEEE 754:2019 'minimumNumber' of a non-empty float array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The smallest float that is not NaN.</returns>
    let minimumNumberFloat (arr: float[]) : float = reduceFloats minimumNumberOf arr

    /// <summary>The IEEE 754:2019 'minimumNumber' of a non-empty float32 array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The smallest float32 that is not NaN.</returns>
    let minimumNumberFloat32 (arr: float32[]) : float32 = reduceFloats minimumNumberOf arr

    /// <summary>The IEEE 754:2019 'maximumNumber' of a non-empty float array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The biggest float that is not NaN.</returns>
    let maximumNumberFloat (arr: float[]) : float = reduceFloats maximumNumberOf arr

    /// <summary>The IEEE 754:2019 'maximumNumber' of a non-empty float32 array.</summary>
    /// <param name="arr">The input array.</param>
    /// <returns>The biggest float32 that is not NaN.</returns>
    let maximumNumberFloat32 (arr: float32[]) : float32 = reduceFloats maximumNumberOf arr

    // -------------------------------------------------------------
    // for Exceptions ( never inlined)
    // -------------------------------------------------------------

    let itemInOneLineWithMaxChars charCount (item:'T) =
        let s = $"{item}".Split('\n') |> Array.map (fun l -> l.Trim()) |> String.concat " "
        if s.Length > charCount then
            s.Substring(0, charCount) + " ..."
        else
            s

    /// <summary>Returns a string with the content of the array up to 'entriesToPrint' entries.
    /// Includes the index of each entry.
    /// Includes the last entry.</summary>
    /// <param name="entriesToPrint">The maximum number of entries to display.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>A formatted string showing array contents.</returns>
    let contentAsString (entriesToPrint) (arr:'T[]) : string = // not inline, it is only used for exception messages and ToString
        let c = arr.Length
        if c > 0 && entriesToPrint > 0 then
            let b = Text.StringBuilder()
            b.AppendLine ":"  |> ignore
            for i,t in arr |> Seq.truncate (max 0 entriesToPrint) |> Seq.indexed do
                b.AppendLine $"  {i}: {itemInOneLineWithMaxChars 200 t}" |> ignore
            // compare with c-1 instead of entriesToPrint+1 to avoid an overflow for Int32.MaxValue
            if c - 1 = entriesToPrint then
                b.AppendLine $"  {c-1}: {itemInOneLineWithMaxChars 200 arr[c-1]}" |> ignore
            elif c - 1 > entriesToPrint then
                b.AppendLine "  ..." |> ignore
                b.AppendLine $"  {c-1}: {itemInOneLineWithMaxChars 200 arr[c-1]}" |> ignore
            b.ToString()
        else
            ""

    /// <summary>Raises an ArrayTArgumentNullException for a null array input.</summary>
    /// <param name="funcName">The name of the function that received null input.</param>
    /// <returns>Never returns (always raises).</returns>
    let nullExn (funcName:string) : 'a =
        raise (ArrayTArgumentNullException(null, "Array." + funcName + ": input is null!"))

    /// <summary>Raises an IndexOutOfRangeException for invalid get operations.</summary>
    /// <param name="i">The invalid index.</param>
    /// <param name="arr">The input array.</param>
    /// <param name="funcName">The name of the function that failed.</param>
    /// <returns>Never returns (always raises).</returns>
    let badGetExn (i:int) (arr:'T[]) (funcName:string) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Can't get index {i} from:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an IndexOutOfRangeException for invalid set operations.</summary>
    /// <param name="i">The invalid index.</param>
    /// <param name="arr">The input array.</param>
    /// <param name="funcName">The name of the function that failed.</param>
    /// <param name="doingSet">The value being set.</param>
    /// <returns>Never returns (always raises).</returns>
    let badSetExn (i:int) (arr:'T[]) (funcName:string) (doingSet:'T) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Can't set index {i} to {doingSet} on:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an IndexOutOfRangeException when the array does not have the expected number of items.</summary>
    /// <param name="arr">The input array.</param>
    /// <param name="funcName">The name of the function that failed.</param>
    /// <param name="expected">A description of the expected item count, e.g. "exactly one item".</param>
    /// <returns>Never returns (always raises).</returns>
    let badCountExn (arr:'T[]) (funcName:string) (expected:string) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Expected {expected} in:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an ArrayTArgumentException with a descriptive message about array operation failure.</summary>
    /// <param name="arr">The input array.</param>
    /// <param name="funcAndReason">A string describing the function and reason for failure.</param>
    /// <returns>Never returns (always raises).</returns>
    let fail (arr:'T[]) (funcAndReason:string) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (ArrayTArgumentException($"Array.{funcAndReason}:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an IndexOutOfRangeException with a descriptive message about array operation failure.</summary>
    /// <param name="arr">The input array.</param>
    /// <param name="funcAndReason">A string describing the function and reason for failure.</param>
    /// <returns>Never returns (always raises).</returns>
    let failIdx (arr:'T[]) (funcAndReason:string) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcAndReason}:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an ArrayTKeyNotFoundException with a descriptive message about array operation failure.</summary>
    /// <param name="arr">The input array.</param>
    /// <param name="funcAndReason">A string describing the function and reason for failure.</param>
    /// <returns>Never returns (always raises).</returns>
    let failKey (arr:'T[]) (funcAndReason:string) : 'a =
        let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (ArrayTKeyNotFoundException($"Array.{funcAndReason}:\n{toStringCore t arr}{contentAsString 5 arr}"))



    /// <summary>A simple Wrapper for an array.
    /// The sole purpose is to provide a better Exception message when an index is out of range.</summary>
    /// <param name="arr">The array to wrap.</param>
    type DebugIndexer<'T>(arr:'T[]) = // [<Struct>] would fails for setter !

        /// <summary>Gets or sets an item at the specified index with descriptive error messages.
        /// Index parameter: the index to access or set.
        /// Set value parameter: the value to set (setter only).</summary>
        member this.Item
            with get i =
                if i < 0 || i >= arr.Length then badGetExn i arr "DebugIdx.[i]"
                arr.[i]

            and set i x =
                if i < 0 || i >= arr.Length then badSetExn i arr "DebugIdx.[i]" x
                arr.[i] <- x

        /// <summary>Gets the length of the wrapped array.</summary>
        member this.Length : int =
            arr.Length

        /// <summary>Gets the wrapped array.</summary>
        member this.Array : 'T[] =
            arr

        override this.ToString() : string =
            let t =
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
                "'T"
            #else
                (typeof<'T>).Name
            #endif
            $"DebugIndexer for {toStringCore t arr}{contentAsString 5 arr}"

