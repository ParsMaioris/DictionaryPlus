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
    }
}
