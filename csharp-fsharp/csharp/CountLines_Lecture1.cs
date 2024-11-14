using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lecture1
{
    public static class Extensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            foreach (var x in xs)
            {
                if (predicate(x))
                {
                    yield return x;
                }
            }
        }

        public static IEnumerable<B> Map<A, B>(IEnumerable<A> xs, Func<A, B> mapping)
        {
            return xs.Select(mapping);
        }

        public static int Sum(IEnumerable<int> xs)
        {
            return xs.Sum();
        }
    }
    class CountLines
    {
        public static int CountLinesImperative(string[] contents)
        {
            var count = 0;
            for (int i = 0; i < contents.Length; i++)
            {
                var content = contents[i];
                var lines = content.Split(Environment.NewLine);
                if (lines.Length > 10)
                {
                    count += lines.Length;
                }
            }
            return count;
        }

        public static int CountLinesFunctional(IEnumerable<string> contents)
        {
            return contents
                .Select(content => content.Split(Environment.NewLine))
                .Where(lines => lines.Length > 10)
                .Select(lines => lines.Length)
                .Sum();
        }

        public static int CountLinesFunctional2(IEnumerable<string> contents)
        {
            return contents
                .Select(content => content.Split(Environment.NewLine))
                .Filter(lines => lines.Length > 10)
                .Select(lines => lines.Length)
                .Sum();
        }

        public static int CountLinesFunctional3(IEnumerable<string> contents)
        {
            var mapped = Extensions.Map(contents, content => content.Split(Environment.NewLine));
            var filtered = Extensions.Filter(mapped, lines => lines.Length > 10);
            var lengths = Extensions.Map(filtered, lines => lines.Length);
            return Extensions.Sum(lengths);
        }


        public static int CountLinesFunctionalParallel(IEnumerable<string> contents)
        {
            return contents
                .AsParallel()
                .Select(content => content.Split(Environment.NewLine))
                .Filter(lines => lines.Length > 10)
                .Select(lines => lines.Length)
                .Sum();
        }

        public static Func<A, C> Sequence<A, B, C>(Func<A, B> first, Func<B, C> second)
        {
            return a => second(first(a));
        }



        public static void Test()
        {
            var path = @"F:\aardvark\aardvark.base\src\Aardvark.Base\AlgoDat";


            var contents =
                    Directory.EnumerateFiles(path)
                    .Select(f => File.ReadAllText(f)).ToArray();

            Console.WriteLine(CountLinesImperative(contents));
            Console.WriteLine(CountLinesFunctional(contents));
            Console.WriteLine(CountLinesFunctional2(contents));
            Console.WriteLine(CountLinesFunctionalParallel(contents));


            Func<int, int> mul2 = x => x * 2;
            Func<int, string> toString = v => v.ToString();

            var multiplied = mul2(2);
            var asString = toString(multiplied);

            var composed = Sequence(mul2, toString);
            composed(2);


            SideEffectsInFP.TestFuncs();
            SideEffectsInFP.TestFuncs2();
            SideEffectsInFP.TestFuncs3();

        }
    }
}
