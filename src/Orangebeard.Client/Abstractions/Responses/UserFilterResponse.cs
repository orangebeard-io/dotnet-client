﻿using Orangebeard.Client.Abstractions.Filtering;
using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class UserFilterResponse
    {
        /// <summary>
        /// ID of user filter.
        /// </summary>
        [DataMember(Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Description of user filter.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// List of conditions to filter data.
        /// </summary>
        [DataMember(Name = "conditions")]
        public IEnumerable<Condition> Conditions { get; set; }

        /// <summary>
        /// Name of user filter.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// list of parameters of selection
        /// </summary>
        [DataMember(Name = "orders")]
        public IEnumerable<FilterOrder> Orders { get; set; }

        /// <summary>
        /// is filter shared
        /// </summary>
        [DataMember(Name = "share")]
        public bool IsShared { get; set; }

        /// <summary>
        /// filter type
        /// </summary>
        [DataMember(Name = "type")]
        private string TypeString { get; set; }

        /// <summary>
        /// user filter type enum
        /// </summary>
        public UserFilterType UserFilterType => EnumConverter.ConvertTo<UserFilterType>(TypeString);

        /// <summary>
        /// owner of user filter
        /// </summary>
        [DataMember(Name = "owner")]
        public string Owner { get; set; }
    }

    [DataContract]
    public class Condition
    {
        /// <summary>
        /// Condition to filter with.
        /// </summary>
        [DataMember(Name = "condition")]
        private string ConditionString { get; set; }

        public FilterOperation UserFilterCondition
        {
            get => EnumConverter.ConvertTo<FilterOperation>(ConditionString);
            set => ConditionString = EnumConverter.ConvertFrom(value);
        }

        /// <summary>
        /// Field to filter by.
        /// </summary>
        [DataMember(Name = "filteringField")]
        public string FilteringField { get; set; }

        /// <summary>
        /// value to filter by
        /// </summary>
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class FilterOrder
    {
        /// <summary>
        /// Is ascendant order.
        /// </summary>
        [DataMember(Name = "isAsc")]
        public bool Asc { get; set; }

        /// <summary>
        /// A column to sort by.
        /// </summary>
        [DataMember(Name = "sortingColumn")]
        public string SortingColumn { get; set; }
    }
}
