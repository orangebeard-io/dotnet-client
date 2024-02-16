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
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _tasks;
        private readonly ConcurrentDictionary<Guid, Guid> _guidMap;
        private readonly OrangebeardV3Client _client;

        private TestRunContext _context;

        public OrangebeardAsyncV3Client(OrangebeardConfiguration config)
        {
            _client = new OrangebeardV3Client(config.Endpoint, Guid.Parse(config.AccessToken), config.ProjectName,
                true, config.ListenerIdentification);
            _tasks = new ConcurrentDictionary<Guid, TaskCompletionSource<object>>();
            _guidMap = new ConcurrentDictionary<Guid, Guid>();
        }

        public OrangebeardAsyncV3Client(string endpoint, Guid accessToken, string projectName,
            bool connectionWithOrangebeardIsValid)
        {
            _client = new OrangebeardV3Client(endpoint, accessToken, projectName, connectionWithOrangebeardIsValid);
            _tasks = new ConcurrentDictionary<Guid, TaskCompletionSource<object>>();
            _guidMap = new ConcurrentDictionary<Guid, Guid>();
        }

        private TaskCompletionSource<object> GetParentTask(Guid taskGuid)
        {
            return _tasks[taskGuid];
        }

        public TestRunContext TestRunContext()
        {
            return _context;
        }

        public Guid StartTestRun(StartTestRun testRun)
        {
            var temporaryUuid = Guid.NewGuid();
            _context = new TestRunContext(temporaryUuid);
            var startTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(temporaryUuid, startTestRunTask);

            Task.Run(async () =>
            {
                var actualUuid = await _client.StartTestRun(testRun);
                CompleteTask(startTestRunTask, temporaryUuid, actualUuid);
            });

            return temporaryUuid;
        }

        public void StartAnnouncedTestRun(Guid testRunUuid)
        {
            _context = new TestRunContext(testRunUuid);
            var startTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(testRunUuid, startTestRunTask);

            Task.Run(async () =>
            {
                await _client.StartAnnouncedTestRun(testRunUuid);
                _guidMap[testRunUuid] = testRunUuid;
                startTestRunTask.SetResult(true);
            });
        }

        public void UpdateTestRun(Guid testRunUuid, UpdateTestRun updateTestRun)
        {
            var updateTestRunTask = new TaskCompletionSource<object>();
            _tasks.TryAdd(Guid.NewGuid(), updateTestRunTask);
            var parentTask = GetParentTask(testRunUuid);
            parentTask.Task.ContinueWith(parent =>
            {
                testRunUuid = (Guid)parent.Result;
                Task.Run(async () =>
                {
                    await _client.UpdateTestRun(testRunUuid, updateTestRun);
                    updateTestRunTask.SetResult(true);
                });
            });
        }

        public void FinishTestRun(Guid testRunUuid, FinishTestRun finishTestRun)
        {
            var pendingTasks = _tasks.Values.Count(t => !t.Task.IsCompleted);
            Console.WriteLine("Waiting for {0}/{1} Orangebeard calls to finish...", pendingTasks, _tasks.Count);

            var timeoutInSeconds =
                int.Parse(Environment.GetEnvironmentVariable("orangebeard.finishtestruntimeout") ?? "60");
            
            var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            var completed = Task.WhenAll(_tasks.Values.Select(task => task.Task).ToArray()).Wait(timeout);
            
            if (!completed)
            {
                Console.WriteLine("Timed out waiting for calls to finish after {1}s. {0} calls are still pending.",
                    _tasks.Values.Count(t => !t.Task.IsCompleted), timeoutInSeconds);
            }
            
            Console.WriteLine("Finishing run in Orangebeard!");
            Task.Run(() => _client.FinishTestRun(_guidMap[testRunUuid], finishTestRun)).Wait(timeout);
        }

        public List<Guid> StartSuite(StartSuite startSuite)
        {
            var tempUuiDs = startSuite.SuiteNames.Select(_ => Guid.NewGuid()).ToList();
            _context.StartSuites(startSuite.SuiteNames, tempUuiDs);

            var startSuiteTask = new TaskCompletionSource<object>();
            tempUuiDs.ForEach(tempUuid => _tasks[tempUuid] = startSuiteTask);

            var parentTask = startSuite.ParentSuiteUUID != null
                ? GetParentTask(startSuite.ParentSuiteUUID.Value)
                : GetParentTask(startSuite.TestRunUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testRunUuid;
                Guid? parentSuiteUuid = null;
                if (startSuite.ParentSuiteUUID == null)
                {
                    testRunUuid = (Guid)parent.Result;
                }
                else
                {
                    testRunUuid = _guidMap[startSuite.TestRunUUID];
                    parentSuiteUuid = parent.Result is List<Guid> list ? list.Last() : (Guid)parent.Result;
                }

                var realStartSuite = new StartSuite()
                {
                    TestRunUUID = testRunUuid,
                    ParentSuiteUUID = parentSuiteUuid,
                    Description = startSuite.Description,
                    Attributes = startSuite.Attributes,
                    SuiteNames = startSuite.SuiteNames
                };

                Task.Run(async () =>
                {
                    var suites = await _client.StartSuite(realStartSuite);
                    var actualUuiDs = suites.Select(suite => suite.SuiteUUID).ToList();
                    startSuiteTask.SetResult(actualUuiDs);
                });
            });

            return tempUuiDs;
        }

        public Guid StartTest(StartTest startTest)
        {
            var temporaryUuid = Guid.NewGuid();
            _context.StartTest(temporaryUuid);

            var startTestTask = new TaskCompletionSource<object>();
            _tasks[temporaryUuid] = startTestTask;

            var parentTask = GetParentTask(startTest.SuiteUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                var parentSuiteUuid = ((List<Guid>)parent.Result).Last();
                var realStartTest = new StartTest()
                {
                    TestRunUUID = _guidMap[startTest.TestRunUUID],
                    SuiteUUID = parentSuiteUuid,
                    TestName = startTest.TestName,
                    TestType = startTest.TestType,
                    Description = startTest.Description,
                    Attributes = startTest.Attributes,
                    StartTime = startTest.StartTime
                };

                Task.Run(async () =>
                {
                    var actualUuid = await _client.StartTest(realStartTest);
                    CompleteTask(startTestTask, temporaryUuid, actualUuid);
                });
            });

            return temporaryUuid;
        }

        public void FinishTest(Guid testUuid, FinishTest finishTest)
        {
            _context.FinishTest(testUuid);
            var finishTestTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishTestTask;

            var parentTask = GetParentTask(testUuid);

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
            var temporaryUuid = Guid.NewGuid();
            _context.StartStep(temporaryUuid);

            var startStepTask = new TaskCompletionSource<object>();
            _tasks[temporaryUuid] = startStepTask;

            var parentTask = startStep.ParentStepUUID != null
                ? GetParentTask(startStep.ParentStepUUID.Value)
                : GetParentTask(startStep.TestUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testUuid;
                Guid? parentStepUuid = null;
                if (startStep.ParentStepUUID == null)
                {
                    testUuid = (Guid)parent.Result;
                }
                else
                {
                    testUuid = _guidMap[startStep.TestUUID];
                    parentStepUuid = (Guid)parent.Result;
                }

                var realStartStep = new StartStep()
                {
                    TestRunUUID = _guidMap[startStep.TestRunUUID],
                    TestUUID = testUuid,
                    ParentStepUUID = parentStepUuid,
                    StepName = startStep.StepName,
                    Description = startStep.Description,
                    StartTime = startStep.StartTime
                };

                Task.Run(async () =>
                {
                    var actualUuid = await _client.StartStep(realStartStep);
                    CompleteTask(startStepTask, temporaryUuid, actualUuid);
                });
            });

            return temporaryUuid;
        }

        public void FinishStep(Guid stepUuid, FinishStep finishStep)
        {
            _context.FinishStep(stepUuid);

            var finishStepTask = new TaskCompletionSource<object>();
            _tasks[Guid.NewGuid()] = finishStepTask;

            var parentTask = GetParentTask(stepUuid);

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
            var temporaryUuid = Guid.NewGuid();
            var logTask = new TaskCompletionSource<object>();
            _tasks[temporaryUuid] = logTask;

            var parentTask = log.StepUUID != null ? GetParentTask(log.StepUUID.Value) : GetParentTask(log.TestUUID);

            parentTask.Task.ContinueWith(parent =>
            {
                Guid testUuid;
                Guid? stepUuid = null;
                if (log.StepUUID == null)
                {
                    testUuid = (Guid)parent.Result;
                }
                else
                {
                    testUuid = _guidMap[log.TestUUID];
                    stepUuid = (Guid)parent.Result;
                }

                var realLog = new Log()
                {
                    TestRunUUID = _guidMap[log.TestRunUUID],
                    TestUUID = testUuid,
                    StepUUID = stepUuid,
                    Message = log.Message,
                    LogLevel = log.LogLevel,
                    LogTime = log.LogTime,
                    LogFormat = log.LogFormat
                };

                Task.Run(async () =>
                {
                    var actualUuid = await _client.Log(realLog);
                    CompleteTask(logTask, temporaryUuid, actualUuid);
                });
            });

            return temporaryUuid;
        }

        [Obsolete("Use single async log calls to ensure synchronization. This method now acts as a forwarder")]
        public void SendLogBatch(List<Log> logs)
        {
            logs.ForEach(l => Log(l));
        }

        public Guid SendAttachment(Attachment attachment)
        {
            var temporaryUuid = Guid.NewGuid();
            var meta = attachment.MetaData;
            var attachmentTask = new TaskCompletionSource<object>();
            _tasks[temporaryUuid] = attachmentTask;

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
                    var actualUuid = await _client.SendAttachment(realAttachment);
                    CompleteTask(attachmentTask, temporaryUuid, actualUuid);
                });
            });

            return temporaryUuid;
        }

        private void CompleteTask(TaskCompletionSource<object> task, Guid temporaryUuid, Guid? result)
        {
            if (result.HasValue)
            {
                _guidMap[temporaryUuid] = result.Value;
                task.SetResult(result.Value);
            }
            else
            {
                task.SetCanceled();
            }
        }
    }
}