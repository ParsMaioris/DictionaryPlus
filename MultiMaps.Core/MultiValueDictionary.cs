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
}
