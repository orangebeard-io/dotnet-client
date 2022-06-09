using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Orangebeard.Client.Entities
{
    public class FinishTestRun
    {
        [JsonProperty("endTime")]
        public DateTime EndTime { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("status")]
        public Status? Status { get; private set; }

        public FinishTestRun(Status status)
        {
            this.EndTime = DateTime.Now;
            this.Status = status;
        }

        public FinishTestRun()
        {
            this.EndTime = DateTime.Now;
        }
    }
}
