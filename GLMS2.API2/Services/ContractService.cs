using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;

namespace GLMS2.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IFileService _fileService;
        private readonly IContractFactory _contractFactory;

        public ContractService(
            IContractRepository contractRepository,
            IFileService fileService,
            IContractFactory contractFactory)
        {
            _contractRepository = contractRepository;
            _fileService = fileService;
            _contractFactory = contractFactory;
        }

        public async Task<Contract> CreateContractAsync(
            ContractCreateViewModel model,
            string webRootPath)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new InvalidOperationException(
                    "Start date must be before end date.");
            }

            if (model.SignedAgreementFile == null)
            {
                throw new InvalidOperationException(
                    "A signed agreement PDF is required.");
            }

            var savedPath = await _fileService.SavePdfAsync(
                model.SignedAgreementFile,
                webRootPath);

            var contract = new Contract
            {
                ClientId = model.ClientId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ServiceLevel = model.ServiceLevel,
                ContractType = model.ContractType,
                SignedAgreementFilePath = savedPath
            };

            var contractPatternObject = _contractFactory.CreateContract(
                model.ContractType,
                contract);

            if (!contractPatternObject.Validate())
            {
                throw new InvalidOperationException(
                    "Contract validation failed.");
            }

            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveChangesAsync();

            return contract;
        }

        public async Task<IEnumerable<Contract>> GetAllContractsAsync()
        {
            return await _contractRepository.GetAllAsync();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            return await _contractRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Contract>> FilterContractsAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status)
        {
            return await _contractRepository.FilterAsync(
                startDateFrom,
                startDateTo,
                status);
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _contractRepository.GetForUpdateAsync(id);

            if (contract == null)
            {
                return false;
            }

            _contractRepository.Remove(contract);
            await _contractRepository.SaveChangesAsync();

            return true;
        }

        public async Task<ContractEditViewModel?> GetContractForEditAsync(int id)
        {
            var contract = await _contractRepository.GetForUpdateAsync(id);

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
                ExistingSignedAgreementFilePath =
                    contract.SignedAgreementFilePath
            };
        }

        public async Task<bool> UpdateContractAsync(
            ContractEditViewModel model,
            string webRootPath)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var contract = await _contractRepository
                .GetForUpdateAsync(model.ContractId);

            if (contract == null)
            {
                return false;
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new InvalidOperationException(
                    "Start date must be before end date.");
            }

            contract.ClientId = model.ClientId;
            contract.StartDate = model.StartDate;
            contract.EndDate = model.EndDate;
            contract.Status = model.Status;
            contract.ServiceLevel = model.ServiceLevel;
            contract.ContractType = model.ContractType;

            if (model.SignedAgreementFile != null)
            {
                var savedPath = await _fileService.SavePdfAsync(
                    model.SignedAgreementFile,
                    webRootPath);

                contract.SignedAgreementFilePath = savedPath;
            }

            var contractPatternObject = _contractFactory.CreateContract(
                model.ContractType,
                contract);

            if (!contractPatternObject.Validate())
            {
                throw new InvalidOperationException(
                    "Contract validation failed.");
            }

            _contractRepository.Update(contract);
            await _contractRepository.SaveChangesAsync();

            return true;
        }

        public async Task<Contract?> UpdateStatusAsync(
            int id,
            ContractStatus status)
        {
            var contract = await _contractRepository.GetForUpdateAsync(id);

            if (contract == null)
            {
                return null;
            }

            contract.Status = status;

            _contractRepository.Update(contract);
            await _contractRepository.SaveChangesAsync();

            return await _contractRepository.GetByIdAsync(id);
        }
    }
}