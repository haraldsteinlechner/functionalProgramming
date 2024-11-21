module Basics where
    
topLevelExpressions = 1


-- C#: Func<int,int> f = x => x + 1
-- Java: Function<Integer,Integer> f = x -> x + 1;
-- C++: auto incr = [](auto x) { return x + 1; };
topLevelExpressionLambda = \x -> x + 1

topLevelFunctions x = x + x

topLevelExpressionsWithType :: Int -> Int
topLevelExpressionsWithType x = x * 2

justListLiterals = [1,2,3]

-- short form for
justListLiterals2 = 1:2:3:[]


-- ifExpression = x < 10 ? "smaller 10" : "greater or equal 10"
ifExpression x = 
    if x < 10 then
        "smaller 10"
    else
        "greater or equal 10"


-- recursion and patterns
fib 0 = 0
fib 1 = 1
fib n = fib (n-1) + fib (n-2)

l = [1,2,3] -- syntax for lists
withoutSugar = 1 : 2 : 3 : []


theFirstElement (firstElementInList:restOfTheList) = restOfTheList

-- patterns on lists
firstElement :: [Int] -> String
firstElement []      = "no first element"
firstElement (x:xs) = show x ++ show xs

-- Int Length<T>(List<T> list)
--myLength :: [a] -> Int
myLength [] = 0
myLength (x:xs) = 1 + myLength xs

-- IEnumerable<T> Filter<T>(Func<T, bool> predicate, IEnumerable<T> list)
myFilter :: (a -> Bool) -> [a] -> [a]
myFilter p []      = []
myFilter p (x:xs)  = 
    if p x then
        x : (myFilter p xs)
    else
        myFilter p xs

mySum :: [Int] -> Int
mySum [] = 0
mySum  (x:xs) = x + mySum xs




infList = [1..]

ignoreList l value = value