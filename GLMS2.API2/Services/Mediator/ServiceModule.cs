using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Represents the service request component in the mediator structure
    // Sends a message when a new service request is created
    public class ServiceModule
    {
        private readonly IMediator _mediator;

        public ServiceModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Notifies the mediator when a service request is created
        // Allows other modules to respond without direct connections
        public void CreateServiceRequest()
        {
            _mediator.Notify(this, "ServiceRequestCreated");
        }
    }
}