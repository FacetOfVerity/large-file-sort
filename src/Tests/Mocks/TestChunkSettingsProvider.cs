using FileSorter;
using FileSorter.Interfaces;
using FileSorter.Services;
using Microsoft.Extensions.Options;

namespace Tests.Mocks;

internal class TestChunkSettingsProvider : IChunkSettingsProvider
{
    /// <summary>
    /// Default file system limit of opened files for process is 1024.
    /// </summary>
    private const int OPENED_FILES_LIMIT = 1000;
    
    public long OptimalChunkSize { get; }

    public int OptimalChunksCount { get; }

    public TestChunkSettingsProvider(IOptions<SortOptions> sortOptions, IOptions<TestOptions> testOptions)
    {
        var unsortedFileSize = new FileInfo(sortOptions.Value.UnsortedFilePath).Length;

        // use test parameters of environment
        var availableMemory = testOptions.Value.AvailableMemory;
        var availableProcessors = testOptions.Value.AvailableProcessors;
        
        Console.WriteLine($"TotalAvailableMemoryBytes: {availableMemory}");
        Console.WriteLine($"AvailableProcessors: {availableProcessors}");

        OptimalChunkSize = availableMemory / availableProcessors;
        OptimalChunksCount = (int)Math.Ceiling(unsortedFileSize / (decimal)OptimalChunkSize);

        // if not enough memory
        if (OptimalChunksCount > OPENED_FILES_LIMIT)
        {
            OptimalChunksCount = OPENED_FILES_LIMIT;
            OptimalChunkSize = unsortedFileSize / OptimalChunksCount;
        }
    }
}