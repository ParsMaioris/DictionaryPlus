using System.Collections;
using MultiMaps.Core.Internal;

namespace MultiMaps.Core;

public class OneToManyMap<TKey, TValue>
    : IEnumerable<KeyValuePair<TKey, ISet<TValue>>>
{
    private List<Bucket<TKey, TValue>>[] _bucketArray;
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly int _bucketCount;
    private int _count;

    public OneToManyMap() : this(16)
    {
    }

    public OneToManyMap(int bucketCount)
        : this(new BucketComparer<TKey>(bucketCount))
    {
    }

    public OneToManyMap(IEqualityComparer<TKey> comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

        _bucketCount = comparer is BucketComparer<TKey> bucketComparer
            ? bucketComparer.BucketCount
            : 16;

        _bucketArray = new List<Bucket<TKey, TValue>>[_bucketCount];
        for (int i = 0; i < _bucketCount; i++)
            _bucketArray[i] = new List<Bucket<TKey, TValue>>();
    }

    public ISet<TValue> this[TKey key]
    {
        get
        {
            var bucket = FindBucket(key)
                ?? throw new KeyNotFoundException($"The key '{key}' was not found.");

            return bucket.Values;
        }
    }

    public int Count => _count;

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(_count);
            foreach (var bucketList in _bucketArray)
                keys.AddRange(bucketList.Select(bucket => bucket.Key));

            return keys;
        }
    }

    public ICollection<ISet<TValue>> Values
    {
        get
        {
            var values = new List<ISet<TValue>>(_count);
            foreach (var bucketList in _bucketArray)
                values.AddRange(bucketList.Select(bucket => bucket.Values));

            return values;
        }
    }

    public void Add(TKey key, TValue value)
    {
        var bucket = FindBucket(key);
        if (bucket == null)
        {
            bucket = new Bucket<TKey, TValue>(key);
            _bucketArray[GetBucketIndex(key)].Add(bucket);
            _count++;
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
            bucket = new Bucket<TKey, TValue>(key);
            _bucketArray[GetBucketIndex(key)].Add(bucket);
            _count++;
        }

        foreach (var value in values)
            bucket.Values.Add(value);
    }

    public bool Remove(TKey key)
    {
        var bucketList = _bucketArray[GetBucketIndex(key)];

        for (int i = 0; i < bucketList.Count; i++)
        {
            if (_comparer.Equals(bucketList[i].Key, key))
            {
                bucketList.RemoveAt(i);
                _count--;
                return true;
            }
        }
        return false;
    }

    public bool RemoveValue(TKey key, TValue value)
    {
        var bucket = FindBucket(key);
        return bucket != null && bucket.Values.Remove(value);
    }

    public bool ContainsKey(TKey key) => FindBucket(key) != null;

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
        foreach (var bucketList in _bucketArray)
            bucketList.Clear();

        _count = 0;
    }

    private Bucket<TKey, TValue>? FindBucket(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return _bucketArray[GetBucketIndex(key)]
            .FirstOrDefault(b => _comparer.Equals(b.Key, key));
    }

    private int GetBucketIndex(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _comparer is BucketComparer<TKey> bucketComparer
            ? bucketComparer.GetBucketIndex(key)
            : Math.Abs(_comparer.GetHashCode(key) % _bucketCount);
    }

    public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator()
    {
        foreach (var bucketList in _bucketArray)
            foreach (var bucket in bucketList)
                yield return new KeyValuePair<TKey, ISet<TValue>>(bucket.Key, bucket.Values);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ISet<TValue> GetOrCreate(TKey key)
    {
        var bucket = FindBucket(key);
        if (bucket == null)
        {
            bucket = new Bucket<TKey, TValue>(key);
            _bucketArray[GetBucketIndex(key)].Add(bucket);
            _count++;
        }
        return bucket.Values;
    }
}