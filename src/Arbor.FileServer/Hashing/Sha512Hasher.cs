using System.IO;
using System.Security.Cryptography;

namespace Arbor.FileServer.Hashing
{
    public class Sha512Hasher
    {
        public static byte[] Hash(Stream stream)
        {
            byte[] hashBytes;
            using (SHA512 hasher = SHA512.Create())
            {
                hashBytes = hasher.ComputeHash(stream);
            }

            return hashBytes;
        }
    }
}
