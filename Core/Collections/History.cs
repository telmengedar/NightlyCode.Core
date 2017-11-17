using System.Collections.Generic;

namespace NightlyCode.Core.Collections
{
    public class History<T> : ICollection<T>
    {
        List<T> items = new List<T>();
        int maxsize = 30;

        public History(int maxsize)
        {
            this.maxsize = maxsize;
        }

        public T this[int index]
        {
            get { return items[index]; }
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            items.Add(item);
            while (items.Count > maxsize) items.RemoveAt(0);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion
    }
}
