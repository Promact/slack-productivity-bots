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
        /// <returns>response</returns>
        public async Task<HttpResponseMessage> GetAsync(string baseUrl, string contentUrl, string accessToken)
        {
            try
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(baseUrl);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);
                var response = await _client.GetAsync(contentUrl);
                _client.Dispose();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
