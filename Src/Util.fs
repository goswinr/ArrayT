namespace ArrayT

open System
open System.Collections.Generic

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


    /// <summary>Converts negative indices to positive ones.
    /// Correct results from -length up to length-1.
    /// e.g.: -1 is last item.
    /// (from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)</summary>
    /// <param name="i">The index to convert (can be negative).</param>
    /// <param name="len">The length of the array.</param>
    /// <returns>The converted positive index.</returns>
    let inline negIdx i len : int =
        let ii = if i < 0 then len + i else i
        if ii < 0 || ii >= len then
            raise <| IndexOutOfRangeException $"UtilArray.negIdx: Bad index {i} for items count {len}."
        ii

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
    // for Exceptions ( never inlined)
    // -------------------------------------------------------------

    /// <summary>Returns a string with the content of the array up to 'entriesToPrint' entries.
    /// Includes the index of each entry.
    /// Includes the last entry (prints one extra if only one more remains to avoid "...").</summary>
    /// <param name="entriesToPrint">The maximum number of entries to display.</param>
    /// <param name="arr">The input array.</param>
    /// <returns>A formatted string showing array contents.</returns>
    let contentAsString (entriesToPrint) (arr:'T[]) : string = // on .NET inline fails because it's using internal DefaultDictUtil
        let c = arr.Length
        if c > 0 && entriesToPrint > 0 then
            let b = Text.StringBuilder()
            b.AppendLine ":"  |> ignore
            for i,t in arr |> Seq.truncate (max 0 entriesToPrint) |> Seq.indexed do
                b.AppendLine $"  {i}: {t}" |> ignore
            if c = entriesToPrint+1 then
                b.AppendLine $"  {c-1}: {arr[c-1]}" |> ignore // print one more line if it's the last instead of "..."
            elif c > entriesToPrint + 1  then
                b.AppendLine "  ..." |> ignore
                b.AppendLine $"  {c-1}: {arr[c-1]}" |> ignore
            b.ToString()
        else
            ""

    /// <summary>Raises an ArgumentNullException for a null array input.</summary>
    /// <param name="funcName">The name of the function that received null input.</param>
    /// <returns>Never returns (always raises).</returns>
    let nullExn (funcName:string) : 'a =
        raise (ArgumentNullException("Array." + funcName + ": input is null!"))

    /// <summary>Raises an IndexOutOfRangeException for invalid get operations.</summary>
    /// <param name="i">The invalid index.</param>
    /// <param name="arr">The input array.</param>
    /// <param name="funcName">The name of the function that failed.</param>
    /// <returns>Never returns (always raises).</returns>
    let badGetExn (i:int) (arr:'T[]) (funcName:string) : 'a =
        let t =
            #if FABLE_COMPILER
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
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Can't set index {i} to {doingSet} on:\n{toStringCore t arr}{contentAsString 5 arr}"))

    /// <summary>Raises an ArgumentException with a descriptive message about array operation failure.</summary>
    /// <param name="arr">The input array.</param>
    /// <param name="funcAndReason">A string describing the function and reason for failure.</param>
    /// <returns>Never returns (always raises).</returns>
    let fail (arr:'T[]) (funcAndReason:string) : 'a =
        let t =
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (ArgumentException($"Array.{funcAndReason}:\n{toStringCore t arr}{contentAsString 5 arr}"))



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
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
            $"DebugIndexer for {toStringCore t arr}{contentAsString 5 arr}"

