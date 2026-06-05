using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GLMS2.Services.Api
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public ServiceRequestApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GLMSApi");
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            var response = await _httpClient.GetAsync("api/serviceRequests");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiRequests = await response.Content
                .ReadFromJsonAsync<List<ServiceRequestApiDto>>(JsonOptions);

            return apiRequests?.Select(ToServiceRequest).ToList()
                   ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/serviceRequests/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiRequest = await response.Content
                .ReadFromJsonAsync<ServiceRequestApiDto>(JsonOptions);

            return apiRequest == null ? null : ToServiceRequest(apiRequest);
        }

        public async Task<ServiceRequestEditViewModel?> GetServiceRequestForEditAsync(int id)
        {
            var serviceRequest = await GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return null;
            }

            return new ServiceRequestEditViewModel
            {
                ServiceRequestId = serviceRequest.ServiceRequestId,
                ContractId = serviceRequest.ContractId,
                Description = serviceRequest.Description,
                CostUSD = serviceRequest.CostUSD,
                CostZAR = serviceRequest.CostZAR
            };
        }

        public async Task<ServiceRequest?> CreateServiceRequestAsync(
            ServiceRequestCreateViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/serviceRequests",
                model,
                JsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiRequest = await response.Content
                .ReadFromJsonAsync<ServiceRequestApiDto>(JsonOptions);

            return apiRequest == null ? null : ToServiceRequest(apiRequest);
        }

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequestEditViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/serviceRequests/{model.ServiceRequestId}",
                model,
                JsonOptions);

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

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/serviceRequests/{id}");

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

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("api/serviceRequests/exchange-rate");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var result = await response.Content
                .ReadFromJsonAsync<ExchangeRateApiDto>(JsonOptions);

            return result?.Rate ?? 0;
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            var response = await _httpClient.GetAsync("api/contracts?status=Active");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiContracts = await response.Content
                .ReadFromJsonAsync<List<ContractApiDto>>(JsonOptions);

            return apiContracts?.Select(ToContract).ToList()
                   ?? new List<Contract>();
        }

        private static ServiceRequest ToServiceRequest(ServiceRequestApiDto dto)
        {
            return new ServiceRequest
            {
                ServiceRequestId = dto.ServiceRequestId,
                ContractId = dto.ContractId,
                Description = dto.Description,
                CostUSD = dto.CostUSD,
                CostZAR = dto.CostZAR,
                Status = dto.Status,
                CreatedDate = dto.CreatedDate,
                Contract = new Contract
                {
                    ContractId = dto.ContractId,
                    Status = dto.ContractStatus ?? ContractStatus.Draft,
                    Client = new Client
                    {
                        Name = dto.ClientName ?? string.Empty
                    }
                }
            };
        }

        private static Contract ToContract(ContractApiDto dto)
        {
            return new Contract
            {
                ContractId = dto.ContractId,
                ClientId = dto.ClientId,
                Client = new Client
                {
                    ClientId = dto.ClientId,
                    Name = dto.ClientName ?? string.Empty
                },
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                ContractType = dto.ContractType,
                SignedAgreementFilePath = dto.SignedAgreementFilePath
            };
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

        private class ServiceRequestApiDto
        {
            public int ServiceRequestId { get; set; }

            public int ContractId { get; set; }

            public string? ClientName { get; set; }

            public ContractStatus? ContractStatus { get; set; }

            public string Description { get; set; } = string.Empty;

            public decimal CostUSD { get; set; }

            public decimal CostZAR { get; set; }

            public ServiceRequestStatus Status { get; set; }

            public DateTime CreatedDate { get; set; }
        }

        private class ContractApiDto
        {
            public int ContractId { get; set; }

            public int ClientId { get; set; }

            public string? ClientName { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public ContractStatus Status { get; set; }

            public string ServiceLevel { get; set; } = string.Empty;

            public ContractType ContractType { get; set; }

            public string? SignedAgreementFilePath { get; set; }
        }

        private class ExchangeRateApiDto
        {
            public decimal Rate { get; set; }
        }
    }
}