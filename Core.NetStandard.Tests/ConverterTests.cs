using System;
using System.Collections.Generic;
using System.Drawing;
using NightlyCode.Core.Conversion;
using NUnit.Framework;

namespace Core.Tests {

    [TestFixture]
    public class ConverterTests {
        static IEnumerable<string> Colors
        {
            get
            {
                yield return "#FF0000";
                yield return "#8893AB";
                yield return "rgb(0,0,0)";
                yield return "rgb(0.0,1.0,0.4)";
                yield return "rgba(122,200,123,255)";
                yield return "rgb(0, 2, 1)";
            }
        }

        [Test]
        public void TestLongToNullableDatetime() {
            DateTime? time = DateTime.Now;
            long ticks = time.Value.Ticks;
            DateTime? result = Converter.Convert<DateTime?>(ticks);
            Assert.AreEqual(time, result);
        }

        [Test]
        public void TestStringToTimespan() {
            TimeSpan timespan=Converter.Convert<TimeSpan>("30.00:00:00");
            Assert.AreEqual(TimeSpan.FromDays(30), timespan);
        }

        [Test]
        public void TestVersionToLong() {
            Version version = new Version(0, 9, 12345, 3233);
            long encoded = Converter.Convert<long>(version);
            Version decoded = Converter.Convert<Version>(encoded);
            Assert.AreEqual(version, decoded);

        }

        [Test]
        public void TestColorFromString([ValueSource(nameof(Colors))] string value) {
            Converter.Convert<Color>(value);
        }
    }
}