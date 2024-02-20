using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using Orangebeard.Client.V3.ClientUtils.Logging;

namespace Orangebeard.Client.V3.ClientUtils
{
    internal class RetryHandler : DelegatingHandler
    {
        private static readonly ILogger Logger = LogManager.Instance.GetLogger<RetryHandler>();
        private readonly AsyncRetryPolicy _retryPolicy;

        public RetryHandler(HttpMessageHandler innerHandler, int maxRetries = 4)
            : base(innerHandler)
        {
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<System.Net.Sockets.SocketException>()
                .Or<Exception>()
                .WaitAndRetryAsync(maxRetries, retryAttempt =>
                {
                    Logger.Warn($"[ORANGEBEARD] Retry attempt {retryAttempt}/{maxRetries}");
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                });
        }

        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            return await _retryPolicy.ExecuteAsync(
                async () =>
                {
                    var response = await base.SendAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    return response;
                });
        }
    }
}
