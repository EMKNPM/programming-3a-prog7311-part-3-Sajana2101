using GLMS2.Interfaces;
using GLMS2.Models;
using System.Net;
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
            var response = await _httpClient.GetAsync("api/clients");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var clients = await response.Content
                .ReadFromJsonAsync<List<Client>>(JsonOptions);

            return clients ?? new List<Client>();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/clients/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            return await response.Content
                .ReadFromJsonAsync<Client>(JsonOptions);
        }

        public async Task CreateClientAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", new
            {
                client.Name,
                client.Region,
                client.Email
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/clients/{client.ClientId}",
                new
                {
                    client.ClientId,
                    client.Name,
                    client.Region,
                    client.Email
                });

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            return true;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            return true;
        }

        private static async Task<string> ReadErrorMessageAsync(
            HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return $"API request failed with status code {(int)response.StatusCode}.";
            }

            return body;
        }
    }
}