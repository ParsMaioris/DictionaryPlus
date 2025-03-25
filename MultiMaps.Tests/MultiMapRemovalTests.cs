using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiMapRemovalTests
{
    [TestMethod]
    public void RemoveValue_RemovesCorrectElement()
    {
        var dictionary = new MultiMap<string, int>();
        dictionary.Add("numbers", 42);
        dictionary.Add("numbers", 100);

        bool removed = dictionary.RemoveValue("numbers", 42);
        var values = dictionary.GetValues("numbers");

        Assert.IsTrue(removed);
        Assert.AreEqual(1, values.Count);
        Assert.AreEqual(100, values.First());
    }

    [TestMethod]
    public void RemoveKey_RemovesAllAssociatedValues()
    {
        var dictionary = new MultiMap<string, int>();
        dictionary.Add("letters", 65);
        dictionary.Add("letters", 66);

        bool removed = dictionary.RemoveKey("letters");
        var values = dictionary.GetValues("letters");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, values.Count);
    }
}