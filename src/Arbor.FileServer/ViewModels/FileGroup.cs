using System;
using System.Collections.Immutable;

namespace Arbor.FileServer.ViewModels
{
    public class FileGroup
    {
        public FileGroup(
            string mainFileAbsolutePath,
            string mainFileRelativePath,
            DateTime? lastModifiedUtc,
            ImmutableArray<HashFile> hashFiles)
        {
            MainFileAbsolutePath = mainFileAbsolutePath;
            MainFileRelativePath = mainFileRelativePath;
            LastModifiedUtc = lastModifiedUtc;
            HashFiles = hashFiles;
        }

        public string MainFileAbsolutePath { get; }

        public string MainFileRelativePath { get; }

        public DateTime? LastModifiedUtc { get; }

        public ImmutableArray<HashFile> HashFiles { get; }
    }
}
