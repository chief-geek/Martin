// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using twtTest.Interfaces;

namespace twtTest
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IFileReader _reader;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IFileWriter _writer;
        private int? _exitCode = 0;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime, IFileReader fileReader, IFileWriter fileWriter)
        {
            _logger = logger;
            _reader = fileReader;
            _appLifetime = appLifetime;
            _writer = fileWriter;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var claims = await _reader.TryReadFile();
                        await _writer.ConvertAndWriteToCSV(claims);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical($"The following exception occurred whilst processing your file {ex.Message} with inner {ex.InnerException}");
                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Exiting with code {_exitCode}");
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }
}