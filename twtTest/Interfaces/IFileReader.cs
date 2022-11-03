using twtTest.Data;

namespace twtTest.Interfaces
{
    public interface IFileReader
    {
        Task<List<ClaimAccumulation>> TryReadFile();
    }
}