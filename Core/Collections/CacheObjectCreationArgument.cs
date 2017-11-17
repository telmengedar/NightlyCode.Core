using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Core.Collections
{
    public class CacheObjectCreationArgument<TKey, TObject>
    {
        TKey key;
        TObject obj = default(TObject);

        public CacheObjectCreationArgument(TKey key)
        {
            this.key = key;
        }

        public TKey Key
        {
            get { return key; }
        }

        public TObject Object
        {
            get { return obj; }
            set { obj = value; }
        }
    }
}
