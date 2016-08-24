using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public interface IHttpClientRepository
    {
        Task<HttpResponseMessage> GetAsync(string baseUrl, string contentUrl, string accessToken);
    }
}
