using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP2023
{
    internal class LazyPerformance
    {
        public static int FibHelper(int n)
        {
            if ((n == 0) || (n == 1))
            {
                return n;
            }
            else
                return FibHelper(n - 1) + FibHelper(n - 2);
        }
        public static void Test()
        {
            var data = 
                Enumerable.Range(0, int.MaxValue)
                .Select(v => {
                    Console.WriteLine("Creating input value: " + v);
                    return v;
                });
            Func<int, int> fib = x =>
            {
                Console.WriteLine("computing fib on: " + x);
                return FibHelper(x);
            };
        
            var result = data.Select(x => fib(x));
            var requiredData = result.Take(8);
            foreach(var r in requiredData)
            {
                Console.WriteLine(r);
            }
            Console.WriteLine("done");

            foreach (var r in requiredData)
            {
                Console.WriteLine(r);
            }
            Console.WriteLine("done");
        }
    }
}
