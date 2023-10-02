using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Challenge
{
    // Interface defining the print method for word frequencies
    public interface IPrinter
    {
        void Print(ConcurrentDictionary<string, int> wordFrequencies);
    }

    public class FreqPrinter : IPrinter
    {
        public void Print(ConcurrentDictionary<string, int> wordFrequencies)
        {
            int totalWordCount = 0;

            // Converting to List and sorting by frequency
            List<KeyValuePair<string, int>> sortedList = wordFrequencies.ToList();
            sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            // Iterating over sorted list to print word frequencies and calculate total word count
            foreach (var pair in sortedList)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
                totalWordCount += pair.Value;
            }

            // Printing total word count
            Console.WriteLine($"Total words: {totalWordCount}");
        }
    }
}
