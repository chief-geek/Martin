using twtTest.Data;

namespace twtTest.Interfaces
{
    public interface IFileWriter
    {
        Task ConvertAndWriteToCSV(List<ClaimAccumulation> claims);
    }
}
