using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.V3.Entity.Suite
{
    public class Suite
    {
        [JsonProperty("suiteUUID")]
        public Guid SuiteUUID { get; set; }
        [JsonProperty("parentUUID")]
        public Guid? ParentUUID { get; set; }
        [JsonProperty("localSuiteName")]
        public string LocalSuiteName { get; set; }
        [JsonProperty("fullSuitePath")]
        public List<string> FullSuitePath { get; set; }
    }
}
