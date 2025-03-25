using MultiMaps.Core;

namespace MultiMaps.Tests.Resizing;

[TestClass]
[TestCategory("Resizing")]
public class ResizeHandlingTests
{
    [TestMethod]
    public void CapacityConstructor_InitialCapacityAffectsLoadFactor()
    {
        var initialCapacity = 2;
        var map = new MultiMap<string, string>(initialCapacity);

        map.Add("a", "1");
        map.Add("b", "2");
        map.Add("c", "3");

        Assert.AreEqual(3, map.Count, "Expected all items to remain after any resize.");
    }

    [TestMethod]
    public void Resize_WhenLoadFactorExceeded_ShouldRehashExistingItems()
    {
        var map = new MultiMap<int, string>(2);
        map.Add(1, "One");
        map.Add(2, "Two");
        map.Add(3, "Three");

        Assert.AreEqual("One", map.GetValues(1).First());
        Assert.AreEqual("Two", map.GetValues(2).First());
        Assert.AreEqual("Three", map.GetValues(3).First());
    }

    [TestMethod]
    public void GetValues_AfterResize_StillReturnsCorrectValues()
    {
        var map = new MultiMap<string, string>(2);
        map.Add("apple", "red");
        map.Add("banana", "yellow");
        map.Add("grape", "purple"); // triggers resize

        var appleValues = map.GetValues("apple");
        var bananaValues = map.GetValues("banana");
        var grapeValues = map.GetValues("grape");

        Assert.AreEqual(1, appleValues.Count);
        Assert.AreEqual(1, bananaValues.Count);
        Assert.AreEqual(1, grapeValues.Count);
        Assert.IsTrue(appleValues.Contains("red"));
        Assert.IsTrue(bananaValues.Contains("yellow"));
        Assert.IsTrue(grapeValues.Contains("purple"));
    }
}