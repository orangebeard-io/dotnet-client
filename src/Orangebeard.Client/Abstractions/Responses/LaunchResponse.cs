﻿using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class LaunchResponse
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "uuid")]
        public string Uuid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; }

        [DataMember(Name = "mode")]
        private string ModeString { get; set; }

        public LaunchMode Mode => EnumConverter.ConvertTo<LaunchMode>(ModeString);

        [DataMember(Name = "startTime")]
        private string StartTimeString { get; set; }

        public DateTime StartTime
        {
            get => DateTimeConverter.ConvertTo(StartTimeString);
        }

        [DataMember(Name = "endTime")]
        private string EndTimeString { get; set; }

        public DateTime? EndTime => EndTimeString == null ? (DateTime?)null : DateTimeConverter.ConvertTo(EndTimeString);

        [DataMember(Name = "hasRetries")]
        public bool HasRetries { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "attributes")]
        public IEnumerable<ItemAttribute> Attributes { get; set; }

        [DataMember(Name = "statistics")]
        public Statistic Statistics { get; set; }
    }

    [DataContract]
    public class Statistic
    {
        [DataMember(Name = "executions")]
        public Executions Executions { get; set; }

        [DataMember(Name = "defects")]
        public Defects Defects { get; set; }
    }

    [DataContract]
    public class Executions
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "passed")]
        public int Passed { get; set; }

        [DataMember(Name = "failed")]
        public int Failed { get; set; }

        [DataMember(Name = "skipped")]
        public int Skipped { get; set; }
    }

    [DataContract]
    public class Defects
    {
        [DataMember(Name = "product_bug")]
        public Defect ProductBugs { get; set; }

        [DataMember(Name = "automation_bug")]
        public Defect AutomationBugs { get; set; }

        [DataMember(Name = "system_issue")]
        public Defect SystemIssues { get; set; }

        [DataMember(Name = "to_investigate")]
        public Defect ToInvestigate { get; set; }

        [DataMember(Name = "no_defect")]
        public Defect NoDefect { get; set; }
    }

    [DataContract]
    public class Defect
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
