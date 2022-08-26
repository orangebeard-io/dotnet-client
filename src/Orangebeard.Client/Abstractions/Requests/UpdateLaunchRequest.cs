using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Requests
{
    /// <summary>
    /// Defines a request to finish specified launch.
    /// </summary>
    [DataContract]
    public class UpdateLaunchRequest
    {
        /// <summary>
        /// Update attributes for launch.
        /// </summary>
        [DataMember(Name = "attributes", EmitDefaultValue = true)]
        public List<ItemAttribute> Attributes { get; set; }

        /// <summary>
        /// Description of launch.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "mode", EmitDefaultValue = true)]
        private string _modeString;

        /// <summary>
        /// Specify whether the launch is executed under debugging.
        /// </summary>
        public LaunchMode? Mode
        {
            get
            {
                return EnumConverter.ConvertTo<LaunchMode>(_modeString);
            }
            set
            {
                _modeString = EnumConverter.ConvertFrom(value);
            }
        }
    }
}
