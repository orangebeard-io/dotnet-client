using Orangebeard.Client.Entities;
using Orangebeard.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Orangebeard.Client
{
    abstract public class AbstractClient : IOrangebeardClient
    {
        public static readonly string APPLICATION_JSON = "application/json"; //TODO?~ Get this from the MIME Type map.

        protected HttpClient httpClient = new HttpClient();

        public abstract void FinishTestItem(Guid itemId, FinishTestItem finishTestItem);
        public abstract void FinishTestRun(Guid testRunUUID, FinishTestRun finishTestRun);
        public abstract void Log(Log log);
        public abstract void Log(ISet<Log> logs);
        public abstract void SendAttachment(Attachment attachment);
        public abstract Guid? StartTestItem(Guid? suiteId, StartTestItem testItem);
        public abstract Guid? StartTestRun(StartTestRun testRun);
        public abstract void UpdateTestRun(Guid testRunUUID, UpdateTestRun updateTestRun);


        public void InitializeHttpClient(Uri baseUri, string token, string userAgentPostFix)
        {
            var httpClientHandler = new HttpClientHandler();

#if !NET45
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
#endif

            httpClient = new HttpClient(httpClientHandler);

            httpClient.BaseAddress = baseUri.Normalize();

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //TODO?- This probably isn't needed anymore.
            /*
            if (userAgentPostFix != null)
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter/" +
                    typeof(OrangebeardV2Client).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion +
                    " " + userAgentPostFix);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter" +
                    typeof(OrangebeardV2Client).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            }
            */
        }
    }
}
