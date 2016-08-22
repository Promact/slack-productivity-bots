using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public class HttpClientRepository:IHttpClientRepository
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
        public async Task<HttpResponseMessage> GetAsync(string baseUrl,string contentUrl)
        {
            try
            {
                _client = new HttpClient();
                //_client.BaseAddress = new Uri(baseUrl);
                var url = baseUrl + contentUrl;
                var response = await _client.GetAsync(url);
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
