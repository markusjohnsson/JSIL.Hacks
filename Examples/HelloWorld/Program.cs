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
            // equivalent to <h1>Hello World</h1>:
            var header = new Element("h1") { TextContent = "Hello World" };

            // equivalent to document.getElementById("target"):
            var target = Element.GetById("target");
            target.AppendChild(header);

            // Hook up mouse events to the element, changing the background color on hover:
            header.MouseOver += (s, e) => { header.SetStyle("background-color", "#f00"); };
            header.MouseOut += (s, e) => { header.SetStyle("background-color", "#fff"); };
        }
    }
}
