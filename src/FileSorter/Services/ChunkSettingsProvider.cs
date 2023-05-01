using FileSorter.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSorter.Services;

internal class ChunkSettingsProvider : IChunkSettingsProvider
{
    /// <summary>
    /// Default file system limit of opened files for process is 1024.
    /// </summary>
    private const int OPENED_FILES_LIMIT = 1000;
    
    public long OptimalChunkSize { get; }

    public int OptimalChunksCount { get; }

    public ChunkSettingsProvider(IOptions<SortOptions> options, ILogger<ChunkSettingsProvider> logger)
    {
        var unsortedFileSize = new FileInfo(options.Value.UnsortedFilePath).Length;
        var availableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        var availableProcessors = Environment.ProcessorCount;

        OptimalChunkSize = availableMemory / availableProcessors;
        OptimalChunksCount = (int) Math.Ceiling(unsortedFileSize / (decimal) OptimalChunkSize);

        // if not enough memory
        if (OptimalChunksCount > OPENED_FILES_LIMIT)
        {
            OptimalChunksCount = OPENED_FILES_LIMIT;
            OptimalChunkSize = unsortedFileSize / OptimalChunksCount;
        }
        
        logger.LogInformation("TotalAvailableMemoryBytes: {AvailableMemory}", availableMemory);
        logger.LogInformation("AvailableProcessors: {AvailableProcessors}", availableProcessors);
        logger.LogInformation("OptimalChunkSize: {OptimalChunkSize}", OptimalChunkSize);
        logger.LogInformation("OptimalChunksCount: {OptimalChunksCount}", OptimalChunksCount);
    }
}