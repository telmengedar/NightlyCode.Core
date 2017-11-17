using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GM.Core.Serilization;
namespace GM.Core.Serialization
{
	public class PacketPath : PacketNode, ICollection<PacketNode>, IEnumerable<PacketNode>, IEnumerable
	{
		protected List<PacketNode> nodes;
		protected Dictionary<string, PacketNode> nodelookup = new Dictionary<string, PacketNode>();
		public PacketNode this[string name]
		{
			get
			{
				return this.GetNode(name);
			}
		}
		public PacketNode this[int index]
		{
			get
			{
				if (index >= this.nodes.Count)
				{
					return null;
				}
				return this.nodes[index];
			}
		}
		public int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		public PacketPath(string name, params PacketNode[] nodes) : base(name)
		{
			this.nodes = new List<PacketNode>();
			for (int i = 0; i < nodes.Length; i++)
			{
				PacketNode item = nodes[i];
				this.Add(item);
			}
		}
		public static PacketPath Create(ISerializable data, string pathname)
		{
			PacketPath packetPath = new PacketPath(pathname, new PacketNode[0]);
			if (data != null)
			{
				data.Serialize(packetPath);
			}
			return packetPath;
		}
		public static T ConvertTo<T>(PacketPath data) where T : ISerializable, new()
		{
			if (data == null)
			{
				return default(T);
			}
			T result = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			result.Deserialize(data);
			return result;
		}
		public static PacketPath CreateTyped(ISerializable data, string pathname)
		{
			PacketPath packetPath = new PacketPath(pathname, new PacketNode[0]);
			if (data != null)
			{
				packetPath.Add(new PacketData("Type", data.GetType().AssemblyQualifiedName));
				data.Serialize(packetPath);
			}
			return packetPath;
		}
		public static ISerializable ConvertTyped(PacketPath data)
		{
			if (data == null)
			{
				return null;
			}
			Type type = Type.GetType(data.GetValue<string>("Type"), false);
			if (type == null)
			{
				return null;
			}
			ISerializable packetPathSerializable = Activator.CreateInstance(type) as ISerializable;
			if (packetPathSerializable == null)
			{
				return null;
			}
			packetPathSerializable.Deserialize(data);
			return packetPathSerializable;
		}
		public bool TryGetNode(string name, out PacketNode node)
		{
			node = this.GetNode(name);
			return node != null;
		}
		public PacketNode GetNode(string name)
		{
			PacketNode packetNode;
			if (name.IndexOf('.') > -1)
			{
				string[] array = name.Split(new char[]
				{
					'.'
				});
				int i = 0;
				PacketPath packetPath = this;
				while (i < array.Length)
				{
					if (!packetPath.nodelookup.TryGetValue(array[i], out packetNode))
					{
						return null;
					}
					if (i == array.Length - 1)
					{
						return packetNode;
					}
					if (!(packetNode is PacketPath))
					{
						return null;
					}
					packetPath = (PacketPath)packetNode;
					i++;
				}
				return null;
			}
			this.nodelookup.TryGetValue(name, out packetNode);
			return packetNode;
		}
		public List<PacketNode> GetNodes(string name)
		{
			int num;
			if ((num = name.IndexOf('.')) > -1)
			{
				string fname = name.Substring(0, num);
				string name2 = name.Substring(num + 1);
				List<PacketNode> list = new List<PacketNode>();
				foreach (PacketNode current in this.nodes.FindAll((PacketNode node) => node.Name == fname))
				{
					if (current is PacketPath)
					{
						list.AddRange(((PacketPath)current).GetNodes(name2));
					}
				}
				return list;
			}
			return this.nodes.FindAll((PacketNode node) => node.Name == name);
		}
		public int RemoveAll(string name)
		{
			return this.nodes.RemoveAll((PacketNode n) => n.Name == name);
		}
		public override PacketNode Clone()
		{
			return new PacketPath(base.Name, this.nodes.ToArray());
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("<{0}>", base.Name));
			foreach (PacketNode current in this.nodes)
			{
				stringBuilder.Append(current.ToString());
			}
			stringBuilder.Append(string.Format("</{0}>", base.Name));
			return stringBuilder.ToString();
		}

		public T GetValue<T>(string path)
		{
			PacketData packetData = this.GetNode(path) as PacketData;
			if (packetData == null || packetData.Data == null)
			{
				return default(T);
			}
			if (packetData.Data is T)
			{
				return (T)packetData.Data;
			}
			T result;
			try
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				result = (T)converter.ConvertFrom(packetData.Data);
			}
			catch
			{
				result = default(T);
			}
			return result;
		}

		public void AddValue(string dataName, object data)
		{
			if (!dataName.Contains("."))
			{
				this.Add(new PacketData(dataName, data));
				return;
			}
			string[] array = dataName.Split(new char[]
			{
				'.'
			});
			PacketPath packetPath = this;
			int i;
			for (i = 0; i < array.Length - 1; i++)
			{
				PacketPath packetPath2 = packetPath.GetNode(array[i]) as PacketPath;
				if (packetPath2 == null)
				{
					packetPath2 = new PacketPath(array[i], new PacketNode[0]);
					packetPath.Add(packetPath2);
				}
				packetPath = packetPath2;
			}
			packetPath.Add(new PacketData(array[i], data));
		}

		public void SetValue(string name, object data)
		{
			if (!name.Contains("."))
			{
				PacketData packetData = this.GetNode(name) as PacketData;
				if (packetData == null)
				{
					packetData = new PacketData(name, data);
					this.Add(packetData);
					return;
				}
				packetData.Data = data;
				return;
			}
			else
			{
				string[] array = name.Split(new char[]
				{
					'.'
				});
				PacketPath packetPath = this;
				int i;
				for (i = 0; i < array.Length - 1; i++)
				{
					PacketPath packetPath2 = packetPath.GetNode(array[i]) as PacketPath;
					if (packetPath2 == null)
					{
						packetPath2 = new PacketPath(array[i], new PacketNode[0]);
						packetPath.Add(packetPath2);
					}
					packetPath = packetPath2;
				}
				PacketData packetData2 = this.GetNode(array[i]) as PacketData;
				if (packetData2 == null)
				{
					packetData2 = new PacketData(array[i], data);
					this.Add(packetData2);
					return;
				}
				packetData2.Data = data;
				return;
			}
		}

		public void Add(PacketNode item)
		{
			this.nodes.Add(item);
			this.nodelookup[item.Name] = item;
		}
		public void Clear()
		{
			this.nodes.Clear();
			this.nodelookup.Clear();
		}
		public bool Contains(PacketNode item)
		{
			return this.nodes.Contains(item);
		}
		public bool Contains(string nodename)
		{
			return nodename != null && this.nodelookup.ContainsKey(nodename);
		}
		public void CopyTo(PacketNode[] array, int arrayIndex)
		{
			foreach (PacketNode current in this.nodes)
			{
				array[arrayIndex++] = current.Clone();
			}
		}
		public bool Remove(PacketNode item)
		{
			return this.nodes.Remove(item) && this.nodelookup.Remove(item.Name);
		}
		public IEnumerator<PacketNode> GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}
		public override bool Equals(object obj)
		{
			PacketPath packetPath = obj as PacketPath;
			if (packetPath == null)
			{
				return false;
			}
			if (packetPath.Name != base.Name)
			{
				return false;
			}
			if (packetPath.nodes.Count != this.nodes.Count)
			{
				return false;
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (!packetPath.nodes[i].Equals(this.nodes[i]))
				{
					return false;
				}
			}
			return true;
		}
		public static PacketPath Serialize(string name, object argument)
		{
			PacketPath packetPath = new PacketPath(name, new PacketNode[0]);
			if (argument is Array)
			{
				packetPath.Add(new PacketData("Type", "Array"));
				IEnumerator enumerator = ((Array)argument).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						packetPath.Add(PacketPath.Serialize("Element", current));
					}
					return packetPath;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (argument is ISerializable)
			{
				packetPath.Add(new PacketData("Type", "ISerializable"));
				packetPath.Add(PacketPath.CreateTyped((ISerializable)argument, "Data"));
			}
			else
			{
				packetPath.Add(new PacketData("Type", "Value"));
				packetPath.Add(new PacketData("Data", argument));
			}
			return packetPath;
		}
		public static object Deserialize(PacketPath serializedpath)
		{
			if (serializedpath == null)
			{
				return null;
			}
			string value;
			if ((value = serializedpath.GetValue<string>("Type")) != null)
			{
				if (value == "Array")
				{
					List<object> list = new List<object>();
					foreach (PacketNode current in serializedpath.GetNodes("Element"))
					{
						if (current is PacketPath)
						{
							list.Add(PacketPath.Deserialize((PacketPath)current));
						}
					}
					return list.ToArray();
				}
				if (value == "ISerializable")
				{
					return PacketPath.ConvertTyped(serializedpath.GetNode("Data") as PacketPath);
				}
				if (value == "Value")
				{
					return serializedpath.GetValue<object>("Data");
				}
			}
			return null;
		}
		public override int GetHashCode()
		{
			int num = base.Name.GetHashCode();
			foreach (PacketNode current in this.nodes)
			{
				num ^= current.GetHashCode();
			}
			return num;
		}
	}
}
