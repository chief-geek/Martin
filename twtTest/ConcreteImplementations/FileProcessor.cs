using Microsoft.Extensions.Logging;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTest.ConcreteImplementations
{
    public class FileProcessor : IFileProcessor
    {
        private readonly ILogger _logger;
        private readonly IFieldMapper _fieldMapper;
        private readonly IClaimsProcesor _claimsProcessor;

        public FileProcessor(ILogger<FileProcessor> logger, IFieldMapper fieldMapper, IClaimsProcesor claimsProcessor)
        {
            _logger = logger;
            _fieldMapper = fieldMapper;
            _claimsProcessor = claimsProcessor;
        }

        public async Task<List<InsuranceClaim>> ConvertFileLinesToInsuranceClaims(string claimresult, int lineNumber)
        {
            var listOfClaims = new List<InsuranceClaim>();
            try
            {
                _logger.LogInformation("Processing line {lineNumber} {claimresult}", lineNumber, claimresult);

                if (!IsValidLine(claimresult))
                {
                    _logger.LogError("Line {lineNumber} contains too many columns. This line will not be included in the results!\n{claimresult}", lineNumber, claimresult);
                }

                var newClaim = await _fieldMapper.TryMapFields(claimresult, lineNumber);
                if (newClaim.IncrementalValue != null) { listOfClaims.Add(newClaim); }
                lineNumber++;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("The following exception occurred whilst processing the line {linenumber}\n {ex.Message}\nThis claim will be ignored!", lineNumber, ex.Message);
            }
            return listOfClaims;
        }

        public async Task<List<ClaimAccumulation>> AccumulateInsuranceClaims(InsuranceClaims claims)
        {
            return await _claimsProcessor.ProcessAccumulatedClaims(claims);
        }

        private static bool IsValidLine(string? claimresult)
        {
            if (claimresult == null) { return false; } 
            return claimresult.Split(",").Length == BaseData.ColumnCount;
        }
    }
}
