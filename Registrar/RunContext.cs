using System;
using ZafiraIntegration.Client.Responses;

namespace ZafiraIntegration.Registrar
{
    public static class RunContext
    {
        internal static SaveTestRunResponse SaveTestRunResponse { get; set; }
        [ThreadStatic] private static SaveTestResponse _currentSaveTestResponse;

        public static void SetCurrentTest(SaveTestResponse saveTestResponse)
        {
            _currentSaveTestResponse = saveTestResponse;
        }

        public static SaveTestResponse GetCurrentTest()
        {
            return _currentSaveTestResponse;
        }

        public static void RemoveCurrentTest()
        {
            _currentSaveTestResponse = null;
        }
    }
}