using Newtonsoft.Json;

namespace Orangebeard.Client.V3.Entity
{
    public class Attribute
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")] 
        public string Value { get; set; }   

        public Attribute() { }

        public Attribute(string key, string value)
        {
            Key = key.Trim();
            Value = value.Trim(); ;
        }   

        public Attribute(string value)
        {
            Value = value.Trim();
        }

        public override string ToString()
        {
            return string.Format("key: {0}; value: {1}", Key, Value);
        }
    }
}
