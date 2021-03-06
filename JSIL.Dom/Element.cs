﻿using JSIL.Meta;
using System;
using System.Linq;
using System.Collections.Generic;

namespace JSIL.Dom
{
    public class Element
    {
        #region Events
        private event EventHandler _change;
        public event EventHandler Change
        {
            add
            {
                AddNativeHandler("change", e =>
                {
                    this._change(this, new EventArgs());
                });
                _change += value;
            }
            remove
            {
                RemoveNativehandler("change");
                _change -= value;
            }
        }

        private event EventHandler _mouseOver;
        public event EventHandler MouseOver
        {
            add
            {
                AddNativeHandler("mouseover", e =>
                {
                    this._mouseOver(this, new EventArgs());
                });
                _mouseOver += value;
            }
            remove
            {
                RemoveNativehandler("mouseover");
                _mouseOver -= value;
            }
        }

        private event EventHandler _mouseOut;
        public event EventHandler MouseOut
        {
            add
            {
                AddNativeHandler("mouseout", e =>
                {
                    this._mouseOut(this, new EventArgs());
                });
                _mouseOut += value;
            }
            remove
            {
                RemoveNativehandler("mouseout");
                _mouseOut -= value;
            }
        }

        private event EventHandler _click;
        public event EventHandler Click
        {
            add
            {
                AddNativeHandler("click", e =>
                {
                    this._click(this, new EventArgs());
                });
                _click += value;
            }
            remove
            {
                RemoveNativehandler("click");
                _click -= value;
            }
        }

        #endregion

        #region Generic event handling
        protected void RemoveNativehandler(string eventName)
        {
            var handler = _handlers[eventName];
            handler.Counter--;
            if (handler.Counter <= 0)
            {
                RemoveEventListener(eventName, handler.Handler);
                _handlers[eventName] = null;
            }
        }

        protected void AddNativeHandler(string eventName, Action<object> handler)
        {

            if (!_handlers.ContainsKey(eventName) || _handlers[eventName] == null)
            {
                var proxy = new Proxy
                {
                    Handler = handler
                };
                AddEventListener(eventName, proxy.Handler);
                _handlers[eventName] = proxy;
            }
            else
            {
                _handlers[eventName].Counter++;
            }
        }

        class Proxy
        {
            public int Counter = 0;
            public Action<object> Handler;
        }

        private Dictionary<object, Proxy> _handlers = new Dictionary<object, Proxy>();

        [JSReplacement("$this._element.addEventListener($name, $handler)")]
        private void AddEventListener(string name, Action<object> handler)
        {
        }

        [JSReplacement("$this._element.removeEventListener($name, $handler)")]
        private void RemoveEventListener(string name, Action<object> handler)
        {
        }

        #endregion

        #region Properties
        public bool Enabled
        {
            get { return (bool)Verbatim.Expression("!this._element.disabled"); }
            set { Verbatim.Expression("this._element.disabled = !value"); }
        }

        public double Width
        {
            get { return (double)Verbatim.Expression("this._element.width"); }
            set { Verbatim.Expression("this._element.width = value"); }
        }

        public double Height
        {
            get { return (double)Verbatim.Expression("this._element.height"); }
            set { Verbatim.Expression("this._element.height = value"); }
        }
        #endregion

        public StyleCollection Style
        {
            get;
            private set;
        }

        protected object _element;
        protected Element _selfReference;

        public Element(string type)
            : this(Verbatim.Expression("document.createElement(type)"))
        {
        }

        protected Element(object element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
            _selfReference = this;
            Style = new StyleCollection(this);

            if (!_creatingTemplate)
                TemplateApplied();
        }

        // this is a horrible thing: a flag that is used to prevent a call to TemplateApplied 
        // in the constructor while creating elements from templates
        private static bool _creatingTemplate = false;

        public static T CreateFromTemplate<T>(string templateId) where T : Element, new()
        {
            _creatingTemplate = true;
            var element = new T();
            _creatingTemplate = false;
            element._element = Verbatim.Expression("document.getElementById(templateId)");
            element._selfReference = element;
            element.TemplateApplied();
            return element;
        }

        protected virtual void TemplateApplied()
        {
        }

        public static Element GetById(string id)
        {
            var element = GetElement(Verbatim.Expression("document.getElementById(id)"));

            if (element == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }
            else
            {
                return element;
            }
        }

        private static Element GetElement(object handle)
        {
            if (handle == null)
            {
                return null;
            }

            object element = Verbatim.Expression("handle._selfReference");

            if (element == null || element == Verbatim.Expression("undefined"))
            {
                return new Element(handle);
            }
            else
            {
                return (Element)element;
            }
        }

        [JSReplacement("$this._element.removeChild($child._element)")]
        public virtual void RemoveChild(Element child)
        {
        }

        public Element FirstChild
        {
            get
            {
                return GetElement(Verbatim.Expression("this._element.firstChild"));
            }
        }

        public Element NextSibling
        {
            get
            {
                return GetElement(Verbatim.Expression("this._element.nextSibling"));
            }
        }

        [JSReplacement("$this._element.nodeType")]
        public int NodeType
        {
            get;
            private set;
        }

        [JSReplacement("$this._element.style[$styleName]=$value")]
        public void SetStyle(string styleName, string value)
        {
        }

        [JSReplacement("$this._element.appendChild($childElement._element)")]
        public virtual void AppendChild(Element childElement)
        {
        }

        public string TextContent
        {
            get { return (string)Verbatim.Expression("this._element.textContent"); }
            set { Verbatim.Expression("this._element.textContent = value"); }
        }

        public string InnerHtml
        {
            get { return (string)Verbatim.Expression("this._element.innerHTML"); }
            set { Verbatim.Expression("this._element.innerHTML = value"); }
        }

        [JSReplacement("$this._element.className = \" \" + $className")]
        public void AddClass(string value)
        {
        }

        public void RemoveClass(string className)
        {
            var classNames = GetAttributeValue("className")
                .Split(' ', '\t')
                .Where(s => s != className);
            
            var names = string.Empty;

            foreach (var name in classNames)
            {
                names += " " + name;
            }

            SetAttributeValue("className", names);
        }

        [JSReplacement("$this._element[$attributeName]")]
        public string GetAttributeValue(string attributeName)
        {
            throw new NotSupportedException();
        }

        [JSReplacement("$this._element[$attributeName] = $value")]
        protected void SetAttributeValue(string attributeName, string value)
        {
        }
    }
}
