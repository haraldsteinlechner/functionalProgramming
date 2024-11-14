
// Ex1
let rec rev xs =
    match xs with
    | [] -> []
    | h::t -> (rev t)@[h]

let rec revv xs acc =
    match xs with 
    | [] -> acc
    | h::t -> revv t (h::acc)

let rec equalBy f xs ys = 
    match xs, ys with 
    | x::rx, y::ry -> (f x y) && equalBy f rx ry
    | [],[] -> true
    | _ -> false

let isPalindrom f xs = equalBy f xs (List.rev xs)

let mapi = List.mapi

let cmpInsensitive a b = 
    System.Char.ToLower a = System.Char.ToLower b
isPalindrom cmpInsensitive ['a';'B';'A'] 