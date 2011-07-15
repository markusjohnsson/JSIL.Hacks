using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL.Ui.Primitives;

namespace JSIL.Ui
{
    public class Container : Element
    {
        public Container()
            : base("div")
        {
            Recreate();
        }

        class ContainerList : HookList<Element>
        {
            private Container _parent;
            public ContainerList(Container parent)
            {
                _parent = parent;
            }
            protected override void OnAdd(Element item)
            {
                _parent.AppendChild(item);
            }
            protected override void OnRemove(Element item, bool success)
            {
                if (success) _parent.RemoveChild(item);
            }
        }

        public IList<Element> Content { get; private set; }

        private Dictionary<Element, Element> _children;

        protected virtual Element Prepare(Element childElement)
        {
            return new ElementWrapper(childElement);
        }

        public override void AppendChild(Element childElement)
        {
            var wrapper = Prepare(childElement);
            _children[childElement] = wrapper;
            base.AppendChild(wrapper);
        }

        public override void RemoveChild(Element child)
        {
            var actualChild = _children[child];
            base.RemoveChild(actualChild);
        }

        public void Clear()
        {
            while (FirstChild != null)
            {
                RemoveChild(FirstChild);
            }
            Recreate();
        }

        private void Recreate()
        {
            _children = new Dictionary<Element, Element>();
            Content = new ContainerList(this);
        }
    }
}
