using System.Collections.Generic;

namespace Orangebeard.Shared.Extensibility
{
    public interface IExtensionManager
    {
        void Explore(string path);

        IList<ILogFormatter> LogFormatters { get; }

        IList<IReportEventsObserver> ReportEventObservers { get; }

        IList<ICommandsListener> CommandsListeners { get; }
    }
}
