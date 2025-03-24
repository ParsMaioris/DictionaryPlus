namespace MultiMaps.Core;

internal class MultiValueDictionaryEnumerator<TKey, TValue> //: IEnumerator<KeyValuePair<TKey, TValue>>
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

    // object IEnumerator.Current => Current;
}
