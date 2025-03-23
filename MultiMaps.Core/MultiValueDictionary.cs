namespace MultiMaps.Core;

public class MultiValueDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 64;
    private const float LoadFactorThreshold = 0.75f;

    private Bucket<TKey, TValue>[] _buckets;
    private int _count;

    public MultiValueDictionary(int capacity)
    {
        _buckets = new Bucket<TKey, TValue>[capacity];
    }

    public MultiValueDictionary() : this(DefaultCapacity) { }

    private void Resize(int newCapacity)
    {
        var oldBuckets = _buckets;
        _buckets = new Bucket<TKey, TValue>[newCapacity];
        _count = 0;

        foreach (var bucket in oldBuckets)
        {
            if (bucket == null) continue;

            var entry = bucket.Head;
            while (entry != null)
            {
                foreach (var value in entry.Values)
                {
                    // Add(entry.Key, value);
                }

                entry = entry.Next;
            }
        }
    }

    private void EnsureCapacity()
    {
        float loadFactor = (float)_count / _buckets.Length;
        if (loadFactor >= LoadFactorThreshold)
        {
            Resize(_buckets.Length * 2);
        }
    }

    private int GetIndex(TKey key)
    {
        return Math.Abs(key!.GetHashCode()) % _buckets.Length;
    }

    private Entry<TKey, TValue>? FindEntry(Bucket<TKey, TValue> bucket, TKey key)
    {
        var current = bucket.Head;
        while (current != null)
        {
            if (current.Key!.Equals(key))
            {
                return current;
            }

            current = current.Next;
        }
        return null;
    }
}