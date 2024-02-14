using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.Step
{
    public class StartStep
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("testUUID")]
        public Guid TestUUID { get; set; }
        [JsonProperty("parentStepUUID")]
        public Guid? ParentStepUUID { get; set; }
        [JsonProperty("stepName")]
        public string StepName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

    }
}
