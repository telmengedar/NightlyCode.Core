using System.Linq;
using System.Reflection;
using NightlyCode.Core.Reflection;
using NUnit.Framework;

namespace Core.Tests {

    [TestFixture]
    public class ReflectorTests {
        public interface IBaseInterface {

            void Method1();

            void Method2(string first);
        }

        public interface IInterface : IBaseInterface {

            void Method3();
        }

        [Test]
        public void FindBaseMethod() {
            MethodInfo method = Reflector.FindMethod(typeof(IInterface), "Method2", new object[] {"bla"}).FirstOrDefault();
            Assert.NotNull(method);
        }
    }
}