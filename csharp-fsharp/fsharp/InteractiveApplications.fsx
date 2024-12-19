

// the entire application state
type ApplicationState = { counter : int }
type ApplicationInput =
    | Increment
    | Decrement
    | NoOperation
   

module ApplicationLogic =
    
    let updateState (state : ApplicationState) (input : ApplicationInput) : ApplicationState =
        match input with
        | Increment -> { state with counter = state.counter + 1 }
        | Decrement -> { state with counter = state.counter - 1 }
        | NoOperation -> state

module IO =
    let readInput () : ApplicationInput =
        match System.Console.ReadLine().ToLower() with
        | "incr" -> Increment
        | "decr" -> Decrement
        | _ -> NoOperation // must be handled in practice

    let showApplicationSate (state : ApplicationState) =
        printfn "%A" state

// impure application loop
let applicationLoop () =
    let mutable state = { counter = 0 }
    printfn "Initial State: %A, valid operations are incr | decr" state
    while true do
        // IO, read input from Keyboard
        let input = IO.readInput()
        let newState = ApplicationLogic.updateState state input 
        state <- newState
        IO.showApplicationSate state

// the loop can also be written using recursion (without mutable)
let rec applicationLoop' (s : ApplicationState) =
    let input = IO.readInput()
    let newState = ApplicationLogic.updateState s input 
    IO.showApplicationSate newState
    applicationLoop' newState