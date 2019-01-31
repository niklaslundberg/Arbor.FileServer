using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Arbor.FileServer.Hashing
{
    public class FileWatcherService : BackgroundService
    {
        private readonly HashWatcher _hashWatcher;
        private readonly DirectoryInfo _directoryInfo;

        public FileWatcherService(HashWatcher hashWatcher, FileServerSettings settings)
        {
            _hashWatcher = hashWatcher;
            _directoryInfo = new DirectoryInfo(settings.BasePath);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HashCreator.RemoveAllHashFiles(_directoryInfo);

            _hashWatcher.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                _directoryInfo.Refresh();

                HashCreator.CleanFiles(_directoryInfo);

                HashCreator.HashFiles(_directoryInfo, SupportedHashAlgorithm.All);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _hashWatcher.Stop();
        }
    }
}