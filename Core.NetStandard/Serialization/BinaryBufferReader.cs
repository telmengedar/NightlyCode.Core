using System;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// reader for a binary buffer
    /// </summary>
    public class BinaryBufferReader {
        int offset = 0;
        byte[] buffer;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="buffer"></param>
        public BinaryBufferReader(byte[] buffer) {
            this.buffer = buffer;
        }

        /// <summary>
        /// read offset
        /// </summary>
        public int Offset {
            get { return offset; }
            set { offset = value; }
        }

        public byte ReadByte() {
            return buffer[offset++];
        }

        public short ReadShort() {
            short value = BinaryConverter.GetShort(buffer, offset);
            offset += 2;
            return value;
        }

        public int ReadInt() {
            int value = BinaryConverter.GetInt(buffer, offset);
            offset += 4;
            return value;
        }

        public long ReadLong() {
            long value = BinaryConverter.GetLong(buffer, offset);
            offset += 8;
            return value;
        }

        public Guid ReadGuid() {
            int a = ReadInt();
            short b = ReadShort();
            short c = ReadShort();
            byte d = ReadByte();
            byte e = ReadByte();
            byte f = ReadByte();
            byte g = ReadByte();
            byte h = ReadByte();
            byte i = ReadByte();
            byte j = ReadByte();
            byte k = ReadByte();
            return new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }
    }
}