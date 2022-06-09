using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orangebeard.Client.Entities
{
    public class Log
    {
        [JsonProperty("itemUuid")]
        public Guid ItemUuid { get; private set; }

        [JsonProperty("launchUuid")]
        public Guid TestRunUUID { get; private set; }

        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        [JsonProperty("message")]
        public String Message { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("level")]
        public LogLevel LogLevel { get; private set; }

        public Log(Guid testRunUUID, Guid testItemUUID, LogLevel logLevel, String message)
        {
            this.ItemUuid = testItemUUID;
            this.TestRunUUID = testRunUUID;
            this.LogLevel = logLevel;
            this.Time = DateTime.Now;
            this.Message = message;
        }
    }
}
