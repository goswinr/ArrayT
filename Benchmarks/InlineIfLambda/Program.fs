module InlineIfLambdaBenchmark

open System
open System.Diagnostics
open System.Runtime.CompilerServices

module A = Prototypes.Adapt
module L = Prototypes.InlineIfLambda
module O = Prototypes.OptimizeClosure

// Short on purpose: about 0.2 s per method, about half a minute per runtime for all functions.
let sharedWarmupMs = 1000.0
let warmupMs = 30.0
let batchMs = 10.0
let samples = 11

let now () =
    #if FABLE_COMPILER
    Fable.Core.JsInterop.emitJsExpr () "performance.now()"
    #else
    float (Stopwatch.GetTimestamp()) * 1000.0 / float Stopwatch.Frequency
    #endif

let mutable consumed = 0

/// Opaque call sites: the callback is a parameter of a function that is not inlined,
/// like library or application code that receives a function and passes it on.
/// NoCompilerInlining stops the F# optimizer from seeing the lambda at the outer call site.
module Opaque =
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let filteriCurrent (f: int -> int -> bool) xs = ArrayT.Array.filteri f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let filteriAdapt (f: int -> int -> bool) xs = A.filteri f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let filteriInline (f: int -> int -> bool) xs = L.filteri f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let filteriOptimized (f: int -> int -> bool) xs = O.filteri f xs

    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let findIndexiCurrent (f: int -> int -> bool) xs = ArrayT.Array.findIndexi f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let findIndexiAdapt (f: int -> int -> bool) xs = A.findIndexi f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let findIndexiInline (f: int -> int -> bool) xs = L.findIndexi f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let findIndexiOptimized (f: int -> int -> bool) xs = O.findIndexi f xs

    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let mapPrevNextCurrent (c: int -> int -> int) (m: int -> int -> int -> int) xs = ArrayT.Array.mapPrevNext c m xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let mapPrevNextAdapt (c: int -> int -> int) (m: int -> int -> int -> int) xs = A.mapPrevNext c m xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let mapPrevNextInline (c: int -> int -> int) (m: int -> int -> int -> int) xs = L.mapPrevNext c m xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let mapPrevNextOptimized (c: int -> int -> int) (m: int -> int -> int -> int) xs = O.mapPrevNext c m xs

    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let pickBackCurrent (f: int -> int option) xs = ArrayT.Array.pickBack f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let pickBackInline (f: int -> int option) xs = L.pickBack f xs

    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let partition3Current (p1: int -> bool) (p2: int -> bool) xs = ArrayT.Array.partition3 p1 p2 xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let partition3Inline (p1: int -> bool) (p2: int -> bool) xs = L.partition3 p1 p2 xs

    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let minIndexByCurrent (f: int -> int) xs = ArrayT.Array.minIndexBy f xs
    [<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
    let minIndexByInline (f: int -> int) xs = L.minIndexBy f xs

type Case = { Fn: string; CallSite: string; Count: int; Methods: (string * (unit -> int))[] }

/// Checks outside the timed section that every method returns the same as the current library,
/// then wraps each method to return an int for consumption.
let mkCase fn callSite count (consume: 'R -> int) (methods: (string * (unit -> 'R)) list) =
    let expected = (snd methods.Head) ()
    for name, f in methods do
        if f () <> expected then failwithf "%s/%s/%d/%s: wrong result" fn callSite count name
    { Fn = fn; CallSite = callSite; Count = count; Methods = [| for name, f in methods -> name, (fun () -> consume (f ())) |] }

let lengthOf (r: int[]) = r.Length
let lengthsOf (a: int[], b: int[], c: int[]) = a.Length + b.Length + c.Length

/// Not inline: k, first, last and mid are runtime values here, so the 'closure' lambdas really capture them.
/// xs.[0] = first and xs.[n-1] = last, so findIndexi and pickBack scan the whole array.
let buildCases (k: int) (first: int) (last: int) (mid: int) (xs: int[]) : Case list =
    let n = xs.Length
    // The opaque callbacks are created once, outside the timed call, and capture values like the 'closure' ones.
    let filteriF = fun i x -> (i + x) % k = 0
    let findIndexiF = fun (_: int) x -> x = last
    let combineF = fun a b -> a + b + k
    let mergeF = fun x p q -> x + p - q + k
    let pickBackF = fun x -> if x = first then Some x else None
    let p1F = fun x -> x < k * 100
    let p2F = fun x -> x < k * 200
    let minIndexByF = fun x -> abs (x - mid)
    [
        mkCase "filteri" "lambda" n lengthOf [
            "current",         fun () -> ArrayT.Array.filteri (fun i x -> (i + x) % 3 = 0) xs
            "adapt",           fun () -> A.filteri (fun i x -> (i + x) % 3 = 0) xs
            "inlineIfLambda",  fun () -> L.filteri (fun i x -> (i + x) % 3 = 0) xs
            "optimizeClosure", fun () -> O.filteri (fun i x -> (i + x) % 3 = 0) xs ]
        mkCase "filteri" "closure" n lengthOf [
            "current",         fun () -> ArrayT.Array.filteri (fun i x -> (i + x) % k = 0) xs
            "adapt",           fun () -> A.filteri (fun i x -> (i + x) % k = 0) xs
            "inlineIfLambda",  fun () -> L.filteri (fun i x -> (i + x) % k = 0) xs
            "optimizeClosure", fun () -> O.filteri (fun i x -> (i + x) % k = 0) xs ]
        mkCase "filteri" "opaque" n lengthOf [
            "current",         fun () -> Opaque.filteriCurrent filteriF xs
            "adapt",           fun () -> Opaque.filteriAdapt filteriF xs
            "inlineIfLambda",  fun () -> Opaque.filteriInline filteriF xs
            "optimizeClosure", fun () -> Opaque.filteriOptimized filteriF xs ]

        mkCase "findIndexi" "lambda" n id [
            "current",         fun () -> ArrayT.Array.findIndexi (fun _ x -> x = 1001) xs
            "adapt",           fun () -> A.findIndexi (fun _ x -> x = 1001) xs
            "inlineIfLambda",  fun () -> L.findIndexi (fun _ x -> x = 1001) xs
            "optimizeClosure", fun () -> O.findIndexi (fun _ x -> x = 1001) xs ]
        mkCase "findIndexi" "closure" n id [
            "current",         fun () -> ArrayT.Array.findIndexi (fun _ x -> x = last) xs
            "adapt",           fun () -> A.findIndexi (fun _ x -> x = last) xs
            "inlineIfLambda",  fun () -> L.findIndexi (fun _ x -> x = last) xs
            "optimizeClosure", fun () -> O.findIndexi (fun _ x -> x = last) xs ]
        mkCase "findIndexi" "opaque" n id [
            "current",         fun () -> Opaque.findIndexiCurrent findIndexiF xs
            "adapt",           fun () -> Opaque.findIndexiAdapt findIndexiF xs
            "inlineIfLambda",  fun () -> Opaque.findIndexiInline findIndexiF xs
            "optimizeClosure", fun () -> Opaque.findIndexiOptimized findIndexiF xs ]

        mkCase "mapPrevNext" "lambda" n lengthOf [
            "current",         fun () -> ArrayT.Array.mapPrevNext (fun a b -> a + b) (fun x p q -> x + p - q) xs
            "adapt",           fun () -> A.mapPrevNext (fun a b -> a + b) (fun x p q -> x + p - q) xs
            "inlineIfLambda",  fun () -> L.mapPrevNext (fun a b -> a + b) (fun x p q -> x + p - q) xs
            "optimizeClosure", fun () -> O.mapPrevNext (fun a b -> a + b) (fun x p q -> x + p - q) xs ]
        mkCase "mapPrevNext" "closure" n lengthOf [
            "current",         fun () -> ArrayT.Array.mapPrevNext (fun a b -> a + b + k) (fun x p q -> x + p - q + k) xs
            "adapt",           fun () -> A.mapPrevNext (fun a b -> a + b + k) (fun x p q -> x + p - q + k) xs
            "inlineIfLambda",  fun () -> L.mapPrevNext (fun a b -> a + b + k) (fun x p q -> x + p - q + k) xs
            "optimizeClosure", fun () -> O.mapPrevNext (fun a b -> a + b + k) (fun x p q -> x + p - q + k) xs ]
        mkCase "mapPrevNext" "opaque" n lengthOf [
            "current",         fun () -> Opaque.mapPrevNextCurrent combineF mergeF xs
            "adapt",           fun () -> Opaque.mapPrevNextAdapt combineF mergeF xs
            "inlineIfLambda",  fun () -> Opaque.mapPrevNextInline combineF mergeF xs
            "optimizeClosure", fun () -> Opaque.mapPrevNextOptimized combineF mergeF xs ]

        mkCase "pickBack" "lambda" n id [
            "current",         fun () -> ArrayT.Array.pickBack (fun x -> if x = 1000 then Some x else None) xs
            "inlineIfLambda",  fun () -> L.pickBack (fun x -> if x = 1000 then Some x else None) xs ]
        mkCase "pickBack" "closure" n id [
            "current",         fun () -> ArrayT.Array.pickBack (fun x -> if x = first then Some x else None) xs
            "inlineIfLambda",  fun () -> L.pickBack (fun x -> if x = first then Some x else None) xs ]
        mkCase "pickBack" "opaque" n id [
            "current",         fun () -> Opaque.pickBackCurrent pickBackF xs
            "inlineIfLambda",  fun () -> Opaque.pickBackInline pickBackF xs ]

        mkCase "partition3" "lambda" n lengthsOf [
            "current",         fun () -> ArrayT.Array.partition3 (fun x -> x < 300) (fun x -> x < 600) xs
            "inlineIfLambda",  fun () -> L.partition3 (fun x -> x < 300) (fun x -> x < 600) xs ]
        mkCase "partition3" "closure" n lengthsOf [
            "current",         fun () -> ArrayT.Array.partition3 (fun x -> x < k * 100) (fun x -> x < k * 200) xs
            "inlineIfLambda",  fun () -> L.partition3 (fun x -> x < k * 100) (fun x -> x < k * 200) xs ]
        mkCase "partition3" "opaque" n lengthsOf [
            "current",         fun () -> Opaque.partition3Current p1F p2F xs
            "inlineIfLambda",  fun () -> Opaque.partition3Inline p1F p2F xs ]

        mkCase "minIndexBy" "lambda" n id [
            "current",         fun () -> ArrayT.Array.minIndexBy (fun x -> abs (x - 500)) xs
            "inlineIfLambda",  fun () -> L.minIndexBy (fun x -> abs (x - 500)) xs ]
        mkCase "minIndexBy" "closure" n id [
            "current",         fun () -> ArrayT.Array.minIndexBy (fun x -> abs (x - mid)) xs
            "inlineIfLambda",  fun () -> L.minIndexBy (fun x -> abs (x - mid)) xs ]
        mkCase "minIndexBy" "opaque" n id [
            "current",         fun () -> Opaque.minIndexByCurrent minIndexByF xs
            "inlineIfLambda",  fun () -> Opaque.minIndexByInline minIndexByF xs ]
    ]

let batch count (action: unit -> int) =
    let started = now ()
    for _ = 1 to count do consumed <- consumed + action ()
    now () - started

let bytesPerCall (action: unit -> int) : string =
    #if FABLE_COMPILER
    ignore action
    "n/a"
    #else
    let calls = 32
    let before = GC.GetAllocatedBytesForCurrentThread()
    for _ = 1 to calls do consumed <- consumed + action ()
    string ((GC.GetAllocatedBytesForCurrentThread() - before) / int64 calls)
    #endif

/// Calls all methods of all cases in turn. Without this, the JIT (with dynamic PGO on .NET, or V8 in Node.js)
/// would optimize a shared non-inline library function like Array.filteri for the callback of the
/// first case that runs. In an application such a function is called with many different callbacks.
/// The inlined variants have a separate copy of the loop at every call site.
let warmupShared (cases: Case list) =
    let all = [| for case in cases do for _, action in case.Methods -> action |]
    let started = now ()
    while now () - started < sharedWarmupMs do
        for action in all do consumed <- consumed + action ()

/// Warmup, independent batch calibration, then rotated samples. Prints median nanoseconds per call.
let run (case: Case) =
    let methods = case.Methods
    for _, action in methods do
        let started = now ()
        while now () - started < warmupMs do batch 8 action |> ignore
    let counts = methods |> Array.map (fun (_, action) ->
        let mutable count = 1
        while batch count action < batchMs do count <- count * 2
        count)
    let times = Array.init methods.Length (fun _ -> Array.zeroCreate samples)
    for round = 0 to samples - 1 do
        for step = 0 to methods.Length - 1 do
            let m = (round + step) % methods.Length
            times.[m].[round] <- batch counts.[m] (snd methods.[m]) * 1e6 / float counts.[m]
    let medians = times |> Array.map (fun xs -> Array.sortInPlace xs; xs.[samples / 2])
    for m = 0 to methods.Length - 1 do
        printfn "%s,%s,%d,%s,%.1f,%.3f,%s" case.Fn case.CallSite case.Count (fst methods.[m]) medians.[m] (medians.[m] / medians.[0]) (bytesPerCall (snd methods.[m]))

#if !FABLE_COMPILER
/// True if the compiled method calls OptimizedClosures.FSharpFunc.Adapt,
/// i.e. if the F# compiler adapted the opaque callback once before the loop.
let callsAdapt (methodName: string) =
    let m = Reflection.Assembly.GetExecutingAssembly().GetType("InlineIfLambdaBenchmark+Opaque").GetMethod methodName
    let il = m.GetMethodBody().GetILAsByteArray()
    let isAdapt i =
        try
            let callee = m.Module.ResolveMethod(BitConverter.ToInt32(il, i + 1))
            callee.Name = "Adapt" && callee.DeclaringType.FullName.StartsWith "Microsoft.FSharp.Core.OptimizedClosures"
        with _ -> false
    Seq.exists (fun i -> il.[i] = 0x28uy && isAdapt i) (seq { 0 .. il.Length - 5 }) // 0x28 = call
#endif

[<EntryPoint>]
let main argv =
    // Optional arguments select functions by name, e.g. "filteri pickBack".
    let selected (case: Case) = argv.Length = 0 || Array.contains case.Fn argv
    #if FABLE_COMPILER
    printfn "# Fable on Node.js %s" (Fable.Core.JsInterop.emitJsExpr () "process.version")
    #else
    printfn "# %s, FSharp.Core %O" Runtime.InteropServices.RuntimeInformation.FrameworkDescription (typeof<int option>.Assembly.GetName().Version)
    let adapts = [ for fn in [ "filteri"; "findIndexi"; "mapPrevNext" ] -> sprintf "%s=%b/%b" fn (callsAdapt (fn + "Inline")) (callsAdapt (fn + "Optimized")) ]
    printfn "# Opaque callback adapted once (inlineIfLambda/optimizeClosure): %s" (String.Join(" ", adapts))
    if not (callsAdapt "filteriOptimized") then
        printfn "# WARNING: OptimizeClosureIfNotInlined had no effect. Build with the .NET 11 SDK (F# 11)."
    try Process.GetCurrentProcess().PriorityClass <- ProcessPriorityClass.High with _ -> ()
    #endif
    let mutable state = 42
    let next () =
        state <- (state * 25173 + 13849) &&& 65535
        state % 1000
    printfn "function,call_site,count,method,ns,relative_to_current,bytes"
    let started = now ()
    let cases = [
        for size in [ 10; 10000 ] do
            let xs = Array.init size (fun _ -> next ())
            xs.[0] <- 1000 // found last by pickBack
            xs.[size - 1] <- 1001 // found last by findIndexi
            yield! buildCases 3 1000 1001 500 xs |> List.filter selected ]
    warmupShared cases
    for case in cases do run case
    printfn "# All correctness checks passed. Total %.0f s. Consumed: %d" ((now () - started) / 1000.0) consumed
    0
