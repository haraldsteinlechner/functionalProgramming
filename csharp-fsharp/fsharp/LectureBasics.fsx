
let intLiteral = 1


let lambdaPlusOne = fun x -> x + 1 
// type is "int -> int"

// semantically equivalent to above
let plusOne x = x + 1

let id x = x
// C# type would be
// T Id<T>(T x) { return x }
// automatically made generic by compiler

// types can also be specified
let plusOne' (x : int) : int = x + 1


let plusOne'' (v : int) =
    v + intLiteral



let addition = fun a -> (fun b -> a + b)
addition 10
// val it: (int -> int) = <fun:it@26>

let add10 = addition 10

List.map (fun a -> a + 1) [1; 2; 3]

List.map add10 [1; 2; 3]

List.map (addition 10) [1; 2; 3]

type MyOption<'a> =
    | NoValue
    | SomeValue of 'a

let aValue = SomeValue 10
let noValue = NoValue

let toStringOfOptionalValue (v : MyOption<int>) =
    match v with
    | NoValue -> "there was no value"
    | SomeValue v -> "the value was: " + string v

// pattern matching allows deep patterns
let toStringOfOptionalValue' (v : MyOption<int>) =
    match v with
    | NoValue -> "there was no value"
    | SomeValue 1 -> "the value was: " + string v
    | _ -> "pattern did not match"

let myOr (a : bool) (b : bool) =
    match (a,b) with
    | (true, true) -> true
    | (true, false) -> true
    | (false, true) -> true
    | _ -> false


let safeParseInt (v : string) : Option<int> = 
    match System.Int32.TryParse v with
    | (true, v) -> Some v
    | _ -> None


let addOneToIntInString (v : string) : Option<int> =
    let parsedResult = safeParseInt v
    match parsedResult with
    | None -> None
    | Some v -> Some (v + 1)

// here we use a function which might result in no result
// next we perform addition.

let addOneToIntInString' (v : string) =
    let parsed = safeParseInt v
    Option.map plusOne parsed

let mapOption (mapping : 'a -> 'b) (input : Option<'a>) =
    match input with
    | None -> None
    | Some v -> 
        let r = mapping v
        Some r

let addOneToIntInString'' (v : string) =
    let parsed = safeParseInt v
    mapOption (addition 1) parsed

let addOneToIntInStringToString'' (v : string) =
    let parsed = safeParseInt v
    let f v = string (v + 1)
    mapOption (fun v -> string (v + 1)) parsed

let plusOnToString = string << plusOne 
let plusOnToString' = plusOne >> string 

let addOneToIntInStringToString3 (v : string) =
    safeParseInt v |> mapOption (plusOne >> string) 


let evens = 
    [1;2;3] 
    |> List.filter (fun x -> x % 2 = 0)
    |> List.sum
