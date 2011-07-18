using System.Collections;
using JSIL.Dom;
using JSIL.Ui;
using System.Collections.Generic;

namespace JSIL.Ui
{
    public class ItemsControl<T>: Element
    {
        private Container _container;
        
        public ItemsControl(Container container): base("div")
        {
            _container = container;
            _itemElementFactory = new DefaultElementFactory();
            AppendChild(container);
        }

        class DefaultElementFactory : IElementFactory<T>
        {
            public Element CreateElement(T o)
            {
                if (o == null)
                {
                    return new Element("div");
                }
                return new Element("div") { TextContent = o.ToString() };
            }
        }

        #region ItemElementFactory
        private IElementFactory<T> _itemElementFactory;
        public IElementFactory<T> ItemElementFactory
        {
            get
            {
                return _itemElementFactory;
            }
            set
            {
                _itemElementFactory = value ?? new DefaultElementFactory();
                Update();
            }
        }
        #endregion

        #region ItemsSource
        private IEnumerable<T> _ItemsSource;
        public IEnumerable<T> ItemsSource
        {
            get
            {
                return _ItemsSource;
            }
            set
            {
                _ItemsSource = value ?? new List<T>();
                Update();
            }
        }
        #endregion

        public void Update()
        {
            _container.Clear();

            var count = 0;
            foreach (var item in ItemsSource)
            {
                count ++;
                _container.AppendChild(ItemElementFactory.CreateElement(item));
            }

            _container.AppendChild(new Label(count.ToString()));
        }

    }
}
