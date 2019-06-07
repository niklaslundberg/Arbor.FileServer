using System.IO;
using System.Security.Cryptography;

namespace Arbor.FileServer.Hashing
{
    public class Sha1Hasher
    {
        public static byte[] Hash(Stream stream)
        {
            byte[] hashBytes;
            using (SHA1 hasher = SHA1.Create())
            {
                hashBytes = hasher.ComputeHash(stream);
            }

            return hashBytes;
        }
    }
}