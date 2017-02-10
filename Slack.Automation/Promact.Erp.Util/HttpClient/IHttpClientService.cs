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
        /// <exception cref="HttpRequestException">Exception will be when request server is closed</exception>
        Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken);


        /// <summary>
        /// Method to use System.Net.Http.HttpClient's PostAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentString"></param>
        /// <param name="contentHeader"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Exception will be when request server is closed</exception>
        Task<string> PostAsync(string baseUrl, string contentString, string contentHeader);
    }
}
