using Orangebeard.Client.Abstractions;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class AfterTestStartedEventArgs : ReportEventBaseArgs
    {
        public AfterTestStartedEventArgs(IClientService clientService, IConfiguration configuration) : base(clientService, configuration)
        {

        }
    }
}
