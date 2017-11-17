using System;
using System.Collections.Generic;
namespace GM.Core.Serialization
{
	public class PacketData : PacketNode
	{
		private object data;
		private BinaryDataTypes datatype;
		private static Dictionary<Type, BinaryDataTypes> typelookup;
		public object Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
				if (this.data == null)
				{
					this.datatype = BinaryDataTypes.Null;
					return;
				}
				if (!PacketData.typelookup.TryGetValue(this.data.GetType(), out this.datatype))
				{
					this.datatype = BinaryDataTypes.Object;
				}
			}
		}
		public BinaryDataTypes DataType
		{
			get
			{
				return this.datatype;
			}
		}
		public override int Size
		{
			get
			{
				switch (this.datatype)
				{
                    default:
					case BinaryDataTypes.None:
					case BinaryDataTypes.Null:
					{
						return base.Size;
					}
					case BinaryDataTypes.Bool:
					{
						return base.Size + 1;
					}
					case BinaryDataTypes.Byte:
					{
						return base.Size + 1;
					}
					case BinaryDataTypes.Short:
					{
						return base.Size + 2;
					}
					case BinaryDataTypes.Int:
					{
						return base.Size + 4;
					}
					case BinaryDataTypes.Long:
					case BinaryDataTypes.DateTime:
					{
						return base.Size + 8;
					}
					case BinaryDataTypes.Float:
					{
						return base.Size + 4;
					}
					case BinaryDataTypes.Double:
					{
						return base.Size + 8;
					}
					case BinaryDataTypes.String:
					{
						return base.Size + ((string)this.data).Length + 4;
					}
					case BinaryDataTypes.ByteArray:
					{
						return base.Size + ((byte[])this.data).Length + 4;
					}
				}
			}
		}
		static PacketData()
		{
			PacketData.typelookup = new Dictionary<Type, BinaryDataTypes>();
			PacketData.typelookup[typeof(bool)] = BinaryDataTypes.Bool;
			PacketData.typelookup[typeof(byte)] = BinaryDataTypes.Byte;
			PacketData.typelookup[typeof(short)] = BinaryDataTypes.Short;
			PacketData.typelookup[typeof(int)] = BinaryDataTypes.Int;
			PacketData.typelookup[typeof(long)] = BinaryDataTypes.Long;
			PacketData.typelookup[typeof(float)] = BinaryDataTypes.Float;
			PacketData.typelookup[typeof(double)] = BinaryDataTypes.Double;
			PacketData.typelookup[typeof(string)] = BinaryDataTypes.String;
			PacketData.typelookup[typeof(byte[])] = BinaryDataTypes.ByteArray;
			PacketData.typelookup[typeof(DateTime)] = BinaryDataTypes.DateTime;
		}
		public PacketData(string name, object data) : base(name)
		{
			this.Data = data;
		}
		public override PacketNode Clone()
		{
			return new PacketData(base.Name, this.data);
		}
		public override bool Equals(object obj)
		{
			PacketData packetData = obj as PacketData;
			if (packetData == null)
			{
				return false;
			}
			if (packetData.Data == null)
			{
				return this.Data == null;
			}
			return packetData.Data.Equals(this.Data) && packetData.Name == base.Name;
		}
		public override int GetHashCode()
		{
			return base.Name.GetHashCode() ^ ((this.data != null) ? this.data.GetHashCode() : 0);
		}
		public override string ToString()
		{
			return string.Format("<{0} value=\"{1}\" type=\"{2}\" />", base.Name, this.data, this.datatype);
		}
	}
}
