module NoMixIO where

lessThanThirty x = do
    print x
    return (x < 30)

moreThanTwenty x = do
    print x 
    return (x > 20)

lessThanThirty2 x = x < 30
moreThanTwenty2 x = x > 20

l = [1, 2, 25]
q0 = [ x | x <- l, lessThanThirty2 x]

q1 = [ x | x <- q0, moreThanTwenty2 x]





