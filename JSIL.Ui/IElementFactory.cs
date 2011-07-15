using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui
{
    public interface IElementFactory<T>
    {
        Element CreateElement(T item);
    }
}
