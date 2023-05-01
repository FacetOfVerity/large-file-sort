namespace FileSorter.Interfaces;

public interface IChunkSettingsProvider
{
    public long OptimalChunkSize { get; }

    public int OptimalChunksCount { get; }
}