﻿using System;

namespace Orangebeard.Shared.Reporter
{
    public class LaunchInfo : IReporterInfo
    {
        public string Uuid { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? FinishTime { get; set; }
    }
}
