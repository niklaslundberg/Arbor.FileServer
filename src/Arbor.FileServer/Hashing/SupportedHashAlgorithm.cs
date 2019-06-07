using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Arbor.FileServer.Hashing
{
    public struct SupportedHashAlgorithm
    {
        public int? Order { get; }

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

        private static readonly Lazy<ImmutableArray<SupportedHashAlgorithm>> _all = new Lazy<ImmutableArray<SupportedHashAlgorithm>>(GetAll);

        private static ImmutableArray<SupportedHashAlgorithm> GetAll()
        {
            Type type = typeof(SupportedHashAlgorithm);
            return type.GetFields()
                .Where(field =>
                    field.FieldType == type && field.IsInitOnly && field.IsPublic && field.IsStatic)
                .Select(field => field.GetValue(null)).OfType<SupportedHashAlgorithm>()
                .Where(s => s.Order.HasValue)
                .OrderBy(s => s.Order)
                .ToImmutableArray();
        }

        public static ImmutableArray<SupportedHashAlgorithm> All => _all.Value;

        private readonly Func<Stream, byte[]> _hasher;
        public string AlgorithmName { get; }
        public string FileExtension { get; }
        public bool IsEmpty => _hasher is null;

        public static readonly SupportedHashAlgorithm Sha512 =
            new SupportedHashAlgorithm(nameof(Sha512), ".sha512", Sha512Hasher.Hash, 1);

        public static readonly SupportedHashAlgorithm Undefined =
            new SupportedHashAlgorithm(nameof(Undefined), null, null);

        public static readonly SupportedHashAlgorithm Sha256 =
            new SupportedHashAlgorithm(nameof(Sha256), ".sha256", Sha256Hasher.Hash, 2);

        public static readonly SupportedHashAlgorithm Sha1 =
            new SupportedHashAlgorithm(nameof(Sha1), ".sha1", Sha1Hasher.Hash, 3);

        public static readonly SupportedHashAlgorithm Md5 =
            new SupportedHashAlgorithm(nameof(Md5), ".md5", Md5Hasher.Hash, 4);

        private SupportedHashAlgorithm(
            string algorithmName,
            string fileExtension,
            Func<Stream, byte[]> hasher,
            int? order = null)
        {
            Order = order;
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
