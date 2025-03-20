namespace MultiMaps.Core.Internal;

public class FnvHashStrategy<TKey> : IEqualityComparer<TKey>
{
    private const uint FNV_PRIME = 16777619;
    private const uint FNV_OFFSET_BASIS = 2166136261;
    private readonly IEqualityComparer<TKey> _innerComparer;

    public FnvHashStrategy() : this(EqualityComparer<TKey>.Default)
    {
    }

    public FnvHashStrategy(IEqualityComparer<TKey> innerComparer)
    {
        _innerComparer = innerComparer ?? throw new ArgumentNullException(nameof(innerComparer));
    }

    public bool Equals(TKey? x, TKey? y)
    {
        return _innerComparer.Equals(x, y);
    }

    public int GetHashCode(TKey obj)
    {
        if (obj == null)
            return 0;

        var originalHash = _innerComparer.GetHashCode(obj);

        uint hash = FNV_OFFSET_BASIS;

        var bytes = BitConverter.GetBytes(originalHash);
        foreach (var b in bytes)
        {
            hash ^= b;
            hash *= FNV_PRIME;
        }

        return (int)hash;
    }
}

public class MurmurHashStrategy<TKey> : IEqualityComparer<TKey>
{
    private const uint SEED = 0x9747b28c;
    private const uint M = 0x5bd1e995;
    private const int R = 24;
    private readonly IEqualityComparer<TKey> _innerComparer;

    public MurmurHashStrategy() : this(EqualityComparer<TKey>.Default)
    {
    }

    public MurmurHashStrategy(IEqualityComparer<TKey> innerComparer)
    {
        _innerComparer = innerComparer ?? throw new ArgumentNullException(nameof(innerComparer));
    }

    public bool Equals(TKey? x, TKey? y)
    {
        return _innerComparer.Equals(x, y);
    }

    public int GetHashCode(TKey obj)
    {
        if (obj == null)
            return 0;

        var originalHash = _innerComparer.GetHashCode(obj);

        var bytes = BitConverter.GetBytes(originalHash);
        uint h = SEED ^ (uint)bytes.Length;

        foreach (var b in bytes)
        {
            uint k = b;
            k *= M;
            k ^= k >> R;
            k *= M;

            h *= M;
            h ^= k;
        }

        h ^= h >> 13;
        h *= M;
        h ^= h >> 15;

        return (int)h;
    }
}