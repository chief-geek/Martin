using Microsoft.Extensions.Logging;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTest.ConcreteImplementations
{
    public class FieldMapper : IFieldMapper
    {
        private readonly ILogger<FieldMapper> _logger;

        public FieldMapper(ILogger<FieldMapper> logger)
        {
            _logger = logger;
        }

        public Task<InsuranceClaim> TryMapFields(string claimresult, int lineNumber)
        {
            var claim = new InsuranceClaim();
            try
            {
                _logger.LogInformation("Attempting to Map {claimresult}", claimresult);

                var line = claimresult.Replace(" ", "").Split(",").ToList();
                if (line!.Count != BaseData.ColumnCount)
                {
                    _logger.LogError("Unable to map Line {lineNumber} as the data within the line is invalid: {claimresult}", lineNumber, claimresult);
                    return Task.FromResult(new InsuranceClaim());
                }
                claim.Product = line[0];
                claim.OriginYear = Convert.ToInt16(line[1]);
                claim.DevelopmentYear = Convert.ToInt16(line[2]);
                claim.IncrementalValue = Convert.ToDouble(line[3]);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to map Line {lineNumber},  {ex.Message}, line will be ignored.", lineNumber, ex.Message);
            }
            return Task.FromResult(claim);
        }
    }
}
