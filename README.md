### Word Frequency Counter
A console application that reads all text files from a specified directory and calculates the 
frequency of each word found in the files. The application is parallelized to enhance performance 
when dealing with a large number of files or substantial file sizes.

### Approach
I opted for a Hash table structure, given its ability to perform add and lookup operations in constant time, O(1).
Emphasis was placed on utilizing thread-safe data structures and rigorously testing them.

An alternative approach could have been optimizing for runtime by selecting methods and data structures that are more efficient for specific data characteristics. However, I prioritized parallelization as a more viable option, even if the overhead compromises runtime for smaller inputs.

Dependency Injection and Modularization were emphasized to adhere to the Single Responsibility Principle (SRP) and to facilitate testing.

### Features
* Concurrently reads files into a thread-safe collection object.
* Employs a thread-safe Hash-table to calculate word frequency.
* Prints word frequencies (case-insensitive) to the console.
* Features robust error-handling mechanisms for file and directory reading.

### Usage
Argument: Full-path or folder name present in the project's root-directory (SimCorp/WordFreqProgram/)
```
dotnet run [DirectoryPath] // Example: 'dotnet run test_files' or 'dotnet run User/Documents/SimCorp/WordFreqProgram/test_files'
```


### Testing
Execute 'dotnet test' inside the test directory to run tests.

* Test 1: Verify correct frequencies for a given input string.
* Test 2: Ensure an empty dictionary is returned for an empty input string.
* Test 3: Confirm if a single-word dictionary is returned for a single-word string.
* Test 4: Confirm correct frequencies for a string with multiple spaces between words.
* Test 5: Validate correct frequencies for a string with special characters.
* Test 6: Examine correct frequencies for words with apostrophes.
* Test 7: Verify correct frequencies for words with hyphens.
* Test 8: Check correct frequencies for words with non-ASCII characters.
* Test 9: Confirm correct frequencies for words with mixed capitalization.
* Test 10: Validate correct frequencies for words with leading or trailing spaces.
* Test 11: Ensure files are correctly read from the directory.
* Test 12: Verify if a DirectoryNotFoundException is thrown for a nonexistent directory.
* Test 13: Ensure an IOException is thrown for a directory with no readable files.
* Test 14: Confirm that the program returns accurate word frequencies.

### Dependencies
* .NET 7.0
* NUnit - Version 3.13.3
* NUnit3TestAdapter - Version 4.4.2
* NUnit.Analyzers - Version 3.6.1
