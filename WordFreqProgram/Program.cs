
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Challenge
{
    public class Program
    {
        private readonly IFileReader _fileReader;
        private readonly IPrinter _frequencyPrinter;

        public Program(IFileReader fileReader, IPrinter frequencyPrinter)
        {
            _fileReader = fileReader;
            _frequencyPrinter = frequencyPrinter;
        }

        // Method to process files and print word frequencies
        public ConcurrentDictionary<string, int> Run(string path, bool print = true)
        {
            ConcurrentBag<string> contents = _fileReader.ReadFiles(path);
            ConcurrentDictionary<string, int> wordFreq = new(StringComparer.Ordinal);

            // Calculating word frequencies in parallel
            Parallel.ForEach(contents, content =>
            {
                var localWordFreq = content.GetWordFrequencies();
                foreach (var word in localWordFreq)
                {
                    wordFreq.AddOrUpdate(word.Key, word.Value, (key, oldValue) => oldValue + word.Value);
                }
            });

            // Print flag is used avoid printing when testing
            if (print == true)
            {
                _frequencyPrinter.Print(wordFreq);
            }
            return wordFreq;
        }

        static void Main(string[] args)
        {
            IFileReader fileReader = new FileReader();
            IPrinter frequencyPrinter = new FreqPrinter();
            Program program = new Program(fileReader, frequencyPrinter);
            program.Run(args[0]);
        }
    }

    public static class StringExtensions
    {

        public static ConcurrentDictionary<string, int> GetWordFrequencies(this string input)
        {

            ConcurrentDictionary<string, int> wordFrequencies = new(StringComparer.Ordinal);

            if (string.IsNullOrEmpty(input))
                return wordFrequencies;

            // Extract words using Regex and compute frequencies in parallel
            var words = Regex.Matches(input, @"(?:\p{L}+[-']?)*\p{L}+");
            Parallel.ForEach(words, word =>
            {
                string lowercasedWord = word.Value.ToLower();
                wordFrequencies.AddOrUpdate(lowercasedWord, 1, (key, oldValue) => oldValue + 1);
            });

            return wordFrequencies;
        }
    }
}