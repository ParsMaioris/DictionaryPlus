namespace MultiMaps.Core;

public enum HashingAlgorithm
{
    Default,
    FowlerNollVo,
    Murmur
}

public static class HashingStrategies
{
    public static IEqualityComparer<TKey> Create<TKey>(HashingAlgorithm algorithm)
    {
        return algorithm switch
        {
            HashingAlgorithm.FowlerNollVo => new Internal.FnvHashStrategy<TKey>(),
            HashingAlgorithm.Murmur => new Internal.MurmurHashStrategy<TKey>(),
            _ => EqualityComparer<TKey>.Default
        };
    }

    public static IEqualityComparer<TKey> CreateBucketed<TKey>(int bucketCount, HashingAlgorithm algorithm = HashingAlgorithm.Default)
    {
        var baseComparer = Create<TKey>(algorithm);
        return new Internal.BucketComparer<TKey>(bucketCount, baseComparer);
    }
}