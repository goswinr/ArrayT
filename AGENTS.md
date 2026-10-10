# Project guidelines

ArrayT is an F# extension and module library for `Array<'T>` for .NET and JavaScript/TypeScript via Fable. Its main purpose is better error messages: out-of-range exceptions include the bad index, the array size, and a preview of the content. Keep changes small, targeted, and consistent with the F# style in `Src/`. See `README.md` for usage and API examples.

## Architecture and code style

Preserve the compilation order in `Src/ArrayT.fsproj`:

1. `Src/Exceptions.fs`: `ArrayTArgumentException`, `ArrayTArgumentNullException`, and `ArrayTKeyNotFoundException`. On .NET they inherit the matching `System` exceptions; in Fable they inherit `Exception`.
2. `Src/Util.fs`: the hidden `UtilArray` module (exception helpers such as `nullExn`, `badGetExn`, `badSetExn`, `fail`, `failIdx`, `failKey`; unchecked access; float min/max helpers) and the `DebugIndexer` type.
3. `Src/Extensions.fs`: the auto-opened `AutoOpenArrayTExtensions` module with members on `'T[]` such as `Get`, `Set`, `Idx`, `GetNeg`, `SetNeg`, `First`, `Last`, `SliceNeg`, `DebugIdx`, and `AsString`.
4. `Src/MinMax.fs`: the internal `MinMax` module with inline helpers and NaN handling for the min and max functions.
5. `Src/Module.fs`: the `Array` module that extends FSharp.Core's `Array` module.

Opening the `ArrayT` namespace exposes the extension members and the extra `Array` module functions.

- Do not reorder source files unless the change requires it.
- Public `Array` module functions check for null input with `nullExn`; the extension members do not.
- Functions named `try...` return an F# `Option`; all others throw descriptive exceptions on failure. Build messages with the `UtilArray` helpers so they include the function name and array content.
- Negative indexing (Python-style, `-1` is the last item) is supported by `GetNeg` / `SetNeg` and `Array.getNeg` / `Array.setNeg`. `arr.DebugIdx.[i]` is an indexer with descriptive exceptions.
- Preserve public API behavior across .NET and Fable targets. JavaScript/TypeScript-specific code uses `#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT`, mainly in `Src/Util.fs` and `Src/Exceptions.fs`.
- Members that need `typeof<'T>`, such as `AsString` and `ToString(n)`, must stay `inline` so reflection works in Fable.
- Defining `UNCHECKED` (for example `dotnet fable --define UNCHECKED`) skips index checks in `Get`, `Set`, `Idx`, `Array.get`, and `Array.set`, and makes `Array.zeroCreateUndef` emit `new Array(len)`. It only affects Fable, which compiles the library from the F# sources packed into the NuGet package.
- Keep `Obsolete` aliases when renaming public API, pointing to the new name.
- The version comes from `CHANGELOG.md` via `Ionide.KeepAChangelog.Tasks`. Add user-facing changes under `## [Unreleased]`, one single-line bullet per item, with CRLF line endings.

## Build and test

Run builds from the repository root with the .NET 10 SDK. The library targets `net8.0`, `net10.0` and `net472`; tests target `net8.0` and `net10.0`. Install the .NET 8 runtime too. FsDocs uses the first library target, `net8.0`. Every build also creates the NuGet package (`GeneratePackageOnBuild`).

```bash
dotnet build ArrayT.sln
dotnet build Src/ArrayT.fsproj
dotnet build --configuration Release   # as CI and the release workflow do
```

Run tests from `Tests/`:

```bash
dotnet run --framework net8.0   # .NET 8 tests
dotnet run --framework net10.0  # .NET 10 tests
npm test             # JavaScript tests via Fable and Node.js, then TypeScript compilation of Src
npm run buildTS      # TypeScript compilation of Src only
npm run watchTS      # Watch mode for TypeScript development
```

For a first JavaScript test run or a clean environment, run `dotnet tool restore` from the repository root and `npm ci` in `Tests/`.

CI (`.github/workflows/build.yml`) builds in Release and runs all three test targets. `releaseNuget.yml` runs on a version tag, checks the tag against the `CHANGELOG.md` version, and publishes to nuget.org via trusted publishing (OIDC, no API key secret).

## Test conventions

The .NET and JavaScript targets share the test lists in `Tests/Extensions.fs`, `Tests/Module2.fs`, and `Tests/FableParity.fs`. Their single entry point is one `runTests [ ... ]` call in `Tests/Main.fs`; on JavaScript the process exits once that run completes. Add or update tests in `Tests/FableParity.fs` when changing code in Fable-specific branches to check parity across targets.

Tests use [Scriptorium](https://fable-hub.github.io/Scriptorium/):

- `Scriptorium.Quill` supplies `testList ("name", [ ... ])`, `test ("name", fun _ -> ...)`, and `Runner.runTests` (`open type Scriptorium.Quill.Test`).
- `Scriptorium.Nib` supplies assertions such as `assertThat result (tag "message" >> isEqualTo expected)`, `isTrue`, `isFalse`, `isNull`, and `throws` (`open Scriptorium.Nib.Assertion`).
- `Tests/Helpers.fs` provides `throwsRange`, `throwsNull`, and `throwsArg` (the exception type is only checked on .NET) and `throwsWith ["part"; ...]` (checks the message content on .NET and JavaScript).
- Test names must be unique within a list; duplicate paths are rejected.
- There is no CLI `--filter`. To run a subset temporarily, use `ftest` / `ftestList` or `xtest` / `xtestList`. Focused tests fail the CI run, so remove focus markers afterward.
