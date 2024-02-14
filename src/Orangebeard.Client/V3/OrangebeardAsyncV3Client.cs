﻿using Orangebeard.Client.V3.Entity.Attachment;
using Orangebeard.Client.V3.Entity.Log;
using Orangebeard.Client.V3.Entity.Step;
using Orangebeard.Client.V3.Entity.Suite;
using Orangebeard.Client.V3.Entity.Test;
using Orangebeard.Client.V3.Entity.TestRun;
using Orangebeard.Client.V3.OrangebeardConfig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orangebeard.Client.V3
{
    public class OrangebeardAsyncV3Client
    {
        private readonly ConcurrentDictionary<Guid?, TaskCompletionSource<object>> _tasks;
        private readonly ConcurrentDictionary<Guid, Guid> _guidMap;
        private readonly OrangebeardV3Client _client;

        private TestRunContext _context;

        public OrangebeardAsyncV3Client(OrangebeardConfiguration config)
        {
            _client = new OrangebeardV3Client(config.Endpoint, Guid.Parse(config.AccessToken), config.ProjectName,
                true);
            _tasks = new ConcurrentDictionary<Guid?, TaskCompletionSource<object>>();
            _guidMap = new ConcurrentDictionary<Guid, Guid>();
        }

        public OrangebeardAsyncV3Client(string endpoint, Guid accessToken, string projectName,
            bool connectionWithOrangebeardIsValid)
        {
            _client = new OrangebeardV3Client(endpoint, accessToken, projectName, connectionWithOrangebeardIsValid);
            _tasks = new ConcurrentDictionary<Guid?, TaskCompletionSource<object>>();
            _guidMap = new ConcurrentDictionary<Guid, Guid>();
        }

        private TaskCompletionSource<object> GetParentTask(Guid? taskGuid)
        {
            return _tasks[taskGuid];
        }

        public TestRunContext TestRunContext()
        {
            return _context;
        }

        public Guid StartTestRun(StartTestRun testRun)
        {
            var temporaryUUID = Guid.NewGuid();
            _context = new TestRunContext(temporaryUUID);
            var startTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(temporaryUUID, startTestRunTask);

            Task.Run(async () =>
            {
                var actualUUID = await _client.StartTestRun(testRun);
                if (actualUUID != null)
                {
                    _guidMap[temporaryUUID] = (Guid)actualUUID;
                    startTestRunTask.SetResult(actualUUID);
                }
                else
                {
                    startTestRunTask.SetCanceled();
                }
            });

            return temporaryUUID;
        }

        public void StartAnnouncedTestRun(Guid testRunUUID)
        {
            _context = new TestRunContext(testRunUUID);
            var startTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(testRunUUID, startTestRunTask);

            Task.Run(async () =>
            {
                await _client.StartAnnouncedTestRun(testRunUUID);
                _guidMap[testRunUUID] = testRunUUID;
                startTestRunTask.SetResult(true);
            });
        }

        public void UpdateTestRun(Guid testRunUUID, UpdateTestRun updateTestRun)
        {
            var updateTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(Guid.NewGuid(), updateTestRunTask);
            var parentTask = GetParentTask(testRunUUID);
            parentTask.Task.ContinueWith(parent =>
            {
                testRunUUID = (Guid)parent.Result;
                Task.Run(async () =>
                {
                    await _client.UpdateTestRun(testRunUUID, updateTestRun);
                    updateTestRunTask.SetResult(true);
                });
            });
        }

        public void FinishTestRun(Guid testRunUUID, FinishTestRun finishTestRun)
        {
            var pendingTasks = _tasks.Values.Count(t => !t.Task.IsCompleted);
            Console.WriteLine("Waiting for {0}/{1} Orangebeard calls to finish...", pendingTasks, _tasks.Count());
            Task.WhenAll(_tasks.Values.Select(task => task.Task))
                .ContinueWith(parent =>
                {
                    Console.WriteLine("Finishing run!");
                    Task.Run(() => _client.FinishTestRun(_guidMap[testRunUUID], finishTestRun)).Wait();
                })
                .Wait();
        }

        public List<Guid> StartSuite(StartSuite startSuite)
        {
            var tempUUIDs = startSuite.SuiteNames.Select(_ => Guid.NewGuid()).ToList();
            _context.StartSuites(startSuite.SuiteNames, tempUUIDs);

            var startSuiteTask = new TaskCompletionSource<object>();
            tempUUIDs.ForEach(tempUUID => _tasks[tempUUID] = startSuiteTask);

            var parentTask = startSuite.ParentSuiteUUID != null
                ? GetParentTask(startSuite.ParentSuiteUUID)
                : GetParentTask(startSuite.TestRunUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testRunUUID;
                Guid? parentSuiteUUID = null;
                if (startSuite.ParentSuiteUUID == null)
                {
                    testRunUUID = (Guid)parent.Result;
                }
                else
                {
                    testRunUUID = _guidMap[startSuite.TestRunUUID];
                    parentSuiteUUID = parent.Result is List<Guid> list ? list.Last() : (Guid)parent.Result;
                }

                var realStartSuite = new StartSuite()
                {
                    TestRunUUID = testRunUUID,
                    ParentSuiteUUID = parentSuiteUUID,
                    Description = startSuite.Description,
                    Attributes = startSuite.Attributes,
                    SuiteNames = startSuite.SuiteNames
                };

                Task.Run(async () =>
                {
                    var suites = await _client.StartSuite(realStartSuite);
                    var actualUUIDs = suites.Select(suite => suite.SuiteUUID).ToList();
                    startSuiteTask.SetResult(actualUUIDs);
                });
            });

            return tempUUIDs;
        }

        public Guid StartTest(StartTest startTest)
        {
            var temporaryUUID = Guid.NewGuid();
            _context.StartTest(temporaryUUID);

            var startTestTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = startTestTask;

            var parentTask = GetParentTask(startTest.SuiteUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                var parentSuiteUUID = ((List<Guid>)parent.Result).Last();
                var realStartTest = new StartTest()
                {
                    TestRunUUID = _guidMap[startTest.TestRunUUID],
                    SuiteUUID = parentSuiteUUID,
                    TestName = startTest.TestName,
                    TestType = startTest.TestType,
                    Description = startTest.Description,
                    Attributes = startTest.Attributes,
                    StartTime = startTest.StartTime
                };

                Task.Run(async () =>
                {
                    var actualUUID = await _client.StartTest(realStartTest);
                    _guidMap[temporaryUUID] = (Guid)actualUUID;
                    startTestTask.SetResult(actualUUID);
                });
            });

            return temporaryUUID;
        }

        public void FinishTest(Guid testUUID, FinishTest finishTest)
        {
            _context.FinishTest(testUUID);
            var finishTestTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishTestTask;

            var parentTask = GetParentTask(testUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                var realFinishTest = new FinishTest()
                {
                    TestRunUUID = _guidMap[finishTest.TestRunUUID],
                    Status = finishTest.Status,
                    EndTime = finishTest.EndTime
                };

                Task.Run(async () =>
                {
                    await _client.FinishTest((Guid)parent.Result, realFinishTest);
                    finishTestTask.SetResult(null);
                });
            });
        }

        public Guid StartStep(StartStep startStep)
        {
            var temporaryUUID = Guid.NewGuid();
            _context.StartStep(temporaryUUID);

            var startStepTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = startStepTask;

            var parentTask = startStep.ParentStepUUID != null
                ? GetParentTask(startStep.ParentStepUUID)
                : GetParentTask(startStep.TestUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testUUID;
                Guid? parentStepUUID = null;
                if (startStep.ParentStepUUID == null)
                {
                    testUUID = (Guid)parent.Result;
                }
                else
                {
                    testUUID = _guidMap[startStep.TestUUID];
                    parentStepUUID = (Guid)parent.Result;
                }

                var realStartStep = new StartStep()
                {
                    TestRunUUID = _guidMap[startStep.TestRunUUID],
                    TestUUID = testUUID,
                    ParentStepUUID = parentStepUUID,
                    StepName = startStep.StepName,
                    Description = startStep.Description,
                    StartTime = startStep.StartTime
                };

                Task.Run(async () =>
                {
                    var actualUUID = await _client.StartStep(realStartStep);
                    _guidMap[temporaryUUID] = (Guid)actualUUID;
                    startStepTask.SetResult(actualUUID);
                });
            });

            return temporaryUUID;
        }

        public void FinishStep(Guid stepUUID, FinishStep finishStep)
        {
            _context.FinishStep(stepUUID);

            var finishStepTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishStepTask;

            var parentTask = GetParentTask(stepUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                var realFinishStep = new FinishStep()
                {
                    TestRunUUID = _guidMap[finishStep.TestRunUUID],
                    Status = finishStep.Status,
                    EndTime = finishStep.EndTime
                };

                Task.Run(async () =>
                {
                    await _client.FinishStep((Guid)parent.Result, realFinishStep);
                    finishStepTask.SetResult(null);
                });
            });
        }

        public Guid Log(Log log)
        {
            var temporaryUUID = Guid.NewGuid();
            var logTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = logTask;

            var parentTask = log.StepUUID != null ? GetParentTask(log.StepUUID) : GetParentTask(log.TestUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testUUID;
                Guid? stepUUID = null;
                if (log.StepUUID == null)
                {
                    testUUID = (Guid)parent.Result;
                }
                else
                {
                    testUUID = _guidMap[log.TestUUID];
                    stepUUID = (Guid)parent.Result;
                }

                var realLog = new Log()
                {
                    TestRunUUID = _guidMap[log.TestRunUUID],
                    TestUUID = testUUID,
                    StepUUID = stepUUID,
                    Message = log.Message,
                    LogLevel = log.LogLevel,
                    LogTime = log.LogTime,
                    LogFormat = log.LogFormat
                };

                Task.Run(async () =>
                {
                    var actualUUID = await _client.Log(realLog);
                    _guidMap[temporaryUUID] = (Guid)actualUUID;
                    logTask.SetResult(actualUUID);
                });
            });

            return temporaryUUID;
        }

        [Obsolete("Use single async log calls to ensure synchronization. This method now acts as a forwarder")]
        public void SendLogBatch(List<Log> logs)
        {
            logs.ForEach(l => Log(l));
        }

        public Guid SendAttachment(Attachment attachment)
        {
            var temporaryUUID = Guid.NewGuid();
            var meta = attachment.MetaData;
            var attachmentTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = attachmentTask;

            var parentTask = GetParentTask(meta.LogUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                var realAttachment = new Attachment()
                {
                    File = attachment.File,
                    MetaData = new AttachmentMetaData()
                    {
                        TestRunUUID = _guidMap[meta.TestRunUUID],
                        TestUUID = _guidMap[meta.TestUUID],
                        StepUUID = meta.StepUUID.HasValue ? _guidMap[meta.StepUUID.Value] : (Guid?)null,
                        LogUUID = (Guid)parent.Result,
                        AttachmentTime = meta.AttachmentTime
                    }
                };

                Task.Run(async () =>
                {
                    var actualUUID = await _client.SendAttachment(realAttachment);
                    attachmentTask.SetResult(actualUUID);
                    _guidMap[temporaryUUID] = (Guid)actualUUID;
                });
            });

            return temporaryUUID;
        }
    }
}