using System;
using System.Collections.Generic;
using GM.Core.Serilization;
namespace GM.Core.Serialization
{
	public class PacketConverterBinary
	{
		public static byte[] Write(PacketPath packet)
		{
			int num = 1;
			Stack<PacketPath> stack = new Stack<PacketPath>();
			stack.Push(packet);
			Dictionary<long, int> dictionary = new Dictionary<long, int>();
			Dictionary<long, int> dictionary2 = new Dictionary<long, int>();
			List<PacketNode> list = new List<PacketNode>();
			dictionary[packet.nodeid] = 0;
			int num2 = 0;
			while (stack.Count > 0)
			{
				PacketPath packetPath = stack.Pop();
				foreach (PacketNode current in packetPath)
				{
					list.Add(current);
					dictionary2[current.nodeid] = dictionary[packetPath.nodeid];
					if (current is PacketPath)
					{
						dictionary[current.nodeid] = num++;
						stack.Push((PacketPath)current);
					}
					num2 += current.Size + 5;
				}
			}
			byte[] array = new byte[num2 + 5];
			int num3 = 0;
			array[num3++] = 0;
			BinaryConverter.GetBytes(list.Count, array, num3);
			num3 += 4;
			foreach (PacketNode current2 in list)
			{
				BinaryConverter.GetBytes(dictionary2[current2.nodeid], array, num3);
				num3 += 4;
				BinaryConverter.GetBytes(current2.Name, array, num3);
				num3 += BinaryConverter.GetStringLength(current2.Name);
				BinaryDataTypes binaryDataTypes;
				if (current2 is PacketData)
				{
					binaryDataTypes = ((PacketData)current2).DataType;
				}
				else
				{
					if (!(current2 is PacketPath))
					{
						throw new ArgumentException("Node type not supported");
					}
					binaryDataTypes = BinaryDataTypes.None;
				}
				array[num3++] = (byte)binaryDataTypes;
				switch (binaryDataTypes)
				{
					case BinaryDataTypes.Bool:
					{
						array[num3++] = (((bool)((PacketData)current2).Data) ? (byte)1 : (byte)0);
						break;
					}
					case BinaryDataTypes.Byte:
					{
						array[num3++] = (byte)((PacketData)current2).Data;
						break;
					}
					case BinaryDataTypes.Short:
					{
						BinaryConverter.GetBytes((short)((PacketData)current2).Data, array, num3);
						num3 += 2;
						break;
					}
					case BinaryDataTypes.Int:
					{
						BinaryConverter.GetBytes((int)((PacketData)current2).Data, array, num3);
						num3 += 4;
						break;
					}
					case BinaryDataTypes.Long:
					{
						BinaryConverter.GetBytes((long)((PacketData)current2).Data, array, num3);
						num3 += 8;
						break;
					}
					case BinaryDataTypes.Float:
					{
						BinaryConverter.GetBytes((float)((PacketData)current2).Data, array, num3);
						num3 += 4;
						break;
					}
					case BinaryDataTypes.Double:
					{
						BinaryConverter.GetBytes((double)((PacketData)current2).Data, array, num3);
						num3 += 8;
						break;
					}
					case BinaryDataTypes.String:
					{
						BinaryConverter.GetBytes((string)((PacketData)current2).Data, array, num3);
						num3 += BinaryConverter.GetStringLength((string)((PacketData)current2).Data);
						break;
					}
					case BinaryDataTypes.DateTime:
					{
						BinaryConverter.GetBytes(((DateTime)((PacketData)current2).Data).Ticks, array, num3);
						num3 += 8;
						break;
					}
					case BinaryDataTypes.ByteArray:
					{
						BinaryConverter.GetBytes((byte[])((PacketData)current2).Data, array, num3);
						num3 += ((byte[])((PacketData)current2).Data).Length + 4;
						break;
					}
				}
			}
			return array;
		}

		public static PacketPath Read(byte[] packetdata)
		{
			int num = 1;
			int num2 = 1;
			Dictionary<int, PacketPath> dictionary = new Dictionary<int, PacketPath>();
			PacketPath packetPath = new PacketPath("Packet", new PacketNode[0]);
			dictionary[0] = packetPath;
			int @int = BinaryConverter.GetInt(packetdata, num);
			num += 4;
			for (int i = 0; i < @int; i++)
			{
				int int2 = BinaryConverter.GetInt(packetdata, num);
				num += 4;
				string @string = BinaryConverter.GetString(packetdata, num);
				num += BinaryConverter.GetStringLength(@string);
				BinaryDataTypes binaryDataTypes = (BinaryDataTypes)packetdata[num++];
				PacketNode packetNode;
				if (binaryDataTypes != BinaryDataTypes.None)
				{
					object obj;
					switch (binaryDataTypes)
					{
						case BinaryDataTypes.None:
						case BinaryDataTypes.Null:
						case BinaryDataTypes.Object:
						{
							goto IL_191;
						}
						case BinaryDataTypes.Bool:
						{
							obj = (packetdata[num++] != 0);
							break;
						}
						case BinaryDataTypes.Byte:
						{
							obj = packetdata[num++];
							break;
						}
						case BinaryDataTypes.Short:
						{
							obj = BinaryConverter.GetShort(packetdata, num);
							num += 2;
							break;
						}
						case BinaryDataTypes.Int:
						{
							obj = BinaryConverter.GetInt(packetdata, num);
							num += 4;
							break;
						}
						case BinaryDataTypes.Long:
						{
							obj = BinaryConverter.GetLong(packetdata, num);
							num += 8;
							break;
						}
						case BinaryDataTypes.Float:
						{
							obj = BinaryConverter.GetFloat(packetdata, num);
							num += 4;
							break;
						}
						case BinaryDataTypes.Double:
						{
							obj = BinaryConverter.GetDouble(packetdata, num);
							num += 8;
							break;
						}
						case BinaryDataTypes.String:
						{
							int int3 = BinaryConverter.GetInt(packetdata, num);
							obj = BinaryConverter.GetString(packetdata, num);
							num += int3 + 4;
							break;
						}
						case BinaryDataTypes.DateTime:
						{
							obj = new DateTime(BinaryConverter.GetLong(packetdata, num));
							num += 8;
							break;
						}
						case BinaryDataTypes.ByteArray:
						{
							obj = BinaryConverter.GetByteArray(packetdata, num);
							num += ((byte[])obj).Length + 4;
							break;
						}
						default:
						{
							goto IL_191;
						}
					}
					IL_194:
					packetNode = new PacketData(@string, obj);
					goto IL_1C3;
					IL_191:
					obj = null;
					goto IL_194;
				}
				packetNode = new PacketPath(@string, new PacketNode[0]);
				dictionary[num2++] = (PacketPath)packetNode;
				IL_1C3:
				dictionary[int2].Add(packetNode);
			}
			return packetPath;
		}
	}
}
