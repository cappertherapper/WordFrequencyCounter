using System.Collections.Concurrent;
using Challenge;
using NUnit.Framework;

namespace WordFreqProgram.Tests
{
    [TestFixture]
    public class WordFreqTests
    {
        private readonly ConcurrentDictionary<string, int> _expectedFrequencies = new(StringComparer.Ordinal)
        {
            ["the"] = 4,
            ["quick"] = 1,
            ["brown"] = 1,
            ["fox"] = 2,
            ["jumps"] = 1,
            ["over"] = 1,
            ["lazy"] = 1,
            ["dog"] = 2,
            ["barked"] = 1,
            ["and"] = 1,
            ["ran"] = 1,
            ["away"] = 1
        };
        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForGivenInputString()
        {
            var input = "The quick brown fox jumps over the lazy dog. The dog barked and the fox ran away.";

            Assert.That(input.GetWordFrequencies(), Is.EqualTo(_expectedFrequencies));
        }

        [Test]
        [Repeat(100)]
        public void ShouldReturnEmptyDictionary_ForEmptyString()
        {
            var input = "";
            var expected = new ConcurrentDictionary<string, int>();
            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(100)]
        public void ShouldReturnSingleWordDictionary_ForSingleWordString()
        {
            var input = "hello";
            var expected = new ConcurrentDictionary<string, int>() { ["hello"] = 1 };
            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForStringWithMultipleSpaces()
        {
            var input = "The    quick   brown      fox jumps over  the lazy dog.   The dog barked   and the    fox ran away.";

            var actual = input.GetWordFrequencies();
            Assert.That(actual, Is.EqualTo(_expectedFrequencies));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForStringWithSpecialChars()
        {
            var input = "The quick brown fox jumps over the lazy dog. The dog barked and the fox ran away!@#$%^&*()_+-={}[]|\\:;\"'<>,.?/";

            Assert.That(input.GetWordFrequencies(), Is.EqualTo(_expectedFrequencies));
        }
        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForWordsWithApostrophes()
        {
            var input = "I'm can't won't he's she's it's they're we're you're";
            var expected = new ConcurrentDictionary<string, int>(StringComparer.Ordinal)
            {
                ["i'm"] = 1,
                ["can't"] = 1,
                ["won't"] = 1,
                ["he's"] = 1,
                ["she's"] = 1,
                ["it's"] = 1,
                ["they're"] = 1,
                ["we're"] = 1,
                ["you're"] = 1
            };

            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForWordsWithHyphens()
        {
            var input = "self-driving well-being co-worker";
            var expected = new ConcurrentDictionary<string, int>(StringComparer.Ordinal)
            {
                ["self-driving"] = 1,
                ["well-being"] = 1,
                ["co-worker"] = 1
            };

            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForWordsWithNonAsciiCharacters()
        {
            var input = "résumé über café";
            var expected = new ConcurrentDictionary<string, int>(StringComparer.Ordinal)
            {
                ["résumé"] = 1,
                ["über"] = 1,
                ["café"] = 1
            };

            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForWordsWithMixedCapitalization()
        {
            var input = "The the tHe";
            var expected = new ConcurrentDictionary<string, int>(StringComparer.Ordinal)
            {
                ["the"] = 3
            };
            Assert.That(input.GetWordFrequencies(), Is.EqualTo(expected));
        }

        [Test]
        [Repeat(1000)]
        public void ShouldReturnCorrectFrequencies_ForWordsWithLeadingOrTrailingSpaces()
        {
            var input = "     The quick brown fox jumps over the lazy dog. The dog barked and the fox ran away.     ";
            Assert.That(input.GetWordFrequencies(), Is.EqualTo(_expectedFrequencies));
        }
    }


    [TestFixture]
    public class FileReaderTests
    {
        private string _testDirectory;
        private IFileReader _fileReader;

        [SetUp]
        public void SetUp()
        {
            _fileReader = new FileReader();

            _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testDirectory);
            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "This is the content of file1.");
            File.WriteAllText(Path.Combine(_testDirectory, "file2.txt"), "This is the content of file2.");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        [Test]
        public void ShouldReadFilesFromDirectory()
        {
            var contents = _fileReader.ReadFiles(_testDirectory);
            Assert.That(contents.Count, Is.EqualTo(2));
            Assert.That(contents, Has.Member("This is the content of file1."));
            Assert.That(contents, Has.Member("This is the content of file2."));
        }

        [Test]
        public void ShouldThrowDirectoryNotFoundException_ForNonexistentDirectory()
        {
            var nonexistentDirectory = Path.Combine(_testDirectory, "nonexistent");
            Assert.Throws<DirectoryNotFoundException>(() => _fileReader.ReadFiles(nonexistentDirectory));
        }

        [Test]
        public void ShouldThrowIOException_ForDirectoryWithNoFiles()
        {
            var emptyDirectory = Path.Combine(_testDirectory, "empty");
            Directory.CreateDirectory(emptyDirectory);
            Assert.Throws<IOException>(() => _fileReader.ReadFiles(emptyDirectory));
        }
    }
    [TestFixture]
    public class ProgramTests
    {
        private Program _program;
        private string _testFolderPath;

        [SetUp]
        public void Setup()
        {
            _program = new Program(new FileReader(), new FreqPrinter());
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            string projectDirectory = "";
            while (directoryInfo != null && directoryInfo.Name != "WordFreqProgram.Tests")
            {
                directoryInfo = directoryInfo.Parent;
                projectDirectory = directoryInfo?.FullName ?? throw new InvalidOperationException("Project directory not found.");
            }
            _testFolderPath = Path.Combine(projectDirectory, "TestFolder");

            if (!Directory.Exists(_testFolderPath))
                throw new InvalidOperationException($"The folder '{Path.GetFullPath(_testFolderPath)}' does not exist in the current directory '{Directory.GetCurrentDirectory()}'.");
        }

        [Test]
        public void ShouldReturnAccurateWordFrequencies()
        {
            var input = _program.Run(_testFolderPath, false);
            var expected = GetExpectedResults();

            Assert.That(input, Is.EqualTo(expected));
        }

        private Dictionary<string, int> GetExpectedResults()
        {
            return new Dictionary<string, int>(StringComparer.Ordinal)
    {
        {"quis", 260},
        {"in", 260},
        {"et", 260},
        {"vestibulum", 200},
        {"ut", 200},
        {"sed", 200},
        {"posuere", 200},
        {"phasellus", 200},
        {"nunc", 200},
        {"nec", 180},
        {"eget", 180},
        {"aenean", 180},
        {"ac", 180},
        {"turpis", 160},
        {"pellentesque", 160},
        {"ipsum", 160},
        {"imperdiet", 160},
        {"a", 160},
        {"orci", 140},
        {"libero", 140},
        {"donec", 140},
        {"ante", 140},
        {"sit", 120},
        {"sem", 120},
        {"pretium", 120},
        {"pede", 120},
        {"nullam", 120},
        {"nisi", 120},
        {"dolor", 120},
        {"amet", 120},
        {"vitae", 100},
        {"velit", 100},
        {"ultricies", 100},
        {"tincidunt", 100},
        {"non", 100},
        {"mi", 100},
        {"leo", 100},
        {"justo", 100},
        {"id", 100},
        {"fringilla", 100},
        {"felis", 100},
        {"faucibus", 100},
        {"eu", 100},
        {"dui", 100},
        {"augue", 100},
        {"arcu", 100},
        {"vulputate", 80},
        {"vel", 80},
        {"ultrices", 80},
        {"tortor", 80},
        {"tellus", 80},
        {"sapien", 80},
        {"sagittis", 80},
        {"rhoncus", 80},
        {"quam", 80},
        {"neque", 80},
        {"mollis", 80},
        {"metus", 80},
        {"malesuada", 80},
        {"magna", 80},
        {"maecenas", 80},
        {"luctus", 80},
        {"lorem", 80},
        {"hendrerit", 80},
        {"feugiat", 80},
        {"etiam", 80},
        {"enim", 80},
        {"elit", 80},
        {"eleifend", 80},
        {"cursus", 80},
        {"curabitur", 80},
        {"consectetuer", 80},
        {"auctor", 80},
        {"volutpat", 60},
        {"viverra", 60},
        {"venenatis", 60},
        {"ullamcorper", 60},
        {"tristique", 60},
        {"tempus", 60},
        {"sodales", 60},
        {"semper", 60},
        {"primis", 60},
        {"praesent", 60},
        {"nulla", 60},
        {"nonummy", 60},
        {"nam", 60},
        {"morbi", 60},
        {"mauris", 60},
        {"massa", 60},
        {"ligula", 60},
        {"euismod", 60},
        {"eros", 60},
        {"curae", 60},
        {"cubilia", 60},
        {"cras", 60},
        {"consequat", 60},
        {"aliquam", 60},
        {"adipiscing", 60},
        {"accumsan", 60},
        {"vivamus", 40},
        {"varius", 40},
        {"urna", 40},
        {"suscipit", 40},
        {"senectus", 40},
        {"rutrum", 40},
        {"purus", 40},
        {"pulvinar", 40},
        {"porttitor", 40},
        {"nibh", 40},
        {"netus", 40},
        {"mattis", 40},
        {"lectus", 40},
        {"lacus", 40},
        {"integer", 40},
        {"habitant", 40},
        {"gravida", 40},
        {"fusce", 40},
        {"fames", 40},
        {"erat", 40},
        {"egestas", 40},
        {"duis", 40},
        {"dapibus", 40},
        {"congue", 40},
        {"condimentum", 40},
        {"blandit", 40},
        {"bibendum", 40},
        {"at", 40},
        {"tempor", 20},
        {"suspendisse", 20},
        {"sollicitudin", 20},
        {"sociis", 20},
        {"scelerisque", 20},
        {"risus", 20},
        {"ridiculus", 20},
        {"quisque", 20},
        {"proin", 20},
        {"porta", 20},
        {"platea", 20},
        {"placerat", 20},
        {"penatibus", 20},
        {"parturient", 20},
        {"ornare", 20},
        {"odio", 20},
        {"nisl", 20},
        {"natoque", 20},
        {"nascetur", 20},
        {"mus", 20},
        {"montes", 20},
        {"magnis", 20},
        {"lobortis", 20},
        {"laoreet", 20},
        {"lacinia", 20},
        {"iaculis", 20},
        {"hac", 20},
        {"habitasse", 20},
        {"facilisis", 20},
        {"est", 20},
        {"elementum", 20},
        {"dis", 20},
        {"dictumst", 20},
        {"dictum", 20},
        {"diam", 20},
        {"cum", 20},
        {"commodo", 20},
        {"aliquet", 20}
    };
        }

    }

}
