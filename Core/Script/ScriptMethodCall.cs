﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NightlyCode.Core.Conversion;

namespace NightlyCode.Core.Script {

    /// <summary>
    /// calls a method in a script
    /// </summary>
    public class ScriptMethodCall : IScriptToken {
        readonly object host;
        readonly string methodname;
        readonly IScriptToken[] parameters;

        /// <summary>
        /// creates a new <see cref="ScriptMethodCall"/>
        /// </summary>
        /// <param name="host">host of method</param>
        /// <param name="methodname">name of method to call</param>
        /// <param name="parameters">parameters for method call</param>
        public ScriptMethodCall(object host, string methodname, IScriptToken[] parameters) {
            this.host = host;
            this.methodname = methodname.ToLower();
            this.parameters = parameters;
        }

        /// <inheritdoc />
        public object Execute() {
            MethodInfo[] methods = host.GetType().GetMethods().Where(m => m.Name.ToLower() == methodname && m.GetParameters().Length == parameters.Length).ToArray();
            if(methods.Length == 0)
                throw new Exception($"Method '{methodname}' matching the specified parameters count not found on type {host.GetType().Name}");

            foreach(MethodInfo method in methods) {
                try {
                    List<object> convertedparameters = new List<object>();
                    ParameterInfo[] targetparameters = method.GetParameters();
                    for(int i = 0; i < targetparameters.Length; ++i)
                        convertedparameters.Add(Converter.Convert(parameters[i].Execute(), targetparameters[i].ParameterType));
                    return method.Invoke(host, convertedparameters.ToArray());
                }
                catch(Exception) {
                    continue;
                }
            }

            throw new Exception("None of the matching methods could be invoked using the specified parameters");
        }

        public override string ToString() {
            return $"{host}.{methodname}({string.Join(",", parameters.Select(p => p.ToString()))})";
        }
    }
}