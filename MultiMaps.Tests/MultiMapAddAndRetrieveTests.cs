using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiMapAddAndRetrieveTests
{
    [TestMethod]
    public void AddAndGetValues_StoresAndReturnsAllValues()
    {
        var dictionary = new MultiMap<string, int>();
        dictionary.Add("fruits", 1);
        dictionary.Add("fruits", 2);
        dictionary.Add("fruits", 3);

        var values = dictionary.GetValues("fruits");

        Assert.AreEqual(3, values.Count);
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, values.ToArray());
    }
}

