namespace ArrayT

open System
open System.Collections.Generic


[<Obsolete("Not Obsolete but hidden, needs to be visible for inlining")>]
module UtilArray =


    /// Converts negative indices to positive ones.
    /// Correct results from -length up to length-1
    /// e.g.: -1 is  last item .
    /// (from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
    let inline negIdx i len =
        let ii = if i < 0 then len + i else i
        if ii < 0 || ii >= len then
            raise <| IndexOutOfRangeException $"UtilArray.negIdx: Bad index {i} for items count {len}."
        ii

    /// Any int will give a valid index for given collection size.
    /// Converts negative indices to positive ones and loops to start after last index is reached.
    /// Returns a valid index for a collection of 'length' items for any integer
    let inline negIdxLooped i length =
        let t = i % length
        if t >= 0 then t else t + length


    let inline toStringCore ofType (arr:'T[]) = // inline needed for Fable reflection
        if isNull arr then
            "null array"
        else
            if arr.Length = 0 then
                $"empty array<{ofType}>"
            elif arr.Length = 1 then
                $"array<{ofType}> with 1 item"
            else
                $"array<{ofType}> with {arr.Length} items"

    let inline toStringInline (arr:'T[]) = // inline needed for Fable reflection
        let t = (typeof<'T>).Name //  Fable reflection works only inline
        toStringCore t arr

    // -------------------------------------------------------------
    // for Exceptions ( never inlined)
    // -------------------------------------------------------------

    /// Returns a string with the content of the array up to 'entriesToPrint' entries.
    /// Includes the index of each entry.
    /// Includes the last entry.
    let contentAsString (entriesToPrint) (arr:'T[]) = // on .NET inline fails because it's using internal DefaultDictUtil
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

    let nullExn (funcName:string) =
        raise (ArgumentNullException("Array." + funcName + ": input is null!"))

    let badGetExn (i:int) (arr:'T[]) (funcName:string) =
        let t =
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Can't get index {i} from:\n{toStringCore t arr}{contentAsString 5 arr}"))

    let badSetExn (i:int) (arr:'T[]) (funcName:string) (doingSet:'T) =
        let t =
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (IndexOutOfRangeException($"Array.{funcName}: Can't set index {i} to {doingSet} on:\n{toStringCore t arr}{contentAsString 5 arr}"))


    let fail (arr:'T[]) (funcAndReason:string)  =
        let t =
            #if FABLE_COMPILER
                "'T"
            #else
                (typeof<'T>).Name
            #endif
        raise (ArgumentException($"Array.{funcAndReason}:\n{toStringCore t arr}{contentAsString 5 arr}"))
