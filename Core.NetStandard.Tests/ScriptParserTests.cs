using System;
using NightlyCode.Core.Script;
using NUnit.Framework;

namespace Core.Tests {

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

        [TestCase("test.member=value", typeof(ScriptMemberAssignment))]
        [TestCase("test.method(service,user,parameter)", typeof(ScriptMethodCall))]
        [TestCase("test.method(test.member)", typeof(ScriptMethodCall))]
        [TestCase("$test", typeof(ScriptValue))]
        [TestCase("test.method($test,2)", typeof(ScriptMethodCall))]
        [TestCase("test.method(\"\",clean)", typeof(ScriptMethodCall))]
        [TestCase("test.speak(It is quite simple,\"CereVoice Stuart - English (Scotland)\")", typeof(ScriptMethodCall))]
        [TestCase("test.method(1,2,3,[4,4])", typeof(ScriptMethodCall))]
        [TestCase("test.property=255.34", typeof(ScriptMemberAssignment))]
        public void TestValidStatements(string statement, Type expectedroot)
        {
            ScriptParser parser = new ScriptParser(new TestHostPool());
            IScriptToken token = parser.Parse(statement, new TestVariableHost());
            Assert.AreEqual(expectedroot, token.GetType());
        }

    }
}