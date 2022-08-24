using Orangebeard.Client.Abstractions;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class BeforeTestStartingEventArgs : ReportEventBaseArgs
    {
        public BeforeTestStartingEventArgs(IClientService clientService, IConfiguration configuration, StartTestItemRequest startTestItemRequest) : base(clientService, configuration)
        {
            StartTestItemRequest = startTestItemRequest;
        }

        public StartTestItemRequest StartTestItemRequest { get; }
    }
}
