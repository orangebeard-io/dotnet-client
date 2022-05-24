using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.Entities
{
    public class Attribute
    {
        [JsonProperty("key")]
        public string Key { get; private set; }
        [JsonProperty("value")]
        public string Value { get; private set; }

        /// <summary>
        /// Create a new attribute. Colons and semicolons are removed because these are special values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Attribute(string key, string value)
        {
            Key = key.Replace(":", "").Replace(";", "").Trim();
            Value = value.Replace(":", "").Replace(";", "").Trim();
        }

        /// <summary>
        /// Create a new attribute. Colons and semicolons are removed because these are special values.
        /// </summary>
        /// <param name="value"></param>
        public Attribute(String value)
        {
            Value = value.Replace(":", "").Replace(";", "").Trim();
        }

        public override string ToString()
        {
            return $"key: {Key}; value:{Value}";
        }
    }
}

