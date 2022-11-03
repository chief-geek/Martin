using twtTest.Data;

namespace twtTest.Interfaces
{
    public interface IFileProcessor
    {
        Task<List<InsuranceClaim>> ConvertFileLinesToInsuranceClaims(string fileLine, int lineNumber);
        Task<List<ClaimAccumulation>> AccumulateInsuranceClaims(InsuranceClaims claims);
    }
}