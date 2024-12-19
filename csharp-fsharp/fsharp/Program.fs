open System
open Basics

let result = Some "jsdf"
match result with
| None -> printfn "no value"
| Some theValue ->  
    let theResult = result
    printfn $"the value was: {theValue}"

[1;2;3] |> List.iter (fun v -> Currying.print ConsoleColor.Red v)

// this varaint is idiomatic
[1;2;3] |> List.iter (Currying.print ConsoleColor.Black)




Lecture1.CountLines.runInSourceDirectory()

