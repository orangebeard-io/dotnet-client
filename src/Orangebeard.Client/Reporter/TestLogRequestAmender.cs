using Orangebeard.Client.Abstractions.Requests;

namespace Orangebeard.Shared.Reporter
{
    class TestLogRequestAmender : ILogRequestAmender
    {
        private ITestReporter _testReporter;

        public TestLogRequestAmender(ITestReporter testReporter)
        {
            _testReporter = testReporter;
        }

        public void Amend(CreateLogItemRequest request)
        {
            if (request.Time < _testReporter.Info.StartTime)
            {
                request.Time = _testReporter.Info.StartTime;
            }

            request.TestItemUuid = _testReporter.Info.Uuid;
            request.LaunchUuid = _testReporter.LaunchReporter.Info.Uuid;
        }
    }
}
