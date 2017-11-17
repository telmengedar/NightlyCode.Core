using System.Collections.Generic;

namespace NightlyCode.Core.ComponentModel
{
    public class BinaryLeaf<TItem> : BinaryNode
    {
        ICollection<TItem> items;

        public BinaryLeaf(IBinaryBounds bounds, ICollection<TItem> items)
            : base(bounds)
        {
            this.items = items;
        }

        public ICollection<TItem> Items
        {
            get { return items; }
        }
    }
}
