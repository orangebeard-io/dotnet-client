using Orangebeard.Shared.Extensibility.ReportEvents;

namespace Orangebeard.Shared.Extensibility
{
    public interface IReportEventsObserver
    {
        void Initialize(IReportEventsSource reportEventsSource);
    }


}
