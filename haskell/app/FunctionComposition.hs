module FunctionComposition where

import System.Directory ( listDirectory  )


composeFunctions f g = \x -> f (g x)


r = composeFunctions (\x -> show x) (\x -> x + 1)


(∘) :: (t1 -> t2) -> (t3 -> t1) -> t3 -> t2
(∘) f g x = f (g x)

-- a pure function
countLines :: String -> Int
countLines =
    length ∘ filter isLarge ∘ lines 
        where 
            isLarge lines = length lines > 10



-- top level function which is allowed to perform IO
printLines :: IO ()
printLines = do
    -- IO....
    files <- listDirectory  "F:\\teaching\\Haskell"
    contents <- mapM readFile files
    -- pure computation ...
    let lineCounts = sum (map countLines contents)
    -- IO ...
    print lineCounts
