# InlineIfLambda and OptimizeClosureIfNotInlined in ArrayT

Would ArrayT's own functions with callbacks get faster if they were written like the
[FSharp.Core 11 Array module](https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/array.fs),
`inline` with `[<InlineIfLambda>]`, and for callbacks with 2 to 5 arguments also with the new F# 11
`[<OptimizeClosureIfNotInlined>]`? The same question was measured for ResizeArrayT in
[ResizeArrayT/Benchmarks/InlineIfLambda](https://github.com/goswinr/ResizeArrayT/tree/main/Benchmarks/InlineIfLambda),
which also explains both attributes.

ArrayT's `Array` module only adds functions to the FSharp.Core `Array` module. `Array.fold`,
`Array.map` and the like come from FSharp.Core itself, which is inline in FSharp.Core 11 for
consumers that reference it. This benchmark covers ArrayT's own functions.

## What is compared

- `current`: the ArrayT library as it was when measured, see the table below.
- `adapt`: not inline, the callback adapted once with `OptimizedClosures.FSharpFunc.Adapt`, as
  ResizeArrayT does. Works with FSharp.Core 6. Only for callbacks with two or more arguments.
- `inlineIfLambda`: `inline` with `[<InlineIfLambda>]`. Works with FSharp.Core 6.
- `optimizeClosure`: as `inlineIfLambda`, plus `[<OptimizeClosureIfNotInlined>]`. Needs F# 11
  and FSharp.Core 11. Only for callbacks with two or more arguments.

The prototypes are in [Prototypes.fs](Prototypes.fs), each with the same algorithm as the library.

| Function | Current implementation | Callback |
| --- | --- | --- |
| `filteri` | not inline | `int -> 'T -> bool` |
| `findIndexi` | not inline, calls the inline `tryFindIndexi` | `int -> 'T -> bool` |
| `mapPrevNext` | not inline | `'T -> 'T -> 'U` and `'T -> 'U -> 'U -> 'V` |
| `pickBack` | not inline | `'T -> 'U option` |
| `partition3` | not inline | two `'T -> bool` |
| `minIndexBy` | `inline`, no attribute, calls the projection at three places | `'T -> 'Key` |

Unlike ResizeArrayT, ArrayT does not adapt its two-argument callbacks today. A call like
`predicate i x` of a function parameter goes through `FSharpFunc.InvokeFast` for every element.

Each is called from three kinds of call sites:

- `lambda`: a small lambda that captures nothing, e.g. `filteri (fun i x -> (i + x) % 3 = 0) xs`.
- `closure`: a lambda that captures a local value, e.g. `filteri (fun i x -> (i + x) % k = 0) xs`.
- `opaque`: a function value received as a parameter by a function that is not inlined
  (`[<NoCompilerInlining>]`), as in code that passes a callback on. The closure is created once.

## Run

The benchmark must be compiled with the .NET 11 SDK (F# 11), otherwise `OptimizeClosureIfNotInlined`
is silently ignored; the project fails the build with an older SDK. It references FSharp.Core
11.0.101 and runs on .NET 8 and .NET 10. The library itself is unchanged, with FSharp.Core 6.0.7.
From the repository root:

```powershell
dotnet build Benchmarks/InlineIfLambda/InlineIfLambda.fsproj -c Release
dotnet Benchmarks/InlineIfLambda/bin/Release/net8.0/InlineIfLambda.dll
dotnet Benchmarks/InlineIfLambda/bin/Release/net10.0/InlineIfLambda.dll
$env:DOTNET_TieredPGO=0; dotnet Benchmarks/InlineIfLambda/bin/Release/net10.0/InlineIfLambda.dll; Remove-Item Env:DOTNET_TieredPGO
dotnet tool restore
dotnet fable Benchmarks/InlineIfLambda/InlineIfLambda.fsproj --outDir Benchmarks/InlineIfLambda/_js --noCache
node Benchmarks/InlineIfLambda/_js/Program.js
```

A full run takes about 25 seconds. Function names as arguments select a subset, for example
`dotnet Benchmarks/InlineIfLambda/bin/Release/net10.0/InlineIfLambda.dll filteri pickBack`.
Run one benchmark process at a time. No npm installation is needed.
The CSV files here combine three runs each:

```powershell
node Benchmarks/InlineIfLambda/combine.mjs run1.csv run2.csv run3.csv > Benchmarks/InlineIfLambda/results-net10.csv
```

## Method

The same harness as in ResizeArrayT:

- Integer arrays of 10 and 10,000 elements with deterministic values from 0 to 999. The first
  element is 1000 and the last is 1001, so `pickBack` and `findIndexi` scan the whole array.
- First all methods of all cases are called in turn for 1 second, so that a shared, not inlined
  library function is optimized by the JIT for many different callbacks, as in an application.
- Then per case and method: 30 ms warmup, batches calibrated to at least 10 ms, 11 measured
  batches with the method order rotated. The result is the median time per call in nanoseconds.
- Bytes allocated per call are measured on .NET with `GC.GetAllocatedBytesForCurrentThread`.
- Outside the timed section every method must return the same result as the current library.
- On .NET the program inspects the IL of the opaque helpers and prints whether they call
  `OptimizedClosures.FSharpFunc.Adapt`: `filteri=false/true` means the `inlineIfLambda` helper does
  not and the `optimizeClosure` helper does. It prints a warning if the attribute had no effect.
- Each CSV row is the median of three separate processes. `ns_min` and `ns_max` are the fastest
  and slowest of the three. `relative_to_current` is the median time divided by that of `current`;
  less than 1 is faster.

For `lambda` and `closure` call sites, `inlineIfLambda` and `optimizeClosure` compile to the same
code, so their differences show the noise. Differences below about 1.3× are within noise on this machine.

## Results (2026-11-21)

Windows 11 x64, Intel Core i5-1135G7 (laptop), .NET SDK 11.0.100 (F# 11), FSharp.Core 11.0.101,
.NET 8.0.34 and 10.0.15, Fable 5.27.0, Node.js 26.12.1. "No PGO" is .NET 10 with
`DOTNET_TieredPGO=0`, similar to runtimes without dynamic PGO, such as .NET Framework.

Time relative to `current` for 10,000 elements, less than 1 is faster.
For two-argument callbacks the cells are `adapt / inlineIfLambda / optimizeClosure`.

| Function | Call site | .NET 8 | .NET 10 | .NET 10, no PGO | Fable |
| --- | --- | ---: | ---: | ---: | ---: |
| filteri | lambda | 0.77 / 0.42 / 0.42 | 0.68 / 0.40 / 0.26 | 0.56 / 0.38 / 0.52 | 1.08 / 1.01 / 1.01 |
| filteri | closure | 0.77 / 0.51 / 0.65 | 0.75 / 0.28 / 0.30 | 0.57 / 0.38 / 0.39 | 1.00 / 1.00 / 1.01 |
| filteri | opaque | 0.44 / 0.96 / 0.43 | 0.41 / 1.01 / 0.41 | 0.60 / 0.97 / 0.59 | 1.00 / 0.55 / 0.55 |
| findIndexi | lambda | 0.55 / 0.21 / 0.22 | 0.50 / 0.25 / 0.23 | 0.48 / 0.18 / 0.20 | 0.99 / 1.00 / 1.00 |
| findIndexi | closure | 0.55 / 0.17 / 0.17 | 0.50 / 0.18 / 0.18 | 0.48 / 0.16 / 0.16 | 1.00 / 1.00 / 1.00 |
| findIndexi | opaque | 0.55 / 1.09 / 0.55 | 0.50 / 1.00 / 0.50 | 0.48 / 1.00 / 0.48 | 1.00 / 0.11 / 0.11 |
| mapPrevNext | lambda | 0.49 / 0.16 / 0.16 | 0.53 / 0.15 / 0.16 | 0.39 / 0.14 / 0.14 | 1.00 / 0.50 / 0.53 |
| mapPrevNext | closure | 0.50 / 0.17 / 0.17 | 0.37 / 0.19 / 0.19 | 0.39 / 0.17 / 0.17 | 0.99 / 0.49 / 0.50 |
| mapPrevNext | opaque | 0.21 / 0.98 / 0.19 | 0.19 / 1.00 / 0.19 | 0.38 / 0.99 / 0.36 | 1.00 / **3.02** / **3.03** |

| Function | Call site | .NET 8 | .NET 10 | .NET 10, no PGO | Fable |
| --- | --- | ---: | ---: | ---: | ---: |
| pickBack | lambda | 0.34 | 0.38 | 0.39 | 1.01 |
| pickBack | closure | 0.29 | 0.54 | 0.40 | 0.98 |
| pickBack | opaque | **2.50** | **1.71** | 1.00 | 0.21 |
| partition3 | lambda | 0.78 | 0.70 | 0.37 | 0.97 |
| partition3 | closure | 0.66 | 0.29 | 0.37 | 1.44 |
| partition3 | opaque | 0.98 | 0.43 | 1.02 | 0.33 |
| minIndexBy | lambda | 1.01 | 1.01 | 0.94 | **1.55** |
| minIndexBy | closure | 0.98 | 1.01 | 0.92 | **1.54** |
| minIndexBy | opaque | 0.92 | 0.99 | 1.02 | 1.00 |

The 10-element results point the same way on .NET, with smaller factors: for example `adapt`
0.29 to 0.78, `inlineIfLambda` with lambdas 0.20 to 0.61. With a capturing lambda, the current
not inlined functions allocate the closure on every call: 24 bytes per callback (48 for
`mapPrevNext` and `partition3` with two callbacks). The inlined versions allocate nothing for
the callback. All other bytes are the result and are the same for all variants.
Full results: [.NET 8](results-net8.csv), [.NET 10](results-net10.csv),
[.NET 10 without PGO](results-net10-nopgo.csv), [Fable](results-fable.csv).

## Conclusions

1. **Two-argument callbacks gain on .NET from every variant.** Because ArrayT does not adapt
   them today, even `adapt` alone makes every call site 1.3× to 5× faster.
   `inlineIfLambda` alone makes lambdas 1.6× to 7× faster and keeps opaque callbacks as fast as
   today (0.96 to 1.09). This is unlike ResizeArrayT, where opaque callbacks were already adapted
   and `[<InlineIfLambda>]` alone made them slower. `optimizeClosure` combines both: lambdas as
   with `inlineIfLambda`, opaque callbacks as with `adapt`.
2. **One-argument callbacks gain for lambdas on .NET.** Inlined, `pickBack` is 1.6× to 3.4×
   faster with lambdas and `partition3` 1.15× to 3.4×. For opaque callbacks they are as fast as
   today without dynamic PGO. With PGO, opaque `pickBack` measured 1.35× to 2.5× slower in the full
   run, but faster when `pickBack` ran alone. This depends on how the JIT profiled the shared
   library function, not on the inlined code itself.
3. **Functions that are already inline gain nothing.** For `minIndexBy` the F# optimizer already
   inlines the lambda without the attribute, even though it is called at three places (0 bytes
   allocated). With the attribute, Fable becomes 1.2× to 1.55× slower, probably because the lambda
   body is copied three times. The same applies to `countIf` and `partitionBy`.
4. **Fable is mixed and depends on V8.** `adapt` changes nothing there. Inlining leaves lambdas
   mostly unchanged (except `partition3` with a closure, 1.44× slower at 10,000 elements) and
   makes `mapPrevNext` 1.2× to 2.1× faster. For opaque callbacks it is often
   much faster (`findIndexi` 3.4× to 9×, `pickBack` 2.6× to 4.8×), but `mapPrevNext` became 2.3×
   to 3× slower. Inlining also changes the JS type of generic results: inlined, `mapPrevNext` on
   `int[]` returns an `Int32Array`, as F# `int[]` normally is in Fable, while the generic library
   function returns a plain JS `Array`.

## Recommendation

Not implemented yet; the library is unchanged by this benchmark.

1. **Without a dependency change:** adapt the two-argument callbacks once with
   `OptimizedClosures.FSharpFunc.Adapt` in `filteri`, `findIndexi`, `mapPrevNext` and `zipDefault`,
   like ResizeArrayT does. That is 1.3× to 5× faster on .NET for every caller and unchanged in Fable.
   Make the not inlined functions with one-argument callbacks (`pickBack`, `tryPickBack`,
   `partition3` to `partition5`, `partition3By` to `partition5By`) `inline` with `[<InlineIfLambda>]`.
   Of these, only `filteri`, `findIndexi`, `mapPrevNext`, `pickBack` and `partition3` were
   measured; the others follow the same patterns.
2. **With FSharp.Core 11 as the minimum:** make the two-argument functions `inline` with
   `[<InlineIfLambda; OptimizeClosureIfNotInlined>]` instead, as in FSharp.Core 11 and in
   ResizeArrayT's `fsharp11` variant.
3. Leave functions that are already inline (`minIndexBy` and the other min/max functions,
   `countIf`, `partitionBy`) as they are.

These are local microbenchmarks with integers, not performance guarantees. Node.js results are not
browser measurements.
