namespace GLMS2.Services.Observers
{
    public class AuditObserver : IGLMSObserver
    {
        public void Update(string status, int serviceRequestId)
        {
            Console.WriteLine(
                $"Audit Log: Service request {serviceRequestId} status changed to {status}.");
        }
    }
}