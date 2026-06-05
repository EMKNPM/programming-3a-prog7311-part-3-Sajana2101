using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Central mediator that coordinates communication between modules
    // Prevents direct dependencies between different components
    public class GLMSMediator : IMediator
    {
        // Handles events triggered by different parts of the system
        public void Notify(object sender, string eventType)
        {
            switch (eventType)
            {
                // Triggered when a contract is approved
                case "ContractApproved":
                    Console.WriteLine("Mediator: handling contract approval workflow.");
                    break;

                // Triggered when a service request is created
                case "ServiceRequestCreated":
                    Console.WriteLine("Mediator: handling service request workflow.");
                    break;

                // Triggered when a new client is registered
                case "ClientRegistered":
                    Console.WriteLine("Mediator: handling client registration workflow.");
                    break;

                // Triggered when currency conversion occurs
                case "CurrencyConverted":
                    Console.WriteLine("Mediator: handling currency conversion workflow.");
                    break;

                // Triggered when a notification is completed
                case "NotificationSent":
                    Console.WriteLine("Mediator: notification process completed.");
                    break;
            }
        }
    }
}