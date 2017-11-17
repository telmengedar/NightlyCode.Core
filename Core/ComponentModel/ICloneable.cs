using System;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// clones an object as deep as needed to use it independently
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<T> : ICloneable {

        /// <summary>
        /// clones the object
        /// </summary>
        /// <returns></returns>
        new T Clone();
    }
}