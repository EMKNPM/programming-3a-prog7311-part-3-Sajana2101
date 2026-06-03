using GLMS2.Enums;
using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    // Creates the correct contract object based on the selected type
    // Centralises contract creation logic in one place
    public class ContractFactory : IContractFactory
    {
        // Returns the appropriate contract implementation
        public IContract CreateContract(ContractType contractType, Contract contract)
        {
            return contractType switch
            {
                // Creates a contract for local services
                ContractType.Domestic => new DomesticContract(contract),

                // Creates a contract for cross-border services
                ContractType.International => new InternationalContract(contract),

                // Creates a contract with premium features
                ContractType.Premium => new PremiumContract(contract),

                // Handles invalid contract types
                _ => throw new ArgumentException("Invalid contract type.")
            };
        }
    }
}