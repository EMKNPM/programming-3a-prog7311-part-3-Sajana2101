using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    // Represents a premium contract implementation
    // Uses the shared contract structure with behaviour for high-priority agreements
    public class PremiumContract : IContract
    {
        private readonly Contract _contract;

        public PremiumContract(Contract contract)
        {
            _contract = contract;
        }

        // Returns basic information about the premium contract
        public string GetDetails()
        {
            return $"Premium contract for Client ID {_contract.ClientId} from {_contract.StartDate:d} to {_contract.EndDate:d}.";
        }

        // Validates that the contract dates are correct
        public bool Validate()
        {
            return _contract.StartDate < _contract.EndDate;
        }
    }
}