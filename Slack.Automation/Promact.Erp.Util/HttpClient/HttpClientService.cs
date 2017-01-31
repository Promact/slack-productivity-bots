using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.Util.ExceptionHandler;

namespace Promact.Erp.Util.HttpClient
{
    public class HttpClientService : IHttpClientService
    {

        #region Private Variable


        private System.Net.Http.HttpClient _client;
        private readonly IStringConstantRepository _stringConstant;


        #endregion


        #region Constructor


        public HttpClientService(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseContent</returns>
        public async Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken)
        {
            _client = new System.Net.Http.HttpClient();
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
                return string.Empty;
        }


        /// <summary>
        /// Method to use System.Net.Http.HttpClient's PostAsync method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>        
        /// <param name="accessToken"></param>
        /// <returns>responseString</returns>
        public async Task<string> PostAsync(string baseUrl, string contentString, string contentHeader)
        {
            try
            {
                _client = new System.Net.Http.HttpClient();
                var response = await _client.PostAsync(baseUrl, new StringContent(contentString, Encoding.UTF8, contentHeader));
                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion


    }
}
