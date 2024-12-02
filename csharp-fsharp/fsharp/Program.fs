open System
open Basics

[1;2;3] |> List.iter (fun v -> Currying.print ConsoleColor.Red v)

// this varaint is idiomatic
[1;2;3] |> List.iter (Currying.print ConsoleColor.Black)




Lecture1.CountLines.runInSourceDirectory()

