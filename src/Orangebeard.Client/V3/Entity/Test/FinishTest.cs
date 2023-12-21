using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.Test
{
    public class FinishTest
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("status")]
        public TestStatus Status { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

    }
}
