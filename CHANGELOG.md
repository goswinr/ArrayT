# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Changed
- `arr.AsString` now has the same name on .NET and in Fable. The old .NET-only `arr.asString` still works but is marked obsolete.
- `arr.AsString` and `arr.ToString(n)` are now inline on .NET too.
### Fixed
- Error messages of `Array.trim` and `Array.swap` showed a literal `%d` instead of the actual values.
- `arr.Slice` and `Array.slice` now report an out-of-range negative end index as such, instead of "start index is bigger than end index".
- `Array.min3`, `max3`, `min3By`, `max3By`, `min3IndicesBy` and `max3IndicesBy` did not keep the original order for ties among the first three items, as documented.
- `Array.matches`, `findArray` and `findLastArray` now raise descriptive exceptions for an empty or null search pattern (and `matches` for a null `searchIn`) instead of a bare `IndexOutOfRangeException` or `NullReferenceException`.
- `arr.ToString(Int32.MaxValue)` printed "..." and the last item twice, and `Array.trim` failed for very large trim counts, both because of an integer overflow.
- `Array.failIfEmpty` and `Array.failIfLessThan` now raise an `ArgumentNullException` for a null input instead of a `NullReferenceException`.
- Error messages of the min and max functions named internal helpers like `Array.MinMax.indexByFun` instead of the public function.
- The docs of `Array.minIndexBy` and `maxIndexBy` named the wrong exception type.
- Error messages of `Array.rotateUpTillLast` and `rotateDownTillLast` named the wrong function.
- The docstring of `Array.toResizeArray` said it returns a fixed-length Array.
- The docs of `Array.duplicates` and `duplicatesBy` now say that the second occurrence of each duplicate is returned; the README example was wrong.
- Clearer error message from `arr.FirstAndOnly` and `Array.firstAndOnly` when the Array does not have exactly one item.

## [0.26.1] - 2026-09-07
### Fixed
- Packaging: the Fable content glob is no longer recursive, so the package no longer ships generated obj AssemblyInfo files, only the real source files.

## [0.26.0] - 2026-03-07
### Changed
- allow Array.asResizeArray only on reference types

## [0.25.0] - 2026-01-24
### Added
- better xml docstrings

## [0.24.2] - 2026-01-24
### Fixed
- minor internal optimizations
### Added
- more tests


## [0.24.1] - 2026-01-17
### Fixed
- Fix bug in `rotateDownTillLast` that checked first element instead of last
- Fix typo in `firstAndOnly` docstring ("the the" → "the")
- Fix wrong function name in `toResizeArray` null exception message

## [0.24.0] - 2025-10-11
### Changed
- BREAKING CHANGE: Reorder parameters for Array.get and Array.set


## [0.23.0] - 2025-02-20
### Added
- DebugIdx Extension for nicer Error Messages on index out of bound exceptions

## [0.22.0] - 2024-11-02
### Added
- mapIfResult, deprecated applyIfResult

## [0.21.0] - 2024-11-01
### Added
- Documentation via [FSharp.Formatting](https://fsprojects.github.io/FSharp.Formatting/)
- better ToString methods

## [0.20.0] - 2024-09-15
### Added
- add filteri

## [0.19.0] - 2024-05-07
### Added
- copy from ResizeArray library 0.19.0

[Unreleased]: https://github.com/goswinr/ArrayT/compare/0.26.1...HEAD
[0.26.1]: https://github.com/goswinr/ArrayT/compare/0.26.0...0.26.1
[0.26.0]: https://github.com/goswinr/ArrayT/compare/0.25.0...0.26.0
<!-- [0.25.0]: https://github.com/goswinr/ArrayT/compare/0.24.2...0.25.0 -->
<!-- [0.24.2]: https://github.com/goswinr/ArrayT/compare/0.24.1...0.24.2 -->
[0.24.1]: https://github.com/goswinr/ArrayT/compare/0.24.0...0.24.1
[0.24.0]: https://github.com/goswinr/ArrayT/compare/0.23.0...0.24.0
[0.23.0]: https://github.com/goswinr/ArrayT/compare/0.22.0...0.23.0
[0.22.0]: https://github.com/goswinr/ArrayT/compare/0.21.0...0.22.0
[0.21.0]: https://github.com/goswinr/ArrayT/compare/0.20.0...0.21.0
[0.20.0]: https://github.com/goswinr/ArrayT/compare/0.19.0...0.20.0
[0.19.0]: https://github.com/goswinr/ArrayT/releases/tag/0.19.0
