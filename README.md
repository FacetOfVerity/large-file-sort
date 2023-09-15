# large-file-sort

### Problem description
The input is an extralarge text file, where each line is a Number. String For example:

<li>415. Apple</li>
<li>30432. Something something something</li>
<li>1. Apple</li>
<li>32. Cherry is the best</li>
<li>2. Banana is yellow</li>

Both parts can be repeated within the file. You need to get another file as output,
where all the lines are sorted. Sorting Criteria: String part is compared first, 
if it matches then Number. Those. in the example above it should be:
<li>1. Apple</li>
<li>415. Apple</li>
<li>2. Banana is yellow</li>
<li>32. Cherry is the best</li>
<li>30432. Something something something</li>

### Solution description
FileGenerator.App generates text file with configured number of lines.
FileSorter.App sorts the custom size text file (generates a new file). FileSorter.App uses FileSorter.Utils.FileLineComparer to order lines.
Also there are Tests project with a few tests

All configurations are set through appsettings.json files.

### Sorting steps:
1. Calculation of optimal values for chunk size and number of chunks (based on available memory and available processors number)
2. Splitting an unsorted file into sorted chunk files
3. The sorted chunks are merged into a new sorted file using the K-Way merge algorithm.
