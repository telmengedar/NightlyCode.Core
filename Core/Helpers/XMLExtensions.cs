using System;
using System.Globalization;
using System.Xml;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// helper operations for xml structures
    /// </summary>
    public static class XMLExtensions {

        /// <summary>
        /// tries to get a value from an attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attributename"></param>
        /// <param name="value"> </param>
        /// <returns></returns>
        public static bool TryGetAttributeValue<T>(this XmlNode node, string attributename, out T value) {
            if(node.Attributes==null) {
                value = default(T);
                return false;
            }

            XmlAttribute attribute = node.Attributes[attributename];
            if(attribute==null) {
                value = default(T);
                return false;
            }

            string attributevalue = attribute.InnerText;
            if(typeof(T) == typeof(string))
                value = (T)(object)attributevalue;
            else value = (T)Convert.ChangeType(attributevalue, typeof(T), CultureInfo.InvariantCulture);
            return true;
        }

        /// <summary>
        /// get the attribute value if it exists or the default value of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attributename"></param>
        /// <returns></returns>
        public static T GetAttributeValueOrDefault<T>(this XmlNode node, string attributename) {
            if(ContainsAttribute(node, attributename))
                return GetAttributeValue<T>(node, attributename);
            return default(T);
        }

        /// <summary>
        /// gets a value from an attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attributename"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(this XmlNode node, string attributename) {
            if(node.Attributes == null)
                throw new InvalidOperationException(attributename + " not found");

            XmlAttribute attribute = node.Attributes[attributename];
            if(attribute == null)
                throw new InvalidOperationException(attributename + " not found");

            string attributevalue = attribute.InnerText;
            if(typeof(T) == typeof(string))
                return (T)(object)attributevalue;
            return (T)Convert.ChangeType(attributevalue, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// determines whether the node contains at least one element of the specified name
        /// </summary>
        /// <param name="node"></param>
        /// <param name="elementname"></param>
        /// <returns></returns>
        public static bool ContainsElement(this XmlNode node, string elementname) {
            return node.SelectSingleNode(elementname) != null;
        }

        /// <summary>
        /// determines whether the specified attribute exists
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributename"></param>
        /// <returns></returns>
        public static bool ContainsAttribute(this XmlNode node, string attributename) {
            if(node.Attributes == null) return false;
            return node.Attributes[attributename] != null;
        }

        /// <summary>
        /// get a value from a node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="nodepath"></param>
        /// <returns></returns>
        public static T GetNodeValue<T>(this XmlNode node, string nodepath) {

            XmlNode subnode = node.SelectSingleNode(nodepath);
            if(subnode==null)
                throw new InvalidOperationException(nodepath + " not found");

            string nodevalue = subnode.InnerText;
            if(typeof(T) == typeof(string))
                return (T)(object)nodevalue;
            return (T)Convert.ChangeType(nodevalue, typeof(T), CultureInfo.InvariantCulture);            
        }

        static XmlDocument GetOwnerDocument(XmlNode node) {
            if(node is XmlDocument)
                return (XmlDocument)node;
            return node.OwnerDocument;
        }

        /// <summary>
        /// creates a new xml element and appends it to the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public static XmlNode CreateAndAppendElement(this XmlNode node, string nodename) {
            XmlDocument owner = GetOwnerDocument(node);
            if (owner == null)
                throw new InvalidOperationException("Node must have an owner document.");

            XmlNode newnode = owner.CreateElement(nodename);
            node.AppendChild(newnode);
            return newnode;
        }

        /// <summary>
        /// creates a new attribute and appends it to the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributename"></param>
        /// <param name="attributevalue"></param>
        /// <returns></returns>
        public static XmlNode CreateAndAppendAttribute(this XmlNode node, string attributename, string attributevalue) {
            XmlDocument owner = GetOwnerDocument(node);
            if (owner == null)
                throw new InvalidOperationException("Node must have an owner document.");

            if(node.Attributes == null)
                throw new InvalidOperationException("node has no attribute collection.");

            XmlAttribute attribute = owner.CreateAttribute(attributename);
            attribute.InnerText = attributevalue;
            node.Attributes.Append(attribute);
            return node;
        }

        /// <summary>
        /// appends a foreign node to the childs of this node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="foreign"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public static XmlNode AppendForeign(this XmlNode node, XmlNode foreign, bool deep=true) {
            XmlDocument owner = GetOwnerDocument(node);
            if(owner == null)
                throw new InvalidOperationException("Node must have an owner document.");

            node.AppendChild(owner.ImportNode(foreign, deep));
            return node;
        }
    }
}