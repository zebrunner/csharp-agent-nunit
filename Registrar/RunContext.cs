using System;
using ZafiraIntegration.Client.Responses;

namespace ZafiraIntegration.Registrar
{
    internal static class RunContext
    {
        private static SaveTestRunResponse _saveTestRunResponse;
        [ThreadStatic] private static SaveTestResponse _currentSaveTestResponse;

        internal static void SetCurrentTestRun(SaveTestRunResponse saveTestRunResponse)
        {
            _saveTestRunResponse = saveTestRunResponse;
        }

        internal static SaveTestRunResponse GetCurrentTestRun()
        {
            return _saveTestRunResponse;
        }

        internal static void SetCurrentTest(SaveTestResponse saveTestResponse)
        {
            _currentSaveTestResponse = saveTestResponse;
        }

        internal static SaveTestResponse GetCurrentTest()
        {
            return _currentSaveTestResponse;
        }

        internal static void RemoveCurrentTest()
        {
            _currentSaveTestResponse = null;
        }
    }
}