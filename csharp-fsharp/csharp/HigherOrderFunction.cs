using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalProgrammingCSharp
{
    public class HigherOrderFunction
    {
        public static Func<A,R> Compose<A,I,R>(Func<A,I> first, Func<I,R> second)
        {
            return a => second(first(a));
        }
        public static void Test()
        {
            Func<int, int> f = x => x * 2;
            Func<int, string> print = x => $"result was: {x.ToString()}";

            Func<int, string> multToString = Compose(f, print);
            Console.WriteLine(multToString(2));
        }
    }
}
