using System;
using System.IO;
using System.Linq;

namespace Arbor.FileServer.Hashing
{
    public class HashWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;

        public HashWatcher(FileServerSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.BasePath) && Directory.Exists(settings.BasePath))
            {
                _watcher = new FileSystemWatcher(settings.BasePath) { IncludeSubdirectories = true };

                _watcher.Changed += FileWatcherEventHandler;
                _watcher.Deleted += FileWatcherEventHandler;
                _watcher.Created += FileWatcherEventHandler;
                _watcher.Renamed += FileWatcherEventHandler;
            }
        }

        public void Start()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }
        }

        private void FileWatcherEventHandler(object sender, FileSystemEventArgs e)
        {
            if (SupportedHashAlgorithm.All.Any(supportedHashAlgorithm =>
                e.FullPath.EndsWith(supportedHashAlgorithm.FileExtension)))
            {
                return;
            }

            foreach (SupportedHashAlgorithm supportedHashAlgorithm in SupportedHashAlgorithm.All)
            {
                string shaFile = e.FullPath + supportedHashAlgorithm.FileExtension;

                if (File.Exists(shaFile))
                {
                    File.Delete(shaFile);
                }

                if (File.Exists(e.FullPath))
                {
                    HashCreator.CreateHashFile(e.FullPath, supportedHashAlgorithm);
                }
            }
        }

        public void Dispose()
        {
            if (_watcher is null)
            {
                return;
            }

            if (_watcher.EnableRaisingEvents)
            {
                _watcher.Changed -= FileWatcherEventHandler;
                _watcher.Deleted -= FileWatcherEventHandler;
                _watcher.Created -= FileWatcherEventHandler;
                _watcher.Renamed -= FileWatcherEventHandler;
                _watcher.EnableRaisingEvents = false;
            }

            _watcher?.Dispose();

            _watcher = null;
        }
    }
}