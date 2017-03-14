using System.Threading.Tasks;

namespace Promact.Erp.Util.HttpClient
{
    public interface IHttpClientService
    {
        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl">base url</param>
        /// <param name="contentUrl">rest of url</param>        
        /// <param name="accessToken">access token</param>
        /// <param name="accessTokenType">access token type</param>
        /// <returns>responseContent</returns>
        /// <exception cref="HttpRequestException">Exception will be when request server is closed</exception>
        Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken, string accessTokenType);


        /// <summary>
        /// Method to use System.Net.Http.HttpClient's PostAsync method
        /// </summary>
        /// <param name="baseUrl">base url</param>
        /// <param name="contentString">text to be send</param>
        /// <param name="contentHeader">content header</param>
        /// <param name="accessToken">access token</param>
        /// <param name="accessTokenType">access token type</param>
        /// <returns>responseString</returns>
        /// <exception cref="HttpRequestException">Exception will be when request server is closed</exception>
        Task<string> PostAsync(string baseUrl, string contentString, string contentHeader, string accessToken, string accessTokenType);
    }
}
