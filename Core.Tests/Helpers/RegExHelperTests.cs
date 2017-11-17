using System.Text.RegularExpressions;
using NightlyCode.Core.Helpers;
using NUnit.Framework;

namespace GM.Core.Tests.Helpers {

    [TestFixture]
    public class RegExHelperTests {
     
        [Test]
        public void FloatingPointPattern() {
            Assert.That(Regex.Match("5", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match("23428", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match("5.2", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match("5.242352", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match("235.3243", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match(".22", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match(".5", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
            Assert.That(Regex.Match("0.0", string.Format("^{0}$", RegExHelper.FloatingPointPattern)).Success);
        }
    }
}