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

        ResetIterationState();
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
            ValidateVersionUnchanged();

            if (TryAdvanceWithinEntry()) return true;

            if (TryAdvanceToNextEntryInChain()) return true;

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
            ValidateVersionUnchanged();
            ResetIterationState();
        }
    }

    public void Dispose()
    {
        // No disposable resources.
    }

    private void ValidateVersionUnchanged()
    {
        if (_expectedVersion != _multiMap.Version)
        {
            throw new InvalidOperationException("Collection was modified during iteration.");
        }
    }

    private bool TryAdvanceWithinEntry()
    {
        if (_currentEntry == null) return false;
        if (_valueIndex < _currentEntry.Values.Count - 1)
        {
            _valueIndex++;
            _currentKeyValue = CreateKeyValue(_currentEntry, _valueIndex);
            return true;
        }
        return false;
    }

    private bool TryAdvanceToNextEntryInChain()
    {
        if (_currentEntry?.Next == null) return false;

        _currentEntry = _currentEntry.Next;
        _valueIndex = 0;

        if (_currentEntry.Values.Count > 0)
        {
            _currentKeyValue = CreateKeyValue(_currentEntry, _valueIndex);
            return true;
        }
        return false;
    }

    private bool AdvanceToNextBucket()
    {
        _currentBucketIndex++;
        if (_currentBucketIndex >= _multiMap.Buckets.Length)
        {
            return false;
        }

        var bucket = _multiMap.Buckets[_currentBucketIndex];
        if (bucket?.Head == null)
        {
            _currentEntry = null;
            return true;
        }

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