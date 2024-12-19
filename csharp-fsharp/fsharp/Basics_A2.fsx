module Basics_A

let one = 1

// function definition via lambda
// one is captured by the lambda
// often called closure
// in c#: Func<int,int> addOne = (x) => x + one;
let addOne = fun x -> x + one

// question by participant? how about action?
// Action<int> print = (x) => Console.WriteLine(x);
let print = fun (x : int) -> System.Console.WriteLine(x)
let ignore (x : int) = ()
// Task<unit> // Task

// participant: but not that often used in practise since unit only useful in inpure function

// function definition
let addOne' x = x + 1

type MyOption<'a> = 
    | MyNone
    | MySome of 'a

let a = MySome 1
let b = MyNone

// always inspect values using pattern matching
let prettyPrint a =
    match a with
    | Some a -> $"was {a}"
    | None -> "had no value"

// expression vs statement
let prettyPrint2 a =
    match a with
    | Some 1 -> $"was {a}" // deep inspection
    | None -> "had no value"

// exrpession vs statement: 
// - pattern matching is expression
// - exhaustiveness checks

// discrimanted unions can be used to model domains
type Customer = 
    | PrivateCustomer of name : string 
    | CorporateCustomer of uuid : string * firma : string


(*
c#/java/c++
interface Customer {}
class PrivateCustomer : Customer {}
class CorporateCustomer : Customer {}

*)

// can parts of union type be reused?
// yes
type CorporateCustomer = { uuid : string; firma : string }
type Customer2 = 
    | PrivateCustomer2 of name : string 
    | CorporateCustomer2 of CorporateCustomer



let privateCustomer = PrivateCustomer "harald"

// customer names are only defined for private customers -> thus an option type can be used
let getCustomerName (c : Customer) : Option<string> =
    match c with
    | PrivateCustomer name -> Some name
    | CorporateCustomer(uuid,firma) -> None

let customers = [ PrivateCustomer "harald"; CorporateCustomer("AT22", "my Company GmbH") ]


let parseInt (s : string) =
    match System.Int32.TryParse(s) with
    | (true, v) -> Some v
    | _ -> None


let myMap f optionalValue =
    match optionalValue with
    | None -> None
    | Some theValue -> 
        let result = f theValue
        Some result

let addOne2 x = x + 1

let parseStringAddOne (s : string) =
    s 
    |> parseInt 
    |> Option.map addOne2
    |> Option.map (fun x -> $"the value is{x}")

let mapOption f optionalValue =
    match optionalValue with
    | None -> None
    | Some v -> Some (f v)


// in practise, for particular problems

type DbResult<'a> =  
    | Ok of 'a
    | NotAuthorized
    | InvalidOperation

module DbResult =
    let map (f : 'a -> 'b) (dbResult : DbResult<'a>) : DbResult<'b> = 
        match dbResult with
        | NotAuthorized -> NotAuthorized
        | InvalidOperation -> InvalidOperation
        | Ok v -> f v |> Ok

// input and output are generic, this allows to transform datatypes
let plusOneToString (x : Option<int>) =
    x |> Option.map (fun x -> x + 1) |> Option.map (fun x -> (string x, x))

// higher order functions to remove code duplication
// takes a function as input or returns a function
// we say: first class functions when functions can appear at any position
// either return type or paramer

// var tuple = Tuple.Create(1,2)
let tuple = (1,Some 2) // construction

let (firstValue,secondValue) = tuple

let xorFunction (a : bool) (b : bool) =
    match (a,b) with
    | (true, true) -> false
    | (true, false) -> true
    | (false, false) -> false
    | (false, true) -> true

type CorporateCustomer3 = { uuid : string; firma : string }
let customer1 = { uuid = "ATU.."; firma = "my company" }

let renameCustomerTo (c : CorporateCustomer3) (newName : string) =
    { c with firma = newName }
    // shorthand for
    // { uuid = c.uuid; firma = newName }
    // is called record update syntax

let tup = (1,2)
let flip (a,b) = (b,a)

// participant: how to change second
let secondToThree (a,b) = (a,3)

let tupMany = (1,2,44,5,765,9)


let add a b = a + b
let add2 = fun a -> (fun b -> a + b)
// curried function

let addDifferentlyDefined (a,b) = a + b
// tupled

// curried functions can by partially applied

List.map (fun a -> a + 1) [1;2;3]
List.map (fun a -> add 1 a) [1;2;3]
// partial application to make code shorter
List.map (add 1) [1;2;3]

// same for option
Option.map (add 1) (Some 1)



type Person = { friends : list<string> }

let queryDb (id : int) : Option<Person> =
    if id = 0 then 
        Some { friends = ["harald"; "fred"] }
    else
        None 


let countFriends (p : Person) : int =
    p.friends.Length

// another pipleine which helps us dealing with missing values cleanly
let queryAndCountFriends (id : int) =
    id 
    |> queryDb 
    |> mapOption countFriends 
    |> mapOption (fun s -> string s) // |> and so on...

// is this now an impure function?
// what is purity, same arguments => same result
// && does not change system date (e.g. writing to database)