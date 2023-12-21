using Orangebeard.Client.V3.Entity.Attachment;
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
            _client = new OrangebeardV3Client(config.Endpoint, Guid.Parse(config.AccessToken), config.ProjectName, true);
            _tasks = new ConcurrentDictionary<Guid?, TaskCompletionSource<object>>();
            _guidMap = new ConcurrentDictionary<Guid, Guid>();
        }

        public OrangebeardAsyncV3Client(string endpoint, Guid accessToken, string projectName, bool connectionWithOrangebeardIsValid)
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
            Guid temporaryUUID = Guid.NewGuid();
            _context = new TestRunContext(temporaryUUID);
            var startTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(temporaryUUID, startTestRunTask);

            Task.Run(async () =>
            {
                Guid? actualUUID = await _client.StartTestRun(testRun);
                if (actualUUID != null)
                {
                    _guidMap[temporaryUUID] = (Guid) actualUUID;
                    startTestRunTask.SetResult(actualUUID);
                } else
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
            int pendingTasks = _tasks.Values.Where(t => !t.Task.IsCompleted).Count();
            Console.WriteLine("Waiting for {0}/{1} Orangebeard calls to finish...", pendingTasks, _tasks.Count()); 
            Task.WhenAll(_tasks.Values.Select(task => task.Task).ToArray()).ContinueWith(parent =>
            {
                Console.WriteLine("Finishing run!");
                Task.WaitAll(_client.FinishTestRun(_guidMap[testRunUUID], finishTestRun));
            });
        }

        public List<Guid> StartSuite(StartSuite startSuite)
        {
            List<Guid> tempUUIDs = startSuite.SuiteNames.Select(_ => Guid.NewGuid()).ToList();
            _context.StartSuites(startSuite.SuiteNames, tempUUIDs);

            TaskCompletionSource<object> startSuiteTask = new TaskCompletionSource<object>();
            tempUUIDs.ForEach(tempUUID => _tasks[tempUUID] = startSuiteTask);

            TaskCompletionSource<object> parentTask = startSuite.ParentSuiteUUID != null ?
                GetParentTask(startSuite.ParentSuiteUUID) :
                GetParentTask(startSuite.TestRunUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testRunUUID;
                Guid? parentSuiteUUID = null;
                if (startSuite.ParentSuiteUUID == null)
                {
                    testRunUUID = (Guid) parent.Result;
                }
                else
                {
                    testRunUUID = _guidMap[startSuite.TestRunUUID];
                    parentSuiteUUID = parent.Result is List<Guid> ? ((List<Guid>)parent.Result).Last() : (Guid)parent.Result;
                }

                StartSuite realStartSuite = new StartSuite()
                {
                    TestRunUUID = testRunUUID,
                    ParentSuiteUUID = parentSuiteUUID,
                    Description = startSuite.Description,
                    Attributes = startSuite.Attributes,
                    SuiteNames = startSuite.SuiteNames
                };

                Task.Run(async () =>
                {
                    List<Suite> suites = await _client.StartSuite(realStartSuite);
                    List<Guid> actualUUIDs = suites.Select(suite => suite.SuiteUUID).ToList();
                    startSuiteTask.SetResult(actualUUIDs);
                });
            });

            return tempUUIDs;
        }

        public Guid StartTest(StartTest startTest)
        {
            Guid temporaryUUID = Guid.NewGuid();
            _context.StartTest(temporaryUUID);
            
            TaskCompletionSource<object> startTestTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = startTestTask;

            TaskCompletionSource<object> parentTask = GetParentTask(startTest.SuiteUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid parentSuiteUUID = ((List<Guid>)parent.Result).Last();

                StartTest realStartTest = new StartTest()
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
                    Guid actualUUID = (Guid)await _client.StartTest(realStartTest);
                    _guidMap[temporaryUUID] = actualUUID;
                    startTestTask.SetResult(actualUUID);
                });                
            });

            return temporaryUUID;
        }

        public void FinishTest(Guid testUUID, FinishTest finishTest)
        {
            _context.FinishTest(testUUID);
            TaskCompletionSource<object> finishTestTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishTestTask;

            TaskCompletionSource<object> parentTask = GetParentTask(testUUID);

            parentTask.Task.ContinueWith(parent =>
            {
            FinishTest realFinishTest = new FinishTest()
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
            Guid temporaryUUID = Guid.NewGuid();
            _context.StartStep(temporaryUUID);

            TaskCompletionSource<object> startStepTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = startStepTask;

            TaskCompletionSource<object> parentTask = startStep.ParentStepUUID != null ?
                GetParentTask(startStep.ParentStepUUID) :
                GetParentTask(startStep.TestUUID);

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

                StartStep realStartStep = new StartStep()
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
                    Guid actualUUID = (Guid)await _client.StartStep(realStartStep);
                    _guidMap[temporaryUUID] = actualUUID;
                    startStepTask.SetResult(actualUUID);
                });                
            });

            return temporaryUUID;
        }

        public void FinishStep(Guid stepUUID, FinishStep finishStep)
        {
            _context.FinishStep(stepUUID);

            TaskCompletionSource<object> finishStepTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishStepTask;

            TaskCompletionSource<object> parentTask = GetParentTask(stepUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                FinishStep realFinishStep = new FinishStep()
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
            Guid temporaryUUID = Guid.NewGuid();
            TaskCompletionSource<object> logTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = logTask;

            TaskCompletionSource<object> parentTask = log.StepUUID != null ?
                GetParentTask(log.StepUUID) :
                GetParentTask(log.TestUUID);

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

                Log realLog = new Log()
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
                    Guid actualUUID = (Guid) await _client.Log(realLog);
                    _guidMap[temporaryUUID] = actualUUID;
                    logTask.SetResult(actualUUID);
                });
            });

            return temporaryUUID;
        }

        [Obsolete ("Use single async log calls to ensure synchronization. This method now acts as a forwarder")]
        public void SendLogBatch(List<Log> logs)
        {
            logs.ForEach(l => Log(l));
        }

        public Guid SendAttachment(Attachment attachment)
        {
            Guid temporaryUUID = Guid.NewGuid();
            AttachmentMetaData meta = attachment.MetaData;
            TaskCompletionSource<object> attachmentTask = new TaskCompletionSource<object>();
            _tasks[temporaryUUID] = attachmentTask;

            TaskCompletionSource<object> parentTask = GetParentTask(meta.LogUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Attachment realAttachment = new Attachment()
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
                    Guid actualUUID = (Guid)await _client.SendAttachment(realAttachment);
                    attachmentTask.SetResult(actualUUID);
                    _guidMap[temporaryUUID] = actualUUID;
                });
            });

            return temporaryUUID;
        }
    }
}
