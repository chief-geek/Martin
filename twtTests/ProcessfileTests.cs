using Microsoft.Extensions.Logging;
using Moq;
using twtTest.ConcreteImplementations;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTests
{
    public class ProcessfileTests
    {
        [Fact]
        public async Task File_But_Does_Not_Produce_Correct_Result()
        {

            List<InsuranceClaim> expectedResult = new List<InsuranceClaim>
            { new InsuranceClaim {
                Product = "Comp",
                OriginYear = 1991,
                DevelopmentYear = 1992,
                IncrementalValue = 50
            }
            };

            List<InsuranceClaim> mappedResult = new List<InsuranceClaim>
            { new InsuranceClaim {
                Product = "Comp",
                OriginYear = 1991,
                DevelopmentYear = 1992,
                IncrementalValue = 100
            } };

            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(x => x.ConvertFileLinesToInsuranceClaims(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult));

            var mockFieldMapper = new Mock<IFieldMapper>();
            mockFieldMapper.Setup(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(mappedResult.First()));

            var mockClaimsProcessor = new Mock<IClaimsProcesor>();
            mockClaimsProcessor.Setup(x => x.ProcessAccumulatedClaims(It.IsAny<InsuranceClaims>()))
                .Returns(() => Task.FromResult(new List<ClaimAccumulation>()));

            var mockLogger = new Mock<ILogger<FileProcessor>>();

            var processor = new FileProcessor(mockLogger.Object, mockFieldMapper.Object, mockClaimsProcessor.Object);
            List<InsuranceClaim> result = await processor.ConvertFileLinesToInsuranceClaims("Comp, 1991, 1992, 100", 1);

            Assert.NotEqual(expectedResult, result);
            mockFieldMapper.Verify(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task File_But_Does_Not_Have_Valid_Data_Incorrect_Result()
        {
            List<InsuranceClaim> expectedResult = new List<InsuranceClaim>
            {
                new InsuranceClaim {
                Product = "Comp",
                OriginYear = 1991,
                DevelopmentYear = 1992,
                IncrementalValue = null
            } };

            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(x => x.ConvertFileLinesToInsuranceClaims(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult));

            var mockFieldMapper = new Mock<IFieldMapper>();
            mockFieldMapper.Setup(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult.First()));

            var mockClaimsProcessor = new Mock<IClaimsProcesor>();
            mockClaimsProcessor.Setup(x => x.ProcessAccumulatedClaims(It.IsAny<InsuranceClaims>()))
                .Returns(() => Task.FromResult(new List<ClaimAccumulation>()));

            var mockLogger = new Mock<ILogger<FileProcessor>>();

            var processor = new FileProcessor(mockLogger.Object, mockFieldMapper.Object, mockClaimsProcessor.Object);
            List<InsuranceClaim> result = await processor.ConvertFileLinesToInsuranceClaims("Comp, 1991, 1992, xjy", 1);
            mockFieldMapper.Verify(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Process_File_Passess()
        {

            List<InsuranceClaim> expectedResult = new List<InsuranceClaim>
            { new InsuranceClaim
            {
                Product = "Comp",
                OriginYear = 1992,
                DevelopmentYear = 1992,
                IncrementalValue = 110
            } };

            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(x => x.ConvertFileLinesToInsuranceClaims(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult));

            var mockFieldMapper = new Mock<IFieldMapper>();
            mockFieldMapper.Setup(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult.First()));

            var mockClaimsProcessor = new Mock<IClaimsProcesor>();
            mockClaimsProcessor.Setup(x => x.ProcessAccumulatedClaims(It.IsAny<InsuranceClaims>()))
                .Returns(() => Task.FromResult(new List<ClaimAccumulation>()));

            var mockLogger = new Mock<ILogger<FileProcessor>>();

            var processor = new FileProcessor(mockLogger.Object, mockFieldMapper.Object, mockClaimsProcessor.Object);
            List<InsuranceClaim> result = await processor.ConvertFileLinesToInsuranceClaims("Comp, 1992, 1992, 110", 1);

            Assert.Equal(expectedResult, result);
            mockFieldMapper.Verify(x => x.TryMapFields(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
