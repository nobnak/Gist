using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Gist.BoundingVolume;

public class AABBTest {

	[Test]
	public void AABBTestSimplePasses() {
        var a = new AABB();
        Assert.True(a.Empty);
        Assert.AreEqual(0f, a.SurfaceArea);
        Assert.AreEqual(Vector3.zero, a.Size);

        var p0 = Vector3.zero;
        var p1 = Vector3.one;
        a.Encapsulate(p1);
        Assert.False(a.Empty);
        Assert.True(a.Contains(p1));
        Assert.AreEqual(Vector3.zero, a.Size);

        a.Encapsulate(p0);
        Assert.True(a.Contains(p0));
        Assert.AreEqual(6f, a.SurfaceArea);
    }
}
