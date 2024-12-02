let sumEvens (xs : list<int>) = 
    xs |> List.filter (fun x -> x % 2 = 0) |> List.sum

open System.Linq

let sumEvensCSharpStyle (xs : list<int>) = 
    xs.Where(fun x -> x % 2 = 0).Sum()

// Func<int,int> negate = x => -x;
// Func<int,int> negage = x => { return -x; };
let negateAsLambda = fun x -> -x

// int Negate(int x) { return -x; };
let negate x = -x

let one = 1

let plusOne x = x + one

plusOne 1
let isThree = (one = 3)
plusOne 1

// one cannot be mutated
//one <- 6

// not types are default
// types can be annotated
let plusOne'' (x : int) : int = x + 1


let add : int -> (int -> int) = (fun x -> (fun y -> x + y))
let someValue : int -> int = add 1

// equivalent to 32
let addBeautiful x y = 
    x + y

// function signature of addBeautiful is: int -> (int -> int)

let anAddtion = (addBeautiful(10))(10)

let someBeautifulValue = addBeautiful 1
someBeautifulValue 10

// can this be done in C#?
//Func<int, Func<int, int>> curriedFunction = x => (y => x + y);
//Func<int, int> partialApplication = curriedFunction(2);
//var result = partialApplication(4);


type MyOption<'a> = 
    | None // constructor
    | Some of 'a // constructors

let noValue = None
let someInt = Some(10)

// 
// automatic generalization happens here

let foo theValue =   
    match theValue with
    | None -> "no value here"
    | Some i -> string i

foo (Some "Jdjf")


type Object = 
    | Car of string
    | Keyboard

let o = Car "jsdfj"

type UUID = string

type Customer =
    | PrivateCustomer of string
    | CooperateCustomer of UUID

let test v =
    match v with
    | Some 1 -> "one"
    | Some 2 -> "two"
    | Some x -> "anoter number"
    | None -> "no value at all"