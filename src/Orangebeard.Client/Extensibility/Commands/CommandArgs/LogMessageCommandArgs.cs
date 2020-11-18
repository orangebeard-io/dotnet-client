using Orangebeard.Shared.Execution.Logging;

namespace Orangebeard.Shared.Extensibility.Commands.CommandArgs
{
    public class LogMessageCommandArgs
    {
        public LogMessageCommandArgs(ILogScope logScope, ILogMessage logMessage)
        {
            LogScope = logScope;
            LogMessage = logMessage;
        }

        public ILogScope LogScope { get; }

        public ILogMessage LogMessage { get; }
    }
}
