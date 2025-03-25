using MultiMaps.Core;

namespace MultiMaps.Tests.Enumerations;

[TestClass]
[TestCategory("Enumeration")]
public class MultiMapIteratorTests
{
    [TestMethod]
    public void Iterator_ShouldIterateOverAllKeyValuePairs()
    {
        var map = new MultiMap<string, int>();
        map.Add("x", 1);
        map.Add("y", 2);
        map.Add("y", 3);
        map.Add("z", 4);

        var list = new List<KeyValuePair<string, int>>();
        foreach (var kvp in map)
        {
            list.Add(kvp);
        }

        Assert.AreEqual(4, list.Count);
        CollectionAssert.Contains(list, new KeyValuePair<string, int>("x", 1));
        CollectionAssert.Contains(list, new KeyValuePair<string, int>("y", 2));
        CollectionAssert.Contains(list, new KeyValuePair<string, int>("y", 3));
        CollectionAssert.Contains(list, new KeyValuePair<string, int>("z", 4));
    }

    [TestMethod]
    public void Iterator_ModifyCollectionDuringIteration_ShouldThrow()
    {
        var map = new MultiMap<string, int>();
        map.Add("alpha", 1);
        map.Add("beta", 2);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            foreach (var kvp in map)
            {
                map.Add("gamma", 3); // triggers modification
            }
        });
    }

    [TestMethod]
    public void Iterator_Reset_ShouldRestartIteration()
    {
        var map = new MultiMap<int, int>();
        map.Add(100, 200);
        map.Add(101, 201);

        var enumerator = map.GetEnumerator();

        // Move next once
        Assert.IsTrue(enumerator.MoveNext());

        // Reset
        enumerator.Reset();

        // Move again from start
        Assert.IsTrue(enumerator.MoveNext());
        enumerator.Dispose();
    }

    [TestMethod]
    public void EmptyMultiMap_EnumeratorMoveNextShouldReturnFalse()
    {
        var map = new MultiMap<string, string>();
        using var enumerator = map.GetEnumerator();

        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void KeyValueIterator_CurrentBeforeMoveNext_ShouldThrow()
    {
        var map = new MultiMap<string, string>();
        map.Add("k1", "v1");
        var enumerator = map.GetEnumerator();

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            _ = enumerator.Current; // attempt before MoveNext
        });
    }

    [TestMethod]
    public void KeyValueIterator_CurrentAfterEnd_ShouldThrow()
    {
        var map = new MultiMap<string, string>();
        map.Add("k1", "v1");
        var enumerator = map.GetEnumerator();

        Assert.IsTrue(enumerator.MoveNext()); // first value
        Assert.IsFalse(enumerator.MoveNext()); // end reached

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            _ = enumerator.Current; // attempt after end
        });
    }
}