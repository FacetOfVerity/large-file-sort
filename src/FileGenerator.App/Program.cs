using System.Diagnostics;
using FileGenerator;
using FileGenerator.App;
using Microsoft.Extensions.Configuration;

Console.WriteLine("FileGenerator.App");

var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
var root = builder.Build();

var options = root.GetSection(FileGeneratorOptions.ConfigSection).Get<FileGeneratorOptions>()!;
var filePath = Path.Combine(options.WorkFolderPath, options.GeneratedFileName);

while (File.Exists(filePath))
{
    Console.WriteLine($"File {filePath} already exists. Remove it and press any key.");
    Console.ReadKey();
}

Console.WriteLine($"Configs:\nNumberOfLines: {options.NumberOfLines}\nGeneratedFilePath: {filePath}");

var watch = new Stopwatch();
watch.Start();

Console.WriteLine("\nGeneration started\n...");
Directory.CreateDirectory(options.WorkFolderPath);
LargeFileGenerator
    .Create(filePath, options.NumberOfLines)
    .Generate();
watch.Stop();

var fileSize = new FileInfo(filePath).Length / 1048576;
Console.WriteLine("File generated successfully. Size: {0} Mb", fileSize.ToString());
Console.WriteLine("Elapsed time: {0} sec.", watch.Elapsed.TotalSeconds);