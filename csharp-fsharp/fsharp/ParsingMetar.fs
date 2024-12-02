// ported from https://entropicthoughts.com/parser-combinators-parsing-for-haskell-beginners

module Parsing

open System

let exampleInput = "BIRK 281500Z 09014KT CAVOK M03/M06 Q0980 R13/910195"

// task extract wind information from METAR report (international semi standard format for
// reporting conditions on airpots, such as weather, cloud layers, humidity and such.


type Input = list<char>
type Parser<'a> = { parse : Input -> Option<'a * Input>}

// helpers...
let toCharList (s : string) =
    s.ToCharArray() |> Array.toList
let toString (chars : list<char>) =
    String(List.toArray chars)

// takes a parser, a string input and optionally 
// returns the parse result and the unconsumed input.
let parse (p : Parser<'a>) (input : string) : Option<'a * Input> =
    p.parse (toCharList input)

// creates a parser, which consumes no input and produces the provided value.
let pReturn (a : 'a) : Parser<'a> =
    { parse = fun str -> Some(a,str) }

// creates the ever failing parser
let pFail : Parser<'a> =
    { parse = fun str -> None }


let pItem =
    { parse = fun input -> 
        match input with
            | [] -> None
            | x::xs -> Some (x, xs)
    }

let pSat (f : char -> bool) =
    { parse = fun input -> 
        match input with
            | x::xs -> 
                if f x then Some (x, xs)
                else None
            | [] -> None
    }

let test1 = parse (pSat (fun c -> c = 'a')) "a" // all input consumed
let test2 = parse (pSat (fun c -> c = 'a')) "ab" // ok, b remains
let test3 = parse (pSat (fun c -> c = 'a')) "cc"  // no parse result

// uses p to parse the input. if p returns a result, use the function to create the next
// parser. handle over the remaining input of p to the parser returned by the function.
let pThen (p : Parser<'a>) (f : 'a -> Parser<'b>) : Parser<'b> =
    { parse = fun input ->
        match p.parse input with
        | None -> None
        | Some(a,rest) -> 
            let secondOne = f a
            secondOne.parse rest
    }

let test4 = 
    let parser =
        let a = pSat (fun c -> c = 'a')
        let b = pSat (fun c -> c = 'b')
        pThen a (fun aResult -> b)
    parse parser "ab"

let test5 = 
    let parser =
        let a = pSat (fun c -> c = 'a')
        let b = pSat (fun c -> c = 'b')
        pThen a (fun aResult -> pThen b (fun bResult -> pReturn (aResult,bResult)))
    parse parser "ab"

// tries to parse the input using l, if this does not work it tries out the second one (r).
let pChoice (l : Parser<'a>) (r : Parser<'a>) : Parser<'a> =
    { parse = fun input ->
        match l.parse input with // try l
        | None -> r.parse input // try r
        | Some(a,rest) -> Some(a,rest) // ok l worked
    }

let (<|>) l r = pChoice l r

let test6 =
   let a = pSat (fun c -> c = 'a')
   let b = pSat (fun c -> c = 'b')
   let aOrB = a <|> b
   parse aOrB "b"


type ParserBuilder() =
    member x.Bind(s : Parser<'a>, f : 'a -> Parser<'b>) : Parser<'b> =
        pThen s f

    member x.Return (s : 'a) : Parser<'a> = pReturn s

let parser = ParserBuilder()

let test7 =
    let a = pSat (fun c -> c = 'a')
    let b = pSat (fun c -> c = 'b')
    let p = 
        parser {
            let! aResult = a // calls bind on parser, which calls thenP
            let! bResult = b // same
            return (aResult,bResult) // calls return on parser, which then calls pReturn
        }
    parse p "ab"

let rec parseSeq (parsers : list<Parser<'a>>) : Parser<list<'a>> =
    parser {
        match parsers with
            | [] -> return []
            | x::xs -> 
                let! px = x
                let! rest = parseSeq xs
                return px::rest
    }

let pSpace = pSat (fun c -> c = ' ')
let pZ = pSat (fun c -> c = 'z')
let parseSpaceThenA =
    parser {
        let! _ = pSpace
        let! c = pZ
        return c
    }

let (.>>) (a : Parser<'a>) (b : Parser<'b>) : Parser<'a> =
    pThen a (fun aResult -> pThen b (fun _ -> pReturn aResult))

let (<<.) (a : Parser<'a>) (b : Parser<'b>) : Parser<'a> =
    pThen a (fun _ -> pThen b (fun bResult -> pReturn bResult))

let test2f (a : Parser<'a>) (b : Parser<'b>) : Parser<'a> =
    parser {
        let! a = a
        let! b = b
        return a
    }
let pA = pSat (fun c -> c = 'a')
let pB = pSat (fun c -> c = 'b')
let test10 = parseSeq [pA;pB;pA]

let test8 =
    let a = pSat (fun c -> c = 'a')
    let b = pSat (fun c -> c = 'b')
    let p = 
        // let! aResult = a
        parser.Bind(a, fun aResult ->
            // let! bResult = b // same
            parser.Bind(b, fun bResult ->
                // return (a,b)
                parser.Return (a,b)
            )
        )
    parse p "ab"



let sym (cs : string) =
    let rec doIt xs =
        match xs with
        | [] -> pReturn ""
        | c::cs ->
            parser {
               let! c = pSat (fun x -> x = c)
               let! ls = doIt cs
               return sprintf "%c%s" c ls
            }
    doIt (toCharList cs)


        
let rec pMany1 (p : Parser<'a>) : Parser<list<'a>> =
    parser {
        let! x = p
        let! rest = pMany p
        return x::rest
    }
and pMany (p : Parser<'a>) = pMany1 p <|> pReturn []

