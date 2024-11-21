using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalProgrammingCSharp
{
    public class SideEffectsLazyEvaluation
    {
        public static bool LessThanThirty(int x)
        {
            Console.WriteLine("{0}? Less than 30;", x);
            return x < 30;
        }
        public static bool MoreThanTwenty(int x)
        {
            Console.WriteLine("{0}? More than 20;", x);
            return x > 20;
        }

        public static void OrderOfEffects()
        {
            var l = new List<int>() { 1, 25, 40, 5, 23 };
            //var linq = from x in l where LessThanThirty(x) select x;


            var q0 = l.Where(LessThanThirty); // normally each statement executed before the next one, right?
            var q1 = q0.Where(MoreThanTwenty);

            q1.Count();

            foreach(var r in q1)
            {
                Console.WriteLine(r);
            }

            foreach (var r in q1)
            {
                Console.WriteLine(r);
            }
        }

        public static void Test1()
        {
            var l = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

            // is func a pure function?
            Func<int> computeLargest =
                () =>
                {
                    return l.Sum();
                };

            Console.WriteLine(computeLargest());

            l.Add(100);
            
            Console.WriteLine(computeLargest());

        }

        public static void ExceptionHandling()
        {
            var l = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };

            IEnumerable<int> divs = null;
            try
            {
                divs = l.Select(x => 1 / x);
            }
            catch
            {
                divs = new List<int>();
            }

            foreach(var r in divs)
            {
                Console.WriteLine(r);
            }

        }

        public static void Disposal()
        {
            Func<string> GetContents = null;
            
            using(var file = File.OpenText(@"..\..\..\Program.cs"))
            {
                GetContents = () => file.ReadToEnd();
            } // file.Dispose()

            GetContents();
        }

        public static void DelayedActions()
        {
            var actions = new List<Func<int>>();

            var s = "";
            for(var i = 4; i < 7; i++)
            {
                actions.Add(() => i);
            }

            foreach(var action in actions) 
            {
                Console.Write(action());
            }

        }

        public static void DelayedActions2()
        {
            var actions = new List<Func<int>>();

            var s = "";
            for (var i = 4; i < 7; i++)
            {
                var r = i;
                actions.Add(() => r);
            }

            foreach (var action in actions) // js 666
            {
                Console.Write(action());
            }

        }

        public static void DelayedActions3()
        {
            var actions = new List<Func<int>>();

            var s = "";
            var r = 0;
            for (var i = 4; i < 7; i++)
            {
                r = i;
                actions.Add(() => r);
            }

            foreach (var action in actions) // js 666
            {
                Console.Write(action());
            }

        }
    }
}
