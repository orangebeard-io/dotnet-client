using Orangebeard.Client.Abstractions;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class AfterLaunchStartedEventArgs : ReportEventBaseArgs
    {
        public AfterLaunchStartedEventArgs(IClientService clientService, IConfiguration configuration) : base(clientService, configuration)
        {

        }
    }
}
