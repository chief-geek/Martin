using Microsoft.Extensions.Options;
using Moq;
using twtTest.ConfigItems;
using twtTests.Properties;

namespace twtTests
{
    //these tests are ONLY for checking the validity of a CSV file and nothing more. It does take into consideration the expected structure of the CSV file too, but this is not
    //the actual code that will be implemented. The other test files that you will find in this project will cover implementation.

    public class FileTests
    {
        [Fact]
        public void File_Is_Not_Present_Test_Fails()
        {
            var options = new FileLocations() { PathToInputFile = string.Empty };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);

            Assert.True(string.IsNullOrEmpty(options.PathToInputFile));
        }

        [Fact]
        public void File_Is_Present_Test_Passes()
        {
            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "claimstodate.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);


            Assert.True(File.Exists(options.PathToInputFile));
        }

        [Fact]
        public async Task File_Is_Not_CSV_File()
        {
            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "fileisnotcsvfile.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);

            using (StreamReader fileReader = new StreamReader(options.PathToInputFile))
            {
                var contents = await fileReader.ReadToEndAsync();
                Assert.DoesNotContain(",", contents);
            }
        }

        [Fact]
        public async void File_Is_Valid_CSV_File()
        {
            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "validcsvfile.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);

            using (StreamReader fileReader = new StreamReader(options.PathToInputFile))
            {
                var contents = await fileReader.ReadToEndAsync();
                foreach (var line in contents)
                {
                    Assert.Contains(",", contents);
                }
            }
        }

        [Fact]
        public void File_Is_Valid_CSV_File_But_Contains_Lines_With_Not_Enough_Data()
        {
            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "notenoughdata.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);


            var content = File.ReadAllLines(options.PathToInputFile).ToList();
            content.RemoveAt(0);
            foreach (var line in content)
            {
                Assert.True(line.ToString().Split(",").Count() < 4);
            }

        }

        [Fact]
        public void File_Is_Valid_CSV_File_But_Contains_Lines_With_Not_Correct_Data()
        {
            var options = new FileLocations() { PathToInputFile = Path.Combine(Resources.TestFilePath, "badfile.csv") };
            var mockOptions = new Mock<IOptions<FileLocations>>();
            mockOptions.Setup(x => x.Value)
                .Returns(options);

            var content = File.ReadAllLines(options.PathToInputFile).ToList();
            content[0].Remove(0);
            foreach (var line in content)
            {
                Assert.Throws<InvalidCastException>(() => Convert.ToDecimal(line[3]));
            }
        }
    }
}
