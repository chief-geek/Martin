using Microsoft.Extensions.Logging;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTest.ConcreteImplementations
{
    public class ClaimsProcessor : IClaimsProcesor
    {
        private readonly ILogger<ClaimsProcessor> _logger;

        public ClaimsProcessor(ILogger<ClaimsProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<List<ClaimAccumulation>> ProcessAccumulatedClaims(InsuranceClaims claims)
        {
            try
            {
                var accumulatedClaims = new List<ClaimAccumulation>();
                _logger.LogInformation($"Accumulating claims results...");

                accumulatedClaims!.Add(new ClaimAccumulation
                {
                    Product = claims.ValidInsuranceClaims[0].Product,
                    AccumulatedValue = claims.ValidInsuranceClaims[0].IncrementalValue
                });

                for (int i = 1; i < claims.ValidInsuranceClaims.Count; i++)
                {
                    var previousClaim = claims.ValidInsuranceClaims[i - 1];

                    if (claims.ValidInsuranceClaims[i].Product != previousClaim.Product)
                    {
                        _logger.LogInformation("New product found [0], creating new line for output", claims.ValidInsuranceClaims[i].Product);
                        previousClaim = new InsuranceClaim() { DevelopmentYear = 0, OriginYear = 0, Product = "", IncrementalValue = 0 };
                    }

                    var newClaimsResult = await CalculateAccumulation(claims.ValidInsuranceClaims[i], previousClaim, accumulatedClaims.Last());
                    if (newClaimsResult != null)
                    {
                        accumulatedClaims.AddRange(newClaimsResult);
                    }
                }

                _logger.LogInformation("Processed claims [0] successfully!", claims.ValidInsuranceClaims.Count.ToString());
                return accumulatedClaims;
            }
            catch (Exception ex)
            {

                _logger.LogCritical("A breaking exception has occurred whilst trying to process the list of claims {ex.Message} with inner exception {ex.Message}\n Please inspect your input file for validity of data!", ex.Message);
                throw new ClaimsException($"A breaking exception has occurred whilst trying to process the list of claims {ex.Message}\n Please inspect your input file for validity of data!");
            }
        }

        private static Task<List<ClaimAccumulation>> CalculateAccumulation(InsuranceClaim claim, InsuranceClaim previousClaim, ClaimAccumulation lastClaimResult)
        {

            var origin = (claim.OriginYear - previousClaim.OriginYear == 0);
            origin = origin || (previousClaim.OriginYear == 0);
            var development = (claim.DevelopmentYear == previousClaim.DevelopmentYear);
            development = development || (origin && claim.DevelopmentYear == previousClaim.OriginYear + 1);

            var result = new List<ClaimAccumulation> { new ClaimAccumulation { Product = claim.Product, AccumulatedValue = claim.IncrementalValue } };

            if (!origin)
            {
                return Task.FromResult(result);
            }
            else if (origin && development)
            {
                result.FirstOrDefault()!.AccumulatedValue += previousClaim.IncrementalValue;
                return Task.FromResult(result);
            }
            else if (origin && (previousClaim.OriginYear > 0 && previousClaim.DevelopmentYear < claim.DevelopmentYear))
            {
                result = new List<ClaimAccumulation>
                {
                    new ClaimAccumulation
                    {
                        Product = claim.Product,
                        AccumulatedValue = lastClaimResult.AccumulatedValue
                    },
                    new ClaimAccumulation
                    {
                        Product = claim.Product,
                        AccumulatedValue = claim.IncrementalValue+lastClaimResult.AccumulatedValue
                    }
                };
                return Task.FromResult(result);
            }
            return Task.FromResult(result);

        }
    }
}
