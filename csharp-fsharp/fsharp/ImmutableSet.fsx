#r "nuget:FsCheck"

type Tree<'k,'v> =
     | Node of Tree<'k,'v> * 'k  * 'v * Tree<'k,'v>
     | Empty


type Set<'k> =
    | SetEmpty
    | SetNode of Set<'k> * 'k * Set<'k>

module Set =
    let rec add (v : 'k) (s : Set<'k>) : Set<'k> =
        match s with
        | SetEmpty -> SetNode(SetEmpty, v, SetEmpty)
        | SetNode(l,value,r) -> 
            if value = v then s
            elif v < value then SetNode(add v l, value, r)
            else SetNode(l, value, add v r)

    let empty = SetEmpty

let values = Set.add 100 (Set.add 1 (Set.add 2 Set.empty))

let rec toListSorted (s : Set<'k>) =
    match s with
    | SetEmpty -> []
    | SetNode(l,v,r) -> List.concat [toListSorted l; [v]; toListSorted r]


let ofList xs = List.fold (fun s e -> Set.add e s) Set.empty xs


module Testing =
    open FsCheck

    let isSorted xs = xs |> List.pairwise |> List.forall (fun (a,b) -> a <= b)

    let canSort (xs:list<int>) = isSorted (toListSorted (ofList xs))
    
    Check.Quick canSort