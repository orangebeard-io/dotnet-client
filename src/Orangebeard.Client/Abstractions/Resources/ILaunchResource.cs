using Orangebeard.Client.Abstractions.Filtering;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Client.Abstractions.Responses;
using System.Threading.Tasks;

namespace Orangebeard.Client.Abstractions.Resources
{
    /// <summary>
    /// Interacts with launches.
    /// </summary>
    public interface ILaunchResource
    {
        /// <summary>
        /// Finishes specified launch.
        /// </summary>
        /// <param name="uuid">UUID of specified launch.</param>
        /// <param name="request">Information about representation of launch to finish.</param>
        /// <returns>A message from service.</returns>
        Task<LaunchFinishedResponse> FinishAsync(string uuid, FinishLaunchRequest request);

        /// <summary>
        /// Deletes specified launch.
        /// </summary>
        /// <param name="id">ID of the launch to delete.</param>
        /// <returns>A message from service.</returns>
        Task<MessageResponse> DeleteAsync(long id);

        /// <summary>
        /// Returns specified launch by ID.
        /// </summary>
        /// <param name="id">ID of the launch to retrieve.</param>
        /// <returns>A representation of launch.</returns>
        Task<LaunchResponse> GetAsync(long id);

        /// <summary>
        /// Returns specified launch by UUID.
        /// </summary>
        /// <param name="uuid">UUID of the launch to retrieve.</param>
        /// <returns>A representation of launch.</returns>
        Task<LaunchResponse> GetAsync(string uuid);

        /// <summary>
        /// Returns a list of launches for current project.
        /// </summary>
        /// <param name="filterOption">Specified criterias for retrieving launches.</param>
        /// <returns>A list of launches.</returns>
        Task<Content<LaunchResponse>> GetAsync(FilterOption filterOption = null);

        /// <summary>
        /// Returns a list of debug launches for current project.
        /// </summary>
        /// <param name="filterOption">Specified criterias for retrieving launches.</param>
        /// <returns>A list of launches.</returns>
        Task<Content<LaunchResponse>> GetDebugAsync(FilterOption filterOption = null);

        /// <summary>
        /// Starts new launch.
        /// </summary>
        /// <param name="request">Information about launch.</param>
        /// <returns>Information about started launch.</returns>
        Task<LaunchCreatedResponse> StartAsync(StartLaunchRequest request);

        /// <summary>
        /// Stopes specified launch even if inner tests are not finished yet.
        /// </summary>
        /// <param name="id">ID of specified launch.</param>
        /// <param name="request">Information about representation of launch to finish.</param>
        /// <returns>A message from service.</returns>
        Task<LaunchFinishedResponse> StopAsync(long id, FinishLaunchRequest request);

        /// <summary>
        /// Update specified launch.
        /// </summary>
        /// <param name="uuid">UUID of launch to update.</param>
        /// <param name="request">Information about launch.</param>
        /// <returns>A message from service.</returns>
        Task<MessageResponse> UpdateAsync(string uuid, UpdateLaunchRequest request);
    }
}
