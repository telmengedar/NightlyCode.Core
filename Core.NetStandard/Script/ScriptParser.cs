using System.Collections.Generic;
using System.Globalization;
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
                if(tokenname.Length == 0 && (char.IsDigit(character) || character == '.' || character == '-'))
                    return ParseNumber(data, ref index);

                switch (character) {
                    case '.':
                        object host = hostpool.GetHost(tokenname.ToString());
                        tokenname.Length = 0;
                        ++index;
                        return ParseMember(host, data, ref index, variablehost);
                    case ',':
                    case ')':
                    case ']':
                        if(tokenname.Length > 0)
                            return new ScriptValue(tokenname.ToString().TrimEnd(' '));
                        break;
                    case '$':
                        ++index;
                        return variablehost.GetVariable(ParseToken(data, ref index, variablehost).ToString());
                    case '"':
                        ++index;
                        return ParseLiteral(data, ref index);
                    case '[':
                        ++index;
                        return new ScriptArray(ParseArray(data, ref index, variablehost));
                    case ' ':
                        if(tokenname.Length == 0)
                            continue;
                        tokenname.Append(character);
                        break;
                    case '\\':
                        ++index;
                        tokenname.Append(ParseSpecialCharacter(data[index]));
                        break;
                    default:
                        tokenname.Append(character);
                        break;
                }
            }

            if(tokenname.Length > 0)
                return new ScriptValue(tokenname.ToString().TrimEnd(' '));
            return new ScriptValue(null);
        }

        IScriptToken ParseNumber(string data, ref int index)
        {
            StringBuilder literal = new StringBuilder();
            bool done = false;
            for (; index < data.Length; ++index)
            {
                char character = data[index];
                switch (character)
                {
                    case ',':
                        ++index;
                        done = true;
                        break;
                    case ')':
                    case ']':
                        done = true;
                        break;
                    default:
                        literal.Append(character);
                        break;
                }

                if(done)
                    break;
            }

            // this can't be a number
            if (literal.Length == 0)
                return new ScriptValue("");

            int dotcount = 0;
            for (int i = 0; i < literal.Length; ++i) {
                if(!char.IsDigit(literal[i]) && literal[i] != '.' || i > 0 && literal[i] == '-')
                    return new ScriptValue(literal.ToString());

                if (literal[i] == '.')
                    ++dotcount;
            }

            switch (dotcount)
            {
                case 0:
                    return new ScriptValue(long.Parse(literal.ToString()));
                case 1:
                    return new ScriptValue(double.Parse(literal.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture));
                default:
                    return new ScriptValue(literal.ToString());
            }
        }

        char ParseSpecialCharacter(char character) {
            switch(character) {
                case 't':
                    return '\t';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                default:
                    return character;
            }
        }

        IScriptToken ParseLiteral(string data, ref int index) {
            StringBuilder literal = new StringBuilder();
            for(; index < data.Length; ++index) {
                char character = data[index];
                switch(character) {
                    case '"':
                        ++index;
                        return new ScriptValue(literal.ToString());
                    case '\\':
                        ++index;
                        literal.Append(ParseSpecialCharacter(data[index]));
                        break;
                    default:
                        literal.Append(character);
                        break;
                }
            }

            throw new ScriptException("Literal not terminated");
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
                            throw new ScriptException("Member name expected");
                        return new ScriptMemberRead(host, membername.ToString());
                    default:
                        membername.Append(character);
                        break;
                }
            }

            if(membername.Length > 0)
                return new ScriptMemberRead(host, membername.ToString());
            throw new ScriptException("Member name expected");
        }

        IScriptToken[] ParseArray(string data, ref int index, IScriptVariableHost variablehost) {
            List<IScriptToken> array = new List<IScriptToken>();
            for (; index < data.Length;)
            {
                char character = data[index];
                switch (character)
                {
                case '[':
                    ++index;
                    array.Add(new ScriptArray(ParseArray(data, ref index, variablehost)));
                    break;
                case ']':
                    ++index;
                    return array.ToArray();
                case ',':
                    ++index;
                    break;
                default:
                    array.Add(ParseToken(data, ref index, variablehost));
                    break;
                }
            }

            throw new ScriptException("Array not terminated");
        }

        IScriptToken[] ParseParameters(string data, ref int index, IScriptVariableHost variablehost) {
            List<IScriptToken> parameters = new List<IScriptToken>();
            for(; index < data.Length;) {
                char character = data[index];
                switch(character) {
                    case '[':
                        ++index;
                        parameters.Add(new ScriptArray(ParseArray(data, ref index, variablehost)));
                        break;
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

            throw new ScriptException("Parameter list not terminated");
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