namespace MultiMaps.Core;

internal class Bucket<TKey, TValue>
{
    public Entry<TKey, TValue>? Head { get; set; }
}
