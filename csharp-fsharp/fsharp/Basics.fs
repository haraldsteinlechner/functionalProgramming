namespace Basics

module Currying =
    open System

    let print (color : ConsoleColor) value =
        let oldColor = Console.ForegroundColor
        Console.ForegroundColor <- color
        Console.WriteLine(string value)
        Console.ForegroundColor <- oldColor




