using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Challenge
{
    public interface IFileReader
    {
        ConcurrentBag<string> ReadFiles(string path);
    }

    public class FileReader : IFileReader
    {
        public ConcurrentBag<string> ReadFiles(string folderName)
        {
            var contentsBag = new ConcurrentBag<string>(); // Thread-safe collection to store file contents

            string directoryPath = ConstructDirectoryPath(folderName);
            ValidateDirectoryExists(directoryPath);

            var files = GetTextFiles(directoryPath);

            // Read the contents of each file using parallel processing
            Parallel.ForEach(files, file =>
            {
                var content = ReadFileContent(file);
                contentsBag.Add(content);
            });

            return contentsBag;
        }

        private string ConstructDirectoryPath(string folderName)
        {

            if (Path.IsPathFullyQualified(folderName))
                return folderName;

            string currentDirectory = Directory.GetCurrentDirectory();
            return Path.Combine(currentDirectory, folderName);
        }

        private void ValidateDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory {directoryPath} not found.");
        }

        private string[] GetTextFiles(string directoryPath)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath, "*.txt");
                if (files.Length == 0)
                    throw new IOException($"No files found in the directory {directoryPath}.");
                return files;
            }
            catch (Exception ex)
            {
                throw new IOException($"Error reading files from {directoryPath}: {ex.Message}");
            }
        }

        private string ReadFileContent(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error reading file {filePath}: {ex.Message}");
            }
        }
    }
}