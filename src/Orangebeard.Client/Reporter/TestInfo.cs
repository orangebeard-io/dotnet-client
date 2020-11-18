using Orangebeard.Client.Abstractions.Models;
using System;

namespace Orangebeard.Shared.Reporter
{
    public class TestInfo : IReporterInfo
    {
        public string Uuid { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public Status Status { get; set; }
    }
}
