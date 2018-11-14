
namespace NightlyCode.Core.Script {

    /// <summary>
    /// host which serves variables of an instance
    /// </summary>
    public class InstanceVariableHost : IScriptVariableHost {
        readonly object instance;

        /// <summary>
        /// creates a new <see cref="InstanceVariableHost"/>
        /// </summary>
        /// <param name="instance">instance of which to serve variables</param>
        public InstanceVariableHost(object instance) {
            this.instance = instance;
        }

        /// <inheritdoc />
        public IScriptToken GetVariable(string name) {
            return new ScriptMemberRead(instance, name);
        }
    }
}