using System;

namespace Promact.Erp.Util
{
    public static class EnvironmentVariableStore
    {
        /// <summary>
        /// This method is used to fetch the value of the environment variable of the given name
        /// </summary>
        /// <param name="VarName"></param>
        /// <returns></returns>
        public static string GetEnvironmentVariableValues(string VarName)
        {
            return Environment.GetEnvironmentVariable(VarName, EnvironmentVariableTarget.Process);
        }
    }
}
