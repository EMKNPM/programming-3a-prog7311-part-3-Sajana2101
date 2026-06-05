using GLMS2.ViewModels;
using GLMS2.Models;

namespace GLMS2.Interfaces
{
    // Handles business logic related to service requests
    // Keeps controllers separate from data access logic
    public interface IServiceRequestService
    {
        // Creates a new service request linked to a contract
        Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequestCreateViewModel model);

        // Retrieves all service requests
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();

        // Gets a specific service request by ID
        Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);

        // Checks whether a service request can be created for a contract
        Task<bool> CanCreateServiceRequestAsync(int contractId);

        // Deletes a service request
        Task<bool> DeleteServiceRequestAsync(int id);

        // Retrieves service request data for editing
        Task<ServiceRequestEditViewModel?> GetServiceRequestForEditAsync(int id);

        // Updates an existing service request
        Task<bool> UpdateServiceRequestAsync(ServiceRequestEditViewModel model);
    }
}