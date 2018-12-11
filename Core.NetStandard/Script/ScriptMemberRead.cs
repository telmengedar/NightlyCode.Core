using System;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Core.Script {

    /// <summary>
    /// reads a value from a host member
    /// </summary>
    public class ScriptMemberRead : IScriptToken {
        readonly object host;
        readonly string membername;

        /// <summary>
        /// creates a new <see cref="ScriptMemberRead"/>
        /// </summary>
        /// <param name="host">host of member</param>
        /// <param name="membername">name of member to read</param>
        public ScriptMemberRead(object host, string membername) {
            this.host = host;
            this.membername = membername.ToLower();
        }

        /// <inheritdoc />
        public object Execute() {
            PropertyInfo property = host.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == membername);
            if(property != null) {
                try {
                    return property.GetValue(host);
                }
                catch(Exception e) {
                    throw new ScriptException("Unable to read property", null, e);
                }
            }


            FieldInfo fieldinfo = host.GetType().GetFields().FirstOrDefault(f => f.Name.ToLower() == membername);
            if (fieldinfo == null)
                throw new ScriptException($"A member with the name of {membername} was not found in type {host.GetType().Name}");

            try {
                return fieldinfo.GetValue(host);
            }
            catch(Exception e) {
                throw new ScriptException("Unable to read field", null, e);
            }
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"{host}.{membername}";
        }
    }
}