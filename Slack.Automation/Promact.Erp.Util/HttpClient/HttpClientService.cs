using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Promact.Erp.Util.StringConstants;
using Autofac.Extras.NLog;

namespace Promact.Erp.Util.HttpClient
{
    public class HttpClientService : IHttpClientService
    {
        private System.Net.Http.HttpClient _client;
        private readonly IStringConstantRepository _stringConstant;
        private readonly ILogger _logger;
        public HttpClientService(IStringConstantRepository stringConstant, ILogger logger)
        {
            _stringConstant = stringConstant;
            _logger = logger;
        }

        /// <summary>
        /// Method to use System.Net.Http.HttpClient's GetAsync method
        /// </summary>
        /// <param name="baseUrl">base url</param>
        /// <param name="contentUrl">rest of url</param>        
        /// <param name="accessToken">access token</param>
        /// <param name="accessTokenType">access token type</param>
        /// <returns>responseContent</returns>
        /// <exception cref="HttpRequestException">Exception will be when request server is closed</exception>
        public async Task<string> GetAsync(string baseUrl, string contentUrl, string accessToken, string accessTokenType)
        {
            try
            {
                _client = new System.Net.Http.HttpClient();
                _logger.Debug("Promact url : " + baseUrl);
                _client.BaseAddress = new Uri(baseUrl);
                // Added access token to request header if provided by user
                if (!String.IsNullOrEmpty(accessToken))
                {
                    _logger.Debug("Access token is not null" + accessToken);
                    if (accessTokenType == _stringConstant.Bearer)
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_stringConstant.Bearer, accessToken);
                    else
                        _client.DefaultRequestHeaders.Add(accessTokenType, accessToken);
                }
                _logger.Debug("ContentUrl : " + contentUrl);
                var response = await _client.GetAsync(contentUrl);
                _client.Dispose();
                string responseContent = null;
                _logger.Debug("Status code : " + response.StatusCode);
                _logger.Debug("Return content : " + responseContent);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseContent = response.Content.ReadAsStringAsync().Result;
                }
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error("Error in HttpRequest : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw new Exception(_stringConstant.HttpRequestExceptionErrorMessage);
            }
        }


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
        public async Task<string> PostAsync(string baseUrl, string contentString, string contentHeader, string accessToken, string accessTokenType)
        {
            try
            {
                _client = new System.Net.Http.HttpClient();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    _client.DefaultRequestHeaders.Add(accessTokenType, accessToken);
                }
                var response = await _client.PostAsync(baseUrl, new StringContent(contentString, Encoding.UTF8, contentHeader));
                _client.Dispose();
                string responseString = null;
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    responseString = response.Content.ReadAsStringAsync().Result;
                }
                return responseString;
            }
            catch (HttpRequestException)
            {
                throw new Exception(_stringConstant.HttpRequestExceptionErrorMessage);
            }
        }
    }
}
