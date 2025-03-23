using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiValueDictionaryTests
{
    [TestMethod]
    public void AddAndGetValuesTest()
    {
        var dictionary = new MultiValueDictionary<string, int>();
        dictionary.Add("fruits", 1);
        dictionary.Add("fruits", 2);
        dictionary.Add("fruits", 3);

        var values = dictionary.GetValues("fruits");

        Assert.AreEqual(3, values.Count);
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, values.ToArray());
    }

    [TestMethod]
    public void RemoveValueTest()
    {
        var dictionary = new MultiValueDictionary<string, int>();
        dictionary.Add("numbers", 42);
        dictionary.Add("numbers", 100);

        bool removed = dictionary.RemoveValue("numbers", 42);
        var values = dictionary.GetValues("numbers");

        Assert.IsTrue(removed);
        Assert.AreEqual(1, values.Count);
        Assert.AreEqual(100, values.First());
    }
}
