using System.Collections;

namespace MultiMaps.Core;

internal class KeyValueIterator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
{
    private readonly MultiMap<TKey, TValue> _multiMap;
    private readonly object _syncRoot;
    private int _expectedVersion;
    private int _bucketIndex;
    private Entry<TKey, TValue>? _currentEntry;
    private int _valueIndex;
    private KeyValuePair<TKey, TValue>? _currentPair;

    public KeyValueIterator(
        MultiMap<TKey, TValue> dictionary,
        object syncRoot)
    {
        _multiMap = dictionary;
        _syncRoot = syncRoot;
        _expectedVersion = dictionary.Version;
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

    object IEnumerator.Current => Current();

    public bool MoveNext()
    {
        lock (_syncRoot)
        {
            if (_expectedVersion != _multiMap.Version)
                throw new InvalidOperationException("Collection was modified during iteration.");

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

            while (++_bucketIndex < _multiMap.Buckets.Length)
            {
                var bucket = _multiMap.Buckets[_bucketIndex];
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
    }

    public void Reset()
    {
        lock (_syncRoot)
        {
            if (_expectedVersion != _multiMap.Version)
                throw new InvalidOperationException("Collection was modified during iteration.");

            _bucketIndex = -1;
            _currentEntry = null;
            _valueIndex = -1;
            _currentPair = null;
        }
    }

    public void Dispose()
    {
    }

    private void ValidateUnmodifiedCollection()
    {
        if (_expectedVersion != _multiMap.Version)
        {
            throw new InvalidOperationException(
                "Collection was modified during iteration.");
        }
    }

    private bool TryMoveNextEntryInChain()
    {
        if (_currentEntry != null && _currentEntry.Next != null)
        {
            _currentEntry = _currentEntry.Next;
            _valueIndex = 0;

            if (_currentEntry.Values.Count > 0)
            {
                //_currentEntry = CreateKeyValue(_currentEntry, _valueIndex);
                return true;
            }
        }

        return false;
    }
}