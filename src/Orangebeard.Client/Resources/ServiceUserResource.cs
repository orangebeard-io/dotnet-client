using Orangebeard.Client.Abstractions.Resources;
using Orangebeard.Client.Abstractions.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orangebeard.Client.Resources
{
    class ServiceUserResource : ServiceBaseResource, IUserResource
    {
        public ServiceUserResource(HttpClient httpClient, string project) : base(httpClient, project)
        {

        }

        public Task<UserResponse> GetAsync()
        {
            return GetAsJsonAsync<UserResponse>("user");
        }
    }
}
