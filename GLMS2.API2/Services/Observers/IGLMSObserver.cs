namespace GLMS2.Services.Observers
{
    public interface IGLMSObserver
    {
        void Update(string status, int serviceRequestId);
    }
}