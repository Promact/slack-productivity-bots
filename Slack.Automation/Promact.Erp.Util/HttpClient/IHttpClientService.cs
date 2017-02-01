using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.Util.HttpClient
{
    public interface IHttpClientService
    {
        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseContent</returns>
        Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken);

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <returns>responseContent</returns>
        Task<string> GetAsync(string baseUrl, string contentUrl);

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's PostAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentString"></param>
        /// <param name="contentHeader"></param>
        /// <returns></returns>
        Task<string> PostAsync(string baseUrl, string contentString, string contentHeader);
    }
}
