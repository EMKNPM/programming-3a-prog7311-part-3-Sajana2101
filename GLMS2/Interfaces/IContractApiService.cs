using GLMS2.Enums;
using GLMS2.Models;
using GLMS2.ViewModels;

namespace GLMS2.Interfaces
{
    public interface IContractApiService
    {
        Task<IEnumerable<Contract>> FilterContractsAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status);

        Task<Contract?> GetContractByIdAsync(int id);

        Task<ContractEditViewModel?> GetContractForEditAsync(int id);

        Task CreateContractAsync(ContractCreateViewModel model);

        Task<bool> UpdateContractAsync(ContractEditViewModel model);

        Task<bool> DeleteContractAsync(int id);

        Task<FileDownloadViewModel?> DownloadAgreementAsync(int id);
    }
}