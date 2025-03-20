using System.Collections;

namespace MultiMaps.Core.Internal;

internal class MapEnumerator<TKey, TValue>
    : IEnumerator<KeyValuePair<TKey, ISet<TValue>>>
{
    private readonly List<Bucket<TKey, TValue>>[] _bucketArray;
    private int _arrayIndex;
    private int _listIndex;
    private KeyValuePair<TKey, ISet<TValue>> _current;

    public MapEnumerator(List<Bucket<TKey, TValue>>[] bucketArray)
    {
        _bucketArray = bucketArray;
        _arrayIndex = 0;
        _listIndex = -1;
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
        while (_arrayIndex < _bucketArray.Length)
        {
            if (_listIndex + 1 < _bucketArray[_arrayIndex].Count)
            {
                _listIndex++;
                var bucket = _bucketArray[_arrayIndex][_listIndex];
                _current = new KeyValuePair<TKey, ISet<TValue>>(
                    bucket.Key,
                    bucket.Values);

                return true;
            }

            _arrayIndex++;
            _listIndex = -1;
        }

        return false;
    }

    public void Reset()
    {
        _arrayIndex = 0;
        _listIndex = -1;
        _current = default;
    }
}