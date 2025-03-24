namespace MultiMaps.Core;

internal class MultiValueDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
{
    private readonly MultiValueDictionary<TKey, TValue> _dictionary;
    private int _version;
    private int _bucketIndex;
    private Entry<TKey, TValue>? _currentEntry;
    private int _valueIndex;
    private KeyValuePair<TKey, TValue>? _currentPair;

    public MultiValueDictionaryEnumerator(MultiValueDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
        _version = dictionary.Version;
        _bucketIndex = -1;
        _currentEntry = null;
        _valueIndex = -1;
        _currentPair = null;
    }

    public KeyValuePair<TKey, TValue> Current()
    {
        if (_currentPair == null)
            throw new InvalidOperationException("Enumeration not started or has ended.");

        return _currentPair.Value;
    }

    KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current => Current();

    object System.Collections.IEnumerator.Current => Current();

    public bool MoveNext()
    {
        if (_version != _dictionary.Version)
            throw new InvalidOperationException("Collection was modified during iteration");

        if (_currentEntry != null && _valueIndex < _currentEntry.Values.Count - 1)
        {
            _valueIndex++;
            _currentPair = new KeyValuePair<TKey, TValue>(
                _currentEntry.Key,
                _currentEntry.Values[_valueIndex]);
            return true;
        }

        if (_currentEntry != null && _currentEntry.Next != null)
        {
            _currentEntry = _currentEntry.Next;
            _valueIndex = 0;
            if (_currentEntry.Values.Count > 0)
            {
                _currentPair = new KeyValuePair<TKey, TValue>(
                    _currentEntry.Key,
                    _currentEntry.Values[_valueIndex]);
                return true;
            }
        }

        while (++_bucketIndex < _dictionary.Buckets.Length)
        {
            var bucket = _dictionary.Buckets[_bucketIndex];
            if (bucket == null || bucket.Head == null)
                continue;

            _currentEntry = bucket.Head;
            _valueIndex = 0;

            if (_currentEntry.Values.Count > 0)
            {
                _currentPair = new KeyValuePair<TKey, TValue>(
                    _currentEntry.Key,
                    _currentEntry.Values[_valueIndex]);
                return true;
            }
        }

        _currentPair = null;
        return false;
    }

    public void Reset()
    {
        if (_version != _dictionary.Version)
            throw new InvalidOperationException("Collection was modified during iteration");

        _bucketIndex = -1;
        _currentEntry = null;
        _valueIndex = -1;
        _currentPair = null;
    }

    public void Dispose() { }
}
