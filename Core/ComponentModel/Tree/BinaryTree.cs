using System.Collections.Generic;

namespace NightlyCode.Core.ComponentModel
{
    public abstract class BinaryTree<TKey, TItem>
    {
        BinaryNode rootnode;
        int maxitemspernode;

        public BinaryTree(int maxitemspernode) {
            this.maxitemspernode = maxitemspernode;
        }

        public void Create(ICollection<TItem> items)
        {
            if (items.Count <= maxitemspernode) rootnode = new BinaryLeaf<TItem>(GetBounds(items), items);
            else { 
            }
        }

        protected abstract IBinaryBounds GetBounds(ICollection<TItem> items);
    }
}
