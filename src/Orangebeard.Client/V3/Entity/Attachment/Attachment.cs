using Newtonsoft.Json;
using System;

namespace Orangebeard.Client.V3.Entity.Attachment
{
    public class Attachment
    {
        [JsonProperty("attachmentFile")]
        public AttachmentFile File { get; set; }
        [JsonProperty("metaData")]
        public AttachmentMetaData MetaData { get; set; }
    }

    public class AttachmentFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("content")]
        public byte[] Content { get; set; }
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
    }

    public class AttachmentMetaData
    {
        [JsonProperty("testRunUUID")]
        public Guid TestRunUUID { get; set; }
        [JsonProperty("testUUID")]
        public Guid TestUUID { get; set; }
        [JsonProperty("stepUUID")]
        public Guid? StepUUID { get; set; }
        [JsonProperty("logUUID")]
        public Guid LogUUID { get; set; }
        [JsonProperty("attachmentTime")]
        public DateTime AttachmentTime { get; set; }
    }
}
