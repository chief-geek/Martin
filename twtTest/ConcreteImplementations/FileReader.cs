using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using twtTest.ConfigItems;
using twtTest.Data;
using twtTest.Interfaces;

namespace twtTest.ConcreteImplementations
{
    public class FileReader : IFileReader
    {
        private readonly IFileProcessor _processor;
        private readonly IOptions<FileLocations> _options;
        private readonly ILogger<FileReader> _logger;

        public FileReader(IFileProcessor fileProcessor, IOptions<FileLocations> options, ILogger<FileReader> logger)
        {
            _processor = fileProcessor;
            _options = options;
            _logger = logger;
        }

        public async Task<List<ClaimAccumulation>> TryReadFile()
        {
            

            var result = new List<ClaimAccumulation>();
            var fileName = _options.Value.PathToInputFile!;

            _logger.LogInformation("Attempting to read claims file {fileName}", fileName);
            if (File.Exists(fileName))
            {
                try
                {
                    if (File.Exists(_options.Value.PathToOutputFile!))
                    {
                        File.Delete(_options.Value.PathToOutputFile!);
                    }

                    if (File.Exists(fileName))
                    {
                        FileInfo fileInfo = new FileInfo(_options.Value.PathToInputFile!);
                        if (fileInfo.Length <= 0)
                        {
                            throw new Exception($"File {_options.Value.PathToInputFile} has no data, aborting!");
                        }
                    }
                    
                    int lineCount = 0;
                    string? currentLine = null;
                    using StreamReader sr = new StreamReader(fileName);

                    currentLine = await sr.ReadLineAsync();
                    if (string.IsNullOrEmpty(currentLine))
                    {
                        throw new UnexpectedFileReadException($"First line of {_options.Value.PathToInputFile} should have contained data but was empty, aborting!");
                    }

                    BaseData.ColumnCount = string.IsNullOrEmpty(BaseData.Headers) ? 0 : BaseData.Headers.Split(',').Length;
                    InsuranceClaims claims = new();
                    while (!sr.EndOfStream)
                    {
                        currentLine = await sr.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(currentLine))
                        {
                            var claimsResults = await _processor.ConvertFileLinesToInsuranceClaims(currentLine, lineCount!);
                            if (claimsResults != null)
                            {
                                claims.ValidInsuranceClaims.AddRange(claimsResults);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Line {lineCount} had no data, ignoring", lineCount);
                        }
                    }

                    _ = claims.ValidInsuranceClaims.OrderBy(o => o.Product);
                    BaseData.MinYear = claims.ValidInsuranceClaims.Min(x => x.OriginYear).ToString();
                    BaseData.YearValues = (claims.ValidInsuranceClaims.Max(x => x.OriginYear) - claims.ValidInsuranceClaims.Min(x => x.OriginYear) + 1).ToString();
                    result = await _processor.AccumulateInsuranceClaims(claims);
                    lineCount++;

                }
                catch (Exception ex)
                {
                    _logger.LogCritical("Press any key to exit {ex.Message}", ex.Message);
                    throw;
                }
            }
            else
            {
                throw new FileNotFoundException($"File not found {_options.Value.PathToInputFile}");
            }
            return result;
        }
    }
}
