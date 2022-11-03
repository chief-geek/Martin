using twtTest.Data;

namespace twtTest.Interfaces
{
    public interface IFieldMapper
    {
        Task<InsuranceClaim> TryMapFields(string claimresult, int lineNumber);
    }
}