using System;
using NightlyCode.Core.Script;
using NUnit.Framework;

namespace NightlyCode.Core.Tests {

    [TestFixture]
    public class ScriptParserTests {
        class TestHostPool : IScriptHostPool
        {
            public object GetHost(string name)
            {
                if (name == "test")
                    return this;
                throw new Exception($"host {name} unknown");
            }
        }

        class TestVariableHost : IScriptVariableHost
        {
            public IScriptToken GetVariable(string name)
            {
                if (name == "test")
                    return new ScriptValue("test");
                throw new Exception($"variable {name} unknown");
            }
        }

        [TestCase("test.member=value")]
        [TestCase("test.method(service,user,parameter)")]
        [TestCase("test.method(test.member)")]
        [TestCase("$test")]
        [TestCase("test.method($test,2)")]
        [TestCase("test.method(\"\",clean)")]
        [TestCase("test.speak(It is quite simple,\"CereVoice Stuart - English (Scotland)\")")]
        public void TestValidStatements(string statement)
        {
            ScriptParser parser = new ScriptParser(new TestHostPool());
            IScriptToken token = parser.Parse(statement, new TestVariableHost());
            Assert.Pass();
        }

    }
}