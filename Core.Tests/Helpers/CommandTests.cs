using System;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Core.Helpers;
using NUnit.Framework;

namespace GoorooMania.Core.Tests.Helpers {

    [TestFixture]
    public class CommandTests {

        IEnumerable<Tuple<string, string[]>> TestCases
        {
            get
            {
                yield return new Tuple<string, string[]>(
                    "item1 item2 item3",
                    new[] {
                        "item1", "item2", "item3"
                    }
                    );

                yield return new Tuple<string, string[]>(
                    "item1 item2   item3",
                    new[] {
                        "item1", "item2", "item3"
                    }
                    );

                yield return new Tuple<string, string[]>(
                    "item1 \"item2 item3\"",
                    new[] {
                        "item1", "item2 item3"
                    }
                    );

                yield return new Tuple<string, string[]>(
                    "item1 \"item2 \\\"\\\" item3\"",
                    new[] {
                        "item1", "item2 \"\" item3"
                    }
                    );

            }
        }

        [Test]
        public void TestCommandSplit([ValueSource(nameof(TestCases))] Tuple<string, string[]> testcase) {
            string[] arguments = Commands.SplitArguments(testcase.Item1).ToArray();
            Assert.AreEqual(testcase.Item2.Length, arguments.Length);
            for(int i = 0; i < arguments.Length; ++i) {
                Assert.AreEqual(testcase.Item2[i], arguments[i]);
            }
        }
    }
}