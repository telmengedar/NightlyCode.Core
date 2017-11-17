using System;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Core.Collections;
using NightlyCode.Core.Helpers;
using NUnit.Framework;

namespace GM.Core.Tests.Collections {

    [TestFixture]
    public class EnumerationExtensionsTest {

        [Test]
        public void RemoveInsignificant() {
            double[] source = new[] {0.1, 0.5, 0.7, 0.99};
            double[] target = new[] {0.5, 0.7, 0.99};

            double[] removed = source.RemoveInsignificant(d => d, 0.5).ToArray();
            Assert.AreEqual(removed.Length, target.Length);
            for(int i = 0; i < target.Length; ++i)
                Assert.AreEqual(target[i], removed[i]);
        }

        [Test]
        public void NormalizeValues() {
            double[] source = new[] { 0.1, 0.2, 0.3 };
            double[] target = new[] { 0.166666666667, 1/3.0, 0.5 };

            double[] normalized = source.NormalizeValues(d => d, (d, d1) => d1).ToArray();
            Assert.AreEqual(normalized.Length, target.Length);
            for(int i = 0; i < target.Length; ++i)
                Assert.That(Math.Abs(target[i] - normalized[i]) < 0.001);
        }

        [Test]
        public void Blocks() {
            int[] source = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
            int[][] target = new[] {new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, new[] {10, 11}};

            List<IEnumerable<int>> blocks = new List<IEnumerable<int>>();

            int index = 0;
            foreach(IEnumerable<int> block in source.Block(3)) {
                Assert.That(ArrayOperations.AreEqual(target[index++], block.ToArray()));
            }

        }
    }
}