using Orangebeard.Client.Abstractions.Requests;

namespace Orangebeard.Shared.Reporter
{
    public interface ILogRequestAmender
    {
        void Amend(CreateLogItemRequest request);
    }
}
