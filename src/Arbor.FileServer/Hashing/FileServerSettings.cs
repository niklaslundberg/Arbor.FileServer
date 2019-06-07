using System;
using System.IO;
using Arbor.KVConfiguration.Urns;

namespace Arbor.FileServer.Hashing
{
    [Urn(Urn)]
    public class FileServerSettings
    {
        public const string Urn = "urn:arbor-file-server:file-settings";

        public FileServerSettings(
            string basePath,
            string baseUrl,
            bool cleanEnabled = false,
            bool settingsDiagnosticsEnabled = false)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(basePath));
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new InvalidOperationException("The base path has not been set");
            }

            CleanEnabled = cleanEnabled;
            SettingsDiagnosticsEnabled = settingsDiagnosticsEnabled;

            BasePath = basePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            BaseUrl = baseUrl.TrimEnd('/') + "/";
        }

        public bool CleanEnabled { get; }
        public bool SettingsDiagnosticsEnabled { get; }

        public string BasePath { get; }

        public string BaseUrl { get; }
    }
}
