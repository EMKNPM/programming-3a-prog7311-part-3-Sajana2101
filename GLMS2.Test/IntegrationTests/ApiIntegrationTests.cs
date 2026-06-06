using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GLMS2.Test.IntegrationTests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiIntegrationTests()
        {
            var handler = new HttpClientHandler
            {
                // Local development only: avoids HTTPS certificate issues during integration tests.
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7160/")
            };
        }

        [Fact]
        public async Task GetContracts_ReturnsOk_AndJsonData()
        {
            var response = await _client.GetAsync("api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsJwtToken()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                username = "admin",
                password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.Contains("token", json, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                username = "wrong",
                password = "wrong"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateContract_WithoutToken_ReturnsUnauthorized()
        {
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent("7"), "ClientId");
            form.Add(new StringContent("2026-06-01"), "StartDate");
            form.Add(new StringContent("2026-12-31"), "EndDate");
            form.Add(new StringContent("Draft"), "Status");
            form.Add(new StringContent("Standard Freight"), "ServiceLevel");
            form.Add(new StringContent("Domestic"), "ContractType");

            var pdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 test pdf content");
            var fileContent = new ByteArrayContent(pdfBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            form.Add(fileContent, "SignedAgreementFile", "test-contract.pdf");

            var response = await _client.PostAsync("api/contracts", form);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateClient_CreateContract_UpdateStatus_CreateServiceRequest_FlowWorks()
        {
            var token = await LoginAndGetTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 1. Create client
            var clientResponse = await _client.PostAsJsonAsync("api/clients", new
            {
                name = $"Integration Test Client {Guid.NewGuid()}",
                region = "South Africa",
                email = $"test{Guid.NewGuid():N}@example.com"
            });

            Assert.True(
                clientResponse.IsSuccessStatusCode,
                await clientResponse.Content.ReadAsStringAsync());

            var clientJson = await clientResponse.Content.ReadAsStringAsync();
            var clientId = GetIntProperty(clientJson, "clientId");

            Assert.True(clientId > 0);

            // 2. Create contract with PDF
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(clientId.ToString()), "ClientId");
            form.Add(new StringContent("2026-06-01"), "StartDate");
            form.Add(new StringContent("2026-12-31"), "EndDate");
            form.Add(new StringContent("Draft"), "Status");
            form.Add(new StringContent("Standard Freight"), "ServiceLevel");
            form.Add(new StringContent("Domestic"), "ContractType");

            var pdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 integration test pdf content");
            var fileContent = new ByteArrayContent(pdfBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            form.Add(fileContent, "SignedAgreementFile", "integration-test-contract.pdf");

            var contractResponse = await _client.PostAsync("api/contracts", form);

            Assert.True(
                contractResponse.IsSuccessStatusCode,
                await contractResponse.Content.ReadAsStringAsync());

            var contractJson = await contractResponse.Content.ReadAsStringAsync();
            var contractId = GetIntProperty(contractJson, "contractId");

            Assert.True(contractId > 0);

            // 3. Patch contract status to Active
            var patchResponse = await _client.PatchAsJsonAsync(
                $"api/contracts/{contractId}/status",
                new
                {
                    status = "Active"
                });

            Assert.True(
                patchResponse.IsSuccessStatusCode,
                await patchResponse.Content.ReadAsStringAsync());

            var patchedJson = await patchResponse.Content.ReadAsStringAsync();

            Assert.Contains("Active", patchedJson);

            // 4. Create service request against active contract
            var serviceRequestResponse = await _client.PostAsJsonAsync(
                "api/serviceRequests",
                new
                {
                    contractId = contractId,
                    description = "Integration test shipping request",
                    costUSD = 100.00m,
                    exchangeRate = 0,
                    costZAR = 0
                });

            Assert.True(
                serviceRequestResponse.IsSuccessStatusCode,
                await serviceRequestResponse.Content.ReadAsStringAsync());

            var serviceRequestJson =
                await serviceRequestResponse.Content.ReadAsStringAsync();

            Assert.Contains("costZAR", serviceRequestJson, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Integration test shipping request", serviceRequestJson);
        }

        [Fact]
        public async Task GetExchangeRate_ReturnsPositiveRate()
        {
            var response = await _client.GetAsync("api/serviceRequests/exchange-rate");

            Assert.True(
                response.IsSuccessStatusCode,
                await response.Content.ReadAsStringAsync());

            var json = await response.Content.ReadAsStringAsync();

            var rate = GetDecimalProperty(json, "rate");

            Assert.True(rate > 0);
        }

        private async Task<string> LoginAndGetTokenAsync()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                username = "admin",
                password = "Admin@123"
            });

            Assert.True(
                response.IsSuccessStatusCode,
                await response.Content.ReadAsStringAsync());

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty("token")
                .GetString()!;
        }

        private static int GetIntProperty(string json, string propertyName)
        {
            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty(propertyName)
                .GetInt32();
        }

        private static decimal GetDecimalProperty(string json, string propertyName)
        {
            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty(propertyName)
                .GetDecimal();
        }
    }
}