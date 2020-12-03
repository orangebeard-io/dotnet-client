using Orangebeard.Client.Abstractions.Resources;

namespace Orangebeard.Client.Abstractions
{
    /// <summary>
    /// Interface to interact with Oramgebeard. Provides possibility to manage almost of service's endpoints.
    /// </summary>
    public interface IClientService
    {
        ILaunchResource Launch { get; }

        ITestItemResource TestItem { get; }

        ILogItemResource LogItem { get; }

    }
}