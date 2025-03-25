using MultiMaps.Core;

namespace MultiMaps.Tests.Concurrency;

[TestClass]
[TestCategory("Concurrency")]
public class ConcurrentAddTests
{
    [TestMethod]
    public void ConcurrentAdd_SameKey_ShouldAggregateValuesWithoutErrors()
    {
        var map = new MultiMap<string, int>();
        int threads = 10;
        int addsPerThread = 100;

        Parallel.For(0, threads, _ =>
        {
            for (int i = 0; i < addsPerThread; i++)
            {
                map.Add("sharedKey", i);
            }
        });

        Assert.AreEqual(threads * addsPerThread, map.GetValues("sharedKey").Count);
    }

    [TestMethod]
    public void ConcurrentAdd_DifferentKeys_ShouldMaintainCorrectCount()
    {
        var map = new MultiMap<string, int>();
        int threads = 5;
        int keysPerThread = 20;

        Parallel.For(0, threads, t =>
        {
            for (int i = 0; i < keysPerThread; i++)
            {
                var key = $"Key_{t}_{i}";
                map.Add(key, i);
            }
        });

        Assert.AreEqual(threads * keysPerThread, map.Count, "Each key should exist exactly once.");
    }
}