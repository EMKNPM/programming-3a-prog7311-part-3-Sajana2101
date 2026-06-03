namespace GLMS2.Enums
{ // Defines the workflow stages for a contract
    // Used to control how contracts behave in the system and what actions are allowed
    public enum ContractStatus
    {
        // Contract has been created but is not yet active
        // Allows editing before the agreement is finalised
        Draft = 0,
        // Contract is valid and operational
        // Only Active contracts can have Service Requests created
        Active = 1,
        // Contract is valid and operational
        // Only Active contracts can have Service Requests created
        Expired = 2,
        // Contract temporarily paused or unavailable
        // Restricts workflow actions until status changes
        OnHold = 3
    }
}
