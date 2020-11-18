using Orangebeard.Client.Abstractions;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public class AfterTestFinishedEventArgs : ReportEventBaseArgs
    {
        public AfterTestFinishedEventArgs(IClientService clientService, IConfiguration configuration) : base(clientService, configuration)
        {

        }
    }
}
