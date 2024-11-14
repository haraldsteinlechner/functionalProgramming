#r "nuget:FsCheck"

// this file contains a rather naive implementation of a persistent map datastructure
// implemented as binary search tree. Notably, it does not perform 
// re-balancing. Search and insertion can thus be O(n) as opposed to O(log n).
// As discussed in the lecture the main idea is as follows:

let users = Map.ofList [ (1, "harald"); (2, "fpIsAwesome")]
let withNewUser = Map.add 3 "functional datastructures" users
let firstUserRemoved = Map.remove 1 withNewUser
// here is the state of all versions of the datastructure:
(*
val users: Map<int,string> = map [(1, "harald"); (2, "fpIsAwesome")]
val withNewUser: Map<int,string> =
  map [(1, "harald"); (2, "fpIsAwesome"); (3, "functional datastructures")]
val firstUserRemoved: Map<int,string> =
  map [(2, "fpIsAwesome"); (3, "functional datastructures")]
*)

// Production quality implementations with balancing can be found here:
// - C#: https://learn.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablesorteddictionary?view=net-8.0
// - Java: https://github.com/hrldcpr/pcollections/blob/master/src/main/java/org/pcollections/TreePMap.java
// - F#: https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-mapmodule.html
// immutable maps often also use hash codes of objects being inserted. An example is:
// - ImmutableDictionary in C#: https://learn.microsoft.com/de-de/dotnet/api/system.collections.immutable.immutabledictionary-2?view=net-8.0

type Tree<'k,'v> =
     | Node of Tree<'k,'v> * 'k  * 'v * Tree<'k,'v>
     | Empty

// define a type alias, for our map we use the tree datatype defined above
type NaiveMap<'k, 'v> = Tree<'k, 'v>

module NaiveMap =
    
    let rec add (k : 'k) (v : 'v) (s : NaiveMap<'k, 'v>) : NaiveMap<'k, 'v> =
        failwith "your task"

    let rec find (k : 'k) (m : NaiveMap<'k, 'v>) : 'v =
        match m with
        | Empty -> failwith "key not found"
        | Node(l, nodeKey, nodeValue, r) ->
            if nodeKey = k then nodeValue
            elif k < nodeKey then find k l
            else find k r


    let rec tryFind (k : 'k) (m : NaiveMap<'k, 'v>) : Option<'v> =
        match m with
        | Empty -> None
        | Node(l, nodeKey, nodeValue, r) ->
            if nodeKey = k then Some nodeValue
            elif k < nodeKey then tryFind k l
            else tryFind k r

    let empty = Empty



let rec toListSorted (s : NaiveMap<'k, 'v>) =
    match s with
    | Empty -> []
    | Node(l, k, v, r) -> 
        let self = [ (k,v) ] // put k,v tuple into singleton list
        // concat concatenates a list of lists, here i used multiline formatting
        // to make it more readable
        List.concat [
            toListSorted l
            self
            toListSorted r
        ]


let ofList xs = List.fold (fun s (k,v) -> NaiveMap.add k v s) NaiveMap.empty xs


// some pretty straightforward tests, using auto-generated data (provided by FsCheck library)
module Testing =
    open FsCheck

    let simpleTests () =
        let users = ofList [ (1, "harald"); (2, "fpIsAwesome")]
        let withNewUser = NaiveMap.add 3 "functional datastructures" users
        let expectedUsers = [(1, "harald"); (2, "fpIsAwesome")]
        let expectedNewUser = [(1, "harald"); (2, "fpIsAwesome"); (3, "functional datastructures")]
        toListSorted users = expectedUsers && toListSorted withNewUser = expectedNewUser


    // checks whether a list is sorted by comparing subsequent elements
    let isSorted xs = xs |> List.pairwise |> List.forall (fun (a,b) -> a <= b)

    // adds elements to a map and retrieves the elements from the map.
    // since it uses a balanced binary tree, the output must be sorted
    let canSort (keys:list<int>) =
        let keyValuePairs = keys |> List.map (fun key -> key, string key)
        isSorted (toListSorted (ofList keyValuePairs))

    let canAdd (m : NaiveMap<int, int>) (newKey : int) (newValue : int) =
        let added = NaiveMap.add newKey newValue m
        NaiveMap.find newKey added = newValue


    let runTests () =   
        Check.Quick simpleTests
        Check.Quick canSort
        Check.Quick canAdd