using twtTest.Data;

namespace twtTest.Interfaces
{
    public interface IClaimsProcesor
    {
        Task<List<ClaimAccumulation>> ProcessAccumulatedClaims(InsuranceClaims claims);
    }
}
