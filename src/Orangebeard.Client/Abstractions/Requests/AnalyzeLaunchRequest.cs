﻿using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Requests
{
    /// <summary>
    /// Defines a request to analyze launch.
    /// </summary>
    [DataContract]
    public class AnalyzeLaunchRequest
    {
        [DataMember(Name = "launchId")]
        public long LaunchId { get; set; }

        [DataMember(Name = "analyzerMode")]
        public string AnalyzerModeString { get; set; }

        public AnalyzerMode AnalyzerMode { get { return EnumConverter.ConvertTo<AnalyzerMode>(AnalyzerModeString); } set { AnalyzerModeString = EnumConverter.ConvertFrom(value); } }

        [DataMember(Name = "analyzeItemsMode")]
        public List<string> AnalyzerItemsModeString { get; set; }

        [DataMember(Name = "analyzerTypeName")]
        public string AnalyzerTypeName { get; set; }

        public List<AnalyzerItemsMode> AnalyzerItemsMode { get { return AnalyzerItemsModeString.Select(i => EnumConverter.ConvertTo<AnalyzerItemsMode>(i)).ToList(); } set { AnalyzerItemsModeString = value.Select(i => EnumConverter.ConvertFrom(i)).ToList(); } }
    }
}
