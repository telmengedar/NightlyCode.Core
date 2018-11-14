namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// interface for a serializable type
    /// </summary>
    public interface ISerializable {

        /// <summary>
        /// serializes the object to the specified array
        /// </summary>
        /// <param name="data">buffer to serialize object to</param>
        /// <param name="offset">offset at which to begin serialization</param>
        void Serialize(byte[] data, int offset);

        /// <summary>
        /// deserializes the object from the specified array
        /// </summary>
        /// <param name="data">buffer from which to deserialize the object</param>
        /// <param name="offset">offset where to start deserialization</param>
        void Deserialize(byte[] data, int offset);
    }
}