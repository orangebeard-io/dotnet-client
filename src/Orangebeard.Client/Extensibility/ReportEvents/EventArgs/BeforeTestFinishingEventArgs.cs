using Orangebeard.Client.Abstractions;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class BeforeTestFinishingEventArgs : ReportEventBaseArgs
    {
        public BeforeTestFinishingEventArgs(IClientService clientService, IConfiguration configuration, FinishTestItemRequest finishTestItemRequest) : base(clientService, configuration)
        {
            FinishTestItemRequest = finishTestItemRequest;
        }

        public FinishTestItemRequest FinishTestItemRequest { get; }
    }
}
