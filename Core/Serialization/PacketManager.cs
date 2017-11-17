using System;
namespace GM.Core.Serialization
{
	public class PacketManager
	{
		public static PacketDataType PacketType;
		public static PacketPath Read(byte[] data)
		{
			if (data.Length <= 0)
			{
				return null;
			}
			switch (data[0])
			{
				case 0:
				{
					return PacketConverterBinary.Read(data);
				}
				case 1:
				{
					return PacketConverterXml.Read(data);
				}
				default:
				{
					throw new ArgumentException("unknown packet type");
				}
			}
		}
		public static byte[] Write(PacketPath packet)
		{
			switch (PacketManager.PacketType)
			{
				case PacketDataType.Binary:
				{
					return PacketConverterBinary.Write(packet);
				}
				case PacketDataType.Xml:
				{
					return PacketConverterXml.Write(packet);
				}
				default:
				{
					throw new ArgumentException("unknown packet type");
				}
			}
		}
	}
}
