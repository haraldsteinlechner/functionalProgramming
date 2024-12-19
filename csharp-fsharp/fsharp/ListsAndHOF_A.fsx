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

type IntList = 
    | Empty
    | Cons of int * IntList

let oneToThree2 = Cons(1,Cons(2, Cons(3, Empty)))

type MyList<'a> = 
    | Empty
    | Cons of 'a * MyList<'a>

let oneToThreeChar = Cons('1',Cons('2', Cons('3', Empty)))
let oneToThree = Cons(1,Cons(2, Cons(3, Empty)))

//let oneToThree = Cons(1,Cons(2, Cons(3, Empty)))

let listInBaseLib = 1 :: 2 :: 3 :: []
let listSyntax = [1;2;3]

let rec filter (predicate : 'a -> bool) (elements : MyList<'a>) : MyList<'a> =
    match elements with
    | Empty -> Empty
    | Cons(head,rest) -> 
        if predicate head then
            Cons(head, filter predicate rest) 
        else    
            filter predicate rest

let larger1 = filter (fun x -> x > 1) oneToThree
let largerTwo = filter (fun x -> x > 2) oneToThree


let sumOdds = [1..30] |> List.filter (fun x -> x % 2 = 1) |> List.sum


let rec sum (xs : MyList<int>) : int =
    match xs with
    | Empty -> 0
    | Cons(head, rest) ->
        head + sum rest


sum (Cons(1,Cons(2, Cons(3, Empty))))

let rec sumBaseLib (xs : list<int>) : int =
    match xs with
    | [] -> 0
    | head :: rest ->
        head + sumBaseLib rest

let rec map f xs =
    match xs with
    | [] -> []
    | h :: rest ->
        f h :: map f rest

let add x y = x + y
let rec sum2 (xs : list<int>) : int =
    match xs with
    | [] -> 0
    | head :: rest ->
        add head (sum2 rest)

let rec anyTrue (xs : list<bool>) : bool =
    match xs with
    | [] -> false
    | head :: rest -> 
        head || anyTrue rest

let rec all (xs : list<bool>) : bool =
    match xs with
    | [] -> true
    | head :: rest -> 
        head && all rest

let allTrue = List.fold (&&) true
let anyTrue2 = List.fold (||) false
let sum23 = List.fold (+) 0

let rec reduce aggregration startValue xs =
    match xs with
    | [] -> startValue
    | x::rest -> aggregration x (reduce aggregration startValue rest)

        
let addToFront (x : 'a) (xs : list<'a>) : list<'a> =
    x :: xs

let oldList = [2;3]
let newList = addToFront 1 oldList