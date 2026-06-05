using GLMS2.Interfaces;
using GLMS2.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace GLMS2.Services.Api
{
    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ClientApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GLMSApi");
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            var clients = await _httpClient
                .GetFromJsonAsync<List<Client>>("api/clients", JsonOptions);

            return clients ?? new List<Client>();
        }
    }
}