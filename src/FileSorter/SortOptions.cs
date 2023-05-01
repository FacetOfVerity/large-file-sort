namespace FileSorter;

public class SortOptions
{
    public string UnsortedFileName { get; init; }
    
    public string SortedFileName { get; init; }
    
    public string WorkFolderPath { get; init; }

    public string TempFolderName { get; init; }

    public string UnsortedFilePath => Path.Combine(WorkFolderPath, UnsortedFileName);
    
    public string SortedFilePath => Path.Combine(WorkFolderPath, SortedFileName);
    
    public string SortedChunksPath => Path.Combine(WorkFolderPath, TempFolderName);
    
    

    public static string ConfigSection = "SortOptions";

    public void Validate()
    {
        if (!File.Exists(UnsortedFilePath))
        {
            throw new ArgumentException("UnsortedFilePath does not exist");
        }
        if (string.IsNullOrWhiteSpace(SortedFilePath))
        {
            throw new ArgumentException("SortedFilePath is invalid");
        }
    }
}