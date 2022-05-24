using Newtonsoft.Json;
using Orangebeard.Client.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orangebeard.Client.Entities
{
    public class StartTestRun
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; private set; }

        [JsonProperty("attributes")]
        public ISet<Attribute> Attributes { get; private set; } = new HashSet<Attribute>();

        [JsonProperty("changedComponents")]
        public ISet<ChangedComponent> ChangedComponents { get; private set; } = new HashSet<ChangedComponent>();

        public StartTestRun(string name, string description, ISet<Attribute> attributes = null, ISet<ChangedComponent> changedComponents = null)
        {
            this.Name = name;
            this.Description = description;
            this.StartTime = DateTime.Now;
            this.Attributes = attributes ?? new HashSet<Attribute>();
            this.ChangedComponents = changedComponents ?? new HashSet<ChangedComponent>();
        }
    }
}
