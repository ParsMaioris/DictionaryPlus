using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiMapEnumerationAndConcurrencyTests
{
    [TestMethod]
    public void Iteration_EnumeratesAllKeyValues()
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
    public void ModifyDuringEnumeration_ThrowsInvalidOperationException()
    {
        var dict = new MultiMap<string, int>();
        dict.Add("test", 42);

        var enumerator = dict.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());

        dict.Add("another", 100);

        Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
    }

    [TestMethod]
    public void ConcurrentAdd_MultipleTasksSuccessfullyAddItems()
    {
        var dict = new MultiMap<string, int>();

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