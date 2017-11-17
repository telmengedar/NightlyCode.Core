namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// several RegEx patterns
    /// </summary>
    public class RegExHelper {

        /// <summary>
        /// default floating point pattern
        /// </summary>
        public static string FloatingPointPattern {get { return @"[\d]*((\.|,)[\d]+)?"; }}
    }
}