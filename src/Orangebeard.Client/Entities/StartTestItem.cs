using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.Entities
{
    public class StartTestItem
    {
        [JsonProperty("launchUuid")]
        public Guid TestRunUuid { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public TestItemType Type { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; private set; }

        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; private set; } = new HashSet<Attribute>();

        [JsonProperty("hasStats")]
        public bool CountAsTestItem { get; private set; } = true;

        public StartTestItem(Guid testRunUUID, String name, TestItemType type, String description, ISet<Attribute> attributes)
        {
            this.TestRunUuid = testRunUUID;
            this.Name = name;
            this.Type = type;
            this.StartTime = DateTime.Now;
            this.Description = description;
            this.Attributes = attributes ?? new HashSet<Attribute>();
        }
    }
}
