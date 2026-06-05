using GLMS2.Models;
using GLMS2.ViewModels;

namespace GLMS2.Interfaces
{
    public interface IServiceRequestApiService
    {
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();

        Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);

        Task<ServiceRequestEditViewModel?> GetServiceRequestForEditAsync(int id);

        Task<ServiceRequest?> CreateServiceRequestAsync(
            ServiceRequestCreateViewModel model);

        Task<bool> UpdateServiceRequestAsync(ServiceRequestEditViewModel model);

        Task<bool> DeleteServiceRequestAsync(int id);

        Task<decimal> GetUsdToZarRateAsync();

        Task<IEnumerable<Contract>> GetActiveContractsAsync();
    }
}