namespace GLMS2.Interfaces
{
    public interface IContract
    {
        // Returns basic contract information
        string GetDetails();
        // Ensures contract data meets required rules
        bool Validate();
    }
}
