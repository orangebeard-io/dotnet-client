﻿using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Requests
{
    /// <summary>
    /// Defines a request for logging messages into Report Portal.
    /// </summary>
    [DataContract]
    public class CreateLogItemRequest
    {
        /// <summary>
        /// ID of test item to add new logs.
        /// </summary>
        [DataMember(Name = "itemUuid")]
        public string TestItemUuid { get; set; }

        /// <summary>
        /// Log item belongs to launch instead of test item.
        /// </summary>
        [DataMember(Name = "launchUuid")]
        public string LaunchUuid { get; set; }

        /// <summary>
        /// Date time of log item.
        /// </summary>
        [DataMember(Name = "time")]
        public string TimeString { get; set; } = DateTimeConverter.ConvertFrom(DateTime.UtcNow);

        public DateTime Time
        {
            get
            {
                return DateTimeConverter.ConvertTo(TimeString);
            }
            set
            {
                TimeString = DateTimeConverter.ConvertFrom(value);
            }
        }

        /// <summary>
        /// A level of log item.
        /// </summary>
        [DataMember(Name = "level")]
        public string LevelString { get { return EnumConverter.ConvertFrom(Level); } set { Level = EnumConverter.ConvertTo<LogLevel>(value); } }

        public LogLevel Level { get; set; } = LogLevel.Info;

        /// <summary>
        /// Message of log item.
        /// </summary>
        [DataMember(Name = "message")]
        public string Text { get; set; }

        /// <summary>
        /// Specify an attachment of log item.
        /// </summary>
        [DataMember(Name = "file", EmitDefaultValue = false)]
        public LogItemAttach Attach { get; set; }
    }

    [DataContract]
    public class LogItemAttach
    {
        // empty ctor for json serialization
        public LogItemAttach()
        {

        }

        public LogItemAttach(string mimeType, byte[] data)
        {
            MimeType = mimeType;
            Data = data;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        [IgnoreDataMember]
        public byte[] Data { get; set; }

        [IgnoreDataMember]
        public string MimeType { get; set; }
    }
}
