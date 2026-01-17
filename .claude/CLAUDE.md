# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ArrayT is an F# extension and module library for `Array<'T>` that provides better IndexOutOfRangeException messages (including the bad index and array size). It works on both .NET and JavaScript/TypeScript via Fable.

## Build Commands

```bash
# Build the solution
dotnet build

# Build in Release mode (generates NuGet package)
dotnet build --configuration Release

# Restore dotnet tools (Fable, FsDocs)
dotnet tool restore
```

## Testing

Tests run on both .NET (Expecto) and JavaScript (Fable.Mocha with Mocha).

```bash
# Run .NET tests
cd Tests
dotnet run

# Run JavaScript tests (includes Fable compilation and TypeScript verification)
cd Tests
npm ci      # first time only
npm test
```

## Project Structure

- `Src/` - Main library source
  - `Util.fs` - Internal utilities for exceptions and index handling (`UtilArray` module, `DebugIndexer` type)
  - `Extensions.fs` - Extension members on `Array<'T>` (`.Get`, `.Set`, `.First`, `.Last`, `.Slice`, etc.)
  - `Module.fs` - `Array` module functions that extend FSharp.Core's Array module
- `Tests/` - Test project that runs on both .NET and Fable/JS
- `Docs/` - FsDocs documentation assets

## Key Patterns

- All public functions check for null input and throw via `nullExn`
- Functions starting with `try...` return F# Option; all others throw descriptive exceptions on failure
- Negative indexing (Python-style, -1 = last item) is supported via `GetNeg`/`SetNeg` and `getNeg`/`setNeg`
- `DebugIdx` property provides indexer with descriptive exceptions: `arr.DebugIdx.[i]`
- Version is managed via CHANGELOG.md using `Ionide.KeepAChangelog.Tasks`

## Fable Compatibility

- Code uses `#if FABLE_COMPILER` / `#if FABLE_COMPILER_JAVASCRIPT` for platform-specific implementations
- The library targets `netstandard2.0` and includes F# source files for Fable compilation
