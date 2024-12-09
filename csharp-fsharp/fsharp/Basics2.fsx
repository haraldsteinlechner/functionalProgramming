module Lecture_14_12

let two = 2

let timesTwo x = two * x
let timesTwoAsLambda = fun x -> two * x

// interesting: two is adressable. why?
// two is in the closure of the lambda function
// this allows lambda functions as first class citizens
// (means they can be stored in variables, given into functions and returned from function)

// but how to deal with non-existing values?
// explicitly model non-existent values
type MyOption<'a> =
    | NoValueHere
    | TheValue of 'a

// by using a discrimnated union, non-existence of values can be modelled explicitly
// this one is also defined in the base and called option with None and Some constructors
(* this one is defined in the standardlib
type Option<'a> = 
    | None
    | Some of 'a
*)

// discrimanted unions can be used to model domains
type Customer = 
    | PrivateCustomer of name : string 
    | CorporateCustomer of uuid : string * firma : string

let privateCustomer = PrivateCustomer "harald"

// customer names are only defined for private customers -> thus an option type can be used
let getCustomerName (c : Customer) : Option<string> =
    match c with
    | PrivateCustomer name -> Some name
    | CorporateCustomer(uuid,firma) -> None

let customers = [ PrivateCustomer "harald"; CorporateCustomer("AT22", "my Company GmbH") ]


// MOB programming - mapping function for option.
// map (fun x -> x + 1) None ==> None
// map (fun x -> x + 1) (Some 1) ==> Some 2
let mapInt (f : int -> int) (input : Option<int>) : Option<int> =
    match input with
    | None -> None
    | Some itHasAValue -> 
        let result = f itHasAValue
        Some result

// Optional<T> MapOption<T>(Func<T, T> f, Optional<T> input)
// (T -> R) -> Option<T> -> Option<R>
let mapOption (f : 'a -> 'b) (input : Option<'a>) : Option<'b>  =
    match input with
    | None -> None
    | Some itHasAValue -> 
        let result = f itHasAValue
        Some result


let parseInt (str : string) : Option<int> =
    match System.Int32.TryParse(str) with
    | (true, v) -> Some v
    | _ -> None

// this allows to create computational pipelies which handle absens of values
// without explicitly dealing with it
let parseAddAndToString (str : string) =
    str 
    |> parseInt
    |> mapOption (fun v -> v + 1) 
    |> mapOption (fun s -> string s)

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



type User = { id : string; email : Option<string> }
type UserId = string

let parseQuery (q : string) : Option<UserId> =
    failwith "... implementation omitted"

let queryUser (id : UserId) : Option<User> =
    failwith "... implementation omitted"

let getContactAddress (user : User) : Option<string> = 
    failwith "... implementation omitted"


let getContactAdress (query : string) =
    match parseQuery query with
    | Some uid -> 
        match queryUser uid with
        | Some u -> 
            match getContactAddress u with
            | Some address -> Some address
            | None -> None
        | None -> None
    | None -> None


let (|>>) (input : Option<'a>) (f : 'a -> Option<'b>)  : Option<'b> =
    match input with
    | None -> None
    | Some v -> f v

let getContactAdress2 (query : string) =
    Some query 
    |>> parseQuery  // chain
    |>> queryUser   // together functions
    |>> getContactAddress // which might fail

// but wait, we have no idea what went wrong. we just know the result was none
// again, domain modeling and discriminated unions to the rescue -> 
// let us define a type which precisely captures what could go wrong in our 
// specific use case!

type QueryResult<'a> =
    | Ok of 'a
    | DatabaseError of string
    | ParseError of string


// let us continue with product types 

type Rect = { width : int; height : int }

// f# automatically derives the type?
let rect = { width = 10; height = 20 }

// access by using .
let width = rect.width


// let us try to create a linked list, just by using the "discriminated union" 
// concept... 
type LinkedListNode<'a> =
    | Node of 'a * LinkedListNode<'a>
    | EndNode

let testList = Node(1, Node(2, Node(3, EndNode)))

// can we write useful functions 
// (remember, sum was one of the building blocks for the first exercise)

let rec mySum (l : LinkedListNode<int>) =
    match l with
    | EndNode -> 0
    | Node(v, restOfTheList) -> 
        v + mySum restOfTheList

// let us test it
mySum testList

(*
type LinkedListNode<'a> =
    | Node of 'a * LinkedListNode<'a>        :: operator
    | EndNode                                [] 

*)

let testListOld = Node(1, Node(2, Node(3, EndNode)))
let betterList = 1 :: 2 :: 3 :: []
let evenBetter = [1;2;3]

let rec mySum2 (l : list<int>) =
    match l with
    | [] -> 0
    | v :: restOfTheList -> 
        v + mySum2 restOfTheList

// this is how we use it:
mySum2 [1;2;3]


// we are looking for a function which looks like this:
// IEnumerable<T> Filter<T>(Func<T, bool> predicate, this IEnumerable<T> input)
// filter :: ('a -> bool) -> list<'a> -> list<'a>
let rec filter (predicate : 'a -> bool) (input : list<'a>) : list<'a> =
    match input with
    | [] -> []
    | firstElement :: restOfTheList ->
        if predicate firstElement then 
            firstElement :: filter predicate restOfTheList
        else
            filter predicate restOfTheList

// wow we can now already filter lists, just by using discrimnated unions
// recursion, pattern matching - and that's it
let evens = filter (fun x -> x % 2 = 0) [1..20]