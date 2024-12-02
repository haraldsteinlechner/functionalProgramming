// this code is based on this blog post: https://entropicthoughts.com/parser-combinators-parsing-for-haskell-beginners

open System

let exampleInput = "BIRK 281500Z 09014KT CAVOK M03/M06 Q0980 R13/910195"

// task extract wind information from METAR report (international semi standard format for
// reporting conditions on airpots, such as weather, cloud layers, humidity and such.
// approach: split strings on keywords which we know.
// we could split the string by empty spaces, then we arrive at the third position we would
// like to parse.

module AttemptUsingPlainParsing =

    let pMetar1 (metar : string) =
        let manyThings = metar.Split(' ')
        let windPart = manyThings.[2]
        // now parse the wind part. how can we do it.....
        failwith "not implemented, let us build utility functions.."

    // subtask: parse "09014KT"
    let pWindSpeed1 (metarWind : string) = 
        // remove prefix
        let knotsString = metarWind.Substring(3)
        // remove knots word
        printfn "%A" knotsString
        let knots = knotsString.Substring(0,knotsString.Length - 2)
        System.Int32.Parse knots

    let testWindSpeed1 = pWindSpeed1 "09014KT"

    // unfortunately sometimes wind speed is in m/s
    
    //let testWindSpeed2 = pWindSpeed1 "09007MPS" 


module Attempt2UsingRegex =
    // ok let us try it with regexes
    let windSpeed2 (metarWind : string) =
        let regex = System.Text.RegularExpressions.Regex("[0-9]{3}([0-9]{2,3})(KT|MPS)")
        let result = regex.Match metarWind
        System.Int32.Parse result.Groups.[1].Value

    // ok
    let testWindSpeed1 = windSpeed2 "09014KT"
    let testWindSpeed2 = windSpeed2 "09007MPS"


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

// let us start with parser combinators. 
// from the beginning our string looks like: "BIRK 281500Z...
// the first is the airport short form. it consists of upper case characters
let isUpper (c : char) = 
    c >= 'A' && c <= 'Z'


let pUpper = pSat isUpper

let test9 = parse pUpper "a"
let test10 = parse pUpper "A"


let pUpperCaseList =
    let pUpperCase = pSat isUpper
    pMany1 pUpperCase

// we just made a parser for parsing upper case characters into a list of charaters. we would like to
// have a parser which creates a string instead of a list of characters

let mapParser (p : Parser<'a>) (f : 'a -> 'b) : Parser<'b> =
    parser {
        let! p = p
        return f p
    }

let pAirport : Parser<string> =
    mapParser pUpperCaseList (fun listOfChars -> String(Array.ofList listOfChars)) 

let testAirport1 = parse pAirport "BIRK"

//reconsider our input... "BIRK 281500Z 09014KT CAVOK M03/M06 Q0980 R13/910195"
// next we need to parse a time and date stamp. the format is
// <day of month><hours><minutes>Z
// ok, we need to parse 2 digit numbers......
// let us start of with parsing a single digit

let isDigit (c : char) =
    c >= '0' && c <= '9'


let pDigit = pSat isDigit

let test11 = parse pDigit "2"
let test12 = parse pDigit "a"

let pDigit2 =
    parser {
        let! d1 = pDigit
        let! d2 = pDigit 
        return [d1;d2]
    }

let test13 = parse pDigit2 "23"

let toInt (l : list<char>) = List.fold (fun s v -> (s * 10 + (int v - int '0'))) 0 l
let a = toInt ['1';'2'];; // ~> 12

let pDigit2Int = mapParser pDigit2 toInt

let test14 = parse pDigit2Int "24" 

type Timestamp = { day : int; hour : int; minute : int }

let pTimestamp =
    parser {
        let! day = pDigit2Int
        let! hour = pDigit2Int
        let! minute = pDigit2Int
        let! delimiter = pSat (fun c -> c = 'Z')
        return { day = day; hour = hour; minute = minute}
    }

let testTimestamp1 = parse  pTimestamp "888990Z"
let testTimestamp2 = parse pTimestamp "302359Z"
let testTimestamp3 = parse pTimestamp "888990Z"

let rec pRepeat (c : int) (p : Parser<'a>) : Parser<list<'a>> =
    parser {
        if c = 0 then return []
        else
            let! current = p
            let! rest = pRepeat (c-1) p
            return current :: rest
    }

let test15 = parse (pRepeat 2 pDigit) "23" //  Some (['2'; '3'], [])
let test16 = parse (pRepeat 2 pDigit) "2" // not sufficient input: None
let test17 = parse (pRepeat 2 pDigit) "333" //  Some (['3'; '3'], ['3'])
let test18 = parse (pRepeat 3 pDigit) "333" // Some (['3'; '3'; '3'], [])

let pNumber (digits : int) =
    mapParser (pRepeat digits pDigit) toInt

let test19 = parse (pNumber 2) "12"
let test20 = parse (pNumber 1) "2"

type WindInfo = { direction : int; speed : int; unit : string }


let pWindInfo1 =
    parser {
        let! direction = pNumber 3
        let! speed = pNumber 3 <|> pNumber 2
        let! unit = sym "KT" <|> sym "MPS"
        return { direction = direction; speed = speed; unit = unit }
    }

let test21 = parse pWindInfo1 "09014KT"

let pChar c = pSat (fun a -> c = a)

let pMetar1 =
    parser {
        let! airport = pAirport
        let! _ = pChar ' '
        let! time = pTimestamp
        let! _ = pChar ' '
        let! wind = pWindInfo1
        return airport,time,wind
    }

let testMetar = parse pMetar1 "BIRK 281500Z 09014KT"

//module Attempt3UsingParserCombinators =
//
//    // same problems, enable baked in parsing code. can we do better?
//
//    // let us start with parser combinators. 
//    // from the beginning our string looks like: "BIRK 281500Z...
//    // the first is the airport short form. it consists of upper case characters
//    let isUpper (c : char) = 
//        c >= 'A' && c <= 'Z'
//
//    let pUpperCaseList =
//        let pUpperCase = sat isUpper
//        many1 pUpperCase
//
//    // we just made a parser for parsing upper case characters into a list of charaters. we would like to
//    // have a parser which creates a string instead of a list of charasters
//
//    let map (p : Parser<'a>) (f : 'a -> 'b) : Parser<'b> =
//        parser {
//            let! p = p
//            return f p
//        }
//
//    let pAirport : Parser<string> =
//        map pUpperCaseList (fun listOfChars -> String(Array.ofList listOfChars)) 
//
//    let testAirport1 = pAirport "BIRK"
//
//    // reconsider our input... "BIRK 281500Z 09014KT CAVOK M03/M06 Q0980 R13/910195"
//    // next we need to parse a time and date stamp. the format is
//    // <day of month><hours><minutes>Z
//    // ok, we need to parse 2 digit numbers......
//    // let us start of with parsing a single digit
//
//    let isDigit (c : char) =
//        c >= '0' && c <= '9'
//
//    let rec pRepeat (c : int) (p : Parser<'a>) : Parser<list<'a>> =
//        parser {
//            if c = 0 then return []
//            else
//                let! current = p
//                let! rest = pRepeat (c-1) p
//                return current :: rest
//        }
//
//    let pDigit = sat isDigit
//
//
//    let testDay = pRepeat 2 pDigit
//    let testDay1 = testDay "23"
//    let testDay2 = testDay "1"
//    let testDay3 = testDay "123"
//
//    // instead of parsing list of charactesr we would like to map those to natural numbers. we can do this by using map
//
//    let rec toIntAcc (acc : int) (d : list<char>) =
//        match d with
//            | [] -> acc
//            | h :: rest ->
//                let hv = int h - int '0'
//                toIntAcc (acc * 10 + hv) rest
//
//    let rec toInt (d : list<char>) =
//        toIntAcc 0 d
//
//    let pNumber (c : int) = 
//        let pChars = pRepeat c pDigit
//        map pChars toInt
//
//    let pTimestamp =
//        parser {
//            let! day = pNumber 2 
//            let! hour = pNumber 2 
//            let! minute = pNumber 2 
//            let! delimiter = sym "Z"
//            return day,hour,minute
//        }
//
//    let testTimestamp1 = pTimestamp "888990Z"
//    let testTimestamp2 = pTimestamp "302359Z"
//    let testTimestamp3 = pTimestamp "888990Z"
//
//    // here we could validate the parsed timestamp (e.g. minute < 59 etc) but let us continue with
//    // Wind information comes in three parts: the first three digits are the wind direction, in degrees. 
//    // The next two or three digits are the wind speed. 
//    // This is followed by either "KT" or "MPS" which signify that the wind speed 
//    // was given in either knots or metres per second.
//    // 09014KT
//    // this means: 090 = direction, 14 knots
//    // we need a way to talk about or......
//
//    let twoOrThree =
//        let two = pRepeat 2 pDigit
//        let three = pRepeat 3 pDigit
//        two <|> three 
//
//    // we are set
//    let pWindInfo1 =
//        parser {
//            let! direction = pNumber 3
//            let! speed = pNumber 2 <|> pNumber 3
//            let! unit = sym "Z" <|> sym "MPS"
//            return (direction,speed,unit) 
//        }
//
//    // we would like to always work in meters per second, roughly they can be converted as such
//    let toMPS (unit : string) (value : int) =
//        match unit with
//            | "KT" -> value / 2
//            | "MPS" -> value
//            | _ -> failwith "for now we throw an exception here, there are much nices methods to do this, stay tuned"
//
//    let pWindInfo2 =
//        parser {
//            let! direction = pNumber 3
//            let! speed = pNumber 3 <|> pNumber 2
//            let! unit = sym "KT" <|> sym "MPS"
//            let! space = char ' '
//            return (direction,toMPS unit speed) 
//        }
//
//    let pUnit = symbol "KT" <|> symbol "MPS"
//
//    let testWind1 = pWindInfo2 "09014KT"
//
//    let pGust : Parser<int> =
//        parser {
//            let! g = sym "G"
//            let! gustSpeed = pNumber 3 <|> pNumber 2
//            return gustSpeed
//        }
//
//    let testGust = pGust "G12"
//
//    let pOption p =
//        choicep (map p Some) (returnp None)
//
//    type WindInfo = { direction : int; speed : int; gusts : Option<int>}
//
//    let pWindInfo =
//        parser {
//            let! direction = pNumber 3
//            let! speed = pNumber 3 <|> pNumber 2
//            let! gusts = pOption pGust 
//            let! unit = sym "KT" <|> sym "MPS"
//            let! space = char ' '
//            return {
//                direction = direction;
//                speed = toMPS unit speed
//                gusts = Option.map (toMPS unit) gusts
//            }
//        }
//
//    let testWind3 = pWindInfo "09014G17KT "
//
//    let pMetar =
//        parser {
//            let! airport = pAirport
//            let! _ = char ' '
//            let! time = pTimestamp
//            let! _ = char ' '
//            let! wind = pWindInfo
//            return airport,time,wind
//        }
//
//    let testMetar = pMetar "BIRK 281500Z 09014G17KT CAVOK M03/M06 Q0980 R13/910195"

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code