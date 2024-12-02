module Main where

-- see: https://wiki.haskell.org/OpenGLTutorial1

import Graphics.UI.GLUT
import qualified PartOne as PartOne

 
main :: IO ()
main = do
  (_progName, _args) <- getArgsAndInitialize
  _window <- createWindow "Functional Languages, Turtle Graphics Project"
  displayCallback $= display
  reshapeCallback $= Just reshape
  mainLoop
 
reshape :: ReshapeCallback
reshape size = do
  viewport $= (Position 0 0, size)
  matrixMode $= Projection
  loadIdentity
  ortho2D 0.0 100.0 0.0 100.0
  matrixMode $= Modelview 0
  loadIdentity
  postRedisplay Nothing

display :: DisplayCallback
display = do 
  clear [ColorBuffer]
  -- let points = PartOne.runTurtleProgram (0,0) spiralProgram
  let points = PartOne.runTurtleProgram (0,0) squareSpiralProgram
  -- let points = PartOne.parseAndRunTurtleProgramm (50,50) squareProgram
  renderPrimitive LineStrip $
     mapM_ (\(x, y) -> vertex $ Vertex2 x y) points
  flush


---------------------------------------------------------------
-- Predefined programs
---------------------------------------------------------------

squareSpiralProgram' :: PartOne.Value -> PartOne.Program
squareSpiralProgram' v
    | v <= 0 = []
    | otherwise = [(PartOne.CForward v), (PartOne.CRight 90)] ++ (squareSpiralProgram' (v-2))

-- | Square spiral
-- Start at (0,0)
squareSpiralProgram :: PartOne.Program
squareSpiralProgram = squareSpiralProgram' 100

squareProgram = "Forward(5); Right(90); Forward(5); Right(90);Forward(5); Right(90);Forward(5);"

spiralProgram' :: Float -> Float -> PartOne.Program
spiralProgram' v max
    | v >= max = []
    | otherwise = [(PartOne.CForward 0.8), (PartOne.CRight v)] ++ (spiralProgram' (v+0.01) max)

spiralProgram :: PartOne.Program
spiralProgram = spiralProgram' 0 10