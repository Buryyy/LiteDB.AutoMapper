namespace Mapper.Core
{
    internal interface IMappedStore<in TKey, TValue>
    {
        void StoreOrUpdate(TKey key, TValue value);

        void Remove(TKey key);

        TValue Get(TKey key);

        void Clear();

        int Count { get; }

        TValue this[TKey key]
        {
            get; set;
        }
    }
}