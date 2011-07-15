using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace HelloWorld
{
    public class Program
    {
        public static void Main()
        {
            var target = Element.GetById("target");
            target.AppendChild(new Element("h1") { TextContent = "Hello World" });
        }
    }
}
