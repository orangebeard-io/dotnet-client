using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Models
{
    public enum AnalyzerMode
    {
        [DataMember(Name = "ALL")]
        All,
        [DataMember(Name = "CURRENT_LAUNCH")]
        CurrentLaunch,
        [DataMember(Name = "LAUNCH_NAME")]
        LaunchName
    }
}
