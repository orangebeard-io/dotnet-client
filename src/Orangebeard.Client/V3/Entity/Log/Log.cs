using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.Log
{
    public class Log
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("testUUID")]
        public Guid TestUUID { get; set; }
        [JsonProperty("stepUUID")]
        public Guid? StepUUID { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("logLevel")]
        public LogLevel LogLevel { get; set; }
        [JsonProperty("logTime")]
        public DateTime LogTime { get; set; }
        [JsonProperty("logFormat")]
        public LogFormat LogFormat { get; set; }
    }
}
