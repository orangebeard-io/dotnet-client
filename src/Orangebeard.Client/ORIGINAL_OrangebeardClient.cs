using Orangebeard.Client.OrangebeardProperties;
using System;
using System.Net;
using System.Net.Http;

namespace Orangebeard.Client
{
    /// <inheritdoc/>
    public partial class ORIGINAL_OrangebeardClient : /* IClientService,*/ IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor to initialize a new object of the client.
        /// </summary>
        /// <param name="config">The configuration of the client.</param>
        public ORIGINAL_OrangebeardClient(OrangebeardConfiguration config)
        {
            //ProjectName = config.ProjectName;
            _httpClient = new HttpClientFactory(new Uri(config.Endpoint), config.AccessToken, config.ListenerIdentification).Create();
            
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
