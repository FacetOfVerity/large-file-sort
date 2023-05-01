using System.Globalization;

namespace FileSorter.Utils;

public class FileLineComparer : IComparer<string>
{
    public int Compare(string? left, string? right)
    {
        var leftSplit = left?.Split('.');
        if (!int.TryParse(leftSplit?[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int leftNumber))
        {
            return 1;
        }
        var leftText = leftSplit[1].Trim();
            
        var rightSplit = right?.Split('.');
        if (!int.TryParse(rightSplit?[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int rightNumber))
        {
            return -1;
        }
        var rightText = rightSplit[1].Trim();
        
        var result = string.Compare(leftText, rightText, StringComparison.Ordinal);
        if (result == 0)
        {
            result = leftNumber.CompareTo(rightNumber);
        }
        
        return result;
    }
}