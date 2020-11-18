using Orangebeard.Shared.Extensibility.Commands;

namespace Orangebeard.Shared.Extensibility
{
    public interface ICommandsListener
    {
        void Initialize(ICommandsSource commandsSource);
    }
}
