using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orangebeard.Client.V3.Entity.TestRun
{
    public class UpdateTestRun
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; set; }
    }
}
