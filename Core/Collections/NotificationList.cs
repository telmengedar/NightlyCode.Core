using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// list supporting <see cref="INotifyPropertyChanged"/> events for a <see cref="IBindingList"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotificationList<T> : IBindingList, IEnumerable<T> {
        readonly List<T> list = new List<T>();
        readonly bool supportsitemchanged;

        /// <summary>
        /// creates a new <see cref="NotificationList{T}"/>
        /// </summary>
        public NotificationList() {
            supportsitemchanged = typeof(T).GetInterface(nameof(INotifyPropertyChanged)) != null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator() {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.-or-The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception><filterpriority>2</filterpriority>
        void ICollection.CopyTo(Array array, int index) {
            foreach(T item in list)
                array.SetValue(item, index++);
        }

        public int Count => list.Count;

        public object SyncRoot => null;

        public bool IsSynchronized => false;

        public void Add(T value) {
            list.Add(value);
            if(supportsitemchanged)
                ((INotifyPropertyChanged)value).PropertyChanged += OnItemChanged;
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, list.Count - 1));
        }

        void OnItemChanged(object sender, PropertyChangedEventArgs e) {
            ItemChanged?.Invoke((T)sender, e.PropertyName);
        }

        public bool Contains(T value) {
            return list.Contains(value);
        }

        int IList.Add(object value) {
            if(!(value is T))
                throw new ArgumentException();
            Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value) {
            if (!(value is T))
                throw new ArgumentException();
            return list.Contains((T)value);
        }

        public void Clear() {
            if(supportsitemchanged)
                foreach(T item in list)
                    ((INotifyPropertyChanged)item).PropertyChanged -= OnItemChanged;
                
            list.Clear();
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public int IndexOf(T value) {
            return list.IndexOf(value);
        }

        int IList.IndexOf(object value) {
            if (!(value is T))
                throw new ArgumentException();
            return IndexOf((T)value);
        }

        public void Insert(int index, T value) {
            list.Insert(index, value);

            if (supportsitemchanged)
                ((INotifyPropertyChanged)value).PropertyChanged += OnItemChanged;

            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        void IList.Insert(int index, object value) {
            if (!(value is T))
                throw new ArgumentException();
            Insert(index, (T)value);
        }

        public void Remove(T value) {
            int index = IndexOf(value);
            if(index > -1)
                RemoveAt(index);
        }

        void IList.Remove(object value) {
            if (!(value is T))
                throw new ArgumentException();
            Remove((T)value);
        }

        /// <summary>
        /// removes all of the specified items
        /// </summary>
        /// <param name="items">items to remove</param>
        public void RemoveAll(T[] items) {
            foreach(T item in items) {
                if(list.Remove(item))
                    ItemRemoved?.Invoke(item);
            }
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// removes all items where the predicate matches
        /// </summary>
        /// <param name="predicate">predicate to match</param>
        public void RemoveWhere(Func<T, bool> predicate) {
            for(int i = list.Count - 1; i >= 0; --i)
                if(predicate(list[i]))
                    RemoveAt(i);
        }

        /// <summary>
        /// removes an item at the specified index
        /// </summary>
        /// <param name="index">index in list where to remove an item</param>
        public void RemoveAt(int index) {
            T item = list[index];
            if (supportsitemchanged)
                ((INotifyPropertyChanged)item).PropertyChanged -= OnItemChanged;

            list.RemoveAt(index);
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            ItemRemoved?.Invoke(item);
        }

        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                Insert(index, value);
            }
        }

        object IList.this[int index]
        {
            get { return list[index]; }
            set
            {
                if (!(value is T))
                    throw new ArgumentException();
                this[index] = (T)value;
            }
        }

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        object IBindingList.AddNew() {
            object value = Activator.CreateInstance<T>();
            Add((T)value);
            return value;
        }

        void IBindingList.AddIndex(PropertyDescriptor property) {
            throw new NotImplementedException();
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
            throw new NotImplementedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key) {
            throw new NotImplementedException();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property) {
            throw new NotImplementedException();
        }

        void IBindingList.RemoveSort() {
            throw new NotImplementedException();
        }

        bool IBindingList.AllowNew => true;
        bool IBindingList.AllowEdit => true;
        bool IBindingList.AllowRemove => true;
        bool IBindingList.SupportsChangeNotification => true;
        bool IBindingList.SupportsSearching => false;
        bool IBindingList.SupportsSorting => false;
        bool IBindingList.IsSorted => false;
        PropertyDescriptor IBindingList.SortProperty { get; }
        ListSortDirection IBindingList.SortDirection { get; }

        /// <summary>
        /// triggered when the list was changed
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// triggered when an item was changed
        /// </summary>
        public event Action<T, string> ItemChanged;

        /// <summary>
        /// triggered when item was removed
        /// </summary>
        public event Action<T> ItemRemoved;
    }
}