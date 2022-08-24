using Orangebeard.Client.Abstractions.Filtering;
using Orangebeard.Client.Abstractions.Requests;
using Orangebeard.Client.Abstractions.Resources;
using Orangebeard.Client.Abstractions.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orangebeard.Client.Resources
{
    class ServiceLaunchResource : ServiceBaseResource, ILaunchResource
    {
        public ServiceLaunchResource(HttpClient httpClient, string project) : base(httpClient, project)
        {

        }

        public Task<Content<LaunchResponse>> GetAsync(FilterOption filterOption = null)
        {
            var uri = $"{ProjectName}/launch";

            if (filterOption != null)
            {
                uri += $"?{filterOption}";
            }

            return GetAsJsonAsync<Content<LaunchResponse>>(uri);
        }

        public Task<Content<LaunchResponse>> GetDebugAsync(FilterOption filterOption = null)
        {
            var uri = $"{ProjectName}/launch/mode";

            if (filterOption != null)
            {
                uri += $"?{filterOption}";
            }

            return GetAsJsonAsync<Content<LaunchResponse>>(uri);
        }

        public Task<LaunchResponse> GetAsync(string uuid)
        {
            return GetAsJsonAsync<LaunchResponse>($"{ProjectName}/launch/uuid/{uuid}");
        }

        public Task<LaunchResponse> GetAsync(long id)
        {
            return GetAsJsonAsync<LaunchResponse>($"{ProjectName}/launch/{id}");
        }

        public Task<LaunchCreatedResponse> StartAsync(StartLaunchRequest request)
        {
            return PostAsJsonAsync<LaunchCreatedResponse, StartLaunchRequest>($"{ProjectName}/launch", request);
        }

        public Task<LaunchFinishedResponse> FinishAsync(string uuid, FinishLaunchRequest request)
        {
            return PutAsJsonAsync<LaunchFinishedResponse, FinishLaunchRequest>($"{ProjectName}/launch/{uuid}/finish", request);
        }

        public Task<LaunchFinishedResponse> StopAsync(long id, FinishLaunchRequest request)
        {
            return PutAsJsonAsync<LaunchFinishedResponse, FinishLaunchRequest>($"{ProjectName}/launch/{id}/stop", request);
        }

        public Task<MessageResponse> DeleteAsync(long id)
        {
            return DeleteAsJsonAsync<MessageResponse>($"{ProjectName}/launch/{id}");
        }
       
        public Task<MessageResponse> UpdateAsync(string uuid, UpdateLaunchRequest request)
        {
            return PutAsJsonAsync<MessageResponse, UpdateLaunchRequest>($"{ProjectName}/launch/{uuid}/update", request);
        }
    }
}
