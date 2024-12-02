namespace TurtleGraphics


[<AutoOpen>]
module PartTwo =
    type Value = float
    type Variable = string

    type BinaryOp = Add | Minus | Mul
    type Comparison = Less | Greater | Equal

    type Cmd =
        | Forward of Variable
        | Left    of Variable
        | Right   of Variable  
        | Assign  of Variable * Variable * BinaryOp * Variable 
        | Declare of Variable * Value
        | While   of Variable * Comparison * Variable * list<Cmd>

    type Program = list<Cmd>

    type Vec2 = float * float
    type Angle = float

    type TurtleState =
        {
            variables     : Map<string,Value>
            trail         : list<Vec2>
            position      : Vec2
            direction     : Angle
            food          : float
        }
        

    module Logics =
        let moveForward ((x,y) : Vec2) (angle : Angle) (distance : float) =
            let angleInRadians = angle * ((2.0 * System.Math.PI) / 360.0)
            x + distance * cos angleInRadians, y + distance * sin angleInRadians

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TurtleState =
 
        let lookup (s : TurtleState) (varName : string) =
            match Map.tryFind varName s.variables with
                | Some v-> v
                | _ -> failwithf "undeclared variable: %s" varName

        let setVariable (s : TurtleState) (varName : string) (value : float) : TurtleState =
            { s with variables = Map.add varName value s.variables }

        let empty intialPos supplies  =
            {
                variables = Map.empty
                position = intialPos
                trail = [intialPos]
                direction = 0.0
                food = supplies
            }

    let interpretOp (o : BinaryOp) =
        match o with
            | Add -> (+)
            | Minus -> (-)
            | Mul -> (*)

    let interpretCmp (o : Comparison) (arg : float) (comparand : float) =
        match o with
            | Less -> arg < comparand 
            | Greater -> arg > comparand
            | Equal -> arg = comparand

    let rec interpretCmd (state : TurtleState) (c : Cmd) : TurtleState =
        match c with
            | Forward distanceVar -> 
                let distance = TurtleState.lookup state distanceVar
                let actualDistance = min state.food distance
                let newPosition = Logics.moveForward state.position state.direction actualDistance
                { state with 
                    trail = newPosition :: state.trail
                    position = newPosition
                    food = state.food - actualDistance
                }
            | Left angleVar -> 
                let angle = TurtleState.lookup state angleVar
                { state with direction = state.direction - angle }
            | Right angleVar -> 
                let angle = TurtleState.lookup state angleVar
                { state with direction = state.direction + angle }
            | Assign(target,opVar1,arith,opVar2) -> 
                let op1 = TurtleState.lookup state opVar1
                let op2 = TurtleState.lookup state opVar2
                let result = (interpretOp arith) op1 op2
                TurtleState.setVariable state target result
            | While(argumentVar,cmp,comparand,body) -> 
                let arg = TurtleState.lookup state argumentVar
                let comparand = TurtleState.lookup state comparand
                let result = interpretCmp cmp arg comparand
                if result then 
                    let s = interpret state body 
                    if s.food > 0.0 then interpretCmd s c else s
                else state
            | Declare(name,value) ->
                    TurtleState.setVariable state name value

    and interpret (state : TurtleState) (commands : list<Cmd>) =
        match commands with
            | x::xs -> 
                let newState = interpretCmd state x
                if newState.food > 0.0 then interpret newState xs
                else newState
            | [] -> state    


    module Examples =

        let spiral =
            let state = TurtleState.empty (0.0,0.0) 0.0 
            let program = [
                Declare("delta",3.0)
                Declare("turn",90.0)
                Declare("lineLen",100.0)
                Declare("minimum",0.0)

                While("lineLen",Comparison.Greater,"minimum", [
                        Forward "lineLen"
                        Right "turn"
                        Assign("lineLen","lineLen",Minus,"delta")
                    ]
                )
            ]
            program, state

        let star =
            let state = TurtleState.empty (50.0,60.0) 0.0 
            let program = [
                Declare("count",0.0)
                Declare("starAngle",144.0)
                Declare("one",1.0)
                Declare("scale",2.0)
                Declare("iterations",100.0)
                While("count",Comparison.Less,"iterations", [
                        Assign("dist","count",Mul,"scale")
                        Forward "dist"
                        Right "starAngle"
                        Assign("count","count",Add,"one")
                    ]
                )
            ]
            program,state

    let runTurtleProgram (initialState : TurtleState) (p : Program) : list<Vec2> =
        let resultState = interpret initialState p
        resultState.trail |> List.rev
