namespace Tests

open Scriptorium.Nib.Assertion

open System


/// Helpers to check thrown exceptions, used by all test lists.
module Exceptions =

    /// Check that the lambda throws an exception of the given type.
    /// In Fable only checks that an exception is thrown, the exception type can't be checked there.
    let CheckThrowsExn<'a when 'a :> exn> (f : unit -> unit) : unit =
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            assertThat f (tag "CheckThrowsExn" >> throws)
        #else
            let thrown =
                try f (); None
                with e -> Some e
            match thrown with
            | None -> failwithf "Expected %O exception, got no exception" typeof<'a>
            | Some (:? 'a) -> ()
            | Some e -> failwithf "Expected %O exception, got: %O" typeof<'a> e
        #endif


    let throwsRange f : unit = CheckThrowsExn<IndexOutOfRangeException> f
    let throwsNull  f : unit = CheckThrowsExn<ArgumentNullException> f
    let throwsArg   f : unit = CheckThrowsExn<ArgumentException> f

    /// Check that the lambda throws and that the exception message contains all the given parts.
    /// Works the same on .NET and in Fable.
    let throwsWith (parts: string list) (f : unit -> unit) : unit =
        let containsAll (e: exn) = parts |> List.forall (fun (p: string) -> e.Message.Contains p)
        assertThat f (tag "throwsWith" >> throws >> assertion containsAll (fun e -> $"expected the exception message to contain {parts} but got:\n{e.Message}"))
