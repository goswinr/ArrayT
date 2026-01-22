# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.24.2] - 2026-01-24
### Fixed
- minor internal optimizations


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


[Unreleased]: https://github.com/goswinr/ArrayT/compare/0.24.1...HEAD
[0.24.1]: https://github.com/goswinr/ArrayT/compare/0.24.0...0.24.1
[0.24.0]: https://github.com/goswinr/ArrayT/compare/0.23.0...0.24.0
[0.23.0]: https://github.com/goswinr/ArrayT/compare/0.22.0...0.23.0
[0.22.0]: https://github.com/goswinr/ArrayT/compare/0.21.0...0.22.0
[0.21.0]: https://github.com/goswinr/ArrayT/compare/0.20.0...0.21.0
[0.20.0]: https://github.com/goswinr/ArrayT/compare/0.19.0...0.20.0
[0.19.0]: https://github.com/goswinr/ArrayT/releases/tag/0.19.0
