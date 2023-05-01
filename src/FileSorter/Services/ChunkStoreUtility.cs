using System.Collections.Concurrent;
using FileSorter.Interfaces;
using FileSorter.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSorter.Services;

public class ChunkStoreUtility
{
    private readonly SortOptions _options;
    private readonly List<Task> _saveTasks;
    private readonly BlockingCollection<SortedChunk> _chunks;
    private readonly ILogger<ChunkStoreUtility> _logger;

    public ChunkStoreUtility(IOptions<SortOptions> options, IChunkSettingsProvider chunkSettingsProvider,
        ILogger<ChunkStoreUtility> logger)
    {
        _logger = logger;
        _options = options.Value;
        _saveTasks = new List<Task>();
        _chunks = new BlockingCollection<SortedChunk>(chunkSettingsProvider.OptimalChunksCount);

        Directory.CreateDirectory(_options.SortedChunksPath);
    }

    public void RunSaveChunk(SortedSet<string> set, Dictionary<string, int> repeatsDict, int chunkNumber)
    {
        _saveTasks.Add(SaveChunk(set, repeatsDict,  chunkNumber));
    }
    
    private async Task SaveChunk(SortedSet<string> set, Dictionary<string, int> repeatsDict, int chunkNumber)
    {
        try
        {
            await using var file = File.Create(Path.Combine(_options.SortedChunksPath, $"chunk_{chunkNumber}.txt"));
            await using var writer = new StreamWriter(file);

            foreach (var line in set)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var count = repeatsDict[line];
                for (int i = 0; i < count; i++)
                {
                    await writer.WriteLineAsync(line);
                }
            }

            _chunks.Add(new SortedChunk(file.Name));
            _logger.LogInformation("Chunk {ChunkNumber} is being processed", chunkNumber);
        }
        catch (Exception e)
        {
            _logger.LogError("Error during save chunk {ChunkNumber}. {EMessage}", chunkNumber, e.Message);
            throw;
        }
    }

    public async Task<SortedChunk[]> GetResult()
    {
        await Task.WhenAll(_saveTasks);

        return _chunks.ToArray();
    }
}