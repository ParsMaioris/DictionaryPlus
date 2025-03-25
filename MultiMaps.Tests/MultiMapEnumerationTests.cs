using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiMapEnumerationTests
{
    [TestMethod]
    public void Iteration_EnumeratesAllKeyValuePairs()
    {
        var dict = new MultiMap<string, int>();
        dict.Add("A", 1);
        dict.Add("A", 2);
        dict.Add("B", 10);

        var pairs = new List<KeyValuePair<string, int>>();
        foreach (var kvp in dict)
        {
            pairs.Add(kvp);
        }

        Assert.AreEqual(3, pairs.Count);
        Assert.IsTrue(pairs.Any(p => p.Key == "A" && p.Value == 1));
        Assert.IsTrue(pairs.Any(p => p.Key == "A" && p.Value == 2));
        Assert.IsTrue(pairs.Any(p => p.Key == "B" && p.Value == 10));
    }

    [TestMethod]
    public void ModifyDuringIteration_ThrowsInvalidOperation()
    {
        var dict = new MultiMap<string, int>();
        dict.Add("test", 42);

        var enumerator = dict.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());

        dict.Add("another", 100);

        Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
    }
}