using System.Collections.Generic;
using System.Linq;
using System.Text;
using NightlyCode.Core.Collections;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// operations useful for data display
    /// </summary>
    public static class DataDisplay {
         
        /// <summary>
        /// determines whether the character is displayable
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsDisplayable(char character) {
            return char.IsLetterOrDigit(character) || char.IsPunctuation(character);
        }

        /// <summary>
        /// converts data to displayable string
        /// </summary>
        /// <param name="data">data to convert</param>
        /// <returns>string which can be used to display data</returns>
        public static string ToDisplayString(this byte[] data) {
            StringBuilder sb = new StringBuilder();
            int offset = 0;
            // ReSharper disable PossibleMultipleEnumeration
            // there are multiple iterations ... over a list, which is not that horrible
            foreach(IEnumerable<byte> block in data.Block(16)) {
                sb.AppendFormat("{0:X08}", offset).Append("|");
                foreach(byte datum in block)
                    sb.AppendFormat("{0:X02}", datum).Append("|");
                int count = block.Count();
                if(count < 16)
                    sb.Append(new string(' ', (16 - count) * 3));
                foreach(byte datum in block)
                    sb.Append(IsDisplayable((char)datum) ? (char)datum : ' ');
                offset += 16;
                sb.AppendLine();
            }
            // ReSharper restore PossibleMultipleEnumeration
            return sb.ToString();
        }
    }
}