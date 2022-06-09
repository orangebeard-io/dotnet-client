using Orangebeard.Client.Entities;
using System;
using System.Collections.Generic;

namespace Orangebeard.Client
{
    interface IOrangebeardClient
    {
        Guid? StartTestRun(StartTestRun testRun);

        void UpdateTestRun(Guid testRunUUID, UpdateTestRun updateTestRun);

        Guid? StartTestItem(Guid? suiteId, StartTestItem testItem);

        void FinishTestItem(Guid itemId, FinishTestItem finishTestItem);

        void FinishTestRun(Guid testRunUUID, FinishTestRun finishTestRun);

        void Log(Log log);

        void Log(ISet<Log> logs);

        void SendAttachment(Attachment attachment);
    }
}
