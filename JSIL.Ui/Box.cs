using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui.Primitives
{
    public class ElementWrapper : Element
    {
        public ElementWrapper(Element childElement)
            : base("div")
        {
            Content = childElement;
        }

        #region Content
        private Element _Content;
        public Element Content
        {
            get
            {
                return _Content;
            }
            private set
            {
                _Content = value;

                while (FirstChild != null)
                {
                    RemoveChild(FirstChild);
                }

                if (value != null)
                {
                    AppendChild(value);
                }
            }
        }
        #endregion
    }
}
