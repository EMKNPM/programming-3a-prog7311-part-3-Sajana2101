using GLMS2.Enums;
using GLMS2.Models;

namespace GLMS2.Interfaces
{
    // Factory interface used to create different types of contracts
    public interface IContractFactory
    {
        // Creates the correct contract type based on the selected enum value
        IContract CreateContract(ContractType contractType, Contract contract);
    }
}