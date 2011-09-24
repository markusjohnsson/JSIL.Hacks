using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL;

namespace HelloInt64
{
    //using TestLong = Int64;
    using TestLong = HelloInt64.Long;

    public class Program
    {
        public static bool Jsil
        {
            get
            {
                return Verbatim.Expression("{}") != null;
            }
        }

        public static void Main(string[] args)
        {
            TestLong x = 1000000000;
            TestLong y = 110;
            TestLong z = x / y;

            Print(z);

            if (!Jsil)
                Console.ReadLine();
        }

        private static void Print(TestLong p)
        {
            if (Jsil)
                Element.GetById("target").InnerHtml += p.ToString() + "<br/>";
            else
                Console.WriteLine(p.ToString());
        }
    }
}
