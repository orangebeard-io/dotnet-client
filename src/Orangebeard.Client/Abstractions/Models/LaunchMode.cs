﻿using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Models
{
    /// <summary>
    /// Describes modes for launches.
    /// </summary>
    public enum LaunchMode
    {
        [DataMember(Name = "DEFAULT")]
        Default,
        [DataMember(Name = "DEBUG")]
        Debug
    }
}
