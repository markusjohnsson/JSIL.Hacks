using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSIL.Ui.Mvvm
{
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs args);

    public class PropertyChangedEventArgs: EventArgs
    {
        public string PropertyName { get; private set; }

        public PropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
