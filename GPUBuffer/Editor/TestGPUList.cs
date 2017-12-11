using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.GPUBuffer {

    public class TestGPUList {

        [Test]
        public void TestGPUListSimplePasses() {
            var length = 16;
            var listA = new GPUList<int>();

            var data = new List<int>();
            for (var i = 0; i < length; i++) {
                var v = Random.Range(0, 9);
                data.Add(v);
                listA.Add(v);
                Assert.AreEqual(v, listA[i]);
                Assert.AreEqual(listA.Count, i + 1);
            }

            Assert.AreEqual(data.Count, listA.Count);
            Assert.True(listA.Upload());
            listA.Download();

            for (var i = 0; i < data.Count; i++) {
                Assert.AreEqual(data[i], listA[i]);
            }

            for (var i = 0; i < length; i++) {
                var j = Random.Range(0, data.Count - 1);
                data.RemoveAt(j);
                listA.RemoveAt(j);
                AreEquals(data, listA);
            }

            listA.Dispose();
        }

        private static void AreEquals<T>(IList<T> a, IList<T> b) {
            Assert.AreEqual(a.Count, b.Count);
            for (var i = 0; i < a.Count; i++)
                Assert.AreEqual(a[i], b[i]);
        }
    }
}
