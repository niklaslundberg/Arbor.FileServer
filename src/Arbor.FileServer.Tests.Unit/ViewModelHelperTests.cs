using System;
using System.IO;
using System.Linq;
using Arbor.FileServer.Hashing;
using Arbor.FileServer.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.FileServer.Tests.Unit
{
    public class ViewModelHelperTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ViewModelHelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("http://localhost", @"C:\temp", @"a", "a")]
        [InlineData("http://localhost", @"C:\temp", @"A", "A")]
        [InlineData("http://localhost", @"C:\temp", @"a.txt", "a.txt")]
        [InlineData("http://localhost", @"C:\temp", @"a/b", "a/b")]
        [InlineData("http://localhost", @"C:\temp", @"a/b\c", "a/b/c")]
        [InlineData("http://localhost", @"C:\temp", @"a\b\c.txt", "a/b/c.txt")]
        [InlineData("http://localhost", @"C:\temp\", @"a\b\c.txt", "a/b/c.txt")]
        [InlineData("http://localhost/", @"C:\temp", @"a\b\c.txt", "a/b/c.txt")]
        [InlineData("http://localhost/", @"C:\temp\", @"a\b\c.txt", "a/b/c.txt")]
        public void Test(string host, string baseDirectory, string fileName, string expected)
        {
            var directoryFiles = new[] {new FileInfo(Path.Combine(baseDirectory.EnsureEndsWith("\\"), fileName)) };

            foreach (FileInfo directoryFile in directoryFiles)
            {
                _testOutputHelper.WriteLine(directoryFile.FullName);
            }

            FilesViewModel filesViewModel = new FileServerSettings(baseDirectory,
                host).CreateViewModel(directoryFiles);

            foreach (FileGroup s in filesViewModel.Files)
            {
                _testOutputHelper.WriteLine(s.MainFileAbsolutePath);
                _testOutputHelper.WriteLine(s.MainFileRelativePath);
            }

            Assert.Equal($"http://localhost/{expected}",filesViewModel.Files[0].MainFileAbsolutePath);
            Assert.Equal(expected, filesViewModel.Files[0].MainFileRelativePath);
        }
    }
}
