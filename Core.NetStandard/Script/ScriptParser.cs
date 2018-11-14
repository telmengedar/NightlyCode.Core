using System;
using System.Collections.Generic;
using System.Text;

namespace NightlyCode.Core.Script {

    /// <summary>
    /// parses scripts from string data
    /// </summary>
    public class ScriptParser {
        readonly IScriptHostPool hostpool;

        /// <summary>
        /// creates a new <see cref="ScriptParser"/>
        /// </summary>
        /// <param name="hostpool">pool containing hosts for members</param>
        public ScriptParser(IScriptHostPool hostpool) {
            this.hostpool = hostpool;
        }

        IScriptToken ParseToken(string data, ref int index, IScriptVariableHost variablehost) {
            StringBuilder tokenname = new StringBuilder();

            for(; index < data.Length; ++index) {
                char character = data[index];
                switch(character) {
                    case '.':
                        object host = hostpool.GetHost(tokenname.ToString());
                        tokenname.Length = 0;
                        ++index;
                        return ParseMember(host, data, ref index, variablehost);
                    case ',':
                    case ')':
                        if(tokenname.Length > 0)
                            return new ScriptValue(tokenname.ToString());
                        break;
                    case '$':
                        ++index;
                        return variablehost.GetVariable(ParseToken(data, ref index, variablehost).ToString());
                    case '"':
                        ++index;
                        return ParseLiteral(data, ref index);
                    default:
                        tokenname.Append(character);
                        break;
                }
            }

            if(tokenname.Length > 0)
                return new ScriptValue(tokenname.ToString());
            return new ScriptValue(null);
        }

        IScriptToken ParseLiteral(string data, ref int index) {
            StringBuilder literal = new StringBuilder();
            for(; index < data.Length; ++index) {
                char character = data[index];
                switch(character) {
                    case '"':
                        ++index;
                        return new ScriptValue(literal.ToString());
                    default:
                        literal.Append(character);
                        break;
                }
            }

            throw new Exception("Literal not terminated");
        }

        IScriptToken ParseMember(object host, string data, ref int index, IScriptVariableHost variablehost) {
            StringBuilder membername = new StringBuilder();
            for (; index < data.Length; ++index)
            {
                char character = data[index];
                switch (character)
                {
                    case '=':
                        ++index;
                        IScriptToken value=ParseToken(data, ref index, variablehost);
                        return new ScriptMemberAssignment(host, membername.ToString(), value);
                    case '(':
                        ++index;
                        return new ScriptMethodCall(host, membername.ToString(), ParseParameters(data, ref index, variablehost));
                    case ',':
                    case ')':
                        if (membername.Length == 0)
                            throw new Exception("Member name expected");
                        return new ScriptMemberRead(host, membername.ToString());
                    default:
                        membername.Append(character);
                        break;
                }
            }

            if(membername.Length > 0)
                return new ScriptMemberRead(host, membername.ToString());
            throw new Exception("Member name expected");
        }

        IScriptToken[] ParseParameters(string data, ref int index, IScriptVariableHost variablehost) {
            List<IScriptToken> parameters = new List<IScriptToken>();
            for(; index < data.Length;) {
                char character = data[index];
                switch(character) {
                    case ')':
                        ++index;
                        return parameters.ToArray();
                    case ',':
                        ++index;
                        break;
                    default:
                        parameters.Add(ParseToken(data, ref index, variablehost));
                        break;
                }
            }

            throw new Exception("Parameter list not terminated");
        }

        /// <summary>
        /// parses a script for execution
        /// </summary>
        /// <param name="data">data to parse</param>
        /// <param name="variablehost">host for variables</param>
        /// <returns>script which can get executed</returns>
        public IScriptToken Parse(string data, IScriptVariableHost variablehost=null) {
            int index = 0;
            return ParseToken(data, ref index, variablehost);
        }
    }
}