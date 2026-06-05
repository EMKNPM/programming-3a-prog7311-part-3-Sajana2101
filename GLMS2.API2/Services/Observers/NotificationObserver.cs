namespace GLMS2.Services.Observers
{
    public class NotificationObserver : IGLMSObserver
    {
        public void Update(string status, int serviceRequestId)
        {
            Console.WriteLine(
                $"Notification: Service request {serviceRequestId} status changed to {status}.");
        }
    }
}