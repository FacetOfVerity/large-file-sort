namespace FileGenerator.App;

public class FileGeneratorOptions
{
    public ulong NumberOfLines { get; set; } //600000 lines ~ 20 Mb

    public string WorkFolderPath { get; set; }
    
    public string GeneratedFileName { get; set; }

    public static string ConfigSection = "FileGeneratorOptions";
}