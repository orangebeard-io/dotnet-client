using Orangebeard.Client.Abstractions;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class BeforeLaunchStartingEventArgs : ReportEventBaseArgs
    {
        public BeforeLaunchStartingEventArgs(IClientService clientService, IConfiguration configuration, StartLaunchRequest startLaunchRequest) : base(clientService, configuration)
        {
            StartLaunchRequest = startLaunchRequest;
        }

        public StartLaunchRequest StartLaunchRequest { get; }
    }
}
