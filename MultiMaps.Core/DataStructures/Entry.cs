namespace MultiMaps.Core;

internal class Entry<TKey, TValue>
{
    public TKey Key { get; }
    public List<TValue> Values { get; }
    public Entry<TKey, TValue>? Next { get; set; }

    public Entry(TKey key)
    {
        Key = key;
        Values = new List<TValue>();
    }
}