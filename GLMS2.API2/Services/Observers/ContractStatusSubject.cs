using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    // Subject that keeps track of contract status changes
    // Notifies observers when the status is updated
    public class ContractStatusSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();

        // Identifies the contract linked to the status change
        public int ContractId { get; set; }

        // Stores the current workflow status
        public string CurrentStatus { get; private set; } = string.Empty;

        // Records when the status was last updated
        public DateTime UpdatedAt { get; private set; }

        // Adds an observer to receive updates
        public void Attach(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        // Removes an observer from receiving updates
        public void Detach(IObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        // Notifies all observers when the status changes
        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        // Returns the current contract status
        public string GetStatus()
        {
            return CurrentStatus;
        }

        // Updates the contract status and triggers notifications
        public void SetStatus(string status)
        {
            CurrentStatus = status;
            UpdatedAt = DateTime.UtcNow;
            Notify();
        }
    }
}