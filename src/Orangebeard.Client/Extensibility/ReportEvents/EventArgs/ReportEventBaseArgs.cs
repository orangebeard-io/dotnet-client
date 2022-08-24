using Orangebeard.Client.Abstractions;
using Orangebeard.Shared.Configuration;

namespace Orangebeard.Shared.Extensibility.ReportEvents.EventArgs
{
    public abstract class ReportEventBaseArgs
    {
        public ReportEventBaseArgs(IClientService clientService, IConfiguration configuration)
        {
            ClientService = clientService;
            Configuration = configuration;
        }

        public IClientService ClientService { get; }

        public IConfiguration Configuration { get; }
    }
}
