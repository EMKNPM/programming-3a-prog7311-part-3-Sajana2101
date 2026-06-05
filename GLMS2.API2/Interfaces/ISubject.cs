namespace GLMS2.Interfaces
{
    // Represents an object that can notify observers when its state changes
    // Allows multiple components to stay updated automatically
    public interface ISubject
    {
        // Adds an observer to be notified of changes
        void Attach(IObserver observer);

        // Removes an observer
        void Detach(IObserver observer);

        // Sends updates to all attached observers
        void Notify();

        // Returns the current status of the subject
        string GetStatus();
    }
}