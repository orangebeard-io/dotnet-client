using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client.Entities
{
    public class UpdateTestRun
    {
        [JsonProperty("description")]
        public String Description { get; private set; }

        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; private set; } = new HashSet<Attribute>();

        public UpdateTestRun(string description, ISet<Attribute> attributes = null)
        {
            Description = description;
            Attributes = attributes ?? new HashSet<Attribute>();
        }
    }
}
