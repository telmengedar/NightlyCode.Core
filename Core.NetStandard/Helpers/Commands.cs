using System.Collections.Generic;
using System.Text;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// command helper operations
    /// </summary>
    public static class Commands {

        /// <summary>
        /// splits an command string into argument strings
        /// </summary>
        /// <param name="command">command string to split</param>
        /// <returns>enumeration of arguments</returns>
        public static IEnumerable<string> SplitArguments(string command)
        {
            bool stringflag = false;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < command.Length; ++i)
            {
                switch (command[i])
                {
                    case '\\':
                        sb.Append(command[++i]);
                        break;
                    case '\"':
                        if (stringflag) {
                            yield return sb.ToString();
                            sb.Length = 0;
                            stringflag = false;
                        }
                        else
                        {
                            stringflag = true;
                        }
                        break;
                    case ' ':
                        if(!stringflag) {
                            if(sb.Length > 0) {
                                yield return sb.ToString();
                                sb.Length = 0;
                            }
                        }
                        else sb.Append(' ');
                        break;
                    default:
                        sb.Append(command[i]);
                        break;
                }
            }

            if(sb.Length > 0)
                yield return sb.ToString();
        }

    }
}