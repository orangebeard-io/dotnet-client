﻿using Orangebeard.Client.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Orangebeard.Client
{
    partial class OrangebeardClient
    {
        class HttpClientFactory : IHttpClientFactory
        {
            private Uri _baseUri;
            private string _token;

            public HttpClientFactory(Uri baseUri, string token)
            {
                _baseUri = baseUri;
                _token = token;
            }

            public HttpClient Create()
            {
                var httpClientHandler = new HttpClientHandler();

#if !NET45
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
#endif

                var httpClient = new HttpClient(httpClientHandler);

                httpClient.BaseAddress = _baseUri.Normalize();

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
                httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter");

                return httpClient;
            }
        }
    }
}
