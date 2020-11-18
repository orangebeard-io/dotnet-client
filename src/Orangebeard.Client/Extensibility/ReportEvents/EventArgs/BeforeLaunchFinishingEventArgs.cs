using Orangebeard.Client.Abstractions;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class BeforeLaunchFinishingEventArgs : ReportEventBaseArgs
    {
        public BeforeLaunchFinishingEventArgs(IClientService clientService, IConfiguration configuration, FinishLaunchRequest finishLaunchRequest) : base(clientService, configuration)
        {
            FinishLaunchRequest = finishLaunchRequest;
        }

        public FinishLaunchRequest FinishLaunchRequest { get; }
    }
}
