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

    [TestMethod]
    public void RemoveKeyTest()
    {
        var dictionary = new MultiValueDictionary<string, int>();
        dictionary.Add("letters", 65);
        dictionary.Add("letters", 66);

        bool removed = dictionary.RemoveKey("letters");
        var values = dictionary.GetValues("letters");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, values.Count);
    }

    [TestMethod]
    public void TestIteration()
    {
        var dict = new MultiValueDictionary<string, int>();
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
    public void TestModifyDuringEnumeration_Throws()
    {
        var dict = new MultiValueDictionary<string, int>();
        dict.Add("test", 42);

        var enumerator = dict.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());

        dict.Add("another", 100);

        Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
    }

    [TestMethod]
    public void ConcurrentAddTest()
    {
        var dict = new MultiValueDictionary<string, int>();

        int taskCount = 5;
        int itemsPerTask = 1000;

        var tasks = new List<Task>();

        for (int t = 0; t < taskCount; t++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < itemsPerTask; i++)
                {
                    dict.Add($"Key_{i}", i);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        Assert.IsTrue(dict.Count > 0);

        var valuesForKey42 = dict.GetValues("Key_42");

        Assert.IsTrue(valuesForKey42.Count >= 1);
    }
}