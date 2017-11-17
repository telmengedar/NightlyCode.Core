using System;
using System.Threading;
namespace GM.Core.Serialization
{
	public abstract class PacketNode
	{
		private static long idserial;
		internal long nodeid;
		private string name;
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public virtual int Size
		{
			get
			{
				if (string.IsNullOrEmpty(this.name))
				{
					return 4;
				}
				return this.name.Length + 4;
			}
		}
		public PacketNode(string name)
		{
			this.name = name;
			this.nodeid = Interlocked.Increment(ref PacketNode.idserial);
		}
		public abstract PacketNode Clone();
		public override string ToString()
		{
			return string.Format("<{0} />", this.Name);
		}
	}
}
