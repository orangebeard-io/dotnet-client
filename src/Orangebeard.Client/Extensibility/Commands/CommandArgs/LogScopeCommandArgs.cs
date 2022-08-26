using Orangebeard.Shared.Execution.Logging;

namespace Orangebeard.Shared.Extensibility.Commands.CommandArgs
{
    public class LogScopeCommandArgs
    {
        public LogScopeCommandArgs(ILogScope logScope)
        {
            LogScope = logScope;
        }

        public ILogScope LogScope { get; }
    }
}
