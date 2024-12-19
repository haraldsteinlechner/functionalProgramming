module ListsAndHOF_B

// 1. recap: what are higher order functions
// how they help getting rid of duplicate code
// use-cases of discrimnated unions
// 2. function composition and its use caes. how to use it to 
// make code more readable and succinct.
// 3. implement single linked list using union type (done together)
// 4. implementation of core list functions (as found in linq, streams)
// e.g. map, filter, sum
// 5. detection of code duplication in those, and how to abstract over
// list operations generially using aggregation (often called fold, reduce etc)

let parseInt (str : string) : Option<int> =
    match System.Int32.TryParse(str) with
    | (true, v) -> Some v
    | _ -> None


let parseIntAndAddOne (s : string) = 
    let parsedInt = parseInt s
    match parsedInt with
    | None -> None
    | Some v -> 
        // here we know that we have
        Some (v + 1)

let parseIntAndAddOneAndtoString (s : string) = 
    match parseIntAndAddOne s with
    | None -> None
    | Some v -> Some $"the value was: {v}"

// higher order function to the rescue!!

let add x y = x + y

let parseIntAndAddOneAndtoString2 (s : string) =
    s |> parseInt |> Option.map (add 1) |> Option.map string


type Result<'r, 'e> =
    | Success of 'r // union type constructors
    | Error of 'e   // cases

let r = Success "yea"

match r with
| Success result -> $"result:{result}"
| Error 0 -> "failed 0"
| Error 1 -> "failed 1"
| Error n -> "failed unspecfic error code"


let mapOption f optionValue =
    match optionValue with
    | None -> None
    | Some r -> Some (f r)

let parseIntAndAddOneAndtoString3 (s : string) =
    s |> parseInt |> Option.map (add 1) |> Option.map string

let parseIntAndAddOneAndtoString4 (s : string) =
    s |> parseInt |> Option.map (add 1 >> string)

let (>>) f g = fun x -> f (g x)

[1;2;3] |> List.map (string >> (add 1))


type MyList<'a> =
    | Empty
    | Cons of 'a * MyList<'a>

let oneTwoThreee = Cons (1, Cons(2, Empty))
let oneTwoThreeeChar = Cons ('1', Cons('2', Empty)) 

let addFront (v : 'a) (myList : MyList<'a>) =
    Cons(v, myList)

let oneTwoThreeeInBaseLib = 1 :: 2 :: [] 
let oneTwoThreeeInBaseLibLiteral = [1;2]

let addFrontUsingBaseLib (v : 'a) (myList : list<'a>) =
    v :: myList

let rec sumMyList (xs : MyList<int>) =
    match xs with
    | Empty -> 0
    | Cons(head,rest) -> head + sumMyList rest
    

let rec sum (xs : list<int>) =
    match xs with
    | [] -> 0
    | head :: rest -> head + sum rest

[1;2;3;4] |> List.map (fun x -> x * 2) |> List.filter (fun x -> x % 2 = 1) |> List.sum

let rec filter (predicate : 'a -> bool) (xs : list<'a>) : list<'a> =
    match xs with
    | [] -> []
    | x::xs -> 
        if predicate x then
            x :: filter predicate xs
        else 
            filter predicate xs 


let rec map f xs =
    match xs with
    | [] -> []
    | x::xs -> f x :: map f xs

let addF x y = x + y
    

let rec any (xs : list<bool>) =
    match xs with
    | [] -> false
    | x::xs -> 
        x || any xs

let rec all (xs : list<bool>) =
    match xs with
    | [] -> true
    | x::xs -> 
        x && any xs

let rec sum2 (xs : list<int>) =
    match xs with
    | [] -> 0
    | head :: rest -> 
        addF head (sum2 rest)


let rec aggregate f startValue xs =
    match xs with
    | [] -> startValue 
    | x::xs -> f x (aggregate f startValue xs)

let sumUsingAggregate = aggregate (+) 0 
let allUsingAggregate = aggregate (&&) true
let anyusingAggregate = aggregate (||) false

let allWith (f : 'a -> bool) (xs : list<'a>) = 
    let predicate (x : 'a) (s : bool) : bool =
        f x && s
    aggregate predicate true xs