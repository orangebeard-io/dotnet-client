using Orangebeard.Client.Abstractions.Requests;
using System.Threading.Tasks;

namespace Orangebeard.Shared.Reporter
{
    public interface ILogsReporter
    {
        Task ProcessingTask { get; }

        void Log(CreateLogItemRequest logRequest);
    }
}