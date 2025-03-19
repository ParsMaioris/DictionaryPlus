using System.Collections;

namespace MultiMaps.Core;

internal class MapEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, ISet<TValue>>>
{
    private readonly List<Bucket<TKey, TValue>> _buckets;
    private int _currentIndex;
    private KeyValuePair<TKey, ISet<TValue>> _current;

    public MapEnumerator(List<Bucket<TKey, TValue>> buckets)
    {
        _buckets = buckets;
        _currentIndex = -1;
        _current = default;
    }

    public KeyValuePair<TKey, ISet<TValue>> Current => _current;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        // No unmanaged resources to dispose
    }

    public bool MoveNext()
    {
        if (_currentIndex < _buckets.Count - 1)
        {
            _currentIndex++;
            var bucket = _buckets[_currentIndex];
            _current = new KeyValuePair<TKey, ISet<TValue>>(bucket.Key, bucket.Values);
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _currentIndex = -1;
        _current = default;
    }
}