using MultiMaps.Core;

namespace MultiMaps.Tests.Concurrency;

[TestClass]
[TestCategory("Concurrency")]
public class ConcurrentIterationTests
{
    [TestMethod]
    public void ConcurrentIteration_WhileModifying_ShouldThrow()
    {
        var map = new MultiMap<int, string>();
        for (int i = 0; i < 50; i++)
        {
            map.Add(i, $"Val_{i}");
        }

        Task modifyingTask = Task.Run(() =>
        {
            for (int i = 50; i < 100; i++)
            {
                map.Add(i, $"Val_{i}");
                Thread.Sleep(2);
            }
        });

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            foreach (var kvp in map)
            {
                Thread.Sleep(1);
            }
        });

        modifyingTask.Wait();
    }

    [TestMethod]
    public void ConcurrentRead_WithLocking_ShouldSucceedIfUnmodified()
    {
        var map = new MultiMap<string, int>();
        for (int i = 0; i < 10_000; i++)
        {
            map.Add($"Key{i}", i);
        }

        Parallel.For(0, 5, _ =>
        {
            foreach (var kvp in map)
            {
                // No-Op
            }
        });

        Assert.IsTrue(true, "Concurrent enumeration with no writes should succeed without exceptions.");
    }
}