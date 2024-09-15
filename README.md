
# ArrayT

[![ArrayT on nuget.org](https://img.shields.io/nuget/v/ArrayT)](https://www.nuget.org/packages/ArrayT/)
[![ArrayT on fuget.org](https://www.fuget.org/packages/ArrayT/badge.svg)](https://www.fuget.org/packages/ArrayT)
![code size](https://img.shields.io/github/languages/code-size/goswinr/ArrayT.svg)
[![license](https://img.shields.io/github/license/goswinr/ArrayT)](LICENSE)

ArrayT is an F# extension and module library for `Array<'T>`

It also works with [Fable](https://fable.io/).


![Logo](https://raw.githubusercontent.com/goswinr/ArrayT/main/Doc/logo.png)

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


### License
[MIT](https://raw.githubusercontent.com/goswinr/ArrayT/main/LICENSE.md)

### Test
All Tests run in both javascript and dotnet.
go to the tests folder

```bash
cd Tests
```

For testing with .NET using Expecto run

```bash
dotnet run
```

for testing with Fable.Mocha run

```bash
npm test
```


### Changelog
`0.20.0`
- add filteri

`0.19.0`
- copy from ResizeArray library 0.19.0