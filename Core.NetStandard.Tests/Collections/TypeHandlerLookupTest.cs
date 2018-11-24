using NightlyCode.Core.Collections;
using NUnit.Framework;

namespace Core.Tests.Collections {

    [TestFixture]
    public class TypeHandlerLookupTest {
         
        [Test]
        public void GetObjectHandler() {
            TypeHandlerLookup<int> lookup = new TypeHandlerLookup<int>();
            lookup[typeof(object)] = 7;
            Assert.AreEqual(7, lookup[typeof(object)]);
        }

        [Test]
        public void GetBaseHandler() {
            TypeHandlerLookup<int> lookup = new TypeHandlerLookup<int>();
            lookup[typeof(object)] = 9;
            Assert.AreEqual(9, lookup[typeof(int)]);
        }

        [Test]
        public void GetSpecificType() {
            TypeHandlerLookup<int> lookup = new TypeHandlerLookup<int>();
            lookup[typeof(int)] = 11;
            Assert.AreEqual(11, lookup[typeof(int)]);
        }

        [Test]
        public void GetDefinedBaseHandlerFromSpecific() {
            TypeHandlerLookup<int> lookup=new TypeHandlerLookup<int>();
            lookup[typeof(TypeHandlerBase)] = 9;
            Assert.AreEqual(9, lookup[typeof(TypeHandlerDerived)]);
        }

        class TypeHandlerBase {}
        class TypeHandlerDerived:TypeHandlerBase{}
    }
}