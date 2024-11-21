module CountLines where
    
import System.Directory ( listDirectory  )

-- just to show some syntax
myFilterSyntax p xs = [x | x <- xs, p x]

-- what if we have only functions, lists and recursion?
myFilter :: (a -> Bool) -> [a] -> [a] 
myFilter p [] = []
myFilter p (x:xs) = 
    if p x then 
        x : myFilter p xs
    else 
        myFilter p xs

-- countLines, written with lots of immediate variables
countLines :: String -> Int
countLines fileContent =
    length largeFiles
    where
        largeFiles = filter isLarge splitInLines
        splitInLines = lines fileContent
        isLarge l = length l > 10

-- top level function which is allowed to perform IO
printLines :: IO ()
printLines = do
    -- IO....
    files <- listDirectory  "."
    contents <- mapM readFile files
    -- pure computation ...
    let lineCounts = map countLines contents
    let summedLines = sum lineCounts
    -- IO ...
    print summedLines


-- a pure function
countLines2 :: String -> Int
countLines2 =
    length . filter isLarge . lines 
        where 
            isLarge lines = length lines > 10



-- top level function which is allowed to perform IO
printLines2 :: IO ()
printLines2 = do
    -- IO....
    files <- listDirectory  "F:\\teaching\\Haskell"
    contents <- mapM readFile files
    -- pure computation ...
    let lineCounts = sum (map countLines2 contents)
    -- IO ...
    print lineCounts



foo x = x + 1

one = 1
foo2 = \x -> x + one

aList = [1..1000000]