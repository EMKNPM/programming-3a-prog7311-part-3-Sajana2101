using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Represents the notification component in the mediator structure
    // Sends a message when a system notification is triggered
    public class NotificationModule
    {
        private readonly IMediator _mediator;

        public NotificationModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Notifies the mediator that a notification has been sent
        // Allows other modules to react if needed
        public void SendNotification()
        {
            _mediator.Notify(this, "NotificationSent");
        }
    }
}