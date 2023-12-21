using System;
using System.Collections.Generic;
using System.Linq;

namespace Orangebeard.Client.V3
{
    public class TestRunContext
    {
        public Guid TestRun;
        public Dictionary<Guid, string> _suites;
        public List<Guid> activeSuiteIds;
        public List<Guid> activeTestIds;
        public List<Guid> activeStepIds;


        public TestRunContext(Guid testRunGuid)
        {
            TestRun = testRunGuid;
            _suites = new Dictionary<Guid, string>();
            activeSuiteIds = new List<Guid>();
            activeStepIds = new List<Guid>();
            activeTestIds = new List<Guid>();
        }


        public void StartSuite(string suiteName, Guid suiteId)
        {
            _suites.Add(suiteId, suiteName);
            activeSuiteIds.Add(suiteId);
        }

        public void StartSuites(IList<string> suiteNames, IList<Guid> suiteIds)
        {
            for (int i = suiteNames.Count - 1; i >= 0; i--)
            {
                _suites.Add(suiteIds[i], suiteNames[i]);
                activeSuiteIds.Add(suiteIds[i]);
            }            
        }

        public void StartTest(Guid testId)
        {
            activeTestIds.Add(testId);
        }

       
        public void StartStep(Guid stepId)
        {
            activeStepIds.Add(stepId);
        }

        public void FinishStep(Guid stepId)
        {
            activeStepIds.Remove(stepId);           
        }

        public void FinishTest(Guid testId)
        {
           activeTestIds.Remove(testId); 
        }

        public void FinishSuite(Guid suiteId)
        {
            activeSuiteIds.Remove(suiteId);
        }
        public Guid ActiveSuite()
        {
            return activeSuiteIds.ElementAtOrDefault(activeSuiteIds.Count - 1);
        }

        public Guid? ActiveTest()
        {
            return activeTestIds.Count > 0 ? (Guid?)activeTestIds[activeTestIds.Count - 1] : null;
        }

        public Guid? ActiveStep()
        {
            return activeStepIds.Count > 0 ? (Guid?)activeStepIds[activeStepIds.Count - 1] : null;
        }
    }
}
