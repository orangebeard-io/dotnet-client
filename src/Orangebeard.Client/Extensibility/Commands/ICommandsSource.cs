using Orangebeard.Shared.Execution;
using Orangebeard.Shared.Extensibility.Commands.CommandArgs;

namespace Orangebeard.Shared.Extensibility.Commands
{
    public interface ICommandsSource
    {
        event LogCommandHandler<LogScopeCommandArgs> OnBeginLogScopeCommand;

        event LogCommandHandler<LogScopeCommandArgs> OnEndLogScopeCommand;

        event LogCommandHandler<LogMessageCommandArgs> OnLogMessageCommand;

        ITestCommandsSource TestCommandsSource { get; }
    }

    public delegate void LogCommandHandler<TCommandArgs>(ILogContext logContext, TCommandArgs args);
}
