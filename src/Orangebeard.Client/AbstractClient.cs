using Orangebeard.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
    }
}
