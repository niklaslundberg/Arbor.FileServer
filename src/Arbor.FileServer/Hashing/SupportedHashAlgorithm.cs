using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Arbor.FileServer.Hashing
{
    public struct SupportedHashAlgorithm
    {
        public bool Equals(SupportedHashAlgorithm other)
        {
            return string.Equals(AlgorithmName, other.AlgorithmName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SupportedHashAlgorithm other && Equals(other);
        }

        public override int GetHashCode()
        {
            return AlgorithmName.GetHashCode();
        }

        public static bool operator ==(SupportedHashAlgorithm left, SupportedHashAlgorithm right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SupportedHashAlgorithm left, SupportedHashAlgorithm right)
        {
            return !left.Equals(right);
        }

        public static bool TryParse(string value, out SupportedHashAlgorithm supportedHashAlgorithm)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                supportedHashAlgorithm = Undefined;
                return false;
            }

            SupportedHashAlgorithm[] found = All.Where(algorithm =>
                algorithm.AlgorithmName.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();

            if (!found.Any())
            {
                supportedHashAlgorithm = Undefined;
                return false;
            }

            supportedHashAlgorithm = found[0];
            return true;
        }

        public static ImmutableArray<SupportedHashAlgorithm> All =>
            new[] { Sha256, Sha512 }.ToImmutableArray();

        private readonly Func<Stream, byte[]> _hasher;
        public string AlgorithmName { get; }
        public string FileExtension { get; }
        public bool IsEmpty => _hasher is null;

        public static readonly SupportedHashAlgorithm Sha512 =
            new SupportedHashAlgorithm(nameof(Sha512), ".sha512", Sha512Hasher.Hash);

        public static readonly SupportedHashAlgorithm Undefined =
            new SupportedHashAlgorithm(nameof(Undefined), null, null);

        public static readonly SupportedHashAlgorithm Sha256 =
            new SupportedHashAlgorithm(nameof(Sha256), ".sha256", Sha256Hasher.Hash);

        private SupportedHashAlgorithm(string algorithmName, string fileExtension, Func<Stream, byte[]> hasher)
        {
            _hasher = hasher;
            AlgorithmName = algorithmName?.ToUpperInvariant();
            FileExtension = fileExtension?.ToLowerInvariant();
        }

        public byte[] ComputeHash(Stream stream)
        {
            return _hasher.Invoke(stream);
        }
    }
}