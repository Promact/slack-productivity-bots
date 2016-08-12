using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public class HttpClientRepository:IHttpClientRepository
    {
        private readonly HttpClient _client;
        public HttpClientRepository(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>
        /// <returns>response</returns>
        public async Task<HttpResponseMessage> GetAsync(string baseUrl,string contentUrl)
        {
            _client.BaseAddress = new Uri(baseUrl);
            var response = await _client.GetAsync(contentUrl);
            return response;
        }
    }
}
