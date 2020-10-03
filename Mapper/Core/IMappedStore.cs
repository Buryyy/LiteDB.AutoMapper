using System.Collections.Generic;
using LiteDB;

namespace Mapper.Core
{
    internal interface IMappedStore<TKey, TValue>
    {
        LiteCollection<BsonDocument> BsonCollection { get; }

        int Count { get;  }

      
        /// <summary>
        /// Indexer for storing and acquiring from the storage.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue this[TKey key]
        {
            get;
            set;
        }

        /// <summary>
        /// Retrieves the <see cref="TValue"/> from database. If the <see cref="TKey"/> doesn't exist then a <see cref="null"/> will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Get(TKey key);

        /// <summary>
        /// If the key exists already, the <see cref="TValue"/> value will be replaced instead.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void StoreOrUpdate(TKey key, TValue value);

        /// <summary>
        /// Returns true if the storage contains this key, with a valid <see cref="TValue"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(TKey key);

        /// <summary>
        /// Removes an record by <see cref="TKey"/>. If you need to remove everything try using the <see cref="Clear"/>.
        /// Please use <see cref="RemoveBatch(TKey[])"/> to remove multiple <see cref="TValue"/> by keys.
        /// </summary>
        /// <param name="key"></param>
        void Remove(TKey key);

        /// <summary>
        /// Batch removes an array of <see cref="TKey[]"/>, returns amount of removed key values.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        int RemoveBatch(TKey[] keys);

        /// <summary>
        /// Stores a dictionary of key-value pairs in bulk.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        void StoreBatch(IDictionary<TKey, TValue> keyValuePairs);

        /// <summary>
        /// Return's a mapped enumerable of <see cref="TValue" />
        /// </summary>
        IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Clears the collection, calls LiteDB's 'DropCollection'.
        /// </summary>
        void Clear();
    }
}