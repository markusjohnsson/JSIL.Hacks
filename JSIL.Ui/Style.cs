using System;
using JSIL.Dom;

namespace JSIL.Ui
{
    public static class Styling
    {
        public static Style Style(this Element element)
        {
            return new Style(element);
        }
    }

    public class Style
    {
        private Element _element;

        public Style(Element element)
        {
            _element = element;
        }

        public Style Height(double h)
        {
            _element.SetStyle("height", h.ToString());
            return this;
        }

        public Style Width(double w)
        {
            _element.SetStyle("width", w.ToString());
            return this;
        }

        public Style Border(double thickness, string style = "solid", string color = "black")
        {
            _element.SetStyle("border", string.Format("{0}px {1} {2}", thickness, style, color));
            return this;
        }

        public Style Custom(string styleName, string value)
        {
            _element.SetStyle(styleName, value);
            return this;
        }
    }

}
