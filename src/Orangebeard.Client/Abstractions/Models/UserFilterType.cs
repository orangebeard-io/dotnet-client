using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Models
{
    public enum UserFilterType
    {
        [DataMember(Name = "launch")]
        Launch,
        [DataMember(Name = "testitem")]
        TestItem,
        [DataMember(Name = "log")]
        Log
    }
}
