using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository
{
    public static class EnvironmentVariableStore
    {
        public static string SetEnvironmentVariables(string VarName)
        {
            return Environment.GetEnvironmentVariable(VarName, EnvironmentVariableTarget.Process);
        }
    }
}
