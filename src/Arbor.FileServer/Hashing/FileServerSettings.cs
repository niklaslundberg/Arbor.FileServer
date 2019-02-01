using System;

namespace Arbor.FileServer.Hashing
{
    public class FileServerSettings
    {
        public FileServerSettings(string basePath, SupportedHashAlgorithm algorithm, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(basePath));
            }

            if (algorithm.IsEmpty)
            {
                throw new ArgumentException("Undefined hash algorithm.", nameof(algorithm));
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));
            }

            BasePath = basePath;
            Algorithm = algorithm;
            BaseUrl = baseUrl.TrimEnd('/');
        }

        public string BasePath { get; }

        public SupportedHashAlgorithm Algorithm { get; }

        public string BaseUrl { get; }
    }
}
