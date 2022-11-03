using Microsoft.Extensions.Logging;
using Moq;
using twtTest.ConcreteImplementations;
using twtTest.Data;

namespace twtTests
{
    public class ClaimsTests
    {
        [Fact]
        public async Task Claim_Is_Not_Valid()
        {

            InsuranceClaims claims = new InsuranceClaims();
            claims.ValidInsuranceClaims = new List<InsuranceClaim>
            {
                new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1991,
                    DevelopmentYear = 1992,
                    IncrementalValue = 91
                }
            };

            var mockLogger = new Mock<ILogger<ClaimsProcessor>>();
            var claimsProcessor = new ClaimsProcessor(mockLogger.Object);
            var result = await claimsProcessor.ProcessAccumulatedClaims(claims);

            Assert.NotEqual(55, result[0].AccumulatedValue);
        }

        [Fact]
        public async Task Multiple_Claims_Passes()
        {


            InsuranceClaims claims = new InsuranceClaims();
            claims.ValidInsuranceClaims = new List<InsuranceClaim>

            {
                new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1991,
                    DevelopmentYear = 1992,
                    IncrementalValue = 50
                },
                new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1991,
                    DevelopmentYear = 1992,
                    IncrementalValue = 65
                },
                new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1991,
                    DevelopmentYear = 1993,
                    IncrementalValue = 80
                },new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1992,
                    DevelopmentYear = 1992,
                    IncrementalValue = 50
                },new InsuranceClaim{
                    Product = "Comp",
                    OriginYear = 1991,
                    DevelopmentYear = 1992,
                    IncrementalValue = 25
                }
            };


            List<ClaimAccumulation> expectedResult = new List<ClaimAccumulation>();
            expectedResult = new List<ClaimAccumulation>
            {
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 50
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 115
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 115
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 80
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 195
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 50
                },
                new ClaimAccumulation
                {
                    Product = "Comp",
                    AccumulatedValue = 25
                }
            };

            var mockLogger = new Mock<ILogger<ClaimsProcessor>>();
            var claimsProcessor = new ClaimsProcessor(mockLogger.Object);
            var result = await claimsProcessor.ProcessAccumulatedClaims(claims);

            Assert.Equal(6, result.Count());
            Assert.Equal(115, result[2].AccumulatedValue);

        }
    }
}
