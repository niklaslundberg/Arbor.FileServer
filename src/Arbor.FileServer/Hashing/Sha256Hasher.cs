using System.IO;
using System.Security.Cryptography;

namespace Arbor.FileServer.Hashing
{
    public class Sha256Hasher
    {
        public static byte[] Hash(Stream stream)
        {
            byte[] hashBytes;
            using (SHA256 hasher = SHA256.Create())
            {
                hashBytes = hasher.ComputeHash(stream);
            }

            return hashBytes;
        }
    }
}
