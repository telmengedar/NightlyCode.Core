using System;
using System.Collections.Generic;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// helps in translation of types to names
    /// </summary>
    public class TypeHelper {
        static readonly Dictionary<Type, string> typetranslation = new Dictionary<Type, string>();

        public static string GetTypeName(Type type) {
            string typename;
            if(!typetranslation.TryGetValue(type, out typename))
                typetranslation[type] = typename = $"{type.Namespace}.{type.Name}, {type.Assembly.GetName().Name}";
            return typename;
        }
    }
}
