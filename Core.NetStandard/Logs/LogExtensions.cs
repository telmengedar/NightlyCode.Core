using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using NightlyCode.Core.Collections;

namespace NightlyCode.Core.Logs {

    /// <summary>
    /// extensions for log use
    /// </summary>
    public static class LogExtensions {

        /// <summary>
        /// converts the byte array to a string hex table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToHexTable(this byte[] data) {
            StringBuilder sb = new StringBuilder();
            int line = 0;
            foreach(IEnumerable<byte> block in data.Block(16)) {
                sb.Append(line.ToString("X4"))
                    .Append("|")
                    .Append(string.Join(" ", block.Take(8).Select(b => b.ToString("X2"))).PadRight(23))
                    .Append("|")
                    .Append(string.Join(" ", block.Skip(8).Select(b => b.ToString("X2"))).PadRight(23))
                    .Append("|")
                    .Append(new string(block.Select(b => b >= 0x20 && b <= 0x80 ? (char)b : ' ').ToArray()))
                    .AppendLine();
                line += 16;
            }
            return sb.ToString();
        }

        /// <summary>
        /// converts an object structure to string
        /// </summary>
        /// <param name="data">data to convert</param>
        /// <param name="showbinary">determines whether to show binary blobs as hex table</param>
        /// <returns>string representation</returns>
        public static string ToLogDisplay(object data, bool showbinary=false) {
            if(data == null)
                return "(null)";

            StringBuilder sb = new StringBuilder();
            ToLogStructure(data, data.GetType().Name, 0, sb, showbinary);
            return sb.ToString();
        }

        static void ToLogStructure(object data, string name, int indent, StringBuilder sb, bool showbinary) {
            sb.Append(new string(' ', indent));
            sb.Append(name);
            if(data == null)
                sb.AppendLine(" = (null)");
            else {
                if(data.GetType().IsArray) {
                    if(data.GetType() == typeof(byte[])) {
                        if(showbinary)
                            sb.AppendLine($"\r\n{((byte[])data).ToHexTable()}");
                        else sb.AppendLine(" = (data)");
                    }
                    else {
                        sb.AppendLine();
                        Array array = (Array)data;
                        for(int i = 0; i < array.Length; ++i)
                            ToLogStructure(array.GetValue(i), "[" + i + "]", indent + 2, sb, showbinary);
                    }
                }
                else if(data is IEnumerable && !(data is string)) {
                    sb.AppendLine();
                    int index=0;
                    foreach(object item in (IEnumerable)data)
                        ToLogStructure(item, $"[{index++}]", indent + 2, sb, showbinary);
                }
                else if(data.GetType().IsEnum) {
                    sb.Append(" = ").AppendLine(data.ToString());
                }
                else if(data is string) {
                    if (name.ToLower().Contains("password"))
                        sb.AppendLine(" = *****");
                    else sb.Append(" = ").AppendLine(data.ToString());
                }
                else if(data.GetType().IsValueType || data is IPAddress || data is DateTime) {
                    sb.Append(" = ").AppendLine(data.ToString());
                }
                else {
                    sb.AppendLine();
                    foreach(PropertyInfo pi in data.GetType().GetProperties()) {
                        if(Attribute.IsDefined(pi, typeof(HideAttribute)))
                            continue;

                        object value = pi.GetValue(data, null);
                        ToLogStructure(value, pi.Name, indent + 2, sb, showbinary);
                    }
                }
            }
        }
    }
}