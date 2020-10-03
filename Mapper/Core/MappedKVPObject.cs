using Newtonsoft.Json;

namespace Mapper.Core
{
    internal sealed class MappedKVPObject<TKey, TValue>
    {
        public string Identifier { get; set; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        /// <summary>
        /// Key gets instantly serialized, if platform is other than .NET Core then we will use hashcodes.
        /// The reason for this approach is that hashcodes are not consistent in .NET core.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MappedKVPObject(TKey key, TValue value)
        {
            //Map key's hashcode or serializing to JSON to make it unique
            Identifier = Constants.IsNetCore ? JsonConvert.SerializeObject(key) : key.GetHashCode().ToString();
            Key = Key;
            Value = value;
        }

        public MappedKVPObject()
        {
        }
    }
}