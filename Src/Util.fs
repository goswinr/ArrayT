namespace ArrayT

open System
open System.Collections.Generic


[<Obsolete("not Obsolete but hidden, needs to be visible for inlining")>]
module UtilArray =

    /// This module is set to auto open when opening Array namespace.
    /// Static Extension methods on Exceptions to cal Exception.Raise "%A" x with F# printf string formatting
    [<AutoOpen>]
    module AutoOpenExtensionsExceptions =

        // type ArgumentException with

        //     /// Raise ArgumentException with F# printf string formatting
        //     /// this is also the base class of ArgumentOutOfRangeException and ArgumentNullException
        //     static member RaiseBase msg =
        //         Printf.kprintf (fun s -> raise (ArgumentException(s))) msg


        type ArgumentOutOfRangeException with

            /// Raise ArgumentOutOfRangeException with F# printf string formatting
            static member Raise msg =
                Printf.kprintf (fun s -> raise (ArgumentOutOfRangeException(s))) msg

        type KeyNotFoundException with

            /// Raise KeyNotFoundException with F# printf string formatting
            static member Raise msg =
                Printf.kprintf (fun s -> raise (KeyNotFoundException(s))) msg


        type ArgumentNullException with

            /// Check if null and raise System.ArgumentNullException
            static member Raise msg =
                    raise (System.ArgumentNullException("Array." + msg + " input is null!"))


    let toNiceString (x: 'T) = // TODO replace with better implementation
        match box x with
        | null -> "null"
        #if FABLE_COMPILER
        | :? ``[]``<'T>  as xs -> sprintf "An Array with %d items." xs.Length
        #else
        | :? ``[]``<'T>  as xs -> sprintf "An Array<%s> with %d items." (typeof<'T>).FullName xs.Length
        #endif
        | _ -> x.ToString()

    let toNiceStringLong (x: 'T) = // TODO replace with better implementation
        toNiceString x

    /// Converts negative indices to positive ones.
    /// Correct results from -length up to length-1
    /// e.g.: -1 is  last item .
    /// (from the release of F# 5 on a negative index can also be done with '^' prefix. E.g. ^0 for the last item)
    let inline negIdx i len =
        let ii = if i < 0 then len + i else i
        if ii < 0 || ii >= len then
            ArgumentOutOfRangeException.Raise "UtilArray.negIdx: Bad index %d for items count %d." i len
        ii

    /// Any int will give a valid index for given collection size.
    /// Converts negative indices to positive ones and loops to start after last index is reached.
    /// Returns a valid index for a collection of 'length' items for any integer
    let inline negIdxLooped i length =
        let t = i % length
        if t >= 0 then t else t + length

    let inline isNullSeq x =
        match x with
        |null -> true
        | _   -> false

