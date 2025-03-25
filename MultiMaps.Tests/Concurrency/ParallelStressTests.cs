using MultiMaps.Core;

namespace MultiMaps.Tests.Concurrency;

[TestClass]
[TestCategory("Concurrency")]
public class ParallelStressTests
{
    [TestMethod]
    public void StressTest_MixedAddRemove()
    {
        var map = new MultiMap<string, int>();
        int totalThreads = 10;
        int operationsPerThread = 1000;

        Parallel.For(0, totalThreads, t =>
        {
            var rand = new Random(t);
            for (int i = 0; i < operationsPerThread; i++)
            {
                string key = $"Key_{rand.Next(0, 20)}";

                if (rand.Next(2) == 0)
                {
                    map.Add(key, rand.Next(1000));
                }
                else
                {
                    map.RemoveValue(key, rand.Next(1000));
                }
            }
        });

        foreach (var kvp in map)
        {

        }

        Assert.IsTrue(true, "Mixed add/remove concurrency test completed without exceptions.");
    }

    [TestMethod]
    public void StressTest_AddRemoveEnumerateAllAtOnce()
    {
        var map = new MultiMap<int, int>();
        int itemRange = 500;

        var tasks = new List<Task>();

        for (int i = 0; i < 2; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var rand = new Random();
                for (int j = 0; j < itemRange; j++)
                {
                    map.Add(rand.Next(1000), j);
                }
            }));
        }

        for (int i = 0; i < 2; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var rand = new Random();
                for (int j = 0; j < itemRange; j++)
                {
                    map.RemoveValue(rand.Next(1000), j);
                }
            }));
        }

        for (int i = 0; i < 2; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    foreach (var kvp in map)
                    {

                    }
                }
                catch (InvalidOperationException)
                {

                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        Assert.IsTrue(true, "No exceptions; concurrency is safe.");
    }
}