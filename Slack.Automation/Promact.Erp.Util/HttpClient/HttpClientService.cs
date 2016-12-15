using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Util.HttpClient
{
    public class HttpClientService : IHttpClientService
    {
        private System.Net.Http.HttpClient _client;
        private readonly IStringConstantRepository _stringConstant;
        public HttpClientService(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
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
                _client = new System.Net.Http.HttpClient();
                _client.BaseAddress = new Uri(baseUrl);
                // Added access token to request header if provided by user
                if (!String.IsNullOrEmpty(accessToken))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_stringConstant.Bearer,accessToken);
                }
                var response = await _client.GetAsync(contentUrl);
                _client.Dispose();
                var responseContent = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseContent;
                }
                else
                    throw new Exception(responseContent);
            }
            catch(HttpRequestException)
            {
                throw new Exception(_stringConstant.HttpRequestExceptionErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                _client.Dispose();
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseString;
                }
                else
                    throw new Exception(responseString);
            }
            catch (HttpRequestException)
            {
                throw new Exception(_stringConstant.HttpRequestExceptionErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
