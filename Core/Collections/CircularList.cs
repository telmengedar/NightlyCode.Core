using System.Collections.Generic;

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// list which provides circular access to items
    /// </summary>
    /// <typeparam name="T">type of item to be added</typeparam>
    public class CircularList<T> {
        readonly object itemlock = new object();
        readonly List<T> list=new List<T>();

        int itemindex = -1;

        /// <summary>
        /// clears all items from list
        /// </summary>
        public void Clear() {
            lock(itemlock)
                list.Clear();
        }

        /// <summary>
        /// adds an item to the list
        /// </summary>
        /// <param name="item">item to be added</param>
        public void Add(T item) {
            lock(itemlock)
                list.Add(item);
        }

        /// <summary>
        /// adds several items to the list
        /// </summary>
        /// <param name="items">items to be added</param>
        public void AddRange(IEnumerable<T> items) {
            lock(itemlock)
                list.AddRange(items);
        }

        /// <summary>
        /// removes an item from list
        /// </summary>
        /// <param name="item">item to be removed</param>
        public void Remove(T item) {
            lock(itemlock)
                list.Remove(item);
        }

        /// <summary>
        /// removes the item at the specified index
        /// </summary>
        /// <param name="index">index at which to remove the item</param>
        public void RemoveAt(int index) {
            lock(itemlock)
                list.RemoveAt(index);
        }

        /// <summary>
        /// get next item in list
        /// </summary>
        public T NextItem
        {
            get
            {
                lock(itemlock) {
                    if(list.Count == 0)
                        return default(T);

                    itemindex = (itemindex + 1) % list.Count;
                    return list[itemindex];
                }
            }
        }
    }
}