{-# LANGUAGE FlexibleContexts #-}
module PartOne where

import Text.Parsec

---------------------------------------------------------------
-- Domain
---------------------------------------------------------------

type Value = Float

data Cmd = CForward Value | CLeft Value | CRight Value deriving Show
type Program = [Cmd]

type Vec2 = (Float, Float)
type Angle = Float

data TurtleState = TurtleState {
    direction :: Angle,
    position  :: Vec2,
    trail     :: [Vec2]
} deriving Show

---------------------------------------------------------------
-- Interpreter
---------------------------------------------------------------

-- | Convert degree to radiant
rad :: Float -> Float
rad x = x/180*pi

-- | Cos of degree
cos' :: Float -> Float
cos' x = cos $ rad x

-- | Sin of degree
sin' :: Float -> Float
sin' x = sin $ rad x

-- | Calculate coordinate of one point
-- Float    x or y coordinate
-- Float    distance
-- angle    angle
-- (Float -> Float)     function to calculate angle transformation (cos/sin)
calcXorY :: Float -> Float -> Angle -> (Float -> Float) -> Float
calcXorY x d a f = x + d * (f a)

-- Vec2             original pos
-- Float            distance to move
-- Angle            angle to move
calcXYPos :: Vec2 -> Float -> Angle -> Vec2
calcXYPos (x, y) d a = ((calcXorY x d a cos'), (calcXorY y d a sin'))

-- | Helper to run commands for turtle
cmdTurtle' :: TurtleState -> Angle -> Value -> TurtleState
cmdTurtle' ts a v = TurtleState a (calcXYPos (position ts) v a) ((trail ts) ++ [(position ts)])

-- | Turtle single command interpreter
cmdTurtle :: TurtleState -> Cmd -> TurtleState
cmdTurtle ts (CForward v) = cmdTurtle' ts (direction ts) v
cmdTurtle ts (CLeft v)    = cmdTurtle' ts ((direction ts) + v) 0
cmdTurtle ts (CRight v)   = cmdTurtle' ts ((direction ts) - v) 0

-- | Turtle multiple command interpreter
interpretTurtleProgram :: TurtleState -> Program -> TurtleState
interpretTurtleProgram ts []     = ts
interpretTurtleProgram ts (c:cs) = interpretTurtleProgram (cmdTurtle ts c) cs

---------------------------------------------------------------
-- Paser
---------------------------------------------------------------

-- Parse float
-- see: https://www.schoolofhaskell.com/user/stevely/parsing-floats-with-parsec#parsing-floats
number = many1 digit
plus = char '+' *> number
minus = (:) <$> char '-' <*> number
integer = plus <|> minus <|> number

float = fmap rd $ (++) <$> integer <*> decimal
    where rd      = read :: String -> Float
          decimal = option "" $ (:) <$> char '.' <*> number

pValueInParenthesis :: Parsec String () Value
pValueInParenthesis = 
    do 
        char '('
        v <- float
        char ')'
        return v

pSeparator :: Parsec String () ()
pSeparator = 
    do
        spaces
        char ';'
        spaces

-- | Generates parser combinators for CMDs
-- String 			string that input shold have, to be parsed in the command
-- (Value -> Cmd) 	Cmd
generateCmdParser :: String -> (Value -> Cmd) -> Parsec String () Cmd
generateCmdParser s c =
    do
        string s
        v <- pValueInParenthesis
        return $ c v

pForward :: Parsec String () Cmd
pForward = generateCmdParser "Forward" CForward

pLeft :: Parsec String () Cmd
pLeft = generateCmdParser "Left" CLeft

pRight :: Parsec String () Cmd
pRight = generateCmdParser "Right" CRight

pCmd :: Parsec String () Cmd
pCmd = pForward
    <|> pLeft 
    <|> pRight 
    <?> "command 'Forwad', 'Left' or 'Right'"

pProgram :: Parsec String () Program
pProgram = many $ do 
        c <- pCmd 
        eof <|> pSeparator
        return c

parseCmd p = parse p ""

---------------------------------------------------------------
-- Run
---------------------------------------------------------------

runTurtleProgram :: (Float,Float) -> Program -> [Vec2]
runTurtleProgram pos program =
    (trail turtle) ++ [position turtle]
    where turtle = interpretTurtleProgram (TurtleState 90 pos []) program


parseAndRunTurtleProgramm :: (Float,Float) -> String -> [Vec2]
parseAndRunTurtleProgramm pos str = 
    case parseCmd pProgram str of 
        Left a -> error "Parse error"
        Right p -> runTurtleProgram pos p
