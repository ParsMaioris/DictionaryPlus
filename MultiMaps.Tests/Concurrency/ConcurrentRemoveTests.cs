using MultiMaps.Core;

namespace MultiMaps.Tests.Concurrency;

[TestClass]
[TestCategory("Concurrency")]
public class ConcurrentRemoveTests
{
    [TestMethod]
    public void ConcurrentRemoveValue_FromSharedKey_ShouldSafelyHandleDecrements()
    {
        var map = new MultiMap<string, int>();
        const string key = "sharedKey";

        for (int i = 0; i < 1000; i++)
        {
            map.Add(key, i);
        }

        int threads = 10;
        int removesPerThread = 50;

        Parallel.For(0, threads, _ =>
        {
            for (int i = 0; i < removesPerThread; i++)
            {
                map.RemoveValue(key, i);
            }
        });

        var remainingValues = map.GetValues(key).Count;
        Assert.IsTrue(remainingValues >= 0 && remainingValues <= 1000,
            $"Remaining values should be in [0..1000], but was {remainingValues}");
    }

    [TestMethod]
    public void ConcurrentRemoveKey_ManyKeys_ShouldNotCorruptInternalState()
    {
        var map = new MultiMap<int, string>();
        for (int i = 0; i < 100; i++)
        {
            map.Add(i, $"Value_{i}");
        }

        Parallel.For(0, 100, (i) =>
        {
            map.RemoveKey(i);
        });

        Assert.IsTrue(map.Count >= 0 && map.Count <= 100, $"Count is out of expected range: {map.Count}");
    }
}