using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    // Represents a domestic contract implementation
    // Uses the shared contract structure but applies its own behaviour
    public class DomesticContract : IContract
    {
        private readonly Contract _contract;

        public DomesticContract(Contract contract)
        {
            _contract = contract;
        }

        // Returns basic information about the domestic contract
        public string GetDetails()
        {
            return $"Domestic contract for Client ID {_contract.ClientId} from {_contract.StartDate:d} to {_contract.EndDate:d}.";
        }

        // Validates that the contract dates are correct
        public bool Validate()
        {
            return _contract.StartDate < _contract.EndDate;
        }
    }
}