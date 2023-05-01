using System.Text;
using FileSorter.Constants;
using FileSorter.Interfaces;
using FileSorter.Models;
using FileSorter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSorter.Services;

public class SortedChunksGenerator
{
    private readonly SortOptions _options;
    private readonly IChunkSettingsProvider _chunkSettingsProvider;
    private readonly ChunkStoreUtility _chunkStoreUtility;
    private readonly ILogger<SortedChunksGenerator> _logger;

    public SortedChunksGenerator(IOptions<SortOptions> options, IChunkSettingsProvider chunkSettingsProvider,
        ChunkStoreUtility chunkStoreUtility, ILogger<SortedChunksGenerator> logger)
    {
        _options = options.Value;
        _chunkSettingsProvider = chunkSettingsProvider;
        _chunkStoreUtility = chunkStoreUtility;
        _logger = logger;
    }

    public async Task<SortedChunk[]> SplitAndSortAsync()
    {
        var sourceStream = File.OpenRead(_options.UnsortedFilePath);

        for (int i = 0; i < _chunkSettingsProvider.OptimalChunksCount; i++)
        {
            _logger.LogInformation("Chunk {ChunkNumber} is being processed", i);
            var sortedSet = new SortedSet<string>(new FileLineComparer()); // contains sorted unique lines
            var repeatsDict = new Dictionary<string, int>(); // contains count of unique lines

            // extract a chunk from source
            var finished = false;
            var lineBytes = new DynamicArray<byte>();
            for (long j = 0; j < _chunkSettingsProvider.OptimalChunkSize; j++)
            {
                var readByte = sourceStream.ReadByte();
                
                if (readByte == -1) // end of file
                {
                    // save last line
                    SaveLine();
                    finished = true;
                    break;
                }

                if (readByte == SortConstants.LINE_SEPARATOR)
                {
                    SaveLine();
                }
                else
                {
                    lineBytes.Add((byte) readByte);
                }
            }

            // add tail
            if (!finished && lineBytes.Any())
            {
                byte tailByte;
                
                while (true)
                {
                    var value = sourceStream.ReadByte();
                    if (value == -1 || value == SortConstants.LINE_SEPARATOR)
                    {
                        break;
                    }
                    tailByte = (byte)value;
                    lineBytes.Add(tailByte);
                }

                SaveLine();
            }

            //save to FS
            _chunkStoreUtility.RunSaveChunk(sortedSet, repeatsDict, i);

            void SaveLine()
            {
                var line = Encoding.Default.GetString(lineBytes.Buffer, 0, lineBytes.Length);
                
                if (repeatsDict.ContainsKey(line))
                {
                    repeatsDict[line]++;
                }
                else
                {
                    repeatsDict.Add(line, 1);
                    sortedSet.Add(line);
                }
                lineBytes.Reset(); // lineBytes buffer will be reused
            }
        }

        return await _chunkStoreUtility.GetResult();
    }
}