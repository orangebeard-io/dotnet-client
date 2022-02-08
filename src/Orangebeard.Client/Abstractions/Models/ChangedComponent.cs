using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Models
{
    [DataContract]
    public class ChangedComponent
    {
        [DataMember(Name = "ComponentName")]
        public string ComponentName { get; private set; }

        [DataMember(Name = "ComponentVersion")]
        public string ComponentVersion { get; private set; }

        public ChangedComponent(string name, string version)
        {
            ComponentName = name;
            ComponentVersion = version;
        }

        public override bool Equals(object obj)
        {
            return obj is ChangedComponent component &&
                   ComponentName == component.ComponentName &&
                   ComponentVersion == component.ComponentVersion;
        }

        public override int GetHashCode()
        {
            int hashCode = 831060359;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ComponentName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ComponentVersion);
            return hashCode;
        }
    }
}
