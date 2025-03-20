namespace MultiMaps.Core.Internal;

public class BucketComparer<TKey> : IEqualityComparer<TKey>
{
    private readonly int _bucketCount;
    private readonly IEqualityComparer<TKey> _innerComparer;

    public BucketComparer(int bucketCount) : this(bucketCount, EqualityComparer<TKey>.Default)
    {
    }

    public BucketComparer(int bucketCount, IEqualityComparer<TKey> innerComparer)
    {
        _bucketCount = bucketCount > 0 ? bucketCount : throw new ArgumentOutOfRangeException(nameof(bucketCount), "Bucket count must be positive");
        _innerComparer = innerComparer ?? throw new ArgumentNullException(nameof(innerComparer));
    }

    public bool Equals(TKey? x, TKey? y)
    {
        return _innerComparer.Equals(x, y);
    }

    public int GetHashCode(TKey obj)
    {
        if (obj == null)
            return 0;

        var originalHash = _innerComparer.GetHashCode(obj);
        return Math.Abs(originalHash % _bucketCount);
    }

    public int GetBucketIndex(TKey key)
    {
        return GetHashCode(key);
    }

    public int BucketCount => _bucketCount;
}