using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace ZafiraIntegration.Registrar
{
    internal static class MaintainerResolver
    {
        internal static string ResolveMaintainer(ITest test)
        {
            return (test.Method?.MethodInfo.GetCustomAttribute(typeof(Maintainer), true) as Maintainer)?.Username
                   ?? test.TypeInfo?.GetCustomAttributes<Maintainer>(true).FirstOrDefault()?.Username;
        }
    }
}