using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using twtTest.ConfigItems;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTest.ConcreteImplementations
{
    public class FileWriter : IFileWriter
    {
        private readonly ILogger<FileWriter> _logger;
        private readonly IOptions<FileLocations> _options;

        public FileWriter(ILogger<FileWriter> logger, IOptions<FileLocations> options)
        {
            _logger = logger;
            _options = options;
        }

        public Task ConvertAndWriteToCSV(List<ClaimAccumulation> claims)
        {


            if (string.IsNullOrEmpty(_options.Value.PathToOutputFile) || claims == null)
            {
                _logger.LogError($"No output file path provided, aborting operation!");
                throw new Exception($"Aborting process: either file is null or claims are null : \nFileLocation : {_options.Value.PathToOutputFile} \n Claims Null : {claims == null}");
            }

            if (!File.Exists(_options.Value.PathToOutputFile))
            {
                File.Create(_options.Value.PathToOutputFile!);
            }

            _logger.LogInformation("Converting claims to CSV");

            try
            {
                var linesToWrite = new List<string>();
                using (StreamWriter writer = new StreamWriter(_options.Value.PathToOutputFile))
                {

                    var products = claims.GroupBy(
                                        c => c.Product,
                                        c => c.AccumulatedValue,
                                        (key, g) => new { Product = key, Claims = g.ToList() });

                    foreach (var product in products)
                    {
                        linesToWrite.Add(product.Product + ", " + string.Join(", ", product.Claims));
                    }

                    linesToWrite.OrderBy(x => x.Length);

                    writer.WriteLine(BaseData.MinYear + ", " + BaseData.YearValues);
                    for (int i = 0; i < linesToWrite.Count; i++)
                    {
                        var columnCount = linesToWrite.Last().Count(c => c == ',');
                        writer.WriteLine(AddLeadingZeroes(linesToWrite[i], columnCount));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("The file could not be written due to the following exception {ex.Message} inner {ex.InnerException}", ex.Message, ex.InnerException);
                throw new Exception($"The file could not be written due to the following exception", ex.InnerException!);
            }

            return Task.CompletedTask;
        }

        private static string AddLeadingZeroes(string inputLine, int columnCount)
        {
            if (inputLine.Count(c => (c == ',')) == columnCount) { return (inputLine); };

            int counter = 0;
            var insertLine = " ";
            while (counter < columnCount)
            {
                insertLine += "0, ";
                counter++;
            }
            int startPosition = inputLine.ToLower(CultureInfo.InvariantCulture).IndexOf(", ");
            inputLine = inputLine.Insert(startPosition + 1, insertLine);
            return string.Join(", ", inputLine);
        }
    }
}
