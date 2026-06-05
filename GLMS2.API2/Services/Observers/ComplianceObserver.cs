using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    // Observer that checks contract status for compliance rules
    // Reacts when the subject notifies observers of changes
    public class ComplianceObserver : IObserver
    {
        // Called when the subject status changes
        public void Update(ISubject subject)
        {
            CheckCompliance(subject);
        }

        // Checks if the contract status meets compliance conditions
        private void CheckCompliance(ISubject subject)
        {
            var status = subject.GetStatus();

            // Example compliance rule when contract has expired
            if (status.Equals("Expired", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Compliance check: Contract is expired.");
            }
        }
    }
}