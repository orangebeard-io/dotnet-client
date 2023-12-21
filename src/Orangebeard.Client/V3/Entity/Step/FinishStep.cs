using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.Step
{
    public class FinishStep
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("status")]
        public TestStatus Status { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
    }
}
