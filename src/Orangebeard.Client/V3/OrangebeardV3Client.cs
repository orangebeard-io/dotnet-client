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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        protected readonly Guid _accessToken;

        public OrangebeardV3Client(string endpoint, Guid accessToken, string projectName, bool connectionWithOrangebeardIsValid)
        {
            _restClient = new HttpClient(new RetryHandler(new HttpClientHandler()))
            {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            _accessToken = accessToken;
            _endpoint = endpoint;
            _projectName = projectName;
            _connectionWithOrangebeardIsValid = connectionWithOrangebeardIsValid;
        }

        protected HttpRequestMessage CreateRequest(
            HttpMethod method, string url, object content = null, MediaTypeHeaderValue contentType = null)
        {
            if (contentType == null)
            {
                contentType = new MediaTypeHeaderValue("application/json");
            }
            var request = new HttpRequestMessage(method, $"{_endpoint}/listener/v3/{_projectName}/{url}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (content != null)
            {
                if(content is MultipartFormDataContent)
                {
                    request.Content = content as MultipartFormDataContent;
                } else
                {
                    request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(content));
                }
                
                request.Content.Headers.ContentType = contentType;
            }

            return request;
        }

        private void HandleException(Exception e)
        {
            _connectionWithOrangebeardIsValid = false;
           Console.WriteLine("Coonection failed. Cancelling other orangebeard calls!" + Environment.NewLine + "    " +  e.InnerException.Message);
        }

        public async Task<Guid?> StartTestRun(StartTestRun testRun)
        {           
            if (_connectionWithOrangebeardIsValid)
            {
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
                    HandleException(e);
                    
                }
            }
            return null;
        }

        public async Task StartAnnouncedTestRun(Guid testRunUUID)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/start/{testRunUUID}");
                    var response = await _restClient.SendAsync(request);
                  
                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task FinishTestRun(Guid testRunUUID, FinishTestRun finishTestRun)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/finish/{testRunUUID}", finishTestRun);
                    var response = await _restClient.SendAsync(request);

                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task UpdateTestRun(Guid testRunUUID, UpdateTestRun updateTestRun)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test-run/update/{testRunUUID}", updateTestRun);
                    var response = await _restClient.SendAsync(request);

                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task<List<Suite>> StartSuite (StartSuite startSuite)
        {
            if (_connectionWithOrangebeardIsValid)
            {
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
                    HandleException(e);
                    
                }
            }
            return new List<Suite>();
        }

        public async Task<Guid?> StartTest(StartTest startTest)
        {
            if (_connectionWithOrangebeardIsValid)
            {
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
                    HandleException(e);
                    
                }
            }
            return null;
        }

        public async Task FinishTest(Guid testUUID, FinishTest finishTest)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"test/finish/{testUUID}", finishTest);
                    var response = await _restClient.SendAsync(request);

                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task<Guid?> StartStep(StartStep startStep)
        {
            if (_connectionWithOrangebeardIsValid)
            {
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
                    HandleException(e);
                    
                }
            }
            return null;
        }

        public async Task FinishStep(Guid stepUUID, FinishStep finishStep)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var request = CreateRequest(HttpMethod.Put, $"step/finish/{stepUUID}", finishStep);
                    var response = await _restClient.SendAsync(request);

                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task<Guid?> Log(Log log)
        {
            if (_connectionWithOrangebeardIsValid)
            {
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
                    HandleException(e);
                    
                }
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
                    var response = await _restClient.SendAsync(request);

                }
                catch (Exception e)
                {
                    HandleException(e);
                    
                }
            }
        }

        public async Task<Guid?> SendAttachment(Attachment attachment)
        {
            if (_connectionWithOrangebeardIsValid)
            {
                try
                {
                    var mpBoundary = Guid.NewGuid().ToString();
                    MultipartFormDataContent attachmentMessage = new MultipartFormDataContent(mpBoundary);

                    var attachmentContent = new ByteArrayContent(attachment.File.Content, 0, attachment.File.Content.Length);
                    attachmentContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(attachment.File.ContentType);

                    var multiPartContentType = new MediaTypeHeaderValue("multipart/form-data")
                    {
                        Parameters = { new NameValueHeaderValue("boundary", mpBoundary) }
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(attachment.MetaData), Encoding.UTF8, "application/json");

                    attachmentMessage.Add(attachmentContent, "attachment", attachment.File.Name);
                    attachmentMessage.Add(jsonContent, "json");

                    var request = CreateRequest(HttpMethod.Post, "attachment", attachmentMessage, multiPartContentType);

                    HttpResponseMessage response = await _restClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Guid>(responseBody);
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            }
             
            return null;
        }

    }
}
