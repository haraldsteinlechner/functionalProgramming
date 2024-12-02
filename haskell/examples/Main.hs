module Main where

import System.Directory ( getDirectoryContents )


mySum :: [Int] -> Int
mySum [] = 0
mySum  (x:xs) = x + mySum xs


main :: IO ()
main = putStrLn "Hello, Haskell!"
