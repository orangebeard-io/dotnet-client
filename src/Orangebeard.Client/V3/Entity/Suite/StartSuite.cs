using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.V3.Entity.Suite
{
    public class StartSuite
    {
        [JsonProperty("testRunUUID")] 
        public Guid TestRunUUID { get; set; }
        [JsonProperty("parentSuiteUUID")]
        public Guid? ParentSuiteUUID { get; set; }
        [JsonProperty("description")]
        public string Description  { get; set; }
        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; set; }
        [JsonProperty("suiteNames")]
        public IList<string> SuiteNames { get; set; }
    }
}
