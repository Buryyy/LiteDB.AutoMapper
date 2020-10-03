using LiteDB;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Mapper.Core
{
    public class LiteDBMapper<TKey, TValue> : IMappedStore<TKey, TValue>
    {
        /// <summary>
        /// In .NET Core the hashcodes of objects are not immutable/consistent,
        /// we must have strategy for handling of the keys for the .NET Core.
        /// </summary>
        private const string DefaultStorageName = @"Storage.db";

        private readonly LiteDatabase _database;
        private readonly string _collectionName;

        /// <summary>
        /// Collection of the <typeparamref name="TKey"/> and <see cref="TValue"/> pairs in a BsonDocument format.
        /// </summary>
        public LiteCollection<BsonDocument> BsonCollection { get; }

        public int Count { get; private set; }

        /// <summary>
        /// Creates a new LiteDBMapper instance, if <see cref="connectionString"/> is left to empty,
        /// the database will be created locally to "Storage.db"
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="connectionString"></param>
        public LiteDBMapper(string collection, string connectionString = null)
        {
            _database = new LiteDatabase(string.IsNullOrEmpty(connectionString) ? DefaultStorageName : connectionString);
            _collectionName = collection;
            BsonCollection = (LiteCollection<BsonDocument>)_database.GetCollection(_collectionName);
            Count = _database.GetCollection(_collectionName).Count();
        }

        /// <summary>
        /// Indexer for storing and acquiring from the storage.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => Get(key);
            set => StoreOrUpdate(key, value);
        }

        /// <summary>
        /// Retrieves the <see cref="TValue"/> from database. If the <see cref="TKey"/> doesn't exist then a <see cref="null"/> will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            var mapper = BsonMapper.Global;
            var result = BsonCollection.FindOne(
                (x) => x["Identifier"].AsString.Equals(SerializeKey(key)));
            return result != null ? mapper.ToObject<MappedKVPObject<TKey, TValue>>(result).Value : default;
        }

        /// <summary>
        /// If the key exists already, the <see cref="TValue"/> value will be replaced instead.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void StoreOrUpdate(TKey key, TValue value)
        {
            var mappedDocument = ToMappedBson(key, value);

            BsonCollection.Upsert(mappedDocument["Identifier"], mappedDocument);
            Count++;
        }

        /// <summary>
        /// Returns true if the storage contains this key, with a valid <see cref="TValue"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(TKey key)
        {
            return Get(key) != null;
        }

        /// <summary>
        /// Removes an record by <see cref="TKey"/>. If you need to remove everything try using the <see cref="Clear"/>.
        /// Please use <see cref="RemoveBatch(TKey[])"/> to remove multiple <see cref="TValue"/> by keys.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            BsonCollection.Delete(new BsonValue(
                Constants.IsNetCore ? JsonConvert.SerializeObject(key) : key.GetHashCode().ToString()));
            Count--;
        }

        /// <summary>
        /// Batch removes an array of <see cref="TKey[]"/>, returns amount of removed key values.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public int RemoveBatch(TKey[] keys)
        {
            var removedDocuments = BsonCollection.DeleteMany(
                Query.In("Identifier", keys.Select(id => new BsonValue(SerializeKey(id))).ToArray()));
            Count -= removedDocuments;
            return removedDocuments;
        }

        /// <summary>
        /// Stores a dictionary of key-value pairs in bulk.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public void StoreBatch(IDictionary<TKey, TValue> keyValuePairs)
        {
            foreach (KeyValuePair<TKey, TValue> entry in keyValuePairs)
            {
                StoreOrUpdate(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Return's a mapped enumerable of <see cref="TValue" />
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get
            {
                return BsonCollection.FindAll().Select(document =>
                    BsonMapper.Global.ToObject<MappedKVPObject<TKey, TValue>>(document).Value);
            }
        }

        /// <summary>
        /// Clears the collection, calls LiteDB's 'DropCollection'.
        /// </summary>
        public void Clear()
        {
            _database.DropCollection(_collectionName);
            Count = 0;
        }

        internal BsonDocument ToMappedBson(TKey key, TValue value)
        {
            return BsonMapper.Global.ToDocument(new MappedKVPObject<TKey, TValue>(key, value));
        }

        private string SerializeKey(TKey key)
        {
            return Constants.IsNetCore ? JsonConvert.SerializeObject(key) : key.GetHashCode().ToString();
        }
    }
}