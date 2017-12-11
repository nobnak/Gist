using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace nobnak.Gist.Extensions.AABB.Test {

    public class AABBExtensionTest {
        public const float EPSILON = 1e-3f;

        [Test]
        public void AABBExtensionTestSimplePasses() {
            var rect = new Rect(0f, 0f, 1f, 1f);

            var mat2 = Matrix4x4.identity;
            var rect2 = rect.EncapsulateInTargetSpace(mat2);
            Debug.LogFormat("Rect in={0} out={1}", rect, rect2);
            Assert.AreEqual(rect.center.x, rect2.center.x, EPSILON);
            Assert.AreEqual(rect.center.y, rect2.center.y, EPSILON);
        }
    }
}