using MultiMaps.Core;

namespace MultiMaps.Tests.Concurrency;

[TestClass]
[TestCategory("Concurrency")]
public class ConcurrentResizeTests
{
    [TestMethod]
    public void ResizeDuringConcurrentAdds_ShouldRemainConsistent()
    {
        var map = new MultiMap<int, int>(capacity: 2);

        int totalTasks = 8;
        int itemsPerTask = 500;

        Parallel.For(0, totalTasks, t =>
        {
            for (int i = 0; i < itemsPerTask; i++)
            {
                map.Add(t * 10_000 + i, i);
            }
        });

        var expectedCount = totalTasks * itemsPerTask;
        Assert.AreEqual(expectedCount, map.Count,
            $"Expected {expectedCount} items after parallel adds with resizing.");
    }

    [TestMethod]
    public void ConcurrentResize_LargeDataSet_ShouldNotLoseData()
    {
        var map = new MultiMap<int, string>(2);
        int numItems = 2000;

        Parallel.For(0, numItems, i =>
        {
            map.Add(i, $"Value_{i}");
        });

        for (int i = 0; i < numItems; i++)
        {
            var values = map.GetValues(i);
            Assert.AreEqual(1, values.Count,
                $"Expected exactly one value for key {i}. Found {values.Count}.");
        }
    }
}