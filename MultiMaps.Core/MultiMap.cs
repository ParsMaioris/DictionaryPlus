using System.Collections;

namespace MultiMaps.Core;

public class MultiMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private const int DefaultCapacity = 64;
    private const float LoadFactorThreshold = 0.75f;

    private readonly object _syncRoot = new object();

    private int _count;
    private Bucket<TKey, TValue>[] _buckets;

    internal int Version { get; private set; }
    internal Bucket<TKey, TValue>[] Buckets => _buckets;

    public int Count => _count;

    public MultiMap(int capacity)
    {
        _buckets = new Bucket<TKey, TValue>[capacity];
    }

    public MultiMap() : this(DefaultCapacity) { }

    public void Add(TKey key, TValue value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        lock (_syncRoot)
        {
            ExpandIfNeeded();
            var bucket = GetOrCreateBucket(key);
            InsertValueIntoBucket(bucket, key, value);
        }
    }

    public IReadOnlyCollection<TValue> GetValues(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        lock (_syncRoot)
        {
            var bucket = FindBucket(key);
            if (bucket == null) return Array.Empty<TValue>();

            var entry = FindEntry(bucket, key);
            return entry == null ? Array.Empty<TValue>() : entry.Values.AsReadOnly();
        }
    }

    public bool RemoveValue(TKey key, TValue value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        lock (_syncRoot)
        {
            var bucket = FindBucket(key);
            if (bucket == null) return false;

            var entry = FindEntry(bucket, key);
            if (entry == null) return false;

            bool removed = entry.Values.Remove(value);
            if (removed)
            {
                IncrementVersion();
                if (entry.Values.Count == 0)
                {
                    RemoveKeyFromBucket(bucket, key);
                }
            }
            return removed;
        }
    }

    public bool RemoveKey(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        lock (_syncRoot)
        {
            var bucket = FindBucket(key);
            if (bucket == null) return false;

            return RemoveKeyFromBucket(bucket, key);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new KeyValueIterator<TKey, TValue>(this, _syncRoot);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void ExpandIfNeeded()
    {
        float loadFactor = (float)_count / _buckets.Length;
        if (loadFactor >= LoadFactorThreshold)
        {
            Resize(_buckets.Length * 2);
        }
    }

    private void Resize(int newCapacity)
    {
        var oldBuckets = _buckets;
        _buckets = new Bucket<TKey, TValue>[newCapacity];
        _count = 0;
        IncrementVersion();

        foreach (var oldBucket in oldBuckets)
        {
            if (oldBucket == null) continue;

            var entry = oldBucket.Head;
            while (entry != null)
            {
                foreach (var value in entry.Values)
                {
                    Add(entry.Key, value);
                }
                entry = entry.Next;
            }
        }
    }

    private void InsertValueIntoBucket(Bucket<TKey, TValue> bucket, TKey key, TValue value)
    {
        var entry = FindEntry(bucket, key);
        if (entry == null)
        {
            entry = new Entry<TKey, TValue>(key);
            entry.Values.Add(value);

            entry.Next = bucket.Head;
            bucket.Head = entry;

            _count++;
            IncrementVersion();
        }
        else
        {
            entry.Values.Add(value);
            IncrementVersion();
        }
    }

    private Bucket<TKey, TValue> GetOrCreateBucket(TKey key)
    {
        int index = GetBucketIndex(key);
        if (_buckets[index] == null)
        {
            _buckets[index] = new Bucket<TKey, TValue>();
        }
        return _buckets[index];
    }

    private Bucket<TKey, TValue>? FindBucket(TKey key)
    {
        int index = GetBucketIndex(key);
        return _buckets[index];
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

    private bool RemoveKeyFromBucket(Bucket<TKey, TValue> bucket, TKey key)
    {
        Entry<TKey, TValue>? previous = null;
        var current = bucket.Head;

        while (current != null)
        {
            if (current.Key!.Equals(key))
            {
                if (previous == null)
                {
                    bucket.Head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                _count--;
                IncrementVersion();
                return true;
            }
            previous = current;
            current = current.Next;
        }
        return false;
    }

    private int GetBucketIndex(TKey key)
    {
        return Math.Abs(key!.GetHashCode()) % _buckets.Length;
    }

    private void IncrementVersion()
    {
        Version++;
    }
}