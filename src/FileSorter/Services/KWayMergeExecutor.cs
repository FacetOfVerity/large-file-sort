using FileSorter.Models;
using FileSorter.Utils;
using Microsoft.Extensions.Options;

namespace FileSorter.Services;

public class KWayMergeExecutor
{
    private record MinHeapNode(string Element, SortedChunk Chunk);
    
    private readonly SortOptions _options;

    public KWayMergeExecutor(IOptions<SortOptions> options)
    {
        _options = options.Value;
    }

    public void MergeSortedFiles(SortedChunk[] chunks)
    {
        if (chunks.Length == 1) // optimization for case with single chunk
        {
            File.Move(chunks[0].FilePath, _options.SortedFilePath);
            
            return;
        }
        
        using var resultWriter = File.CreateText(_options.SortedFilePath);
        var priorityQueue = new PriorityQueue<MinHeapNode, string>(chunks.Length, new FileLineComparer());
        
        // create a min Heap and insert the first element of all the K arrays.
        foreach (var sortedChunk in chunks)
        {
            var line = sortedChunk.ReadLine();
            priorityQueue.Enqueue(new MinHeapNode(line, sortedChunk), line);
        }

        // write sorted layer to result file
        while (priorityQueue.TryDequeue(out MinHeapNode node, out _))
        {
            resultWriter.WriteLine(node.Element);

            if (node.Chunk.Closed) // skip closed chunk
            {
                continue;
            }

            var nextInChunk = node.Chunk.ReadLine();
            if (node.Chunk.EndAchieved()) // nextInChunk is last line
            {
                node.Chunk.Close();
            }
            
            priorityQueue.Enqueue(node with {Element = nextInChunk}, nextInChunk);
        }
    }
}