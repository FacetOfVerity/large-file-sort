using System.Diagnostics;
using System.Security.Cryptography;
using FileGenerator;
using FileSorter;
using FileSorter.Interfaces;
using FileSorter.Models;
using FileSorter.Services;
using FileSorter.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tests.Mocks;

namespace Tests;

public class MergeTest
{
    private readonly SortOptions _sortOptions;
    private readonly TestOptions _testOptions;
    private readonly IServiceProvider _serviceProvider;
    
    private readonly string _sortedTestFilePath;
    private readonly Stopwatch _watch;
    private SortedChunk[] _chunks;
    

    public MergeTest()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
        var root = builder.Build();
    
        var services =  new ServiceCollection()
            .AddSingleton<IChunkSettingsProvider, TestChunkSettingsProvider>()
            .Configure<TestOptions>(root.GetSection(TestOptions.ConfigSection))
            .RegisterFileSorterServices(root)
            .AddLogging(conf => conf.AddConsole());;
        
        _serviceProvider = services.BuildServiceProvider();
        _sortOptions = _serviceProvider.GetRequiredService<IOptions<SortOptions>>().Value;
        _testOptions = _serviceProvider.GetRequiredService<IOptions<TestOptions>>().Value;
        _sortedTestFilePath = Path.Combine(_sortOptions.WorkFolderPath, "sortedTestFile.txt");
        
        _watch = new Stopwatch();
    }
    
    [SetUp]
    public void Setup()
    {
        Directory.CreateDirectory(_sortOptions.WorkFolderPath);
        Directory.CreateDirectory(_sortOptions.SortedChunksPath);
        
        if (!File.Exists(_sortOptions.UnsortedFilePath))
        {
            LargeFileGenerator.Create(_sortOptions.UnsortedFilePath, _testOptions.NumberOfLines).Generate();

            var fileSize = new FileInfo(_sortOptions.UnsortedFilePath).Length / 1048576;
            Console.WriteLine("File created successfully. Size: {0} Mb", fileSize.ToString());
        }

        if (!File.Exists(_sortedTestFilePath))
        {
            var lines = File.ReadAllLines(_sortOptions.UnsortedFilePath);
            Array.Sort(lines, new FileLineComparer());
        
            File.WriteAllLines(_sortedTestFilePath, lines);
        }

        if (!Directory.GetFiles(_sortOptions.SortedChunksPath).Any())
        {
            var generator = _serviceProvider.GetRequiredService<SortedChunksGenerator>();
            _chunks = generator.SplitAndSortAsync().Result;
        }
        else
        {
            _chunks = Directory
                .GetFiles(_sortOptions.SortedChunksPath)
                .Select(a => new SortedChunk(a))
                .ToArray();
        }
    }

    [Test]
    public async Task KWayMergeTest()
    {
        _watch.Start();
        var kWayMergeExecutor = _serviceProvider.GetRequiredService<KWayMergeExecutor>();
        _watch.Stop();
        Console.WriteLine($"Merge time: {_watch.Elapsed.TotalSeconds} sec.");
        
        kWayMergeExecutor.MergeSortedFiles(_chunks);
        
        var sortedTestFile = await File.ReadAllBytesAsync(_sortedTestFilePath);
        var sortedTestFileHash = MD5.HashData(sortedTestFile);
        
        var sortedFile = await File.ReadAllBytesAsync(_sortOptions.SortedFilePath);
        var sortedFileHash = MD5.HashData(sortedFile);

        Assert.That(sortedFileHash, Is.EqualTo(sortedTestFileHash));
    }
}