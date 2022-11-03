
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using twtTest.ConcreteImplementations;
using twtTest.ConfigItems;
using twtTest.Data;
using twtTest.Interfaces;
using twtTests.Properties;

namespace twtTests
{
    public class FileReaderTests
    {
        [Fact]
        public async Task TestRead_Fails()
        {

            var optionsValues = new FileLocations() { PathToInputFile = "", PathToOutputFile = "" };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(optionsValues);

            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(x => x.TryReadFile())
                .Returns(() => Task.FromResult(new List<ClaimAccumulation>()));

            var mockFileProcessor = new Mock<IFileProcessor>();
            mockFileProcessor.Setup(x => x.ConvertFileLinesToInsuranceClaims(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(new List<InsuranceClaim>()));

            var mockLogger = new Mock<ILogger<FileReader>>();

            var expectedResult = new List<ClaimAccumulation>() { new ClaimAccumulation { Product = "Comp", AccumulatedValue = 1100 } };
            var fileReader = new FileReader(mockFileProcessor.Object, mockOptions.Object, mockLogger.Object);
            var fileReaderReaderResult = await fileReader.TryReadFile();

            Assert.NotEqual(expectedResult, fileReaderReaderResult);
        }

        [Fact]
        public async Task TestRead_Passes()
        {

            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "goodfile.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);

            List<InsuranceClaim> expectedResult = new List<InsuranceClaim>
            {
                new InsuranceClaim {
                Product = "Comp",
                OriginYear = 1991,
                DevelopmentYear = 1992,
                IncrementalValue = 100
            } };

            var fileReader = new Mock<IFileReader>();
            fileReader.Setup(x => x.TryReadFile());

            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(x => x.ConvertFileLinesToInsuranceClaims(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(expectedResult));

            var mockLogger = new Mock<ILogger<FileReader>>();

            var reader = new FileReader(fileProcessor.Object, mockOptions.Object, mockLogger.Object);
            var result = await reader.TryReadFile();

            fileProcessor.Verify(x => x.AccumulateInsuranceClaims(It.IsAny<InsuranceClaims>()), Times.Once);
        }
    }
}
