namespace NightlyCode.Core.Script {

    /// <summary>
    /// interface for a pool of script hosts
    /// </summary>
    public interface IScriptHostPool {

        /// <summary>
        /// get host providing members
        /// </summary>
        /// <param name="name">name of host</param>
        /// <returns>host instance</returns>
        object GetHost(string name);
    }
}