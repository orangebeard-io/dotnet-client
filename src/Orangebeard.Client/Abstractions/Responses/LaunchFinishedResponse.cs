using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class LaunchFinishedResponse
    {
        [DataMember(Name = "id")]
        public string Uuid { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}
