using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.V3.Entity.Test
{
    public class StartTest
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("suiteUUID")]
        public Guid SuiteUUID { get; set; }
        [JsonProperty("testName")]
        public string TestName { get; set; }
        [JsonProperty("testType")]
        public TestType TestType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

    }
}
