using System.Net.Http;

namespace CodeBucket.Core.Services
{
    public interface IHttpClientService
    {
		HttpClient Create();
    }
}

