using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSIL.Ui
{
    public struct Color
    {
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;
        public readonly byte Alpha;

        public Color(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public override string ToString()
        {
            return string.Format("rgba({0},{1},{2},{3})", Red, Green, Blue, Alpha);
        }
    }
}
