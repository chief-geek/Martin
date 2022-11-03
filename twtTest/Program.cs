// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using twtTest;
using twtTest.ConcreteImplementations;
using twtTest.ConfigItems;
using twtTest.Interfaces;



await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddHostedService<Worker>()
            .AddTransient<IClaimsProcesor, ClaimsProcessor>()
            .AddTransient<IFieldMapper, FieldMapper>()
            .AddTransient<IFileProcessor, FileProcessor>()
            .AddTransient<IFileReader, FileReader>()
            .AddTransient<IFileWriter, FileWriter>();

        services.AddOptions<FileLocations>().Bind(hostContext.Configuration.GetSection("FileLocations"));
    })
    .RunConsoleAsync();








