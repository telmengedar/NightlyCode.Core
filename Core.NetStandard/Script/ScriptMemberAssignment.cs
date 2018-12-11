using System;
using System.Linq;
using System.Reflection;
using NightlyCode.Core.Conversion;

namespace NightlyCode.Core.Script {

    /// <summary>
    /// assigns a value to a member of a host in a script command
    /// </summary>
    public class ScriptMemberAssignment : IScriptToken {
        readonly object host;
        readonly string membername;
        readonly IScriptToken value;

        /// <summary>
        /// creates a new <see cref="ScriptMemberAssignment"/>
        /// </summary>
        /// <param name="host">host of member</param>
        /// <param name="member">member to assign value to</param>
        /// <param name="value">value to assign to member</param>
        public ScriptMemberAssignment(object host, string member, IScriptToken value) {
            this.host = host;
            membername = member.ToLower();
            this.value = value;
        }

        object SetProperty(PropertyInfo property) {
            object targetvalue = Converter.Convert(value.Execute(), property.PropertyType);
            try {
                property.SetValue(host, targetvalue, null);
            }
            catch(Exception e) {
                throw new ScriptException("Unable to set property", null, e);
            }
            
            return targetvalue;
        }

        object SetField(FieldInfo fieldinfo)
        {
            object targetvalue = Converter.Convert(value.Execute(), fieldinfo.FieldType);
            try {
                fieldinfo.SetValue(host, targetvalue);
            }
            catch(Exception e) {
                throw new ScriptException("Unable to set field", null, e);
            }
            
            return targetvalue;
        }

        /// <inheritdoc />
        public object Execute() {            
            PropertyInfo property = host.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == membername);
            if(property != null)
                return SetProperty(property);

            FieldInfo fieldinfo = host.GetType().GetFields().FirstOrDefault(f => f.Name.ToLower() == membername);
            if(fieldinfo == null)
                throw new ScriptException($"A member with the name of {membername} was not found in type {host.GetType().Name}");

            return SetField(fieldinfo);
        }

        public override string ToString() {
            return $"{host}.{membername}={value}";
        }
    }
}