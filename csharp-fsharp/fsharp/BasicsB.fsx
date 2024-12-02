(* content:
- immutable variable bindings
- type inference system (including automatic generalization)
- type annotations
- scoping rules
- curried functions
- partial evaluations
- union types
- dealing with optional values
*)


// how does F# code look like?
let evenNumbersSum (xs : list<int>) =
    xs |> List.filter (fun x -> x % 2 = 0) |> List.sum

// |> is pipe operator, good for creating data pipelines

// f# runs in dotnet environent => can use C# libs (e.g. linq)
open System.Linq

// same as above but written in extension method chaining syntax
// as opposed to piping
let evenNumbersSum2 (xs : list<int>) =
    xs.Where(fun x -> x % 2 = 0).Sum()


// variables introduced by 'let'
let one = 1
// implicitly typed, but can be specified (var : type) form
let aString : string = "hello world"

// wie ist das scoping?
// statical lexical scoping: alles sieht seinen containg scope

// C#: Func<int,int> negate = x => -x;
let negate = fun x -> -x

// same as above
let negateAsTopLevelFunction x = -x

// body can be written in second line. no {} braces but intendation
let looksAbitLikePython x = 
    -x
// but with types

let two = 2
let addTwo x = x + two
let addTwoWithLambda = fun x -> x + two

// functions can genrate functions => leads to curried functions
let add : int -> (int -> int)= 
    fun x -> 
        (fun y -> x + y)

let addOne = add 2 3

// types signatures bind right associatively
// (int -> int) -> int

// function application binds left associtavely
let addOne2 = (add 2)(3) // equivalent to add 2 3

let addTwo2 = add 2


// partial application can be used to create "specialized functions"
// they can be used to reduce syntactic clutter introduced by lambda functions everywhere

(*
does this work in c#
Func<int, Func<int, int>> curriedFunction =
    x => (y => x + y);

Func<int,int> partialApplication = curriedFunction(5);
var result2= partialApplication(5);

var result = curriedFunction(5)(5);
*)

(*
let add : int -> (int -> int)= 
    fun x -> 
        (fun y -> x + y)
*)
// special syntax for curried functions:
let addWrittenAsTopLevelFunction x y = x + y
let partialAppliction5 = addWrittenAsTopLevelFunction 3



let list = [1 ; 2; 3]

let mappedList = List.map (fun x -> x + 1) list

// use partial application here
let mappedList2 = List.map (add 1) list


type MyOption<'a> = 
    | None
    | Some of 'a

(*
interface MyOption<T> {}
class None<T> : MyOption<T> {}
class Some<T> : MyOption<T>{
    public Some(T v) {
        V = v;
    }
}
*)

type CustomerId = string
type UUID = string
type Customer =
    | PrivateCustomer of CustomerId * string
    | CooperateCustomer of UUID

let p = PrivateCustomer("id", "harald")
let c = CooperateCustomer("....")


type MyOption2<'a> = 
    | None
    | Some of 'a


let noValue = None
let aString2 = Some "the string"
let anInt = Some 2

let addOption x =
    match x with
    | Some 0 -> Some 1
    | Some 1 -> Some 2
    | Some theValue -> Some (theValue + 1)
    | None -> None

let parseInt (s : string) =
    match System.Int32.TryParse(s) with
    | (true, v) -> Some v
    | _ -> None

let optionToString v = 
    match v with
    | None -> "no value"
    | Some v -> $"value is {v}"

let parseThenIncrement (s : string) =
    match parseInt s with
    | Some v -> Some (v + 1)
    | None -> None

let mapOption f o =
    match o with
    | None -> None
    | Some value -> Some (f value)

let parseAndIncrement (s : string) =
    mapOption (add 1) (parseInt s)


let parseAndIncrementAsPipeline (s : string) =
    s |> parseInt |> mapOption (add 1) |> mapOption string