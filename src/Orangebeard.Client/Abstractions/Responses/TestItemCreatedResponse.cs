using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class TestItemCreatedResponse
    {
        [DataMember(Name = "id")]
        public string Uuid { get; set; }
    }
}
