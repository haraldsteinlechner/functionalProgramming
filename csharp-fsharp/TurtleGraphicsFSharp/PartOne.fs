namespace TurtleGraphics


[<AutoOpen>]
module PartOne =
    type Value = float

    type Cmd =
        | Forward of Value
        | Left    of Value
        | Right   of Value  

    type Program = list<Cmd>

    type Vec2 = float * float
    type Angle = float

    type TurtleState = 
        {
            direction : Angle     // direction in degrees
            position : Vec2       // current position
            trail    : list<Vec2> // points produced so far
        }

    let interpretTurtleProgram (s : TurtleState) (commands : Program) =
        let runCommand s cmd =
            match cmd with
                | Forward d -> 
                    let (px,py) = s.position
                    let angleInRadians = s.direction * ((2.0 * System.Math.PI) / 360.0)
                    let newPos = px + d * cos angleInRadians, py + d * sin angleInRadians
                    { s with position = newPos; trail = newPos :: s.trail }
                | Right angle ->
                    { s with direction = s.direction + angle }
                | Left angle -> 
                    { s with direction = s.direction - angle }
        List.fold runCommand s commands


    module Examples =
    
        let quad = 
            let program =
                [ Forward 30.0; Left 90.0; Forward 30.0; Left 90.0; 
                  Forward 30.0; Left 90.0; Forward 30.0 ]
            program, (50.0,50.0)

        let spiral =
            let program =
                [
                    for lineLen in [100.0 .. (-2.00) .. 0.0] do
                        yield Forward lineLen
                        yield Left 90.0
                ]
            program, (0.0,0.0)

    let runTurtleProgram startPos (p : Program) : list<Vec2> =
        let initialState = { direction = 90.0; position = startPos; trail = [startPos] }
        let resultState = interpretTurtleProgram initialState p
        resultState.trail |> List.rev


    module Parser =

        open FParsec

        let pForward : Parser<Cmd,unit> = 
            skipString "Forward(" >>. pfloat .>> pchar ')' |>> Forward
        
        let pRight : Parser<Cmd,unit> = 
            skipString "Right(" >>. pfloat .>> pchar ')' |>> Right

        let pLeft : Parser<Cmd,unit> = 
            skipString "Left(" >>. pfloat .>> pchar ')' |>> Left

        let pCmd : Parser<Cmd,unit> =
            pForward <|> pRight <|> pLeft

        let pProgram : Parser<Program,unit> =
            let cmdWithSemicolon = 
                pCmd .>> spaces .>> pchar ';' .>> spaces
            manyTill cmdWithSemicolon eof

        let parseProgram (s : string) =
            run pProgram s 


        module Examples = 
        
            let test1 = """Forward(30.0); Left(90.0); Forward(30.0); Left(90.0); 
    Forward(30.0); Left(90.0); f Forward(30.0);"""