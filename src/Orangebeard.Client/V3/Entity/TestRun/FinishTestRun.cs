using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.TestRun
{
    public class FinishTestRun
    {
        [JsonProperty("endTime")]
        private DateTime EndTime { get; }

        public FinishTestRun()
        {
            EndTime = DateTime.UtcNow;
        }
    }
}
