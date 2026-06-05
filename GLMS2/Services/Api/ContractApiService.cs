using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GLMS2.Services.Api
{
    public class ContractApiService : IContractApiService
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

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("GLMSApi");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?
                .Session
                .GetString("JwtToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        public async Task<IEnumerable<Contract>> FilterContractsAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status)
        {
            var queryParts = new List<string>();

            if (startDateFrom.HasValue)
            {
                queryParts.Add($"startDateFrom={Uri.EscapeDataString(startDateFrom.Value.ToString("yyyy-MM-dd"))}");
            }

            if (startDateTo.HasValue)
            {
                queryParts.Add($"startDateTo={Uri.EscapeDataString(startDateTo.Value.ToString("yyyy-MM-dd"))}");
            }

            if (status.HasValue)
            {
                queryParts.Add($"status={Uri.EscapeDataString(status.Value.ToString())}");
            }

            var url = "api/contracts";

            if (queryParts.Any())
            {
                url += "?" + string.Join("&", queryParts);
            }

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiContracts = await response.Content
                .ReadFromJsonAsync<List<ContractApiDto>>(JsonOptions);

            return apiContracts?.Select(ToContract).ToList()
                   ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var apiContract = await response.Content
                .ReadFromJsonAsync<ContractApiDto>(JsonOptions);

            return apiContract == null ? null : ToContract(apiContract);
        }

        public async Task<ContractEditViewModel?> GetContractForEditAsync(int id)
        {
            var contract = await GetContractByIdAsync(id);

            if (contract == null)
            {
                return null;
            }

            return new ContractEditViewModel
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                ContractType = contract.ContractType,
                ExistingSignedAgreementFilePath = contract.SignedAgreementFilePath
            };
        }

        public async Task CreateContractAsync(ContractCreateViewModel model)
        {
            using var form = CreateContractMultipartContent(model);
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsync("api/contracts", form);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }
        }

        public async Task<bool> UpdateContractAsync(ContractEditViewModel model)
        {
            using var form = CreateContractMultipartContent(model);
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsync(
                $"api/contracts/{model.ContractId}",
                form);

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

        public async Task<bool> DeleteContractAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");

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

        public async Task<FileDownloadViewModel?> DownloadAgreementAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}/agreement");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await ReadErrorMessageAsync(response));
            }

            var content = await response.Content.ReadAsByteArrayAsync();

            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                           ?? response.Content.Headers.ContentDisposition?.FileName
                           ?? $"contract-{id}-agreement.pdf";

            fileName = fileName.Trim('"');

            return new FileDownloadViewModel
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString()
                              ?? "application/pdf",
                FileName = fileName
            };
        }

        private static MultipartFormDataContent CreateContractMultipartContent(
            ContractCreateViewModel model)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(model.ClientId.ToString()), nameof(model.ClientId));
            form.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), nameof(model.StartDate));
            form.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), nameof(model.EndDate));
            form.Add(new StringContent(model.Status.ToString()), nameof(model.Status));
            form.Add(new StringContent(model.ServiceLevel ?? string.Empty), nameof(model.ServiceLevel));
            form.Add(new StringContent(model.ContractType.ToString()), nameof(model.ContractType));

            if (model.SignedAgreementFile != null && model.SignedAgreementFile.Length > 0)
            {
                var fileContent = new StreamContent(model.SignedAgreementFile.OpenReadStream());

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                    model.SignedAgreementFile.ContentType);

                form.Add(
                    fileContent,
                    nameof(model.SignedAgreementFile),
                    model.SignedAgreementFile.FileName);
            }

            return form;
        }

        private static MultipartFormDataContent CreateContractMultipartContent(
            ContractEditViewModel model)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(model.ContractId.ToString()), nameof(model.ContractId));
            form.Add(new StringContent(model.ClientId.ToString()), nameof(model.ClientId));
            form.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), nameof(model.StartDate));
            form.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), nameof(model.EndDate));
            form.Add(new StringContent(model.Status.ToString()), nameof(model.Status));
            form.Add(new StringContent(model.ServiceLevel ?? string.Empty), nameof(model.ServiceLevel));
            form.Add(new StringContent(model.ContractType.ToString()), nameof(model.ContractType));

            if (!string.IsNullOrWhiteSpace(model.ExistingSignedAgreementFilePath))
            {
                form.Add(
                    new StringContent(model.ExistingSignedAgreementFilePath),
                    nameof(model.ExistingSignedAgreementFilePath));
            }

            if (model.SignedAgreementFile != null && model.SignedAgreementFile.Length > 0)
            {
                var fileContent = new StreamContent(model.SignedAgreementFile.OpenReadStream());

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                    model.SignedAgreementFile.ContentType);

                form.Add(
                    fileContent,
                    nameof(model.SignedAgreementFile),
                    model.SignedAgreementFile.FileName);
            }

            return form;
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

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return $"API request failed with status code {(int)response.StatusCode}.";
            }

            return body;
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
    }
}