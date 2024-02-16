using Polly;
using Polly.Retry;
using System;
using System.Net.Http;

namespace Orangebeard.Client.V3.ClientUtils
{
    internal class RetryHandler : DelegatingHandler
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public RetryHandler(HttpMessageHandler innerHandler, int maxRetries = 3)
            : base(innerHandler)
        {
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<System.Net.Sockets.SocketException>()
                .OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(maxRetries, retryAttempt =>
                {
                    Console.WriteLine("Retry attempt {0}/3", retryAttempt);
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                });
        }

        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            return await _retryPolicy.ExecuteAsync(
                async () => await base.SendAsync(request, cancellationToken)
            );
        }
    }
}
