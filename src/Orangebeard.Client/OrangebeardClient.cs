using Orangebeard.Client.Abstractions;
using Orangebeard.Client.Abstractions.Resources;
using Orangebeard.Client.OrangebeardProperties;
using Orangebeard.Client.Resources;
using System;
#if NET45
using System.Net;
#endif
using System.Net.Http;

namespace Orangebeard.Client
{
    /// <inheritdoc/>
    public partial class OrangebeardClient : IClientService, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor to initialize a new object of the client.
        /// </summary>
        /// <param name="config">The configuration of the client.</param>
        public OrangebeardClient(OrangebeardConfiguration config)
        {
            ProjectName = config.ProjectName;
            _httpClient = new HttpClientFactory(new Uri(config.Endpoint), config.AccessToken).Create();
            

#if NET45
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
#endif

            Launch = new ServiceLaunchResource(_httpClient, ProjectName);
            TestItem = new ServiceTestItemResource(_httpClient, ProjectName);
            LogItem = new ServiceLogItemResource(_httpClient, ProjectName);
            User = new ServiceUserResource(_httpClient, ProjectName);
            UserFilter = new ServiceUserFilterResource(_httpClient, ProjectName);
            Project = new ServiceProjectResource(_httpClient, ProjectName);
        }

       
        /// <summary>
        /// Get or set project name to interact with.
        /// </summary>
        public string ProjectName { get; }

        /// <inheritdoc/>
        public ILaunchResource Launch { get; }

        /// <inheritdoc/>
        public ITestItemResource TestItem { get; }

        /// <inheritdoc/>
        public ILogItemResource LogItem { get; }

        /// <inheritdoc/>
        public IUserResource User { get; }

        /// <inheritdoc/>
        public IUserFilterResource UserFilter { get; }

        /// <inheritdoc/>
        public IProjectResource Project { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
