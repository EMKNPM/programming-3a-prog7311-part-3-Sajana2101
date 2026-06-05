using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Represents the client component that communicates through the mediator
    // Sends notifications when a client-related action occurs
    public class ClientModule
    {
        private readonly IMediator _mediator;

        public ClientModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Notifies the mediator when a new client is registered
        // Allows other parts of the system to react without direct dependency
        public void RegisterClient()
        {
            _mediator.Notify(this, "ClientRegistered");
        }
    }
}