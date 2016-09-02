using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public class HttpClientRepository : IHttpClientRepository
    {
        private HttpClient _client;
        public HttpClientRepository()
        {

        }

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseContent</returns>
        public async Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken)
        {
            try
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(baseUrl);
                // Added access token to request header if provided by user
                if (accessToken != null)
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);
                }
                var response = await _client.GetAsync(contentUrl);
                _client.Dispose();
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return responseContent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
