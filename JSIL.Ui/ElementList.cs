using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui
{
    class HookList<T>: IList<T>
    {
        private List<T> _inner = new List<T>();

        public int IndexOf(T item)
        {
            return _inner.IndexOf(item);
        }

        protected virtual void OnInsert(int index, T item)
        {
        }

        public void Insert(int index, T item)
        {
            _inner.Insert(index, item);
            OnInsert(index, item);
        }

        protected virtual void OnRemoveAt(int index)
        {
        }

        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
            OnRemoveAt(index);
        }

        protected virtual void OnSet(int index, T oldElement, T newElement)
        {
        }

        public T this[int index]
        {
            get
            {
                return _inner[index];
            }
            set
            {
                var oldElement = _inner[index];
                _inner[index] = value;
                OnSet(index, oldElement, value);
            }
        }
        
        protected virtual void OnAdd(T item)
        {
        }

        public void Add(T item)
        {
            _inner.Add(item);
            OnAdd(item);
        }

        protected virtual void OnClear()
        {
        }

        public void Clear()
        {
            _inner.Clear();
            OnClear();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        protected virtual void OnRemove(T item, bool success)
        {
        }

        public bool Remove(T item)
        {
            var result = _inner.Remove(item);
            OnRemove(item, result);
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
