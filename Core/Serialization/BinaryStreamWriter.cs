using System;
using System.IO;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// writes binary data to a stream
    /// </summary>
    public class BinaryStreamWriter {
        Stream outstream;
        byte[] buffer = new byte[16];

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="outstream"></param>
        public BinaryStreamWriter(Stream outstream) {
            this.outstream = outstream;
        }

        /// <summary>
        /// writes an int64
        /// </summary>
        /// <param name="data"></param>
        public void WriteLong(long data) {
            BinaryConverter.GetBytes(data, buffer, 0);
            outstream.Write(buffer, 0, 8);
        }

        /// <summary>
        /// writes an int32
        /// </summary>
        /// <param name="data"></param>
        public void WriteInt(int data) {
            BinaryConverter.GetBytes(data, buffer, 0);
            outstream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// writes an int16
        /// </summary>
        /// <param name="data"></param>
        public void WriteShort(short data) {
            BinaryConverter.GetBytes(data, buffer, 0);
            outstream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// writes a GUID
        /// </summary>
        /// <param name="data"></param>
        public void WriteGuid(Guid data) {
            outstream.Write(data.ToByteArray(), 0, 16);
        }
    }
}