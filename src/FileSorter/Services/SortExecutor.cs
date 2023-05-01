using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSorter.Services;

public class SortExecutor
{
    private readonly SortedChunksGenerator _sortedChunksGenerator;
    private readonly KWayMergeExecutor _kWayMergeExecutor;
    private readonly SortOptions _options;
    private readonly ILogger<SortExecutor> _logger;

    public SortExecutor(SortedChunksGenerator sortedChunksGenerator, KWayMergeExecutor kWayMergeExecutor, IOptions<SortOptions> options, ILogger<SortExecutor> logger)
    {
        _sortedChunksGenerator = sortedChunksGenerator;
        _kWayMergeExecutor = kWayMergeExecutor;
        _logger = logger;
        _options = options.Value;
    }

    public async Task RunSort()
    {
        _options.Validate();
        
        _logger.LogInformation("Processing started");
        
        var chunks = await _sortedChunksGenerator.SplitAndSortAsync();
        
        _logger.LogInformation("All chunks saved. Merge is being run");
        _kWayMergeExecutor.MergeSortedFiles(chunks);
    }
}