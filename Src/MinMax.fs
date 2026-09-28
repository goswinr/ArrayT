namespace ArrayT

open System

#nowarn "44" //for opening the hidden but not Obsolete UtilArray module
open UtilArray
#warnon "44"

// internal, only for finding MinMax values
// cmp is (<) for the smallest and (>) for the biggest values.
// funcName is the name of the public function, for the error message.
//
// NaN handling:
// IEEE 754:2019 defines 'minimum' and 'maximum', which propagate NaN,
// and 'minimumNumber' and 'maximumNumber', which skip NaN.
// Both are legitimate, but different operations. See https://github.com/dotnet/fsharp/issues/13207#issuecomment-1194411950
// Array.min_IEEE754 and max_IEEE754 propagate NaN, Array.minNumber and maxNumber skip it.
// All other functions, like minNumberBy, minIndexBy or min2, rank NaN after all other values.
// Every comparison with NaN is false, so a loop that only replaces its current best value when cmp is true
// never picks a NaN, unless it starts with one. That's why the functions below only need extra NaN checks
// while their current best value is NaN.
// For float and float32 the functions in UtilArray are used instead of propagateNaN and skipNaN,
// to also treat -0.0 as smaller than +0.0.
[<RequireQualifiedAccess>]
module internal MinMax =

    /// True for NaN, or for a key that contains NaN, like the tuple (nan, 1).
    /// NaN is the only value that is not smaller than or equal to itself.
    /// This uses comparison and not equality, so it does not add an equality constraint.
    let inline isNaN (x: 'T) : bool = not (x <= x)

    // The index counterpart of a stable sorting network for three values.
    // Since cmp is strict (< or >), equal keys always stay in their original index order.
    // Only cmp is used, not '=', so this also works for types whose equality disagrees with their comparison.
    let inline indexOfSort3By f cmp aa bb cc : int * int * int =
        let a = f aa
        let b = f bb
        let c = f cc
        if cmp b a then
            if cmp c b then 2, 1, 0
            elif cmp c a then 1, 2, 0
            else 1, 0, 2
        else
            if cmp c a then 2, 0, 1
            elif cmp c b then 0, 2, 1
            else 0, 1, 2

    /// The index of the best key. NaN keys are skipped, if all keys are NaN, 0 is returned.
    let inline indexByFun (funcName: string) cmp func (arr: 'T[]) : int =
        if arr.Length < 1 then fail arr $"{funcName}: Count must be at least one"
        let mutable ii = 0
        let mutable mf = func arr.[0]
        let mutable i = 1
        // skip leading NaN keys, but keep index 0 if all keys are NaN
        while i < arr.Length && isNaN mf do
            let f = func arr.[i]
            if not (isNaN f) then
                ii <- i
                mf <- f
            i <- i + 1
        // from here on NaN keys are skipped without any check, because cmp is false for NaN
        while i < arr.Length do
            let f = func arr.[i]
            if cmp f mf then
                ii <- i
                mf <- f
            i <- i + 1
        ii

    /// The indices of the best two keys. NaN keys are ranked last.
    let inline index2ByFun (funcName: string) cmp func (arr: 'T[]) : int * int =
        if arr.Length < 2 then fail arr $"{funcName}: Count must be at least two"
        let mutable i1 = 0
        let mutable i2 = 1
        let mutable mf1 = func arr.[i1]
        let mutable mf2 = func arr.[i2]
        // While a best key is NaN, any key that is not NaN is better.
        let mutable nan1 = isNaN mf1
        let mutable nan2 = isNaN mf2
        for i = 1 to arr.Length - 1 do // starts at 1 to put the first two in order
            let f = func arr.[i]
            if cmp f mf1 || (nan1 && not (isNaN f)) then
                i2 <- i1
                mf2 <- mf1
                nan2 <- nan1
                i1 <- i
                mf1 <- f
                nan1 <- false
            elif cmp f mf2 || (nan2 && not (isNaN f)) then
                i2 <- i
                mf2 <- f
                nan2 <- false
        i1, i2

    /// The indices of the best three keys. NaN keys are ranked last.
    let inline index3ByFun (funcName: string) (cmp: 'U -> 'U -> bool) (func: 'T -> 'U) (arr: 'T[]) : int * int * int =
        if arr.Length < 3 then fail arr $"{funcName}: Count must be at least three"
        // like cmp, but any key that is not NaN is also better than a NaN key
        let inline better a b = cmp a b || (isNaN b && not (isNaN a))
        // sort first 3
        let mutable i1, i2, i3 = indexOfSort3By func better arr.[0] arr.[1] arr.[2] // otherwise would fail on sorting first 3, test on Array([5;6;3;1;2;0])|> Array.max3
        let mutable e1 = func arr.[i1]
        let mutable e2 = func arr.[i2]
        let mutable e3 = func arr.[i3]
        // While a best key is NaN, any key that is not NaN is better.
        let mutable nan1 = isNaN e1
        let mutable nan2 = isNaN e2
        let mutable nan3 = isNaN e3
        for i = 3 to arr.Length - 1 do
            let f = func arr.[i]
            if cmp f e1 || (nan1 && not (isNaN f)) then
                i3 <- i2
                e3 <- e2
                nan3 <- nan2
                i2 <- i1
                e2 <- e1
                nan2 <- nan1
                i1 <- i
                e1 <- f
                nan1 <- false
            elif cmp f e2 || (nan2 && not (isNaN f)) then
                i3 <- i2
                e3 <- e2
                nan3 <- nan2
                i2 <- i
                e2 <- f
                nan2 <- false
            elif cmp f e3 || (nan3 && not (isNaN f)) then
                i3 <- i
                e3 <- f
                nan3 <- false
        i1, i2, i3

    /// The best two values. NaN is ranked last.
    let inline simple2 (funcName: string) cmp (arr: 'T[]) : 'T * 'T =
        let i1, i2 = index2ByFun funcName cmp id arr
        arr.[i1], arr.[i2]

    /// The best three values. NaN is ranked last.
    let inline simple3 (funcName: string) cmp (arr: 'T[]) : 'T * 'T * 'T =
        let i1, i2, i3 = index3ByFun funcName cmp id arr
        arr.[i1], arr.[i2], arr.[i3]

    /// The IEEE 754:2019 'minimum' or 'maximum' for any 'T: NaN propagates.
    /// keep is (>=) for the minimum and (<=) for the maximum.
    /// Unlike cmp, 'not keep' is also true for NaN, so a NaN is taken, and then the loop stops.
    let inline propagateNaN keep (arr: 'T[]) : 'T =
        let mutable acc = arr.[0]
        let mutable foundNaN = isNaN acc
        let mutable i = 1
        while not foundNaN && i < arr.Length do
            let x = arr.[i]
            if not (keep x acc) then // x is better or NaN
                acc <- x
                foundNaN <- isNaN x
            i <- i + 1
        acc

    /// The IEEE 754:2019 'minimumNumber' or 'maximumNumber' for any 'T: NaN is skipped.
    let inline skipNaN (funcName: string) cmp (arr: 'T[]) : 'T =
        arr.[indexByFun funcName cmp id arr]
