namespace NightlyCode.Core.Script {

    /// <summary>
    /// host for script variables
    /// </summary>
    public interface IScriptVariableHost {

        /// <summary>
        /// get value of a variable
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <returns>variable value</returns>
        IScriptToken GetVariable(string name);
    }
}