using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public interface IHttpClientRepository
    {
        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseContent</returns>
        Task<string> GetAsync(string baseUrl, string contentUrl,string accessToken);
    }
}
