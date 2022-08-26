using Orangebeard.Client.Abstractions.Requests;
using System.Collections.Generic;

namespace Orangebeard.Shared.Reporter
{
    public interface ILaunchReporter : IReporter
    {
        void Start(StartLaunchRequest startLaunchRequest);

        void Finish(FinishLaunchRequest finishLaunchRequest);

        ITestReporter StartChildTestReporter(StartTestItemRequest startTestItemRequest);

        IList<ITestReporter> ChildTestReporters { get; }

        void Log(CreateLogItemRequest createLogItemRequest);

        void Sync();
    }
}
