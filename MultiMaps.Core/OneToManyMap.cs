using System.Collections;

namespace MultiMaps.Core;

public class OneToManyMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ISet<TValue>>>
{
    private class Bucket
    {
        public TKey Key { get; }
        public HashSet<TValue> Values { get; }

        public Bucket(TKey key)
        {
            Key = key;
            Values = new HashSet<TValue>();
        }
    }

    private readonly List<Bucket> _buckets;
    private readonly IEqualityComparer<TKey> _comparer;

    public OneToManyMap() : this(EqualityComparer<TKey>.Default)
    {
    }

    public OneToManyMap(IEqualityComparer<TKey> comparer)
    {
        _buckets = new List<Bucket>();
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public ISet<TValue> this[TKey key]
    {
        get
        {
            var bucket = FindBucket(key);
            if (bucket == null)
            {
                throw new KeyNotFoundException($"The key '{key}' was not found in the dictionary.");
            }
            return bucket.Values;
        }
    }

    public int Count => _buckets.Count;

    public ICollection<TKey> Keys => _buckets.Select(b => b.Key).ToList();

    public ICollection<ISet<TValue>> Values => _buckets.Select(b => (ISet<TValue>)b.Values).ToList();

    public void Add(TKey key, TValue value)
    {
        var bucket = FindBucket(key);
        if (bucket == null)
        {
            bucket = new Bucket(key);
            _buckets.Add(bucket);
        }
        bucket.Values.Add(value);
    }

    public void AddRange(TKey key, IEnumerable<TValue> values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        var bucket = FindBucket(key);
        if (bucket == null)
        {
            bucket = new Bucket(key);
            _buckets.Add(bucket);
        }

        foreach (var value in values)
        {
            bucket.Values.Add(value);
        }
    }

    public bool Remove(TKey key)
    {
        for (int i = 0; i < _buckets.Count; i++)
        {
            if (_comparer.Equals(_buckets[i].Key, key))
            {
                _buckets.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool RemoveValue(TKey key, TValue value)
    {
        var bucket = FindBucket(key);
        if (bucket != null)
        {
            return bucket.Values.Remove(value);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return FindBucket(key) != null;
    }

    public bool ContainsValue(TKey key, TValue value)
    {
        var bucket = FindBucket(key);
        return bucket != null && bucket.Values.Contains(value);
    }

    public bool TryGetValues(TKey key, out ISet<TValue> values)
    {
        var bucket = FindBucket(key);
        if (bucket != null)
        {
            values = bucket.Values;
            return true;
        }
        values = new HashSet<TValue>();
        return false;
    }

    public void Clear()
    {
        _buckets.Clear();
    }

    private Bucket? FindBucket(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return _buckets.FirstOrDefault(b => _comparer.Equals(b.Key, key));
    }

    public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator()
    {
        foreach (var bucket in _buckets)
        {
            yield return new KeyValuePair<TKey, ISet<TValue>>(bucket.Key, bucket.Values);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ISet<TValue> GetOrCreate(TKey key)
    {
        var bucket = FindBucket(key);
        if (bucket == null)
        {
            bucket = new Bucket(key);
            _buckets.Add(bucket);
        }
        return bucket.Values;
    }
}