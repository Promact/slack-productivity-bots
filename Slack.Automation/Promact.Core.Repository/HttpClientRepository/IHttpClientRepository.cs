using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public interface IHttpClientRepository
    {
        Task<HttpResponseMessage> GetAsync(string baseUrl, string contentUrl);
    }
}
