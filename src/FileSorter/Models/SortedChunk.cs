namespace FileSorter.Models;

public class SortedChunk : IDisposable
{
    public string FilePath { get; }
    public bool Closed { get; private set; }
    
    private StreamReader? _reader;

    public SortedChunk(string filePath)
    {
        FilePath = filePath;
    }

    public void Close()
    {
        Closed = true;
        Dispose();
    }

    public bool EndAchieved()
    {
        EnsureReaderOpened();
        
        return _reader!.EndOfStream;
    }

    public string ReadLine()
    {
        EnsureReaderOpened();
        
        if (!Closed)
        {
            return _reader!.ReadLine()!;
        }

        throw new InvalidOperationException("EndOfStream achieved");
    }

    private void EnsureReaderOpened()
    {
        if (_reader is null)
        {
            _reader = new StreamReader(FilePath);
        }
    }

    public void Dispose()
    {
        _reader?.Dispose();
    }
}