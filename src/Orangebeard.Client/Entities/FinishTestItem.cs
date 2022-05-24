using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.Entities
{
    public class FinishTestItem
    {
        [JsonProperty("launchUuid")]
        public Guid TestRunUUID { get; private set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("status")]
        public Status Status { get; private set; }

        [JsonProperty("description")]
        public String Description { get; private set; }

        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; private set; }

        public FinishTestItem(Guid testRunUUID, Status status, String description, ISet<Attribute> attributes)
        {
            TestRunUUID = testRunUUID;
            Status = status;
            Description = description;
            EndTime = DateTime.Now;
            Attributes = attributes;
        }

        public FinishTestItem(Guid testRunUUID, Status status)
        {
            TestRunUUID = testRunUUID;
            Status = status;
            EndTime = DateTime.Now;
        }
    }
}

