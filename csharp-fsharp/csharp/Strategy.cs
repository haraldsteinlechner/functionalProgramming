using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP2023
{
    internal class Strategy
    {
        public record Person(string Name, string activationId);

        public interface ValidCustomer
        {
            bool IsAllowed();
        }

        public static void Test()
        {

        }
    }
}
