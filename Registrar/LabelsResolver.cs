using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using NUnit.Framework.Interfaces;

namespace ZafiraIntegration.Registrar
{
    internal static class LabelsResolver
    {
        internal static List<Client.Requests.Label> ResolveLabels(ITest test)
        {
            var keyToValues = new Dictionary<string, string[]>();

            var classTestLabels = test.TypeInfo?.GetCustomAttributes<TestLabel>(true) ?? new TestLabel[] { };
            foreach (var testLabel in classTestLabels)
            {
                keyToValues[testLabel.Key] = testLabel.Values;
            }

            var methodTestLabels = test.Method?.MethodInfo.GetCustomAttributes<TestLabel>(true) ?? new TestLabel[] { };
            foreach (var testLabel in methodTestLabels)
            {
                keyToValues[testLabel.Key] = testLabel.Values;
            }

            return keyToValues.SelectMany(
                pair => pair.Value.Select(value => new Client.Requests.Label(pair.Key, value))
            ).ToList();
        }
    }
}