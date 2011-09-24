using System;
using JSIL.Ui;
using JSIL.Dom;
using JSIL;

namespace JSIL.Ui
{
    public class TemplateElementFactory<T>: IElementFactory<T>
    {
        private string _templateName;
        private string _htmlText;
        
        public TemplateElementFactory(string templateName)
        {
            _templateName = templateName;
        }

        public Element CreateElement(T item)
        {
            var html = GetHtml();
            var element = new Element("div");

            Verbatim.Expression(@"
            var regex = new RegExp(""{([a-zA-Z]*[a-zA-Z0-9]*)}"", ""g"");
            
            var matcher = function (match, property, offset, str) 
            {
                return item[property];
            };
            html = html.replace(regex, matcher);
            ");

            element.InnerHtml = html;
            return element;
        }

        private string GetHtml()
        {
            if (_htmlText == null)
                Load();
            return _htmlText;
        }

        private void Load()
        {
            var templateScript = Element.GetById(_templateName);
            _htmlText = templateScript.TextContent;
        }
    }
}
