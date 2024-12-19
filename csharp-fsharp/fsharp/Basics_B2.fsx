
// immutalbe binding, type inference
let one = 1

// type inference also for functions
let add a b = a + b

// int -> int -> int
// type signatures 

let add2 = fun a -> (fun b -> a + b)

// curried function
let addOne = add 1

let curriedFunctionsInAction =
    List.map (fun x -> x + 1) [1;2;3]
    // unnecessarily complex
    let l1 = List.map (fun x -> add 1 x) [1;2;3]
    // refactoring for less complex code
    let l2 = List.map (add 1) [1;2;3]
    // l2 is same as l1 but more elegant
    // (add 1) = partial application
    l2

// scoping
let add' a = a + one
// add' captures the variable one
// "one is in the closure"

// captures the value or the reference?
// c# captures by reference
// java captures by reference but disallows mutablity of cpatures


// Func<int,int> f = (x) => x + 1;
// Func<int,void> does not work!
// Action<int> == Func<int,void>
let print (x : int) =
    System.Console.WriteLine(x)
   
// automatical generalization of functions
let ignore x = ()

let unitList : list<unit> = List.map ignore [1;2;]
unitList |> List.length

type MyOption<'a> =
    | MyNone
    | MySome of 'a


let optionalString = MySome "some string"
let optionalInt = MySome 2

type Customer = 
    | PrivateCustomer of name : string 
    | CorporateCustomer of uuid : string * firma : string

(*
interface Customer {}
class PrivateCustomer : Customer {}
class CorporateCustomer : Customer {}

*)

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

let parseIntAndAddOneAndtoString2 (s : string) =
    s |> parseInt |> Option.map (add 1) |> Option.map string


let parseIntAndAddOneAndtoString3 (s : string) =
    match s |> parseInt |> Option.map (add 1) with
    | None -> "no value"
    | Some x -> string x

let parseIntAndAddOneAndtoString4 (s : string) =
    s |> parseInt |> Option.map (add 1) |> Option.map string |> Option.defaultValue "no value"

type AccessTokenFailure = 
    | Expired
    | Invalid

type DbResult<'a> = 
    | Ok of 'a
    | NotAuthorized
    | InvalidOperation of AccessTokenFailure

module DbResult =
    let map (f : 'a -> 'b) (dbResult : DbResult<'a>) : DbResult<'b> = 
        match dbResult with
        | NotAuthorized -> NotAuthorized
        | InvalidOperation s -> InvalidOperation s
        | Ok v -> f v |> Ok

// type is: int * int
let tuple = (1,2)

let tuple2 = (1, Some "js") // right hand side is construction

let (a,b) = tuple

let (a2,b2) = (1,2)
let (_,_,c,d) = (1,2,4,4)


let booleanOr (a : bool) (b : bool) =
    match (a,b) with
    | (true, _) -> true
    | (_, true) -> true
    | (false, _) -> false

let booleanAnd (a : bool) (b : bool) =
    match (a,b) with
    | (true, true) -> true
    | _ -> false
    
type CorporateCustomer3 = { uuid : string; firma : string }

type Customer2 = 
    | PrivateCustomer of name : string 
    | CorporateCustomer of CorporateCustomer3

let customer1 = { uuid = "ATU.."; firma = "my company" }

module Customer =
    
    let renameCustomer (customer : CorporateCustomer3) (newName : string) =
        { uuid = customer.uuid; firma = newName }

    let renameCustomer2 (customer : CorporateCustomer3) (newName : string) =
        { customer with firma = newName } 


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
    |> Option.map countFriends 
    |> Option.map string // |> and so on...


let countFriends2 (p : Person) : Option<int> =
    Some p.friends.Length


// another pipleine which helps us dealing with missing values cleanly
let queryAndCountFriends2 (id : int) =
    id 
    |> queryDb 
    |> Option.bind countFriends2
    |> Option.map (fun (count : int) -> string count) // |> and so on...