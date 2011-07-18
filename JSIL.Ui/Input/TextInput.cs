using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui.Input
{
    public class TextInput : InputBase
    {
        public TextInput()
        {
            SetAttributeValue("type", "text");
        }

        public string PlaceHolder
        {
            get { return GetAttributeValue("placeholder"); }
            set { SetAttributeValue("placeholder", value); }
        }

    }
}
