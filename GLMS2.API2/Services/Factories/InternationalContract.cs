using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    // Represents an international contract implementation
    // Uses the shared contract structure with behaviour specific to international agreements
    public class InternationalContract : IContract
    {
        private readonly Contract _contract;

        public InternationalContract(Contract contract)
        {
            _contract = contract;
        }

        // Returns basic information about the international contract
        public string GetDetails()
        {
            return $"International contract for Client ID {_contract.ClientId} from {_contract.StartDate:d} to {_contract.EndDate:d}.";
        }

        // Validates that the contract dates are logically correct
        public bool Validate()
        {
            return _contract.StartDate < _contract.EndDate;
        }
    }
}