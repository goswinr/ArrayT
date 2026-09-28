
![Logo](https://raw.githubusercontent.com/goswinr/ArrayT/main/Docs/img/logo128.png)

# ArrayT

[![ArrayT on nuget.org](https://img.shields.io/nuget/v/ArrayT)](https://www.nuget.org/packages/ArrayT/)
[![Build and Test Status](https://github.com/goswinr/ArrayT/actions/workflows/build.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/build.yml)
[![Docs Build Status](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml)

[![license](https://img.shields.io/github/license/goswinr/ArrayT)](LICENSE.md)
![code size](https://img.shields.io/github/languages/code-size/goswinr/ArrayT.svg)

ArrayT is an F# extension and module library for `Array<'T>`

It also works in Javascript and Typescript with [Fable](https://fable.io/).

## Motivation
I was always annoyed that an IndexOutOfRangeException does not include the actual index that was out of bounds nor the actual size of the array.
This library fixes that in `array.Get`, `array.Set`, `array.SliceNeg` and other item access functions.

This library was designed for use with F# scripting.<br>
Functions and methods never return null.<br>
Only functions starting with `try...` will return an F# Option.<br>
Otherwise when a function fails on invalid input it will throw a descriptive exception.

See also https://github.com/goswinr/ResizeArray/ for a similar library for `ResizeArray<'T>`.

### It Includes

- An `Array` module that adds functions to the `Array` module from [`FSharp.Core`](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html).<br>
See [docs](https://goswinr.github.io/ArrayT/reference/arrayt-array.html)

- Extension members on `Array` like <br>
`.Get idx` `.Set idx item` `.First` `.Last` `.SecondLast` `xs.DebugIdx.[i]` and more..<br>
With nicer IndexOutOfRangeExceptions that include the bad index and the actual size.<br>
See [docs](https://goswinr.github.io/ArrayT/reference/arrayt-autoopenarraytextensions.html)


## Full API Documentation

[goswinr.github.io/ArrayT](https://goswinr.github.io/ArrayT/reference/arrayt.html)


## Usage
Just open the namespace

```fsharp
open ArrayT
```

this namespace contains:

- a module also called `Array`. It will add additional functions to the `Array` module from `FSharp.Core`.

- this will also auto open the extension members on `Array<'T>`

## Examples

### Better error messages

The core motivation: descriptive exceptions when accessing out-of-range indices.

```fsharp
#r "nuget: ArrayT"
open ArrayT

let xs = [| 0 .. 88 |]

xs.Get(99)
```

throws

```txt
System.IndexOutOfRangeException: Array.Get: Can't get index 99 from:
array<Int32> with 89 items:
  0: 0
  1: 1
  2: 2
  3: 3
  4: 4
  ...
  88: 88
```

instead of the usual unhelpful `System.IndexOutOfRangeException: Index was outside the bounds of the array.`

The same applies to `Set`:

```fsharp
xs.Set 99 0
// throws: Array.Set: Can't set index 99 to 0 on:
// array<Int32> with 89 items: ...
```

If you want to use bracket notation `xs.[i]` with these descriptive errors, use the `DebugIdx` member:

```fsharp
let item = xs.DebugIdx.[99]   // throws with same descriptive message
xs.DebugIdx.[99] <- 0         // setter also has descriptive errors
```

### Positional access

Convenient properties for accessing items at common positions:

```fsharp
let arr = [| "a"; "b"; "c"; "d"; "e" |]

arr.First       // "a"
arr.Second      // "b"
arr.Third       // "c"
arr.Last        // "e"
arr.SecondLast  // "d"
arr.ThirdLast   // "c"

// These are settable too
arr.Last <- "z"          // arr is now [| "a"; "b"; "c"; "d"; "z" |]

// For single-element arrays
let single = [| 42 |]
single.FirstAndOnly  // 42
```

Or via module functions for use in pipelines:

```fsharp
[| 10; 20; 30 |] |> Array.first       // 10
[| 10; 20; 30 |] |> Array.last        // 30
[| 10; 20; 30 |] |> Array.secondLast  // 20
```

### Negative indexing (Python-style)

Use negative indices where `-1` is the last item, `-2` the second last, and so on:

```fsharp
let arr = [| "a"; "b"; "c"; "d"; "e" |]

arr.GetNeg(-1)    // "e" (last)
arr.GetNeg(-2)    // "d" (second last)
arr.SetNeg -1 "z"

// Module functions
arr |> Array.getNeg -1   // "e"
arr |> Array.setNeg -2 "x"
```

### Looped indexing

Treats the array as circular, wrapping around in both directions:

```fsharp
let arr = [| "a"; "b"; "c" |]

arr.GetLooped(3)    // "a" (wraps around)
arr.GetLooped(4)    // "b"
arr.GetLooped(-1)   // "c" (wraps backward)
arr.GetLooped(-4)   // "c"
```

### Slicing with negative indices

Unlike the built-in `a.[1..3]` slice syntax, `SliceNeg` supports negative indices:

```fsharp
let arr = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9 |]

arr.SliceNeg(2, 5)     // [| 2; 3; 4; 5 |]     (inclusive on both ends)
arr.SliceNeg(0, -1)    // [| 0; 1; .. ; 9 |]   (full copy, -1 = last)
arr.SliceNeg(1, -2)    // [| 1; 2; .. ; 8 |]   (skip first and last)
arr.SliceNeg(-3, -1)   // [| 7; 8; 9 |]        (last 3 items)

// Module function
arr |> Array.sliceNeg 1 -2    // [| 1; 2; .. ; 8 |]
```

(`arr.Slice` and `Array.slice` still work, but are obsolete: in .NET the `.Slice` method of some collections, like `List<'T>` and `Span<'T>`, takes a start index and a length.
For a start index and a length use `Array.sub`.)

`SliceIdx` rejects negative indices, `SliceLooped` wraps any index around with modulo:

```fsharp
arr.SliceIdx(2, 5)       // [| 2; 3; 4; 5 |]
arr.SliceIdx(-3, -1)     // throws IndexOutOfRangeException
arr.SliceLooped(-3, -1)  // [| 7; 8; 9 |]
arr.SliceLooped(10, 12)  // [| 0; 1; 2 |]  (10 wraps to 0, 12 to 2)
arr.SliceLooped(8, 11)   // [||]           (11 wraps to 1, which is before 8)

// Module functions
arr |> Array.sliceIdx 2 5
arr |> Array.sliceLooped -3 -1
```

### Grouping into a dictionary

`Array.groupByDict` uses F# structural equality for grouping and dictionary lookups on both .NET and Fable.
Keys must support equality (`'Key : equality`); null and `None` keys are rejected. Elements within each group keep their input order.

```fsharp
let groups = [|1; 2; 3; 4; 5|] |> Array.groupByDict (fun x -> [|x % 2|])
groups.[[|1|]]    // [|1; 3; 5|], looked up using a fresh, structurally equal array key
```

### Trimming

Remove items from the start and end:

```fsharp
let arr = [| 0; 1; 2; 3; 4; 5 |]

arr |> Array.trim 1 1   // [| 1; 2; 3; 4 |]  (trim 1 from each end)
arr |> Array.trim 2 0   // [| 2; 3; 4; 5 |]  (trim 2 from start)
```

### Rotating

Shift elements circularly:

```fsharp
let arr = [| 1; 2; 3; 4; 5 |]

arr |> Array.rotate 1    // [| 5; 1; 2; 3; 4 |]  (rotate up by 1)
arr |> Array.rotate -1   // [| 2; 3; 4; 5; 1 |]  (rotate down by 1)
arr |> Array.rotate 2    // [| 4; 5; 1; 2; 3 |]  (rotate up by 2)

// Rotate until a condition is met
[| 3; 1; 4; 1; 5 |] |> Array.rotateUpTill (fun x -> x = 5)
// [| 5; 3; 1; 4; 1 |]
```

### Windowed pairs and triples

Iterate over consecutive elements:

```fsharp
let arr = [| "a"; "b"; "c"; "d" |]

// Consecutive pairs (not looped, result is 1 shorter)
arr |> Array.windowed2
// seq { ("a","b"); ("b","c"); ("c","d") }

// Looped pairs (includes wrap-around, same length as input)
arr |> Array.thisNext
// seq { ("a","b"); ("b","c"); ("c","d"); ("d","a") }

arr |> Array.prevThis
// seq { ("d","a"); ("a","b"); ("b","c"); ("c","d") }

// Consecutive triples (not looped, result is 2 shorter)
arr |> Array.windowed3
// seq { ("a","b","c"); ("b","c","d") }

// Looped triples (same length as input)
arr |> Array.prevThisNext
// seq { ("d","a","b"); ("a","b","c"); ("b","c","d"); ("c","d","a") }
```

With indices:

```fsharp
let arr = [| 10; 20; 30; 40 |]

arr |> Array.windowed2i
// seq { (0, 10, 20); (1, 20, 30); (2, 30, 40) }

arr |> Array.iThisNext
// seq { (0, 10, 20); (1, 20, 30); (2, 30, 40); (3, 40, 10) }
```

### Min and max (top 2 and top 3)

Find the smallest or largest elements:

```fsharp
let arr = [| 5; 1; 9; 3; 7 |]

Array.min2 arr          // (1, 3)     smallest and second smallest
Array.max2 arr          // (9, 7)     biggest and second biggest
Array.min3 arr          // (1, 3, 5)
Array.max3 arr          // (9, 7, 5)

// By projection
let words = [| "hi"; "hello"; "hey" |]
Array.min2By String.length words   // ("hi", "hey")
Array.max2By String.length words   // ("hello", "hey")

// Get indices instead of values
Array.min2IndicesBy String.length words   // (0, 2)
Array.max2IndicesBy String.length words   // (1, 2)
```

### Finding duplicates

```fsharp
Array.duplicates [| 1; 2; 3; 2; 4; 3 |]
// [| 2; 3 |]    (each duplicate reported once, ordered by where it repeats)

Array.duplicatesBy String.length [| "hi"; "hey"; "go"; "bye" |]
// [| "go"; "bye" |]    (the items where length 2 and length 3 repeat)
```

### Partitioning

```fsharp
let small, medium, large =
    [| 1; 15; 150; 7; 70 |] |> Array.partition3 (fun x -> x < 10) (fun x -> x < 100)
// [| 1; 7 |], [| 15; 70 |], [| 150 |]   (each item goes to the first predicate that is true)

// With different result types:
let numbers, words =
    [| "1"; "a"; "2" |]
    |> Array.partitionBy (fun s -> match Int32.TryParse s with | true, i -> Choice1Of2 i | _ -> Choice2Of2 s)
// [| 1; 2 |], [| "a" |]
```

`partition4`, `partition5`, `partition3By`, `partition4By` and `partition5By` work the same way.

### Searching

```fsharp
let arr = [| 10; 20; 30; 40; 50 |]

Array.findValue 30 0 4 arr       // 2     (found at index 2)
Array.findValue 99 0 4 arr       // -1    (not found)
Array.findLastValue 30 0 4 arr   // 2     (search from end)

// Search for sub-array pattern
let hay = [| 1; 2; 3; 4; 5; 3; 4 |]
Array.findArray [| 3; 4 |] 0 6 hay       // 2   (first match)
Array.findLastArray [| 3; 4 |] 0 6 hay   // 5   (last match)
```

### Array checks

```fsharp
let arr = [| 1; 2; 3 |]

arr.IsEmpty         // false
arr.IsNotEmpty      // true
arr.HasItems        // true   (same as IsNotEmpty)
arr.IsSingleton     // false

// Module functions
arr |> Array.isNotEmpty         // true
arr |> Array.isSingleton        // false
arr |> Array.hasItems 3         // true   (exactly 3 items)
arr |> Array.hasMinimumItems 2  // true   (at least 2 items)
arr |> Array.hasMaximumItems 5  // true   (at most 5 items)
arr |> Array.count              // 3
arr |> Array.countIf (fun x -> x > 1)  // 2
```

### Validation

Chainable validation methods for defensive programming:

```fsharp
let result =
    someArray
    |> Array.failIfEmpty "Input array must not be empty"
    |> Array.failIfLessThan 3 "Need at least 3 elements"
    |> Array.map doSomething

// Extension member versions
someArray
    .FailIfEmpty("Input must not be empty")
    .FailIfLessThan(3, "Need at least 3 elements")
```

### Swapping

```fsharp
let arr = [| "a"; "b"; "c"; "d" |]
Array.swap 0 3 arr
// arr is now [| "d"; "b"; "c"; "a" |]
```

### String representation

```fsharp
let arr = [| 1; 2; 3; 4; 5; 6; 7 |]

arr.AsString
// "array<Int32> with 7 items:
//   0: 1
//   1: 2
//   2: 3
//   3: 4
//   4: 5
//   ...
//   6: 7"

arr.ToString(3)   // show only first 3 entries (and the last one)
// "array<Int32> with 7 items:
//   0: 1
//   1: 2
//   2: 3
//   ...
//   6: 7"
```

### Skipping index checks in Fable

When compiling with Fable you can define the `UNCHECKED` symbol, e.g. `dotnet fable --define UNCHECKED`.
Then `xs.Get`, `xs.Set`, `xs.Idx`, `Array.get` and `Array.set` skip their index checks (and `Array.get` and `Array.set` their null checks too)
and compile to plain JavaScript `arr[i]` access.
`Array.zeroCreateUndef` then emits `new Array(len)` without filling the items with default values.
It is only available for reference types.

This works because Fable compiles ArrayT from source.
On .NET the precompiled `ArrayT.dll` from NuGet is used, so `UNCHECKED` has no effect there.

## Use of AI and LLMs
All core functions are written by hand to ensure performance and correctness.<br>
However, AI tools have been used for code review, typo and grammar checking in documentation<br>
and to generate not all but many of the tests.


## Tests
All Tests run in both javascript and dotnet.
Successful Fable compilation to typescript is verified too.
Go to the tests folder:

```bash
cd Tests
```

The tests use [Scriptorium](https://fable-hub.github.io/Scriptorium/) (Scriptorium.Quill + Scriptorium.Nib).
The same test suite runs unchanged on .NET and on JS via Fable.

For testing with .NET:

```bash
dotnet run
```

for JS testing via Fable and TS verification:

```bash
npm ci      # first time only
npm test
```

## License
[MIT](https://github.com/goswinr/ArrayT/blob/main/LICENSE.md)

## Changelog
see [CHANGELOG.md](https://github.com/goswinr/ArrayT/blob/main/CHANGELOG.md)