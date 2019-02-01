using System;
using System.Collections.Immutable;

namespace Arbor.FileServer.ViewModels
{
    public class FileGroup
    {
        public FileGroup(
            string mainFile,
            string mainFileRelative,
            DateTime lastModifiedUtc,
            ImmutableArray<HashFile> hashFiles)
        {
            MainFile = mainFile;
            MainFileRelative = mainFileRelative;
            LastModifiedUtc = lastModifiedUtc;
            HashFiles = hashFiles;
        }

        public string MainFile { get; }

        public string MainFileRelative { get; }

        public DateTime LastModifiedUtc { get; }

        public ImmutableArray<HashFile> HashFiles { get; }
    }
}
