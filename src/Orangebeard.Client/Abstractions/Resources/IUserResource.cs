using Orangebeard.Client.Abstractions.Responses;
using System.Threading.Tasks;

namespace Orangebeard.Client.Abstractions.Resources
{
    /// <summary>
    /// Interacts with current user.
    /// </summary>
    public interface IUserResource
    {
        /// <summary>
        /// Gets the current user's information.
        /// </summary>
        /// <returns></returns>
        Task<UserResponse> GetAsync();
    }
}
