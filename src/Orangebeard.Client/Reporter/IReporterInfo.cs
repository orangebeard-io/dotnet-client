using System;

namespace Orangebeard.Shared.Reporter
{
    public interface IReporterInfo
    {
        string Uuid { get; }

        string Name { get; }

        DateTime StartTime { get; }

        DateTime? FinishTime { get; }
    }
}
