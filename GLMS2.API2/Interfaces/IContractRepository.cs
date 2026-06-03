using GLMS2.Enums;
using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();

        Task<Contract?> GetByIdAsync(int id);

        Task<Contract?> GetForUpdateAsync(int id);

        Task<IEnumerable<Contract>> FilterAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status);

        Task AddAsync(Contract contract);

        void Update(Contract contract);

        void Remove(Contract contract);

        Task SaveChangesAsync();
    }
}