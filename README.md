
![Logo](https://raw.githubusercontent.com/goswinr/ArrayT/main/Docs/img/logo128.png)
# ArrayT

[![ArrayT on nuget.org](https://img.shields.io/nuget/v/ArrayT)](https://www.nuget.org/packages/ArrayT/)
[![Build Status](https://github.com/goswinr/ArrayT/actions/workflows/build.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/build.yml)
[![Docs Build Status](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml)
[![Test Status](https://github.com/goswinr/ArrayT/actions/workflows/test.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/test.yml)
[![Check dotnet tools](https://github.com/goswinr/ArrayT/actions/workflows/outdatedDotnetTool.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/outdatedDotnetTool.yml)
[![license](https://img.shields.io/github/license/goswinr/ArrayT)](LICENSE.md)
![code size](https://img.shields.io/github/languages/code-size/goswinr/ArrayT.svg)

ArrayT is an F# extension and module library for `Array<'T>`

It also works in Javascript and Typescript with [Fable](https://fable.io/).

### Motivation
I was always annoyed that an IndexOutOfRangeException does not include the actual index that was out of bounds nor the actual size of the array.
This library fixes that in `array.Get`, `array.Set`, `array.Slice` and other item access functions.

This library was designed for use with F# scripting.<br>
Functions and methods never return null.<br>
Only functions starting with `try...` will return an F# Option.<br>
Otherwise when a function fails on invalid input it will throw a descriptive exception.

See also https://github.com/goswinr/ResizeArray/ for a similar library for `ResizeArray<'T>`.

### It Includes:

- An `Array` module that has a additional functions to the  `Array` module from [`FSharp.Core`](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html).<br>
See [docs](https://goswinr.github.io/ArrayT/reference/arrayt-array.html)

- Extension members on `Array` like <br>
`.Get(idx)` `.Set(idx,item)` `.First` `.Last` `.SecondLast` `xs.DebugIdx.[i]` and more..<br>
With nicer IndexOutOfRangeExceptions that include the bad index and the actual size.<br>
See [docs](https://goswinr.github.io/ArrayT/reference/arrayt-autoopenarraytextensions.html)


### Usage
Just open the namespace

```fsharp
open ArrayT
```
this namespace contains:

- a module also called `Array`. It will add additional functions to the `Array` module from `FSharp.Core`.

- this will also auto open the extension members on `Array<'T>`

### Example

```fsharp
#r "nuget: ArrayT"
open ArrayT

let xs = [| 0 .. 88 |]

xs.Get(99)
```
throws

```
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

instead of the usual

`System.IndexOutOfRangeException: Index was outside the bounds of the array.`

If you want to use the index notation `xs.[i]` instead of the Get method you can use the `DebugIdx` member

`xs.DebugIdx.[i]`



### Full API Documentation

[goswinr.github.io/ArrayT](https://goswinr.github.io/ArrayT/reference/arrayt.html)


### Tests
All Tests run in both javascript and dotnet.
Successful Fable compilation to typescript is verified too.
Go to the tests folder:

```bash
cd Tests
```

For testing with .NET using Expecto:

```bash
dotnet run
```

for JS testing with Fable.Mocha and TS verification:

```bash
npm test
```

### License
[MIT](https://github.com/goswinr/ArrayT/blob/main/LICENSE.md)

### Changelog
see [CHANGELOG.md](https://github.com/goswinr/ArrayT/blob/main/CHANGELOG.md)