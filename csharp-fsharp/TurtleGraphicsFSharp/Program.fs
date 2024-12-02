open System

open Aardvark.Base
open FSharp.Data.Adaptive
open Aardvark.Rendering
open Aardvark.Application
open Aardvark.Application.Slim
open Aardvark.SceneGraph
open TurtleGraphics


type TurtleExerciseTask = PartOne | PartTwo

[<EntryPoint;STAThread>]
let main argv = 
    
    Aardvark.Init()
    use app = new Slim.OpenGlApplication()
    let win = app.CreateGameWindow()

    let task = TurtleExerciseTask.PartTwo
    let vertices = 
        let sw = System.Diagnostics.Stopwatch.StartNew()
        win.Time |> AVal.map (fun time -> 
            match task with
            | PartOne ->
                let exampleProgram,desiredStart = PartOne.Examples.spiral
                PartOne.runTurtleProgram desiredStart exampleProgram  |> List.toArray
            | PartTwo -> 
                let exampleProgram,initialState = PartTwo.Examples.star
                let foodSupply = sw.Elapsed.TotalSeconds * 300.0
                let points = PartTwo.runTurtleProgram  { initialState with food = foodSupply } exampleProgram
                points |> List.toArray
        )

    let renderTask = 
        let toV3f (x,y) = V3f(x, y, 0.0)
        Sg.draw IndexedGeometryMode.LineStrip
        |> Sg.vertexAttribute DefaultSemantic.Positions (vertices |> AVal.map (Array.map toV3f))
        |> Sg.vertexBufferValue DefaultSemantic.Colors (AVal.constant V4f.IIII)
        |> Sg.viewTrafo' Trafo3d.Identity
        |> Sg.projTrafo' (Frustum.ortho (Box3d.FromMinAndSize(V3d.OOO, V3d.III * 100.0)) |> Frustum.projTrafo)
        |> Sg.shader {
            do! DefaultSurfaces.trafo
        }
        |> Sg.compile win.Runtime win.FramebufferSignature

    win.RenderTask <- renderTask
    win.Run()

    0 