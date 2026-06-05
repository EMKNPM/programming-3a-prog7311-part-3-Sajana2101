using GLMS2.Interfaces;
using GLMS2.ViewModels;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GLMS2.Services.Api
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GLMSApi");
        }

        public async Task<string?> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                model.Username,
                model.Password
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(body);
            }

            var loginResponse = await response.Content
                .ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

            return loginResponse?.Token;
        }

        private class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;

            public DateTime ExpiresAt { get; set; }
        }
    }
}