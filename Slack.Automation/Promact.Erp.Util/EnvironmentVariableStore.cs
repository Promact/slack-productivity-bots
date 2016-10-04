using System;

namespace Promact.Erp.Util
{
    public class EnvironmentVariableStore
    {

        /// <summary>
        /// This method is used to fetch the value of the environment variable of the given name
        /// </summary>
        /// <param name="VarName"></param>
        /// <returns></returns>
        public string FetchEnvironmentVariableValues(string VarName)
        {
            return Environment.GetEnvironmentVariable(VarName, EnvironmentVariableTarget.Process);
        }
    }
}
