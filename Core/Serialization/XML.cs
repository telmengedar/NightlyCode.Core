using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NightlyCode.Core.Conversion;
using NightlyCode.Core.Helpers;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// xml serialization
    /// </summary>
    public static class XML {
         
        /// <summary>
        /// saves the data to xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="node"></param>
        public static void SaveTo<T>(this T data, XmlNode node) {
            // needed for proper array serialization
            if(data == null)
                return;

            if(data is IXMLSerializable) {
                ((IXMLSerializable)data).Serialize(node);
                return;
            }

            if(ParserAttribute.IsDefined(data.GetType())){
                node.CreateAndAppendAttribute("value", data.ToString());
                return;
            }

            foreach(PropertyInfo property in data.GetType().GetProperties()) {
                object value = property.GetValue(data, null);
                string propertyname = property.Name.ToLower();
                if(property.PropertyType.IsArray) {
                    Array array = (Array)value;
                    for(int i = 0; i < array.Length; ++i) {
                        object item = array.GetValue(i);
                        SaveTo(item, node.CreateAndAppendElement(propertyname));
                    }
                }
                else if(property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    node.CreateAndAppendAttribute(property.Name.ToLower(), value.ToString());
                else if(ParserAttribute.IsDefined(property.PropertyType)) {
                    node.CreateAndAppendAttribute(property.Name.ToLower(), value.ToString());
                }
                else {
                    if(value == null)
                        continue;

                    XmlNode propertynode = node.CreateAndAppendElement(property.Name.ToLower());
                    if(value.GetType() == typeof(XmlDocument)) {
                        propertynode.AppendForeign((XmlNode)value);
                    }
                    else {
                        if(value.GetType() != property.PropertyType)
                            propertynode.CreateAndAppendAttribute("type", TypeHelper.GetTypeName(value.GetType()));
                        SaveTo(value, propertynode);
                    }

                }
            }
        }

        /// <summary>
        /// creates an object from file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T CreateFrom<T>(string filename) {
            return (T)CreateFrom(filename, typeof(T));
        }

        /// <summary>
        /// creates an object from file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="basetype"> </param>
        /// <returns></returns>
        public static object CreateFrom(string filename, Type basetype=null) {
            using(FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return CreateFrom(fs, basetype);
        }

        /// <summary>
        /// creates an object from stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T CreateFrom<T>(Stream stream) {
            return (T)CreateFrom(stream, typeof(T));
        }

        /// <summary>
        /// creates an object from xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <returns></returns>
        public static T CreateFrom<T>(XmlDocument document) {
            return (T)CreateFrom(document.DocumentElement, typeof(T));
        }

        /// <summary>
        /// creates an object from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="basetype"> </param>
        /// <returns></returns>
        public static object CreateFrom(Stream stream, Type basetype=null) {
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            return CreateFrom(document.DocumentElement, basetype);
        }

        /// <summary>
        /// creates data from xml
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T CreateFrom<T>(XmlNode node) {
            return (T)CreateFrom(node, typeof(T));
        }

        /// <summary>
        /// creates data from xml
        /// </summary>
        /// <param name="node"></param>
        /// <param name="basetype"> </param>
        /// <returns></returns>
        public static object CreateFrom(XmlNode node, Type basetype = null) {
            Type datatype = basetype;
            if(node.ContainsAttribute("type")) {
                string typename = node.GetAttributeValue<string>("type");
                datatype = Type.GetType(typename);
            }

            if(datatype == null)
                throw new InvalidOperationException("No type specified.");

            if(ParserAttribute.IsDefined(datatype))
                return ParseData(datatype, node.GetAttributeValue<string>("value"));
            
            object data = Activator.CreateInstance(datatype, true);
            if(data is IXMLSerializable)
                ((IXMLSerializable)data).Deserialize(node);
            else LoadFrom(data, node);
            return data;
        }

        static object ParseData(Type type, string value) {
            ParserAttribute parser = ParserAttribute.Get(type);
            MethodInfo parsermethod = type.GetMethod(parser.Method, BindingFlags.Public | BindingFlags.Static);
            if(parsermethod == null)
                throw new InvalidOperationException($"parser '{parser.Method}' on type '{type.Name}' not found");

            return parsermethod.Invoke(null, new object[] { value });            
        }

        /// <summary>
        /// loads (already created) data from xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="node"></param>
        public static void LoadFrom<T>(this T data, XmlNode node) {
            if(data is IXMLSerializable) {
                ((IXMLSerializable)data).Deserialize(node);
                return;
            }

            foreach(PropertyInfo property in data.GetType().GetProperties()) {
                string propertyname = property.Name.ToLower();
                if(property.PropertyType.IsArray) {
                    XmlNodeList valuelist = node.SelectNodes(propertyname);
                    if(valuelist != null) {
                        Array array = Array.CreateInstance(property.PropertyType.GetElementType(), valuelist.Count);
                        int i = 0;
                        foreach(XmlNode itemnode in valuelist.Cast<XmlNode>())
                            array.SetValue(CreateFrom(itemnode, property.PropertyType.GetElementType()), i++);
                        property.SetValue(data, array, null);
                    }
                }
                else if(property.PropertyType.IsValueType || property.PropertyType == typeof(string)) {
                    if(node.ContainsAttribute(propertyname))
                        property.SetValue(data, Converter.Convert(node.GetAttributeValue<string>(propertyname), property.PropertyType), null);
                }
                else if(ParserAttribute.IsDefined(property.PropertyType)) {
                    if(node.ContainsAttribute(propertyname))
                        property.SetValue(data, ParseData(property.PropertyType, node.GetAttributeValue<string>(propertyname)), null);
                }
                else {
                    if(node.ContainsElement(property.Name.ToLower())) {
                        if(property.PropertyType == typeof(XmlDocument)) {
                            XmlDocument document = new XmlDocument();
                            XmlNode propertynode = node.SelectSingleNode(propertyname);
                            if(propertynode != null) {
                                document.AppendForeign(propertynode.FirstChild);
                                property.SetValue(data, document, null);
                            }
                        }
                        else property.SetValue(data, CreateFrom(node.SelectSingleNode(propertyname), property.PropertyType), null);
                    }
                }
            }
        }
    }
}