module LazyPerformance where
    
import Debug.Trace

fib 0 = 0
fib 1 = 1
fib n = fib (n-1) + fib (n-2) 

-- lists can be infinte (often called streams)
-- enumerables in c# and streams in java are very similar
fibs = [fib n | n <- [1 ..]] 

-- unnecessary computation can be spared out
onlyComputeNecessary = take 5 fibs

-- pipelines can be composed of primitve functions, no specialization needed
pipeline = concat . map show . map (+2) 


fibsTraced = [fib n | n <- stream] 
    where stream = [trace ("generating element: " ++ show x) x | x <- [1..]]

-- unnecessary computation can be spared out
onlyComputeNecessary2 = take 5 fibs

-- streams allow nifty formulations (maybe not that practical though)
fibs' = 0 : 1 : zipWith (+) fibs' (tail fibs')


