using System;

namespace NightlyCode.Core.Serialization
{
	/// <summary>
	/// a class which converts between several types and byte representations
	/// </summary>
	public static class BinaryConverter
	{
		/// <summary>
		/// get the byte representation of a boolean
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public static void GetBytes(bool value, byte[] buffer, int offset)
		{
			buffer[offset] = (value ? (byte)1 : (byte)0);
		}
		/// <summary>
		/// get the byte representation of a byte ( :) )
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public static void GetBytes(byte value, byte[] buffer, int offset)
		{
			buffer[offset] = value;
		}
		/// <summary>
		/// get the byte representation of a short integer
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(short value, byte[] buffer, int offset)
		{
			fixed (byte* ptr = &buffer[offset])
			{
				if (BitConverter.IsLittleEndian)
				{
					*(short*)ptr = value;
				}
				else
				{
					*ptr = ((byte*)(&value))[1];
					ptr[1] = *(byte*)(&value);
				}
			}
		}
		/// <summary>
		/// get the byte representation of an integer
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(int value, byte[] buffer, int offset)
		{
			fixed (byte* ptr = &buffer[offset])
			{
				if (BitConverter.IsLittleEndian)
				{
					*(int*)ptr = value;
				}
				else
				{
					*ptr = ((byte*)(&value))[3];
					ptr[1] = *((byte*)(&value) + 2);
					ptr[2] = *((byte*)(&value) + 1);
					ptr[3] = *(byte*)(&value);
				}
			}
		}
		/// <summary>
		/// get the byte representation of a long integer
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(long value, byte[] buffer, int offset)
		{
			fixed (byte* ptr = &buffer[offset])
			{
				if (BitConverter.IsLittleEndian)
				{
					*(long*)ptr = value;
				}
				else
				{
					*ptr = ((byte*)(&value))[7];
					ptr[1] = *((byte*)(&value) + 6);
					ptr[2] = *((byte*)(&value) + 5);
					ptr[3] = *((byte*)(&value) + 4);
					ptr[4] = *((byte*)(&value) + 3);
					ptr[5] = *((byte*)(&value) + 2);
					ptr[6] = *((byte*)(&value) + 1);
					ptr[7] = *(byte*)(&value);
				}
			}
		}
		/// <summary>
		/// get the byte representation of a float
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(float value, byte[] buffer, int offset)
		{
			fixed (byte* ptr = &buffer[offset])
			{
				if (BitConverter.IsLittleEndian)
				{
					*(float*)ptr = value;
				}
				else
				{
					*ptr = ((byte*)(&value))[3];
					ptr[1] = *((byte*)(&value) + 2);
					ptr[2] = *((byte*)(&value) + 1);
					ptr[3] = *(byte*)(&value);
				}
			}
		}
		/// <summary>
		/// get the byte representation of a double
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(double value, byte[] buffer, int offset)
		{
			fixed (byte* ptr = &buffer[offset])
			{
				if (BitConverter.IsLittleEndian)
				{
					*(double*)ptr = value;
				}
				else
				{
					*ptr = ((byte*)(&value))[7];
					ptr[1] = *((byte*)(&value) + 6);
					ptr[2] = *((byte*)(&value) + 5);
					ptr[3] = *((byte*)(&value) + 4);
					ptr[4] = *((byte*)(&value) + 3);
					ptr[5] = *((byte*)(&value) + 2);
					ptr[6] = *((byte*)(&value) + 1);
					ptr[7] = *(byte*)(&value);
				}
			}
		}
		/// <summary>
		/// get the byte representation of a string
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(string value, byte[] buffer, int offset)
		{
			int length = value.Length;
			fixed (char* src = value)
			{
				BinaryConverter.GetBytes(length, buffer, offset);
				if (length > 0)
				{
					fixed (byte* ptr = &buffer[offset + 4])
					{
						BinaryConverter.DuffsDeviceUTF8(src, ptr, length);
					}
				}
			}
		}
		/// <summary>
		/// get the byte representation of a byte array
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static void GetBytes(byte[] value, byte[] buffer, int offset)
		{
			int num = value.Length;
			fixed (byte* ptr = value)
			{
				BinaryConverter.GetBytes(num, buffer, offset);
				if (num > 0)
				{
					fixed (byte* ptr2 = &buffer[offset + 4])
					{
						BinaryConverter.DuffsDevice(ptr, ptr2, num);
					}
				}
			}
		}
		/// <summary>
		/// get the boolean represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public static bool GetBoolean(byte[] buffer, int offset)
		{
			return buffer[offset] != 0;
		}
		/// <summary>
		/// get the byte represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public static byte GetByte(byte[] buffer, int offset)
		{
			return buffer[offset];
		}
		/// <summary>
		/// get the short integer represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static short GetShort(byte[] buffer, int offset)
		{
			short result;
            fixed(byte* p = &buffer[offset]) {
                if(BitConverter.IsLittleEndian) {
                    result = *(short*)p;
                }
                else {
                    short num;
                    *(byte*)(&num) = *p;
                    ((byte*)(&num))[1] = *(p + 1);
                    result = num;
                }
            }
			return result;
		}

		/// <summary>
		/// get the integer represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static int GetInt(byte[] buffer, int offset)
		{
			int result;

            fixed(byte* p = &buffer[offset]) {
                if(BitConverter.IsLittleEndian) {
                    result = *(int*)p;
                }
                else {
                    int num;
                    ((byte*)(&num))[3] = *p;
                    ((byte*)(&num))[2] = *(p + 1);
                    ((byte*)(&num))[1] = *(p + 2);
                    *(byte*)(&num) = *(p + 3);
                    result = num;
                }
            }
			return result;
		}
		/// <summary>
		/// get the long integer represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static long GetLong(byte[] buffer, int offset)
		{
			long result;
            fixed(byte* p = &buffer[offset]) {
                if(BitConverter.IsLittleEndian) {
                    result = *(long*)p;
                }
                else {
                    long num;
                    ((byte*)(&num))[7] = *p;
                    ((byte*)(&num))[6] = *(p + 1);
                    ((byte*)(&num))[5] = *(p + 2);
                    ((byte*)(&num))[4] = *(p + 3);
                    ((byte*)(&num))[3] = *(p + 4);
                    ((byte*)(&num))[2] = *(p + 5);
                    ((byte*)(&num))[1] = *(p + 6);
                    *(byte*)(&num) = *(p + 7);
                    result = num;
                }
            }
			return result;
		}
		/// <summary>
		/// get the float represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static float GetFloat(byte[] buffer, int offset)
		{
			float result;
            fixed(byte* p = &buffer[offset]) {
                if(BitConverter.IsLittleEndian) {
                    result = *(float*)p;
                }
                else {
                    float num;
                    ((byte*)(&num))[3] = *p;
                    ((byte*)(&num))[2] = *(p + 1);
                    ((byte*)(&num))[1] = *(p + 2);
                    *(byte*)(&num) = *(p + 3);
                    result = num;
                }
            }
			return result;
		}

		/// <summary>
		/// get the double represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static double GetDouble(byte[] buffer, int offset)
		{
			double result;
            fixed(byte* p = &buffer[offset]) {

                if(BitConverter.IsLittleEndian) {
                    result = *(double*)p;
                }
                else {
                    double num;
                    ((byte*)(&num))[7] = *p;
                    ((byte*)(&num))[6] = *(p + 1);
                    ((byte*)(&num))[5] = *(p + 2);
                    ((byte*)(&num))[4] = *(p + 3);
                    ((byte*)(&num))[3] = *(p + 4);
                    ((byte*)(&num))[2] = *(p + 5);
                    ((byte*)(&num))[1] = *(p + 6);
                    *(byte*)(&num) = *(p + 7);
                    result = num;
                }
            }
			return result;
		}

		/// <summary>
		/// get the string represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static string GetString(byte[] buffer, int offset)
		{
			int @int = BinaryConverter.GetInt(buffer, offset);
			if (@int == 0)
			{
				return "";
			}
            fixed(byte* p = &buffer[offset])
                return new string((sbyte*)(p + 4), 0, @int);
		}

		/// <summary>
		/// get the length of a string when converted to a byte array
		/// </summary>
		/// <param name="value">string to be analysed</param>
		/// <returns>
		/// length of the string in byte representation
		/// </returns>
		public static int GetStringLength(string value)
		{
			return value.Length + 4;
		}

		/// <summary>
		/// get the byte array represented by a byte order
		/// </summary>
		/// <param name="buffer">buffer in which to write the result</param>
		/// <param name="offset">offset where the result is to be written</param>
		public unsafe static byte[] GetByteArray(byte[] buffer, int offset)
		{
			int @int = BinaryConverter.GetInt(buffer, offset);
			byte[] array = new byte[@int];
			if (@int > 0)
			{
				fixed (byte* ptr = array)
				{
					fixed (byte* ptr2 = &buffer[offset + 4])
					{
						BinaryConverter.DuffsDevice(ptr2, ptr, @int);
					}
				}
			}
			return array;
		}

		private unsafe static void DuffsDevice(byte* src, byte* dst, int length)
		{
			int num = length & 7;
			length >>= 3;
			long* ptr = (long*)src;
			long* ptr2 = (long*)dst;
			if (length > 0)
			{
				switch (length & 7)
				{
					case 1:
					{
						goto IL_9E;
					}
					case 2:
					{
						goto IL_90;
					}
					case 3:
					{
						goto IL_82;
					}
					case 4:
					{
						goto IL_74;
					}
					case 5:
					{
						goto IL_66;
					}
					case 6:
					{
						goto IL_58;
					}
					case 7:
					{
						goto IL_4A;
					}
				}
				IL_3C:
				long* expr_3D = ptr2;
				ptr2 = expr_3D + 8;
				long* expr_43 = ptr;
				ptr = expr_43 + 8;
				*expr_3D = *expr_43;
				IL_4A:
				long* expr_4B = ptr2;
				ptr2 = expr_4B + 8;
				long* expr_51 = ptr;
				ptr = expr_51 + 8;
				*expr_4B = *expr_51;
				IL_58:
				long* expr_59 = ptr2;
				ptr2 = expr_59 + 8;
				long* expr_5F = ptr;
				ptr = expr_5F + 8;
				*expr_59 = *expr_5F;
				IL_66:
				long* expr_67 = ptr2;
				ptr2 = expr_67 + 8;
				long* expr_6D = ptr;
				ptr = expr_6D + 8;
				*expr_67 = *expr_6D;
				IL_74:
				long* expr_75 = ptr2;
				ptr2 = expr_75 + 8;
				long* expr_7B = ptr;
				ptr = expr_7B + 8;
				*expr_75 = *expr_7B;
				IL_82:
				long* expr_83 = ptr2;
				ptr2 = expr_83 + 8;
				long* expr_89 = ptr;
				ptr = expr_89 + 8;
				*expr_83 = *expr_89;
				IL_90:
				long* expr_91 = ptr2;
				ptr2 = expr_91 + 8;
				long* expr_97 = ptr;
				ptr = expr_97 + 8;
				*expr_91 = *expr_97;
				IL_9E:
				long* expr_9F = ptr2;
				ptr2 = expr_9F + 8;
				long* expr_A5 = ptr;
				ptr = expr_A5 + 8;
				*expr_9F = *expr_A5;
				if ((length -= 8) > 0)
				{
					goto IL_3C;
				}
			}
			if (num != 0)
			{
				src = (byte*)ptr;
				dst = (byte*)ptr2;
				while (num-- > 0)
				{
					byte* expr_C1 = dst;
					dst = expr_C1 + 1;
					byte* expr_C8 = src;
					src = expr_C8 + 1;
					*expr_C1 = *expr_C8;
				}
			}
		}
		private unsafe static void DuffsDeviceUTF8(char* src, byte* dst, int length)
		{
			switch (length & 7)
			{
				case 1:
				{
					goto IL_9F;
				}
				case 2:
				{
					goto IL_8E;
				}
				case 3:
				{
					goto IL_7D;
				}
				case 4:
				{
					goto IL_6C;
				}
				case 5:
				{
					goto IL_5B;
				}
				case 6:
				{
					goto IL_4A;
				}
				case 7:
				{
					goto IL_39;
				}
			}
            IL_28:
            *dst++ = (byte)(*(ushort*)src++);
			IL_39:
            *dst++ = (byte)(*(ushort*)src++);
            IL_4A:
            *dst++ = (byte)(*(ushort*)src++);
            IL_5B:
            *dst++ = (byte)(*(ushort*)src++);
IL_6C:
            *dst++ = (byte)(*(ushort*)src++);
IL_7D:
            *dst++ = (byte)(*(ushort*)src++);
IL_8E:
            *dst++ = (byte)(*(ushort*)src++);
IL_9F:
            *dst++ = (byte)(*(ushort*)src++); 

            if((length -= 8) <= 0)
			{
				return;
			}
			goto IL_28;
		}

		private unsafe static void DuffsDeviceUTF8(byte* src, char* dst, int length)
		{
			switch (length & 7)
			{
				case 1:
				{
					goto IL_98;
				}
				case 2:
				{
					goto IL_88;
				}
				case 3:
				{
					goto IL_78;
				}
				case 4:
				{
					goto IL_68;
				}
				case 5:
				{
					goto IL_58;
				}
				case 6:
				{
					goto IL_48;
				}
				case 7:
				{
					goto IL_38;
				}
			}
IL_28:
            *dst++ = (char)(*src++);
IL_38:
            *dst++ = (char)(*src++);
IL_48:
            *dst++ = (char)(*src++);
IL_58:
            *dst++ = (char)(*src++);
IL_68:
            *dst++ = (char)(*src++);
IL_78:
            *dst++ = (char)(*src++);
IL_88:
            *dst++ = (char)(*src++);
IL_98:
            *dst++ = (char)(*src++);

            if((length -= 8) <= 0)
			{
				return;
			}
			goto IL_28;
		}
	}
}
