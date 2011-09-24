using JSIL.Meta;
using System;
using System.Collections.Generic;

namespace JSIL.Dom
{
    public class StyleCollection : IEnumerable<object>
    {
        private Element _parent;

        public StyleCollection(Element element)
        {
            _parent = element;
        }

        //[JSReplacement("$this._parent._element.style[$name] = $value")]
        public void Add(string name, string value)
        {
            Verbatim.Expression("this._parent._element.style[name] = value");
        }

        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
