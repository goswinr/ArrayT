namespace Tests
open ArrayT

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open System
open System.Collections.Generic


type Assert =

    static member AreEqual(expected : 'T[], actual : 'T[], message : string) : unit =
        if  expected <> actual then
            failwithf  "%s: AreEqual Expected \r\n%A \r\nbut got \r\n%A" message expected actual

    static member AreNotEqual(expected : 'T[], actual : 'T[], message : string) : unit =
        if expected = actual then
            failwithf "%s: AreNotEqual expected \r\n%A \r\n NOT to equal \r\n%A" message expected actual
            Exception message |> raise

    static member AreEqual(expected : 'T[], actual : 'T[]) : unit =
        if  expected <> actual then
            failwithf "AreEqual expected \r\n%A \r\nbut got \r\n%A" expected actual


    static member AreEqual(expected : 'T option, actual : 'T option) : unit =
        match expected, actual with
        | None, None -> ()
        | None, Some _
        | Some _, None -> failwithf "AreEqual expected \r\n%A \r\nbut got \r\n%A" expected actual
        | Some e, Some a ->
            if not <| e.Equals(a) then failwithf "AreEqual expected \r\n%A \r\nbut got \r\n%A" expected actual

    static member AreEqual(expected : float, actual : float) : unit =
        //use a tolerance the first 12 digits for float comparisons
        let tol = abs expected * 1e-12 + abs actual * 1e-12
        if abs(expected-actual) > tol then
            failwithf "AreEqual expected \r\n%A \r\nbut got \r\n%A" expected actual

    static member AreEqual(expected : string, actual : string) : unit =
        if expected <> actual then
            failwithf "AreEqual expected \r\n%A \r\nbut got \r\n%A" expected actual


    static member Fail(message : string) : unit = Exception(message) |> raise

    static member Fail() : unit = Assert.Fail("")

    static member True(condition : bool) : unit =
        if not condition then
            Exception("Assertion failed: Expected true but got false") |> raise

    static member False(condition) : unit =
        if condition then
            Exception("Assertion failed: Expected false but got true") |> raise


module Exceptions =

    /// Check that the lambda throws an exception of the given type. Otherwise
    /// calls Assert.Fail()
    let CheckThrowsExn<'a when 'a :> exn> (f : unit -> unit) : unit =
        #if FABLE_COMPILER
            Expect.throws f "CheckThrowsExn"
        #else
            try
                let _ = f ()
                sprintf "Expected %O exception, got no exception" typeof<'a> |> Assert.Fail
            with
            | :? 'a -> ()
            | e -> sprintf "Expected %O exception, got: %O" typeof<'a> e |> Assert.Fail
        #endif

    let private CheckThrowsExn2<'a when 'a :> exn> _s (f : unit -> unit) : unit =

        #if FABLE_COMPILER
            Expect.throws f "CheckThrowsExn2"
        #else

            let funcThrowsAsExpected =
                try
                    let _ = f ()
                    false // Did not throw!
                with
                | :? 'a
                    -> true   // Thew null ref, OK
                | _ -> false  // Did now throw a null ref exception!
            if funcThrowsAsExpected
            then ()
            else Assert.Fail(_s)
        #endif


    let throwsRange   f : unit = CheckThrowsExn<IndexOutOfRangeException>    f

    let throwsNull f : unit = CheckThrowsExn<ArgumentNullException>    f
    let throwsArg f : unit = CheckThrowsExn<ArgumentException>    f



[<AutoOpen>]
module ExtensionOnArray =


    let inline (==) (a: 'T[]) b : bool =  a = b
    let inline (<!>) (a: 'T[]) b : bool =  a <>b
    let inline (=++=) (a: 'T[] * 'T[]) (b: 'T[] * 'T[]) : bool = (fst a == fst b) && (snd a == snd b)
    let inline (<!!>) (a: 'T[] * 'T[]) (b: 'T[] * 'T[]) : bool = (fst a <!> fst b) || (snd a <!> snd b)
    let inline (<!!!>) (aaa: 'T[] * 'T[] * 'T[]) (bbb: 'T[] * 'T[] * 'T[]) : bool =
        let a,b,c = aaa
        let x,y,z = bbb
        (a <!> x) || (b <!> y) || (c <!> z)

    let inline (=+=) (aa:'T[][]) (bb:'T[][]) : bool =
        let rec eq i =
            if i < aa.Length then
                let a = aa.[i]
                let b = bb.[i]
                if  a = b then eq (i+1)
                else false
            else
                aa.Length=bb.Length
        eq 0


    let eqi (rarr1: ('K* 'T[])[]) (rarr2: ('K* 'T[])[]) : bool =
        if rarr1.Length <> rarr2.Length then false
        else
            let rec eq i =
                if i < rarr1.Length then
                    let i1,r1 = rarr1.[i]
                    let i2,r2 = rarr2.[i]
                    if r1 = r2 && i1=i2 then eq (i+1)
                    else false
                else
                    true
            eq 0

    let inline (<*>) a b : bool = not <| eqi a b
    //let inline (=*=) a b = eqi a b
