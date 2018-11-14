using System.IO;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// extensions for binary reader
    /// </summary>
    public static class BinaryReaderExtensions {

        /// <summary>
        /// reads an uint16 in big endian format
        /// </summary>
        /// <param name="reader">reader to use</param>
        /// <returns>uint16</returns>
        public static ushort ReadUInt16BE(this BinaryReader reader) {
            byte b1 = reader.ReadByte();
            return (ushort)((b1 << 8) | reader.ReadByte());
        }

        /// <summary>
        /// reads an int16 in big endian format
        /// </summary>
        /// <param name="reader">reader to use</param>
        /// <returns>int16</returns>
        public static short ReadInt16BE(this BinaryReader reader) {
            byte b1 = reader.ReadByte();
            return (short)((b1 << 8) | reader.ReadByte());
        }
    }
}