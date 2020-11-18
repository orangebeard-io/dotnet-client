using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class MessageResponse
    {
        [DataMember(Name = "message")]
        public string Info { get; set; }
    }
}
