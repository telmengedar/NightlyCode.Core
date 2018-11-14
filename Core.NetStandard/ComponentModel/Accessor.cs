using System;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// accessor for data
    /// </summary>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class Accessor<TParam1, TParam2, TData> {
        readonly Action<TParam1, TParam2, TData> setter;
        readonly Func<TParam1, TParam2, TData> getter;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        public Accessor(Action<TParam1, TParam2, TData> setter, Func<TParam1, TParam2, TData> getter) {
            this.setter = setter;
            this.getter = getter;
        }

        /// <summary>
        /// indexer
        /// </summary>
        public TData this[TParam1 param1, TParam2 param2] {
            get { return getter(param1, param2); }
            set { setter(param1, param2, value); }
        }

        /// <summary>
        /// getter
        /// </summary>
        public Func<TParam1, TParam2, TData> Getter { get { return getter; } }

        /// <summary>
        /// setter
        /// </summary>
        public Action<TParam1, TParam2, TData> Setter { get { return setter; } }
    }
}