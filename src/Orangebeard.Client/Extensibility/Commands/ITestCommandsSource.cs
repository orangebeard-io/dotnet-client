using Orangebeard.Shared.Execution;
using Orangebeard.Shared.Extensibility.Commands.CommandArgs;

namespace Orangebeard.Shared.Extensibility.Commands
{
    public interface ITestCommandsSource
    {
        event TestCommandHandler<TestAttributesCommandArgs> OnGetTestAttributes;

        event TestCommandHandler<TestAttributesCommandArgs> OnAddTestAttributes;

        event TestCommandHandler<TestAttributesCommandArgs> OnRemoveTestAttributes;
    }

    public delegate void TestCommandHandler<TCommandArgs>(ITestContext testContext, TCommandArgs args);
}
