
![Logo](https://raw.githubusercontent.com/goswinr/ArrayT/main/Docs/img/logo128.png)
# ArrayT

[![ArrayT on nuget.org](https://img.shields.io/nuget/v/ArrayT)](https://www.nuget.org/packages/ArrayT/)
[![Build Status](https://github.com/goswinr/ArrayT/actions/workflows/build.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/build.yml)
[![Docs Build Status](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/docs.yml)
[![Test Status](https://github.com/goswinr/ArrayT/actions/workflows/test.yml/badge.svg)](https://github.com/goswinr/ArrayT/actions/workflows/test.yml)
[![license](https://img.shields.io/github/license/goswinr/ArrayT)](LICENSE.md)
![code size](https://img.shields.io/github/languages/code-size/goswinr/ArrayT.svg)

ArrayT is an F# extension and module library for `Array<'T>`

It also works in JS and TS with [Fable](https://fable.io/).

This library was designed for use with F# scripting.
Functions and methods never return null.
Only functions starting with `try...` will return an F# Option.
Otherwise when a function fails on invalid input it will throw a descriptive exception.

I was always annoyed that an IndexOutOfRangeException does not include the actual index that was out of bounds nor the actual size of the array.
This library fixes that in `array.Get`, `array.Set`, `array.Slice` and other item access functions.

See also https://github.com/goswinr/ResizeArray/ for a similar library for `ResizeArray<'T>`.

### It Includes:

- A `Array` module that has a additional functions to the  [`Array` module from `FSharp.Core`](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html).

- Extension members on `Array` like `.Get` `.Set` `.First` `.Last` `.SecondLast` and more.
With nicer IndexOutOfRangeExceptions that include the bad index and the actual size.



### Usage
Just open the namespace

```fsharp
open ArrayT
```
this namespace contains:

- a module also called `Array`

- this will also auto open the extension members on `Array<'T>`


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