using System.Diagnostics;
using FileSorter;
using FileSorter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

Console.WriteLine("FileSorter.App");

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();

var root = builder.Build();
var services = new ServiceCollection()
    .RegisterFileSorterServices(root)
    .AddLogging(conf => conf.AddConsole());
var serviceProvider = services.BuildServiceProvider();

// check files
var options = serviceProvider.GetRequiredService<IOptions<SortOptions>>().Value;
if (!File.Exists(options.UnsortedFilePath))
{
    throw new InvalidOperationException(
        $"File {options.UnsortedFilePath} doesn't exists. Generate it and restart the app.");
}

if (File.Exists(options.SortedFilePath))
{
    throw new InvalidOperationException(
        $"File {options.SortedFilePath} already exists. Remove it and restart the app.");
}

var sortExecutor = serviceProvider.GetRequiredService<SortExecutor>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var watch = new Stopwatch();

logger.LogInformation("Sorting started...");
watch.Start();
await sortExecutor.RunSort();
watch.Stop();
logger.LogInformation("Sorting finished. Elapsed time: {ElapsedTotalSeconds} sec.", watch.Elapsed.TotalSeconds);

logger.LogInformation("Removing temp files...");
Directory.Delete(options.SortedChunksPath, true);
logger.LogInformation("Temp files removed.");