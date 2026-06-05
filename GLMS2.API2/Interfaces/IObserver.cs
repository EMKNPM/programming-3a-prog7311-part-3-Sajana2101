namespace GLMS2.Interfaces
{
    // Defines objects that receive updates from a subject
    // Used to react when important changes occur in the system
    public interface IObserver
    {
        // Called when the subject sends a notification
        void Update(ISubject subject);
    }
}