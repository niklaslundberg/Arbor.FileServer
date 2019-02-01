using System;
using Arbor.FileServer.Hashing;

namespace Arbor.FileServer.ViewModels
{
    public class HashFile
    {
        public HashFile(
            string file,
            string relativePath,
            DateTime lastModifiedUtc,
            SupportedHashAlgorithm hashAlgorithm)
        {
            File = file;
            RelativePath = relativePath;
            LastModifiedUtc = lastModifiedUtc;
            HashAlgorithm = hashAlgorithm;
        }

        public string File { get; }

        public string RelativePath { get; }

        public DateTime LastModifiedUtc { get; }

        public SupportedHashAlgorithm HashAlgorithm { get; }
    }
}
