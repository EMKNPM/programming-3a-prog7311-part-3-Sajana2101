using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Represents the contract component that communicates through the mediator
    // Sends notifications when contract-related actions occur
    public class ContractModule
    {
        private readonly IMediator _mediator;

        public ContractModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Notifies the mediator when a contract is approved
        // Allows other components to respond without direct coupling
        public void ApproveContract()
        {
            _mediator.Notify(this, "ContractApproved");
        }
    }
}