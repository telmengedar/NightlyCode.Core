using System.Xml;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// interface for custom xml serialization
    /// </summary>
    /// <remarks>
    /// used when class is not designed to be deserialized using automatic property serialization
    /// </remarks>
    public interface IXMLSerializable {

        /// <summary>
        /// deserializes type from node
        /// </summary>
        /// <param name="node"></param>
        void Deserialize(XmlNode node);

        /// <summary>
        /// serializes type to a node
        /// </summary>
        /// <param name="node"></param>
        void Serialize(XmlNode node);
    }
}