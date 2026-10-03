namespace ArrayT

open System
open System.Collections.Generic

/// Invalid argument to an ArrayT operation.
type ArrayTArgumentException(message: string) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    inherit Exception(message)
    #else
    inherit ArgumentException(message)
    #endif

/// A required array or projected value was null.
type ArrayTArgumentNullException(paramName: string, message: string) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    inherit Exception(message)
    member _.ParamName = paramName
    #else
    inherit ArgumentNullException(paramName, message)
    #endif

/// No matching item was found.
type ArrayTKeyNotFoundException(message: string) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    inherit Exception(message)
    #else
    inherit KeyNotFoundException(message)
    #endif
