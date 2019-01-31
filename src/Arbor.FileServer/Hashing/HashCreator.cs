using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arbor.FileServer.Hashing
{
    public class HashCreator
    {
        public static bool IsHashFile(FileInfo fileInfo, ImmutableArray<SupportedHashAlgorithm> supportedHashAlgorithms)
        {
            if (!Path.HasExtension(fileInfo.Name))
            {
                return false;
            }

            return supportedHashAlgorithms.Any(algorithm =>
                Path.GetExtension(fileInfo.FullName)
                    .Equals(algorithm.FileExtension, StringComparison.OrdinalIgnoreCase));
        }

        public static void CleanFiles(DirectoryInfo directoryInfo)
        {
            directoryInfo.Refresh();

            ImmutableArray<FileInfo> hashesWithoutFiles = directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                .Where(file => IsHashFile(file, SupportedHashAlgorithm.All)
                               && !File.Exists(Path.Combine(file.Directory.FullName,
                                   Path.GetFileNameWithoutExtension(file.Name))))
                .ToImmutableArray();

            foreach (FileInfo hasFile in hashesWithoutFiles)
            {
                try
                {
                    hasFile.Refresh();
                    if (hasFile.Exists)
                    {
                        hasFile.Delete();
                    }
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }

        public static ImmutableArray<string> RemoveAllHashFiles(DirectoryInfo filesDirectory)
        {
            filesDirectory.Refresh();

            List<string> removed = new List<string>();

            ImmutableArray<FileInfo> hashFilesToRemove = filesDirectory.GetFiles("*", SearchOption.AllDirectories)
                .Where(file => SupportedHashAlgorithm.All.Any(algorithm =>
                    Path.GetExtension(file.Name).Equals(algorithm.FileExtension, StringComparison.OrdinalIgnoreCase)))
                .ToImmutableArray();

            foreach (FileInfo fileInfo in hashFilesToRemove)
            {
                fileInfo.Refresh();

                if (fileInfo.Exists)
                {
                    try
                    {
                        fileInfo.Delete();
                        removed.Add(fileInfo.FullName.Substring(filesDirectory.FullName.Length).Replace("\\", "/"));
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }

            return removed.ToImmutableArray();
        }


        public static ImmutableArray<string> HashFiles(DirectoryInfo directoryInfo,
            ImmutableArray<SupportedHashAlgorithm> supportedHashAlgorithms)
        {
            List<string> hashFiles = new List<string>();

            foreach (SupportedHashAlgorithm hashAlgorithm in supportedHashAlgorithms)
            {
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                    .Where(file => !IsHashFile(file, supportedHashAlgorithms)))
                {
                    string hashFile = fileInfo.FullName + hashAlgorithm.FileExtension;

                    if (!File.Exists(hashFile))
                    {
                        string createdFile = CreateHashFile(fileInfo.FullName, hashAlgorithm);

                        if (!string.IsNullOrWhiteSpace(createdFile))
                        {
                            hashFiles.Add(createdFile.Substring(directoryInfo.FullName.Length).Replace("\\", "/"));
                        }
                    }
                }
            }

            return hashFiles.ToImmutableArray();
        }

        public static string CreateHashFile(string fullPath, SupportedHashAlgorithm supportedHashAlgorithm)
        {
            int attempt = 0;
            int maxAttempts = 5;

            while (attempt < maxAttempts)
            {
                try
                {
                    string hashFilePath = fullPath + supportedHashAlgorithm.FileExtension;

                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] hashBytes = supportedHashAlgorithm.ComputeHash(fs);

                        string content = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant() + "  " +
                                         Path.GetFileName(fullPath);

                        File.WriteAllText(hashFilePath, content, Encoding.UTF8);
                    }

                    return hashFilePath;
                }
                catch (Exception)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    attempt++;
                }
            }

            return null;
        }
    }
}