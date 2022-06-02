using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Linq;

namespace Orangebeard.Client.Entities
{
    public class Attachment
    {
        [JsonProperty("launchUuid")]
        public Guid TestRunUUID { get; private set; }

        [JsonProperty("itemUuid")]
        public Guid ItemUuid { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("level")]
        public LogLevel LogLevel { get; private set; }

        [JsonProperty("message")]
        public String Message { get; private set; }

        [JsonProperty("file")]
        public AttachmentFile File;

        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        public /*static*/ class AttachmentFile
        {
            public AttachmentFile(FileInfo fileInfo)
            {
                Name = fileInfo.Name;
                Content = System.IO.File.ReadAllBytes(fileInfo.FullName);
                ContentType = Shared.MimeTypes.MimeTypeMap.GetMimeType(fileInfo.Extension);
            }

            public AttachmentFile(string fileName, string contentType, byte[] data)
            {
                Name = fileName;
                Content = data.ToArray();
                ContentType = contentType;
            }

            [JsonProperty("name")]
            public static string Name { get; private set; }

            [JsonIgnore]
            public static byte[] Content { get; private set; }

            [JsonIgnore]
            public static string ContentType { get; private set; }
        }

        public Attachment(Guid testRunUUID, Guid testItemUUID, LogLevel logLevel, String fileName, AttachmentFile file)
        {
            TestRunUUID = testRunUUID;
            ItemUuid = testItemUUID;
            LogLevel = logLevel;
            Message = fileName;
            Time = DateTime.Now;
            File = file;
        }
    }
}

