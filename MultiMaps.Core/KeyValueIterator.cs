using System.Collections;

namespace MultiMaps.Core;

internal class KeyValueIterator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
{
    private readonly MultiMap<TKey, TValue> _multiMap;
    private readonly object _syncRoot;
    private readonly int _expectedVersion;

    private int _currentBucketIndex;
    private Entry<TKey, TValue>? _currentEntry;
    private int _valueIndex;
    private KeyValuePair<TKey, TValue>? _currentKeyValue;

    public KeyValueIterator(MultiMap<TKey, TValue> multiMap, object syncRoot)
    {
        _multiMap = multiMap;
        _syncRoot = syncRoot;
        _expectedVersion = multiMap.Version;

        _currentBucketIndex = -1;
        _currentEntry = null;
        _valueIndex = -1;
        _currentKeyValue = null;
    }

    public KeyValuePair<TKey, TValue> Current
    {
        get
        {
            if (_currentKeyValue == null)
                throw new InvalidOperationException("Enumeration not started or has ended.");

            return _currentKeyValue.Value;
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        lock (_syncRoot)
        {
            ValidateUnmodifiedCollection();

            if (TryMoveNextValueInCurrentEntry())
                return true;

            if (TryMoveToNextEntryInChain())
                return true;

            while (AdvanceToNextBucket())
            {
                if (_currentEntry != null && _currentEntry.Values.Count > 0)
                {
                    _currentKeyValue = CreateKeyValue(_currentEntry, 0);
                    return true;
                }
            }

            _currentKeyValue = null;
            return false;
        }
    }

    public void Reset()
    {
        lock (_syncRoot)
        {
            ValidateUnmodifiedCollection();
            ResetIterationState();
        }
    }

    public void Dispose()
    {
        // No resources to dispose
    }

    private void ValidateUnmodifiedCollection()
    {
        if (_expectedVersion != _multiMap.Version)
            throw new InvalidOperationException("Collection was modified during iteration.");
    }

    private bool TryMoveNextValueInCurrentEntry()
    {
        if (_currentEntry != null && _valueIndex < _currentEntry.Values.Count - 1)
        {
            _valueIndex++;
            _currentKeyValue = CreateKeyValue(_currentEntry, _valueIndex);
            return true;
        }
        return false;
    }

    private bool TryMoveToNextEntryInChain()
    {
        if (_currentEntry != null && _currentEntry.Next != null)
        {
            _currentEntry = _currentEntry.Next;
            _valueIndex = 0;

            if (_currentEntry.Values.Count > 0)
            {
                _currentKeyValue = CreateKeyValue(_currentEntry, _valueIndex);
                return true;
            }
        }
        return false;
    }

    private bool AdvanceToNextBucket()
    {
        if (++_currentBucketIndex >= _multiMap.Buckets.Length)
            return false;

        var bucket = _multiMap.Buckets[_currentBucketIndex];
        if (bucket == null || bucket.Head == null)
            return true;

        _currentEntry = bucket.Head;
        _valueIndex = 0;
        return true;
    }

    private KeyValuePair<TKey, TValue> CreateKeyValue(Entry<TKey, TValue> entry, int valueIndex)
    {
        return new KeyValuePair<TKey, TValue>(
            entry.Key,
            entry.Values[valueIndex]
        );
    }

    private void ResetIterationState()
    {
        _currentBucketIndex = -1;
        _currentEntry = null;
        _valueIndex = -1;
        _currentKeyValue = null;
    }
}