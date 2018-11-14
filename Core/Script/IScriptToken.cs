namespace NightlyCode.Core.Script {

    /// <summary>
    /// token of a script which can get executed
    /// </summary>
    public interface IScriptToken {

        /// <summary>
        /// executes the token returning a result
        /// </summary>
        /// <returns>result of token call</returns>
        object Execute();
    }
}