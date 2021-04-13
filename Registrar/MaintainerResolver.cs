using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace ZafiraIntegration.Registrar
{
    public static class MaintainerResolver
    {
        public static string ResolveMaintainer(ITest test)
        {
            return (test.Method?.MethodInfo.GetCustomAttribute(typeof(Maintainer), true) as Maintainer)?.Username
                   ?? test.TypeInfo?.GetCustomAttributes<Maintainer>(true).FirstOrDefault()?.Username;
        }
    }
}