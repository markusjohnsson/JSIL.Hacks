using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui
{
    public struct Thickness
    {
        public readonly int Top;
        public readonly int Left;
        public readonly int Right;
        public readonly int Bottom;
        
        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(int uniformThickness): 
            this(uniformThickness, uniformThickness, uniformThickness, uniformThickness)
        {
            
        }
    }
}
