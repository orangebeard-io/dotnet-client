using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orangebeard.Client.Entities;
using Orangebeard.Client.OrangebeardProperties;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Orangebeard.Client
{
    public class OrangebeardV2Client : AbstractClient
    {
        private readonly ILog LOGGER = LogManager.GetLogger("OrangebeardV2Client");

        private bool connectionWithOrangebeardIsValid = false;
        private readonly OrangebeardConfiguration config;

        private OrangebeardV2Client()
        {
        }

        private void Init(Uri endpoint, Guid uuid)
        {
            InitializeHttpClient(endpoint, uuid.ToString(), null);

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public OrangebeardV2Client(string endpoint, Guid uuid, string projectName, string testSetName, bool connectionWithOrangebeardIsValid) : this()
        {
            config = new OrangebeardConfiguration(endpoint, uuid, projectName, testSetName);
            Init(new Uri(endpoint), uuid);
            this.connectionWithOrangebeardIsValid = connectionWithOrangebeardIsValid;
        }

        public OrangebeardV2Client(Uri endpoint, Guid uuid, string projectName, string testSetName, bool connectionWithOrangebeardIsValid) : this()
        {
            config = new OrangebeardConfiguration(endpoint, uuid, projectName, testSetName);
            Init(endpoint, uuid);
            this.connectionWithOrangebeardIsValid = connectionWithOrangebeardIsValid;
        }

        public OrangebeardV2Client(OrangebeardConfiguration config, bool connectionWithOrangebeardIsValid) : this()
        {
            this.config = config;
            Init(config.Endpoint, new Guid(config.AccessToken));
            this.connectionWithOrangebeardIsValid = connectionWithOrangebeardIsValid;
        }

        #region Start, update, and finish test runs.

        public override Guid? StartTestRun(StartTestRun testRun)
        {
            if (connectionWithOrangebeardIsValid)
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                    //Uri uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/launch");
                    Uri uri = new Uri(config.Endpoint, $"listener/v2/{config.ProjectName}/launch");
                    string json = JsonConvert.SerializeObject(testRun);
                    StringContent content = new StringContent(json, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);

                    var result = httpClient.PostAsync(uri, content).Result;

                    string jsonResponseString = result.Content.ReadAsStringAsync().Result;
                    JObject jsonResponseObject = JObject.Parse(jsonResponseString);
                    string id = jsonResponseObject.Value<string>("id");
                    return new Guid(id);
                }
                catch (Exception)
                {
                    LOGGER.Error("The connection with Orangebeard could not be established! Check the properties and try again!");
                    connectionWithOrangebeardIsValid = false;
                }
            }
            return null;
        }

        public override void UpdateTestRun(Guid testRunUUID, UpdateTestRun updateTestRun)
        {
            if (connectionWithOrangebeardIsValid)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                //Uri uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/launch/{testRunUUID}/update");
                Uri uri = new Uri(config.Endpoint, $"listener/v2/{config.ProjectName}/launch/{testRunUUID}/update");
                String json = JsonConvert.SerializeObject(updateTestRun);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                _ = httpClient.PutAsync(uri, content).Result;
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
        }

        public override void FinishTestRun(Guid testRunUUID, FinishTestRun finishTestRun)
        {
            if (connectionWithOrangebeardIsValid)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                //Uri uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/launch/{testRunUUID}/finish");
                Uri uri = new Uri(config.Endpoint, $"/listener/v2/{config.ProjectName}/launch/{testRunUUID}/finish");
                string json = JsonConvert.SerializeObject(finishTestRun);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                _ = httpClient.PutAsync(uri, content).Result;
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
        }


        #endregion

        #region Start and Finish test items.

        public override Guid? StartTestItem(Guid? suiteId, StartTestItem testItem)
        {
            if (connectionWithOrangebeardIsValid)
            {
                Uri uri;
                if (suiteId == null)
                {
                    //uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/item");
                    uri = new Uri(config.Endpoint, $"listener/v2/{config.ProjectName}/item");
                }
                else
                {
                    //uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/item/{suiteId}");
                    uri = new Uri(config.Endpoint, $"listener/v2/{config.ProjectName}/item/{suiteId}");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                string json = JsonConvert.SerializeObject(testItem);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                var result = httpClient.PostAsync(uri, content).Result;
                string jsonResponseString = result.Content.ReadAsStringAsync().Result;
                JObject jsonResponseObject = JObject.Parse(jsonResponseString);
                string id = jsonResponseObject.Value<string>("id");
                return new Guid(id);
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
            return null;
        }

        public override void FinishTestItem(Guid itemId, FinishTestItem finishTestItem)
        {
            if (connectionWithOrangebeardIsValid)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                //Uri uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/item/{itemId}");
                Uri uri = new Uri(config.Endpoint, $"/listener/v2/{config.ProjectName}/item/{itemId}");
                string json = JsonConvert.SerializeObject(finishTestItem);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                var result = httpClient.PutAsync(uri, content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
        }

        #endregion

        #region Logs and attachments.

        public override void Log(Log log)
        {
            var logSet = new HashSet<Log> { log };
            Log(logSet);
        }

        public override void Log(ISet<Log> logs)
        {
            if (connectionWithOrangebeardIsValid)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                var content = new MultipartFormDataContent();

                var jsonLogArray = JsonConvert.SerializeObject(logs);
                var jsonLogArrayAsStringContent = new StringContent(jsonLogArray, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                content.Add(jsonLogArrayAsStringContent, "json_request_part");

                try
                {
                    //Uri uri = new Uri($"{config.Endpoint}/listener/v2/{config.ProjectName}/log");
                    Uri uri = new Uri(config.Endpoint, $"/listener/v2/{config.ProjectName}/log");
                    var result = httpClient.PostAsync(uri, content).Result;
                }
                catch (HttpRequestException exc)
                {
                    string logDetails = "";
                    var iterator = logs.GetEnumerator();
                    if (iterator.MoveNext())
                    {
                        Log anyLog = iterator.Current;
                        logDetails = $"One of the logs that cannot be reported Uuid=[{anyLog.ItemUuid}]; loglevel=[{anyLog.LogLevel}]; message=[{anyLog.Message}] ";
                    }
                    LOGGER.Error($"Logs cannot be reported to Orangebeard. {logDetails}{exc}");
                }
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
        }

        public override void SendAttachment(Attachment attachment)
        {
            if (connectionWithOrangebeardIsValid)
            {
                var content = new MultipartFormDataContent();

                // Create and add the JSON request part...
                Attachment[] attachmentArray = new Attachment[1];
                attachmentArray[0] = attachment;
                var attachmentJson = JsonConvert.SerializeObject(attachmentArray);

                var attachmentJsonStringContent = new StringContent(attachmentJson, System.Text.Encoding.UTF8, AbstractClient.APPLICATION_JSON);
                content.Add(attachmentJsonStringContent, "json_request_part");

                // Create the headers and content for the file part, and add them to "content".
                byte[] fileBytes = Attachment.AttachmentFile.Content;
                var fileContent = new ByteArrayContent(fileBytes);

                var mediaTypeString = Attachment.AttachmentFile.ContentType;
                var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(mediaTypeString);
                fileContent.Headers.ContentType = mediaTypeHeaderValue;

                var contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = Attachment.AttachmentFile.Name };
                fileContent.Headers.ContentDisposition = contentDispositionHeaderValue;

                content.Add(fileContent, "file");

                // Send the contents to the server.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken.ToString());
                //var result = httpClient.PostAsync($"{config.Endpoint}/listener/v2/{config.ProjectName}/log", content).Result;
                var result = httpClient.PostAsync(new Uri(config.Endpoint, $"listener/v2/{config.ProjectName}/log"), content).Result;

                _ = result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                LOGGER.Warn("The connection with Orangebeard could not be established!");
            }
        }

        #endregion

    }
}
