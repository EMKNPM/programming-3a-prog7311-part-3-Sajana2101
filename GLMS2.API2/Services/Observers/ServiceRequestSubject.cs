namespace GLMS2.Services.Observers
{
    public class ServiceRequestSubject
    {
        private readonly List<IGLMSObserver> _observers = new();

        public int ServiceRequestId { get; set; }

        public void Attach(IGLMSObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IGLMSObserver observer)
        {
            _observers.Remove(observer);
        }

        public void SetStatus(string status)
        {
            Notify(status);
        }

        private void Notify(string status)
        {
            foreach (var observer in _observers)
            {
                observer.Update(status, ServiceRequestId);
            }
        }
    }
}