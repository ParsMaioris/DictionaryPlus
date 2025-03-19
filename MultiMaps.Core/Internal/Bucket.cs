namespace MultiMaps.Core.Internal;

internal class Bucket<TKey, TValue>
{
    public TKey Key { get; }
    public HashSet<TValue> Values { get; }

    public Bucket(TKey key)
    {
        Key = key;
        Values = new HashSet<TValue>();
    }
}