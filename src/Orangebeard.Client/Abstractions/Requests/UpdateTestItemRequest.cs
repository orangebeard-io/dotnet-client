using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Requests
{
    /// <summary>
    /// Defines a request to finish specified test item.
    /// </summary>
    [DataContract]
    public class UpdateTestItemRequest
    {
        /// <summary>
        /// Update tags for test item.
        /// </summary>
        [Obsolete("Use Attributes instead of Tags.")]
        [DataMember(Name = "tags", EmitDefaultValue = true)]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Update attributes for test item.
        /// </summary>
        [DataMember(Name = "attributes", EmitDefaultValue = true)]
        public List<ItemAttribute> Attributes { get; set; }

        /// <summary>
        /// Description of test item.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = true)]
        private string _statusString;

        /// <summary>
        /// New status for test item.
        /// </summary>
        public Status? Status
        {
            get
            {
                return EnumConverter.ConvertTo<Status>(_statusString);
            }
            set
            {
                _statusString = EnumConverter.ConvertFrom(value);
            }
        }
    }
}
