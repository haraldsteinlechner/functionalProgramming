module Sorting where
    
import Test.QuickCheck

sorted :: Ord a => [a] -> Bool 
sorted (x:y:ys) = x <= y && sorted (y:ys)
sorted _        = True


qs [] = []
qs (head:rest) = qs smaller ++ [head] ++ qs larger
    where smaller = [x | x <- rest, x <= head] 
          larger = [x | x <- rest, x > head]

prop_sorted :: [Int] -> Bool
prop_sorted xs = sorted (qs xs)

runTests = quickCheck prop_sorted



extractFirstElement (x:y:xs) = xs

theList = 1 : [2,3,4]

aList = [1..1000000]

mappedList = map (\x -> x * 2) aList