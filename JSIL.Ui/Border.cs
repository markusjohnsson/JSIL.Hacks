using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL;

namespace JSIL.Ui
{
    public class Border: Element
    {
        private Thickness _BorderThickness;
        public Thickness BorderWidth
        {
            get
            {
                return _BorderThickness;
            }
            set
            {
                _BorderThickness = value;
                var styleValue = string.Format("{0}px {1}px {2}px {3}px",
                    value.Top, value.Right, value.Bottom, value.Left);
                SetStyle("borderWidth", styleValue);
            }
        }

        private Color _BorderColor;
        public Color BorderColor
        {
            get
            {
                return _BorderColor;
            }
            set
            {
                _BorderColor = value;
                SetStyle("borderColor", value.ToString());
            }
        }

        private Color _BackgroundColor;
        public Color BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                _BackgroundColor = value;
                SetStyle("backgroundColor", value.ToString());
            }
        }

        private Element _Content;
        public Element Content
        {
            get
            {
                return _Content;
            }
            set
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

        public Border(): base("div")
        {
            SetStyle("borderStyle", "solid");
            SetStyle("display", "inline-block");
        }
    }
}
