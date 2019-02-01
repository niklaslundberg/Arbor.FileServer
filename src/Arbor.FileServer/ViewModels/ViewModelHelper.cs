using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Arbor.FileServer.Hashing;

namespace Arbor.FileServer.ViewModels
{
    public static class ViewModelHelper
    {
        public static FilesViewModel CreateViewModel(this FileServerSettings fileServerSettings)
        {
            string GetMainFile(FileInfo file)
            {
                if (!Path.HasExtension(file.Name))
                {
                    return file.FullName;
                }

                return HashCreator.IsHashFile(file, SupportedHashAlgorithm.All)
                    ? Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(file.Name))
                    : file.FullName;
            }

            string AbsolutePath(string file)
            {
                return fileServerSettings.BaseUrl +
                       file.Substring(fileServerSettings.BasePath.Length).Replace("\\", "/");
            }

            string RelativePath(string file)
            {
                return file.Substring(fileServerSettings.BaseUrl.Length);
            }

            SupportedHashAlgorithm Parse(string file)
            {
                if (!Path.HasExtension(file))
                {
                    return SupportedHashAlgorithm.Undefined;
                }

                ImmutableArray<SupportedHashAlgorithm> supportedHashAlgorithms = SupportedHashAlgorithm.All.Where(
                        supportedHashAlgorithm =>
                            supportedHashAlgorithm.FileExtension.Equals(Path.GetExtension(file),
                                StringComparison.OrdinalIgnoreCase))
                    .ToImmutableArray();

                if (supportedHashAlgorithms.Length == 0)
                {
                    return SupportedHashAlgorithm.Undefined;
                }

                return supportedHashAlgorithms[0];
            }

            var directoryInfo = new DirectoryInfo(fileServerSettings.BasePath);
            ImmutableArray<FileGroup> files = directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                .Select(file => (File: file, MainFile: GetMainFile(file), LastModified: file.LastWriteTimeUtc))
                .Select(file => (File: AbsolutePath(file.File.FullName), MainFile: AbsolutePath(file.MainFile),
                    LastModified: file.LastModified))
                .OrderBy(file => file.File)
                .GroupBy(group => group.MainFile)
                .Select(group => new FileGroup(group.Key,
                    RelativePath(group.Key),
                    group.First(file => file.MainFile == file.File).LastModified,
                    group.Select(file =>
                            new HashFile(file.File, RelativePath(file.File), file.LastModified, Parse(file.File)))
                        .Where(hashFile => hashFile.HashAlgorithm != SupportedHashAlgorithm.Undefined)
                        .ToImmutableArray()))
                .ToImmutableArray();

            var filesViewModel = new FilesViewModel(files);

            return filesViewModel;
        }
    }
}
