using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.HttpClientRepository
{
    public interface IHttpClientRepository
    {
        Task<string> GetAsync(string baseUrl, string contentUrl,string accessToken);
    }
}
