using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.Services.Observers;
using GLMS2.ViewModels;

namespace GLMS2.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IMediator _mediator;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            ICurrencyService currencyService,
            IMediator mediator)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _currencyService = currencyService;
            _mediator = mediator;
        }

        public async Task<ServiceRequest> CreateServiceRequestAsync(
            ServiceRequestCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var contract = await _contractRepository.GetByIdAsync(model.ContractId);

            if (contract == null)
            {
                throw new InvalidOperationException("Selected contract does not exist.");
            }

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                throw new InvalidOperationException(
                    "A service request cannot be created for an expired or on-hold contract.");
            }

            if (contract.Status != ContractStatus.Active)
            {
                throw new InvalidOperationException(
                    "A service request can only be created for an active contract.");
            }

            if (model.CostUSD <= 0)
            {
                throw new InvalidOperationException("USD cost must be greater than 0.");
            }

            var rate = await _currencyService.GetUsdToZarRateAsync();
            var localCost = _currencyService.ConvertUsdToZar(model.CostUSD, rate);

            var serviceRequest = new ServiceRequest
            {
                ContractId = model.ContractId,
                Description = model.Description,
                CostUSD = model.CostUSD,
                CostZAR = localCost,
                Status = ServiceRequestStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            var subject = new ServiceRequestSubject
            {
                ServiceRequestId = serviceRequest.ServiceRequestId
            };

            subject.Attach(new NotificationObserver());
            subject.Attach(new AuditObserver());
            subject.SetStatus(serviceRequest.Status.ToString());

            _mediator.Notify(this, "ServiceRequestCreated");

            return serviceRequest;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            return await _serviceRequestRepository.GetAllAsync();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            return await _serviceRequestRepository.GetByIdAsync(id);
        }

        public async Task<bool> CanCreateServiceRequestAsync(int contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);

            return contract != null && contract.Status == ContractStatus.Active;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetForUpdateAsync(id);

            if (serviceRequest == null)
            {
                return false;
            }

            _serviceRequestRepository.Remove(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return true;
        }

        public async Task<ServiceRequestEditViewModel?> GetServiceRequestForEditAsync(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetForUpdateAsync(id);

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

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequestEditViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var serviceRequest = await _serviceRequestRepository
                .GetForUpdateAsync(model.ServiceRequestId);

            if (serviceRequest == null)
            {
                return false;
            }

            var contract = await _contractRepository.GetByIdAsync(model.ContractId);

            if (contract == null)
            {
                throw new InvalidOperationException("Selected contract does not exist.");
            }

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                throw new InvalidOperationException(
                    "A service request cannot be assigned to an expired or on-hold contract.");
            }

            if (contract.Status != ContractStatus.Active)
            {
                throw new InvalidOperationException(
                    "A service request can only be assigned to an active contract.");
            }

            if (model.CostUSD <= 0)
            {
                throw new InvalidOperationException("USD cost must be greater than 0.");
            }

            var rate = await _currencyService.GetUsdToZarRateAsync();
            var localCost = _currencyService.ConvertUsdToZar(model.CostUSD, rate);

            serviceRequest.ContractId = model.ContractId;
            serviceRequest.Description = model.Description;
            serviceRequest.CostUSD = model.CostUSD;
            serviceRequest.CostZAR = localCost;

            _serviceRequestRepository.Update(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            var subject = new ServiceRequestSubject
            {
                ServiceRequestId = serviceRequest.ServiceRequestId
            };

            subject.Attach(new NotificationObserver());
            subject.Attach(new AuditObserver());
            subject.SetStatus(serviceRequest.Status.ToString());

            _mediator.Notify(this, "ServiceRequestCreated");

            return true;
        }
    }
}