using System.IO;
using System.Security.Cryptography;

namespace Arbor.FileServer.Hashing
{
    public class Md5Hasher
    {
        public static byte[] Hash(Stream stream)
        {
            byte[] hashBytes;
            using (MD5 hasher = MD5.Create())
            {
                hashBytes = hasher.ComputeHash(stream);
            }

            return hashBytes;
        }
    }
}