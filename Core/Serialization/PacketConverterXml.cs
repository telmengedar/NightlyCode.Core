using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
namespace GM.Core.Serialization
{
	public class PacketConverterXml
	{
		public static byte[] Write(PacketPath packet)
		{
			XmlDocument xmlDocument = new XmlDocument();
			Stack<Pair<XmlNode, PacketNode>> stack = new Stack<Pair<XmlNode, PacketNode>>();
			stack.Push(new Pair<XmlNode, PacketNode>(xmlDocument, packet));
			while (stack.Count > 0)
			{
				Pair<XmlNode, PacketNode> pair = stack.Pop();
				XmlNode xmlNode = pair.X.AppendChild(xmlDocument.CreateElement(pair.Y.Name));
				if (pair.Y is PacketData)
				{
					if (((PacketData)pair.Y).Data is byte[])
					{
						xmlNode.InnerText = Convert.ToBase64String((byte[])((PacketData)pair.Y).Data);
					}
					else
					{
						XmlNode xmlNode2 = xmlNode.Attributes.Append(xmlDocument.CreateAttribute("Value"));
						xmlNode2.InnerText = ((PacketData)pair.Y).Data.ToString();
						xmlNode2 = xmlNode.Attributes.Append(xmlDocument.CreateAttribute("Type"));
						xmlNode2.InnerText = ((PacketData)pair.Y).Data.GetType().Name;
					}
				}
				else
				{
					if (pair.Y is PacketPath)
					{
						foreach (PacketNode current in (PacketPath)pair.Y)
						{
							stack.Push(new Pair<XmlNode, PacketNode>(xmlNode, current));
						}
					}
				}
			}
			List<byte> list = new List<byte>();
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlDocument.WriteTo(xmlTextWriter);
					list.Add(1);
					list.AddRange(Encoding.UTF8.GetBytes(stringWriter.ToString()));
					list.Add(0);
				}
			}
			return list.ToArray();
		}
		public static PacketPath Read(byte[] packetdata)
		{
			XmlDocument xmlDocument = new XmlDocument();
			MemoryStream memoryStream = new MemoryStream(packetdata);
			memoryStream.Position = 1L;
			try
			{
				xmlDocument.Load(memoryStream);
			}
			catch
			{
				return null;
			}

			if (xmlDocument.FirstChild.Name != "Packet")
			{
				return null;
			}
			PacketPath packetPath = new PacketPath("Packet", new PacketNode[0]);
			Stack<Pair<XmlNode, PacketPath>> stack = new Stack<Pair<XmlNode, PacketPath>>();
			stack.Push(new Pair<XmlNode, PacketPath>(xmlDocument.FirstChild, packetPath));
			while (stack.Count > 0)
			{
				Pair<XmlNode, PacketPath> pair = stack.Pop();
				PacketNode packetNode;
				if (pair.X.ChildNodes.Count > 0)
				{
					packetNode = new PacketPath(pair.X.Name, new PacketNode[0]);
					IEnumerator enumerator = pair.X.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlNode x = (XmlNode)enumerator.Current;
							stack.Push(new Pair<XmlNode, PacketPath>(x, (PacketPath)packetNode));
						}
						goto IL_1AB;
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
				XmlNode xmlNode = pair.X.Attributes["Type"];
				if (xmlNode == null)
				{
					continue;
				}
				Type type = Type.GetType(xmlNode.InnerText);
				if (type == typeof(byte[]))
				{
					Convert.FromBase64String(pair.X.InnerText);
				}
				else
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
					{
						new NullableConverter(type).ConvertFromString(pair.X.InnerText);
					}
					else
					{
						Convert.ChangeType(pair.X.InnerText, type);
					}
				}
				packetNode = new PacketData(pair.X.Name, xmlDocument);
				IL_1AB:
				pair.Y.Add(packetNode);
			}
			memoryStream.Close();
			return packetPath;
		}
	}
}
