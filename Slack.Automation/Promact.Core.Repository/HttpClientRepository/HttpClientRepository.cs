using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;

namespace Promact.Core.Repository.HttpClientRepository
{
    public class HttpClientRepository : IHttpClientRepository
    {


        #region Private Variable

        private HttpClient _client;
        private readonly IStringConstantRepository _stringConstant;

        #endregion


        #region Constructor

        public HttpClientRepository(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
        }

        #endregion


        #region Public Method

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseContent</returns>
        public async Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
            // Added access token to request header if provided by user
            if (!String.IsNullOrEmpty(accessToken))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);

            HttpResponseMessage response = await _client.GetAsync(contentUrl);
            _client.Dispose();
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                return responseContent;
            }
            else if (response.ReasonPhrase.Equals(_stringConstant.Forbidden))
                throw new ForbiddenUserException(_stringConstant.UnAuthorized);
            else
                return null;
        }

        #endregion


    }
}
