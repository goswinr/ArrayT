namespace Tests

open ArrayT
open type Scriptorium.Quill.Runner

module Main =

    // Scriptorium runs the same test suite on .NET and on JS/TS via Fable.
    // It must be a single runTests call: on JS the process exits when the run completes.
    [<EntryPoint>]
    let main _argv : int =
        runTests [
            Tests.Extensions.tests
            Tests.Module2.tests
            Tests.FableParity.tests
        ]
