namespace Tests

open ArrayT

module Main =

    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT

    open Fable.Mocha
    Mocha.runTests Tests.Extensions.tests |> ignore
    Mocha.runTests Tests.Module2.tests |> ignore
    Mocha.runTests Tests.FableParity.tests |> ignore

    #else

    open Expecto
    [<EntryPoint>]
    let main argv : int =
        runTestsWithCLIArgs [] [||] Tests.Extensions.tests
        |||
        runTestsWithCLIArgs [] [||] Tests.Module2.tests
        |||
        runTestsWithCLIArgs [] [||] Tests.FableParity.tests


    #endif