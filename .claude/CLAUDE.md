# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ArrayT is an F# extension and module library for `Array<'T>` that provides better IndexOutOfRangeException messages (including the bad index and array size). It works on both .NET and JavaScript/TypeScript via Fable.

## Build Commands

```bash
# Build the solution
dotnet build

# Build in Release mode, as CI and the release workflow do (every build also generates the NuGet package)
dotnet build --configuration Release

# Restore dotnet tools (Fable, FsDocs)
dotnet tool restore
```

## Testing

Tests use [Scriptorium](https://fable-hub.github.io/Scriptorium/) (`Scriptorium.Quill` for the test DSL and runner, `Scriptorium.Nib` for assertions).
The same test suite runs unchanged on .NET and on JavaScript via Fable.

```bash
# Run .NET tests
cd Tests
dotnet run

# Run JavaScript tests (Fable compiles Tests.fsproj to _js and runs it with node, then verifies TypeScript compilation of Src)
cd Tests
npm ci      # first time only
npm test
```

Test style:

- Tests are written as `test ("name", fun _ -> ...)` inside `testList ("name", [ ... ])` (`open type Scriptorium.Quill.Test`).
- Assertions use `assertThat actual (tag "message" >> isEqualTo expected)`, `isTrue`, `isFalse`, `isNull` and `throws` (`open Scriptorium.Nib.Assertion`).
- `Tests/Helpers.fs` has the exception helpers `throwsRange`, `throwsNull`, `throwsArg` (exception type is only checked on .NET) and `throwsWith ["part"; ...]` (checks the message content on .NET and JS).
- All test lists are run by a single `runTests [ ... ]` call in `Tests/Main.fs`; on JS the process exits once the run completes.
- Test names must be unique within a list, Scriptorium rejects duplicate test paths.

## Project Structure

- `Src/` - Main library source
  - `Util.fs` - Internal utilities for exceptions and index handling (`UtilArray` module, `DebugIndexer` type)
  - `Extensions.fs` - Extension members on `Array<'T>` (`.Get`, `.Set`, `.First`, `.Last`, `.SliceNeg`, etc.)
  - `MinMax.fs` - Internal `MinMax` module with the inline helpers and NaN handling for the min and max functions
  - `Module.fs` - `Array` module functions that extend FSharp.Core's Array module
- `Tests/` - Test project that runs on both .NET and Fable/JS
- `.github/workflows/` - `build.yml` builds and runs all tests; `releaseNuget.yml` runs it on a version tag, then publishes to nuget.org via trusted publishing (OIDC, no API key secret)
- `Docs/` - FsDocs documentation assets

## Key Patterns

- All public `Array` module functions check for null input and throw via `nullExn` (the extension members don't)
- Functions starting with `try...` return F# Option; all others throw descriptive exceptions on failure
- Negative indexing (Python-style, -1 = last item) is supported via `GetNeg`/`SetNeg` and `getNeg`/`setNeg`
- `DebugIdx` property provides indexer with descriptive exceptions: `arr.DebugIdx.[i]`
- Version is managed via CHANGELOG.md using `Ionide.KeepAChangelog.Tasks`; add user-facing changes under `## [Unreleased]`

## Fable Compatibility

- Code uses `#if FABLE_COMPILER` / `#if FABLE_COMPILER_JAVASCRIPT` for platform-specific implementations
- Members that need `typeof<'T>` (like `AsString`, `ToString(n)`) are `inline` so reflection works in Fable
- Defining `UNCHECKED` (e.g. `dotnet fable --define UNCHECKED`) skips index checks in `Get`/`Set`/`Idx`/`Array.get`/`Array.set`; it only affects Fable, which compiles the library from source
- The library targets `net8.0` and `net472` and includes F# source files for Fable compilation
