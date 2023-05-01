# large-file-sort

### Problem description
The input is a large text file, where each line is a Number. String For example:
415. Apple
30432. Something something something
1. Apple
32. Cherry is the best
2. Banana is yellow

Both parts can be repeated within the file. You need to get another file as output,
where all the lines are sorted. Sorting Criteria: String part is compared first, 
if it matches then Number. Those. in the example above it should be:
1. Apple
415. Apple
2. Banana is yellow
32. Cherry is the best
30432. Something something something

### Solution description
FileGenerator.App generates text file with configured number of lines.
FileSorter.App sorts the custom size text file (generates a new file). FileSorter.App uses FileSorter.Utils.FileLineComparer to order lines.
Also there are Tests project with a few tests

All configurations are set through appsettings.json files.

### Sorting steps:
1. Calculation of optimal values for chunk size and number of chunks (based on available memory and available processors number)
2. Splitting an unsorted file into sorted chunk files
3. The sorted chunks are merged into a new sorted file using the K-Way merge algorithm.
