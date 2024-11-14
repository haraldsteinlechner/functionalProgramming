namespace Lecture1

module CountLines =

    module IO =
        open System.IO

        // performs IO, not purely functional
        let readFileContents (dir : string) (extension : string)=
            Directory.EnumerateFiles(dir, extension) |> Seq.map (fun f -> File.ReadAllText f)


    module LineCounter =

        let countLines (shortLineThreshold : int) (contents : seq<string>) =
            contents 
            |> Seq.map (fun c -> c.Split System.Environment.NewLine)
            |> Seq.filter (fun c -> c.Length > shortLineThreshold)
            |> Seq.map (fun c -> c.Length)
            |> Seq.sum


    module Tests = 
        open System
        open FsCheck
        open LineCounter

        type SourceFile = { content : string; lineCount : int }

        let charNonSpecial = 
            gen {
                let! n = Gen.choose(int 'a', int 'z')
                return char n
            }

        let arbSourceFile (minLen : int) (maxLen : int) = 
            gen {
                let! n = Gen.choose (minLen, maxLen) 
                let! r = Gen.arrayOfLength n (Gen.arrayOf charNonSpecial |> Gen.map String)
                return { content = (String.concat Environment.NewLine r); lineCount = n }
            }

        let count (s : list<SourceFile>) =
            let largeFiles = s |> List.sumBy (fun s -> if s.lineCount > 10 then s.lineCount else 0)
            let c = countLines 10 (s |> List.map (fun s -> s.content))
            c = largeFiles

    
        let canCount = Prop.forAll (Arb.fromGen (Gen.listOf (arbSourceFile 0 100))) count
        FsCheck.Check.Quick canCount




    let countLocDir path extension = 
        IO.readFileContents path extension |> LineCounter.countLines 10


    let runInSourceDirectory () =
        countLocDir __SOURCE_DIRECTORY__ "*.fs" |> printfn "LOC: %d"
