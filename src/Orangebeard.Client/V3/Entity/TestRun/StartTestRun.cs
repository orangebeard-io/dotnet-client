using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.V3.Entity.TestRun
{
    public class StartTestRun
    {
        [JsonProperty("testSetName")]
        public string TestSetName { get; set; }

        [JsonProperty("description")] 
        public string Description { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("attributes")] 
        public ISet<Attribute> Attributes { get; set; }
        [JsonProperty("sutComponents")] 
        public ISet<SUTComponent> SUTComponents { get; set; }

        public StartTestRun()
        {

        }

        public StartTestRun(string testSetName, string description, ISet<Attribute> attributes)
        {
            TestSetName = testSetName;
            Description = description;
            StartTime = DateTime.UtcNow;
            Attributes = attributes;
        }

        public StartTestRun(string testSetName, string description, ISet<Attribute> attributes, ISet<SUTComponent> sutComponents)
        {
            TestSetName = testSetName;
            Description = description;
            StartTime = DateTime.UtcNow;
            Attributes = attributes;
            SUTComponents = sutComponents;
        }
    }
}
