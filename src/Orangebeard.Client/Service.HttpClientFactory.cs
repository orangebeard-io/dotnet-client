using Orangebeard.Client.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Orangebeard.Client
{
    partial class OrangebeardClient
    {
        class HttpClientFactory : IHttpClientFactory
        {
            private Uri _baseUri;
            private string _token;
            private string _userAgentPostFix;

            [Obsolete("HttpClientFactory(Uri baseUri, string token) is deprecated, please use HttpClientFactory(Uri baseUri, string token, string userAgentPostfix) instead, so listeners can be identified.")]
            public HttpClientFactory(Uri baseUri, string token)
            {
                _baseUri = baseUri;
                _token = token;
            }

            public HttpClientFactory(Uri baseUri, string token, string userAgentPostfix)
            {
                _baseUri = baseUri;
                _token = token;
                _userAgentPostFix = userAgentPostfix;
            }

            public HttpClient Create()
            {
                var httpClientHandler = new HttpClientHandler();

                var httpClient = new HttpClient(httpClientHandler);

                httpClient.BaseAddress = _baseUri.Normalize();

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
                if (_userAgentPostFix != null)
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter/" +
                        typeof(OrangebeardClient).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion + 
                        " " + _userAgentPostFix);
                } else
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter" +
                        typeof(OrangebeardClient).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                }
                return httpClient;
            }
        }
    }
}
