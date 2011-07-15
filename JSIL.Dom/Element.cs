using JSIL.Meta;
using System;
using System.Collections.Generic;

namespace JSIL.Dom
{
    public class Element
    {
        #region Events
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
        private void RemoveNativehandler(string eventName)
        {
            var handler = _handlers[eventName];
            handler.Counter--;
            if (handler.Counter <= 0)
            {
                RemoveEventListener(eventName, handler.Proxy);
                _handlers[eventName] = null;
            }
        }

        private void AddNativeHandler(string eventName, Action<object> proxy)
        {

            if (!_handlers.ContainsKey(eventName) || _handlers[eventName] == null)
            {
                var handler = new Handler
                {
                    Proxy = proxy
                };
                AddEventListener(eventName, handler.Proxy);
                _handlers[eventName] = handler;
            }
            else
            {
                _handlers[eventName].Counter++;
            }
        }

        class Handler
        {
            public int Counter = 0;
            public Action<object> Proxy;
        }

        private Dictionary<object, Handler> _handlers = new Dictionary<object, Handler>();

        [JSReplacement("$this._element.addEventListener($name, $handler)")]
        private void AddEventListener(string name, Action<object> handler)
        {
        }

        [JSReplacement("$this._element.removeEventListener($name, $handler)")]
        private void RemoveEventListener(string name, Action<object> handler)
        {
        }

        #endregion

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

        protected object _element;

        public Element(string type)
        {
            _element = Verbatim.Expression("document.createElement(type)");
        }

        private Element()
        {
        }

        public static Element GetById(string id)
        {
            return new Element()
            {
                _element = Verbatim.Expression("document.getElementById(id)")
            };
        }

        [JSReplacement("$this._element.removeChild($child._element)")]
        public virtual void RemoveChild(Element child)
        {
        }

        [JSChangeName("firstChild")]
        public Element FirstChild
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
    }
}
