namespace GLMS2.Interfaces
{
    // Defines communication between different components
    // Helps coordinate actions without creating tight dependencies
    public interface IMediator
    {
        // Sends notifications between objects when an event occurs
        void Notify(object sender, string eventType);
    }
}