using Newtonsoft.Json;
using Orangebeard.Client.V3.ClientUtils;
using Orangebeard.Client.V3.Entity.Attachment;
using Orangebeard.Client.V3.Entity.Log;
using Orangebeard.Client.V3.Entity.Step;
using Orangebeard.Client.V3.Entity.Suite;
using Orangebeard.Client.V3.Entity.Test;
using Orangebeard.Client.V3.Entity.TestRun;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orangebeard.Client.V3
{
    internal class OrangebeardV3Client
    {
        private readonly HttpClient _restClient;
        private readonly string _endpoint;
        private readonly string _projectName;
        private bool _connectionWithOrangebeardIsValid;
        private readonly Guid _accessToken;
        private int _failCount;

        public OrangebeardV3Client(string endpoint, Guid accessToken, string projectName,
            bool connectionWithOrangebeardIsValid, string listenerPostfix = null)
        {
            _restClient = new HttpClient(new RetryHandler(new HttpClientHandler()))
            {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            var clientVersion = typeof(OrangebeardV3Client).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0];
            
            var clientIdentification = ".Net client/" + clientVersion;
            var userAgent = listenerPostfix != null
                ? clientIdentification + " " + listenerPostfix
                : clientIdentification;

            _restClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

            _accessToken = accessToken;
            _endpoint = endpoint;
            _projectName = projectName;
            _connectionWithOrangebeardIsValid = connectionWithOrangebeardIsValid;
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod method, string url, object content = null, MediaTypeHeaderValue contentType = null)
        {
            if (contentType == null)
            {
                contentType = new MediaTypeHeaderValue("application/json");
            }

            var request = new HttpRequestMessage(method, $"{_endpoint}/listener/v3/{_projectName}/{url}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            switch (content)
            {
                case null:
                    return request;
                case MultipartFormDataContent dataContent:
                    request.Content = dataContent;
                    break;
                default:
                    request.Content = new StringContent(JsonConvert.SerializeObject(content));
                    break;
            }

            request.Content.Headers.ContentType = contentType;

            return request;
        }

        private void HandleException(Exception e, string failedMethod, bool forceCancel = false)
        {
            _failCount++;
            _connectionWithOrangebeardIsValid = _failCount < 20 && !forceCancel;

            Console.WriteLine("Connection failed for {0}.{1}    {2}:{3}", failedMethod,
                Environment.NewLine, e.InnerException?.GetType().Name, e.InnerException?.Message);

            if (!_connectionWithOrangebeardIsValid)
            {
                Console.Error.WriteLine("Cancelled Orangebeard report!");
            }
        }

        public async Task<Guid?> StartTestRun(StartTestRun testRun)
        {
            if (!_connectionWithOrangebeardIsValid) return null;

            try
            {
                var request = CreateRequest(HttpMethod.Post, $"test-run/start", testRun);
                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Guid>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "StartTestRun", true);
            }

            return null;
        }

        public async Task StartAnnouncedTestRun(Guid testRunUuid)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/start/{testRunUuid}");
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "StartAnnounced", true);
                }
            }
        }

        public async Task FinishTestRun(Guid testRunUuid, FinishTestRun finishTestRun)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/finish/{testRunUuid}", finishTestRun);
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "FinishTestRun");
                }
            }
        }

        public async Task UpdateTestRun(Guid testRunUuid, UpdateTestRun updateTestRun)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/update/{testRunUuid}", updateTestRun);
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "UpdateTestRun");
                }
            }
        }

        public async Task<List<Suite>> StartSuite(StartSuite startSuite)
        {
            if (!_connectionWithOrangebeardIsValid) return new List<Suite>();

            try
            {
                var request = CreateRequest(HttpMethod.Post, $"suite/start", startSuite);
                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Suite>>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "StartSuite", true);
            }

            return new List<Suite>();
        }

        public async Task<Guid?> StartTest(StartTest startTest)
        {
            if (!_connectionWithOrangebeardIsValid) return null;

            try
            {
                var request = CreateRequest(HttpMethod.Post, $"test/start", startTest);
                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Guid>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "StartTest");
            }

            return null;
        }

        public async Task FinishTest(Guid testUuid, FinishTest finishTest)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test/finish/{testUuid}", finishTest);
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "FinishTest");
                }
            }
        }

        public async Task<Guid?> StartStep(StartStep startStep)
        {
            if (!_connectionWithOrangebeardIsValid) return null;

            try
            {
                var request = CreateRequest(HttpMethod.Post, $"step/start", startStep);
                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Guid>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "StartStep");
            }

            return null;
        }

        public async Task FinishStep(Guid stepUuid, FinishStep finishStep)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"step/finish/{stepUuid}", finishStep);
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "FinishStep");
                }
            }
        }

        public async Task<Guid?> Log(Log log)
        {
            if (!_connectionWithOrangebeardIsValid) return null;

            try
            {
                var request = CreateRequest(HttpMethod.Post, $"log", log);
                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Guid>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "Log");
            }

            return null;
        }

        public async Task SendLogBatch(List<Log> logs)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Post, $"log/batch", logs);
                    await _restClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    HandleException(e, "LogBatch");
                }
            }
        }

        public async Task<Guid?> SendAttachment(Attachment attachment)
        {
            if (!_connectionWithOrangebeardIsValid) return null;
            try
            {
                var mpBoundary = Guid.NewGuid().ToString();
                var attachmentMessage = new MultipartFormDataContent(mpBoundary);

                var attachmentContent =
                    new ByteArrayContent(attachment.File.Content, 0, attachment.File.Content.Length);
                attachmentContent.Headers.ContentType = new MediaTypeHeaderValue(attachment.File.ContentType);

                var multiPartContentType = new MediaTypeHeaderValue("multipart/form-data")
                {
                    Parameters = { new NameValueHeaderValue("boundary", mpBoundary) }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(attachment.MetaData), Encoding.UTF8,
                    "application/json");

                attachmentMessage.Add(attachmentContent, "attachment", attachment.File.Name);
                attachmentMessage.Add(jsonContent, "json");

                var request = CreateRequest(HttpMethod.Post, "attachment", attachmentMessage, multiPartContentType);

                var response = await _restClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Guid>(responseBody);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "SendAttachment");
            }

            return null;
        }
    }
}