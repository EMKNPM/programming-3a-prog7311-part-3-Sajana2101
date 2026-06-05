using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();

        Task<ServiceRequest?> GetByIdAsync(int id);

        Task<ServiceRequest?> GetForUpdateAsync(int id);

        Task AddAsync(ServiceRequest serviceRequest);

        void Update(ServiceRequest serviceRequest);

        void Remove(ServiceRequest serviceRequest);

        Task SaveChangesAsync();
    }
}