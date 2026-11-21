/// Benchmark-only prototypes of ArrayT functions that take callbacks, written like the FSharp.Core 11 Array module:
/// https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/array.fs
/// The current library implementations are benchmarked by calling ArrayT.Array directly.
module Prototypes

#nowarn "44" // the hidden UtilArray module, same error helpers as the library
open ArrayT.UtilArray

/// Not inline, the callback adapted once with OptimizedClosures, as ResizeArrayT does it.
/// Works with FSharp.Core 6. Only for callbacks with two or more arguments.
module Adapt =

    let filteri (predicate: int -> 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "filteri"
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt predicate
        let res = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let t = arr.[i]
            if f.Invoke(i, t) then res.Add t
        res.ToArray()

    let findIndexi (predicate: int -> 'T -> bool) (arr: 'T[]) : int =
        if isNull arr then nullExn "findIndexi"
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt predicate
        // like the current tryFindIndexi, then the match of findIndexi
        let mutable i = 0
        let mutable result = None
        let k = arr.Length
        while i < k do
            if f.Invoke(i, arr.[i]) then
                result <- Some i
                i <- k // break the loop
            else
                i <- i + 1
        match result with
        | Some i -> i
        | None -> failKey arr "findIndexi did not find for given predicate in"

    let mapPrevNext (combineAdjacent: 'T -> 'T -> 'U) (mergePrevAndNextCombineResults: 'T -> 'U -> 'U -> 'V) (arr: 'T[]) : 'V[] =
        if isNull arr then nullExn "mapPrevNext"
        let combine = OptimizedClosures.FSharpFunc<_, _, _>.Adapt combineAdjacent
        let merge = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt mergePrevAndNextCombineResults
        let len = arr.Length
        if len = 0 then
            [||]
        else
            let res = Array.zeroCreate len
            let mutable this = arr.[0]
            let mutable prevCombined = combine.Invoke(arr.[len - 1], this)
            for i = 1 to len - 1 do
                let next = arr.[i]
                let nextCombined = combine.Invoke(this, next)
                res.[i - 1] <- merge.Invoke(this, prevCombined, nextCombined)
                this <- next
                prevCombined <- nextCombined
            let nextCombined = combine.Invoke(this, arr.[0])
            res.[len - 1] <- merge.Invoke(this, prevCombined, nextCombined)
            res


/// <c>inline</c> with <c>[&lt;InlineIfLambda&gt;]</c> only. Works with FSharp.Core 6.
module InlineIfLambda =

    let inline filteri ([<InlineIfLambda>] predicate: int -> 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "filteri"
        let res = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let t = arr.[i]
            if predicate i t then res.Add t
        res.ToArray()

    let inline findIndexi ([<InlineIfLambda>] predicate: int -> 'T -> bool) (arr: 'T[]) : int =
        if isNull arr then nullExn "findIndexi"
        // like the current tryFindIndexi, then the match of findIndexi
        let mutable i = 0
        let mutable result = None
        let k = arr.Length
        while i < k do
            if predicate i arr.[i] then
                result <- Some i
                i <- k // break the loop
            else
                i <- i + 1
        match result with
        | Some i -> i
        | None -> failKey arr "findIndexi did not find for given predicate in"

    let inline mapPrevNext
        ([<InlineIfLambda>] combineAdjacent: 'T -> 'T -> 'U)
        ([<InlineIfLambda>] mergePrevAndNextCombineResults: 'T -> 'U -> 'U -> 'V)
        (arr: 'T[])
        : 'V[] =
        if isNull arr then nullExn "mapPrevNext"
        let len = arr.Length
        if len = 0 then
            [||]
        else
            let res = Array.zeroCreate len
            let mutable this = arr.[0]
            let mutable prevCombined = combineAdjacent arr.[len - 1] this
            for i = 1 to len - 1 do
                let next = arr.[i]
                let nextCombined = combineAdjacent this next
                res.[i - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
                this <- next
                prevCombined <- nextCombined
            let nextCombined = combineAdjacent this arr.[0]
            res.[len - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
            res

    // One-argument callbacks: here [<InlineIfLambda>] is the complete FSharp.Core 11 pattern.

    let inline pickBack ([<InlineIfLambda>] chooser: 'T -> 'U option) (arr: 'T[]) : 'U =
        if isNull arr then nullExn "pickBack"
        let mutable i = arr.Length - 1
        let mutable result = None
        while i >= 0 && result.IsNone do
            result <- chooser arr.[i]
            i <- i - 1
        match result with
        | Some res -> res
        | None -> failKey arr $"pickBack: Key not found in {arr.Length} elements"

    let inline partition3 ([<InlineIfLambda>] predicate1: 'T -> bool) ([<InlineIfLambda>] predicate2: 'T -> bool) (arr: 'T[]) : 'T[] * 'T[] * 'T[] =
        if isNull arr then nullExn "partition3"
        let p1True = ResizeArray()
        let p2True = ResizeArray()
        let allFalse = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let el = arr.[i]
            if predicate1 el then p1True.Add el
            elif predicate2 el then p2True.Add el
            else allFalse.Add el
        p1True.ToArray(), p2True.ToArray(), allFalse.ToArray()

    /// A copy of the internal MinMax.indexByFun of ArrayT, with [<InlineIfLambda>] on the projection.
    /// The projection is called at three places.
    let inline minIndexBy ([<InlineIfLambda>] projection: 'T -> 'Key) (arr: 'T[]) : int =
        if isNull arr then nullExn "minIndexBy"
        if arr.Length < 1 then fail arr "minIndexBy: Count must be at least one"
        let inline isNaN (x: 'Key) = not (x <= x)
        let mutable ii = 0
        let mutable mf = projection arr.[0]
        let mutable i = 1
        while i < arr.Length && isNaN mf do
            let f = projection arr.[i]
            if not (isNaN f) then
                ii <- i
                mf <- f
            i <- i + 1
        while i < arr.Length do
            let f = projection arr.[i]
            if f < mf then
                ii <- i
                mf <- f
            i <- i + 1
        ii


/// <c>inline</c> with <c>[&lt;InlineIfLambda; OptimizeClosureIfNotInlined&gt;]</c>, for callbacks with 2 to 5 arguments.
/// Needs FSharp.Core 11 and F# 11. Older compilers and Fable ignore the attribute.
module OptimizeClosure =

    let inline filteri ([<InlineIfLambda; OptimizeClosureIfNotInlined>] predicate: int -> 'T -> bool) (arr: 'T[]) : 'T[] =
        if isNull arr then nullExn "filteri"
        let res = ResizeArray()
        for i = 0 to arr.Length - 1 do
            let t = arr.[i]
            if predicate i t then res.Add t
        res.ToArray()

    let inline findIndexi ([<InlineIfLambda; OptimizeClosureIfNotInlined>] predicate: int -> 'T -> bool) (arr: 'T[]) : int =
        if isNull arr then nullExn "findIndexi"
        // like the current tryFindIndexi, then the match of findIndexi
        let mutable i = 0
        let mutable result = None
        let k = arr.Length
        while i < k do
            if predicate i arr.[i] then
                result <- Some i
                i <- k // break the loop
            else
                i <- i + 1
        match result with
        | Some i -> i
        | None -> failKey arr "findIndexi did not find for given predicate in"

    let inline mapPrevNext
        ([<InlineIfLambda; OptimizeClosureIfNotInlined>] combineAdjacent: 'T -> 'T -> 'U)
        ([<InlineIfLambda; OptimizeClosureIfNotInlined>] mergePrevAndNextCombineResults: 'T -> 'U -> 'U -> 'V)
        (arr: 'T[])
        : 'V[] =
        if isNull arr then nullExn "mapPrevNext"
        let len = arr.Length
        if len = 0 then
            [||]
        else
            let res = Array.zeroCreate len
            let mutable this = arr.[0]
            let mutable prevCombined = combineAdjacent arr.[len - 1] this
            for i = 1 to len - 1 do
                let next = arr.[i]
                let nextCombined = combineAdjacent this next
                res.[i - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
                this <- next
                prevCombined <- nextCombined
            let nextCombined = combineAdjacent this arr.[0]
            res.[len - 1] <- mergePrevAndNextCombineResults this prevCombined nextCombined
            res
