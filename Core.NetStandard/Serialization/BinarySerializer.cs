using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using NightlyCode.Core.Collections;
using NightlyCode.Core.Conversion;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// serializes objects to binary streams
    /// </summary>
    public static class BinarySerializer {
        static readonly TypeHandlerLookup<Func<BinaryReader, PropertyInfo, object>> datareaders = new TypeHandlerLookup<Func<BinaryReader, PropertyInfo, object>>();
        static readonly TypeHandlerLookup<Action<BinaryWriter, PropertyInfo, object>> datawriters = new TypeHandlerLookup<Action<BinaryWriter, PropertyInfo, object>>();
        static readonly TypeHandlerLookup<int> datasizes = new TypeHandlerLookup<int>();

        /// <summary>
        /// initializes all static members
        /// </summary>
        static BinarySerializer()
        {

            datareaders[typeof(bool)] = (stream,pi) => stream.ReadByte() != 0;
            datareaders[typeof(byte)] = (stream, pi) => stream.ReadByte();
            datareaders[typeof(sbyte)] = (stream, pi) => (sbyte)stream.ReadByte();
            datareaders[typeof(short)] = (stream, pi) => stream.ReadInt16();
            datareaders[typeof(int)] = (stream, pi) => stream.ReadInt32();
            datareaders[typeof(long)] = (stream, pi) => stream.ReadInt64();
            datareaders[typeof(ushort)] = (stream, pi) => stream.ReadUInt16();
            datareaders[typeof(uint)] = (stream, pi) => stream.ReadUInt32();
            datareaders[typeof(ulong)] = (stream, pi) => stream.ReadUInt64();
            datareaders[typeof(float)] = (stream, pi) => stream.ReadSingle();
            datareaders[typeof(IPAddress)] = (stream, pi) => new IPAddress(stream.ReadBytes(4));
            datareaders[typeof(byte[])] = ReadByteArray;

            datawriters[typeof(bool)] = (writer, pi, value) => writer.Write((byte)((bool)value ? 0x01 : 0x00));
            datawriters[typeof(byte)] = (writer, pi, value) => writer.Write((byte)value);
            datawriters[typeof(sbyte)] = (writer, pi, value) => writer.Write((byte)value);
            datawriters[typeof(short)] = (writer, pi, value) => writer.Write((short)value);
            datawriters[typeof(int)] = (writer, pi, value) => writer.Write((int)value);
            datawriters[typeof(long)] = (writer, pi, value) => writer.Write((long)value);
            datawriters[typeof(ushort)] = (writer, pi, value) => writer.Write((ushort)value);
            datawriters[typeof(uint)] = (writer, pi, value) => writer.Write((uint)value);
            datawriters[typeof(ulong)] = (writer, pi, value) => writer.Write((ulong)value);
            datawriters[typeof(float)] = (writer, pi, value) => writer.Write((float)value);
            datawriters[typeof(IPAddress)] = (writer, pi, value) => writer.Write(((IPAddress)value).GetAddressBytes());
            datawriters[typeof(byte[])] = WriteByteArray;
            datawriters[typeof(Array)] = WriteArray;

            datasizes[typeof(bool)] = 1;
            datasizes[typeof(byte)] = 1;
            datasizes[typeof(sbyte)] = 1;
            datasizes[typeof(short)] = 2;
            datasizes[typeof(int)] = 4;
            datasizes[typeof(long)] = 8;
            datasizes[typeof(ushort)] = 2;
            datasizes[typeof(uint)] = 4;
            datasizes[typeof(ulong)] = 8;
            datasizes[typeof(float)] = 4;
            datasizes[typeof(IPAddress)] = 4;
        }

        static object ReadArray(Type elementtype, BinaryReader reader, PropertyInfo info)
        {
            int elementsize = datasizes.ContainsKey(elementtype) ? datasizes[elementtype] : SizeAttribute.GetSize(elementtype);
            int size = (int)((reader.BaseStream.Length-reader.BaseStream.Position) / elementsize);

            Array array = Array.CreateInstance(elementtype, size);
            for(int i = 0; i < array.Length; ++i) {
                object element = datareaders.ContainsKey(elementtype) ? datareaders[elementtype](reader, info) : Deserialize(elementtype, reader);
                array.SetValue(element, i);
            }
            return array;
        }

        static void WriteArray(BinaryWriter writer, PropertyInfo info, object value)
        {
            if (value == null)
                return;

            //int elementsize = SizeAttribute.GetSize(value.GetType().GetElementType());
            Array array = (Array)value;
            for(int i = 0; i < array.Length; ++i) {
                if(datawriters.ContainsKey(value.GetType().GetElementType()))
                    datawriters[value.GetType().GetElementType()](writer, null, array.GetValue(i));
                else Serialize(array.GetValue(i), writer);
            }
        }

        static void WriteString(StringEncoding encoding, BinaryWriter writer, int size, object value)
        {
            if (size == -1)
                size = ((string)value).Length;

            string data = ((string)value).PadRight(size).Substring(0, size);

            Encoding enc = encoding == StringEncoding.ASCII ? Encoding.ASCII : Encoding.Unicode;
            writer.Write(enc.GetBytes(data));
        }

        static void WriteByteArray(BinaryWriter writer, PropertyInfo info, object value) {
            int size = SizeAttribute.GetSize(info);
            if (size == -1)
                throw new InvalidOperationException($"Must specify size for byte arrays using {nameof(SizeAttribute)}");

            if(value == null) {
                writer.Write(new byte[size]);
            }
            else {
                writer.Write((byte[])value);
                int left = size - ((byte[])value).Length;
                if(left > 0)
                    writer.Write(new byte[left]);
            }
        }

        static object ReadString(StringEncoding encoding, BinaryReader reader, int size)
        {
            if(size == -1)
                size = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
            else {
                if(encoding == StringEncoding.Unicode)
                    size <<= 1;
            }
            Encoding enc = encoding == StringEncoding.ASCII ? Encoding.ASCII : Encoding.Unicode;
            return enc.GetString(reader.ReadBytes(size)).TrimEnd(' ');
        }

        static object ReadByteArray(BinaryReader reader, PropertyInfo property) {
            int size = SizeAttribute.GetSize(property);
            if (size == -1)
                throw new InvalidOperationException($"Must specify size for byte arrays using {nameof(SizeAttribute)}");

            return reader.ReadBytes(size);
        }

        /// <summary>
        /// serializes the data to a byte array
        /// </summary>
        /// <param name="data">data to serialize</param>
        /// <returns>serialized data</returns>
        public static byte[] Serialize(object data) {
            using(MemoryStream ms = new MemoryStream()) {
                Serialize(data, ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// serializes the data to a stream
        /// </summary>
        /// <param name="data">data to serialize</param>
        /// <param name="stream">stream to serialize data to</param>
        public static void Serialize(object data, Stream stream) {
            using(BinaryWriter writer = new BinaryWriter(stream))
                Serialize(data, writer);
        }

        /// <summary>
        /// serializes data to a writer
        /// </summary>
        /// <param name="data">data to serialize</param>
        /// <param name="writer">writer to use for serialization</param>
        public static void Serialize(object data, BinaryWriter writer) {
            if (data == null)
                return;

            if(data.GetType().IsArray) {
                WriteArray(writer, null, data);
                return;
            }

            foreach (PropertyInfo property in data.GetType().GetProperties())
            {
                object value = property.GetValue(data, null);

                // no value means nothing to serialize
                if (value == null)
                    continue;

                Type type = property.PropertyType.IsEnum ? Enum.GetUnderlyingType(property.PropertyType) : property.PropertyType;

                if (value is string)
                {
                    WriteString(EncodingAttribute.GetEncoding(property), writer, SizeAttribute.GetSize(property), value);
                }
                else if (value is Enum)
                {
                    Type underlying = Enum.GetUnderlyingType(value.GetType());
                    datawriters[underlying](writer, property, Converter.Convert(value, underlying));
                }
                else if(value is byte[])
                    WriteByteArray(writer, property, value);
                else {
                    if(datawriters.ContainsKey(type))
                        datawriters[type](writer, property, value);
                    else Serialize(value, writer);
                }
            }
        }

        /// <summary>
        /// deserializes an object from stream
        /// </summary>
        /// <typeparam name="T">type of data to deserialize</typeparam>
        /// <param name="data">data from which to deserialize the object</param>
        /// <returns>deserialized object</returns>
        public static T Deserialize<T>(byte[] data, int offset=0) {
            using(MemoryStream ms = new MemoryStream(data))
                return Deserialize<T>(ms, offset);
        }

        /// <summary>
        /// deserializes the type
        /// </summary>
        /// <param name="stream">stream from which to deserialize data</param>
        public static T Deserialize<T>(Stream stream, int offset = 0) {
            return (T)Deserialize(typeof(T), new BinaryReader(stream), offset);
        }

        /// <summary>
        /// deserializes the type
        /// </summary>
        /// <typeparam name="T">type to read</typeparam>
        /// <param name="reader">reader used to read data</param>
        /// <returns>deserializes data</returns>
        public static T Deserialize<T>(BinaryReader reader, int offset = 0) {
            return (T)Deserialize(typeof(T), reader, offset);
        }

        static object Deserialize(Type datatype, BinaryReader reader, int offset = -1) {
            if (offset >= 0)
                reader.BaseStream.Position = offset;

            if(datatype.IsArray)
                return ReadArray(datatype.GetElementType(), reader, null);

            object data = Activator.CreateInstance(datatype);
            foreach (PropertyInfo property in datatype.GetProperties())
            {
                // skip properties without setters
                if (!property.CanWrite)
                    continue;

                Type type = property.PropertyType.IsEnum ? Enum.GetUnderlyingType(property.PropertyType) : property.PropertyType;

                object value;
                if (type == typeof(string))
                {
                    value = ReadString(EncodingAttribute.GetEncoding(property), reader, SizeAttribute.GetSize(property));
                }
                else if (type.IsEnum)
                {
                    value = Converter.Convert(datareaders[Enum.GetUnderlyingType(type)](reader, property), type);
                }
                else if(type == typeof(byte[])) {
                    value = ReadByteArray(reader, property);
                }
                else if (type.IsArray)
                {
                    value = ReadArray(property.PropertyType.GetElementType(), reader, property);
                }
                else {
                    value = datareaders.ContainsKey(type) ? datareaders[type](reader, property) : Deserialize(type, reader);
                }

                property.SetValue(data, value, null);
            }
            return data;
        }
    }
}