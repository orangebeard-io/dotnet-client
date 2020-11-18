using Orangebeard.Client.Abstractions;
using Orangebeard.Shared.Configuration;
using Orangebeard.Shared.Reporter;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class AfterLaunchFinishedEventArgs : ReportEventBaseArgs
    {
        public AfterLaunchFinishedEventArgs(IClientService clientService, IConfiguration configuration) : base(clientService, configuration)
        {

        }
    }
}
