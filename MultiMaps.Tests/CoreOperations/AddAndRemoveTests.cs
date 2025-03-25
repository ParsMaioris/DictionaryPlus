using MultiMaps.Core;

namespace MultiMaps.Tests.CoreOperations;

[TestClass]
[TestCategory("CoreOperations")]
public class AddAndRemoveTests
{
    [TestMethod]
    public void Add_SingleKeySingleValue_ShouldIncreaseCount()
    {
        var map = new MultiMap<string, int>();
        map.Add("hello", 1);

        Assert.AreEqual(1, map.Count);
    }

    [TestMethod]
    public void Add_SameKeyMultipleValues_ShouldGroupValues()
    {
        var map = new MultiMap<string, int>();
        map.Add("numbers", 10);
        map.Add("numbers", 20);
        map.Add("numbers", 30);

        Assert.AreEqual(3, map.GetValues("numbers").Count);
    }

    [TestMethod]
    public void Add_DifferentKeys_ShouldStoreSeparately()
    {
        var map = new MultiMap<string, int>();
        map.Add("A", 1);
        map.Add("B", 2);

        Assert.AreEqual(1, map.GetValues("A").Count);
        Assert.AreEqual(1, map.GetValues("B").Count);
    }

    [TestMethod]
    public void RemoveValue_ExistingValue_ShouldReduceValueCount()
    {
        var map = new MultiMap<string, int>();
        map.Add("test", 5);
        map.Add("test", 10);

        var removed = map.RemoveValue("test", 5);

        Assert.IsTrue(removed, "Expected existing value to be removed successfully.");
        Assert.AreEqual(1, map.GetValues("test").Count, "Only one value should remain.");
    }

    [TestMethod]
    public void RemoveValue_LastValue_ShouldRemoveKeyCompletely()
    {
        var map = new MultiMap<string, string>();
        map.Add("hello", "world");

        var removed = map.RemoveValue("hello", "world");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, map.Count, "Removing the last value should remove the key entirely.");
        Assert.AreEqual(0, map.GetValues("hello").Count);
    }

    [TestMethod]
    public void RemoveValue_NonExistentValue_ShouldReturnFalse()
    {
        var map = new MultiMap<string, int>();
        map.Add("key1", 100);

        var removed = map.RemoveValue("key1", 999);

        Assert.IsFalse(removed);
        Assert.AreEqual(1, map.GetValues("key1").Count);
    }

    [TestMethod]
    public void RemoveKey_ExistingKey_ShouldSucceedAndReduceCount()
    {
        var map = new MultiMap<int, int>();
        map.Add(1, 10);
        map.Add(1, 20);
        map.Add(2, 30);

        var removed = map.RemoveKey(1);

        Assert.IsTrue(removed);
        Assert.AreEqual(1, map.Count, "Expected only one remaining entry (key=2).");
    }

    [TestMethod]
    public void RemoveKey_NonExistentKey_ShouldReturnFalse()
    {
        var map = new MultiMap<string, string>();
        map.Add("x", "1");

        var removed = map.RemoveKey("unknown");

        Assert.IsFalse(removed);
        Assert.AreEqual(1, map.Count);
    }
}