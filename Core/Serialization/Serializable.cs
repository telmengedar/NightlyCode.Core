using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using NightlyCode.Core.Collections;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// basic implementation for serializable types
    /// </summary>
    public abstract class Serializable : ISerializable {
        static readonly TypeHandlerLookup<Func<byte[], int, int, object>> datareaders = new TypeHandlerLookup<Func<byte[], int, int, object>>();
        static readonly TypeHandlerLookup<Action<byte[], int, int, object>> datawriters = new TypeHandlerLookup<Action<byte[], int, int, object>>();
        static readonly TypeHandlerLookup<int> datasizes = new TypeHandlerLookup<int>(); 

        /// <summary>
        /// initializes all static members
        /// </summary>
        static Serializable() {

            datareaders[typeof(bool)] = (bytes, offset, size) => bytes[offset] != 0;
            datareaders[typeof(byte)] = (bytes, offset, size) => bytes[offset];
            datareaders[typeof(sbyte)] = (bytes, offset, size) => bytes[offset];
            datareaders[typeof(short)] = (bytes, offset, size) => BitConverter.ToInt16(bytes, offset);
            datareaders[typeof(int)] = (bytes, offset, size) => BitConverter.ToInt32(bytes, offset);
            datareaders[typeof(long)] = (bytes, offset, size) => BitConverter.ToInt64(bytes, offset);
            datareaders[typeof(ushort)] = (bytes, offset, size) => BitConverter.ToUInt16(bytes, offset);
            datareaders[typeof(uint)] = (bytes, offset, size) => BitConverter.ToUInt32(bytes, offset);
            datareaders[typeof(ulong)] = (bytes, offset, size) => BitConverter.ToUInt64(bytes, offset);
            datareaders[typeof(float)] = (bytes, offset, size) => BitConverter.ToSingle(bytes, offset);
            datareaders[typeof(IPAddress)] = (bytes, offset, size) => new IPAddress(bytes.Skip(offset).Take(4).ToArray());
            datareaders[typeof(byte[])] = ReadByteArray;

            datawriters[typeof(bool)] = (bytes, offset, size, value) => bytes[offset] = (byte)(((bool)value) ? 0x01 : 0x00);
            datawriters[typeof(byte)] = (bytes, offset, size, value) => bytes[offset] = (byte)value;
            datawriters[typeof(sbyte)] = (bytes, offset, size, value) => bytes[offset] = (byte)value;
            datawriters[typeof(short)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((short)value), 0, bytes, offset, 2);
            datawriters[typeof(int)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((int)value), 0, bytes, offset, 4);
            datawriters[typeof(long)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((long)value), 0, bytes, offset, 8);
            datawriters[typeof(ushort)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((ushort)value), 0, bytes, offset, 2);
            datawriters[typeof(uint)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((uint)value), 0, bytes, offset, 4);
            datawriters[typeof(ulong)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((ulong)value), 0, bytes, offset, 8);
            datawriters[typeof(float)] = (bytes, offset, size, value) => Array.Copy(BitConverter.GetBytes((float)value), 0, bytes, offset, 4);
            datawriters[typeof(IPAddress)] = (bytes, offset, size, value) => Array.Copy(((IPAddress)value).GetAddressBytes(), 0, bytes, offset, 4);
            datawriters[typeof(byte[])] = WriteByteArray;
            datawriters[typeof(ISerializable)] = (data, offset, size, o) => ((ISerializable)o).Serialize(data, offset);
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

        static object ReadArray(Type elementtype, byte[] bytes, int offset, int size) {
            int elementsize;
            if(datasizes.ContainsKey(elementtype))
                elementsize = datasizes[elementtype];
            else {
                elementsize = SizeAttribute.GetSize(elementtype);
                if(size == -1)
                    throw new InvalidOperationException("Custom type need size specification to be used with arrays.");
            }

            if(size==-1)
                size = (bytes.Length - offset) / elementsize;

            Array array = Array.CreateInstance(elementtype, size);
            if(typeof(ISerializable).IsAssignableFrom(elementtype)) {
                
                for(int i = 0; i < array.Length; ++i) {
                    object element = ReadSerializable(elementtype, bytes, offset);
                    array.SetValue(element, i);
                    offset += elementsize;
                }                
            }
            else {
                for(int i = 0; i < array.Length; ++i) {
                    object element = datareaders[elementtype](bytes, offset, elementsize);
                    array.SetValue(element, i);
                    offset += elementsize;
                }                
            }
            return array;
        }

        static void WriteArray(byte[] bytes, int offset, int size, object value) {
            if(value == null)
                return;

            int elementsize = SizeAttribute.GetSize(value.GetType().GetElementType());
            Array array = (Array)value;
            for(int i = 0; i < array.Length; ++i)
                datawriters[value.GetType().GetElementType()](bytes, offset + elementsize * i, -1, array.GetValue(i));
        }

        static object ReadSerializable(Type type, byte[] data, int offset) {
            ISerializable serializable = (ISerializable)Activator.CreateInstance(type.IsArray ? type.GetElementType() : type, true);
            serializable.Deserialize(data, offset);
            return serializable;
        }

        static void WriteString(StringEncoding encoding, byte[] bytes, int offset, int size, object value) {
            if(size == -1)
                size = ((string)value).Length;

            if (value == null)
                return;

            Encoding enc = encoding == StringEncoding.ASCII ? Encoding.ASCII : Encoding.Unicode;
            enc.GetBytes((string)value, 0, Math.Min(((string)value).Length, size), bytes, offset);
        }

        static void WriteByteArray(byte[] bytes, int offset, int size, object value)
        {
            if (size == -1)
                throw new InvalidOperationException("Must specify size for byte arrays using SizeAttribute");

            if (value == null)
                return;

            Array.Copy((byte[])value, 0, bytes, offset, Math.Min(size, ((byte[])value).Length));
        }

        static object ReadString(StringEncoding encoding, byte[] bytes, int offset, int size)
        {
            if(size == -1) {
                size = bytes.Length - offset;
                if(encoding == StringEncoding.Unicode)
                    size /= 2;
            }

            Encoding enc = encoding == StringEncoding.ASCII ? Encoding.ASCII : Encoding.Unicode;
            byte[] stringdata = new byte[(size + 1) * (encoding == StringEncoding.ASCII ? 1 : 2)];
            Array.Copy(bytes, offset, stringdata, 0, size * (encoding == StringEncoding.ASCII ? 1 : 2));
            string decoded = enc.GetString(stringdata);
            int indexof = decoded.IndexOf('\0');
            if (indexof > -1)
                decoded = decoded.Substring(0, indexof);
            return decoded;
        }

        static object ReadByteArray(byte[] bytes, int offset, int size)
        {
            if (size == -1)
                throw new InvalidOperationException("Must specify size for byte arrays using SizeAttribute");

            int length = Math.Min(size, bytes.Length - offset);
            byte[] data = new byte[length];
            Array.Copy(bytes, offset, data, 0, length);
            return data;
        }

        /// <summary>
        /// serializes the data to a buffer
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public void Serialize(byte[] data, int offset) {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                OffsetAttribute propertyoffset = OffsetAttribute.Get(property);
                if (propertyoffset == null)
                    continue;

                object value = property.GetValue(this, null);
                // no value means nothing to serialize
                if(value == null)
                    continue;

                Type type = property.PropertyType.IsEnum ? Enum.GetUnderlyingType(property.PropertyType) : property.PropertyType;

                if(value is string) {
                    WriteString(EncodingAttribute.GetEncoding(property), data, propertyoffset.Offset + offset, SizeAttribute.GetSize(property), value);
                }
                else {                
                    EndianessAttribute endianess = EndianessAttribute.Get(property);
                    if(endianess != null && endianess.Endianess == Endianess.BigEndian) {
                        byte[] buffer = new byte[datasizes[type]];
                        datawriters[type](buffer, 0, SizeAttribute.GetSize(property),value);
                        Array.Reverse(buffer);
                        Array.Copy(buffer, 0, data, propertyoffset.Offset + offset, buffer.Length);
                    }
                    else datawriters[type](data, propertyoffset.Offset + offset, SizeAttribute.GetSize(property), value);
                }
            }
        }

        /// <summary>
        /// deserializes the type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public void Deserialize(byte[] data, int offset) {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                // skip properties without setters
                if(!property.CanWrite)
                    continue;

                OffsetAttribute propertyoffset = OffsetAttribute.Get(property);
                if (propertyoffset == null)
                    continue;

                Type type = property.PropertyType.IsEnum ? Enum.GetUnderlyingType(property.PropertyType) : property.PropertyType;

                object value;
                if(typeof(ISerializable).IsAssignableFrom(type)) {
                    value = ReadSerializable(property.PropertyType, data, propertyoffset.Offset + offset);
                }
                else if(type==typeof(string)) {
                    value = ReadString(EncodingAttribute.GetEncoding(property), data, propertyoffset.Offset + offset, SizeAttribute.GetSize(property));
                }
                else if(type.IsArray) {
                    value = ReadArray(property.PropertyType.GetElementType(), data, propertyoffset.Offset + offset, SizeAttribute.GetSize(property));
                }
                else {
                    
                    int size = datasizes[type];
                    EndianessAttribute endianess = EndianessAttribute.Get(property);
                    if(endianess != null && endianess.Endianess == Endianess.BigEndian) {
                        byte[] buffer = new byte[size];
                        for(int i = 0; i < size; ++i)
                            buffer[size - 1 - i] = data[propertyoffset.Offset + offset + i];
                        value = datareaders[type](buffer, 0, SizeAttribute.GetSize(property));
                    }
                    else value = datareaders[type](data, propertyoffset.Offset + offset, SizeAttribute.GetSize(property));                    
                }

                property.SetValue(this, value, null);
            }
        }
    }
}