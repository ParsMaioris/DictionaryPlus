using MultiMaps.Core;

namespace MultiMaps.Tests;

[TestClass]
public class MultiMapConcurrencyTests
{
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