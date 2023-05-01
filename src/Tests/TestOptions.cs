namespace Tests;

public class TestOptions
{
    public long AvailableMemory { get; set; }
    public int AvailableProcessors { get; set; }
    public ulong NumberOfLines { get; set; } //600000 lines ~ 20 Mb
    
    public static string ConfigSection = "TestOptions";
}