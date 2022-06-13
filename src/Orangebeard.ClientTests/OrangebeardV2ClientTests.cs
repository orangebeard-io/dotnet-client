using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Entities;
using Orangebeard.Client.OrangebeardProperties;
using System;
using System.Collections.Generic;
using System.IO;
using Attribute = Orangebeard.Client.Entities.Attribute;

namespace Orangebeard.Client.Tests
{
    /// <summary>
    /// Automated round-trip tests to see if the client works well with the Orangebeard V2 API.
    /// Note that this is improper use of the unit-testing framework.
    /// </summary>
    [TestClass()]
    public class IntegrationTest
    {
        //private readonly string propertiesFile = @"orangebeard.properties";
        private readonly string imageFile = @"TestImageV2Client.png";

        [TestMethod()]
        public void RoundTripTest()
        {
            OrangebeardConfiguration config = new OrangebeardConfiguration(/*propertiesFile*/);
            var client = new OrangebeardV2Client(config.Endpoint, new Guid(config.AccessToken), config.ProjectName, "Testset Name", true);

            var testRunAttributes = new HashSet<Attribute>() { new Attribute("Test-Run-Key-1", "Test-Run-Value-1"), new Attribute("Test-Run-Value-Only-Attribute") };
            testRunAttributes.UnionWith(config.Attributes);
            var changedComponents = new HashSet<ChangedComponent>() { new ChangedComponent("Changed-Component-Name-1", "Changed-Component-Version-1") };
            var startTestRun = new StartTestRun("E2E test nieuwe .NET client", "End-to-end test van de nieuwe .NET Client", testRunAttributes, changedComponents);
            var finishTestRun = new FinishTestRun(Status.FAILED);

            var testRunId = client.StartTestRun(startTestRun);
            Assert.IsNotNull(testRunId, "Unable to start test run.");

            var testSuiteAttributes = new HashSet<Attribute>() { new Attribute("Test-Suite-Key-1", "Test-Suite-Value-1"), new Attribute("Test-Suite-Value-Only-Attribute") };
            var startSuite = new StartTestItem(testRunId.Value, "Suite name", TestItemType.SUITE, "Suite description", testSuiteAttributes);
            var suiteId = client.StartTestItem(null, startSuite);
            Assert.IsNotNull(suiteId, "Unable to start test suite.");

            var testItemAttributes = new HashSet<Attribute>() { new Attribute("Test-Item-Key-1", "Test-Item-Value-1"), new Attribute("Test-Item-Value-Only-Attribute") };
            var startTest = new StartTestItem(testRunId.Value, "Test name", TestItemType.TEST, "Test description", testItemAttributes);
            var testId = client.StartTestItem(suiteId, startTest);
            Assert.IsNotNull(testId, "Unable to start test.");

            //var testRunUpdateAttributes = new HashSet<Attribute>() { new Attribute("Test-Run-Updated-Key-1", "Test-Run-Updated-Value-1"), new Attribute("Test-Run-Updated-Value-Only-Attribute") };
            //var updateTestRun = new UpdateTestRun("Updated description", testRunUpdateAttributes);
            //client.UpdateTestRun(testRunId.Value, updateTestRun);

            var log1 = new Log(testRunId.Value, testId.Value, LogLevel.warn, "Logging a warning (1).", LogFormat.PLAIN_TEXT);
            var log2 = new Log(testRunId.Value, testId.Value, LogLevel.warn, "Logging a warning (2).", LogFormat.HTML);
            var logSet = new HashSet<Log>() { log1, log2 };
            client.Log(logSet);

            string addressOfPicture = imageFile;
            var pictureFileInfo = new FileInfo(addressOfPicture);
            Attachment.AttachmentFile file = new Attachment.AttachmentFile(pictureFileInfo);
            var attachment = new Attachment(testRunId.Value, testId.Value, LogLevel.error, "TestImageV2Client.png", file);
            client.SendAttachment(attachment);

            FinishTestItem finishTest = new FinishTestItem(testRunId.Value, Status.FAILED);
            client.FinishTestItem(testId.Value, finishTest);

            FinishTestItem finishSuite = new FinishTestItem(testRunId.Value, Status.FAILED);
            client.FinishTestItem(suiteId.Value, finishSuite);

            client.FinishTestRun(testRunId.Value, finishTestRun);
        }

        [TestMethod()]
        public void TestUpdateAttributes()
        {
            OrangebeardConfiguration config = new OrangebeardConfiguration(/*propertiesFile*/);
            var client = new OrangebeardV2Client(config.Endpoint, new Guid(config.AccessToken), config.ProjectName, "Testset name", true);

            var testRunAttributes = new HashSet<Attribute>() { new Attribute("Test-Run-Key-1", "Test-Run-Value-1"), new Attribute("Test-Run-Value-Only-Attribute") };
            testRunAttributes.UnionWith(config.Attributes);
            var changedComponents = new HashSet<ChangedComponent>() { new ChangedComponent("Changed-Component-Name-1", "Changed-Component-Version-1") };
            var startTestRun = new StartTestRun("E2E test nieuwe .NET client", "Test update testrun attributes", testRunAttributes, changedComponents);
            var finishTestRun = new FinishTestRun(Status.PASSED);

            var testRunId = client.StartTestRun(startTestRun);
            Assert.IsNotNull(testRunId, "Unable to start test run.");

            var testRunUpdateAttributes = new HashSet<Attribute>() { new Attribute("Test-Run-Updated-Key-1", "Test-Run-Updated-Value-1"), new Attribute("Test-Run-Updated-Value-Only-Attribute") };
            var updateTestRun = new UpdateTestRun("Updated description", testRunUpdateAttributes);
            client.UpdateTestRun(testRunId.Value, updateTestRun);

            client.FinishTestRun(testRunId.Value, finishTestRun);
        }
    }

}