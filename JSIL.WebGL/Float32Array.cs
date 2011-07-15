using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Meta;

namespace JSIL.WebGL
{
    public class Float32Array
    {
        [JSReplacement("new Float32Array($array)")]
        public static Float32Array Create(double [] array)
        {
            throw new NotSupportedException();
        }
    }
}
