using GLMS2.Enums;
using GLMS2.Models;
using GLMS2.ViewModels;

namespace GLMS2.Interfaces
{
    public interface IContractService
    {
        Task<Contract> CreateContractAsync(
            ContractCreateViewModel model,
            string webRootPath);

        Task<IEnumerable<Contract>> GetAllContractsAsync();

        Task<Contract?> GetContractByIdAsync(int id);

        Task<IEnumerable<Contract>> FilterContractsAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status);

        Task<bool> DeleteContractAsync(int id);

        Task<ContractEditViewModel?> GetContractForEditAsync(int id);

        Task<bool> UpdateContractAsync(
            ContractEditViewModel model,
            string webRootPath);

        Task<Contract?> UpdateStatusAsync(int id, ContractStatus status);
    }
}